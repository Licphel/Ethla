using Ethla.Api;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class BlockMaterials
{

	public static readonly IdMap<BlockMaterial> Registry = ModRegistry.BlockMaterials;

	public static BlockMaterial Unknown = Registry.RegisterDefaultValue("ethla:unknown", new BlockMaterial());
	public static BlockMaterial Powder = Registry.Register("ethla:powder", new BlockMaterial());
	public static BlockMaterial Rock = Registry.Register("ethla:rock", new BlockMaterial());
	public static BlockMaterial Metal = Registry.Register("ethla:metal", new BlockMaterial());
	public static BlockMaterial Wooden = Registry.Register("ethla:wooden", new BlockMaterial());
	public static BlockMaterial Glass = Registry.Register("ethla:glass", new BlockMaterial());
	public static BlockMaterial Foliage = Registry.Register("ethla:foliage", new BlockMaterial());

}
