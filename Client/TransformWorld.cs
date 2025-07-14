using Spectrum.Core.Input;
using Spectrum.Graphic;
using Spectrum.Maths;

namespace Ethla.Client;

public class TransformWorld
{

	public ImVector2 Cursor = new ImVector2();
	public Vector4 Viewport;

	public void DoTransform(Graphics graphics, Camera camera)
	{
		float screenWidth = Surface.Current.Size.X;
		float screenHeight = Surface.Current.Size.Y;

		float rt = Options.Ratio;
		float fw, fh;

		if (screenWidth / screenHeight > rt)
		{
			fw = screenWidth;
			fh = screenWidth / rt;
		}
		else
		{
			fh = screenHeight;
			fw = screenHeight * rt;
		}

		float x0 = (screenWidth - fw) / 2, y0 = (screenHeight - fh) / 2;

		Cursor.Copy(Keyboard.Global.Cursor);
		Viewport = new Vector4(x0, y0, fw, fh);
		camera.Viewport.Locate(x0, y0);
		camera.Viewport.Resize(fw, fh);
		graphics.Viewport(Viewport);
		Cursor.X = camera.ToWldX(Cursor.X, Viewport);
		Cursor.Y = camera.ToWldY(Cursor.Y, Viewport);
	}

}
