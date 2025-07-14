using Ethla.Client.Iteming;
using Ethla.World;
using Ethla.World.Iteming;
using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public class GuiSlotLayout : Gui
{

	public Image Background;

	public SlotLayout Layout;
	public float I, J;
	public float W, H;

	public Lore Name;

	public GuiSlotLayout(SlotLayout layout, Image icon, Vector2 used, Lore name)
	{
		Layout = layout;
		Background = icon;
		W = used.X;
		H = used.Y;
		Name = name;
	}

	public override void Reflush()
	{
		I = (int)(Size.X / 2 - W / 2);
		J = (int)(Size.Y / 2 - H / 2);

		//pass i, j into component-adding
		base.Reflush();
	}

	public override List<Lore> CollectTooltips()
	{
		List<Lore> lst = base.CollectTooltips();

		if (Layout.ChosenSlot != null)
			Layout.ChosenSlot.Stack.GetTooltips(lst);

		return lst;
	}

	public void DrawSlots(Graphics graphics)
	{
		Slot chosenSlot = Layout.ChosenSlot;

		foreach (Slot slot in Layout.Slots)
		{
			if (chosenSlot == slot)
				graphics.DrawImage(Images.SlotChosen, I + slot.X, J + slot.Y, 20, 20);

			int ofx = 2;

			ItemModels.GetRenderer(slot.Stack).Draw(graphics, I + slot.X + ofx, J + slot.Y + ofx, 16, 16, slot.Stack);
		}
	}

	public override void Draw(Graphics graphics)
	{
		graphics.Color4(0, 0, 0, Math.Clamp(OpenSeconds * 1.6f, 0, 0.6f));
		graphics.DrawRect(0, 0, Size.X, Size.Y);
		graphics.NormalizeColor();

		graphics.DrawImage(Background, I, J, W, H, 0, 0, W, H);
		graphics.DrawLore(Name, I + W / 2, J + H - 10, FontAlign.Center);
		DrawSlots(graphics);

		base.Draw(graphics);

		ItemModels.GetRenderer(Layout.Pickup).Draw(graphics, Cursor.X - 8, Cursor.Y - 8, 16, 16, Layout.Pickup);
	}

	public override void Tick(ImVector2 cursor)
	{
		base.Tick(cursor);

		float xc = cursor.X, yc = cursor.Y;
		xc -= I;
		yc -= J;

		Layout.ChosenSlot = null;

		foreach (Slot slot in Layout.Slots)
		{
			int x = slot.X, y = slot.Y;
			if (xc >= x && xc <= x + 20 && yc >= y && yc <= y + 20)
			{
				Layout.ChosenSlot = slot;
				break;
			}
		}

		if (OpenSeconds <= 0.05f)
			return; // Avoid mistake op

		SlotAction action = SlotAction.None;
		bool outside = xc < 0 || yc < 0 || xc > W || yc > W;

		if (KeyBind.MouseLeft.Holding() && KeyBind.KeyShift.Holding()) action = SlotAction.FastTransfer;

		if (KeyBind.MouseLeft.Pressed() && !KeyBind.KeyShift.Holding())
		{
			action = SlotAction.SwapPickup;

			if (outside)
				action = SlotAction.ThrowAll;
		}

		if (KeyBind.MouseRight.Pressed())
		{
			action = SlotAction.HalfPickup;

			if (outside)
				action = SlotAction.ThrowOne;
		}

		Layout.DoAction(action);

		if (KeyBind.KeyEsc.Pressed() || KeyBind.KeyF.Pressed())
		{
			Close();
			SlotLayout.CloseGracefully(Layout.Player);
		}
	}

	public override void OnClosed()
	{
		base.OnClosed();
	}

}
