namespace Ethla.World.Lighting;

public class LightPass
{

	public static byte Red = 0, Green = 1, Blue = 2;

	public static float Switch3(byte pass, float r, float g, float b)
	{
		if (pass == Red) return r;
		if (pass == Green) return g;
		return b;
	}

}
