using ConsoleApp1.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ConsoleApp1.Classes.Table;

public static class WordTable
{

    private static int GetTotalTableWidthInTwips(WordTableRow row)
    {
        int totalWidth = 0;
        foreach (var cell in row.Cells)
        {
            totalWidth += cell.Width;
        }

        return NumberHelpers.PixelsToTwips(totalWidth);
    }
    public static DocumentFormat.OpenXml.Wordprocessing.Table CreateTable(List<WordTableRow> rows)
    {
        var table = new DocumentFormat.OpenXml.Wordprocessing.Table();

        // Table properties: optional, e.g., fixed layout
        table.AppendChild(new TableProperties(
            new TableLayout { Type = TableLayoutValues.Fixed },
            new TableWidth
            {
                Type = TableWidthUnitValues.Dxa,
                Width = GetTotalTableWidthInTwips(rows.First()).ToString() // sum of all cell widths
            }
        ));


        foreach (var row in rows)
        {
            TableRow tableRow = new TableRow();

            foreach (var cell in row.Cells)
            {
                TableCell tableCell = new TableCell();

                // Cell properties: borders + width
                TableCellProperties cellProps = new TableCellProperties(
                    new TableCellWidth
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = NumberHelpers.PixelsToTwips(cell.Width).ToString()
                    },
                    new TableCellBorders(
                        new TopBorder
                        {
                            Val = cell.Borders.Top.Style,
                            Size = (UInt32Value)(uint)cell.Borders.Top.Size,
                            Color = cell.Borders.Top.Color
                        },
                        new BottomBorder
                        {
                            Val = cell.Borders.Bottom.Style,
                            Size = (UInt32Value)(uint)cell.Borders.Bottom.Size,
                            Color = cell.Borders.Bottom.Color
                        },
                        new LeftBorder
                        {
                            Val = cell.Borders.Left.Style,
                            Size = (UInt32Value)(uint)cell.Borders.Left.Size,
                            Color = cell.Borders.Left.Color
                        },
                        new RightBorder
                        {
                            Val = cell.Borders.Right.Style,
                            Size = (UInt32Value)(uint)cell.Borders.Right.Size,
                            Color = cell.Borders.Right.Color
                        }
                    )
                );

                tableCell.Append(cellProps);
                
                tableCell.Append(new WordParagraph(cell.TextObject).Export());
                row.LastObject = cell.TextObject;
                

                tableRow.Append(tableCell);
            }

            table.Append(tableRow);
        }
        

        return table;
    }
}