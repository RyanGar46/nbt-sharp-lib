using System;
using System.IO;
using System.IO.Compression;
using NBTSharpLib.NBT;

namespace NBTSharpLib.Chunk
{
	class ChunkReader
	{
		public static Stream Decompress(byte[] input)
		{
			var output = new MemoryStream();

			using (var compressStream = new MemoryStream(input))
			using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
				decompressor.CopyTo(output);

			output.Position = 0;
			return output;
		}
	}
}
