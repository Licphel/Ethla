using System;
using System.Runtime.CompilerServices;
using Ethla.World;
using Ethla.World.Lighting;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Voxel;

public interface WallRenderer
{

	public static Dictionary<string, WallRenderer> Renderers = new Dictionary<string, WallRenderer>();

    public static WallRenderer Single = new WallRendererSingle();
 	public static WallRenderer Repeat = new WallRendererRepeat();

	static WallRenderer()
	{
		Renderers["single"] = Single;
		Renderers["repeat"] = Repeat;
	}

    public void Draw(Graphics graphics, Level level, Chunk chunk, Wall wall, int x, int y);

	public void DrawItemSymbol(Graphics graphics, Wall wall, float x, float y, float w, float h);

	public void SetState(Graphics graphics);

	public void ResetState(Graphics graphics);

	//requirements:
	//painter == null: false
	//painter == self: true
	public bool IsInSameState(WallRenderer renderer)
	{
		return renderer == this;
	}

}
