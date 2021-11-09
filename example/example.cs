using System;
using NBTSharpLib.Region;
using NBTSharpLib.NBT;

namespace NBTSharpLib.Example
{
	class example
	{
		static void Main()
		{
			string dir = $@"D:\minecraft\saves\Our Realm (1.17)\{Dimension.Overworld}";

			RegionReader regionReader = new RegionReader(dir, 0, 0);

			NBTReader nbtReader = new NBTReader(@"D:\Code Projects\C#\mcwLib\example\PlayerData.nbt");

			for (int i = 0; i < nbtReader.root.payload.Length; i++)
			{
				Console.WriteLine(nbtReader.root.payload[i].name);
			}


			/*
			ChunkReader.Decompress(regionReader.chunks[2].data);

			ChunkReader chunkReader = new ChunkReader(regionReader.chunks[0].data);
			*/
		}
	}
}
