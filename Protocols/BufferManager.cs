using System.Diagnostics;

namespace S7PpiMonitor.Protocols;

public class BufferManager
{
    private DotNetty.Buffers.IByteBuffer _buffer;

    public DotNetty.Buffers.IByteBuffer Buffer => _buffer;

    /// <summary>
    /// 上一个帧数据(用于判断下一个合法帧的附加判断)
    /// </summary>
    private MpiFrame _prevFrame;

    public BufferManager()
    {
        _prevFrame = null;
        _buffer = DotNetty.Buffers.Unpooled.Buffer(102400);
    }

    /// <summary>
    /// 将接收到的数据放入缓冲区
    /// </summary>
    public void WriteBytes(byte[] dataBuffer)
    {
        Debug.Assert(_buffer.IsWritable());
        Debug.Assert(dataBuffer != null && dataBuffer.Length > 0);

        _buffer.WriteBytes(dataBuffer);
    }

    /// <summary>
    /// 尝试从缓冲区获取一帧数据
    /// </summary>
    public bool TryGetFrame(out MpiFrame frame)
    {
        frame = null;

        // 没有数据可读
        if (_buffer.ReadableBytes < 1)
            return false;

        // 一次性读入所有数据
        byte[] dest = new byte[_buffer.ReadableBytes];
        _buffer.GetBytes(_buffer.ReaderIndex, dest);

        var header = (MpiFrameType)dest[0];

        // 检查是否 SC 帧
        if (header == MpiFrameType.SC && dest.Length >= 1) { // SC 帧
            frame = new MpiSCFrame(dest);
            _prevFrame = frame;

            _buffer.ReadByte(); // 移动读取指针
            return true;
        }

        // 检查是否 SD1 帧
        if (header == MpiFrameType.SD1 && dest.Length >= 6) { // SD1 帧
            frame = new MpiSD1Frame(dest);
            _prevFrame = frame;

            _buffer.ReadBytes(6); // 移动读取指针到下一帧起始位置
            return true;
        }

        // 检查是否 MPI/SD2 帧
        //if (header == MpiFrameType.SD2 && dest.Length >= 30) { // MPI/SD2
        //    frame = new MpiSD2Frame(dest);
        //    _prevFrame = frame;

        //    _buffer.ReadBytes(frame.GetFrameLength()); // 移动读取指针到下一帧起始位置
        //    return true;
        //}

        // 检查是否 PPI/SD2 帧
        if (header == MpiFrameType.SD2 && PpiSD2Frame.IsMeetFrameLength(dest)) { // PPI/SD2
            frame = new PpiSD2Frame(dest);
            _prevFrame = frame;

            _buffer.ReadBytes(frame.GetFrameLength()); // 移动读取指针到下一帧起始位置
            return true;
        }

        // 检查是否 SD4 帧
        if (header == MpiFrameType.SD4 && dest.Length >= 3) {// SD4帧
            frame = new MpiSD4Frame(dest);
            _prevFrame = frame;

            _buffer.ReadBytes(frame.GetFrameLength()); // 移动读取指针到下一帧起始位置
            return true;
        }

#if DEBUG
        var idx1 = _buffer.ReaderIndex;
        byte[] tmp = new byte[_buffer.ReadableBytes];
        _buffer.GetBytes(_buffer.ReaderIndex, tmp);
        var idx2 = _buffer.ReaderIndex;
#endif
        // 未发现合法帧，向前推进若干字节，查找合法的SD2帧头位置
        while (_buffer.IsReadable() && _buffer.ReadableBytes > 5) {
            int readerIndex = _buffer.ReaderIndex;
            byte[] buf = new byte[Math.Min(5, _buffer.ReadableBytes)];
            _buffer.GetBytes(_buffer.ReaderIndex, buf);

            byte b0 = buf[0]; // 0x68
            byte b1 = buf[1]; // LEN
            byte b2 = buf[2]; // LENr
            byte b3 = buf[3]; // 0x68

            if (b0 != 0x68 || b3 != 0x68 || b1 != b2) {
                _buffer.SkipBytes(1);
            } else {
                break;
            }
        }

        return false;
    }
}
