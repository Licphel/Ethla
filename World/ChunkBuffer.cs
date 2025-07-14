using Spectrum.Codec;
using Spectrum.IO.Bin;
using Spectrum.IO.Pathfind;
using Spectrum.Maths;

namespace Ethla.World;

public class ChunkBuffer
{

	public static int Size = 16;

	private readonly int coord;
	private readonly PathHandle path;

	private BinaryCompound temp;

	public ChunkBuffer(PathHandle file0, int coord)
	{
		this.coord = coord;
		path = file0.Find("chunks_" + coord + ".bin");
	}

	public void TryWrite()
	{
		if (!path.Exists) path.Mkfile();

		BinaryRw.Write(temp, path);
	}

	public void TryRead(Chunk chunk)
	{
		readTemp();
		if (temp.Try("chunk_" + chunk.Coord, out dynamic compound1)) chunk.Read(compound1);
	}

	public void WriteToBuffer(Chunk chunk, bool removal = false)
	{
		readTemp();
		BinaryCompound compound1 = BinaryCompound.New();
		chunk.Write(compound1, removal);
		temp.Set("chunk_" + chunk.Coord, compound1);
	}

	public bool IsChunkArchived(Chunk chunk)
	{
		readTemp();

		return temp.Has("chunk_" + chunk.Coord);
	}

	private void readTemp()
	{
		if (temp == null)
		{
			if (path.Exists)
				temp = BinaryRw.Read(path);
			else
				temp = BinaryCompound.New();
		}
	}

	public bool IsBufferAvailable(Level level)
	{
		foreach (Chunk chunk in level.ActiveChunks)
			if (Mathf.FastFloor((float)chunk.Coord / Size) == coord)
				return true;
		return false;
	}

}
