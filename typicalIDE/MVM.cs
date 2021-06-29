using CodeBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace typicalIDE
{
    public class MVM: INotifyPropertyChanged
    {
        public MVM()
        {
            UndoStack.Push(new UndoOperation(0, "first op"));
            UndoStack.Push(new UndoOperation(0, "second op"));
            UndoStack.Push(new UndoOperation(0, "third op"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #region Txt

        private string txt = "";
        public string Txt
        {
            get => txt;
            set
            {
                txt = value;
                OnPropertyChanged("Txt");
            }
        }

        #endregion

        #region UndoStack

        private UndoStack undoStack = new UndoStack();
        public UndoStack UndoStack
        {
            get => undoStack;
            set
            {
                undoStack = value;
                OnPropertyChanged("UndoStack");
            }
        }

        #endregion
    }
}
