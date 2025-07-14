using System.Runtime.CompilerServices;
using Ethla.World;
using Ethla.World.Lighting;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public class BlockRendererSingle : BlockRenderer
{

	private static readonly string Vert = Loads.Get("ethla:shader/block_vert.shd");
	private static readonly string Frag = Loads.Get("ethla:shader/block_frag.shd");
	private static ShaderUniform uniMask;

	private static readonly Shader Program = Surface.Current.CompileShaderGlsl(Vert, Frag, program =>
	{
		ShaderAttrib posAttrib = program.GetAttrib("i_position");
		ShaderAttrib colAttrib = program.GetAttrib("i_color");
		ShaderAttrib texAttrib = program.GetAttrib("i_texCoord");
		ShaderAttrib texAttrib1 = program.GetAttrib("i_texCoord1");

		posAttrib.Pointer(ShaderAttrib.Type.Float, 2, 40, 0);
		colAttrib.Pointer(ShaderAttrib.Type.Float, 4, 40, 8);
		texAttrib.Pointer(ShaderAttrib.Type.Float, 2, 40, 24);
		texAttrib1.Pointer(ShaderAttrib.Type.Float, 2, 40, 32);

		uniMask = program.GetUniform("u_mask");
	});

	private static float u1, v1, u2, v2;
	private static Image prevMask;
	private static readonly VertexAppender[] Vertappd = new VertexAppender[4];
	private static readonly UniformAppender Uniappd;

	static BlockRendererSingle()
	{
		Vertappd[0] = b => b.Buffer.Append(u1).Append(v2);
		Vertappd[1] = b => b.Buffer.Append(u1).Append(v1);
		Vertappd[2] = b => b.Buffer.Append(u2).Append(v1);
		Vertappd[3] = b => b.Buffer.Append(u2).Append(v2);
		Uniappd = b => uniMask.SetImageUnit(1, prevMask);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Draw(Graphics graphics, Level level, Chunk chunk, BlockState state, int x, int y)
	{
		LightWare lights = chunk.GetSubLightware(x, y);
		Image mask = BlockModels.GetMask(state);

		if (mask != prevMask)
		{
			if (prevMask != null) graphics.Flush();
			prevMask = mask;
		}

		DrawDetailed(graphics, level, chunk, state, x, y, lights);
	}

	public void SetState(Graphics graphics)
	{
		graphics.UseShader(Program);
		graphics.UniformAppender = Uniappd;
		graphics.VertAppenders = Vertappd;
	}

	public void ResetState(Graphics graphics)
	{
		graphics.UseDefaultShader();
		graphics.UniformAppender = null;
		graphics.VertAppenders = null;
		graphics.NormalizeColor();
		prevMask = null;
	}

	public void DrawItemSymbol(Graphics graphics, BlockState state, float x, float y, float w, float h)
	{
		SetState(graphics);
		prevMask = BlockModels.GetMask(state);
		int u = 112, v = 0;
		u1 = u / 128f;
		v1 = v / 128f;
		u2 = (u + 16) / 128f;
		v2 = (v + 16) / 128f;
		drawItemSymbolInternal(graphics, state, x, y, w, h);
		ResetState(graphics);
	}

	public void DrawDetailed(Graphics graphics, Level level, Chunk chunk, BlockState state, int x, int y, LightWare lights)
	{
		int u = 0, v = 0;

		Chunk cleft = level.LightEngine.GetBufferedChunk(x - 1);
		Chunk cright = level.LightEngine.GetBufferedChunk(x + 1);

		if (cleft == null || cright == null || chunk == null) return;

		BlockState upBlock = chunk.GetBlock(x, y + 1);
		BlockState downBlock = chunk.GetBlock(x, y - 1);
		BlockState leftBlock = cleft.GetBlock(x - 1, y);
		BlockState rightBlock = cright.GetBlock(x + 1, y);

		bool up = y == Chunk.MaxY || BlockModels.IsConnectable(upBlock, state, Direction.Down) || BlockModels.IsConnectable(state, upBlock, Direction.Up);
		bool down = y == 0 || BlockModels.IsConnectable(downBlock, state, Direction.Up) || BlockModels.IsConnectable(state, downBlock, Direction.Down);
		bool left = BlockModels.IsConnectable(leftBlock, state, Direction.Right) || BlockModels.IsConnectable(state, leftBlock, Direction.Left);
		bool right = BlockModels.IsConnectable(rightBlock, state, Direction.Left) || BlockModels.IsConnectable(state, rightBlock, Direction.Right);

		bool upc = BlockModels.IsSpreadable(upBlock, state, Direction.Down) || BlockModels.IsSpreadable(state, upBlock, Direction.Up);
		bool downc = BlockModels.IsSpreadable(downBlock, state, Direction.Up) || BlockModels.IsSpreadable(state, downBlock, Direction.Down);
		bool leftc = BlockModels.IsSpreadable(leftBlock, state, Direction.Right) || BlockModels.IsSpreadable(state, leftBlock, Direction.Left);
		bool rightc = BlockModels.IsSpreadable(rightBlock, state, Direction.Left) || BlockModels.IsSpreadable(state, rightBlock, Direction.Right);

		if (up && down) u = 0;
		else if (up && !down) u = 17;
		else if (!up && down) u = 34;
		else if (!up && !down) u = 51;

		if (left && right) v = 0;
		else if (left && !right) v = 17;
		else if (!left && right) v = 34;
		else if (!left && !right) v = 51;

		u1 = u / 128f;
		v1 = v / 128f;
		u2 = (u + 16) / 128f;
		v2 = (v + 16) / 128f;

		lights.GetBlockLights(graphics.VertexColors, false);

		drawInternal(graphics, state, null, x, y);

		if (lights.IsDark)
			return;

		int pid = state.StateUid.GetHashCode();

		if (leftc && left && !leftBlock.IsEmpty)
		{
			if (leftBlock.StateUid.GetHashCode() > pid)
			{
				lights = cleft.GetSubLightware(x - 1, y);
				lights.GetBlockLights(graphics.VertexColors, false);
				u1 = 0 / 128f;
				v1 = 68 / 128f;
				u2 = (0 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				drawInternal(graphics, state, Direction.Left, x - 1, y);
			}
			else
			{
				lights = chunk.GetSubLightware(x, y);
				lights.GetBlockLights(graphics.VertexColors, false);
				u1 = 34 / 128f;
				v1 = 68 / 128f;
				u2 = (34 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				drawInternal(graphics, leftBlock, Direction.Right, x, y);
			}
		}

		if (downc && down && !downBlock.IsEmpty)
		{
			if (downBlock.StateUid.GetHashCode() > pid)
			{
				lights = chunk.GetSubLightware(x, y - 1);
				lights.GetBlockLights(graphics.VertexColors, false);
				u1 = 17 / 128f;
				v1 = 68 / 128f;
				u2 = (17 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				drawInternal(graphics, state, Direction.Down, x, y - 1);
			}
			else
			{
				lights = chunk.GetSubLightware(x, y);
				lights.GetBlockLights(graphics.VertexColors, false);
				u1 = 51 / 128f;
				v1 = 68 / 128f;
				u2 = (51 + 16) / 128f;
				v2 = (68 + 16) / 128f;
				drawInternal(graphics, downBlock, Direction.Up, x, y);
			}
		}
	}

	public void DrawFast(Graphics graphics, Level level, Chunk chunk, BlockState state, int x, int y, LightWare lights)
	{
		u1 = v1 = 0;
		u2 = v2 = 16 / 128f;
		lights.GetBlockLights(graphics.VertexColors, false);
		drawInternal(graphics, state, null, x, y);
	}

	//Override this for simple customized behaviors.
	//If looking for advanced functions, you can extend the base class.
	protected virtual void drawInternal(Graphics graphics, BlockState state, Direction overlapping, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockModels.GetIcon(state);
		if (icon != null) graphics.DrawIcon(icon, x, y, w, h);
	}

	//Override this for simple customized behaviors.
	//If looking for advanced functions, you can extend the base class.
	protected virtual void drawItemSymbolInternal(Graphics graphics, BlockState state, float x, float y, float w = 1, float h = 1)
	{
		Icon icon = BlockModels.GetIcon(state);
		if (icon != null) graphics.DrawIcon(icon, x, y, w, h);
	}

}
