using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Maths;

namespace Ethla.Util;

public struct Ray
{

	public const float Step = 0.5f;

	public static bool IsBlocked(Level level, Pos pos, Pos dest, float maxlen = float.MaxValue, float maxblocks = 1)
	{
		float dst = Posing.Distance(pos, dest);

		if (dst > maxlen)
			return true;

		float tan = Posing.PointDeg(pos, dest);
		float d2 = maxlen * maxlen;
		float ct = 0;

		for (int i = 0; i < dst / Step; i++)
		{
			float x = pos.X + i * Step * Mathf.CosDeg(tan);
			float y = pos.Y + i * Step * Mathf.SinDeg(tan);
			Pos pos1 = new PrecisePos(x, y);

			if (Posing.Distance2(pos1, pos) > d2)
				break;

			BlockState state = level.GetBlock(pos1);
			if (state.GetShape().IsFull && new BlockPos(pos1) != new BlockPos(dest))
				ct += Step;
		}

		return ct > maxblocks;
	}

}
