using FastReport;

namespace ConsoleApp1.Classes.Table;

public class WordTableRow
{
    public List<WordTableCell> Cells { get; set; } = new();
    public ReportComponentBase? LastObject { get; set; } // for vertical spacing tracking
}
