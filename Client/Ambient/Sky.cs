using Ethla.World;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Client.Ambient;

public class Sky
{

	protected static Sky StaticSky = new Sky();
	protected static TransformAbsolute Transformation = new TransformAbsolute();
	public static float Secs = 30;
	public static float Msecs = 120;
	public static List<SkyLayer> SkyLayers = new List<SkyLayer>();

	private float dayTime;
	public ImVector2 Delta = new ImVector2();
	public float ExpectedStarSizeBase = 128;
	public bool JustFlushed;
	public Level Level; //Nullable
	private int lw, lh;
	public List<CelestStar> Stars = new List<CelestStar>();

	static Sky()
	{
		SkyLayers.Add(new SkyLayer().AddLayer(Images.CeBg3, false, 0.975f).AddLayer(Images.CeBg1, false, 0.925f).AddLayer(Images.CeBg2, false, 0.85f).SetVisible((l, p) => p.Y >= Chunk.YOfSea));
		SkyLayers.Add(new SkyLayer().AddLayer(Images.UCeBg2, true, 0.95f).AddLayer(Images.UCeBg1, true, 0.85f).SetVisible((l, p) => p.Y < Chunk.YOfSea));
	}

	public Sky(Level level = null)
	{
		Level = level;
	}

	public void CenterMoved(float x, float y)
	{
		Delta.X = x;
		Delta.Y = y;
	}

	public void Tick(Pos pos)
	{
		int w = Surface.Current.Size.Xi;
		int h = Surface.Current.Size.Yi;

		JustFlushed = false;

		if (lw != w || lh != h)
		{
			Flush();
			JustFlushed = true;
		}

		lw = w;
		lh = h;

		for (int i = SkyLayers.Count - 1; i >= 0; i--)
		{
			SkyLayer sl = SkyLayers[i];
			sl.Tick(Level, pos);
		}

		int av = (w + h) / 2;

		for (int i = 0; Stars.Count < ExpectedStarSizeBase * (av / 300f); i++)
			Stars.Add(new CelestStar(Seed.Global.NextFloat(0, w), Seed.Global.NextFloat(0, h)));
	}

	public void Flush()
	{
		Stars.Clear();
	}

	public void Draw(Graphics graphics, float secs, float msecs, Pos pos)
	{
		int w = Surface.Current.Size.Xi;
		int h = Surface.Current.Size.Yi;

		dayTime = CelestComputation.Sin(secs, msecs);

		Color temp = CelestComputation.ColorOfSky(secs, msecs, pos);
		CelestComputation.RecolorDuskAndDawn(dayTime, graphics.VertexColors, ref temp);

		graphics.DrawRect(0, 0, w, h);
		graphics.NormalizeColor();

		float spf = CelestComputation.SpaceFactor(pos); //Space factor.

		for (int i = Stars.Count - 1; i >= 0; i--)
		{
			CelestStar star = Stars[i];
			star.Draw(graphics, dayTime, spf);
		}

		float ctx = w / 2f, cty = h / 8f;
		float deg = Mathf.Deg(CelestComputation.BodyRadians(secs, msecs));

		Vector2 vec = new Vector2();
		vec.SetDeg(h, deg);
		vec *= new Vector2(1.25f, 0.75f);
		float sx = ctx + vec.X, sy = cty + vec.Y;

		float scl = Resolution.Latest.Factor / 2;

		graphics.DrawImage(Images.CeBody1, sx - 32 * scl, sy - 32 * scl, 64 * scl, 64 * scl);

		sx = ctx - vec.X;
		sy = cty - vec.Y;

		graphics.Color4(1, 1, 1, 0.75f);
		graphics.DrawImage(Images.CeBody2, sx - 32 * scl, sy - 32 * scl, 64 * scl, 64 * scl);
		graphics.NormalizeColor();

		for (int i = SkyLayers.Count - 1; i >= 0; i--)
		{
			SkyLayer sl = SkyLayers[i];
			sl.Draw(graphics, Delta, secs, msecs, spf, Level, pos);
		}

		graphics.NormalizeColor();
	}

	public static void DrawCelesph(Graphics graphics)
	{
		Transformation.DoTransform(graphics);
		graphics.UseCamera(Transformation.Camera);
		StaticSky.Draw(graphics, Secs, Msecs, new PrecisePos(0, Chunk.YOfSurface));
		graphics.UseCamera(Bootstrap.TransformGui.Camera);
	}

	public static void TickStaticSky()
	{
		Secs += Time.Delta * 3;
		StaticSky.CenterMoved(Time.Seconds, 0);
		StaticSky.Tick(new PrecisePos(0, Chunk.YOfSurface));
	}

}
