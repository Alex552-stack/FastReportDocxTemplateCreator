using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ConsoleApp1.Classes;
using ConsoleApp1.Classes.Table;
using ConsoleApp1.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using FastReport.Export;
using FastReport.Utils;

namespace ConsoleApp1;

/// <summary>
/// Simple DOCX exporter using DocX (Novacode.DocX) v1.0.0.22.
/// This initial implementation exports every report page as a raster image inserted into a DOCX file.
/// It's a pragmatic first step so you no longer need the external python-based pdf->docx flow.
/// </summary>
public class DocxExport : ExportBase
{
    private WordprocessingDocument wordDoc = null!;
    private ReportComponentBase? _lastObject;
    private Body body = null!;
    private List<WordTableRow>? _currentRows;


    protected override void Start()
    {
        base.Start();

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "testare.docx");

        // Create a new Word document
        wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);

        // Add main document part
        var mainPart = wordDoc.AddMainDocumentPart();
        mainPart.Document = new Document();
        body = mainPart.Document.AppendChild(new Body());
    }

    protected override void ExportPageBegin(ReportPage page)
    {
        _lastObject = null;
        base.ExportPageBegin(page);
    }

    protected override void ExportPageEnd(ReportPage page)
    {
        // Insert page break at the end of each page
        body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));

        base.ExportPageEnd(page);
    }

    protected override void ExportBand(BandBase band)
    {
        base.ExportBand(band);

        _lastObject = band;

        switch (band)
        {
            case DataHeaderBand headerBand:
                StartNewTable(headerBand);
                break;
            case DataBand dataBand:
                AddDataRow(dataBand);
                break;
            default:
                ExportUntypedBand(band);
                break;
        }
    }

    private void AddDataRow(DataBand dataBand)
    {
        if (_currentRows == null) _currentRows = new List<WordTableRow>();

        var wordRow = new WordTableRow();
        foreach (var c in dataBand.GetTextObjectsOrdered())
        {
            var cell = new WordTableCell
            {
                Width = (int)c.Width,
                TextObject = (TextObject)c,
                Borders = new CellBorders
                {
                    Top = new BorderInfo { Style = BorderValues.Single, Size = 4 },
                    Bottom = new BorderInfo { Style = BorderValues.Single, Size = 4 },
                    Left = new BorderInfo { Style = BorderValues.Single, Size = 4 },
                    Right = new BorderInfo { Style = BorderValues.Single, Size = 4 }
                }
            };

            wordRow.Cells.Add(cell);
            _lastObject = c;
        }

        _currentRows.Add(wordRow);
    }



    private void StartNewTable(DataHeaderBand headerBand)
    {
        // Flush any existing table first
        FlushCurrentTable(); 

        _currentRows = new List<WordTableRow>();

        var headerRow = new WordTableRow();
        foreach (var c in headerBand.GetTextObjectsOrdered())
        {
            var textObj = (TextObject)c;

            var cell = new WordTableCell
            {
                Width = (int)c.Width,
                TextObject = textObj,
                Borders = new CellBorders
                {
                    Top = FastReportExtensions.ConvertFRBorder(textObj.Border.TopLine),
                    Bottom = FastReportExtensions.ConvertFRBorder(textObj.Border.BottomLine),
                    Left = FastReportExtensions.ConvertFRBorder(textObj.Border.LeftLine),
                    Right = FastReportExtensions.ConvertFRBorder(textObj.Border.RightLine)
                }
            };

            headerRow.Cells.Add(cell);
            _lastObject = c;
        }

        _currentRows.Add(headerRow);
    }


    private void FlushCurrentTable()
    {
        if (_currentRows == null || _currentRows.Count == 0) return;

        var currentTable = WordTable.CreateTable(_currentRows);
        body.Append(currentTable);

        _currentRows = null;
    }


    private void ExportUntypedBand(BandBase band)
    {
        foreach (var c in band.GetTextObjectsOrdered())
        {
            ExportObj(c);
            _lastObject = c;
        }
    }

    protected void ExportObj(Base obj)
    {
        if (obj is not TextObject textObj) return;

        var paragraph = new WordParagraph(textObj, _lastObject!).Export();

        body.Append(paragraph);
    }

    protected override void Finish()
    {
        base.Finish();

        FlushCurrentTable();

        // Save and close the document
        wordDoc.MainDocumentPart?.Document.Save();
        wordDoc.Dispose(); // This will flush everything and close the file
        wordDoc = null;
    }
}