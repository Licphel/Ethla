using Ethla.Api;
using Ethla.Util;
using Ethla.World.Iteming;
using Ethla.World.Lighting;
using Ethla.World.Mob;
using Spectrum.Core;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.IO.Bin;
using Spectrum.Maths.Shapes;

namespace Ethla.World.Physics;

public abstract partial class Entity : DamageSource
{

	public bool AddedToChunk;
	public ChunkUnit ChunkUnit;
	public float DeathProcessLength = 0.5f;
	public float DeathTimer;
	public float[] Glow = [0, 0, 0];
	public int InvCursor = 0;
	public Inventory Inventory; //May be null.
	public bool IsDead;
	public long LastTick;
	public Color Light;
	public float LiveTime;

	public bool Lowp;
	public Uuid OwnerUniqueId = new Uuid(0, 0);
	public Uuid UniqueId = Uuid.Generate();
	public virtual bool ShouldRemove => IsDead && DeathTimer >= 1;
	public Prefab Prefab => Prefab.Prefabs.GetValueOrDefault(GetType(), null);
	public Chunk Chunk => ChunkUnit.Chunk;

	public virtual string GetDamageSourceInfo(Damage damage)
	{
		return GetLocalizationName();
	}

	public virtual void Tick()
	{
		RegrabLight();
		LiveTime += Time.Delta;
	}

	public virtual void OnSpawned()
	{
		Prefab.MakePrefab(this);
	}

	public virtual void OnDespawned()
	{
	}

	public void Draw(Graphics graphics)
	{
		Quad smoothbox = new Quad();
		smoothbox.W = VisualSize.X;
		smoothbox.H = VisualSize.Y;
		smoothbox.Xcentral = Time.Lerp(PosLastTick.X, Pos.X);
		smoothbox.Ycentral = Time.Lerp(PosLastTick.Y, Pos.Y);
		Draw(graphics, smoothbox);
	}

	public virtual void Draw(Graphics graphics, Quad rect)
	{
	}

	public virtual float CastLight(byte pipe)
	{
		return Glow[pipe];
	}

	public virtual InterResult OnInteract(Entity other, Pos clickPos)
	{
		return InterResult.Pass;
	}

	public virtual string GetLocalizationName()
	{
		return I18N.GetText($"{Prefab.Uid.Space}:entity.{Prefab.Uid.Key}");
	}

	public virtual string GetPediaDesc()
	{
		return I18N.GetText($"{Prefab.Uid.Space}:pedia.entity.{Prefab.Uid.Key}");
	}

	//Codec

	public virtual void Write(BinaryCompound compound)
	{
		compound.Set("prefab", Prefab.Uid.ToString());
		compound.Set("livetime", LiveTime);
		compound.Set("x", Pos.X);
		compound.Set("y", Pos.Y);
		compound.Set("vx", Velocity.X);
		compound.Set("vy", Velocity.Y);
		compound.Set("uid1", UniqueId.Value1);
		compound.Set("uid2", UniqueId.Value2);
		compound.Set("ouid1", OwnerUniqueId.Value1);
		compound.Set("ouid2", OwnerUniqueId.Value2);
		compound.Set("collidable", IsCollidable);
		compound.Set("visual_w", VisualSize.X);
		compound.Set("visual_h", VisualSize.Y);
		compound.Set("mass", Mass);
		compound.Set("bound_w", Bound.W);
		compound.Set("bound_h", Bound.H);
		compound.Set("ignore_land_f", IgnoreLandFriction);
		compound.Set("slidable", IsSlidable);
		compound.Set("rebound_factor", ReboundFactor);
		compound.Set("airk", AirK);
		compound.Set("gmult", GravityMult);
	}

	public virtual void Read(BinaryCompound compound)
	{
		float x = compound.Get<float>("x");
		float y = compound.Get<float>("y");

		Locate(x, y);

		LiveTime = compound.Get<float>("livetime");
		Velocity.X = compound.Get<float>("vx");
		Velocity.Y = compound.Get<float>("vy");
		UniqueId = new Uuid(compound.Get<long>("uid1"), compound.Get<long>("uid1"));
		OwnerUniqueId = new Uuid(compound.Get<long>("ouid1"), compound.Get<long>("ouid2"));
		IsCollidable = compound.Get<bool>("collidable");
		VisualSize.X = compound.Get<float>("visual_w");
		VisualSize.Y = compound.Get<float>("visual_h");
		Mass = compound.Get<float>("mass");
		Bound.W = compound.Get<float>("bound_w");
		Bound.H = compound.Get<float>("bound_h");
		IgnoreLandFriction = compound.Get<bool>("ignore_land_f");
		IsSlidable = compound.Get<bool>("slidable");
		ReboundFactor = compound.Get<float>("rebound_factor");
		AirK = compound.Get<float>("airk");
		GravityMult = compound.Get<float>("gmult");
	}

	public static Entity ReadFromType(BinaryCompound compound)
	{
		if (compound == null)
			return null;

		Prefab prefab = ModRegistry.Prefabs[compound.Get<string>("prefab")];
		if (prefab == null)
			return null;

		Entity entity = prefab.Instantiate();
		entity.Read(compound);

		return entity;
	}

	public virtual void RegrabLight(bool force = false)
	{
		LightEngine engine = Level.LightEngine;

		if (engine.IsStableRoll || force)
		{
			Color vec = engine.GetLinearLight(Pos.X, Pos.Y);
			Light.R = Math.Max(vec.R, CastLight(LightPass.Red));
			Light.G = Math.Max(vec.G, CastLight(LightPass.Green));
			Light.B = Math.Max(vec.B, CastLight(LightPass.Blue));
			Light.A = 1;
		}
	}

	public void Die(bool blurOut = true)
	{
		IsDead = true;
		if (!blurOut)
			DeathTimer = 1;
	}

}
