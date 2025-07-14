using System;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.Maths;

namespace Ethla.Common.Voxel;

public class WallBorder : Wall
{

    public override float CastLight(byte pipe, int x, int y)
	{
		return Mathf.SinRad(Time.Seconds + pipe + y * x) * 0.7f;
	}

}
