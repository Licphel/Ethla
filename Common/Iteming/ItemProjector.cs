using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Common.Iteming;

public class ItemProjector : Item
{

	public Item Ammo;
	public Func<Prefab> CastObject;
	public float Cooldown;
	public Vector2 PowerRange;

	public ItemProjector(Item ammo, Vector2 powerRange, Func<Prefab> castObject, float cd)
	{
		Ammo = ammo;
		PowerRange = powerRange;
		CastObject = castObject;
		Cooldown = cd;
	}

	public override int GetStackSize(ItemStack stack)
	{
		return 1;
	}

	public override bool IsRemoteUsage(ItemStack stack)
	{
		return true;
	}

	public override InterResult OnUseItem(ItemStack stack, Entity entity, Level level, Pos pos, bool sim = false)
	{
		if (entity.Inventory == null)
			return InterResult.Pass;
		if (Time.Seconds - stack.EnsuredCompound.Get<float>("cooldown") < Cooldown)
			return InterResult.Pass;
		if (Ammo != Empty && entity.Inventory.Extract(1, s => s.Is(Ammo), sim).Count == 0)
			return InterResult.Pass;

		if (!sim)
		{
			float deg = Posing.PointDeg(entity.Pos, pos);
			Entity e = CastObject().Instantiate();
			e.OwnerUniqueId = entity.UniqueId;
			e.Locate(entity.Pos);
			float power = Seed.Global.NextGaussian(PowerRange.X, PowerRange.Y);
			e.Velocity = new ImVector2(power * Mathf.CosDeg(deg), power * Mathf.SinDeg(deg));
			level.SpawnEntity(e);

			stack.EnsuredCompound.Set("cooldown", Time.Seconds);
		}

		return InterResult.Blocked;
	}

}
