using Spectrum.Core.Manage;
using Spectrum.IO.Pathfind;
using Spectrum.Utils;

namespace Ethla.Api;

public class Mod
{

	public ModAssembly Assembly;
	public string Description;
	public PathHandle ModPathContent; //The content directory of the mod.
	public PathHandle ModPathOverride; //The content directory of the mod.
	public PathHandle ModPathRoot; //The root directory of the mod.
	public string Space;
	public SemanticVersion Version;

	//Here, a mod should scan in your #this.ModFileRoot and add a subloader to loader.
	//Please go to #Ethla.Ethla to learn how to use it.
	public virtual void Initialize(LoadingQueue loader)
	{
	}

	//Utilities methods

	public LoadingQueue GetLoadingQueue()
	{
		LoadingQueue upl = LoadingManager.GetUploadQueueWithProcessors(Space);
		upl.Filebase = ModPathContent;
		return upl;
	}

	public LoadingQueue GetOverridingQueue(string space)
	{
		LoadingQueue upl = LoadingManager.GetUploadQueueWithProcessors(space);
		upl.Filebase = ModPathRoot.Find($"override/{space}");
		return upl;
	}

}
