using Ethla.Client.Messaging;
using Ethla.Common.Mob;
using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Voxel;
using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Graphic;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public partial class OverlayMain : Gui
{

	private bool details;

	public override void Tick(ImVector2 cursor)
	{
		base.Tick(cursor);

		if (KeyBind.KeyF1.Pressed()) details = !details;

		MessageManager.Tick();
	}

	public override void Draw(Graphics graphics)
	{
		base.Draw(graphics);

		EntityPlayer player = Main.Player;
		Level level = Main.Level;

		if (!Main.Pausing)
		{
			DrawHotBarAndHint(graphics, player);
			DrawPStates(graphics, player);
		}

		MessageManager.Draw(graphics);

		if (!details) return;

		int dc0 = graphics.Drawcalls;

		EntityPlayer p = player;

		const float dy = 9;
		const float dx = 4;
		float hy = Size.Y;

		BlockState state = level.GetBlock(Main.HoverPos);
		Biome biome = level.GetBiome(Main.HoverPos.BlockX, Main.HoverPos.BlockY);

		graphics.DrawText($"DEBUGGER (press F1 to hide)", dx, hy -= dy);
		graphics.DrawText($"Total Seconds: {level.Seconds} Day Time = {level.Ticks % level.TicksPerDay}/{level.TicksPerDay}]", dx, hy -= dy);
		graphics.DrawText($"Highp Entities: {level.EntitiesById.Count} Lowp Entities: {level.LowpEntities.Count}", dx, hy -= dy);
		graphics.DrawText($"Player Velocity: {p.Velocity}", dx, hy -= dy);
		graphics.DrawText($"Player Position: {p.Pos}", dx, hy -= dy);
		graphics.DrawText($"Pointed Position: {Main.HoverPosPrecise}", dx, hy -= dy);
		graphics.DrawText($"Pointed Block: {state.Block.Uid.ToString()}#{state.Meta}#{state.SpecialMeta}", dx, hy -= dy);
		graphics.DrawText($"Pointed Biome: {biome.Uid.ToString()}", dx, hy -= dy);
		graphics.DrawText($"Tps: {Application.Tps}/{Application.MaxTps}", dx, hy -= dy);
		graphics.DrawText($"Fps: {Application.Fps}/{Application.MaxFps}", dx, hy -= dy);

		int dcminus = graphics.Drawcalls - dc0;

		graphics.DrawText($"Drawcall Count: {Application.Dcpt - dcminus}(-{dcminus})", dx, hy -= dy);

	}

}
