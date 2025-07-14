using Ethla.Common.Mob;
using Ethla.World;
using Ethla.World.Lighting;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class LiquidLava : Liquid
{

	public override float GetFriction()
	{
		return 21.33f;
	}

	public override float GetDensity()
	{
		return 42.8f;
	}

	public override float CastLight(byte pipe, int x, int y, int am)
	{
		return LightPass.Switch3(pipe, 0.75f, 0.35f, 0.3f);
	}

	public override void OnEntityCollided(Entity entity, float intersection)
	{
		base.OnEntityCollided(entity, intersection);

		if (entity is EntityItem)
			entity.Die();

		float damage = intersection * 5;

		if (entity is Creature c)
			c.Hit(new Damage(this, DamageTypes.Flaming, damage));
	}

	public override void OnTouch(LiquidStack stack, LiquidStack other, Level level, int x, int y, int x1, int y1)
	{
		if (other.Liquid == Liquids.Water)
		{
			level.SetBlock(Blocks.Obsidian.MakeState(), x1, y1);
			level.SetLiquid(new LiquidStack(stack.Liquid, stack.Amount - other.Amount), x, y);
		}
	}

}
