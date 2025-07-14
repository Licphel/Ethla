namespace Ethla.World.Voxel;

public delegate float FilterLight(int pipe, float v, int x, int y);

public class BlockShape
{

	public static readonly BlockShape Solid = new BlockShape((p, v, x, y) => v * 0.9f - 0.1f, true);
	public static readonly BlockShape Parital = new BlockShape((p, v, x, y) => v * 0.95f - 0.05f, true);
	public static readonly BlockShape Hollow = new BlockShape((p, v, x, y) => v * 0.975f - 0.04f, false);
	public static readonly BlockShape Vacuum = new BlockShape((p, v, x, y) => v - 0.04f, false);

	public FilterLight FilterLight;
	public bool IsFull;

	public BlockShape(FilterLight fl, bool ful)
	{
		FilterLight = fl;
		IsFull = ful;
	}

}
