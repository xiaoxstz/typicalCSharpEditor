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
    public class CSharpIndent : CSharpIndentationStrategy
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
                SetCloseBracketIndent(line, document, indentation);
            }
        }

        private string GetSharpIndentation(string indentation, DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.EndOffset - prevLine.Offset);
            string noSpacesText = prevLineText.Replace(" ", "");
            if (noSpacesText.Length > 0)
            {
                char lastChar = noSpacesText.Last();
                if (lastChar == OPEN_BRACKET)
                    indentation += INDENT_STRING;
                if (lastChar == CLOSE_BRACKET && indentation.Length > 0)
                {
                    indentation = indentation.Remove(indentation.IndexOf(INDENT_STRING), INDENT_STRING.Length);
                    SetLastLineIndent(prevLine, doc);
                }
            }
            return indentation;
        }

        private void SetCloseBracketIndent(DocumentLine line, TextDocument document, string indentation)
        {
            string currentLineText = document.GetText(line.Offset, line.Length);
            string noSpacesText = currentLineText.Replace(" ", "");
            if (noSpacesText.Length > 0 && noSpacesText.Last() == CLOSE_BRACKET)
            {
                indentation = indentation.Remove(indentation.IndexOf(INDENT_STRING), INDENT_STRING.Length);
                currentLineText = currentLineText.Replace(" ", "").Insert(0, indentation);
                document.Replace(line.Offset, line.Length, currentLineText, OffsetChangeMappingType.RemoveAndInsert);
            }
        }

        private void SetLastLineIndent(DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.EndOffset - prevLine.Offset);
            string indentedText = prevLineText.Remove(prevLineText.IndexOf(INDENT_STRING), INDENT_STRING.Length);
            doc.Replace(prevLine.Offset, prevLine.Length, indentedText);
        }
    }
}

