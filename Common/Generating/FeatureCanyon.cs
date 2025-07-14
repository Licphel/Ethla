using System;
using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Common.Generating;

public class FeatureCanyon : Feature
{

    public FeatureCanyon()
    {
        TryTimesPerChunk = 0.08f;
        IsSurfacePlaced = true;
    }

    public override bool IsPlacable(Level level, int x, int y, Seed seed)
    {
        return true;
    }

    public override void Place(Level level, int x, int y, Seed seed)
    {
        BlockState state = level.GetBlock(x, y);

        if (!state.Is(Groups.Carvable))
            return;

        int dep = seed.NextGaussianInt(16, 64);

        float j = 0;
        int el = 0;

        for (int i = 0; i < dep; i++)
        {
            int y1 = y - i;
            el++;

            if (seed.NextFloat() < 0.2f * el)
            {
                j += seed.NextFloat(-1f, 1f);
                el = 1;
            }

            int x1 = x + Mathf.Round(j);

            if (level.GetBlock(x1, y1).IsEmpty)
                break;

            level.SetBlock(BlockState.Empty, x1, y1);

            if (seed.NextFloat() < (dep - i) * 0.015f * el)
            {
                level.SetBlock(BlockState.Empty, x1 - 1, y1);
            }

            if (seed.NextFloat() < (dep - i) * 0.015f * el)
            {
                level.SetBlock(BlockState.Empty, x1 + 1, y1);
            }
        }
    }

}
