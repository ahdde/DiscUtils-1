﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscUtils
{
    internal class Utilities
    {
        /// <summary>
        /// The number of bytes in a standard disk sector (512).
        /// </summary>
        internal const int SectorSize = 512;

        /// <summary>
        /// Prevent instantiation.
        /// </summary>
        private Utilities() { }

        /// <summary>
        /// Converts between two arrays.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source array</typeparam>
        /// <typeparam name="U">The type of the elements of the destination array</typeparam>
        /// <param name="source">The source array</param>
        /// <param name="func">The function to map from source type to destination type</param>
        /// <returns>The resultant array</returns>
        public static U[] Map<T, U>(ICollection<T> source, Func<T, U> func)
        {
            U[] result = new U[source.Count];
            int i = 0;

            foreach (T sVal in source)
            {
                result[i++] = func(sVal);
            }

            return result;
        }

        /// <summary>
        /// Filters a collection into a new collection.
        /// </summary>
        /// <typeparam name="C">The type of the new collection</typeparam>
        /// <typeparam name="T">The type of the collection entries</typeparam>
        /// <param name="source">The collection to filter</param>
        /// <param name="predicate">The predicate to select which entries are carried over</param>
        /// <returns>The new collection, containing all entries where the predicate returns <code>true</code></returns>
        public static C Filter<C, T>(ICollection<T> source, Func<T, bool> predicate) where C : ICollection<T>, new()
        {
            C result = new C();
            foreach (T val in source)
            {
                if (predicate(val))
                {
                    result.Add(val);
                }
            }
            return result;
        }

        /// <summary>
        /// Indicates if two ranges overlap.
        /// </summary>
        /// <typeparam name="T">The type of the ordinals</typeparam>
        /// <param name="xFirst">The lowest ordinal of the first range (inclusive)</param>
        /// <param name="xLast">The highest ordinal of the first range (exclusive)</param>
        /// <param name="yFirst">The lowest ordinal of the second range (inclusive)</param>
        /// <param name="yLast">The highest ordinal of the second range (exclusive)</param>
        /// <returns><code>true</code> if the ranges overlap, else <code>false</code></returns>
        public static bool RangesOverlap<T>(T xFirst, T xLast, T yFirst, T yLast) where T : IComparable<T>
        {
            return !((xLast.CompareTo(yFirst) <= 0) || (xFirst.CompareTo(yLast) >= 0));
        }

        #region Bit Twiddling
        public static ushort BitSwap(ushort value)
        {
            return (ushort)(((value & 0x00FF) << 8) | ((value & 0xFF00) >> 8));
        }

        public static uint BitSwap(uint value)
        {
            return ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0x00FF0000) >> 8) | ((value & 0xFF000000) >> 24);
        }

        public static ulong BitSwap(ulong value)
        {
            return ((ulong)(BitSwap((uint)(value & 0xFFFFFFFF))) << 32) | BitSwap((uint)(value >> 32));
        }

        public static short BitSwap(short value)
        {
            return (short)BitSwap((ushort)value);
        }

        public static int BitSwap(int value)
        {
            return (int)BitSwap((uint)value);
        }

        public static long BitSwap(long value)
        {
            return (long)BitSwap((ulong)value);
        }

        public static void WriteBytesLittleEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
        }

        public static void WriteBytesLittleEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
        }

        public static void WriteBytesLittleEndian(ulong val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 4] = (byte)((val >> 32) & 0xFF);
            buffer[offset + 5] = (byte)((val >> 40) & 0xFF);
            buffer[offset + 6] = (byte)((val >> 48) & 0xFF);
            buffer[offset + 7] = (byte)((val >> 64) & 0xFF);
        }

        public static void WriteBytesLittleEndian(short val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((ushort)val, buffer, offset);
        }

        public static void WriteBytesLittleEndian(int val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((uint)val, buffer, offset);
        }

        public static void WriteBytesLittleEndian(long val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((ulong)val, buffer, offset);
        }

        public static void WriteBytesBigEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val >> 8);
            buffer[offset + 1] = (byte)(val & 0xFF);
        }

        public static void WriteBytesBigEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(val & 0xFF);
        }

        public static void WriteBytesBigEndian(ulong val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((val >> 56) & 0xFF);
            buffer[offset + 1] = (byte)((val >> 48) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 40) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 32) & 0xFF);
            buffer[offset + 4] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 5] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 6] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 7] = (byte)(val & 0xFF);
        }

        public static void WriteBytesBigEndian(short val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((ushort)val, buffer, offset);
        }

        public static void WriteBytesBigEndian(int val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((uint)val, buffer, offset);
        }

        public static void WriteBytesBigEndian(long val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((ulong)val, buffer, offset);
        }

        public static void WriteBytesBigEndian(Guid val, byte[] buffer, int offset)
        {
            byte[] le = val.ToByteArray();
            WriteBytesBigEndian(ToUInt32LittleEndian(le, 0), buffer, offset + 0);
            WriteBytesBigEndian(ToUInt16LittleEndian(le, 4), buffer, offset + 4);
            WriteBytesBigEndian(ToUInt16LittleEndian(le, 6), buffer, offset + 6);
            Array.Copy(le, 8, buffer, offset + 8, 8);
        }

        public static ushort ToUInt16LittleEndian(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset + 1] << 8) & 0xFF00) | ((buffer[offset + 0] << 0) & 0x00FF));
        }

        public static uint ToUInt32LittleEndian(byte[] buffer, int offset)
        {
            return (uint)(((buffer[offset + 3] << 24) & 0xFF000000U) | ((buffer[offset + 2] << 16) & 0x00FF0000U)
                | ((buffer[offset + 1] << 8) & 0x0000FF00U) | ((buffer[offset + 0] << 0) & 0x000000FFU));
        }

        public static ulong ToUInt64LittleEndian(byte[] buffer, int offset)
        {
            return (ToUInt32LittleEndian(buffer, offset + 4) << 32) | ToUInt32LittleEndian(buffer, offset + 0);
        }

        public static short ToInt16LittleEndian(byte[] buffer, int offset)
        {
            return (short)ToUInt16LittleEndian(buffer, offset);
        }

        public static int ToInt32LittleEndian(byte[] buffer, int offset)
        {
            return (int)ToUInt32LittleEndian(buffer, offset);
        }

        public static long ToInt64LittleEndian(byte[] buffer, int offset)
        {
            return (long)ToUInt64LittleEndian(buffer, offset);
        }

        public static ushort ToUInt16BigEndian(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset] << 8) & 0xFF00) | ((buffer[offset + 1] << 0) & 0x00FF));
        }

        public static uint ToUInt32BigEndian(byte[] buffer, int offset)
        {
            uint val = (uint)(((buffer[offset + 0] << 24) & 0xFF000000U) | ((buffer[offset + 1] << 16) & 0x00FF0000U)
                | ((buffer[offset + 2] << 8) & 0x0000FF00U) | ((buffer[offset + 3] << 0) & 0x000000FFU));
            return val;
        }

        public static ulong ToUInt64BigEndian(byte[] buffer, int offset)
        {
            return (((ulong)ToUInt32BigEndian(buffer, offset + 0)) << 32) | ToUInt32BigEndian(buffer, offset + 4);
        }

        public static short ToInt16BigEndian(byte[] buffer, int offset)
        {
            return (short)ToUInt16BigEndian(buffer, offset);
        }

        public static int ToInt32BigEndian(byte[] buffer, int offset)
        {
            return (int)ToUInt32BigEndian(buffer, offset);
        }

        public static long ToInt64BigEndian(byte[] buffer, int offset)
        {
            return (long)ToUInt64BigEndian(buffer, offset);
        }

        public static Guid ToGuidBigEndian(byte[] buffer, int offset)
        {
            return new Guid(
                ToUInt32BigEndian(buffer, offset + 0),
                ToUInt16BigEndian(buffer, offset + 4),
                ToUInt16BigEndian(buffer, offset + 6),
                buffer[offset + 8],
                buffer[offset + 9],
                buffer[offset + 10],
                buffer[offset + 11],
                buffer[offset + 12],
                buffer[offset + 13],
                buffer[offset + 14],
                buffer[offset + 15]
                );
        }

        /// <summary>
        /// Primitive conversion from Unicode to ASCII that preserves special characters.
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="dest">The buffer to fill</param>
        /// <param name="offset">The start of the string in the buffer</param>
        /// <param name="count">The number of characters to convert</param>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points by removing the top 16 bits of each character.</remarks>
        public static void StringToBytes(string value, byte[] dest, int offset, int count)
        {
            char[] chars = value.ToCharArray();

            int i = 0;
            while (i < chars.Length)
            {
                dest[i + offset] = (byte)chars[i];
                ++i;
            }
            while (i < count)
            {
                dest[i + offset] = 0;
                ++i;
            }
        }

        /// <summary>
        /// Primitive conversion from ASCII to Unicode that preserves special characters.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <param name="offset">The first byte to convert</param>
        /// <param name="count">The number of bytes to convert</param>
        /// <returns>The string</returns>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points.</remarks>
        public static string BytesToString(byte[] data, int offset, int count)
        {
            char[] result = new char[count];

            for (int i = 0; i < count; ++i)
            {
                result[i] = (char)data[i + offset];
            }

            return new String(result);
        }

        #endregion

        #region Path Manipulation
        /// <summary>
        /// Extracts the directory part of a path.
        /// </summary>
        /// <param name="path">The path to process</param>
        /// <returns>The directory part</returns>
        public static string GetDirectoryFromPath(string path)
        {
            string trimmed = path.Trim('\\');

            int index = trimmed.LastIndexOf('\\');
            if (index < 0)
            {
                return ""; // No directory, just a file name
            }

            return trimmed.Substring(0, index);
        }

        /// <summary>
        /// Extracts the file part of a path.
        /// </summary>
        /// <param name="path">The path to process</param>
        /// <returns>The file part of the path</returns>
        public static string GetFileFromPath(string path)
        {
            string trimmed = path.Trim('\\');

            int index = trimmed.LastIndexOf('\\');
            if (index < 0)
            {
                return trimmed; // No directory, just a file name
            }

            return trimmed.Substring(index + 1);
        }

        /// <summary>
        /// Combines two paths.
        /// </summary>
        /// <param name="a">The first part of the path</param>
        /// <param name="b">The second part of the path</param>
        /// <returns>The combined path</returns>
        public static string CombinePaths(string a, string b)
        {
            return a.TrimEnd('\\') + '\\' + b.TrimStart('\\');
        }

        #endregion

        #region Stream Manipulation
        /// <summary>
        /// Read bytes until buffer filled or EOF.
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <param name="buffer">The buffer to populate</param>
        /// <param name="offset">Offset in the buffer to start</param>
        /// <param name="length">The number of bytes to read</param>
        /// <returns>The number of bytes actually read.</returns>
        internal static int ReadFully(Stream stream, byte[] buffer, int offset, int length)
        {
            int totalRead = 0;
            int numRead = stream.Read(buffer, offset, length);
            while (numRead > 0)
            {
                totalRead += numRead;
                if (totalRead == length)
                {
                    break;
                }

                numRead = stream.Read(buffer, offset + totalRead, length - totalRead);
            }

            return totalRead;
        }

        /// <summary>
        /// Read bytes until buffer filled or throw IOException.
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The data read from the stream</returns>
        public static byte[] ReadFully(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            if (ReadFully(stream, buffer, 0, count) == count)
            {
                return buffer;
            }
            else
            {
                throw new IOException("Unable to complete read of " + count + " bytes");
            }
        }

        /// <summary>
        /// Reads a disk sector (512 bytes).
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <returns></returns>
        public static byte[] ReadSector(Stream stream)
        {
            return ReadFully(stream, SectorSize);
        }

        /// <summary>
        /// Copies the contents of one stream to another.
        /// </summary>
        /// <param name="source">The stream to copy from</param>
        /// <param name="dest">The destination stream</param>
        /// <remarks>Copying starts at the current stream positions</remarks>
        public static void PumpStreams(Stream source, Stream dest)
        {
            byte[] buffer = new byte[64 * 1024];

            int numRead = source.Read(buffer, 0, buffer.Length);
            while (numRead != 0)
            {
                dest.Write(buffer, 0, numRead);
                numRead = source.Read(buffer, 0, buffer.Length);
            }
        }

        #endregion

        #region Filesystem Support
        /// <summary>
        /// Converts a 'standard' wildcard file/path specification into a regular expression.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert</param>
        /// <returns>The resultant regular expression</returns>
        /// <remarks>
        /// The wildcard * (star) matches zero or more characters (including '.'), and ?
        /// (question mark) matches precisely one character (except '.').
        /// </remarks>
        internal static Regex ConvertWildcardsToRegEx(string pattern)
        {
            if (!pattern.Contains('.'))
            {
                pattern += ".";
            }
            string query = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", "[^.]") + "$";
            return new Regex(query, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        #endregion
    }
}
