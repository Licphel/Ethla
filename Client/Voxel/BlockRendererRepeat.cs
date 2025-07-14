using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class BlockRendererRepeat : BlockRendererSingle
{

	//Only support 16 * n x 16 * n texture.
	protected override void drawInternal(Graphics graphics, BlockState state, Direction overlapping, float x, float y, float w, float h)
	{
		Icon icon = BlockModels.GetIcon(state);

		float v = 0;
		float u = 0;

		if (overlapping == Direction.Down)
			v += 14;
		else if (overlapping == Direction.Up)
			v -= 14;
		else if (overlapping == Direction.Right)
			u += 14;
		else if (overlapping == Direction.Left)
			u -= 14;

		if (icon is Image part)
		{
			int fw = part.Width / 16;
			int fh = part.Height / 16;
			u += Math.Abs(x) % fw * 16;
			v += Math.Abs(y) % fh * 16;
			u += part.U;
			v += part.V;
			graphics.DrawImage(part.Reference, x, y, w, h, u, v, 16, 16);
		}
		else if (icon is Image tex)
		{
			int fw = tex.Width / 16;
			int fh = tex.Height / 16;
			u += Math.Abs(x) % fw * 16;
			v += Math.Abs(y) % fh * 16;
			graphics.DrawImage(tex, x, y, w, h, u, v, 16, 16);
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
