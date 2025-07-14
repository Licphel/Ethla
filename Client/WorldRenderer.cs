using Ethla.Client.Voxel;
using Ethla.World;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.Client;

public class WorldRenderer
{

	private static readonly List<Entity> Cache = new List<Entity>();

	public static void Draw(Graphics graphics, Level level, Quad camera)
	{
		drawWalls(graphics, level, camera);
		drawBlocks(graphics, level, camera, true);
		level.GetNearbyEntities(Cache, camera, null, 8);
		foreach (Entity e in Cache)
			e?.Draw(graphics);
		drawLiquidLayer(graphics, level, camera);
		drawBlocks(graphics, level, camera);
		level.LowpEntities.Draw(graphics);
	}

	private static void drawWalls(Graphics graphics, Level level, Quad camera)
	{
		int cy0 = Mathf.Round(camera.Y - 1);
		int cy1 = Mathf.Round(camera.Yprom + 1);
		int cx0 = Mathf.Round(camera.X - 1);
		int cx1 = Mathf.Round(camera.Xprom + 1);

		cy0 = Math.Clamp(cy0, 0, Chunk.Height);
		cy1 = Math.Clamp(cy1, 0, Chunk.Height);

		WallRenderer lastp = null;

		for (int x = cx0; x <= cx1; x++)
		{
			Chunk c = level.GetChunkByBlock(x);

			if (c == null) continue;

			for (int y = cy0; y < cy1; y++)
			{
				Wall wall = c.GetWall(x, y);

				if (wall == Wall.Empty) continue;
				
				WallRenderer painter = WallModels.GetRenderer(wall);

				if (painter == null) continue;

				if (!painter.IsInSameState(lastp))
				{
					lastp?.ResetState(graphics);
					painter.SetState(graphics);
				}

				lastp = painter;

				painter.Draw(graphics, level, c, wall, x, y);
			}
		}

		lastp?.ResetState(graphics);
	}

	private static void drawBlocks(Graphics graphics, Level level, Quad camera, bool trywall = false)
	{
		int cy0 = Mathf.Round(camera.Y - 1);
		int cy1 = Mathf.Round(camera.Yprom + 1);
		int cx0 = Mathf.Round(camera.X - 1);
		int cx1 = Mathf.Round(camera.Xprom + 1);

		cy0 = Math.Clamp(cy0, 0, Chunk.Height);
		cy1 = Math.Clamp(cy1, 0, Chunk.Height);

		BlockRenderer lastp = null;

		for (int x = cx0; x <= cx1; x++)
		{
			Chunk c = level.GetChunkByBlock(x);

			if (c == null) continue;

			for (int y = cy0; y < cy1; y++)
			{
				BlockState state = c.GetBlock(x, y);

				if (state.IsEmpty || state.SpecialMeta == BlockState.Invisible) continue;
				if (trywall == !state.IsWallAttached()) continue;

				BlockRenderer painter = BlockModels.GetRenderer(state);

				if (painter == null) continue;

				if (!painter.IsInSameState(lastp))
				{
					lastp?.ResetState(graphics);
					painter.SetState(graphics);
				}

				lastp = painter;

				painter.Draw(graphics, level, c, state, x, y);
			}
		}

		lastp?.ResetState(graphics);
	}

	private static void drawLiquidLayer(Graphics graphics, Level level, Quad camera)
	{
		int cy0 = Mathf.Round(camera.Y - 1);
		int cy1 = Mathf.Round(camera.Yprom + 1);
		int cx0 = Mathf.Round(camera.X - 1);
		int cx1 = Mathf.Round(camera.Xprom + 1);

		cy0 = Math.Clamp(cy0, 0, Chunk.MaxY);
		cy1 = Math.Clamp(cy1, 0, Chunk.MaxY);

		for (int x = cx0; x <= cx1; x++)
		{
			Chunk c = level.GetChunkByBlock(x);

			if (c == null) continue;

			for (int y = cy0; y < cy1; y++) LiquidRenderer.Draw(graphics, level, c, x, y);
		}
	}

}
