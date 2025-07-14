using Spectrum.Graphic;
using Spectrum.Maths.Shapes;

namespace Ethla.World.Lighting;

public abstract class LightEngine
{

	public const float Amplifier = 1.25f;
	public static int EntityDynamicLightCalcRadius = 4;
	public static float DarkLuminance = 0.05f;
	public static float Cps;

	public static float MaxValue = 1; //To use brighter light, see #maxValue.
	public static float Unit = MaxValue / 8f;
	public float[] Max = { 10, 10, 10 };
	public float[] Min = { 0, 0, 0 };

	public abstract bool IsStableRoll { get; }

	public abstract void DrawSmooth(float x, float y, float v1, float v2, float v3);
	public abstract void DrawDirect(int x, int y, float v1, float v2, float v3);

	public abstract void Glow(int x, int y, float v1, float v2, float v3);
	public abstract Chunk GetBufferedChunk(int xb);

	public abstract Color GetLinearLight(float x, float y);
	public abstract void CalculateByViewdim(Quad cam);

}
