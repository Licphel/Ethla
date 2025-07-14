using Spectrum.Maths.Shapes;

namespace Ethla.World.Voxel;

public class VoxelOutline : List<VoxelBox>, VoxelClip
{

	public VoxelOutline()
	{
	}

	private VoxelOutline(VoxelBox[] boxes) : base(boxes)
	{
	}

	//END

	public bool Interacts(Quad aabb, float ox, float oy)
	{
		foreach (VoxelBox box in this)
			if (box.Interacts(aabb, ox, oy))
				return true;
		return false;
	}

	public bool Contains(float x, float y, float ox, float oy)
	{
		foreach (VoxelBox box in this)
			if (box.Contains(x, y, ox, oy))
				return true;
		return false;
	}

	public float ClipX(float dx, Quad aabb, float ox, float oy)
	{
		foreach (VoxelBox box in this) dx = box.ClipX(dx, aabb, ox, oy);
		return dx;
	}

	public float ClipY(float dy, Quad aabb, float ox, float oy)
	{
		foreach (VoxelBox box in this) dy = box.ClipY(dy, aabb, ox, oy);
		return dy;
	}

	public static VoxelOutline From(params VoxelBox[] boxes)
	{
		return new VoxelOutline(boxes);
	}

}
