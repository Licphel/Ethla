using Ethla.World.Physics;
using Spectrum.Graphic;
using Spectrum.Maths.Shapes;

namespace Ethla.Client.Physics;

public class BonedRendering
{

	public static void Draw(Graphics graphics, BoneGroup group, Entity entity, Quad rect)
	{
		List<Bone> begin = group.Begin, end = group.End;

		if (entity.Face.X < 0)
		{
			begin = group.End;
			end = group.Begin;
		}

		foreach (Bone b in begin)
		{
			b.FindPosRelatively(entity);
			b.Draw(graphics, entity, rect.X, rect.Y);
		}

		graphics.DrawIcon(group.Body, rect);

		foreach (Bone b in end)
		{
			b.FindPosRelatively(entity);
			b.Draw(graphics, entity, rect.X, rect.Y);
		}
	}

}
