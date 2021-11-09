using System;
using System.IO;

namespace NBTSharpLib.Region
{
	class Dimension
	{
		public const string Nether = @"DIM-1\region\";
		public const string Overworld = @"region\";
		public const string End = @"DIM1\region\";
	}

	// Parses all region data
	class RegionReader
	{
		public ChunkLocation[] locations;
		public ChunkTimestamp[] timestamps;
		public Chunk[] chunks;

		public RegionReader(string dir, int x, int y)
		{
			Region region = new Region(dir, x, y);
			locations = ParseLocations(region.data);
			timestamps = ParseTimestamps(region.data);
			chunks = ParseChunks(region.data);
		}

		ChunkLocation[] ParseLocations(byte[] data)
		{
			ChunkLocation[] locations = new ChunkLocation[1024];

			for (int i = 0; i < 1024; i++)
			{
				locations[i] = new ChunkLocation((ushort)MultiByteConvet(data, i * 4, 3), data[i * 4 + 3]);
			}

			return locations;
		}

		ChunkTimestamp[] ParseTimestamps(byte[] data)
		{
			ChunkTimestamp[] timestamps = new ChunkTimestamp[1024];

			for (int i = 0; i < 1024; i++)
			{
				timestamps[i] = new ChunkTimestamp((uint)MultiByteConvet(data, (i + 1024) * 4, 4));
			}

			return timestamps;
		}

		Chunk[] ParseChunks(byte[] data)
		{
			Chunk[] chunks = new Chunk[1024];

			for (int i = 0; i < 1024; i++)
			{
				if (locations[i].offset != 0)
				{
					int offset = locations[i].offset;
					uint length = (uint)MultiByteConvet(data, offset * 4096, 4);
					byte compressionType = data[offset * 4096 + 4];
					byte[] chunkData = data[(offset * 4096)..((offset + locations[i].sectorCount) * 4096)];
					chunks[i] = new Chunk(length, compressionType, chunkData);
				}
				else
				{
					chunks[i] = null;
					Console.WriteLine("empty");
				}
			}

			return chunks;
		}

		// The number of generated chunks
		int NumberOfChunks
		{
			get
			{
				int chunks = 0;

				for (int i = 0; i < 1024; i++)
				{
					if (locations[i].sectorCount != 0) chunks++;
				}

				return chunks;
			}
		}

		static int MultiByteConvet(byte[] bytes, int start, int amount)
		{
			int number = 0;

			for (int i = start; i < amount + start; i++)
			{
				int offset = (amount - i) * 8 - 8;
				number += (bytes[i] & 0xff) << offset;
			}

			return number;
		}
	}

	// Stores all region data
	class Region
	{
		public readonly int[] position;
		public readonly byte[] data;

		public Region(string dir, int x, int y)
		{
			position = new int[] { x, y };
			data = File.ReadAllBytes(@$"{dir}r.{x}.{y}.mca");
		}
	}

	// Stores all of the chunk locations
	class ChunkLocation
	{
		public readonly ushort offset;
		public readonly byte sectorCount;

		public ChunkLocation(ushort offset, byte sectorCount)
		{
			this.offset = offset;
			this.sectorCount = sectorCount;
		}
	}
	
	// Stores all of the chunk timestamps in epoch seconds
	class ChunkTimestamp
	{
		public readonly uint timestamp;

		public ChunkTimestamp(uint timestamp)
		{
			this.timestamp = timestamp;
		}
	}

	// Stores all chunk data
	class Chunk
	{
		public readonly uint length;
		public readonly byte compression;
		public readonly byte[] data;

		public Chunk(uint length, byte compression, byte[] data)
		{
			this.length = length;
			this.compression = compression;
			this.data = data;
		}
	}
}
