using System;
using System.Runtime.CompilerServices;
using Ethla.Api;

namespace Ethla.World.Voxel;

public unsafe class WallMap
{

    public delegate void XyForeach(int x, int y);

	private const int Sx = Chunk.Width;
	private const int Sy = Chunk.Height;
	public byte[] Bytes;
	public Wall Default;

	public int SizeX;
	public int SizeY;

	public WallMap()
	{
		Default = Wall.Empty;
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
			Unsafe.Write(p + idx, v);
	}

	protected int readBytes(int idx)
	{
		fixed (byte* p = Bytes)
			return Unsafe.Read<int>(p + idx);
	}

	protected byte readByte(int idx)
	{
		return Bytes[idx];
	}

	protected void writeByte(int idx, byte v)
	{
		Bytes[idx] = v;
	}

	public void Set(int x, int y, Wall obj)
	{
		int idx = Index(x, y);
		writeBytes(idx, obj.Uid);
	}

	public Wall Get(int x, int y)
	{
		int idx = Index(x, y);
		int id = readBytes(idx);
        return ModRegistry.Walls[id];
	}

	public void Cover(WallMap map)
	{
		Foreach((x, y) =>
		{
			int idx = Index(x, y);
			int id1 = readBytes(idx);

			if (id1 != 0) // 0 is default id.
			{
				map.writeBytes(idx, id1);
			}
		});
	}

	public void Foreach(XyForeach r)
	{
		for (int x = 0; x < Sx; x++)
			for (int y = 0; y < Sy; y++)
				r.Invoke(x, y);
	}

}
