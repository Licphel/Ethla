using Spectrum.Core.Input;
using Spectrum.Graphic;
using Spectrum.Maths;

namespace Ethla.Client;

public class TransformAbsolute
{

	public Camera Camera = new Camera();

	public ImVector2 Cursor = new ImVector2();
	public Vector4 Viewport;

	public void DoTransform(Graphics graphics)
	{
		float screenWidth = Surface.Current.Size.X;
		float screenHeight = Surface.Current.Size.Y;
		Cursor.Copy(Keyboard.Global.Cursor);

		Viewport = new Vector4(0, 0, screenWidth, screenHeight);
		graphics.Viewport(Viewport);

		Camera.ScaleX = Camera.ScaleY = 1;
		Camera.Viewport.Resize(screenWidth, screenHeight);
		Camera.ToCenter();
		Camera.Push();
		Cursor.X = Camera.ToWldX(Cursor.X, Viewport);
		Cursor.Y = Camera.ToWldY(Cursor.Y, Viewport);
	}

}
