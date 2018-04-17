using Microsoft.Win32;
using SharpKml.Engine;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KMZ
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public KmzFile CurrentFile { get; set; }
        public KmlFile CurrentKML { get; set; }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            Stream str = null;
            OpenFileDialog LoadWindow = new OpenFileDialog()
            {
                InitialDirectory = "c:\\Pulpit",
                Filter = "KMZ (*.kmz)|*.kmz|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            try
            {
                LoadWindow.ShowDialog();
                str = LoadWindow.OpenFile();
                if (str != null)
                {
                    CurrentFile = KmzFile.Open(str);
                    str.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }

            foreach (var name in CurrentFile.Files)
            {
                Stack.Children.Add(new TextBlock()
                {
                    Text = name.ToString()
                });
            }

            CurrentKML = CurrentFile.GetDefaultKmlFile();
        }
    }
}