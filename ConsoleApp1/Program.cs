using FastReport;
using FastReport.Export.PdfSimple;
using ConsoleApp1;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

// Create input and output directories
string inputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input");
string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
if(!Directory.Exists(inputDir))
    Directory.CreateDirectory(inputDir);
if(!Directory.Exists(outputDir))
    Directory.CreateDirectory(outputDir);

// Paths to your prepared reports and scripts
string fpxPath = Path.Combine(inputDir, "Test.fpx"); // Place your .fpx here
string frxPath = Path.Combine(inputDir, "Test.frx"); // Place your .frx here
string docxPath = Path.Combine(outputDir, "Stress-Test.docx");
string workingDir = AppDomain.CurrentDomain.BaseDirectory;

Report report = new Report();
report.LoadPrepared(fpxPath);

// PDFSimpleExport

// Export to DOCX using the new DocxExport
using (var fs = File.Create(docxPath))
{
    var exporter = new DocxExport();
    exporter.Export(report, fs);
}

Console.WriteLine($"DOCX exported to {docxPath}");

// If you want to load .frx instead, uncomment:
// report.Load(frxPath);

// Export empty report to PDF
// PdfStrippedConverter pdfExporter = new();
// pdfExporter.CustomExport(report, pdfPath);
// Console.WriteLine($"PDF exported to {pdfPath}");
