using System.Drawing;
using FastReport.Utils;

namespace ConsoleApp1.Helpers;

public static class NumberHelpers
{
    public static int PixelsToPoints(double pixels)
        => (int)(pixels / Units.Inches * 72.0);
    
    public static int PixelsToTwips(double pixels)
        => (int)(PixelsToPoints(pixels) * 20.0);
    
    public static string ToOpenXmlHexWithOpacity(Color color)
    {
        if (color == Color.Empty)
            return "000000"; // default black

        // Convert alpha to 0..1
        double alpha = color.A / 255.0;

        // Background = white
        int r = (int)((1 - alpha) * 255 + alpha * color.R);
        int g = (int)((1 - alpha) * 255 + alpha * color.G);
        int b = (int)((1 - alpha) * 255 + alpha * color.B);

        // Clamp to 0-255
        r = Math.Min(255, Math.Max(0, r));
        g = Math.Min(255, Math.Max(0, g));
        b = Math.Min(255, Math.Max(0, b));

        return (r << 16 | g << 8 | b).ToString("X6");
    }


}