using Ethla.World;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;

namespace Ethla.Client.Ambient;

public class SkyLayer
{

	public float Opacity;

	public List<Parallax> ParallaxList = new List<Parallax>();
	public Func<Level, Pos, bool> VisibilityCondition = (level, pos) => true;

	public SkyLayer SetVisible(Func<Level, Pos, bool> cond)
	{
		VisibilityCondition = cond;
		return this;
	}

	public SkyLayer AddLayer(Image img, bool light, float resist)
	{
		Parallax p = new Parallax();
		p.Resist = resist;
		p.Image = img;
		p.LightImpact = light;

		if (light)
		{
			p.Cache = new Color[Parallax.Nx + 1, Parallax.Ny + 1];
			p.Cache1 = new Color[Parallax.Nx + 1, Parallax.Ny + 1, 4];
		}

		ParallaxList.Add(p);

		return this;
	}

	public void Draw(Graphics graphics, ImVector2 delta, float secs, float msecs, float spf, Level level, Pos pos)
	{
		if (Opacity <= 0)
			return;
		foreach (Parallax p in ParallaxList)
			p.Draw(graphics, delta, secs, msecs, spf, level, pos);
	}

	public void Tick(Level level, Pos pos)
	{
		if (VisibilityCondition(level, pos))
			Opacity = Math.Clamp(Opacity + 0.04f, 0, 1);
		else
			Opacity = Math.Clamp(Opacity - 0.04f, 0, 1);

		foreach (Parallax p in ParallaxList)
		{
			if (Opacity > 0)
				p.Tick(level, pos);
			p.Opacity = Opacity;
		}
	}

}
