# FastReport PDF/DOCX Template Automation

## How to Use

1. **Clone this repository**
2. **Place your `.fpx` and `.frx` files in the `input/` folder**
3. **Install .NET 9 SDK and Python 3.11**
4. **Install NuGet dependencies:**
   - Run `dotnet restore` in the project folder
5. **Install Python dependencies:**
   - Run `pip install -r requirements.txt` (use Python 3.11)
6. **Run the project:**
   - `dotnet run --project ConsoleApp1/ConsoleApp1.csproj`

## Output
- PDF and DOCX templates will be generated in the `output/` folder.

## Notes
- The workflow uses relative paths for portability.
- You can use the generated DOCX template for further data population using C# DocX or Open XML SDK.
- If you want to use a different report, update the filenames in `input/` and adjust the code if needed.
pdf2docx

