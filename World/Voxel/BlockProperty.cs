using Ethla.Common;

namespace Ethla.World.Voxel;

public class BlockProperty
{

	public bool CanBeReplace;
	public float Floating = 0.0f;
	public float Friction = 1.0f;

	public float Hardness = 0.0f;
	public float[] Light = [0, 0, 0];
	public BlockMaterial Material = BlockMaterials.Unknown;
	public float Preventing = 0.0f;
	public int Rarity;
	public BlockShape Shape = BlockShape.Solid;
	public VoxelClip Silhouette = VoxelClip.Cube;
	public bool WallAttach;

}
