using System;
using Spectrum.Graphic;
using Spectrum.Maths;
using Spectrum.Maths.Curves;

namespace Ethla.Client;

public static class RenderHelper
{

    public static void DrawRope(Graphics graphics, float x1, float y1, float x2, float y2)
    {
        float minx = Math.Min(x1, x2);
        float maxx = Math.Max(x1, x2);
        float miny = Math.Min(y1, y2);
        float maxy = Math.Max(y1, y2);
        float ln = Math.Abs(x1 - x2);

        Bezier bezier;

        if (x1 < x2)
        {
            bezier = new Bezier(
                new Vector2(x1, y1),
                new Vector2(minx + ln * 0.33f, miny - ln * 0.25f),
                new Vector2(minx + ln * 0.66f, miny - ln * 0.25f),
                new Vector2(x2, y2)
            );
        }
        else
        {
            bezier = new Bezier(
                new Vector2(x2, y2),
                new Vector2(minx + ln * 0.33f, miny - ln * 0.25f),
                new Vector2(minx + ln * 0.66f, miny - ln * 0.25f),
                new Vector2(x1, y1)
            );
        }

        graphics.DrawCurve(bezier);
    }

}
