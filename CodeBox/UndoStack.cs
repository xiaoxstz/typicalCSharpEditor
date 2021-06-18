using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace CodeBox
{
    /// <summary>
    /// Extension for stack reversing.
    /// </summary>
    internal static class StackExtension
    {
        internal static void Reverse(this Stack<UndoOperation> stack)
        {
            List<UndoOperation> tempOps = stack.ToList();
            stack.Clear();
            foreach (UndoOperation op in tempOps)
                stack.Push(op);
        }
    }

    /// <summary>
    /// Implementaion of history stack for TextEditor.
    /// </summary>
    internal class UndoStack
    {

        #region Constants
        /// <summary>
        /// Max size of undo stack.
        /// </summary>
        private const int MAX_CAPACITY = 50000;
        #endregion

        #region Properties
        private Stack<UndoOperation> undoOperations { get; set; } = new Stack<UndoOperation>();
        private Stack<UndoOperation> redoOperations { get; set; } = new Stack<UndoOperation>();

        public int Count => undoOperations.Count;
        public int RedoCount => redoOperations.Count;
        #endregion

        #region Methods

        #region Private

        #region ContainingCheck

        /// <summary>
        /// Checks: is <paramref name="op"/> already exists in <see cref="undoOperations"/>
        /// </summary>
        private bool IsContainsUndo(UndoOperation op)
        {
            if (Count == 0 || op.Text != undoOperations.Peek().Text)
            {
                if (Count > 1 && op.Text == undoOperations.ToList()[1].Text)
                    return true;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks: is <paramref name="op"/> already exists in <see cref="redoOperations"/>
        /// </summary>
        private bool IsContainsRedo(UndoOperation op)
        {
            if (RedoCount == 0 || op.Text != undoOperations.Peek().Text)
            {
                if (RedoCount > 1 && op.Text == redoOperations.ToList()[1].Text)
                    return true;
                return false;
            }
            return true;
        }
        #endregion

        #region CheckStackCapacity

        private void CheckUndoStackCapacity()
        {
            if (undoOperations.Count > MAX_CAPACITY)
            {
                undoOperations.Reverse();
                while (undoOperations.Count > MAX_CAPACITY)
                    undoOperations.Pop();
                undoOperations.Reverse();
            }
        }

        private void CheckRedoStackCapacity()
        {
            if (redoOperations.Count > MAX_CAPACITY)
            {
                redoOperations.Reverse();
                while (redoOperations.Count > MAX_CAPACITY)
                    redoOperations.Pop();
                redoOperations.Reverse();
            }
        }

        #endregion

        /// <summary>
        /// Method for excluding situtaions when offset in undo is greater than max offset of the editor.
        /// </summary>
        private int GetCaretOffset(TextDocument doc, int caretOffset)
        {
            DocumentLine lastLine = doc.Lines.Last();
            DocumentLine firstLine = doc.Lines.First();
            if (lastLine.EndOffset < caretOffset || caretOffset < firstLine.Offset)
                return lastLine.EndOffset;
            else
                return caretOffset;
        }
        #endregion

        #region Public
        public void Push(UndoOperation op)
        {
            if (!IsContainsUndo(op))
                undoOperations.Push(op);
        }

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
                area.Caret.Offset = GetCaretOffset(doc, offset);
                undoOperations.Pop();
                CheckUndoStackCapacity();
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
                if (Count > 0)
                    undoOperations.Pop();
                undoOperations.Push(redoOperations.Pop());
                CheckRedoStackCapacity();
            }
        }
        #endregion

        #endregion
    }
}
