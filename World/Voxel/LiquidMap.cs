using System.Runtime.CompilerServices;
using Ethla.Api;

namespace Ethla.World.Voxel;

public unsafe class LiquidMap
{

	public delegate void XyForeach(int x, int y);

	private const int Sx = Chunk.Width;
	private const int Sy = Chunk.Height;
	public byte[] Bytes;
	public Liquid Default;

	public LiquidMap()
	{
		Default = Liquid.Empty;
		Bytes = new byte[Sx * Sy * (sizeof(int) + 1)];
	}

	public int Index(int x, int y)
	{
		x &= Sx - 1;
		y &= Sy - 1;
		return (x * Sy + y) * (sizeof(int) + 1);
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

	protected byte readByte(int idx)
	{
		return Bytes[idx];
	}

	protected void writeByte(int idx, byte v)
	{
		Bytes[idx] = v;
	}

	public void Set(int x, int y, LiquidStack obj)
	{
		int idx = Index(x, y);
		writeBytes(idx, obj.Liquid.Uid);
		writeByte(idx + sizeof(int), (byte)obj.Amount);
	}

	public LiquidStack Get(int x, int y)
	{
		int idx = Index(x, y);
		int id = readBytes(idx);
		int a = readByte(idx + sizeof(int));
		Liquid lq = ModRegistry.Liquids[id];
		LiquidStack stack = new LiquidStack(lq, a);
		return stack;
	}

	public void Cover(LiquidMap map)
	{
		Foreach((x, y) =>
		{
			int idx = Index(x, y);
			int id1 = readBytes(idx);

			if (id1 != 0) // 0 is default id.
			{
				int meta = readByte(idx + sizeof(int));
				map.writeBytes(idx, id1);
				map.writeByte(idx + sizeof(int), (byte)meta);
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
