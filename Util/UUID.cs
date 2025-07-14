using Spectrum.Maths.Random;

namespace Ethla.Util;

public readonly struct Uuid
{

	public static Uuid Empty = new Uuid(0, 0);

	public readonly long Value1, Value2;

	public Uuid(long value1, long value2)
	{
		Value1 = value1;
		Value2 = value2;
	}

	public static Uuid Generate()
	{
		return new Uuid(DateTime.Now.Ticks, Seed.Global.NextInt(0, int.MaxValue));
	}

	public override bool Equals(object? obj)
	{
		return obj is Uuid uuid && equals(uuid);
	}

	private bool equals(Uuid other)
	{
		return Value1 == other.Value1 && Value2 == other.Value2;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Value1, Value2);
	}

	public static bool operator ==(Uuid p1, Uuid p2)
	{
		return p1.equals(p2);
	}

	public static bool operator !=(Uuid p1, Uuid p2)
	{
		return !p1.equals(p2);
	}

}
