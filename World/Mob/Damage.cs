using Spectrum.Graphic.Text;

namespace Ethla.World.Mob;

public record struct Damage(DamageSource Src, DamageType Type, float Value)
{

	public Lore GetMurderInfo()
	{
		string s1 = Type.GetLocalizationName();
		if (Src == null)
			return Lore.Translate("ethla:text.death_reasonless", s1);
		string s0 = Src.GetDamageSourceInfo(this);
		return Lore.Translate("ethla:text.death", s0, s1);
	}

}
