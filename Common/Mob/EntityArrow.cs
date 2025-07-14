using Ethla.Client;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityArrow : Entity
{

	public EntityArrow()
	{
		Bound.Resize(1 / 16f, 1 / 16f);
		VisualSize.X = 8 / 16f;
		VisualSize.Y = 8 / 16f;
		GravityMult = 0.5f;
		IsSlidable = false;
		ReboundFactor = 0;
	}

	public override void Tick()
	{
		base.Tick();

		Move();

		List<Entity> lst = Level.GetNearbyEntities(Bound, typeof(Creature));
		lst.ForEach(e =>
		{
			Creature c = (Creature)e;
			if (c.UniqueId == OwnerUniqueId)
				return;
			c.Hit(new Damage(this, DamageTypes.Mechanic, 20));
			Die();
		});

		if (Touch && !IsDead)
		{
			Quad box2 = Bound;
			box2.Scale(3f, 3f);
			lst = Level.GetNearbyEntities(box2, typeof(Creature));

			foreach (Creature e in lst)
				if (e.Inventory != null && e.Inventory.Add(Items.Arrow.MakeStack()).IsEmpty)
				{
					Die();
					break;
				}
		}
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		graphics.Color4(Light);
		graphics.TransformStack.Push();
		graphics.TransformStack.Rotate(Rotation.Get(rect, Angle.Radian(VelocityRadian)));
		graphics.DrawImage(Images.EntityArrow, rect);
		graphics.TransformStack.Pop();
		graphics.NormalizeColor();
	}

}
