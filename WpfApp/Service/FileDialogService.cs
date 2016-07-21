using Microsoft.Win32;

namespace WpfApp.Service
{
    internal class FileDialogService : IFileDialogService
    {
        public bool IsDialog => true;
        public string Filter
        {
            get { return FileDialog.Filter; }
            set { FileDialog.Filter = value; }
        }
        protected readonly FileDialog FileDialog;

        public string FileName
        {
            get { return FileDialog.FileName; }
            set { FileDialog.FileName = value; }
        }

        public FileDialogService(FileDialog fileDialog)
        {
            FileDialog = fileDialog;
        }

        public bool? Show()
        {
            return FileDialog.ShowDialog();
        }
    }

    public static class FileDialogServices
    {
        public static IFileDialogService ImageLoader { get; } = new FileDialogService(new OpenFileDialog
        {
            Filter = "�����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;|" +
                     "��� �����������|*.jpg;*.jpeg;*.png;*.gif;*.ico;*.bmp;*.tif;*.tiff;*.wmphoto;|" +
                     "��� �����|*.*"
        });
    }
}