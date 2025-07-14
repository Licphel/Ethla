using System.Runtime.CompilerServices;
using Ethla.Api;

namespace Ethla.World.Voxel;

public unsafe class BlockMap
{

	public delegate void XyForeach(int x, int y);

	private const int Sx = Chunk.Width;
	private const int Sy = Chunk.Height;
	public byte[] Bytes;
	public BlockState Default;

	public int SizeX;
	public int SizeY;

	public BlockMap()
	{
		Default = Block.Empty.MakeState();
		Bytes = new byte[Sx * Sy * (sizeof(int) + 2)];
	}

	public int Index(int x, int y)
	{
		x &= Sx - 1;
		y &= Sy - 1;
		return (x * Sy + y) * (sizeof(int) + 2);
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

	public void Set(int x, int y, BlockState obj)
	{
		int idx = Index(x, y);
		writeBytes(idx, obj.Block.Uid);
		writeByte(idx + sizeof(int), (byte)obj.Meta);
		writeByte(idx + sizeof(int) + 1, (byte)obj.SpecialMeta);
	}

	public BlockState Get(int x, int y)
	{
		int idx = Index(x, y);
		int id = readBytes(idx);
		int meta = readByte(idx + sizeof(int));
		int smeta = readByte(idx + sizeof(int) + 1);
		Block block = ModRegistry.Blocks[id];
		BlockState state = block.MakeState(meta, smeta);
		return state;
	}

	public void Cover(BlockMap map)
	{
		Foreach((x, y) =>
		{
			int idx = Index(x, y);
			int id1 = readBytes(idx);

			if (id1 != 0) // 0 is default id.
			{
				int meta = readByte(idx + sizeof(int));
				int smeta = readByte(idx + sizeof(int) + 1);
				map.writeBytes(idx, id1);
				map.writeByte(idx + sizeof(int), (byte)meta);
				map.writeByte(idx + sizeof(int) + 1, (byte)smeta);
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
