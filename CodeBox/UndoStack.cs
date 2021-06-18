using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace CodeBox
{
    internal class UndoStack
    {
        private Stack<UndoOperation> undoOperations { get; set; } = new Stack<UndoOperation>();
        private Stack<UndoOperation> redoOperations { get; set; } = new Stack<UndoOperation>();

        public int Count => undoOperations.Count;
        public int RedoCount => redoOperations.Count;

        public UndoOperation Pop()
        {
            if (Count > 0)
            {
                UndoOperation op = undoOperations.Pop();
                if (RedoCount == 0 || op.Text != RedoPeek().Text)
                    redoOperations.Push(op);
                if (RedoCount > 2 && RedoPeek().Text == redoOperations.ToList()[2].Text)
                    redoOperations.Pop();
                return redoOperations.Peek();
            }
            return null; 
        }

        public UndoOperation Peek()
        {
            if(Count > 0)
                return undoOperations.Peek();
            return null;
        }


        public void Push(UndoOperation op)
        {
            if (!IsContainsUndo(op))
                undoOperations.Push(op);
        }

        public UndoOperation RedoPop()
        {
            if (redoOperations.Count > 0)
            {
                Push(redoOperations.Pop());
                return undoOperations.Peek();
            }
            return null;
        }

        public UndoOperation RedoPeek()
        {
            if (redoOperations.Count > 0)
                return redoOperations.Peek();
            return null;
        }

        private bool IsContainsUndo(UndoOperation op)
        {
            if (Count == 0 || op.Text != Peek().Text)
            {
                if (Count > 1 && op.Text == undoOperations.ToList()[1].Text)
                    return true;
                return false;
            }
            return true;
        }
        private bool IsContainsRedo(UndoOperation op)
        {
            if (RedoCount == 0 || op.Text != RedoPeek().Text)
            {
                if (RedoCount > 1 && op.Text == redoOperations.ToList()[1].Text)
                    return true;
                return false;
            }
            return true;
        }
        //int offset = Pop().CaretOffset;
        //    if (undoOperations.Count > 0)
        //    {
        //        UndoOperation op = undoOperations.Peek();
        //offset -= doc.TextLength - op.Text.Length;
        //        doc.Text = op.Text;
        //        DocumentLine lastLine = doc.Lines.Last();
        //        if (lastLine.EndOffset<offset || offset<lastLine.Offset)
        //            area.Caret.Offset = doc.Lines.Last().EndOffset;
        //        else
        //            area.Caret.Offset = offset;
        //       undoOperations.Pop();
        //    }
        public void Undo(TextDocument doc, TextArea area)
        {
            UndoOperation tempOp = undoOperations.Pop();
            redoOperations.Push(tempOp);
            int offset = tempOp.CaretOffset;
            if (Count > 0)
            {
                UndoOperation op = undoOperations.Peek();
                offset -= doc.TextLength - op.Text.Length;
                doc.Text = op.Text;
                area.Caret.Offset = GetCaretOffset(doc,offset);
                undoOperations.Pop();
            }
        }

        public void Redo(TextDocument doc, TextArea area)
        {
            if (RedoCount > 0)
            {
                UndoOperation op = redoOperations.Peek();
                int offset = op.CaretOffset;
                offset -= doc.TextLength - op.Text.Length;
                doc.Text = op.Text;
                area.Caret.Offset = GetCaretOffset(doc, op.CaretOffset);
                if(Count > 0)
                    undoOperations.Pop();
                undoOperations.Push(redoOperations.Pop());
            }
        }

        private int GetCaretOffset(TextDocument doc, int caretOffset)
        {
            DocumentLine lastLine = doc.Lines.Last();
            DocumentLine firstLine = doc.Lines.First();
            if (lastLine.EndOffset < caretOffset || caretOffset < firstLine.Offset)
                return lastLine.EndOffset;
            else
                return caretOffset;
        }

    }
}
