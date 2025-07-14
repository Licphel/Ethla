using Spectrum.Utils;

namespace Ethla;

public class Launch
{

	public static void Main(string[] args)
	{
		foreach (string s in args)
		{
			Logger.Info($"Launching args '{s}' detected.");
		}
		Bootstrap.Launch();
	}

}
