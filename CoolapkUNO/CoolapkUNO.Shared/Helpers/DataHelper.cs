using CoolapkUNO.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Uno.Extensions;
using Windows.Storage.Streams;

namespace CoolapkUNO.Helpers
{
    public static partial class DataHelper
    {
        /// <summary>
        /// Get the MD5 hash of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The MD5 hash of the input string.</returns>
        public static string GetMD5(this string input)
        {
#if NET
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.HashData(Encoding.UTF8.GetBytes(input));
#else
            // Create a new instance of the MD5CryptoServiceProvider object.
            using MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
#endif

            string results = BitConverter.ToString(data).ToLowerInvariant();

            return results.Replace("-", "");
        }

        /// <summary>
        /// Get the Base64 string of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="isRaw"><see langword="true"/> to remove the padding characters; otherwise, <see langword="false"/>.</param>
        /// <returns>The Base64 string of the input string.</returns>
        public static string GetBase64(this string input, bool isRaw = false)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string result = Convert.ToBase64String(bytes);
            if (!isRaw) { result = result.Replace("=", string.Empty); }
            return result;
        }

        public static string GetSizeString(this double size)
        {
            int index = 0;
            while (index <= 11)
            {
                index++;
                size /= 1024;
                if (size > 0.7 && size < 716.8) { break; }
                else if (size >= 716.8) { continue; }
                else if (size <= 0.7)
                {
                    size *= 1024;
                    index--;
                    break;
                }
            }
            string str = string.Empty;
            switch (index)
            {
                case 0: str = "B"; break;
                case 1: str = "KB"; break;
                case 2: str = "MB"; break;
                case 3: str = "GB"; break;
                case 4: str = "TB"; break;
                case 5: str = "PB"; break;
                case 6: str = "EB"; break;
                case 7: str = "ZB"; break;
                case 8: str = "YB"; break;
                case 9: str = "BB"; break;
                case 10: str = "NB"; break;
                case 11: str = "DB"; break;
                default:
                    break;
            }
            return $"{size:0.##}{str}";
        }

        public static string GetNumString(this double num)
        {
            string str = string.Empty;
            if (num < 1000) { }
            else if (num < 10000)
            {
                str = "k";
                num /= 1000;
            }
            else if (num < 10000000)
            {
                str = "w";
                num /= 10000;
            }
            else
            {
                str = "kw";
                num /= 10000000;
            }
            return $"{num:N2}{str}";
        }

        public static string CSStoString(this string str)
        {
            try
            {
                HtmlToText HtmlToText = new();
                return HtmlToText.Convert(str);
            }
            catch (Exception ex)
            {
                LogExtensionPoint.Log(typeof(DataHelper)).LogWarning(ex, "{HtmlToText} occurred some error. {message} (0x{hResult:X})", typeof(HtmlToText), ex.GetMessage(), ex.HResult);
                //换行和段落
                string s = str.Replace("<br>", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br/>", "\n").Replace("<p>", "").Replace("</p>", "\n").Replace("&nbsp;", " ").Replace("<br />", "").Replace("<br />", "");
                //链接彻底删除！
                while (s.IndexOf("<a", StringComparison.Ordinal) > 0)
                {
                    s = s.Replace(@"<a href=""" + Regex.Split(Regex.Split(s, @"<a href=""")[1], @""">")[0] + @""">", "");
                    s = s.Replace("</a>", "");
                }
                return s;
            }
        }

#if WINDOWS_UWP
        /// <summary>
        /// Determines whether this string instance starts with the specified character.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="value">The character to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> matches the beginning of this string; otherwise, <see langword="false"/>.</returns>
        public static bool StartsWith(this string text, char value) => text.StartsWith(new string([value]));

        /// <summary>
        /// Returns a value indicating whether a specified character occurs within this string.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="value">The character to seek.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> parameter occurs within this string; otherwise, <see langword="false"/>.</returns>
        public static bool Contains(this string text, char value) => text.IndexOf(value) != -1;

        /// <summary>
        /// Returns a value indicating whether all of a specified array of string occurs within this string, using the specified comparison rules.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="allOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
        /// <returns><see langword="true"/> if the <paramref name="allOf"/> parameter occurs within this string; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsAll(this string text, string[] allOf, StringComparison comparisonType) => allOf.All(x => text.Contains(x, comparisonType));
#endif

        /// <summary>
        /// Returns a value indicating whether any of a specified array of string occurs within this string.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <returns><see langword="true"/> if the <paramref name="anyOf"/> parameter occurs within this string; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsAny(this string text, params string[] anyOf) => anyOf.Any(text.Contains);

        /// <summary>
        /// Returns a value indicating whether any of a specified array of string occurs within this string, using the specified comparison rules.
        /// </summary>
        /// <param name="text">A sequence in which to locate a value.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
        /// <returns><see langword="true"/> if the <paramref name="anyOf"/> parameter occurs within this string; otherwise, <see langword="false"/>.</returns>
        public static bool ContainsAny(this string text, string[] anyOf, StringComparison comparisonType) => anyOf.Any(x => text.Contains(x, comparisonType));

        public static IBuffer GetBuffer(this IRandomAccessStream randomStream)
        {
            using Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
            return stream.GetBuffer();
        }

        public static byte[] GetBytes(this IRandomAccessStream randomStream)
        {
            using Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
            return stream.GetBytes();
        }

        public static IBuffer GetBuffer(this Stream stream)
        {
            byte[] bytes = [];
            if (stream != null)
            {
                bytes = stream.GetBytes();
            }
            return bytes.AsBuffer();
        }

        public static byte[] GetBytes(this Stream stream)
        {
            if (stream.CanSeek) // stream.Length 已确定
            {
                byte[] bytes = new byte[stream.Length];
#if NET7_0_OR_GREATER
                stream.ReadExactly(bytes);
#else
                stream.Read(bytes, 0, bytes.Length);
#endif
                stream.Seek(0, SeekOrigin.Begin);
                return bytes;
            }
            else // stream.Length 不确定
            {
                int initialLength = 32768; // 32k

                byte[] buffer = new byte[initialLength];
                int read = 0;

                int chunk;
                while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
                {
                    read += chunk;

                    if (read == buffer.Length)
                    {
                        int nextByte = stream.ReadByte();

                        if (nextByte == -1)
                        {
                            return buffer;
                        }

                        byte[] newBuffer = new byte[buffer.Length * 2];
                        Array.Copy(buffer, newBuffer, buffer.Length);
                        newBuffer[read] = (byte)nextByte;
                        buffer = newBuffer;
                        read++;
                    }
                }

                byte[] ret = new byte[read];
                Array.Copy(buffer, ret, read);
                return ret;
            }
        }

#if BROWSER
        private static class MD5
        {
            public static byte[] HashData(byte[] input)
            {
                uint num = 1732584193;
                uint num2 = 4023233417;
                uint num3 = 2562383102;
                uint num4 = 271733878;
                int num5 = (56 - (input.Length + 1) % 64) % 64;
                byte[] array = new byte[input.Length + 1 + num5 + 8];
                Array.Copy(input, array, input.Length);
                array[input.Length] = 128;
                Array.Copy(BitConverter.GetBytes(input.Length * 8), 0, array, array.Length - 8, 4);
                for (int i = 0; i < array.Length / 64; i++)
                {
                    uint[] array2 = new uint[16];
                    for (int j = 0; j < 16; j++)
                    {
                        array2[j] = BitConverter.ToUInt32(array, i * 64 + j * 4);
                    }

                    uint num6 = num;
                    uint num7 = num2;
                    uint num8 = num3;
                    uint num9 = num4;
                    uint num12 = 0;
                    while (true)
                    {
                        uint num10, num11;
                        switch (num12)
                        {
                            case >= 0 and <= 15:
                                num10 = num7 & num8 | ~num7 & num9;
                                num11 = num12;
                                goto IL_0138;
                            case >= 16 and <= 31:
                                num10 = num8 ^ (num7 | ~num9);
                                num11 = (5 * num12 + 1) % 16;
                                goto IL_0138;
                            case >= 32 and <= 47:
                                num10 = num7 ^ num8 ^ num9;
                                num11 = (3 * num12 + 5) % 16;
                                goto IL_0138;
                            case >= 48 and <= 63:
                                num10 = num8 ^ (num7 | ~num9);
                                num11 = 7 * num12 % 16;
                                goto IL_0138;
                        }
                        break;
                    IL_0138:
                        uint num13 = num9;
                        num9 = num8;
                        num8 = num7;
                        num7 += LeftRotate(num6 + num10 + K[num12] + array2[num11], s[num12]);
                        num6 = num13;
                        num12++;
                    }

                    num += num6;
                    num2 += num7;
                    num3 += num8;
                    num4 += num9;
                }
                byte[] hashBytes = new byte[16];
                BitConverter.GetBytes(num).CopyTo(hashBytes, 0);
                BitConverter.GetBytes(num2).CopyTo(hashBytes, 4);
                BitConverter.GetBytes(num3).CopyTo(hashBytes, 8);
                BitConverter.GetBytes(num4).CopyTo(hashBytes, 12);
                return hashBytes;
            }

            private static readonly int[] s =
            [
                7, 12, 17, 22, 7, 12, 17, 22, 7, 12,
                17, 22, 7, 12, 17, 22, 5, 9, 14, 20,
                5, 9, 14, 20, 5, 9, 14, 20, 5, 9,
                14, 20, 4, 11, 16, 23, 4, 11, 16, 23,
                4, 11, 16, 23, 4, 11, 16, 23, 6, 10,
                15, 21, 6, 10, 15, 21, 6, 10, 15, 21,
                6, 10, 15, 21
            ];

            private static readonly uint[] K =
            [
                3614090360u, 3905402710u, 606105819u, 3250441966u, 4118548399u, 1200080426u, 2821735955u, 4249261313u, 1770035416u, 2336552879u,
                4294925233u, 2304563134u, 1804603682u, 4254626195u, 2792965006u, 1236535329u, 4129170786u, 3225465664u, 643717713u, 3921069994u,
                3593408605u, 38016083u, 3634488961u, 3889429448u, 568446438u, 3275163606u, 4107603335u, 1163531501u, 2850285829u, 4243563512u,
                1735328473u, 2368359562u, 4294588738u, 2272392833u, 1839030562u, 4259657740u, 2763975236u, 1272893353u, 4139469664u, 3200236656u,
                681279174u, 3936430074u, 3572445317u, 76029189u, 3654602809u, 3873151461u, 530742520u, 3299628645u, 4096336452u, 1126891415u,
                2878612391u, 4237533241u, 1700485571u, 2399980690u, 4293915773u, 2240044497u, 1873313359u, 4264355552u, 2734768916u, 1309151649u,
                4149444226u, 3174756917u, 718787259u, 3951481745u
            ];

            private static uint LeftRotate(uint x, int c) => x << c | x >> 32 - c;
        }
#endif
    }
}
