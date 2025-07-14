using Ethla.Api.Grouping;
using Ethla.Client.Audio;
using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Ethla.World.Physics;

namespace Ethla.World.Voxel;

public sealed class BlockState : DamageSource
{

	public static readonly BlockState Empty = Block.Empty.MakeState();

	//Special Metas
	public const byte Virtual = 1;
	public const byte Invisible = 2;
	
	public Block Block;
	public IntPair StateUid;

	public BlockState(Block block)
	{
		Block = block;
		SetMeta(0);
	}

	public bool IsEmpty => Block == Block.Empty;

	public string GetDamageSourceInfo(Damage damage)
	{
		return GetLocalizationName();
	}

	public int Meta { get; private set; }
	public int SpecialMeta;

	public BlockState SetMeta(int meta)
	{
		Meta = meta;
		StateUid = IntPair.Create(Block.Uid, Meta);
		return this;
	}

	public BlockState SetSpecialMeta(int smeta)
	{
		SpecialMeta = smeta;
		return this;
	}

	//Paletting Requirements

	public override bool Equals(object obj)
	{
		return obj is BlockState state && state.Block == Block && state.Meta == Meta;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Block, Meta);
	}

	//Comparison

	public bool Is(Group group)
	{
		return group.Contains(Block);
	}

	public bool Is(BlockState state)
	{
		return state.Is(Block);
	}

	public bool IsStrictly(BlockState state)
	{
		return state.Block == Block && state.Meta == Meta;
	}

	//Instance Relative

	public string GetLocalizationName()
	{
		return Block.GetLocalizationName(this);
	}

	public Sound GetSound(string type)
	{
		return Block.GetSound(this, type);
	}

	public BlockMaterial GetMaterial()
	{
		return Block.GetMaterial(this);
	}

	public float GetHardness()
	{
		return Block.GetHardness(this);
	}

	public bool CanBeReplace()
	{
		return Block.CanBeReplace(this);
	}

	public VoxelClip GetSilhouette(Entity bounder)
	{
		if (SpecialMeta == Virtual)
			return VoxelClip.Void;
		return Block.GetSilhouette(this, bounder);
	}

	public bool IsWallAttached()
	{
		return Block.IsWallAttached(this);
	}

	public float GetFloatingForce(Entity bounder)
	{
		return Block.GetFloatingForce(this, bounder);
	}

	public float GetFricitonForce(Entity bounder)
	{
		return Block.GetFrictionForce(this, bounder);
	}

	public float GetPreventingForce(Entity bounder)
	{
		return Block.GetPreventingForce(this, bounder);
	}

	public BlockShape GetShape()
	{
		return Block.GetShape(this);
	}

	public float CastLight(byte pipe, int x, int y)
	{
		return Block.CastLight(this, pipe, x, y);
	}

	public float FilterLight(byte pipe, float v, int x, int y)
	{
		return Block.FilterLight(this, pipe, v, x, y);
	}

	public BlockEntity CreateEntityBehavior(Level level, BlockPos pos)
	{
		return Block.CreateEntityBehavior(this, level, pos);
	}

	public SlotLayout CreateSlotLayout(BlockPos pos, EntityPlayer player)
	{
		return Block.CreateCaridge(pos, player);
	}

	public bool HasEntityBehavior(Level level, BlockPos pos)
	{
		return Block.HasEntityBehavior(this, level, pos);
	}

	public bool HasSlotLayout(BlockPos pos, EntityPlayer player)
	{
		return Block.HasCaridge(pos, player);
	}

	public List<ItemStack> GetDrop(Level level, BlockPos pos)
	{
		return Block.GetDrop(this, level, pos);
	}

	public void OnRandomTick(Level level, BlockPos pos)
	{
		Block.OnRandomTick(this, level, pos);
	}

	public bool CheckSustainability(Level level, BlockPos pos)
	{
		return Block.CheckSustainability(this, level, pos);
	}

	public void OnStepped(Entity entity)
	{
		Block.OnStepped(this, entity);
	}

	public void OnDestroyed(DamageSource src, BlockPos pos)
	{
		Block.OnDestroyed(this, src, pos);
	}

}
