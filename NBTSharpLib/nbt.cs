using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NBTSharpLib.NBT
{
	class NBTReader
	{
		public byte[] data;
		public TagCompound root;

		public NBTReader(string dir)
		{
			data = File.ReadAllBytes(dir);
			root = ParseTags(data);
		}

		public static TagCompound ParseTags(byte[] data)
		{
			// Recusivly parses all tags in the root
			return new TagCompound("", data[3..]);
		}

		public static long MultiByteConvet(byte[] bytes, int amount)
		{
			long number = 0;

			for (int i = 0; i < amount; i++)
			{
				int offset = (amount - i) * 8 - 8;
				number += (bytes[i] & 0xff) << offset;
			}

			return number;
		}

		public static int[] MultiByteConvetArray(byte[] bytes, int size)
		{
			int[] array = new int[bytes.Length / size];

			for (int i = 0; i < bytes.Length / size; i++)
			{
				int offset = i * size;
				array[i] = (int)MultiByteConvet(bytes[offset..(offset + size)], size);
			}

			return array;
		}

		public static long[] MultiByteConvetArrayLong(byte[] bytes, int size)
		{
			long[] array = new long[bytes.Length / size];

			for (int i = 0; i < bytes.Length / size; i++)
			{
				int offset = i * size;
				array[i] = MultiByteConvet(bytes[offset..(offset + size)], size);
			}

			return array;
		}

		public static double MultiByteConvetDecimal(byte[] bytes, int amount)
		{
			double number = 0d;

			for (int i = 0; i < amount; i++)
			{
				int offset = (amount - i) * 8 - 8;
				number += (bytes[i] & 0xff) << offset;
			}

			return number;
		}

		public static string ByteArrayToString(byte[] data)
		{
			return Encoding.UTF8.GetString(data);
		}
	}

	enum Tags : byte
	{
		End,
		Byte,
		Short,
		Int,
		Long,
		Float,
		Double,
		ByteArray,
		String,
		List,
		Compound,
		IntArray,
		LongArray
	}

	class Tag
	{
		public Tags id;
		public string name;
	}

	class TagEnd : Tag
	{
		public TagEnd()
		{
			id = Tags.End;
		}
	}

	class TagByte : Tag
	{
		public sbyte payload;

		public TagByte(string _name, sbyte _payload)
		{
			id = Tags.Byte;
			name = _name;
			payload = _payload;
		}
	}

	class TagShort : Tag
	{
		public short payload;

		public TagShort(string _name, short _payload)
		{
			id = Tags.Short;
			name = _name;
			payload = _payload;
		}
	}

	class TagInt : Tag
	{
		public int payload;

		public TagInt(string _name, int _payload)
		{
			id = Tags.Int;
			name = _name;
			payload = _payload;
		}
	}

	class TagLong : Tag
	{
		public long payload;

		public TagLong(string _name, long _payload)
		{
			id = Tags.Long;
			name = _name;
			payload = _payload;
		}
	}

	class TagFloat : Tag
	{
		public float payload;

		public TagFloat(string _name, float _payload)
		{
			id = Tags.Float;
			name = _name;
			payload = _payload;
		}
	}

	class TagDouble : Tag
	{
		public double payload;

		public TagDouble(string _name, double _payload)
		{
			id = Tags.Double;
			name = _name;
			payload = _payload;
		}
	}

	class TagByteArray : Tag
	{
		public sbyte[] payload;

		public TagByteArray(string _name, sbyte[] _payload)
		{
			id = Tags.ByteArray;
			name = _name;
			payload = _payload;
		}
	}

	class TagString : Tag
	{
		public string payload;

		public TagString(string _name, string _payload)
		{
			id = Tags.String;
			name = _name;
			payload = _payload;
		}
	}

	class TagList : Tag
	{
		public Tag[] payload;
		public int endIndex;

		public TagList(string _name, Tag[] _payload)
		{
			id = Tags.List;
			name = _name;
			payload = _payload;
		}
		public TagList(string _name, byte[] _payload)
		{
			id = Tags.List;
			name = _name;
			payload = ParseBytes(_payload);
		}

		public Tag[] ParseBytes(byte[] data)
		{
			int index = 5; // Says where the next tag starts
			Tags id = (Tags)data[0]; // The id that every tag has in the list
			ushort length = (ushort)NBTReader.MultiByteConvet(data[1..], 4); // Number of tags in list
			List<Tag> tags = new List<Tag>();

			for (int i = 0; i < length; i++)
			{
				switch (id)
				{
					case Tags.End:
						tags.Add(new TagEnd());
						index++;
						break;
					case Tags.Byte:
						tags.Add(new TagByte("", (sbyte)data[index]));
						index++;
						break;
					case Tags.Short:
						tags.Add(new TagDouble("", NBTReader.MultiByteConvet(data[index..], 2)));
						index += 2;
						break;
					case Tags.Int:
						tags.Add(new TagDouble("", NBTReader.MultiByteConvet(data[index..], 4)));
						index += 4;
						break;
					case Tags.Long:
						tags.Add(new TagDouble("", NBTReader.MultiByteConvet(data[index..], 8)));
						index += 8;
						break;
					case Tags.Float:
						tags.Add(new TagFloat("", (float)NBTReader.MultiByteConvetDecimal(data[index..], 4)));
						index += 4;
						break;
					case Tags.Double:
						tags.Add(new TagDouble("", NBTReader.MultiByteConvetDecimal(data[index..], 8)));
						index += 8;
						break;
					case Tags.ByteArray:
						Console.WriteLine("Byte array is not supported in list yet");
						break;
					case Tags.String:
						ushort stringLength = (ushort)NBTReader.MultiByteConvet(data[index..], 2);
						tags.Add(new TagString("", NBTReader.ByteArrayToString(data[(index + 2)..(index + 2 + stringLength)])));
						index += 2 + stringLength;
						break;
					case Tags.List:
						TagList listTag = new TagList("", data[index..]);
						tags.Add(listTag);
						index += listTag.endIndex;
						break;
					case Tags.Compound:
						TagCompound compoundTag = new TagCompound("", data[index..]);
						tags.Add(compoundTag);
						index += 1 + compoundTag.endIndex;
						break;
					case Tags.IntArray:
						Console.WriteLine("Int array is not supported in list yet");
						break;
					case Tags.LongArray:
						Console.WriteLine("Long array is not supported in list yet");
						break;
					default:
						Console.WriteLine($"Tag:{index}, not found ");
						break;
				}
			}

			endIndex = index;

			return tags.ToArray();
		}
	}

	class TagCompound : Tag
	{
		public Tag[] payload;
		public int endIndex;

		public TagCompound(string _name, Tag[] _payload)
		{
			id = Tags.Compound;
			name = _name;
			payload = _payload;
		}
		public TagCompound(string _name, byte[] _payload)
		{
			id = Tags.Compound;
			name = _name;
			payload = ParseBytes(_payload);
		}
		
		public Tag[] ParseBytes(byte[] data)
		{
			int index = 0; // Says where the next tag starts
			List<Tag> tags = new List<Tag>();

			Tags id = Tags.Byte;

			while (id != Tags.End)
			{
				id = (Tags)data[index];
				Console.WriteLine("ID: " + id);
				ushort nameLength = 0;
				string name = "";

				if (id != Tags.End)
					nameLength = (ushort)NBTReader.MultiByteConvet(data[(1 + index)..], 2);

				if (nameLength != 0)
					name = NBTReader.ByteArrayToString(data[(3 + index)..(3 + nameLength + index)]);

				if (id != Tags.End)
					Console.WriteLine($"Type: {id}, Name: {name}");

				switch (id)
				{
					case Tags.End:
						endIndex = index;
						break;
					case Tags.Byte:
						tags.Add(new TagByte(name, (sbyte)data[3 + nameLength + index]));
						index += 4 + nameLength;
						break;
					case Tags.Short:
						tags.Add(new TagShort(name, (short)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 2)));
						index += 5 + nameLength;
						break;
					case Tags.Int:
						tags.Add(new TagInt(name, (int)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 4)));
						index += 7 + nameLength;
						break;
					case Tags.Long:
						tags.Add(new TagLong(name, NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 8)));
						index += 11 + nameLength;
						break;
					case Tags.Float:
						tags.Add(new TagFloat(name, (float)NBTReader.MultiByteConvetDecimal(data[(3 + nameLength + index)..], 4)));
						index += 7 + nameLength;
						break;
					case Tags.Double:
						tags.Add(new TagDouble(name, NBTReader.MultiByteConvetDecimal(data[(3 + nameLength + index)..], 8)));
						index += 11 + nameLength;
						break;
					case Tags.ByteArray:
						ushort byteLength = (ushort)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 4);
						sbyte[] array = Array.ConvertAll(data[(7 + nameLength + index)..(7 + nameLength + byteLength + index)], b => unchecked((sbyte)b));
						tags.Add(new TagByteArray(name, array));
						index += 7 + nameLength + byteLength;
						break;
					case Tags.String:
						ushort stringLength = (ushort)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 2);
						tags.Add(new TagString(name, NBTReader.ByteArrayToString(data[(5 + nameLength + index)..(5 + nameLength + stringLength + index)])));
						index += 5 + nameLength + stringLength;
						break;
					case Tags.List:
						TagList listTag = new TagList(name, data[(3 + nameLength + index)..]);
						tags.Add(listTag);
						index += 3 + listTag.endIndex + nameLength;
						break;
					case Tags.Compound:
						Console.WriteLine($"Name: {name}");
						TagCompound tag = new TagCompound(name, data[(3 + nameLength + index)..]);
						tags.Add(tag);
						index += 4 + tag.endIndex + nameLength;
						break;
					case Tags.IntArray:
						ushort intLength = (ushort)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 4);
						tags.Add(new TagIntArray(name, NBTReader.MultiByteConvetArray(data[(7 + nameLength + index)..(7 + nameLength + (intLength * 4) + index)], 4)));
						index += 7 + nameLength + (intLength * 4);
						break;
					case Tags.LongArray:
						ushort longLength = (ushort)NBTReader.MultiByteConvet(data[(3 + nameLength + index)..], 4);
						tags.Add(new TagLongArray(name, NBTReader.MultiByteConvetArrayLong(data[(7 + nameLength + index)..(7 + nameLength + (longLength * 8) + index)], 8)));
						index += 7 + nameLength + (longLength * 8);
						break;
					default:
						Console.WriteLine($"Tag:{index}, not found ");
						break;
				}
			}

			return tags.ToArray();
		}
	}

	class TagIntArray : Tag
	{
		public int[] payload;

		public TagIntArray(string _name, int[] _payload)
		{
			id = Tags.IntArray;
			name = _name;
			payload = _payload;
		}
	}

	class TagLongArray : Tag
	{
		public long[] payload;

		public TagLongArray(string _name, long[] _payload)
		{
			id = Tags.LongArray;
			name = _name;
			payload = _payload;
		}
	}
}
