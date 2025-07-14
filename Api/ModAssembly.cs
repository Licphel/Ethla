using System.Reflection;
using System.Text;
using Labyrinth;
using Spectrum.Core.Manage;
using Spectrum.IO;
using Spectrum.IO.Bin;
using Spectrum.IO.Pathfind;
using Spectrum.Utils;

namespace Ethla.Api;

public class ModAssembly
{

	public static Dictionary<string, Mod> Mods = new Dictionary<string, Mod>();

	public Assembly LibColct;
	public Type MainClass;
	public Mod MainObj;

	public static void InitMods(LoadingQueue uploader)
	{
		foreach (Mod m in Mods.Values)
		{
			m.Initialize(uploader);
			Logger.Info($"Init mod '{m.Space}' in directory {m.ModPathRoot.Name}.");
		}
	}

	public static void ScanLoad(PathHandle root)
	{
		if (!root.Exists)
			return;
		foreach (PathHandle file in root.Directories)
			Load(file);
	}

	public static ModAssembly Load(PathHandle path)
	{
		if (path.Find("disabled").Exists) return null;

		try
		{
			ExecutionResult result = ExecutionResult.From(new TextAccess(path.Find("mod.lab")));
			BinaryCompound compound = result.Result<BinaryCompound>();

			string mid = compound.Get<string>("id");
			string mv = compound.Get<string>("version");
			string mdesc = compound.Get<string>("description");
			string mtype = compound.Get<string>("main_class");
			string mdll = compound.Get<string>("library");

			Assembly assembly = Assembly.LoadFrom(path.Find(mdll).Path);

			ModAssembly ma = new ModAssembly();

			ma.LibColct = assembly;
			ma.MainClass = assembly.GetType(mtype);
			ma.MainObj = (Mod)Activator.CreateInstance(ma.MainClass);

			ma.MainObj.ModPathRoot = path;
			ma.MainObj.ModPathContent = path.Find("content");
			ma.MainObj.ModPathOverride = path.Find("override");
			ma.MainObj.Assembly = ma;
			ma.MainObj.Space = mid;
			ma.MainObj.Description = mdesc;
			ma.MainObj.Version = new SemanticVersion(mv);

			Mods[mid] = ma.MainObj;
			return ma;
		}
		catch (Exception e)
		{
			Logger.Info("Mod loading failed.");
			Logger.Fatal(e);
		}

		return null;
	}

	public static PathHandle GetFile(Id idt)
	{
		if (idt.Space == Bootstrap.Space)
			return Bootstrap.ContentPath.Find(idt.Key);
		Mod mod = Mods[idt.Space];
		return mod.ModPathContent.Find(idt.Key);
	}

	public static string GetCombinedVersionIdentity()
	{
		StringBuilder k = new StringBuilder();
		k.AppendJoin('-', Bootstrap.Version.Iteration);
		foreach (Mod mod in Mods.Values) k.AppendJoin('-', mod.Space + ":" + mod.Version.Iteration);
		return k.ToString();
	}

}
