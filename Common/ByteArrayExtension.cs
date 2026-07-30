using System;
using System.Security.Cryptography;
using System.Text;

namespace S7PpiMonitor.Common;

public static class ByteArrayExtension
{
    /// <summary>
    /// 颠倒字节顺序、并返回结果
    /// </summary>
    public static byte[] SwapBytes(this byte[] src)
    {
        byte[] buff = new byte[src.Length];
        Array.Copy(src, buff, src.Length);
        Array.Reverse(buff);

        return buff;
    }

    public static string ToHexString(this byte[] buff, bool delimiter = false, bool spanSpace = false)
    {
        return ToHexString(buff, 0, buff.Length, delimiter, spanSpace);
    }

    public static string ToHexString(this byte[] buff, int index, int length, bool delimiter = false, bool insertSpace = false)
    {
        StringBuilder sb = new StringBuilder(length * 3);

        for (int i = 0; i < Math.Min(length, buff.Length - index); i++) {
            sb.AppendFormat("{0:X2}", buff[index + i]);
            if (insertSpace)
                sb.Append(' ');
            if (delimiter) {
                if ((i > 0) && (i % 8 == 0))
                    sb.Append(' ');
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 使用默认算法Hash
    /// </summary>
    public static byte[] Hash(this byte[] data)
    {
        var sha1 = SHA1.Create();
        return sha1.ComputeHash(data);

    }

}
