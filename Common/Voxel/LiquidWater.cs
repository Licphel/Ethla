using Ethla.World;
using Ethla.World.Voxel;

namespace Ethla.Common.Voxel;

public class LiquidWater : Liquid
{

	public override void OnTouch(LiquidStack stack, LiquidStack other, Level level, int x, int y, int x1, int y1)
	{
		if (other.Liquid == Liquids.Lava)
		{
			level.SetBlock(Blocks.Obsidian.MakeState(), x1, y1);
			level.SetLiquid(new LiquidStack(stack.Liquid, stack.Amount - other.Amount), x, y);
		}
	}

}
