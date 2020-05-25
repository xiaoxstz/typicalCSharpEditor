
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Snippets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Completions.CSharpCompletion
{
    public class CSharpCompletion: INotifyPropertyChanged, ICompletionData
    {
        #region Constructors
        public CSharpCompletion(string text)
        {
            this.Text = text;
        }

        public CSharpCompletion(string text, CompletionTypes type)
        {
            Image = CompletionImage.GetImageSource(type);
            CompletionType = type;
            this.Text = text;
        }
        #endregion

        #region Completion properties
        public CompletionTypes CompletionType { get; }

        public ImageSource Image { get; }

        public string Text { get; private set; }


        public object Content => this.Text;

        #region Description
        private object description;
        public object Description
        {
            get
            {
                if (description == null)
                    return $"Keyword: {Text}";
                return $"{description}{Text}";
            }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        #endregion

        public double Priority { get; }

        #region SelectionColor

        private Brush selectionColor;
        public Brush SelectionColor
        {
            get { return selectionColor; }
            set
            {
                selectionColor = value;
                OnPropertyChanged("SelectionColor");
            }
        }

        #endregion

        #endregion

        #region Completion methods
        public void Complete(TextArea textArea, ISegment completionSegment,
    EventArgs insertionRequestEventArgs)
        {

            textArea.Document.Replace(completionSegment.Offset, completionSegment.Length,
                                      Text, OffsetChangeMappingType.CharacterReplace);
        }

        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}