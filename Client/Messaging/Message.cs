using Spectrum.Graphic.Text;

namespace Ethla.Client.Messaging;

public class Message
{

	public float Opacity = 1;

	public float Seconds;

	public Message(Lore lore)
	{
		Lore = lore;
	}

	public Lore Lore { get; set; }

}
