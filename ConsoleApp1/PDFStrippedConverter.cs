using System.Text.RegularExpressions;
using FastReport;
using FastReport.Export.PdfSimple;

namespace ConsoleApp1;

public class PdfStrippedConverter : PDFSimpleExport
{
    private HashSet<string> _usedNames = new();
    
    protected override void ExportBand(BandBase band)
    {
        Console.WriteLine(band.Name);
        Console.WriteLine(band.BaseName);
        Console.WriteLine(band.ClassName);

        if (!_usedNames.Add(band.Name))
            return;

        foreach (Base c in band.ForEachAllConvectedObjects(this))
        {
            if (c.GetType() == typeof(TextObject))
            {
                var typedTextObj = (TextObject)c;

                int requiredLength = typedTextObj.Text.Length;

                string filtered = Regex.Replace(
                    typedTextObj.Name,
                    "[^A-Z0-9]", // remove everything except uppercase or digits
                    ""
                );

// pad with 'p' to match the original length
                if (filtered.Length < requiredLength)
                    filtered = filtered.PadRight(requiredLength, 'p');

                typedTextObj.Text =  filtered;


            }
        }
        
        base.ExportBand(band);
        
    }

    // private void ExportObj(Base obj)
    // {
    //     if (pageGraphics != null)
    //     {
    //         if (obj is ReportComponentBase && (obj as ReportComponentBase).Exportable)
    //             (obj as ReportComponentBase).Draw(new FRPaintEventArgs(pageGraphics, scaleFactor, scaleFactor, Report.GraphicCache));
    //     }
    // }

    public void CustomExport(Report report, string filePath)
    {
        base.Export(report, filePath);
    }
}