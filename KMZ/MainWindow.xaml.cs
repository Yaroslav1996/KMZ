using KMZ.Classes;
using KMZ.Pages;
using KMZ.Controls;
using Microsoft.Win32;
using SharpKml.Dom;
using SharpKml.Engine;
using SharpKml.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml.Serialization;

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
            KMLCollection = new List<KmlFile>();
            LoadSettings();
        }

        public List<KmlFile> KMLCollection { get; set; }
        public KmlFile ChosenFile;
        public Settings Setts;

        public void SaveSettings()
        {
            XmlSerializer writter = new XmlSerializer(typeof(Settings));
            StreamWriter newFile = new StreamWriter("setts.xml");
            writter.Serialize(newFile, Setts);
            newFile.Close();
        }

        public void SaveSettings(Settings file)
        {
            XmlSerializer writter = new XmlSerializer(typeof(Settings));
            StreamWriter newFile = new StreamWriter("setts.xml");
            writter.Serialize(newFile, file);
            newFile.Close();
        }

        public void LoadSettings()
        {
            try
            {
                FileStream file = new FileStream("setts.xml", FileMode.OpenOrCreate);
                XmlSerializer reader = new XmlSerializer(typeof(Settings));
                Setts = reader.Deserialize(file) as Settings;
            }
            catch (Exception ex)
            {
                if (ex.GetType().IsEquivalentTo(typeof(FileNotFoundException)))
                {
                    SaveSettings(new Settings());
                }
            }
        }

        public void AddToList(KmlFile file)
        {
            KMLButton newButt = new KMLButton(file)
            {
                Margin = new Thickness(10, 5, 10, 5),
                Content = ((Kml)file.Root).Feature.Name.ToString()
            };
            newButt.Click += OnFileButtonClick;
            Stack.Children.Add(newButt);
            KMLCollection.Add(file);
        }

        private Map CreateMap(KmlFile file)
        {
            Map map = new Map(file);
            map.MapBrowser.Source = new Uri(@"C:\Users\Yaroslav\Documents\Visual Studio 2017\Projects\KMZ\KMZ\Pages\MapPage.html");

            List<Placemark> lines = new List<Placemark>();
            ExtractPlacemarks(((Kml)file.Root).Feature, lines);

            return map;
        }

        private void EditFile()
        {
            KmlFile outputFile;
            if (Setts.SpareCopy == Setting.Yes)
            {
                outputFile = KmlFile.Create(ChosenFile.Root.Clone(), true);
                ((Kml)outputFile.Root).Feature.Name = ((Kml)outputFile.Root).Feature.Name.Replace(".kml", "(Copy).kml");
                AddToList(outputFile);
                KMLButton butt = Stack.Children[Stack.Children.Count - 1] as KMLButton;
                butt.IsEnabled = false;
            }
            else if (Setts.SpareCopy == Setting.No)
            {
                
            }
            else
            {
                if (MessageBox.Show("Czy chcesz stworzyć kopię zapasową?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    outputFile = KmlFile.Create(ChosenFile.Root.Clone(), true);
                    ((Kml)outputFile.Root).Feature.Name = ((Kml)outputFile.Root).Feature.Name.Replace(".kml", "(Copy).kml");
                    AddToList(outputFile);
                    KMLButton butt = Stack.Children[Stack.Children.Count - 1] as KMLButton;
                    butt.IsEnabled = false;
                }
            }
            EditWindow editWindow = new EditWindow();
            Kml kml = ChosenFile.Root as Kml;
            List<Placemark> placemarks = new List<Placemark>();
            ExtractPlacemarks(kml.Feature, placemarks);
            foreach(var place in placemarks)
            {
                TextBox placeBox = new TextBox();
                placeBox.TextAlignment = TextAlignment.Center;
                placeBox.Text = place.Name;
                placeBox.Height = 50;
                placeBox.FontSize = 30;
                placeBox.BorderThickness = new Thickness(5);
                editWindow.Stack.Children.Add(placeBox);
                int iter = 0;
                foreach (var i in ((LineString)place.Geometry).Coordinates)
                {
                    editWindow.Stack.Children.Add(new EditPointControl(i, ++iter));
                }
            }

            editWindow.Show();
        }

        private void ShowFile(KmlFile inputFile)
        {
            Map MapWindow = CreateMap(inputFile);

            MapWindow.Show();
        }

        public static void ExtractPlacemarks(Feature feat, List<Placemark> placemarks)
        {
            Placemark placemark = feat as Placemark;
            if (placemark != null)
            {
                placemarks.Add(placemark);
            }
            else
            {
                Container cont = feat as Container;
                if(cont != null)
                {
                    foreach(var i in cont.Features)
                    {
                        ExtractPlacemarks(i, placemarks);
                    }
                }
            }
        }

        private void ChangeButtons(bool to)
        {
            this.ShowButton.IsEnabled = to;
            this.EditButton.IsEnabled = to;
            this.ChangeName.IsEnabled = to;
            this.DeleteButton.IsEnabled = to;
        }

        private void OnManualClick(object sender, RoutedEventArgs e)
        {
            List<SharpKml.Base.Vector> coorList = new List<SharpKml.Base.Vector>();
            NewFileWindow newFileWindow = new NewFileWindow(this);
            newFileWindow.Show();
            this.IsEnabled = false;
        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            Stream str = null;
            OpenFileDialog LoadWindow = new OpenFileDialog()
            {
                InitialDirectory = "c:\\Pulpit",
                Filter = "All files (*.*)|*.*|KML (*.kml)|*.kml",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            try
            {
                LoadWindow.ShowDialog();
                str = LoadWindow.OpenFile();
                if (str != null)
                {
                    KmlFile file = KmlFile.Load(str);
                    AddToList(file);
                    str.Dispose();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWind = new SettingsWindow(this);
            setWind.Show();
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Na pewno wyczyścić listę plików?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Stack.Children.Clear();
                KMLCollection.Clear();
            }
            ChangeButtons(false);
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            KMLButton butt = sender as KMLButton;

            if (!butt.IsClicked)
            {
                butt.IsClicked = true;
                ChangeButtons(true);
                butt.Background = new SolidColorBrush(Colors.DarkGray);
                ChosenFile = butt.File;

                foreach (KMLButton i in Stack.Children)
                {
                    if (!i.IsClicked)
                        i.IsEnabled = false;
                }
                try
                {
                    ChosenFile.Save(new FileStream("Actual.kml", FileMode.Open));
                }
                catch(Exception ex)
                {
                    if (ex.GetType().IsEquivalentTo(typeof(FileNotFoundException)))
                    {
                        MessageBox.Show("Brak pliku Actual.kml");
                    }
                }
            }
            else if (butt.IsClicked)
            {
                butt.IsClicked = false;
                ChangeButtons(false);
                butt.Background = new SolidColorBrush(Colors.LightGray);
                ChosenFile = null;

                foreach (KMLButton i in Stack.Children)
                {
                    i.IsEnabled = true;
                }
            }
        }

        private void OnShowButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowFile(ChosenFile);
            }
            catch(Exception ex)
            {
                
            }
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            EditFile();
        }

        private void OnNewNameButtonClick(object sender, RoutedEventArgs e)
        {
            NewName newName = new NewName(this);
            newName.Show();
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            foreach(KmlFile i in KMLCollection)
            {
                try
                {
                    FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                    i.Save(str);
                    MessageBox.Show("File saved");
                }
                catch(Exception ex)
                {
                    if (ex.GetType().IsEquivalentTo(typeof(FileNotFoundException)))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\");
                        FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                        i.Save(str);
                        MessageBox.Show("File saved");
                    }
                    else
                        MessageBox.Show("Save failed");
                }
            }
        }

        private void OnLoadAllFilesClick(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\");
            FileInfo[] files = di.GetFiles("*.kml");
            foreach(FileInfo file in files)
            {
                KmlFile kmlFile;
                try
                {
                    kmlFile = KmlFile.Load(file.Open(FileMode.Open));
                    AddToList(kmlFile);
                }
                catch
                {

                }
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            KMLCollection.Remove(ChosenFile);
            foreach(KMLButton i in Stack.Children)
            {
                if (i.IsClicked)
                {
                    Stack.Children.Remove(i);
                    break;
                }
            }
            foreach(KMLButton i in Stack.Children)
            {
                i.IsEnabled = true;
            }
        }
    }
}