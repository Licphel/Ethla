using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Maths.Random;

namespace Ethla.World;

public class Climate
{

	public bool Rain, Snow;
	public float RainStrength, SnowStrength;
	public float RainTime, SnowTime;
	public float SeasonClock;
	public float Wind;

	public void Tick()
	{
		if (Keyboard.Global.Observe(KeyId.KeyF8).Pressed())
			Rain = !Rain;
		if (Keyboard.Global.Observe(KeyId.KeyF9).Pressed())
			Snow = !Snow;

		float bd = Rain ? 60 : 120;
		float sd = Snow ? 60 : 120;

		if (RainTime > bd && TimeSchedule.PeriodicTask(1) && Seed.Global.NextFloat() < (RainTime - bd) / 1000f)
		{
			Rain = !Rain;
			RainTime = 0;
		}

		if (SnowTime > sd && TimeSchedule.PeriodicTask(1) && Seed.Global.NextFloat() < (SnowTime - sd) / 1000f)
		{
			Snow = !Snow;
			SnowTime = 0;
		}

		if (Rain)
		{
			RainTime += Time.Delta;
			RainStrength = Math.Clamp(RainStrength + Time.Delta * 0.2f, 0, 1);
		}
		else
		{
			RainStrength = Math.Clamp(RainStrength - Time.Delta * 0.2f, 0, 1);
		}

		if (Snow)
		{
			SnowTime += Time.Delta;
			SnowStrength = Math.Clamp(SnowStrength + Time.Delta * 0.2f, 0, 1);
		}
		else
		{
			SnowStrength = Math.Clamp(SnowStrength - Time.Delta * 0.2f, 0, 1);
		}
	}

}
