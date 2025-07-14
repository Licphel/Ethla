using Ethla.World;
using Ethla.World.Voxel;
using Spectrum.Graphic;

namespace Ethla.Client.Voxel;

public interface BlockRenderer
{

	public static Dictionary<string, BlockRenderer> Renderers = new Dictionary<string, BlockRenderer>();

	public static BlockRenderer Single = new BlockRendererSingle();
	public static BlockRenderer Repeat = new BlockRendererRepeat();
	public static BlockRenderer Random = new BlockRendererRandom();

	static BlockRenderer()
	{
		Renderers["repeat"] = Repeat;
		Renderers["single"] = Single;
		Renderers["random"] = Random;
	}

	public void Draw(Graphics graphics, Level level, Chunk chunk, BlockState state, int x, int y);

	public void DrawItemSymbol(Graphics graphics, BlockState state, float x, float y, float w, float h);

	public void SetState(Graphics graphics);

	public void ResetState(Graphics graphics);

	//requirements:
	//painter == null: false
	//painter == self: true
	public bool IsInSameState(BlockRenderer renderer)
	{
		return renderer == this;
	}

}
