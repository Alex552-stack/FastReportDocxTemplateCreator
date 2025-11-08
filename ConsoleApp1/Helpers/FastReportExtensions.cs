using ConsoleApp1.Classes.Table;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using Color = System.Drawing.Color;

namespace ConsoleApp1.Helpers;

public static  class FastReportExtensions
{
    public static IEnumerable<ReportComponentBase> GetTextObjectsOrdered(this BandBase band)
    {
        return band
            .ForEachAllConvectedObjects(null) // pass null if you don’t need ExportBase
            .OfType<ReportComponentBase>()
            .OrderBy(to => to.Top)   // top to bottom
            .ThenBy(to => to.Left);  // left to right
    }
    
    public static BorderInfo ConvertFRBorder(BorderLine? frBorder)
    {
        if (frBorder == null) return new BorderInfo { Style = BorderValues.Nil, Size = 0 };
        if(frBorder.Color.A == 255) // Transparent
            return new BorderInfo { Style = BorderValues.Nil, Size = 0 };

        return new BorderInfo
        {
            Style = frBorder.Style switch
            {
                LineStyle.Solid => BorderValues.Single,
                LineStyle.Double => BorderValues.Double,
                LineStyle.Dash => BorderValues.DashSmallGap,
                LineStyle.Dot => BorderValues.Dotted,
                _ => BorderValues.Nil
            },
            Size = (int)(frBorder.Width * 8), // FR width is probably in pixels; OpenXML uses 1/8 pt units for borders
            Color = NumberHelpers.ToOpenXmlHexWithOpacity(frBorder.Color)
        };
    }


}