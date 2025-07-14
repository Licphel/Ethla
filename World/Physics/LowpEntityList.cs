using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Maths.Random;

namespace Ethla.World.Physics;

public class LowpEntityList
{

	private readonly Level level;
	private readonly List<Entity> lows = new List<Entity>();

	public LowpEntityList(Level level)
	{
		this.level = level;
	}

	public int Count => lows.Count;

	public void Add(Entity particle, Pos pos)
	{
		particle.Level = level;
		particle.Lowp = true;
		particle.Locate(pos, false);
		particle.OnSpawned();
		lows.Add(particle);
	}

	public void AddSpreading(Entity particle, Pos pos, float r)
	{
		float rx = Seed.Global.NextFloat(-r, r);
		float ry = Seed.Global.NextFloat(-r, r);
		PrecisePos ppos = new PrecisePos(pos.X + rx, pos.Y + ry);
		Add(particle, ppos);
	}

	public void Tick()
	{
		for (int i = lows.Count - 1; i >= 0; i--)
		{
			Entity p = lows[i];

			if (p == null)
				continue;

			if (p.IsDead)
			{
				p.DeathTimer += Time.Delta / p.DeathProcessLength;
				if (p.ShouldRemove)
				{
					lows.RemoveAt(i);
				}
			}

			p.Tick();
		}
	}

	public void Draw(Graphics graphics)
	{
		for (int i = lows.Count - 1; i >= 0; i--)
		{
			Entity p = lows[i];

			if (p == null)
				continue;

			p.Draw(graphics);
		}
	}

}
