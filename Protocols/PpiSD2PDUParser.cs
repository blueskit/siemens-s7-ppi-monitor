using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Xml.Linq;

using DotNetty.Buffers;

using S7PpiMonitor.Utilities;

namespace S7PpiMonitor.Protocols;

/// <summary>
/// </summary>
public partial class PpiSD2PDU
{
    /// <summary>
    /// 以 PDU_REF 为Key，保存对应的请求命令变量列表
    /// </summary>
    internal static ConcurrentDictionary<int, List<PpiVarElement>> _pduRefMap = new();

    /// <summary>
    /// 解析SD2的PDU字节数组 _rawData 到各个字段
    /// </summary>
    /// <returns></returns>
    public bool TryParse()
    {
        var buff = Unpooled.WrappedBuffer(_rawData);

        // 从PDU开始解析
        var protoId = buff.ReadByte(); // 0x32  
        this.RosCTR = (PpiRosCtr)buff.ReadByte();

        // 跳过冗余识别/调用标识
        buff.SkipBytes(2);

        this.PduRef = buff.ReadUnsignedShortLE();

        var paramLen = buff.ReadUnsignedShortLE();
        var dataLen = buff.ReadUnsignedShortLE();

        var errClass = (byte)0;
        var errCode = (byte)0;

        // 读或写的响应，才有错误类型和错误代码
        if ((RosCTR == PpiRosCtr.Response || RosCTR == PpiRosCtr.ResponseEx) && paramLen >= 2) {
            errClass = buff.ReadByte();
            errCode = buff.ReadByte();
        }

        // 获取serviceId，但不移动readerIndex
        this.ServiceId = (PpiServiceId)buff.GetByte(buff.ReaderIndex);    // 4读;5写

        // 下面需根据帧类型，进行不同的解析
        // 1、读请求
        if (RosCTR == PpiRosCtr.Request && ServiceId == PpiServiceId.Read) {
            parseReadRequest(buff);
        }
        // 2、读反馈
        if (RosCTR == PpiRosCtr.Response && ServiceId == PpiServiceId.Read) {
            parseReadResponse(buff);
        }
        // 3、写请求
        if (RosCTR == PpiRosCtr.Request && ServiceId == PpiServiceId.Write) {
            parseWriteRequest(buff);
        }
        // 4、写反馈
        if (RosCTR == PpiRosCtr.Response && ServiceId == PpiServiceId.Write) {
            parseWriteResponse(buff);
        }

        return true;
    }

    /// <summary>
    /// PDU-读请求，从serviceId开始解析
    /// PDU请求读格式(Byte7~30或更长)
    ///     PROTO_ID   1B,协议标识 总是 0x32
    ///     ROSCTR     1B,远程控制。01读写请求; 02无字段相应; 03读写响应; 07异常响应
    ///     RED_ID     2B,冗余识别/调用标识,
    ///     PDU_REF    2B, 协议数据单元参考
    ///     PARM_LEN   2B, 后续参数字节长度()
    ///     DATA_LEN   2B, 后续数据字节长度。读命令总是0，写命令根据数据长度变化
    ///     VARS_NUM   1B, 变量存储区块数量,通常为1
    ///     VAR_SPEC   1B, 变量规格,读写通常为0x12
    ///     VADR_LEN   1B, 变量地址长度,通常为0x0A
    ///     SYNTAX_LEN 1B, 语法类型长度,通常为0x10
    ///     VAR_TYPE   1B, 变量类型（PpiVarType)
    ///     ITEM_CNT   2B, 元件/变量个数, BOOL总是占一个字节，其它类型占1~4个字节
    ///     SUBAREA    2B, 子区域,V区域为0x0001,其它为0x0000
    ///     AREATYPE   1B, 存储器类型(PpiArea) 04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T
    ///     OFFSET     3B, 偏移量,起始字节地址*8, 高字节在前
    /// </summary>
    private List<PpiVarElement> parseReadRequest(IByteBuffer buff)
    {
        this.Elements = new List<PpiVarElement>();

        var serviceId = (PpiServiceId)buff.ReadByte();    // 4读;5写
        var varsNum = buff.ReadByte();      // 变量存储区块数量,通常为1

        // 按变量区块数量，循环读取
        for (var i = 0; i < varsNum; i++) {
            var varspec = buff.ReadByte();              // 变量规格,读写通常为0x12
            var varAddrLen = buff.ReadByte();           // 变量地址长度,通常为0x0A
            var syntaxLen = buff.ReadByte();            // 语法类型长度,通常为0x10
            var varType = (PpiVarType)buff.ReadByte();  // 变量类型（PpiVarType)
            var itemCnt = buff.ReadUnsignedShort();     // 元件/变量个数, BOOL总是占一个字节，其它类型占1~4个字节
            var subArea = buff.ReadUnsignedShort();     // 子区域,V区域为0x0001,其它为0x0000 
            var areaType = (PpiArea)buff.ReadByte();    // 存储器类型(PpiArea) 04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T   
            var offset = (buff.ReadByte() << 16) | (buff.ReadByte() << 8) | buff.ReadByte(); // 偏移量,起始字节地址*8, 高字节在前

            var item = new PpiVarElement() {
                Type = varType,
                Area = areaType,
                Offset = offset,
                Count = (int)itemCnt,
                Values = new List<int>()
            };

            // 更新Bit类型的Type值
            var bitAreaList = new List<PpiArea>() {
                PpiArea.SystemMarker,
                PpiArea.Input ,
                PpiArea.Output,
                PpiArea.Marker
            };
            if (bitAreaList.Contains(areaType))
                item.Type = PpiVarType.Bit;

            this.Elements.Add(item);
        }

        _pduRefMap[this.PduRef] = this.Elements;

        return this.Elements;
    }

    /// <summary>
    /// PDU-读反馈，从serviceId开始解析
    /// 1、利用PduRef查询对应的读命令，获取变量类型和数量
    /// 2、将读取到的值更新到 varElements 即可
    /// 2、PDU读响应格式(Byte7~30或更长)
    ///     PROTO_ID   1B,协议标识 总是 0x32
    ///     ROSCTR     1B,远程控制。01读写请求; 02无字段相应; 03读写响应; 07异常响应
    ///     RED_ID     2B,冗余识别/调用标识,
    ///     PDU_REF    2B, 协议数据单元参考
    ///     PARM_LEN   2B, 后续参数字节长度()
    ///     DATA_LEN   2B, 后续数据字节长度。读命令总是0，写命令根据数据长度变化
    ///     ERR_CLASS  1B, 错误类型
    ///     ERR_CODE   1B, 错误代码
    ///                 80 0l:  Switch in wrong position for requested operation
    ///                 81 04:  Miscellaneous structure error in command.Command is not supported by CPU
    ///                 84 04:  CPU is busy processing an upload or downloadCPU cannot process command because of System Fault condition85 00: Length fields are not correct or do not agree with the amountof data received
    ///                 D2 xx:  Error in upload or download command
    ///                 D6 xx:  Protection error(password)
    ///                 DC 01:  Error in Time-of-Day clock data
    ///     *SERVICE_ID  1B, 服务代码,读0x04,写0x05
    ///     ITEM_CNT   1B, 元件/变量个数, 指变量地址数
    ///     AccessResult 1B, 变量访问结果, 00=成功,其它=失败
    ///                 FFH No error
    ///                 01H Hardware fault
    ///                 03H Illegal object access
    ///                 05H Invalid address(incorrect variable address
    ///                 06H Data type is not supported
    ///                 OAH Object does not exist or length error
    ///                 00H Returned if access result indicates an error
    ///                 03H Returned for bit access
    ///                 04H Returned for byte, word, double word, etc.acc
    ///      VarValue   6B*N, 变量值
    ///                 根据变量类型不同，长度不同，从第一个字节开始
    /// </summary>
    private List<PpiVarElement> parseReadResponse(IByteBuffer buff)
    {
#if DEBUG
        var idx0 = buff.ReaderIndex;
        byte[] buff0 = new byte[buff.ReadableBytes];
        buff.GetBytes(idx0, buff0);
#endif

        var varElements = new List<PpiVarElement>();

        var serviceId = (PpiServiceId)buff.ReadByte();    // 4读;5写
        var varsNum = buff.ReadByte();      // 变量存储区块数量,通常为1

        // 从帧中获取所有变量


        // 如果没有对应的请求命令，则不解析对应反馈
        if (!_pduRefMap.TryGetValue(this.PduRef, out varElements))
            return new List<PpiVarElement>();

        if (varElements is null)
            return new List<PpiVarElement>();

        this.Elements = varElements;

        Debug.Assert(varElements.Count == varsNum, "反馈变量区块数量与请求不符");

        // 按变量区块数量，循环读取
        for (int varIdx = 0; varIdx < varsNum; varIdx++) {
            var element = varElements[varIdx];
            element.Values = new List<int>();

            byte varspec = 0;      // 变量规格,0xFF表示数据正常合法
            byte varDataLen = 0;   // 变量数据长度,通常为0x04

            // 如果变量不合法(跳过4个字节）、一个变量占位
            while (buff.ReadableBytes > 2) {
                varspec = buff.ReadByte();      // 变量规格,0xFF表示数据正常合法
                varDataLen = buff.ReadByte();   // 变量数据长度,通常为0x04

                if (varspec == 0xff)
                    break;
                else {
                    buff.SkipBytes(Math.Min(4, buff.ReadableBytes));
                }
            }

            if (buff.ReadableBytes == 0)
                break;

            switch (element.Type) {
            case PpiVarType.Bit:
                byte[] rawBitBytes = new byte[4];
                buff.ReadBytes(rawBitBytes, 0, Math.Min(rawBitBytes.Length, buff.ReadableBytes));
                int baseOffset = 0;
#if DEBUG
                var idx0name = element.GetVarName();
                if (idx0name.Contains("Q"))
                    Console.WriteLine(idx0name);
#endif

                if (element.Area == PpiArea.Output) {          // QX.X
                    baseOffset = element.Count switch {
                        > 24 => 0,
                        <= 16 => 16,
                        _ => 8
                    };
                } else if (element.Area == PpiArea.Marker) {   // MX.X
                    baseOffset = 16;
                } else if (element.Count < 4) {
                    baseOffset = 0;
                } else {
                    baseOffset = 0;
                    rawBitBytes = new byte[varDataLen];
                    buff.ReadBytes(rawBitBytes, 0, Math.Min(rawBitBytes.Length, buff.ReadableBytes));
                }

                var bits = new BitArray(rawBitBytes);
                var bitOffset = 0;
                for (int j = 0; j < element.Count; j++) {
                    var v = bits.Get(bitOffset++);
                    element.Values.Add(varspec == 0xff ? (v ? 1 : 0) : -1);
                }
                break;
            case PpiVarType.Byte:
#if DEBUG
                var idx1name = element.GetVarName(0);
                var idx1Byte = buff.ReaderIndex;
                var tmpBuff1 = new byte[2 + buff.ReadableBytes];
                buff.GetBytes(idx1Byte - 2, tmpBuff1, 0, buff.ReadableBytes + 2);

                if (idx1name.Contains("DBB886"))
                    Console.WriteLine(idx1name);
#endif

                if (element.Count < 4) {
                    if (element.Count < buff.ReadableBytes)
                        buff.SkipBytes(Math.Min(4 - element.Count, buff.ReadableBytes));
                } else {  // 此时根据下一个 FF 04 的位置决定指针是否跳过2Bytes
                    var buf4 = new byte[buff.ReadableBytes];
                    buff.GetBytes(buff.ReaderIndex, buf4);
                    if (buf4.Length == (element.Count + 2))
                        buff.SkipBytes(2);
                    else {
                        for (int k = 0; k < buf4.Length - 2; k++) {
                            if (buf4[k] == 0xff && buf4[k + 1] == 0x04) {
                                if ((k - element.Count) == 2)
                                    buff.SkipBytes(2);
                            }
                        }
                    }
                }

                var bytes = new byte[element.Count];
                for (int j = 0; j < element.Count && buff.ReadableBytes > 0; j++)
                    bytes[j] = buff.ReadByte();

                // 按 BYTE 构建
                for (int j = 0; j < element.Count; j++) {
                    element.Values.Add(bytes[j]);
                }

                // 按 WORD 构建
                for (int j = 0; j < element.Count - 1; j++) {
                    if ((j % 2) == 0) {
                        var v = bytes.PPI_GetShortBE(j);
                        element.WordValues.Add(v);
                    } else {
                        element.WordValues.Add(0);
                    }
                }

                // 按 DWORD/REAL 构建
                for (int j = 0; j < element.Count - 3; j++) {

#if DEBUG
                    var idx4name = element.GetVarName(j);
                    if (idx4name.Contains("300"))
                        Console.WriteLine(idx4name);
#endif

                    if ((j % 4) == 0) {
                        var v4ibuf = bytes.PPI_GetBytesBE(j, 4);
                        var v4i = BitConverter.ToInt32(v4ibuf, 0);
                        var v4f = BitConverter.ToSingle(v4ibuf, 0);

                        element.DWordValues.Add(v4i);
                        element.RealValues.Add(v4f);
                    } else {
                        element.DWordValues.Add(0);
                        element.RealValues.Add(0);
                    }
                }

                break;
            case PpiVarType.Word:
                for (int j = 0; j < element.Count; j++) {
                    var v = buff.ReadShort();
                    element.Values.Add(v);
                    element.WordValues.Add(v);
                }
                if (element.Count < 2)
                    buff.SkipBytes(2);
                break;
            case PpiVarType.DWord:
                for (int j = 0; j < Math.Max(1, element.Count); j++) {
                    var v = buff.ReadInt();
                    element.Values.Add(v);
                    element.DWordValues.Add(v);
                    element.RealValues.Add(Convert.ToSingle(v));
                }
                break;
            default:
                break;
            }
        }

        // _pduRefMap.TryRemove(this.PduRef, out var _);   

        return varElements;
    }

    /// <summary>
    /// PDU-写请求，从serviceId开始解析
    /// PDU请求写格式(Byte7~30或更长)
    ///     PROTO_ID   1B,协议标识 总是 0x32
    ///     ROSCTR     1B,远程控制。01读写请求; 02无字段相应; 03读写响应; 07异常响应
    ///     RED_ID     2B,冗余识别/调用标识,
    ///     PDU_REF    2B, 协议数据单元参考
    ///     PARM_LEN   2B, 后续参数字节长度()
    ///     DATA_LEN   2B, 后续数据字节长度。
    ///     SERVICE_ID  1B, 服务代码,写0x05
    ///     VARS_NUM   1B, 变量存储区块数量,通常为1
    ///     VAR_SPEC   1B, 变量规格,读写通常为0x12
    ///     VADR_LEN   1B, 变量地址长度,通常为0x0A
    ///     SYNTAX_LEN 1B, 语法类型长度,通常为0x10
    ///     VAR_TYPE   1B, 变量类型（PpiVarType)
    ///     ITEM_CNT   2B, 元件/变量个数, 写仅支持一个
    ///     SUBAREA    2B, 子区域,V区域为0x0001,其它为0x0000
    ///     AREATYPE   1B, 存储器类型(PpiArea) 04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T
    ///     OFFSET     3B, 偏移量,起始字节地址*8, 高字节在前
    ///     REV        1B, 保留, 固定为0x00
    ///     VarValue   4B, 变量值
    ///                 根据变量类型不同，长度不同，从第一个字节开始
    /// </summary>
    private List<PpiVarElement> parseWriteRequest(IByteBuffer buff)
    {
#if DEBUG
        var tmpBuff1 = new byte[buff.ReadableBytes];
        buff.GetBytes(buff.ReaderIndex, tmpBuff1);
#endif

        this.Elements = new List<PpiVarElement>();

        var serviceId = (PpiServiceId)buff.ReadByte();    // 4读;5写
        var varsNum = buff.ReadByte();      // 变量存储区块数量,通常为1

        // 按变量区块数量，循环读取
        for (var i = 0; i < varsNum; i++) {
            var varspec = buff.ReadByte();          // 变量规格,读写通常为0x12
            var varAddrLen = buff.ReadByte();       // 变量地址长度,通常为0x0A
            var syntaxLen = buff.ReadByte();        // 语法类型长度,通常为0x10
            var varType = (PpiVarType)buff.ReadByte(); // 变量类型（PpiVarType)
            var itemCnt = buff.ReadUnsignedShort(); // 元件/变量个数, BOOL总是占一个字节，其它类型占1~4个字节
            var subArea = buff.ReadUnsignedShort(); // 子区域,V区域为0x0001,其它为0x0000 
            var areaType = (PpiArea)buff.ReadByte();// 存储器类型(PpiArea) 04/S、05/M、06/AI、 07/AQ、 81:I、 82:Q、 83:M、84:V、 1F:T   
            var offset = (buff.ReadByte() << 16) | (buff.ReadByte() << 8) | buff.ReadByte(); // 偏移量,起始字节地址*8, 高字节在前

            buff.SkipBytes(1); // 保留1, 固定为0x00
            buff.SkipBytes(3); // 保留3, 内容含义待定

            var element = new PpiVarElement() {
                Type = varType,
                Area = areaType,
                Offset = offset,
                Count = (int)itemCnt,
                Values = new List<int>()
            };

            switch (element.Type) {
            case PpiVarType.Bit:
                var bitRawdata = buff.ReadByte();
                var bits = new BitArray(new byte[] { bitRawdata });
                for (int j = 0; j < element.Count; j++) {
                    var v = bits.Get(j) ? 1 : 0;
                    element.Values.Add(v);
                }
                break;
            case PpiVarType.Byte:   // 根据字节数，按不同格式处理
                switch (element.Count) {
                case 1:
                    var v1 = buff.ReadByte();
                    element.Values.Add(v1);
                    break;
                case 2:
                    var v2 = buff.ReadShort();
                    element.Values.Add(v2);
                    element.WordValues.Add(v2);
                    break;
                case 4:
                    var v4i = buff.ReadIntLE();
                    var v4ibuf = BitConverter.GetBytes(v4i);
                    Array.Reverse(v4ibuf);
                    var v4f = BitConverter.ToSingle(v4ibuf, 0);

                    element.Values.Add(v4i);
                    element.DWordValues.Add(v4i);
                    element.RealValues.Add(Convert.ToSingle(v4f));
                    break;
                default:
                    break;
                }

                break;
            case PpiVarType.Word:
                for (int j = 0; j < getMinMax(2, element.Count); j++) {
                    var v = buff.ReadShort();
                    element.Values.Add(v);
                }
                break;
            case PpiVarType.DWord:
                for (int j = 0; j < getMinMax(1, element.Count); j++) {
                    var v = buff.ReadInt();
                    element.Values.Add(v);
                }
                break;
            default:
                break;
            }

            Elements.Add(element);
        }

        _pduRefMap[this.PduRef] = Elements;

        return Elements;
    }

    /// <summary>
    /// 如果N2小于N1，返回N2，否则返回N1
    /// </summary>
    private int getMinMax(int n1, int n2)
    {
        if (n2 < n1)
            return n2;
        return n1;
    }

    /// <summary>
    /// PDU-写反馈，从serviceId开始解析
    /// 1、一般写反馈命令可以忽略
    /// 2、利用PduRef查询对应的写命令，获取变量类型和数量
    /// </summary>
    private List<PpiVarElement> parseWriteResponse(IByteBuffer buff)
    {
        var result = new List<PpiVarElement>();

        var serviceId = (PpiServiceId)buff.ReadByte();    // 4读;5写
        var varsNum = buff.ReadByte();      // 变量存储区块数量,通常为1

        // 如果没有对应的请求命令，则不解析对应反馈
        if (!_pduRefMap.TryGetValue(this.PduRef, out var varElements))
            return result;

        Debug.Assert(varElements.Count == varsNum, "反馈变量区块数量与请求不符");

        // 按变量区块数量，循环读取
        for (var i = 0; i < varsNum; i++) {
            var element = varElements[i];

            var successed = buff.ReadByte();    // 0xFF表示成功

            result.Add(element);
        }

        // _pduRefMap.TryRemove(this.PduRef, out var _);   

        return result;
    }


    /// <summary>
    /// 把 DU 区数据解析成期望的数组
    /// </summary>
    /// <param name="pdu">DU 区原始字节（不含头部）</param>
    /// <param name="unit">期望单位</param>
    /// <param name="count">元素个数</param>
    private object ParseDu(byte[] pdu, PpiVarType unit, int count)
    {
        switch (unit) {
        case PpiVarType.Bit: {
                var bits = new bool[count];
                for (int i = 0; i < count; i++) {
                    int byteIdx = i / 8;
                    int bitIdx = i % 8;
                    bits[i] = ((pdu[byteIdx] >> bitIdx) & 1) == 1;
                }
                return bits;
            }
        case PpiVarType.Byte: {
                var bytes = new byte[count];
                Array.Copy(pdu, 0, bytes, 0, count);
                return bytes;
            }
        case PpiVarType.Word: {
                var words = new ushort[count];
                for (int i = 0; i < count; i++)
                    words[i] = (ushort)(pdu[i * 2] << 8 | pdu[i * 2 + 1]);
                return words;
            }
        case PpiVarType.DWord: {
                var dwords = new uint[count];
                for (int i = 0; i < count; i++)
                    dwords[i] = (uint)(pdu[i * 4] << 24 | pdu[i * 4 + 1] << 16 |
                                        pdu[i * 4 + 2] << 8 | pdu[i * 4 + 3]);
                return dwords;
            }
        default:
            throw new NotSupportedException();
        }
    }
}
