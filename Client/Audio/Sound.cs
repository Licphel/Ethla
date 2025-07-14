using Ethla.World;
using Spectrum.Audio;
using Spectrum.Core.Manage;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.Client.Audio;

public readonly struct Sound
{

	public static float HearableLimit = 24;
	public static Sound None = new Sound(null);

	public readonly Id? SoundLocation;
	public readonly bool RandomGp;

	public Sound(string soundloc, bool randomGp = true)
	{
		if (soundloc != null)
			SoundLocation = new Id(soundloc);
		RandomGp = randomGp;
	}

	public TimeSpan PlaySound()
	{
		return Play(SoundLocation.GetValueOrDefault(), RandomGp);
	}

	public TimeSpan PlaySound(Pos pos)
	{
		Pos listenerPos = Main.Player.Pos;
		float dist = Posing.Distance(pos, listenerPos);

		if (dist >= HearableLimit)
			return TimeSpan.Zero;

		return Play(SoundLocation.GetValueOrDefault(), RandomGp, Mathf.Pow(1 - dist / HearableLimit, 2) * 1.5f);
	}

	public static TimeSpan Play(Id id, bool randomGp = true, float percent = 1)
	{
		TrackData audioData = Loads.Get(id);
		if (audioData == null)
			return TimeSpan.Zero;
		Track clip = audioData.NewTrack();
		if (randomGp)
		{
			clip.Set(Track.Controller.Gain, Seed.Global.NextFloat(0.8f, 1.2f) * percent);
			clip.Set(Track.Controller.Pitch, Seed.Global.NextFloat(0.8f, 1.2f));
		}
		else
		{
			clip.Set(Track.Controller.Gain, percent);
		}

		clip.Play();
		return audioData.Length;
	}

}
