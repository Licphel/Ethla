using Labyrinth;
using Spectrum.Core.Manage;
using Spectrum.Graphic.Ui;
using Spectrum.IO;
using Spectrum.IO.Bin;

namespace Ethla;

public class Options
{

	public static string LanguageGroup;
	public static bool AutoIconify, Maximized, AutoRes;
	public static string Title, CursorLoc, IconLoc, FontLoc, FontLatinLoc;
	public static int FontSize, FontLatinSize;
	public static BinaryList Hotspot;
	public static float Ratio;

	public static void Load()
	{
		ExecutionResult result = ExecutionResult.From(new TextAccess(Bootstrap.OptionPath));
		BinaryCompound compound = result.Context;

		LanguageGroup = compound.Get<string>("language_group");

		CursorLoc = compound.Search<string>("cursor");
		Hotspot = compound.Search<BinaryList>("cursor_hotspot");
		IconLoc = compound.Search<string>("icon");
		Title = compound.Search<string>("title");
		Title = Title.Replace("${game_version}", Bootstrap.Version.FullName);
		AutoIconify = compound.Search<bool>("auto_iconify");
		Maximized = compound.Search<bool>("maximized");
		AutoRes = compound.Search<bool>("auto_resolution");
		Ratio = compound.Search<float>("ratio");
		FontLoc = compound.Search<string>("font");
		FontLatinLoc = compound.Search<string>("font_latin");
		FontSize = compound.Search<int>("font_size");
		FontLatinSize = compound.Search<int>("font_latin_size");

		//Settings builtin
		I18N.LangKey = LanguageGroup;
		Resolution.AllowResolution = AutoRes;

		if (!AutoRes)
			Resolution.GlobalLocked = compound.Search<float>("gui_scale");
	}

}
