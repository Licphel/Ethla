using Spectrum.Maths;

namespace Ethla.World;

public class Posing
{

	public static float Distance(Pos pos1, Pos pos2)
	{
		return Mathf.Sqrt(Distance2(pos1, pos2));
	}

	public static float Distance2(Pos pos1, Pos pos2)
	{
		return Mathf.Pow(pos1.X - pos2.X, 2) + Mathf.Pow(pos1.Y - pos2.Y, 2);
	}

	public static float PointRad(Pos pos1, Pos pos2)
	{
		return Mathf.AtanRad(pos2.Y - pos1.Y, pos2.X - pos1.X);
	}

	public static float PointDeg(Pos pos1, Pos pos2)
	{
		return Mathf.AtanDeg(pos2.Y - pos1.Y, pos2.X - pos1.X);
	}

	public static int ToCoord(int block)
	{
		return Mathf.FastFloor(block / 16f);
	}

}

//1 Chunk = 16 Block;
//1 Grid = 16 Pixel;
//1 Pixel => N Precise;
public interface Pos
{

	public float X { get; }
	public float Y { get; }
	public int BlockX { get; }
	public int BlockY { get; }
	public int UnitX { get; }
	public int UnitY { get; }

}

public struct PrecisePos : Pos
{

	public PrecisePos(Pos grid)
	{
		X = grid.X;
		Y = grid.Y;
	}

	public PrecisePos(float x, float y)
	{
		X = x;
		Y = y;
	}

	public float X { get; }
	public float Y { get; }
	public int BlockX => Mathf.FastFloor(X);
	public int BlockY => Mathf.FastFloor(Y);
	public int UnitX => Mathf.FastFloor(BlockX / 16f);
	public int UnitY => Mathf.FastFloor(BlockY / 16f);

	public PrecisePos Offset(float x, float y)
	{
		return new PrecisePos(X + x, Y + y);
	}

	public override bool Equals(object obj)
	{
		return obj is PrecisePos other && Equals(other);
	}

	public bool Equals(PrecisePos other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public static PrecisePos operator +(PrecisePos p1, PrecisePos p2)
	{
		return new PrecisePos(p1.X + p2.X, p1.Y + p2.Y);
	}

	public static PrecisePos operator -(PrecisePos p1, PrecisePos p2)
	{
		return new PrecisePos(p1.X - p2.X, p1.Y - p2.Y);
	}

	public static PrecisePos operator -(PrecisePos p)
	{
		return new PrecisePos(-p.X, -p.Y);
	}

	public static bool operator ==(PrecisePos p1, PrecisePos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(PrecisePos p1, PrecisePos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"%P[{X}, {Y}]";
	}

}

public struct BlockPos : Pos
{

	public BlockPos(Pos grid)
	{
		BlockX = grid.BlockX;
		BlockY = grid.BlockY;
	}

	public BlockPos(int x, int y)
	{
		BlockX = x;
		BlockY = y;
	}

	public float X => BlockX + 0.5f;
	public float Y => BlockY + 0.5f;
	public int BlockX { get; }
	public int BlockY { get; }
	public int UnitX => Mathf.FastFloor(BlockX / 16f);
	public int UnitY => Mathf.FastFloor(BlockY / 16f);

	public BlockPos Offset(int x, int y)
	{
		return new BlockPos(BlockX + x, BlockY + y);
	}

	public override bool Equals(object obj)
	{
		return obj is BlockPos other && Equals(other);
	}

	public bool Equals(BlockPos other)
	{
		return BlockX == other.BlockX && BlockY == other.BlockY;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(BlockX, BlockY);
	}

	public static BlockPos operator +(BlockPos p1, BlockPos p2)
	{
		return new BlockPos(p1.BlockX + p2.BlockX, p1.BlockY + p2.BlockY);
	}

	public static BlockPos operator -(BlockPos p1, BlockPos p2)
	{
		return new BlockPos(p1.BlockX - p2.BlockX, p1.BlockY - p2.BlockY);
	}

	public static BlockPos operator -(BlockPos p)
	{
		return new BlockPos(-p.BlockX, -p.BlockY);
	}

	public static bool operator ==(BlockPos p1, BlockPos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(BlockPos p1, BlockPos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"%B[{BlockX}, {BlockY}]";
	}

}

public struct UnitPos : Pos
{

	public UnitPos(Pos grid)
	{
		UnitX = grid.UnitX;
		UnitY = grid.UnitY;
	}

	public UnitPos(int x, int y)
	{
		UnitX = x;
		UnitY = y;
	}

	public float X => BlockX;
	public float Y => BlockY;
	public int BlockX => UnitX * 16;
	public int BlockY => UnitY * 16;
	public int UnitX { get; }
	public int UnitY { get; }

	public override bool Equals(object obj)
	{
		return obj is UnitPos other && Equals(other);
	}

	public bool Equals(UnitPos other)
	{
		return UnitX == other.UnitX && UnitY == other.UnitY;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(UnitX, UnitY);
	}

	public static UnitPos operator +(UnitPos p1, UnitPos p2)
	{
		return new UnitPos(p1.UnitX + p2.UnitX, p1.UnitY + p2.UnitY);
	}

	public static UnitPos operator -(UnitPos p1, UnitPos p2)
	{
		return new UnitPos(p1.UnitX - p2.UnitX, p1.UnitY - p2.UnitY);
	}

	public static UnitPos operator -(UnitPos p)
	{
		return new UnitPos(-p.UnitX, -p.UnitY);
	}

	public static bool operator ==(UnitPos p1, UnitPos p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(UnitPos p1, UnitPos p2)
	{
		return !p1.Equals(p2);
	}

	public override string ToString()
	{
		return $"%U[{UnitX}, {UnitY}]";
	}

}
