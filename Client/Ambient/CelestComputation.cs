using Ethla.World;
using Spectrum.Graphic;
using Spectrum.Maths;

namespace Ethla.Client.Ambient;

public class CelestComputation
{

	public static float BodyRadians(float secs, float msecs)
	{
		return (float)Math.PI * 2 / msecs * (secs % msecs) + 0.75f;
	}

	public static float Cos(float secs, float msecs)
	{
		float i = Mathf.CosRad(BodyRadians(secs, msecs)) * 0.5f + 0.5f;
		return Math.Clamp(i, 0, 1);
	}

	public static float Sin(float secs, float msecs)
	{
		float i = Mathf.SinRad(BodyRadians(secs, msecs)) * 0.5f + 0.5f;
		return Math.Clamp(i, 0, 1);
	}

	public static float SpaceFactor(Pos grid)
	{
		if (grid.Y <= Chunk.YOfSpace) return 0;
		float dy = (grid.Y - Chunk.YOfSpace) * 2.25f;
		return Math.Clamp(dy / (Chunk.Height - Chunk.YOfSpace), 0, 1);
	}

	public static Color ColorOfSky(float secs, float mSecs, Pos pos)
	{
		float f = Cos(secs, mSecs);
		float f1 = Sin(secs, mSecs);

		const float tempNow = 0.34f;
		Color rgb0 = Color.HsvToRgb(0.6f - tempNow * 0.05f - (f - 0.5f) * 0.1f, 0.15f + (1 - f) * 0.4f + tempNow * 0.1f, 0.98f);

		rgb0 *= f1;
		rgb0 *= 1.05f - SpaceFactor(pos);

		return new Color(rgb0.R + 0.03f, rgb0.G + 0.03f, rgb0.B + 0.05f);
	}

	public static void RecolorDuskAndDawn(float day, Color[] colors, ref Color color0)
	{
		float i = Math.Abs(day - 0.45f);
		const float len = 0.4f;
		if (i >= len)
			i = 0;
		else
			i = len - i;
		float per = i / len * 2;
		float rx = 1;
		float gx = 1 - per * 0.45f;
		float bx = 1 - per * 0.2f;

		colors[0] = colors[1] = colors[2] = colors[3] = color0;
		colors[0] *= new Color(rx, gx, bx);
		colors[1] *= new Color(0.85f, 0.9f, 1f);
		colors[2] *= new Color(0.85f, 0.9f, 1f);
		colors[3] *= new Color(rx, gx, bx);
	}

	public static Color BgSunlight(float secs, float msecs)
	{
		float f1 = Sin(secs, msecs);
		f1 = Math.Clamp(f1, 0.1f, 1f);
		return new Color(f1, f1, f1);
	}

	public static Color LightingSunlight(Level level, float hard = 1)
	{
		float f1 = Sin(level.Ticks, level.TicksPerDay) + 0.25f;
		float i = -f1 + 0.8f;
		return new Color(Math.Clamp(f1 - i * 0.15f * hard, 0, 1.1f), Math.Clamp(f1 - i * 0.1f * hard, 0, 1.1f), Math.Clamp(f1 + i * 0.15f * hard, 0, 1.1f));
	}

}
