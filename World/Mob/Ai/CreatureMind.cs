namespace Ethla.World.Mob.Ai;

public class CreatureMind
{

	public List<Activity> Activities = new List<Activity>();

	public void Tick(Creature creature)
	{
		foreach (Activity act in Activities) act.Act(creature);
	}

}
