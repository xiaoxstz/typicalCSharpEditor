namespace CodeBox
{
    internal class UndoOperation
    {
        public string Text { get; set; }
        public int CaretOffset { get; set; }
        public UndoOperation(int caretOffset, string text)
        {
            Text = text;
            CaretOffset = caretOffset;
        }
    }
}
