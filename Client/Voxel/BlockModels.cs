using Ethla.Util;
using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class BlockModels
{

	public static readonly Dictionary<IntPair, BlockRenderData> DataMap = new Dictionary<IntPair, BlockRenderData>();

	public static BlockRenderer GetRenderer(BlockState state)
	{
		return DataMap[state.StateUid].Renderer;
	}

	public static Icon GetIcon(BlockState state)
	{
		return DataMap[state.StateUid].Icon;
	}

	public static Image GetMask(BlockState state)
	{
		return DataMap[state.StateUid].Mask;
	}

	public static bool IsConnectable(BlockState inst, BlockState other, Direction direction)
	{
		return other.GetShape().IsFull && inst.GetShape().IsFull;
	}

	public static bool IsSpreadable(BlockState inst, BlockState other, Direction direction)
	{
		return !other.IsStrictly(inst);
	}

	public class BlockRenderData
	{

		public Icon Icon;
		public Image Mask;

		public BlockRenderer Renderer;

	}

}
