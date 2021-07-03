using CodeBox;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
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

        private CodeBox.UndoStack undoStack = new CodeBox.UndoStack();
        public CodeBox.UndoStack UndoStack
        {
            get => undoStack;
            set
            {
                undoStack = value;
                OnPropertyChanged("UndoStack");
            }
        }

        #endregion

        #region IStrategy

        private IIndentationStrategy iStrategy;
        public IIndentationStrategy IStrategy
        {
            get => iStrategy;
            set
            {
                iStrategy = value;
                OnPropertyChanged("IStrategy");
            }
        }

        #endregion

 

        #region ButtonClickCommand
        private RelayCommand buttonClickCommand;
        public RelayCommand ButtonClickCommand
        {
            get => buttonClickCommand ?? (buttonClickCommand = new RelayCommand(obj =>
            {
                TextDocument doc = obj as TextDocument;
                IStrategy.IndentLines(doc, 1, doc.LineCount);
            }));
        }
        #endregion
    }
}
