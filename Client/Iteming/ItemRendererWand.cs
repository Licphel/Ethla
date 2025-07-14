using Ethla.World.Iteming;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Iteming;

public class ItemRendererWand : ItemRenderer
{

	public void Draw(Graphics graphics, float x, float y, float w, float h, ItemStack stack, bool overlay = true, bool forcecount = false)
	{
		int type = stack.EnsuredCompound.GetOrDefault("type", 0);
		int gem = stack.EnsuredCompound.GetOrDefault("gem", 0);
		int deco = stack.EnsuredCompound.GetOrDefault("deco", 0);

		int t0 = type * 16;
		int g0 = gem * 16;
		int d0 = deco * 16;

		Image img = (Image)ItemModels.GetIcon(stack);

		graphics.DrawImage(img, x, y, w, h, 0, t0, 16, 16);
		graphics.DrawImage(img, x, y, w, h, 16, g0, 16, 16);
		graphics.DrawImage(img, x, y, w, h, 32, d0, 16, 16);
	}

}
