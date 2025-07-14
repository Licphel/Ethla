namespace Ethla.Util;

public class IntPair
{

	public int A, B;
	private int hash0;

	public static IntPair Create(int a, int b)
	{
		return new IntPair { A = a, B = b, hash0 = HashCode.Combine(a, b) };
	}

	public override bool Equals(object? obj)
	{
		return obj is IntPair other && A == other.A && B == other.B;
	}

	public override int GetHashCode()
	{
		return hash0;
	}

}
