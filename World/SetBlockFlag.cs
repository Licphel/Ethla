using System;

namespace Ethla.World;

public class SetBlockFlag
{

    public const long NoFlag = 1 << 0;
    public const long Isolated = 1 << 1;

    public static bool Has(long v, long o)
    {
        return (v & o) != 0;
    }

}
