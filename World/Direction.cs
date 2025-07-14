using Spectrum.Maths;

namespace Ethla.World;

public class Direction : IComparable<Direction>
{

	public static readonly Direction Left = new Direction(-1, 0, 180);
	public static readonly Direction Right = new Direction(1, 0, 0);
	public static readonly Direction Up = new Direction(0, 1, 90);
	public static readonly Direction Down = new Direction(0, -1, 270);

	public static readonly Direction[] Values = { Left, Right, Up, Down };
	public float Deg;

	public Vector2 Offset;
	public float Rad;

	private Direction(int ox, int oy, int deg)
	{
		Offset = new Vector2(ox, oy);
		Deg = deg;
		Rad = Mathf.Rad(deg);
	}

	public Direction Cw
	{
		get
		{
			if (this == Right)
				return Down;
			if (this == Left)
				return Up;
			if (this == Up)
				return Right;
			if (this == Down) return Left;

			return null;
		}
	}

	public Direction Ccw
	{
		get
		{
			if (this == Right)
				return Up;
			if (this == Left)
				return Down;
			if (this == Up)
				return Left;
			if (this == Down) return Right;

			return null;
		}
	}

	public Direction Opposite
	{
		get
		{
			if (this == Right)
				return Left;
			if (this == Left)
				return Right;
			if (this == Up)
				return Down;
			if (this == Down) return Up;

			return null;
		}
	}

	public int CompareTo(Direction other)
	{
		return Deg.CompareTo(other.Deg);
	}

	public static Direction operator ++(Direction d)
	{
		return d.Cw;
	}

	public static Direction operator --(Direction d)
	{
		return d.Ccw;
	}

	public static Direction operator !(Direction d)
	{
		return d.Opposite;
	}

	public BlockPos Step(BlockPos pos)
	{
		return pos.Offset(Offset.Xi, Offset.Yi);
	}

	public PrecisePos Step(Pos pos)
	{
		return new PrecisePos(pos.X + Offset.Xi, pos.Y + Offset.Yi);
	}

}
