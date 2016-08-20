using Microsoft.Win32;

namespace WpfApp.View.DialogService
{
    // ReSharper disable once InconsistentNaming
    public static class IODialog
    {
        private const string ImageFilter =
                "�����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;|" +
                "��� �����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;*.bmp;*.tif;*.tiff;*.wmphoto;|" +
                "��� �����|*.*";

        public static OpenFileDialog LoadOneImage { get; } = new OpenFileDialog {Filter = ImageFilter};


        public static string InputDialog(string text, string title, string message = "", string extraInfo = null)
        {
            var dialog = new InputTextDialog
            {
                Title = title,
                TextField = {Text = text},
                MessageField = {Text = message},
                ExtraInfoField = {Text = extraInfo},
            };
            var showDialog = dialog.ShowDialog();
            return showDialog == true ? dialog.MessageField.Text : null;
        }
    }
}