using System;
using System.IO;

namespace Galaxy
{
    public static partial class Utility
    {
        /// <summary>
        /// 压缩解压缩相关的实用函数。
        /// </summary>
        public static partial class Zip
        {
            private static IZipHelper s_ZipHelper = null;

            /// <summary>
            /// 设置压缩解压缩辅助器。
            /// </summary>
            /// <param name="zipHelper">要设置的压缩解压缩辅助器。</param>
            public static void SetZipHelper(IZipHelper zipHelper)
            {
                s_ZipHelper = zipHelper;
            }

            /// <summary>
            /// 压缩数据。
            /// </summary>
            /// <param name="bytes">要压缩的数据的二进制流。</param>
            /// <returns>压缩后的数据的二进制流。</returns>
            public static byte[] Compress(byte[] bytes)
            {
                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                return Compress(bytes, 0, bytes.Length);
            }

            /// <summary>
            /// 压缩数据。
            /// </summary>
            /// <param name="bytes">要压缩的数据的二进制流。</param>
            /// <param name="compressedStream">压缩后的数据的二进制流。</param>
            /// <returns>是否压缩数据成功。</returns>
            public static bool Compress(byte[] bytes, Stream compressedStream)
            {
                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                return Compress(bytes, 0, bytes.Length, compressedStream);
            }

            /// <summary>
            /// 压缩数据。
            /// </summary>
            /// <param name="bytes">要压缩的数据的二进制流。</param>
            /// <param name="offset">要压缩的数据的二进制流的偏移。</param>
            /// <param name="length">要压缩的数据的二进制流的长度。</param>
            /// <returns>压缩后的数据的二进制流。</returns>
            public static byte[] Compress(byte[] bytes, int offset, int length)
            {
                using (MemoryStream result = new MemoryStream())
                {
                    if (Compress(bytes, offset, length, result))
                    {
                        return result.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// 压缩数据。
            /// </summary>
            /// <param name="bytes">要压缩的数据的二进制流。</param>
            /// <param name="offset">要压缩的数据的二进制流的偏移。</param>
            /// <param name="length">要压缩的数据的二进制流的长度。</param>
            /// <param name="compressedStream">压缩后的数据的二进制流。</param>
            /// <returns>是否压缩数据成功。</returns>
            public static bool Compress(byte[] bytes, int offset, int length, Stream compressedStream)
            {
                if (s_ZipHelper == null)
                {
                    throw new GalaxyException("Zip helper is invalid.");
                }

                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                if (offset < 0)
                {
                    throw new GalaxyException("Offset is invalid.");
                }

                if (length > bytes.Length)
                {
                    throw new GalaxyException("Length is invalid.");
                }

                if (compressedStream == null)
                {
                    throw new GalaxyException("Result is invalid.");
                }

                try
                {
                    return s_ZipHelper.Compress(bytes, offset, length, compressedStream);
                }
                catch (Exception exception)
                {
                    if (exception is GalaxyException)
                    {
                        throw;
                    }

                    throw new GalaxyException(Text.Format("Can not compress bytes with exception '{0}'.", exception.ToString()), exception);
                }
            }

            /// <summary>
            /// 解压缩数据。
            /// </summary>
            /// <param name="bytes">要解压缩的数据的二进制流。</param>
            /// <returns>解压缩后的数据的二进制流。</returns>
            public static byte[] Decompress(byte[] bytes)
            {
                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                return Decompress(bytes, 0, bytes.Length);
            }

            /// <summary>
            /// 解压缩数据。
            /// </summary>
            /// <param name="bytes">要解压缩的数据的二进制流。</param>
            /// <param name="decompressedStream">解压缩后的数据的二进制流。</param>
            /// <returns>是否解压缩数据成功。</returns>
            public static bool Decompress(byte[] bytes, Stream decompressedStream)
            {
                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                return Decompress(bytes, 0, bytes.Length, decompressedStream);
            }

            /// <summary>
            /// 解压缩数据。
            /// </summary>
            /// <param name="bytes">要解压缩的数据的二进制流。</param>
            /// <param name="offset">要解压缩的数据的二进制流的偏移。</param>
            /// <param name="length">要解压缩的数据的二进制流的长度。</param>
            /// <returns>解压缩后的数据的二进制流。</returns>
            public static byte[] Decompress(byte[] bytes, int offset, int length)
            {
                using (MemoryStream result = new MemoryStream())
                {
                    if (Decompress(bytes, offset, length, result))
                    {
                        return result.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// 解压缩数据。
            /// </summary>
            /// <param name="bytes">要解压缩的数据的二进制流。</param>
            /// <param name="offset">要解压缩的数据的二进制流的偏移。</param>
            /// <param name="length">要解压缩的数据的二进制流的长度。</param>
            /// <param name="decompressedStream">解压缩后的数据的二进制流。</param>
            /// <returns>是否解压缩数据成功。</returns>
            public static bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream)
            {
                if (s_ZipHelper == null)
                {
                    throw new GalaxyException("Zip helper is invalid.");
                }

                if (bytes == null)
                {
                    throw new GalaxyException("Bytes is invalid.");
                }

                if (offset < 0)
                {
                    throw new GalaxyException("Offset is invalid.");
                }

                if (length > bytes.Length)
                {
                    throw new GalaxyException("Length is invalid.");
                }

                if (decompressedStream == null)
                {
                    throw new GalaxyException("Result is invalid.");
                }

                try
                {
                    return s_ZipHelper.Decompress(bytes, offset, length, decompressedStream);
                }
                catch (Exception exception)
                {
                    if (exception is GalaxyException)
                    {
                        throw;
                    }

                    throw new GalaxyException(Text.Format("Can not decompress bytes with exception '{0}'.", exception.ToString()), exception);
                }
            }
        }
    }
}
