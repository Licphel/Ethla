namespace Ethla.Util;

public struct Multiplier
{

	public float Percent;
	public float Value;

	public Multiplier()
	{
		Percent = 0;
		Value = 0;
	}

	public Multiplier(float percent = 0, float value = 0)
	{
		Percent = percent;
		Value = value;
	}

	public override string ToString()
	{
		return $"{Percent * 100}% " + (Value > 0 ? "+" + Value : Value);
	}

	public static Multiplier operator +(Multiplier m1, Multiplier m2)
	{
		return new Multiplier(m1.Percent + m2.Percent, m1.Value + m2.Value);
	}

	public static float operator *(float v, Multiplier m)
	{
		return v * (m.Percent + 1) + m.Value;
	}

}
