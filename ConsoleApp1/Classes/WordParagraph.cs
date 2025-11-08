using ConsoleApp1.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;

namespace ConsoleApp1.Classes;

public class WordParagraph 
{
    private readonly TextObject _textObject;
    private readonly ReportComponentBase? _lastObject;
    
    public WordParagraph(TextObject textObject, ReportComponentBase? lastObject = null)
    {
        _textObject = textObject;
        _lastObject = lastObject;
    }

    public OpenXmlCompositeElement Export()
    {
        var paragraphProps = CreateParagraphProperties();
        var run = CreateRun();

        return new Paragraph(paragraphProps, run);
    }
    
    private ParagraphProperties CreateParagraphProperties()
    {
        // Horizontal offsets in twips
        double leftTwips = 0;

        // Vertical spacing before paragraph
        double topTwips = 0;
        if (_lastObject != null)
        {
            double spacingPixels = _textObject.Top - _lastObject.Bottom;
            topTwips = Math.Max(0, NumberHelpers.PixelsToTwips(spacingPixels));
            
            leftTwips = NumberHelpers.PixelsToTwips(_textObject.Left);
        }

        var paragraphProps = new ParagraphProperties
        {
            SpacingBetweenLines = new SpacingBetweenLines { Before = ((int)topTwips).ToString() }
        };

        // Map alignment and apply horizontal offsets correctly
        switch (_textObject.HorzAlign)
        {
            case HorzAlign.Left:
                paragraphProps.Justification = new Justification { Val = JustificationValues.Left };
                paragraphProps.Indentation = new Indentation { Left = ((int)leftTwips).ToString() };
                break;

            case HorzAlign.Center:
                paragraphProps.Justification = new Justification { Val = JustificationValues.Center };
                // Optional: add a small left indent if needed
                break;

            case HorzAlign.Right:
                paragraphProps.Justification = new Justification { Val = JustificationValues.Right };
                paragraphProps.Indentation = new Indentation { Right = ((int)leftTwips).ToString() };
                break;

            default:
                paragraphProps.Justification = new Justification { Val = JustificationValues.Left };
                paragraphProps.Indentation = new Indentation { Left = ((int)leftTwips).ToString() };
                break;
        }

        // Optional: shading/background
        if (_textObject.FillColor != System.Drawing.Color.Empty)
        {
            paragraphProps.Append(new Shading
            {
                Val = ShadingPatternValues.Clear,
                Color = "auto",
                Fill = (_textObject.FillColor.ToArgb() & 0xFFFFFF).ToString("X6")
            });
        }

        return paragraphProps;
    }


    private Run CreateRun()
    {
        var runProps = new RunProperties();

        // Font name
        runProps.Append(new RunFonts
        {
            Ascii = _textObject.Font.Name,
            HighAnsi = _textObject.Font.Name
        });

        // Font size (OpenXML uses half-points)
        runProps.Append(new FontSize { Val = (_textObject.Font.Size * 2).ToString() });

        // Bold, italic, underline
        if (_textObject.Font.Bold) runProps.Append(new Bold());
        if (_textObject.Font.Italic) runProps.Append(new Italic());
        if (_textObject.Font.Underline) runProps.Append(new Underline { Val = UnderlineValues.Single });

        //nmot sure where it stores the text color
        // // Font color
        if (_textObject.TextColor != System.Drawing.Color.Empty)
        {
            int rgb = _textObject.TextColor.ToArgb() & 0xFFFFFF;
            runProps.Append(new Color { Val = rgb.ToString("X6") });
        }

        var run = new Run();
        run.Append(runProps);
        run.Append(new Text(_textObject.Text) { Space = SpaceProcessingModeValues.Preserve });

        return run;
    }

    private JustificationValues MapAlignment(HorzAlign align) =>
        align switch
        {
            HorzAlign.Left => JustificationValues.Left,
            HorzAlign.Center => JustificationValues.Center,
            HorzAlign.Right => JustificationValues.Right,
            _ => JustificationValues.Left
        };

}