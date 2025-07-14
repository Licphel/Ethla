using Spectrum.Core;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public class GuiLoading : Gui
{

	private static readonly Image LogoImage = Loads.Get("ethla:image/misc/logo.png");
	private readonly LoadingQueue loader;

	public GuiLoading(LoadingQueue loader)
	{
		this.loader = loader;
	}

	public override void Draw(Graphics graphics)
	{
		base.Draw(graphics);

		float value = Mathf.SinDeg(loader.Progress * 90);
		graphics.Color4(1, 1, 1, value);
		graphics.DrawImage(LogoImage, Size.X / 2 - 64, Size.Y / 2 - 32, 128, 64);

		graphics.NormalizeColor();
		graphics.DrawText($"Preparing: {loader.Run} / {loader.Total}", Size.X / 2, 5, FontAlign.Center);
		graphics.DrawRect(0, 0, Size.X * loader.Progress, 0.5f);
	}

	public override void Tick(ImVector2 cursor)
	{
		base.Tick(cursor);

		if (!loader.Done)
		{
			float time = Time.Microsecs;
			while (Time.Microsecs - time < 1000f / Application.MaxTps)
				loader.Next();
		}
		else
		{
			loader.EndTask();

			Close();
			Bootstrap.LoadDone.Call();
			new GuiMenu().Display(true);
		}
	}

}
