using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureTreeMaple : Feature
{

	private BlockState Logs = Blocks.MapleLog.MakeState(0, BlockState.Virtual);
	private BlockState Leaves = Blocks.MapleLeaves.MakeState(0, BlockState.Virtual);

	public FeatureTreeMaple()
	{
		IsSurfacePlaced = true;
		TryTimesPerChunk = 1;
	}

	public override bool IsPlacable(Level level, int x, int y, Seed seed)
	{
		int times = 0;

		for (int i = -5; i < 5; i++)
			for (int j = 1; j < 10; j++)
				if (level.GetBlock(x + i, y + j).GetShape().IsFull)
					times++;

		return times < 10 && level.GetBlock(x, y).Is(Groups.Dirt);
	}

	public override void Place(Level level, int x, int y, Seed seed)
	{
		y++;
		int h = seed.NextInt(3, 8);
		for (int i = 0; i < h; i++)
			level.SetBlock(Logs, x, y + i);

		for (int i = h; i < h + 2; i++)
			for (int j = -4; j < 4 + 1; j++)
				level.SetBlock(Leaves, x + j, y + i);

		for (int i = h + 2; i < h + 4; i++)
			for (int j = -3; j < 3 + 1; j++)
				level.SetBlock(Leaves, x + j, y + i);

		for (int i = h + 4; i < h + 6; i++)
			for (int j = -4; j < 4 + 1; j++)
				level.SetBlock(Leaves, x + j, y + i);

		for (int i = h + 6; i < h + 8; i++)
			for (int j = -2; j < 2 + 1; j++)
				level.SetBlock(Leaves, x + j, y + i);

		for (int i = h + 8; i < h + 9; i++)
			for (int j = -1; j < 1 + 1; j++)
				level.SetBlock(Leaves, x + j, y + i);

		if (2 < h - 5)
		{
			if (seed.NextFloat() < 0.2f)
				placeBranch(level, x, y + seed.NextInt(2, h - 5), seed, true);
			if (seed.NextFloat() < 0.2f)
				placeBranch(level, x, y + seed.NextInt(2, h - 5), seed, false);
		}

		bool right = seed.Next();
		if (seed.NextFloat() < 0.2f)
			placeBranch(level, x, y + h - 2, seed, right);
	}

	private void placeBranch(Level level, int x, int y, Seed seed, bool right)
	{
		if (right)
		{
			level.SetBlock(Logs, x + 1, y);
			level.SetBlock(Logs, x + 2, y + 1);
			level.SetBlock(Leaves, x + 1, y + 2);
			level.SetBlock(Leaves, x + 2, y + 2);
			level.SetBlock(Leaves, x + 3, y + 2);
			level.SetBlock(Leaves, x + 2, y + 3);
		}
		else
		{
			level.SetBlock(Logs, x - 1, y);
			level.SetBlock(Logs, x - 2, y + 1);
			level.SetBlock(Leaves, x - 1, y + 2);
			level.SetBlock(Leaves, x - 2, y + 2);
			level.SetBlock(Leaves, x - 3, y + 2);
			level.SetBlock(Leaves, x - 2, y + 3);
		}
	}

}
