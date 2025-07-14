using Spectrum.Maths.Shapes;

namespace Ethla.World.Voxel;

public interface VoxelClip
{

	public static VoxelClip Void = new Empty();
	public static VoxelClip Cube = new VoxelBox(0, 0, 16, 16);

	float ClipX(float dx, Quad aabb, float ox, float oy);

	float ClipY(float dy, Quad aabb, float ox, float oy);

	bool Interacts(Quad aabb, float ox, float oy);

	bool Contains(float x, float y, float ox, float oy);

	public class Empty : VoxelClip
	{

		public float ClipX(float dx, Quad aabb, float ox, float oy)
		{
			return dx;
		}

		public float ClipY(float dy, Quad aabb, float ox, float oy)
		{
			return dy;
		}

		public bool Interacts(Quad aabb, float ox, float oy)
		{
			return false;
		}

		public bool Contains(float x, float y, float ox, float oy)
		{
			return false;
		}

	}

}
