using Ethla.Client;
using Ethla.World.Mob.Ai;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityWorm : Creature
{

	private static readonly Animation Animation = new Animation(Images.EntityWorm, 4, 4, 4, 0, 0);

	public EntityWorm()
	{
		Health = MaxHealth = 5;
		Mind.Activities.Add(new ActivityWander());
	}

	public override void Tick()
	{
		base.Tick();

		Move();
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		Animation.Seconds(0.35f - Math.Min(0.27f, Math.Abs(Velocity.X / 20f)));
		graphics.Color4(Light);
		if (HurtCooldown > 0)
			graphics.Merge4(1, 0.3f, 0.3f);
		graphics.Merge4(1, 1 - DeathTimer, 1 - DeathTimer, 1 - DeathTimer);
		graphics.DrawIcon(Animation, rect);
		graphics.NormalizeColor();
	}

}
