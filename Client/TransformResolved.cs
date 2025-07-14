using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Graphic;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client;

public class TransformResolved
{

	public Camera Camera = new Camera();

	public ImVector2 Cursor = new ImVector2();
	public Vector4 Viewport;

	static TransformResolved()
	{
		Application.Resize.Add("ethla:resize_matf", () =>
		{
			for (int i = 0; i < Gui.Viewings.Count; i++)
			{
				Gui gui = Gui.Viewings[i];
				new Resolution(gui);
			}
		});
	}

	public void DoTransform(Gui gui, Graphics graphics)
	{
		float screenWidth = Surface.Current.Size.X;
		float screenHeight = Surface.Current.Size.Y;
		Cursor.Copy(Keyboard.Global.Cursor);

		Viewport = new Vector4(0, 0, screenWidth, screenHeight);
		graphics.Viewport(Viewport);

		Camera.ScaleX = Camera.ScaleY = 1;
		Camera.Viewport.Resize(gui.Resolution.Xsize, gui.Resolution.Ysize);
		Camera.ToCenter();
		Camera.Push();
		Cursor.X = Camera.ToWldX(Cursor.X, Viewport);
		Cursor.Y = Camera.ToWldY(Cursor.Y, Viewport);
	}

}
