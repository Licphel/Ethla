using Ethla.Common;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.IO.Bin;
using Spectrum.Maths;

namespace Ethla.World.Mob.Ai;

public abstract class Creature : Entity
{

	public Damage CausingDamage;

	public float Health, MaxHealth;
	public float Hunger, MaxHunger;
	public int HurtCooldown;
	public float Mana, MaxMana;

	public CreatureMind Mind = new CreatureMind();
	public float Thirst, MaxThirst;

	public override void Tick()
	{
		base.Tick();

		Mind.Tick(this);

		HurtCooldown--;
		if (HurtCooldown < 0) HurtCooldown = 0;

		if (TimeSchedule.PeriodicTask(LiveTime, 1))
		{
			Hunger -= 0.05f;
			Thirst -= 0.1f;

			if (Hunger <= 0)
			{
				Hit(new Damage(null, DamageTypes.Biotic, 1), false);
				Hunger = 0;
			}

			if (Thirst <= 0)
			{
				Hit(new Damage(null, DamageTypes.Biotic, 1), false);
				Thirst = 0;
			}
		}
	}

	public virtual float Hit(Damage value, bool cd = true)
	{
		float v = value.Value;

		if (HurtCooldown <= 0)
		{
			Health -= v;
			if (Health <= Mathf.Tolerance)
			{
				Health = 0;
				IsDead = true;
				CausingDamage = value;
			}

			if (cd)
				HurtCooldown = 5;

			OnHit(value);
		}

		return Health;
	}

	public virtual void OnHit(Damage value) {}

	public override float CastLight(byte pipe)
	{
		if (Inventory == null || Inventory.Count <= 0)
			return 0;
		return Inventory[InvCursor].CastLight(pipe);
	}

	public override void Read(BinaryCompound compound)
	{
		base.Read(compound);

		Health = compound.Get<int>("health");
		MaxHealth = compound.Get<int>("max_health");
		Mana = compound.Get<int>("mana");
		MaxMana = compound.Get<int>("max_mana");
		Hunger = compound.Get<int>("hunger");
		MaxHunger = compound.Get<int>("max_hunger");
		Thirst = compound.Get<int>("thirst");
		MaxThirst = compound.Get<int>("max_thirst");
		HurtCooldown = compound.Get<int>("hcd");
	}

	public override void Write(BinaryCompound compound)
	{
		base.Write(compound);

		compound.Set("health", Health);
		compound.Set("max_health", MaxHealth);
		compound.Set("mana", Mana);
		compound.Set("max_mana", MaxMana);
		compound.Set("hunger", Hunger);
		compound.Set("max_hunger", MaxHunger);
		compound.Set("thirst", Thirst);
		compound.Set("max_thirst", MaxThirst);
		compound.Set("hcd", HurtCooldown);
	}

}
