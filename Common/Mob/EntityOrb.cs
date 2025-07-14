using Ethla.Client;
using Ethla.World.Lighting;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityOrb : Entity
{

	private static readonly Animation Anim = new Animation(Images.EntityOrb, 4, 16, 16, 0, 0).Seconds(0.25f);

	public EntityOrb()
	{
		AirK = 0;
		GravityMult = 0;
	}

	public override void Tick()
	{
		base.Tick();

		Move();

		List<Entity> lst = Level.GetNearbyEntities(Bound, typeof(Creature));
		lst.ForEach(e =>
		{
			Creature c = (Creature)e;
			if (c.UniqueId == OwnerUniqueId)
				return;
			c.Hit(new Damage(this, DamageTypes.Elemental, 20));
			Die();
		});

		if (LiveTime > 20)
			Die();
	}

	public override float CastLight(byte pipe)
	{
		return LightPass.Switch3(pipe, 0.75f, 0.25f, 0.25f);
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		graphics.Color4(1, 1, 1, 1 - DeathTimer);
		graphics.DrawIcon(Anim, rect);
		graphics.NormalizeColor();
	}

}
