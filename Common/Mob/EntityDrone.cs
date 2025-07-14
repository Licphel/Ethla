using Ethla.Client;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityDrone : Creature
{

	public EntityDrone()
	{
		Bound.Resize(12 / 16f, 12 / 16f);
		VisualSize.X = 16 / 16f;
		VisualSize.Y = 16 / 16f;
		Health = MaxHealth = 10000;
		GravityMult = 0;
	}

	public override void Tick()
	{
		base.Tick();

		Move();

		if (TimeSchedule.PeriodicTask(LiveTime, 1))
		{
			Quad b1 = Bound;
			b1.Expand(20, 20);
			List<Entity> lst = Level.GetNearbyEntities(Bound, typeof(Creature));
			lst.ForEach(e =>
			{
				Creature c = (Creature)e;
				if (c == this)
					return;
				if (c.UniqueId == OwnerUniqueId)
					return;
				EntityOrb eo = Prefabs.MagicOrb.Instantiate();
				eo.OwnerUniqueId = UniqueId;
				float deg = Mathf.AtanDeg(-Pos.Y + e.Pos.Y, -Pos.X + e.Pos.X);
				eo.Velocity.FromDeg(15, deg);
				Level.SpawnEntity(eo, Pos);
			});
		}
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		graphics.Color4(Light);
		if (HurtCooldown > 0)
			graphics.Merge4(1, 0.3f, 0.3f);
		graphics.DrawImage(Images.EntityDrone, rect, 0, 0, 16, 16);
		graphics.NormalizeColor();
		graphics.DrawImage(Images.EntityDrone, rect, 0, 16, 16, 16);
	}

}
