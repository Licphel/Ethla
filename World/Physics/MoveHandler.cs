using Spectrum.Maths;

namespace Ethla.World.Physics;

public class MoveHandler
{

	public static void SimpleMove(Entity o, in Vector2 speed, float accel)
	{
		if (Math.Abs(o.Velocity.X) < Math.Abs(speed.X) || o.Velocity.X * speed.X < 0)
		{
			if (speed.X > 0)
				o.Velocity.X = Math.Clamp(o.Velocity.X + accel, float.NegativeInfinity, speed.X);
			else
				o.Velocity.X = Math.Clamp(o.Velocity.X - accel, speed.X, float.PositiveInfinity);
		}

		if (Math.Abs(o.Velocity.Y) < Math.Abs(speed.Y) || o.Velocity.Y * speed.Y < 0)
		{
			if (speed.Y > 0)
				o.Velocity.Y = Math.Clamp(o.Velocity.Y + accel, float.NegativeInfinity, speed.Y);
			else
				o.Velocity.Y = Math.Clamp(o.Velocity.Y - accel, speed.Y, float.PositiveInfinity);
		}
	}

	public static void LerpedMove(Entity o, in Vector2 speed, float muler)
	{
		if (Math.Abs(o.Velocity.X) < Math.Abs(speed.X) || o.Velocity.X * speed.X < 0)
			o.Velocity.X += (speed.X - o.Velocity.X) * muler;
		if (Math.Abs(o.Velocity.Y) < Math.Abs(speed.Y) || o.Velocity.Y * speed.Y < 0)
			o.Velocity.Y += (speed.Y - o.Velocity.Y) * muler;
	}

}
