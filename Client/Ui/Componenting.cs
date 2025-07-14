using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;

namespace Ethla.Client.Ui;

public class Componenting
{

	public static Button NewTextedButton(Lore text, float w, Action click)
	{
		Button button = new Button();
		button.Text = text;
		button.Bound.Resize(w, 12);
		if (click != null) button.OnLeftFired += click;
		button.Icons = [Images.ButtonP1, Images.ButtonP2, Images.ButtonP3];
		button.TextOffset.Y = 3f;
		return button;
	}

	public static TextField NewTextField(string text, float w, float h)
	{
		TextField field = new TextField();
		field.InputHint = text;
		field.Bound.Resize(w, h);
		field.Icons = [Images.ButtonP1, Images.ButtonP2, Images.ButtonP3];
		return field;
	}

}
