using Ethla.Client.Ambient;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public class GuiMenu : Gui
{

	private readonly Button[] buttons = new Button[5];

	public override void Draw(Graphics graphics)
	{
		Sky.DrawCelesph(graphics);

		graphics.DrawImage(Images.GuiMenuTitle, Size.X / 2 - 64, Size.Y - 112, 128, 64);

		base.Draw(graphics);
	}

	public override void InitComponents()
	{
		base.InitComponents();

		float x = Size.X / 2f;
		float y = Size.Y - 140;
		const float bw = 150;

		buttons[0] = Componenting.NewTextedButton(Lore.Translate("ethla:button.menu_new_game"), bw, () =>
		{
			Bootstrap.RootPath.Find("save").Delete();
			Close();
			Main.Load();
			new OverlayMain().Display();
		});
		Join(buttons[0]);
		buttons[1] = Componenting.NewTextedButton(Lore.Translate("ethla:button.menu_load_game"), bw, () =>
		{
			Close();
			Main.Load();
			new OverlayMain().Display();
		});
		Join(buttons[1]);
		buttons[2] = Componenting.NewTextedButton(Lore.Translate("ethla:button.menu_options"), bw, () => { });
		Join(buttons[2]);
		buttons[3] = Componenting.NewTextedButton(Lore.Translate("ethla:button.menu_mod_mngmt"), bw, () => { });
		Join(buttons[3]);
		buttons[4] = Componenting.NewTextedButton(Lore.Translate("ethla:button.menu_quit_game"), bw, () => { Application.Stop(); });
		Join(buttons[4]);
	}

	public override void RelocateComponents()
	{
		for (int i = 0; i < buttons.Length; i++) buttons[i].Bound.LocateCentral(Size.X / 2f, Size.Y - 120 - i * 15);
	}

	public override void Tick(ImVector2 cursor)
	{
		base.Tick(cursor);
		Sky.TickStaticSky();
	}

}
