using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.Maths;
using Spectrum.Maths.Random;
using Spectrum.Maths.Shapes;

namespace Ethla.World.Physics;

public abstract partial class Entity
{

	private readonly List<BlockPos> posCache = new List<BlockPos>();

	public float AirK = 0.141f;
	public BlockState BlockStaying = BlockState.Empty;
	public BlockState BlockStepping = BlockState.Empty;
	public Quad Bound;
	public ImVector2 Face = new ImVector2();
	public float GravityMult = 1;
	public bool IgnoreLandFriction;
	public bool IsCollidable = true;
	public bool IsFloating;
	public bool IsSlidable = true;
	public bool JustMoved;
	public Level Level;
	public Dictionary<Liquid, float> LiquidAreas = new Dictionary<Liquid, float>();
	public float Mass = 1;
	public PrecisePos Pos;
	public PrecisePos PosLastTick;
	public float ReboundFactor = 0;
	public bool Touch;
	public bool TouchDown;
	public bool TouchHorizontal;
	public bool TouchLeft;
	public bool TouchRight;
	public bool TouchUp;
	public bool TouchVertical;
	public ImVector2 Velocity = new ImVector2();
	public float VelocityRadian;
	public ImVector2 VisualSize = new ImVector2();
	public float StepHeight = 0.5f;
	private float movedAccumulation;

	public static float SoundStepMoveDistance = 2;
	public static float SoundStepVergeSpeed = 3;
	public static float SoundStepVergeSpeedLn = 3;

	public void ApplyImpulse(in Vector2 i)
	{
		Velocity += i / Mass;
	}

	public void ApplyLimitedImpulse(in Vector2 i)
	{
		float dvx = i.X / Mass;
		float dvy = i.Y / Mass;

		if (Velocity.X > 0)
			Velocity.X = Math.Clamp(Velocity.X + dvx, 0, float.PositiveInfinity);
		else if (Velocity.X < 0)
			Velocity.X = Math.Clamp(Velocity.X + dvx, float.NegativeInfinity, 0);

		if (Velocity.Y > 0)
			Velocity.Y = Math.Clamp(Velocity.Y + dvy, 0, float.PositiveInfinity);
		else if (Velocity.Y < 0)
			Velocity.Y = Math.Clamp(Velocity.Y + dvy, float.NegativeInfinity, 0);
	}

	public Quad Move()
	{
		if (!Lowp)
		{
			Level.GetNearbyLiquids(posCache, Bound);

			LiquidAreas.Clear();

			foreach (BlockPos pos in posCache)
			{
				LiquidStack liquidStack = Level.GetLiquid(pos);
				Quad posr = new Quad().Set(pos.BlockX, pos.BlockY, 1, liquidStack.Percent);
				float area = posr.GetIntersection(Bound).Area;

				if (area <= 0)
					continue;

				if (!LiquidAreas.ContainsKey(liquidStack.Liquid))
					LiquidAreas[liquidStack.Liquid] = area;
				else
					LiquidAreas[liquidStack.Liquid] += area;
			}

			foreach (KeyValuePair<Liquid, float> kv in LiquidAreas)
			{
				Liquid liquid = kv.Key;
				float area = kv.Value;
				liquid.OnEntityCollided(this, area);
				Vector2 lqf = new Vector2().SetRad(Velocity.Len2, (float)Math.PI + Mathf.AtanRad(Velocity.Y, Velocity.X));
				ApplyLimitedImpulse(lqf * liquid.GetFriction() * Time.Delta);
				ApplyImpulse(new Vector2(0, area * liquid.GetDensity() * -Level.Gravity.Y * Time.Delta));
			}

			IsFloating = LiquidAreas.Count > 0;
		}

		//Forcing recalc
		Vector2 airf = new Vector2().SetRad(Velocity.Len2, (float)Math.PI + Mathf.AtanRad(Velocity.Y, Velocity.X));
		ApplyLimitedImpulse(airf * AirK * Time.Delta);
		ApplyImpulse((Vector2)Level.Gravity * Mass * Time.Delta * GravityMult);

		if (TouchDown && !IgnoreLandFriction)
		{
			float m0 = BlockStepping.GetFricitonForce(this);
			float f0 = m0 * -Level.Gravity.Y * Mass;
			ApplyLimitedImpulse(new Vector2(f0 * Math.Sign(-Velocity.X), 0) * Time.Delta);
		}

		//Move
		Quad origin = Bound; //value copy.
		Quad destination = Bound;

		float dx = Velocity.X * Time.Delta, dy = Velocity.Y * Time.Delta;
		float dx0 = dx, dy0 = dy;

		JustMoved = true;

		if (IsCollidable && dx != 0)
		{
			Level.GetNearbyBlocks(posCache, destination);

			for (int i = 0; i < posCache.Count; i++)
			{
				BlockPos pos = posCache[i];
				VoxelClip otl = Level.GetBlock(pos).GetSilhouette(this);
				float ox = pos.BlockX;
				float oy = pos.BlockY;
				dx = otl.ClipX(dx, destination, ox, oy);
			}

			//Step check
			if (TouchDown)
			{
				Quad stepDest = origin;
				float dx1 = dx0;
				stepDest.Translate(0, StepHeight);

				for (int i = 0; i < posCache.Count; i++)
				{
					BlockPos pos = posCache[i];
					VoxelClip otl = Level.GetBlock(pos).GetSilhouette(this);
					dx1 = otl.ClipX(dx1, stepDest, pos.BlockX, pos.BlockY);
				}

				if (Math.Abs(dx1 - dx) > Mathf.Tolerance && Math.Abs(dx) >= Mathf.Tolerance)
				{
					destination.Translate(dx1, StepHeight);
					//falling check later in y axis moving.
					dy = -StepHeight;
					dx = dx0;
				}
			}
		}

		if (Math.Abs(dx) < Mathf.Tolerance) dx = 0;

		destination.Translate(dx, 0);

		if (IsCollidable && dy != 0)
		{
			Level.GetNearbyBlocks(posCache, destination);

			for (int i = 0; i < posCache.Count; i++)
			{
				BlockPos pos = posCache[i];
				VoxelClip otl = Level.GetBlock(pos).GetSilhouette(this);
				float ox = pos.BlockX;
				float oy = pos.BlockY;
				dy = otl.ClipY(dy, destination, ox, oy);
			}
		}

		if (Math.Abs(dy) < Mathf.Tolerance) dy = 0;

		//clamp it in section
		if (origin.Yprom + dy >= Chunk.Height) dy = Chunk.Height - origin.Yprom;
		if (origin.Y + dy <= 0) dy = -origin.Y;

		destination.Translate(0, dy);

		if (dx == 0 && dy == 0) JustMoved = false;

		bool xClip = Math.Abs(dx - dx0) > Mathf.Tolerance;
		bool yClip = Math.Abs(dy - dy0) > Mathf.Tolerance;

		TouchLeft = dx0 < 0 && xClip;
		TouchRight = dx0 > 0 && xClip;
		TouchDown = dy0 < 0 && yClip;
		TouchUp = dy0 > 0 && yClip;
		TouchHorizontal = TouchLeft || TouchRight;
		TouchVertical = TouchUp || TouchDown;
		Touch = TouchHorizontal || TouchVertical;

		movedAccumulation += new Vector2(dx, dy).Len;

		if (!IsSlidable && Touch)
		{
			dx = dy = 0;
			JustMoved = false;
			destination = origin;
		}

		if (JustMoved)
		{
			VelocityRadian = Mathf.AtanRad(dy, dx);
			Face.X = dx > 0 ? 1 : -1;
			Face.Y = dy > 0 ? 1 : -1;
			//if not moved, keep the previous face still.
		}

		//TryPushOutsideBlocks(posCache, ref destination);
		Locate(destination.Xcentral, destination.Ycentral);

		if (!Lowp)
		{
			BlockStaying = Level.GetBlock(Pos);
			BlockStepping = Level.GetBlock(Direction.Down.Step(new BlockPos(Pos)));
			BlockStepping.OnStepped(this);

			if ((TouchHorizontal && Math.Abs(Velocity.X) > SoundStepVergeSpeed)
			|| (TouchVertical && Math.Abs(Velocity.Y) > SoundStepVergeSpeed)
			|| Touch && Velocity.Len > SoundStepVergeSpeedLn && movedAccumulation > SoundStepMoveDistance)
			{
				BlockStepping.GetSound("step").PlaySound(Pos);
				movedAccumulation = 0;
			}
		}

		//Postchecks
		float reboundFactor = ReboundFactor;

		if (TouchUp && Velocity.Y > 0) Velocity.Y *= -reboundFactor;
		if (TouchDown && Velocity.Y < 0) Velocity.Y *= -reboundFactor;
		if (TouchLeft && Velocity.X < 0) Velocity.X *= -reboundFactor;
		if (TouchRight && Velocity.X > 0) Velocity.X *= -reboundFactor;

		return destination;
	}

	public void TryPushOutsideBlocks(List<BlockPos> posCache, ref Quad dest)
	{
		for (int i = 0; i < posCache.Count; i++)
		{
			BlockPos pos = posCache[i];
			VoxelClip otl = Level.GetBlock(pos).GetSilhouette(this);

			if (otl.Interacts(dest, pos.BlockX, pos.BlockY))
			{
				const float factor = 1.5f;

				bool left = dest.Xcentral < pos.X;
				bool down = dest.Ycentral < pos.Y;

				if (left)
					ApplyImpulse(new Vector2(-(dest.Xprom - pos.BlockX) * Mass, 0) * factor);
				else
					ApplyImpulse(new Vector2((pos.BlockX + 1 - dest.X) * Mass, 0) * factor);

				if (down)
					ApplyImpulse(new Vector2(-(dest.Yprom - pos.BlockY) * Mass, 0) * factor);
				else
					ApplyImpulse(new Vector2((pos.BlockY + 1 - dest.Y) * Mass, 0) * factor);
			}
		}
	}

	public void Locate(Pos pos, bool keepLastPos = true)
	{
		Locate(pos.X, pos.Y, keepLastPos);
	}

	public void Locate(float x, float y, bool keepLastPos = true)
	{
		PosLastTick = Pos;
		Bound.LocateCentral(x, y);
		Pos = new PrecisePos(x, y);
		if (!keepLastPos)
			PosLastTick = Pos;
	}

}
