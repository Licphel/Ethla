using Ethla.Client.Voxel;
using Ethla.World.Iteming;
using Ethla.World.Voxel;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Graphic.Text;
using Spectrum.Maths;

namespace Ethla.Client.Iteming;

public class ItemRendererNormal : ItemRenderer
{

	public virtual void Draw(Graphics graphics, float x, float y, float w, float h, ItemStack stack, bool overlay = true, bool forcecount = false)
	{
		graphics.PushColor();
		if (!stack.IsEmpty)
		{
			Icon ico = GetCutIcon(stack);
			if (ico == null && stack.Item is ItemBlock blockp)
			{
				Block block = blockp.GetBlockPlaced(stack);
				BlockState state = block.MakeState();
				BlockModels.GetRenderer(state).DrawItemSymbol(graphics, state, x, y, w, h);
			}
			else if (ico == null && stack.Item is ItemWall wallp)
			{
				Wall wall = wallp.GetWallPlaced(stack);
				WallModels.GetRenderer(wall).DrawItemSymbol(graphics, wall, x, y, w, h);
			}
			else
			{
				graphics.DrawIcon(ico, x, y, w, h);
			}

			if (overlay)
			{
				/*
				int dr = stack.getBasicDurability();
				int scale = Mth.round(w / 16f);

				if(dr >= 0)
				{
				    int lf = stack.getDurabilityLeft();
				    float p = (float) lf / dr;
				    if(p != 1)
				    {
				        Mth.hsvToRGB(p / 3, 0.8f, 0.9f, rgb);
				        graphics.color4f(0f, 0f, 0f, 1f);
				        graphics.colorFill(x + scale, y + scale, 14 * scale, scale);
				        graphics.color4f(rgb, 1f);
				        graphics.colorFill(x + scale, y + scale, 14 * p * scale, scale);
				        graphics.normalizeColor();
				    }
				}
				*/

				int count = stack.Count;
				float scale = Mathf.Round(w / 16f);

				if (count > 1 || forcecount)
				{
					string str = count >= 1000 ? "-" : count.ToString();

					graphics.NormalizeColor();
					graphics.DrawText(str, x + w + scale * 2, y - scale, FontAlign.Right);
				}
			}
		}

		graphics.PopColor();
	}

	public virtual Icon GetCutIcon(ItemStack stack)
	{
		return ItemModels.GetIcon(stack);
	}

}
