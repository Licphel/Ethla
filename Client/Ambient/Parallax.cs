using Ethla.World;
using Ethla.World.Lighting;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;

namespace Ethla.Client.Ambient;

public class Parallax
{

	public const int Nx = 24, Ny = 16;
	public Color[,] Cache;
	public Color[,,] Cache1;
	public Image Image;
	public bool LightImpact;
	public float Opacity;

	public float Resist;

	public void Draw(Graphics graphics, ImVector2 delta, float secs, float msecs, float spf, Level level, Pos pos)
	{
		const float exp = 16;

		Vector2 size = Surface.Current.Size;
		float disX = delta.X * (1 - Resist) * exp;
		float disY = -delta.Y * (1 - Resist) * exp;
		float ratio = size.Y / 256;
		float disX0 = disX / ratio;
		float disY0 = disY / ratio;

		if (LightImpact)
		{
			Camera cam = Main.Camera;
			Vector4 vp = Bootstrap.TransformWorld.Viewport;
			LightEngine engine = level.LightEngine;

			float blockx = size.X / Nx, blocky = size.Y / Ny;
			float tx = blockx / ratio;
			float ty = blocky / ratio;

			for (int x = 0; x < Nx; x++)
			{
				float px = blockx * x;
				float wx = cam.ToWldX(px, vp);
				float dx = disX0 + px / ratio;

				for (int y = 0; y < Ny; y++)
				{
					float py = blocky * y;
					float wy = cam.ToWldY(py, vp);

					graphics.VertexColors[0] = Cache1[x, y, 0];
					graphics.VertexColors[1] = Cache1[x, y, 1];
					graphics.VertexColors[2] = Cache1[x, y, 2];
					graphics.VertexColors[3] = Cache1[x, y, 3];
					graphics.DrawImage(Image, px, py, blockx, blocky, dx, disY0 - py / ratio, tx, ty);
				}
			}
		}
		else
		{
			Color sunlight = CelestComputation.BgSunlight(secs, msecs);
			graphics.Color4(sunlight);
			spf = 1 - spf;
			graphics.Merge4(spf, spf, spf, spf * Opacity);
			float w = 512 * ratio;
			float h = 256 * ratio;
			float w0 = size.X / ratio;
			float h0 = size.Y / ratio;
			graphics.DrawImage(Image, 0, disY - (1 - Resist) * h, size.X, h, disX0, 0, w0, 256);
		}

		graphics.NormalizeColor();
	}

	public virtual void Tick(Level level, Pos pos)
	{
		if (!LightImpact || level == null)
			return;

		Vector2 size = Surface.Current.Size;
		Camera cam = Main.Camera;
		Vector4 vp = Bootstrap.TransformWorld.Viewport;
		LightEngine engine = level.LightEngine;

		float blockx = size.X / Nx, blocky = size.Y / Ny;

		for (int x = 0; x < Nx; x++)
		{
			float px = blockx * x;
			float wx = cam.ToWldX(px, vp);

			for (int y = 0; y < Ny; y++)
			{
				float py = blocky * y;
				float wy = cam.ToWldY(py, vp);

				if (engine.IsStableRoll)
				{
					Color col = engine.GetLinearLight(wx, wy);
					ref Color rcol = ref Cache[x, y];
					rcol.R += (col.R - rcol.R) * 0.5f;
					rcol.G += (col.G - rcol.G) * 0.5f;
					rcol.B += (col.B - rcol.B) * 0.5f;
				}

				int xm1 = (x - 1 + Nx) % Nx;
				int xp1 = (x + 1 + Nx) % Nx;
				int ym1 = (y - 1 + Ny) % Ny;
				int yp1 = (y + 1 + Ny) % Ny;

				Cache1[x, y, 0] = new Color(Cache[x, y] / 4 + Cache[xm1, ym1] / 4 + Cache[xm1, y] / 4 + Cache[x, ym1] / 4, Opacity);
				Cache1[x, y, 1] = new Color(Cache[x, y] / 4 + Cache[xm1, yp1] / 4 + Cache[x, yp1] / 4 + Cache[xm1, y] / 4, Opacity);
				Cache1[x, y, 2] = new Color(Cache[x, y] / 4 + Cache[xp1, yp1] / 4 + Cache[xp1, y] / 4 + Cache[x, yp1] / 4, Opacity);
				Cache1[x, y, 3] = new Color(Cache[x, y] / 4 + Cache[xp1, ym1] / 4 + Cache[xp1, y] / 4 + Cache[x, ym1] / 4, Opacity);
			}
		}
	}

}
