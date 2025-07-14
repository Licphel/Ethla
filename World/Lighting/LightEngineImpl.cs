using System.Runtime.CompilerServices;
using Ethla.Client.Ambient;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Random;
using Spectrum.Maths.Shapes;
using Spectrum.Utils;

namespace Ethla.World.Lighting;

//The worst code in this game! Do not touch anything unless you want a crash!
public class LightEngineImpl : LightEngine
{

	private readonly List<Entity> cache = new List<Entity>();

	private readonly Level level;

	private readonly Queue<Action> rqDraw = new Queue<Action>();

	private readonly float[] sunlight = new float[3];
	private readonly Chunk[] tmpChunks = new Chunk[10];
	private int chkOrigin;
	public bool Done;
	private int calcs;

	public long Elapsed;
	private Coroutine tasking;

	public LightEngineImpl(Level level)
	{
		this.level = level;
	}

	public override bool IsStableRoll => true;

	public override void DrawSmooth(float x, float y, float v1, float v2, float v3)
	{
		rqDraw.Enqueue(() => InnerDrawSmooth(x, y, v1, v2, v3));
	}

	public void InnerDrawSmooth(float x, float y, float v1, float v2, float v3)
	{
		if (v1 <= DarkLuminance && v2 <= DarkLuminance && v3 <= DarkLuminance)
			return;

		int r = EntityDynamicLightCalcRadius;
		int lx = Mathf.FastFloor(x);
		int ly = Mathf.FastFloor(y);

		for (int tx = lx - r; tx < lx + r; tx += 1)
		{
			for (int ty = ly - r; ty < ly + r; ty += 1)
			{
				float dist2 = Mathf.Pow(tx - x, 2) + Mathf.Pow(ty - y, 2);
				if (dist2 < 1) dist2 = 1;
				DrawDirect(tx, ty, v1 / dist2, v2 / dist2, v3 / dist2);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void DrawDirect(int x, int y, float v1, float v2, float v3)
	{
		Chunk chunk = GetBufferedChunk(x);
		if (chunk == null) return;

		if (y < 0 || y > Chunk.Height) return;

		LightWare tl = chunk.GetSurLightware(x, y);
		tl.Light[0] = Math.Max(tl.Light[0], v1);
		tl.Light[1] = Math.Max(tl.Light[1], v2);
		tl.Light[2] = Math.Max(tl.Light[2], v3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Color GetLinearLight(float x, float y)
	{
		if (Main.GodMode)
			return new Color(1, 1, 1);

		int ix = Mathf.FastFloor(x);
		int iy = Mathf.FastFloor(y);

		Chunk chunk = level.GetChunkByBlock(ix);

		if (chunk == null) return new Color(1, 1, 1);

		float[] rgb00 = chunk.GetSubLightware(ix, iy).Light;

		Color rgb = new Color();

		rgb.R = Math.Clamp(Math.Max(rgb00[0], rgb00[3] * sunlight[0]), 0, 1);
		rgb.G = Math.Clamp(Math.Max(rgb00[1], rgb00[4] * sunlight[1]), 0, 1);
		rgb.B = Math.Clamp(Math.Max(rgb00[2], rgb00[5] * sunlight[2]), 0, 1);

		return rgb;
	}

	public override void Glow(int x, int y, float v1, float v2, float v3)
	{
		Chunk chunk = GetBufferedChunk(x);
		if (chunk == null) return;

		if (y < 0 || y > Chunk.Height) return;

		LightWare p = chunk.GetSurLightware(x, y);
		p.Light[0] = Math.Max(p.Light[0], v1);
		p.Light[1] = Math.Max(p.Light[1], v2);
		p.Light[2] = Math.Max(p.Light[2], v3);
	}

	public override void CalculateByViewdim(Quad cam)
	{
		Color sun = CelestComputation.LightingSunlight(level);
		sunlight[0] = sun.R;
		sunlight[1] = sun.G;
		sunlight[2] = sun.B;

		float spd = MaxValue / Unit;

		if (Done)
		{
			//Swap surface buffer and back buffer (back buffer is used to render).
			Swap();
			Done = false;
		}

		if (tasking == null || tasking.IsCompleted)
		{
			//Write the result into surface buffer, in coroutines.
			tasking = new Coroutine(() =>
			{
				Calculate(cam, (int)(cam.X - spd), (int)(cam.Y - spd), (int)(cam.Xprom + spd), (int)(cam.Yprom + spd));

				Done = true;
				calcs++;
			});
			tasking.Start();
		}

		if (TimeSchedule.PeriodicTask(1))
		{
			Cps = calcs;
			calcs = 0;
		}

		Elapsed++;
	}

	public void Swap()
	{
		foreach (Chunk chk in tmpChunks) chk?.SwapLightwareBuffer();
	}

	//Two steps contained. One is in-screen process, the other is out-screen buffer zone process.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Calculate(Quad cam, int x0, int y0, int x1, int y1)
	{
		chkOrigin = Mathf.FastFloor(cam.Xcentral / 16f) - 5;

		for (int i = chkOrigin; i < chkOrigin + 10; i++) tmpChunks[i - chkOrigin] = level.GetChunk(i);

		y0 = Math.Clamp(y0, 0, Chunk.MaxY);
		y1 = Math.Clamp(y1, 0, Chunk.MaxY);

		for (int x = x0; x <= x1; x++)
		{
			Chunk chunk = GetBufferedChunk(x);
			if (chunk == null) continue;

			for (int y = y0; y <= y1; y++)
			{
				LightWare s0 = chunk.GetSurLightware(x, y);
				s0.Light[0] = GetBlockCastLight(chunk, x, y, 0);
				s0.Light[1] = GetBlockCastLight(chunk, x, y, 1);
				s0.Light[2] = GetBlockCastLight(chunk, x, y, 2);

				LightWare s1 = chunk.GetSurLightware(x, y);
				s1.Light[3] = GetFilteredSkyLight(chunk, x, y, 0);
				s1.Light[4] = GetFilteredSkyLight(chunk, x, y, 1);
				s1.Light[5] = GetFilteredSkyLight(chunk, x, y, 2);
			}
		}

		Quad aabb = new Quad();
		aabb.Resize(x1 - x0, y1 - y0);
		aabb.LocateCentral((x1 + x0) / 2f, (y1 + y0) / 2f);
		level.GetNearbyEntities(cache, aabb);

		//Must after the block shedding.
		//Or the entity shedding will be replaced.

		foreach (Entity e in cache)
		{
			float l11 = e.CastLight(LightPass.Red);
			float l12 = e.CastLight(LightPass.Green);
			float l13 = e.CastLight(LightPass.Blue);

			if (l11 == 0 && l12 == 0 && l13 == 0) continue;

			float sx = e.Pos.X;
			float sy = e.Pos.Y;

			DrawSmooth(sx, sy, l11, l12, l13);
		}

		while (rqDraw.Count != 0) rqDraw.Dequeue().Invoke();

		for (int x = x1; x >= x0; x--)
		{
			Chunk chunk = GetBufferedChunk(x);
			Chunk cn1 = GetBufferedChunk(x - 1);
			Chunk cp1 = GetBufferedChunk(x + 1);
			if (chunk == null) continue;

			for (int y = y1; y >= y0; y--)
			{
				Calculate0(chunk, cn1, cp1, chunk.GetSurLightware(x, y), x, y);
			}
		}

		for (int x = x0; x <= x1; x++)
		{
			Chunk chunk = GetBufferedChunk(x);
			Chunk cn1 = GetBufferedChunk(x - 1);
			Chunk cp1 = GetBufferedChunk(x + 1);

			if (chunk == null) continue;

			for (int y = y0; y <= y1; y++)
			{
				Calculate0(chunk, cn1, cp1, chunk.GetSurLightware(x, y), x, y);
			}
		}

		for (int x = x0; x <= x1; x++)
		{
			Chunk chunk = GetBufferedChunk(x);
			Chunk cn1 = GetBufferedChunk(x - 1);
			Chunk cp1 = GetBufferedChunk(x + 1);
			if (chunk == null) continue;

			for (int y = y1; y >= y0; y--)
			{
				Calculate0(chunk, cn1, cp1, chunk.GetSurLightware(x, y), x, y);
			}
		}

		for (int x = x1; x >= x0; x--)
		{
			Chunk chunk = GetBufferedChunk(x);
			Chunk cn1 = GetBufferedChunk(x - 1);
			Chunk cp1 = GetBufferedChunk(x + 1);

			if (chunk == null) continue;

			for (int y = y0; y <= y1; y++)
			{
				Calculate0(chunk, cn1, cp1, chunk.GetSurLightware(x, y), x, y);
			}
		}

		for (int x = x0; x <= x1; x++)
		{
			Chunk chunk = GetBufferedChunk(x);

			if (chunk == null) continue;

			for (int y = y0; y <= y1; y++)
			{
				chunk.GetSurLightware(x, y).Submit(x, y, sunlight);
			}
		}
	}

	public void Calculate0(Chunk chunk, Chunk cn1, Chunk cp1, LightWare s, int x, int y)
	{
		SpreadLight(chunk, cn1, cp1, s, x, y, 0, 0);
		SpreadLight(chunk, cn1, cp1, s, x, y, 1, 0);
		SpreadLight(chunk, cn1, cp1, s, x, y, 2, 0);
		SpreadLight(chunk, cn1, cp1, s, x, y, 0, 3);
		SpreadLight(chunk, cn1, cp1, s, x, y, 1, 3);
		SpreadLight(chunk, cn1, cp1, s, x, y, 2, 3);
		
	}

	public void SpreadLight(Chunk chunk, Chunk cn1, Chunk cp1, LightWare s, int x, int y, byte idx, int offset)
	{
		float l1;
		float l2 = 0, l3 = 0, l4 = 0, l5 = 0;
		if (y >= 0 && y <= Chunk.MaxY)
		{
			if (cn1 != null) l2 = cn1.GetSurLightware(x - 1, y).Light[idx + offset];

			if (cp1 != null) l3 = cp1.GetSurLightware(x + 1, y).Light[idx + offset];

			if (y > 0)
				//out of bound!
				l4 = chunk.GetSurLightware(x, y - 1).Light[idx + offset];

			if (y < Chunk.MaxY) l5 = chunk.GetSurLightware(x, y + 1).Light[idx + offset];

			l1 = wmix(l2, wmix(l3, wmix(l4, l5)));
		}
		else
		{
			if (y <= 0)
				l1 = 0;
			else
				l1 = 1;
		}

		if (l1 <= DarkLuminance)
		{
			s.Light[idx + offset] = 0; //No need to calc in this case.
		}
		else
		{
			l1 = chunk.GetBlock(x, y).FilterLight(idx, l1, x, y);
			LiquidStack lqs = chunk.GetLiquid(x, y);
			l1 = lqs.Liquid.FilterLight(idx, l1, x, y, lqs.Amount);
			s.Light[idx + offset] = Math.Min(Max[idx], Math.Max(Min[idx], l1));
		}
	}

	public float GetBlockCastLight(Chunk chunk, int x, int y, byte pipe)
	{
		BlockState b1 = chunk.GetBlock(x, y);
		Wall b2 = chunk.GetWall(x, y);
		LiquidStack lqs = chunk.GetLiquid(x, y);
		float l1 = b1.CastLight(pipe, x, y);
		float l2 = b2.CastLight(pipe, x, y);
		float l3 = lqs.Liquid.CastLight(pipe, x, y, lqs.Amount);
		return wmix(l1, wmix(l2, l3));
	}

	public float GetFilteredSkyLight(Chunk chunk, int x, int y, byte pipe)
	{
		if (y < Chunk.YOfSea - 5) return 0;

		BlockState b1 = chunk.GetBlock(x, y);
		Wall b2 = chunk.GetWall(x, y);
		float amp = y >= Chunk.YOfSea ? 1 : Math.Max(0, 1 - (Chunk.YOfSea - y) * 0.25f);

		BlockShape s1 = b1.GetShape();
		BlockShape s2 = b2.GetShape();

		LiquidStack lqs = chunk.GetLiquid(x, y);

		if (s1 == BlockShape.Vacuum && s2 == BlockShape.Vacuum)
			return lqs.Liquid.FilterLight(pipe, amp, x, y, lqs.Amount);

		if (s1 != BlockShape.Solid && s2 != BlockShape.Solid)
		{
			float v = b2.FilterLight(pipe, amp, x, y);
			return lqs.Liquid.FilterLight(pipe, v, x, y, lqs.Amount);
		}

		return 0;
	}

	public int Corof(int xb)
	{
		return Mathf.FastFloor(xb / 16f) - chkOrigin;
	}

	private float wmix(float a, float b)
	{
		return Math.Max(a, b);
	}

	public override Chunk GetBufferedChunk(int xb)
	{
		int x = Corof(xb);
		if (x < 0 || x >= tmpChunks.Length || tmpChunks[x] == null) return level.GetChunkByBlock(x);

		return tmpChunks[x];
	}

}
