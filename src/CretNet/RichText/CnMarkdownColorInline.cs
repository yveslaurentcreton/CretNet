using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace CretNet.RichText;

/// <summary>
/// Palette-coloured text, written <c>[text]{.accent}</c>. Its children are the
/// ordinary inlines of the text (emphasis, links, …); renderers apply
/// <see cref="Color"/> around them.
/// </summary>
public sealed class CnMarkdownColorInline : ContainerInline
{
    public required CnPaletteColor Color { get; init; }
}

/// <summary>
/// The palette attribute that follows a bracketed text: <c>{.accent}</c> in
/// <c>[text]{.accent}</c>. It lives only between parsing and
/// <see cref="CnMarkdown"/>'s normalisation, which turns it into a
/// <see cref="CnMarkdownColorInline"/> or back into plain text.
/// </summary>
internal sealed class CnPaletteAttributeInline : LeafInline
{
    public required string ClassName { get; init; }

    public required string Source { get; init; }
}

/// <summary>
/// Generic attributes limited to the palette: only <c>{.name}</c> directly
/// after a closing bracket is recognised. Markdig's full generic-attributes
/// extension is deliberately not used: it passes any attribute (including
/// <c>onclick</c>) to the HTML and silently swallows ordinary text in braces.
/// </summary>
internal sealed class CnPaletteAttributeParser : InlineParser
{
    public CnPaletteAttributeParser() => OpeningCharacters = ['{'];

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var text = slice.Text;
        var start = slice.Start;
        if (start < 1 || text[start - 1] != ']' || (start >= 2 && text[start - 2] == '\\'))
            return false;

        if (processor.Inline is not LiteralInline previous || !previous.Content.ToString().EndsWith(']'))
            return false;

        var position = start + 1;
        if (position >= slice.End + 1 || text[position] != '.')
            return false;

        position++;
        var nameStart = position;
        while (position <= slice.End && (char.IsAsciiLetterOrDigit(text[position]) || text[position] is '-' or '_'))
            position++;

        if (position == nameStart || !char.IsAsciiLetter(text[nameStart]) || position > slice.End || text[position] != '}')
            return false;

        var source = text.Substring(start, position - start + 1);
        processor.Inline = new CnPaletteAttributeInline
        {
            ClassName = text[nameStart..position],
            Source = source,
            Span = new SourceSpan(processor.GetSourcePosition(start, out var line, out var column), processor.GetSourcePosition(position)),
            Line = line,
            Column = column,
        };
        slice.Start = position + 1;
        return true;
    }
}

internal sealed class CnMarkdownColorInlineRenderer : HtmlObjectRenderer<CnMarkdownColorInline>
{
    protected override void Write(HtmlRenderer renderer, CnMarkdownColorInline inline)
    {
        if (renderer.EnableHtmlForInline)
            renderer.Write("<span class=\"").Write(inline.Color.CssClass).Write("\">");

        renderer.WriteChildren(inline);

        if (renderer.EnableHtmlForInline)
            renderer.Write("</span>");
    }
}

internal sealed class CnPaletteExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<CnPaletteAttributeParser>())
            pipeline.InlineParsers.Insert(0, new CnPaletteAttributeParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer html && !html.ObjectRenderers.Contains<CnMarkdownColorInlineRenderer>())
            html.ObjectRenderers.Insert(0, new CnMarkdownColorInlineRenderer());
    }
}
