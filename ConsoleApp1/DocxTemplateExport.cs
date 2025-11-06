using System.IO;
using FastReport;
using FastReport.Export;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ConsoleApp1
{
    // Custom export class to generate DOCX template from a prepared FastReport
    public class DocxTemplateExport : ExportBase
    {
        private DocX doc = null!; // initialized in Start()
        private Table? table;
        private string docxPath;

        public DocxTemplateExport(string outputPath)
        {
            docxPath = outputPath;
        }

        protected override void Start()
        {
            // If an existing file is present, try to remove it first to avoid sharing violations
            try
            {
                if (File.Exists(docxPath))
                {
                    // attempt to delete the file with a few retries if it's locked
                    int tries = 0;
                    while (File.Exists(docxPath) && tries < 5)
                    {
                        try
                        {
                            File.Delete(docxPath);
                        }
                        catch
                        {
                            Thread.Sleep(100);
                        }
                        tries++;
                    }
                }
            }
            catch
            {
                // swallow any errors here; we'll still attempt to create the doc
            }

            doc = DocX.Create(docxPath);
        }

        protected override void ExportPageBegin(ReportPage page)
        {
            // Create a new table for each page (if needed)
            table = doc.AddTable(page.Bands.Count, 1); // 1 column for simplicity, can be adjusted
        }

        protected override void ExportBand(BandBase band)
        {
            // Add a row for each band, with placeholder text
            if (table == null)
            {
                // fall back: create a minimal table if ExportBand is called unexpectedly
                table = doc.AddTable(1, 1);
            }
            int rowIndex = Math.Max(0, table.Rows.Count - 1);
            table.Rows[rowIndex].Cells[0].Paragraphs[0].Append($"{{{band.Name}}}");
        }

        protected override void ExportPageEnd(ReportPage page)
        {
            if (table != null)
                doc.InsertTable(table);
            // clear table to avoid reusing it across pages
            table = null;
        }

        protected override void Finish()
        {
            doc.Save();
        }
    }
}
