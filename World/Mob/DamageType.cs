using Spectrum.Core.Manage;

namespace Ethla.World.Mob;

public class DamageType : IdHolder
{

	public string GetLocalizationName()
	{
		return I18N.GetText($"{Uid.Space}:damage_type.{Uid.Key}");
	}

}
