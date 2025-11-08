using DocumentFormat.OpenXml.Wordprocessing;

namespace ConsoleApp1.Classes.Table;

public class BorderInfo
{
    public BorderValues Style { get; set; } = BorderValues.Single;
    public int Size { get; set; } = 4; // 1/8 pt
    public string Color { get; set; } = "000000"; // hex RGB
}