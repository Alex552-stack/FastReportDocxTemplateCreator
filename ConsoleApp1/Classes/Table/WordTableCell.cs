using FastReport;

namespace ConsoleApp1.Classes.Table;

public class WordTableCell
{
    public TextObject TextObject { get; set; } = new();
    public int Width { get; set; } // in pixels
    public CellBorders Borders { get; set; } = new();
}

