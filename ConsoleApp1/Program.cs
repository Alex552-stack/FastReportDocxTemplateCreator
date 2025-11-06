using FastReport;
using FastReport.Export.PdfSimple;
using System.Diagnostics;
using ConsoleApp1;
using FastReport.Data;
using FastReport.Data.JsonConnection;
using System.IO;
using System.Threading;

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
string pdfPath = Path.Combine(outputDir, "Stress-Test.pdf");
string docxPath = Path.Combine(outputDir, "Stress-Test.docx");
string templateDocxPath = Path.Combine(outputDir, "Stress-Test-template.docx");
string pythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdf2docx_convert.py");
string workingDir = AppDomain.CurrentDomain.BaseDirectory;

Report report = new Report();
report.LoadPrepared(fpxPath);
// If you want to load .frx instead, uncomment:
// report.Load(frxPath);

// Export empty report to PDF
PdfStrippedConverter pdfExporter = new();
pdfExporter.CustomExport(report, pdfPath);
Console.WriteLine($"PDF exported to {pdfPath}");

// Convert PDF to DOCX template using Python script
try
{
    if (!File.Exists(pythonScriptPath))
    {
        Console.WriteLine($"Python script not found at: {pythonScriptPath}");
    }
    else
    {
        // Try a list of possible python launchers until one succeeds
        string[] candidates = new[] { "py", "python", "python3" };
        bool started = false;
        foreach (var candidate in candidates)
        {
            try
            {
                var args2 = candidate == "py" ? $"-3.11 \"{pythonScriptPath}\" \"{pdfPath}\" \"{templateDocxPath}\"" : $"\"{pythonScriptPath}\" \"{pdfPath}\" \"{templateDocxPath}\"";
                var psi = new ProcessStartInfo
                {
                    FileName = candidate,
                    Arguments = args2,
                    WorkingDirectory = workingDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using var process = Process.Start(psi);
                if (process == null)
                {
                    Console.WriteLine($"Attempted to start '{candidate}' but Process.Start returned null.");
                    continue;
                }
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
                if (!string.IsNullOrWhiteSpace(error))
                    Console.WriteLine($"Python error (from {candidate}): {error}");
                else
                    Console.WriteLine($"DOCX template exported to {templateDocxPath} (invoked with '{candidate}')");

                started = true;
                break;
            }
            catch (System.ComponentModel.Win32Exception wex)
            {
                // executable not found or cannot be started, try next candidate
                Console.WriteLine($"Could not start '{candidate}': {wex.Message}");
            }
        }

        if (!started)
        {
            Console.WriteLine("Failed to start any Python executable. Make sure Python is installed and available on PATH, or the 'py' launcher is present.");
        }
        else
        {
            // give a short breathing room for file handles to be released by external processes
            Thread.Sleep(250);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to convert PDF to DOCX: {ex.Message}");
}

// // Export report structure directly to DOCX template using custom exporter
// try
// {
//     var docxExporter = new DocxTemplateExport(docxPath);
//     docxExporter.Export(report, docxPath);
//     Console.WriteLine($"DOCX template exported to {docxPath} using custom exporter.");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Failed to export DOCX template: {ex.Message}");
// }
