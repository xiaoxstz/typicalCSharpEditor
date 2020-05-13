using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Completions.CSharpCompletion
{
    internal class CompletionImage
    {
        private const string IMAGES_PATH = @"\Completions\CSharpCompletion\Images\";
        private const string PNG_EXTENSION = ".png";

        internal static ImageSource GetImageSource(CompletionTypes type)
        {
            string completionImagePath = $"{IMAGES_PATH}{type.ToString()}{PNG_EXTENSION}";
            Uri uri = new Uri(completionImagePath, UriKind.Relative);
            return new BitmapImage(uri);
        }
    }
}
