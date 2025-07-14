using Ethla.Api;
using Ethla.Api.Grouping;
using Ethla.Api.Looting;
using Ethla.Client.Audio;
using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Ethla.World.Physics;
using Spectrum.Core.Manage;
using Spectrum.Maths.Random;

namespace Ethla.World.Voxel;

public class Block : Group
{

	public Item Item;
	public BlockProperty Property = new BlockProperty();
	public bool IsWall;

	public Dictionary<IntPair, BlockState> StatePaletteCache = new Dictionary<IntPair, BlockState>();
	
	public static Block Empty => ModRegistry.Blocks.DefaultValue;

	public BlockState MakeState(int meta = 0, int smeta = 0)
	{
		IntPair pair = IntPair.Create(meta, smeta);
		if (StatePaletteCache.TryGetValue(pair, out BlockState state))
			return state;
		return StatePaletteCache[pair] = new BlockState(this).SetMeta(meta).SetSpecialMeta(smeta);
	}

	public Block DeriveItem(Item item = null)
	{
		Item ip = item ?? new ItemBlock(this);
		Item = ModRegistry.Items.Register(Uid, ip);
		return this;
	}

	public override bool Contains<T>(T o)
	{
		return o is Block block && block == this;
	}

	//Instance Relative

	public virtual string GetLocalizationName(BlockState state)
	{
		return I18N.GetText($"{Uid.Space}:block.{Uid.Key}");
	}

	public virtual Sound GetSound(BlockState state, string type)
	{
		BlockMaterial material = GetMaterial(state);
		return material.GetNormalSound(type);
	}

	public virtual BlockMaterial GetMaterial(BlockState state)
	{
		return Property.Material;
	}

	// 1 means 1 second to break it with bare hands.
	public virtual float GetHardness(BlockState state)
	{
		return Property.Hardness;
	}

	public virtual bool CanBeReplace(BlockState state)
	{
		return state.IsEmpty || Property.CanBeReplace;
	}

	public virtual VoxelClip GetSilhouette(BlockState state, Entity bounder)
	{
		if (state.IsEmpty)
			return VoxelClip.Void;
		return Property.Silhouette;
	}

	//Attention: full cube block rendering will go wrong if this is enabled.
	public virtual bool IsWallAttached(BlockState state)
	{
		return Property.WallAttach;
	}

	//1 is keeping still, > 1 is pushing upwards, (0, 1) is not able to hold an entity, 0 is no effect.
	public virtual float GetFloatingForce(BlockState state, Entity bounder)
	{
		return Property.Floating;
	}

	public virtual float GetFrictionForce(BlockState state, Entity bounder)
	{
		return Property.Friction;
	}

	public virtual float GetPreventingForce(BlockState state, Entity bounder)
	{
		return Property.Preventing;
	}

	public virtual BlockShape GetShape(BlockState state)
	{
		return Property.Shape;
	}

	public virtual float CastLight(BlockState state, byte pipe, int x, int y)
	{
		return Property.Light[pipe];
	}

	public virtual float FilterLight(BlockState state, byte pipe, float v, int x, int y)
	{
		return GetShape(state).FilterLight(pipe, v, x, y);
	}

	public virtual BlockEntity CreateEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return null;
	}

	public virtual SlotLayout CreateCaridge(BlockPos pos, EntityPlayer player)
	{
		return null;
	}

	public virtual bool HasEntityBehavior(BlockState state, Level level, BlockPos pos)
	{
		return false;
	}

	public virtual bool HasCaridge(BlockPos pos, EntityPlayer player)
	{
		return false;
	}

	public virtual List<ItemStack> GetDrop(BlockState state, Level level, BlockPos pos)
	{
		List<ItemStack> list = new List<ItemStack>();

		Loot loot = LootManager.GetForBlock(Uid);

		if (loot == null)
		{
			if (!state.IsEmpty && state.Block.Item != null)
				list.Add(state.Block.Item.MakeStack());
		}
		else
		{
			loot.Generate(list, Seed.Global);
		}

		BlockEntity te = level.GetBlockEntity(pos);
		if (te != null) te.OverrideBlockDrops(list);

		return list;
	}

	public virtual BlockState GetStateForPlacing(Level level, Entity entity, Pos pos, ItemStack placerItem)
	{
		if (placerItem.Compound != null && placerItem.Compound.Try("place_meta", out int m))
			return MakeState(m);
		return MakeState();
	}

	public virtual void OnRandomTick(BlockState state, Level level, BlockPos pos)
	{
	}

	public virtual bool CheckSustainability(BlockState state, Level level, BlockPos pos)
	{
		return true;
	}

	public virtual void OnStepped(BlockState state, Entity entity)
	{
	}

	public virtual void OnDestroyed(BlockState state, DamageSource src, BlockPos pos)
	{
	}

}
