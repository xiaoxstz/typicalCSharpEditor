using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace typicalIDE.CodeBox.Indents
{
    public class CSharpIndent : DefaultIndentationStrategy
    {
        public const string INDENT_STRING = "   ";
        private const char OPEN_BRACKET = '{';
        private const char CLOSE_BRACKET = '}';
       
        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (line == null)
                throw new ArgumentNullException("line");
            DocumentLine previousLine = line.PreviousLine;
            if (previousLine != null)
            {
                ISegment indentationSegment = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
                string indentation = document.GetText(indentationSegment);
                indentation = GetSharpIndentation(indentation, previousLine, document);
                indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
                document.Replace(indentationSegment.Offset, indentationSegment.Length, indentation,
                                 OffsetChangeMappingType.RemoveAndInsert);
            }
        }

        private string GetSharpIndentation(string indentation, DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.EndOffset - prevLine.Offset);
            char lastChar = prevLineText.Replace(" ", "").Last();
            if (lastChar == OPEN_BRACKET)
                indentation += INDENT_STRING;
            if (lastChar == CLOSE_BRACKET && indentation.Length > 0)
            {
                indentation = indentation.Remove(indentation.IndexOf(INDENT_STRING), INDENT_STRING.Length);
                SetLastLineIndent(prevLine, doc);
            }
            return indentation;
        }

        private void SetCloseBracketIndent()
        {

        }

        private void SetLastLineIndent(DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.EndOffset - prevLine.Offset);
            string indentedText = prevLineText.Remove(prevLineText.IndexOf(INDENT_STRING), INDENT_STRING.Length);
            doc.Replace(prevLine.Offset, prevLine.Length, indentedText);
        }
    }
}

