using Ethla.World;
using Ethla.World.Lighting;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;

namespace Ethla.Client.Voxel;

public class LiquidRenderer
{

	public static void Draw(Graphics graphics, Level level, Chunk chunk, int x, int y)
	{
		LiquidStack stack = chunk.LiquidMap.Get(x, y);

		if (stack.Amount == 0 || stack.Liquid == Liquid.Empty)
			return;

		LightWare lights = chunk.GetSubLightware(x, y);

		if (lights.IsDark && !Main.GodMode)
			return;

		BlockState bl = level.GetBlock(x - 1, y);
		BlockState bu = level.GetBlock(x, y + 1);
		BlockState bd = level.GetBlock(x, y - 1);
		BlockState br = level.GetBlock(x + 1, y);

		LiquidStack stacku = chunk.LiquidMap.Get(x, y + 1);
		LiquidStack stackd = chunk.LiquidMap.Get(x, y - 1);

		Image img = LiquidModels.GetImage(stack.Liquid);
		Image imged = LiquidModels.GetImageEdge(stack.Liquid);

		float p = stack.Percent;

		if (stacku.Amount == 0 && (p < 1 || bu.GetShape() != BlockShape.Solid))
			p += 0.025f * Mathf.SinRad(x * 0.5f + y * 0.05f + Time.Seconds * 3);

		float h = p;

		lights.GetBlockLights(graphics.VertexColors, false);

		const float overhang = 1 / 16f;
		float u = Time.Seconds * 2;
		float v = Time.Seconds * 0.5f;

		graphics.DrawImage(img, x, y, 1, h, u, v + 16 * (1 - h), 16, 16 * h);
		if (bl.GetShape() == BlockShape.Solid)
			graphics.DrawImage(img, x - overhang, y, overhang, h, u + 15, v, 1, 16);
		if (br.GetShape() == BlockShape.Solid)
			graphics.DrawImage(img, x + 1, y, overhang, h, u, v, 1, 16);
		if (bu.GetShape() == BlockShape.Solid && h >= 1)
			graphics.DrawImage(img, x, y + 1, 1, overhang, u, v, 16, 1);
		if (bd.GetShape() == BlockShape.Solid)
			graphics.DrawImage(img, x, y - overhang, 1, overhang, u, v + 15, 16, 1);
		if (!bu.GetShape().IsFull && stacku.Amount == 0 || h < 1)
			graphics.DrawImage(imged, x, y + h - 1 / 16f, 1, 2 / 16f, u, 0, 16, 2);


		graphics.NormalizeColor();
	}

}
