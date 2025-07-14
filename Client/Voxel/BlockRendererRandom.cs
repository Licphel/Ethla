using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class BlockRendererRandom : BlockRendererSingle
{

	//Only support 16 * n x 16 * n texture.
	protected override void drawInternal(Graphics graphics, BlockState state, Direction overlapping, float x, float y, float w, float h)
	{
		Icon icon = BlockModels.GetIcon(state);

		if (icon is Image part)
			graphics.DrawImage(part.GetTrueImageSurface(), x, y, w, h, part.U, part.V + PosToRand(x, y, part.Height), 16, 16);
		else if (icon is Image tex)
			graphics.DrawImage(tex, x, y, w, h, 0, PosToRand(x, y, tex.Height), 16, 16);

		return;

		int PosToRand(float x, float y, int h)
		{
			long i = (long)(x * 31) + (long)(y * 17);
			return (int)(Math.Abs(i) % (h >> 4)) << 4;
		}
	}

	protected override void drawItemSymbolInternal(Graphics graphics, BlockState state, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockModels.GetIcon(state);

		if (icon is Image part)
			graphics.DrawImage(part.Reference, x, y, w, h, part.U, part.V, 16, 16);
		else if (icon is Image tex) graphics.DrawImage(tex, x, y, w, h, 0, 0, 16, 16);
	}

}
