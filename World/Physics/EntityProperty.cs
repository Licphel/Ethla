namespace Ethla.World.Physics;

public class EntityProperty
{

	public float BoundX, BoundY;
	public bool IsCollidable = true;
	public bool IsSlidable = true;
	public float[] Light = [0, 0, 0];

	public bool Lowp;
	public float Mass = 1.0f;
	public float ReboundFactor;
	public float VisualX, VisualY;

}
