using Ethla.Common;
using Ethla.World.Voxel;

namespace Ethla.World.Iteming;

public class ItemToolType
{

	public static Dictionary<string, ItemToolType> ToolTypeMap = new Dictionary<string, ItemToolType>();

	public static ItemToolType None = new ItemToolType("none");
	public static ItemToolType Pickaxe = new ItemToolType("pickaxe");
	public static ItemToolType Axe = new ItemToolType("axe");
	public static ItemToolType Shovel = new ItemToolType("shovel");
	public static ItemToolType Omni = new ItemToolType("omni");
	public static ItemToolType Hammer = new ItemToolType("hammer");

	private readonly HashSet<BlockMaterial> materialsDiggable = new HashSet<BlockMaterial>();

	static ItemToolType()
	{
		Pickaxe.AddTargetMaterial(BlockMaterials.Rock, BlockMaterials.Glass, BlockMaterials.Metal);
		Axe.AddTargetMaterial(BlockMaterials.Wooden, BlockMaterials.Foliage);
		Shovel.AddTargetMaterial(BlockMaterials.Powder);
	}

	public ItemToolType(string name)
	{
		ToolTypeMap[name] = this;
	}

	public void AddTargetMaterial(params BlockMaterial[] mat)
	{
		foreach (BlockMaterial mat1 in mat)
			materialsDiggable.Add(mat1);
	}

	public virtual bool IsTarget(BlockMaterial mat)
	{
		if (this == Omni)
			return true;
		return materialsDiggable.Contains(mat);
	}

	public virtual bool ShouldHarvestWall()
	{
		return this == Hammer;
	}

}
