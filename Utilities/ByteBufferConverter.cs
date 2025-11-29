namespace S7PpiMonitor.Utilities;

public static class ByteBufferConverter
{
    /// <summary>
    /// 按“小端Little-Endian(Intel CPU默认）”获取字节数组
    /// </summary>
    public static byte[] PPI_GetBytesLE(this byte[] source, int offset, int length)
    {
        var tmp = new byte[length];
        for (int i = 0; i < length; i++) {
            tmp[i] = source[offset + i];
        }
        return tmp;
    }

    /// <summary>
    /// 按“大端Big-Endian”获取获取字节数组
    /// </summary>
    public static byte[] PPI_GetBytesBE(this byte[] source, int offset, int length)
    {
        var tmp = new byte[length];
        for (int i = 0; i < length; i++) {
            tmp[length - i - 1] = source[offset + i];
        }
        return tmp;
    }

    /// <summary>
    /// 按“小端Little-Endian(Intel CPU默认）”获取short
    /// </summary>
    public static short PPI_GetShortLE(this byte[] source, int offset)
    {
        var tmpBuff = PPI_GetBytesLE(source, offset, 2);
        return BitConverter.ToInt16(tmpBuff, 0);
    }

    /// <summary>
    /// 按“大端Big-Endian”获取short
    /// </summary>
    public static short PPI_GetShortBE(this byte[] source, int offset)
    {
        var tmpBuff = PPI_GetBytesBE(source, offset, 2);
        return BitConverter.ToInt16(tmpBuff, 0);
    }

    /// <summary>
    /// 按“小端Little-Endian(Intel CPU默认）”获取int
    /// </summary>
    public static int PPI_GetIntLE(this byte[] source, int offset)
    {
        var tmpBuff = PPI_GetBytesLE(source, offset, 4);
        return BitConverter.ToInt32(tmpBuff, 0);
    }

    /// <summary>
    /// 按“大端Big-Endian”获取int
    /// </summary>
    public static int PPI_GetIntBE(this byte[] source, int offset)
    {
        var tmpBuff = PPI_GetBytesBE(source, offset, 4);
        return BitConverter.ToInt16(tmpBuff, 0);
    }
}
