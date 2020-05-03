using System;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Indentation.CSharp;

namespace typicalIDE.CodeBox.Indents
{
    public class CSharpIndent : CSharpIndentationStrategy
    {
        public CSharpIndent() { }

        private Caret caret { get; set; }
        public CSharpIndent(Caret caret) //to set position after auto indent
        {
            this.caret = caret;
        }

        public const string INDENT_STRING = "   ";
        private const char OPEN_BRACE = '{';
        private const char CLOSE_BRACE = '}';
       
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
                SetCloseBraceIndent(line, document, indentation);
            }
        }

        private string GetSharpIndentation(string indentation, DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.Length);
            string noSpacesText = prevLineText.Replace(" ", "");
            if (noSpacesText.Length > 0)
            {
                char lastChar = noSpacesText.Last();
                if (lastChar == OPEN_BRACE)
                    indentation += INDENT_STRING;
                int index = indentation.IndexOf(INDENT_STRING);
                if (lastChar == CLOSE_BRACE && indentation.Length > 0 && index > -1)
                {
                    indentation = indentation.Remove(index, INDENT_STRING.Length);
                    SetLastLineIndent(prevLine, doc);
                }
            }
            return indentation;
        }

        private void SetCloseBraceIndent(DocumentLine line, TextDocument document, string indentation)
        {
            string currentLineText = document.GetText(line.Offset, line.Length);
            string noSpacesText = currentLineText.Replace(" ", "");
            int index = indentation.IndexOf(INDENT_STRING);
            if (noSpacesText.Length > 0 && noSpacesText.Last() == CLOSE_BRACE && index > -1)
            {
                string tempIndent = indentation.Remove(index, INDENT_STRING.Length);
                currentLineText = currentLineText.Replace(" ", "").Insert(0, $"\n{tempIndent}");
                document.Replace(line.Offset, line.Length, currentLineText, OffsetChangeMappingType.RemoveAndInsert);
                document.Replace(line.Offset, line.Length, indentation);
                caret.Line--;
                caret.Column = line.Length;
            }
        }

        private void SetLastLineIndent(DocumentLine prevLine, TextDocument doc)
        {
            string prevLineText = doc.GetText(prevLine.Offset, prevLine.Length);
            string indentedText = prevLineText.Remove(prevLineText.IndexOf(INDENT_STRING), INDENT_STRING.Length);
            doc.Replace(prevLine.Offset, prevLine.Length, indentedText);
        }
    }
}

