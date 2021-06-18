namespace CodeBox
{
    /// <summary>
    /// Class with information about changes in text editor.
    /// </summary>
    internal class UndoOperation
    {
        /// <summary>
        /// Text change.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Caret position change.
        /// </summary>
        public int CaretOffset { get; set; }
        public UndoOperation(int caretOffset, string text)
        {
            Text = text;
            CaretOffset = caretOffset;
        }
    }
}
