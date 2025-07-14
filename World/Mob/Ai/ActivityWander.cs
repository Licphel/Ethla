using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.Maths;
using Spectrum.Maths.Random;

namespace Ethla.World.Mob.Ai;

public class ActivityWander : Activity
{

	private float speed;
	private float time;

	public void Act(Creature e)
	{
		if (time <= 0 && TimeSchedule.PeriodicTask(e.LiveTime, 1) && Seed.Global.NextFloat() < 0.05f)
		{
			time = Seed.Global.NextFloat(1.0f, 15.0f);
			speed = Seed.Global.NextFloat(1.0f, 5.0f);
			if (Seed.Global.Next())
				speed *= -1;
		}

		time -= Time.Delta;

		if (time > 0)
		{
			MoveHandler.SimpleMove(e, new Vector2(speed, 0), Math.Abs(speed / 5f));
		}
	}

}
