using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Messaging;

public static class MessageManager
{

	public static List<Message> Messages = new List<Message>();
	public static float LiveSeconds = 5;

	public static void Send(Message msg)
	{
		Messages.Insert(0, msg);
	}

	public static void Tick()
	{
		for (int i = Messages.Count - 1; i >= 0; i--)
		{
			Message msg = Messages[i];
			msg.Seconds += Time.Delta;
			if (msg.Seconds >= LiveSeconds)
			{
				float t = 1 - msg.Opacity;
				msg.Opacity -= 0.1f * Mathf.SinRad(t * 2 / (float)Math.PI) + 0.01f;
				if (msg.Opacity <= 0.01f)
					Messages.RemoveAt(i);
			}
		}
	}

	public static void Draw(Graphics graphics)
	{
		float mwid = Resolution.Latest.Xsize / 2;
		float x = 2;
		float y = Resolution.Latest.Ysize / 8;

		foreach (Message msg in Messages)
		{
			GlyphBounds gb = graphics.Font.GetBounds(msg.Lore, mwid);
			x -= (1 - msg.Opacity) * gb.Width;
			graphics.Color4(0, 0, 0, 0.25f);
			graphics.DrawRect(x - 2, y, gb.Width + 4, gb.Height);
			graphics.NormalizeColor();
			graphics.DrawLore(msg.Lore, x, y, mwid);

			y += gb.Height;
		}
	}

}
