using Spectrum.Core;
using Spectrum.Maths.Random;

namespace Ethla.Client.Audio;

public class MusicPlayer
{

	public float SecondsToSwitch = Seed.Global.NextFloat(1, 10);

	public void Tick()
	{
		SecondsToSwitch -= Time.Delta;

		if (SecondsToSwitch <= 0)
		{
			Music music = Seed.Global.Select(Music.Musics);
			if (music == null)
				return;

			SecondsToSwitch = music.PlayMusic() + Seed.Global.NextFloat(5, 120);
		}
	}

}
