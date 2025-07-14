using Ethla.Client.Iteming;
using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World.Iteming;
using Spectrum.Graphic;
using Spectrum.Graphic.Text;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public partial class OverlayMain
{

	private const float W = 209f;
	private const float H = 20;
	private const float Lptr = 0;
	private const float Dptr = 0;
	private const float Lptri = 1;
	private const float Dptri = 1;
	private const float St = 20;
	private const float Off = 20;

	public void DrawPStates(Graphics graphics, EntityPlayer player)
	{
		float w = 34, h = 12, spc = w + 1;
		float bx = (Size.X - W) / 2;
		float y = H + 4;
		float v = 70;

		DrawBar(player.Health, player.MaxHealth, bx, y);
		DrawBar(player.Mana, player.MaxMana, bx + spc, y);
		DrawBar(player.Hunger, player.MaxHunger, bx + spc * 2, y);
		DrawBar(player.Thirst, player.MaxThirst, bx + spc * 3, y);

		void DrawBar(float p, float pm, float xB, float yB)
		{
			float per = Mathf.SafeDiv(p, pm);
			graphics.DrawImage(Images.Hotbar, xB, yB, w, h, 0, v, w, h);
			graphics.DrawImage(Images.Hotbar, xB + 1, yB + 1, 9, (h - 2) * per, w + 1, v + 1, 9, (h - 2) * per);

			string str = NumberForm.GetCompress(Mathf.Ceiling(p));
			graphics.DrawText(str, xB + 23, yB + 2, FontAlign.Center);
			v += h + 1;
		}
	}

	public void DrawHotBarAndHint(Graphics graphics, EntityPlayer player)
	{
		float bx = (Size.X - W) / 2;
		float by = 2;

		graphics.DrawImage(Images.Hotbar, bx, by, W, H, 0, 0, W, H);
		graphics.DrawImage(Images.Hotbar, bx + Lptr + (St + 1) * player.InvCursor, by + Dptr, St, St, 0, Off, St, St);

		for (int i = 0; i <= 9; i++)
		{
			ItemStack stack = player.Inventory.Get(i);
			ItemModels.GetRenderer(stack).Draw(graphics, bx + (Lptri + 1) + (St + 1) * i, by + (Dptri + 1), 16, 16, stack);
		}

		/*
		Block block = Main.HoverBlockState.Block;

		if (block != Block.Empty)
		{
			string? name = Main.HoverBlockState.GetLocalizationName();
			string? fromw = block.Uid.Space;
			GlyphBounds gb = graphics.Font.GetBounds(name);
			GlyphBounds gb1 = graphics.Font.GetBounds(fromw);
			float frw = Math.Max(gb.Width, gb1.Width) + 8;
			float frh = gb.Height + gb1.Height + 6;

			graphics.DrawIcon(DefaultTooltipPatches, (Size.x - frw) / 2, Size.y - frh - 6, frw, frh);

			graphics.DrawText(name, Size.x / 2, Size.y - frh + 5, FontAlign.Center);
			graphics.Color4(0.7f, 0.5f, 1, 0.75f);
			graphics.DrawText(fromw, Size.x / 2, Size.y - frh - 3, FontAlign.Center);
			graphics.NormalizeColor();
		}
		*/
	}

}
