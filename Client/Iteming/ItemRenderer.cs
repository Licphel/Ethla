using Ethla.World.Iteming;
using Spectrum.Graphic;
using Spectrum.Maths.Shapes;

namespace Ethla.Client.Iteming;

public interface ItemRenderer
{

	public static ItemRenderer Normal = new ItemRendererNormal();
	public static ItemRenderer Wand = new ItemRendererWand();

	public void Draw(Graphics graphics, float x, float y, float w, float h, ItemStack stack, bool overlay = true, bool forcecount = false);

	public void Draw(Graphics graphics, Quad rect, ItemStack stack, bool overlay = true)
	{
		Draw(graphics, rect.X, rect.Y, rect.W, rect.H, stack, overlay);
	}

}
