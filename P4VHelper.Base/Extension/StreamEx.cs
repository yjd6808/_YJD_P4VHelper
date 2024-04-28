// jdyun 24/04/27(토)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class StreamEx
    {
        public static void WriteString(this Stream _stream, string _value)
        {
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(_value);
            _stream.WriteInt16((short)utf16Bytes.Length);
            _stream.Write(utf16Bytes);
        }

        public static void WriteDateTime(this Stream _stream, DateTime _value)
        {
            byte[] bytes = BitConverter.GetBytes(_value.Ticks);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            _stream.Write(bytes);
        }

        public static void WriteInt32(this Stream _stream, int _value)
        {
            byte[] bytes = BitConverter.GetBytes(_value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            _stream.Write(bytes);
        }

        public static void WriteInt16(this Stream _stream, short _value)
        {
            byte[] bytes = BitConverter.GetBytes(_value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            _stream.Write(bytes);
        }

        public static string ReadString(this Stream _stream)
        {
            short size = _stream.ReadInt16();
            byte[] buffer = new byte[size];
            int bytes = _stream.Read(buffer, 0, size);
            if (bytes < size)
                throw new EndOfStreamException($"{size}바이트 못 읽음");
            return Encoding.Unicode.GetString(buffer);
        }

        public static DateTime ReadDateTime(this Stream _stream)
        {
            const int size = sizeof(long);
            byte[] buffer = new byte[size];
            int bytes = _stream.Read(buffer, 0, size);
            if (bytes < size)
                throw new EndOfStreamException($"{size}바이트 못 읽음");
            return new DateTime(BitConverter.ToInt64(buffer));
        }

        public static int ReadInt32(this Stream _stream)
        {
            const int size = sizeof(int);
            byte[] buffer = new byte[size];
            int bytes = _stream.Read(buffer, 0, size);
            if (bytes < size)
                throw new EndOfStreamException($"{size}바이트 못 읽음");
            return BitConverter.ToInt32(buffer);
        }

        public static short ReadInt16(this Stream _stream)
        {
            const int size = sizeof(short);
            byte[] buffer = new byte[size];
            int bytes = _stream.Read(buffer, 0, size);
            if (bytes < size)
                throw new EndOfStreamException($"{size}바이트 못 읽음");
            return BitConverter.ToInt16(buffer);
        }
    }
}
