using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBox
{
    internal class UndoStack
    {
        private Stack<UndoOperation> operations { get; set; } = new Stack<UndoOperation>();

        public int Count => operations.Count;

        public UndoOperation Pop()
        {
            if(Count > 0)
               return operations.Pop();
            return null; 
        }

        public UndoOperation Peek()
        {
            if(Count > 0)
                return operations.Peek();
            return null;
        }


        public void Push(UndoOperation op)
        {
            if (Count == 0 || op.Text != Peek().Text)
            {
                operations.Push(op);
                if (Count > 2 && op.Text == operations.ToList()[2].Text)
                    Pop();
            }
        }

        private int IsContains(UndoOperation op)
        {
            List<UndoOperation> ops = operations.ToList();
            for(int i = 0; i < ops.Count; i++)
            {
                if (ops[i].Text == op.Text)
                    return i;
            }
            return -1;
        }
    }
}
