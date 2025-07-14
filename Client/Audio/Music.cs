using Spectrum.Audio;
using Spectrum.Core.Manage;

namespace Ethla.Client.Audio;

public class Music
{

	public static Music None = new Music(null);

	public static List<Music> Musics = new List<Music>();

	private readonly Id? soundLocation;

	public Music(string soundloc)
	{
		if (soundloc != null)
			soundLocation = new Id(soundloc);
	}

	public float PlayMusic()
	{
		if (soundLocation == null)
			return 0;
		TrackData audioData = Loads.Get(soundLocation.Value);
		Track clip = audioData.NewTrack();
		clip.Set(Track.Controller.Gain, 0.75f);
		clip.Play();
		return (float)audioData.Length.TotalSeconds;
	}

}
