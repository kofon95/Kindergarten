using Microsoft.Win32;

namespace WpfApp.View.DialogService
{
    public static class FileDialogs
    {
        private const string ImageFilter =
                "�����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;|" +
                "��� �����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;*.bmp;*.tif;*.tiff;*.wmphoto;|" +
                "��� �����|*.*";

        public static OpenFileDialog LoadOneImage { get; } = new OpenFileDialog {Filter = ImageFilter};
        public static OpenFileDialog LoadManyImages { get; } = new OpenFileDialog {Filter = ImageFilter, Multiselect = true};
    }
}