using System.Text;

namespace Ethla.Util;

public class NumberForm
{

	private static readonly string[] Namings = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

	public static string GetCompress(float i)
	{
		if (i >= 1000_000_000)
		{
			string formatted = Math.Round(i / 1000_000_000d, 2).ToString("0.#");
			return formatted + "G";
		}

		if (i >= 1000_000)
		{
			string formatted = Math.Round(i / 1000_000d, 2).ToString("0.#");
			return formatted + "M";
		}

		if (i >= 1000)
		{
			string formatted = Math.Round(i / 1000d, 2).ToString("0.#");
			return formatted + "K";
		}

		return i.ToString();
	}

	public static string GetRoman(int i)
	{
		StringBuilder builder = new StringBuilder();
		while (i > 0)
		{
			if (i >= 1000)
			{
				i -= 1000;
				builder.Append('M');
				continue;
			}

			if (i >= 500)
			{
				i -= 500;
				builder.Append('D');
				continue;
			}

			if (i >= 100)
			{
				i -= 100;
				builder.Append('C');
				continue;
			}

			if (i >= 50)
			{
				i -= 50;
				builder.Append('L');
				continue;
			}

			if (i >= 10)
			{
				i -= 10;
				builder.Append('X');
				continue;
			}

			string si = i.ToString();
			int last = si[^1] - '0';
			builder.Append(Namings[last]);
			i -= last;
		}

		return builder.ToString();
	}

}
