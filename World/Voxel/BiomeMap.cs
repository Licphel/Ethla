using System.Runtime.CompilerServices;
using Ethla.Api;
using Ethla.World.Generating;

namespace Ethla.World.Voxel;

public unsafe class BiomeMap
{

	public delegate void XyForeach(int x, int y);

	private const int Sx = Chunk.Width;
	private const int Sy = Chunk.Height;
	public byte[] Bytes;

	public BiomeMap()
	{
		Bytes = new byte[Sx * Sy * sizeof(int)];
	}

	public int Index(int x, int y)
	{
		x &= Sx - 1;
		y &= Sy - 1;
		return (x * Sy + y) * sizeof(int);
	}

	protected void writeBytes(int idx, int v)
	{
		fixed (byte* p = Bytes)
		{
			Unsafe.Write(p + idx, v);
		}
	}

	protected int readBytes(int idx)
	{
		fixed (byte* p = Bytes)
		{
			return Unsafe.Read<int>(p + idx);
		}
	}

	public void Set(int x, int y, Biome obj)
	{
		int idx = Index(x, y);
		writeBytes(idx, obj.Uid);
	}

	public Biome Get(int x, int y)
	{
		int idx = Index(x, y);
		int id = readBytes(idx);
		return ModRegistry.Biomes[id];
	}

	public void Foreach(XyForeach r)
	{
		for (int x = 0; x < Sx; x++)
			for (int y = 0; y < Sy; y++)
				r.Invoke(x, y);
	}

}
