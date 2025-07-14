using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Client.Ambient;

public class CelestStar
{

	private static readonly Seed StarSeed = new SeedXoroshiro();
	public float MaxOpacity;
	public float Opacity;
	public float OpacityOffs;
	public bool Removed;
	public float Size;

	public float X, Y;

	public CelestStar(float x, float y)
	{
		X = x;
		Y = y;
		Size = 3;
		Opacity = StarSeed.NextFloat();
		OpacityOffs = StarSeed.NextInt(128);
		MaxOpacity = StarSeed.NextFloat(0.75f, 1f) - StarSeed.NextFloat(0, 0.25f);
	}

	public void Draw(Graphics graphics, float daytime, float space)
	{
		float opa1 = Math.Clamp(Opacity - daytime + space, 0, 1) * (Mathf.SinRad(Time.Seconds + X) * 0.5f + 0.25f);
		graphics.Color4(1, 1, 1, opa1);
		graphics.DrawRect(X - Size / 2, Y - Size / 2, Size, Size);
		graphics.NormalizeColor();
	}

}
