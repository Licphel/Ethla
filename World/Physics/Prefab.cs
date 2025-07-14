using Spectrum.Core.Manage;
using Spectrum.Maths;

namespace Ethla.World.Physics;

public delegate Entity SupplyEntity();

public class Prefab : IdHolder
{

	public static Dictionary<Type, Prefab> Prefabs = new Dictionary<Type, Prefab>();
	public EntityProperty Property = new EntityProperty();

	public Type Type;

	public Prefab(Type type)
	{
		Type = type;
		Prefabs[type] = this;
	}

	public dynamic Instantiate()
	{
		return (Entity)Activator.CreateInstance(Type);
	}

	public void MakePrefab(Entity entity)
	{
		entity.Mass = Property.Mass;
		entity.Bound.Resize(Property.BoundX, Property.BoundY);
		entity.Glow = Property.Light;
		entity.IsCollidable = Property.IsCollidable;
		entity.IsSlidable = Property.IsSlidable;
		entity.VisualSize = new ImVector2(Property.VisualX, Property.VisualY);
		entity.ReboundFactor = Property.ReboundFactor;
		entity.Lowp = Property.Lowp;
	}

}
