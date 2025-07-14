namespace Ethla.Util;

public class Tier
{

	public static Tier None = new Tier(0, new Multiplier());

	public int Level;
	public Multiplier Multiplier;

	public Tier(int level, in Multiplier mul)
	{
		Level = level;
		Multiplier = mul;
	}

}
