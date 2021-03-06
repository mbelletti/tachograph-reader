using System;
using System.IO;
using System.Text;

namespace DataFileReader
{
	/// <summary>
	/// Simple extension to BinaryReader with convenience methods for reading from tachograph file
	/// </summary>
	public class CustomBinaryReader : System.IO.BinaryReader
	{
		// get clock ticks since 1 January 1970
		private static readonly long ticks1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks; 

		public CustomBinaryReader(Stream s) : base(s)
		{
		}

		public uint ReadSInt32()
		{
			// in tachograph file number is little-endian

			byte r1=ReadByte();
			byte r2=ReadByte();
			byte r3=ReadByte();
			byte r4=ReadByte();

			return (uint) (r4 | r3 << 8 | r2 << 16 | r1 << 24);
		}

		public uint ReadSInt24()
		{
			byte r1=ReadByte();
			byte r2=ReadByte();
			byte r3=ReadByte();

			return (uint) (r3 | r2 << 8 | r1 << 16);
		}

		public uint ReadSInt16()
		{
			byte r1=ReadByte();
			byte r2=ReadByte();

			return (uint) (r2 | r1 << 8);
		}

		public string ReadString(int length)
		{
			return ReadString(length, Encoding.Default);
		}

		public string ReadString(int length, Encoding enc)
		{
			byte[] buf=new byte[length];

			int amountRead=Read(buf, 0, length);
			if ( amountRead != length )
				throw new InvalidOperationException("End of file while reading a string");

			char[] chars=enc.GetChars(buf);
			return new string(chars);
		}

		public DateTime ReadTimeReal()
		{
			// the offset is seconds since 1 January 1970
			uint offset=ReadSInt32();

			// Calculate the absolute number of ticks (100ths of nanoseconds since 0000)
			long absTicks=ticks1970 + offset * 10000000L;

			// and convert to actual date time class
			return new DateTime(absTicks);
		}
	}
}
