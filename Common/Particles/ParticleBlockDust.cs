using Ethla.Client.Voxel;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;
using Spectrum.Maths.Random;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Particles;

public class ParticleBlockDust : Entity
{

	private readonly Image blockicon;
	private readonly float degree;
	private readonly int u;
	private readonly int v;
	public float MaxAliveTime;

	public ParticleBlockDust(object o)
	{
		degree = Seed.Global.NextFloat(0, 360);

		Icon icon = null;

		if (o is BlockState state)
			icon = BlockModels.GetIcon(state);
		if (o is Wall wall)
			icon = WallModels.GetIcon(wall);

		if (icon is Image p)
		{
			blockicon = p;

			u = (int)(Seed.Global.NextFloat(0, 1) * p.Width);
			v = (int)(Seed.Global.NextFloat(0, 1) * p.Height);
			if (u > p.Width) u = 0;
			if (v > p.Height) v = 0;
			u += (int)p.U;
			v += (int)p.V;
		}
		else
		{
			Die(false);
		}

		Velocity.X = 3f * Mathf.CosDeg(degree);
		MaxAliveTime = Seed.Global.NextFloat(0.5f, 2f);
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		Color color = Light;
		color.A = 1 - DeathTimer;

		TransformStack transformStack = graphics.TransformStack;

		transformStack.Push();
		transformStack.Rotate(Rotation.Get(rect, Angle.Radian(VelocityRadian)));
		graphics.Color4(color);
		graphics.DrawImage(blockicon.Reference, rect, u, v, 1, 1);
		graphics.NormalizeColor();
		transformStack.Pop();
	}

	public override void Tick()
	{
		base.Tick();

		if (LiveTime >= MaxAliveTime)
			Die();

		Move();
	}

}
