﻿using System;
using System.Configuration;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NLog;
using WpfApp.Framework;
using WpfApp.Settings;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Path to Camera.exe</summary>
        public static readonly string WebcamPath = ConfigurationManager.AppSettings["WebcamPath"];
        public const string WebcamArguments = "photo 1";
        public static readonly Logger Logger = LogManager.GetLogger(string.Empty);

        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Logger.Trace("On Startup");
            WindowStateSaver.InitializeSettings();
            var res = new ResourceDictionary {Source = GetFontsXamlUri()};
            Resources.MergedDictionaries.Add(res);
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            WindowStateSaver.SaveAllSettings();
            Logger.Trace("On Exit");
        }

        private static Uri GetFontsXamlUri()
        {
            var width = SystemParameters.PrimaryScreenWidth;
            var height = SystemParameters.PrimaryScreenHeight;
            string xaml;
            if (width <= 1366 || height <= 768)
            {
                xaml = "/Dictionaries/Fonts_HD.xaml";
            }
            else if (width <= 1680 || height <= 1050)
            {
                xaml = "/Dictionaries/Fonts_HD2.xaml";
            }
            else
            {
                xaml = "/Dictionaries/Fonts_FullHD.xaml";
            }
            Console.WriteLine(xaml);
            return new Uri(xaml, UriKind.Relative);
        }
        public static void EnsureAllDirectories()
        {
            if (Directory.Exists(AppFilePaths.PersonImages)) return;
            Directory.CreateDirectory(AppFilePaths.PersonImages);
            Logger.Trace("Directories was created");
        }


        public static SaveFileDialog GetDocumentSaveFileDialog(string fileName)
        {
            if (_documentSaveFileDialog == null)
                _documentSaveFileDialog = new SaveFileDialog { Filter = "Word|*.docx" };
            _documentSaveFileDialog.FileName = fileName;
            return _documentSaveFileDialog;
        }
        private static SaveFileDialog _documentSaveFileDialog;
    }
}
