using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBox.Properties;

namespace Completions.CSCompletion
{
    internal class CompletionImage
    {
        internal static ImageSource GetImageSource(CompletionTypes type)
        {
            switch(type)
            {
                case CompletionTypes.Class:
                    return ImageSourceFromBitmap(Resources.Class);
                case CompletionTypes.Delegate:
                    return ImageSourceFromBitmap(Resources.Delegate);
                case CompletionTypes.Enum:
                    return ImageSourceFromBitmap(Resources.Enum);
                case CompletionTypes.Field:
                    return ImageSourceFromBitmap(Resources.Field);
                case CompletionTypes.Interface:
                    return ImageSourceFromBitmap(Resources.Interface);
                case CompletionTypes.Keyword:
                    return ImageSourceFromBitmap(Resources.Keyword);
                case CompletionTypes.Namespace:
                    return ImageSourceFromBitmap(Resources.Namespace);
                case CompletionTypes.Property:
                    return ImageSourceFromBitmap(Resources.Property);
                case CompletionTypes.Snippet:
                    return ImageSourceFromBitmap(Resources.Snippet);
                case CompletionTypes.Structure:
                    return ImageSourceFromBitmap(Resources.Struct);
                default:
                    throw new Exception("Incorrect type");
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        private static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
