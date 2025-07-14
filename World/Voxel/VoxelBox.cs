using Spectrum.Maths.Shapes;

namespace Ethla.World.Voxel;

public class VoxelBox : VoxelClip
{

	public float H;
	public float W;

	public float X;
	public float Y;

	public VoxelBox(float x, float y, float w, float h)
	{
		X = x / 16f;
		Y = y / 16f;
		W = w / 16f;
		H = h / 16f;
	}

	public bool Interacts(Quad aabb, float ox, float oy)
	{
		Quad b1 = new Quad();
		b1.Resize(W, H);
		b1.Locate(ox + X, oy + Y);
		return b1.Interacts(aabb);
	}

	public bool Contains(float x0, float y0, float ox, float oy)
	{
		return X + ox <= x0 && Y + oy <= y0 && X + ox + W >= x0 && Y + oy + H >= y0;
	}

	public float ClipX(float dx, Quad aabb, float ox, float oy)
	{
		float ax = ox + X;
		float ay = oy + Y;
		if (aabb.Yprom <= ay || aabb.Y >= ay + H) return dx; //No collide on y, impossible to collide.

		if (dx > 0 && aabb.Xprom <= ax) dx = Math.Min(dx, ax - aabb.Xprom);
		if (dx < 0 && aabb.X >= ax + W) dx = Math.Max(dx, ax + W - aabb.X);

		return dx;
	}

	public float ClipY(float dy, Quad aabb, float ox, float oy)
	{
		float ax = ox + X;
		float ay = oy + Y;
		if (aabb.Xprom <= ax || aabb.X >= ax + W) return dy; //No collide on x, impossible to collide.

		if (dy > 0 && aabb.Yprom <= ay) dy = Math.Min(dy, ay - aabb.Yprom);
		if (dy < 0 && aabb.Y >= ay + H) dy = Math.Max(dy, ay + H - aabb.Y);

		return dy;
	}

}
