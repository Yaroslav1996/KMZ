using KMZ.Classes;
using KMZ.Controls;
using KMZ.Pages;
using Microsoft.Win32;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            SectionCollection = new List<Section>();
            LoadSettings();
        }

        public List<KmlFile> KMLCollection { get; }
        public List<Section> SectionCollection { get; }
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
                throw ex;
            }
        }

        public void AddToList(KmlFile file)
        {
            FileButton<KmlFile> newButt = new FileButton<KmlFile>(file)
            {
                Margin = new Thickness(10, 5, 10, 5),
                Content = ((Kml)file.Root).Feature.Name.ToString()
            };
            newButt.Click += OnFileButtonClick;
            Stack.Children.Add(newButt);
            KMLCollection.Add(file);
        }

        public void AddToList(Section file)
        {
            FileButton<Section> newButt = new FileButton<Section>(file)
            {
                Margin = new Thickness(10, 5, 5, 10),
                Content = file.Name
            };
            newButt.Click += OnSectionFileClick;
            SectionStack.Children.Add(newButt);
            SectionCollection.Add(file);
        }

        private void OnSectionFileClick(object sender, RoutedEventArgs e)
        {
            FileButton<Section> button = sender as FileButton<Section>;
            OpenSection(button.File);
        }

        //private Map CreateMap(KmlFile file)
        //{
        //    Map map = new Map(file);
        //    string path = @"pack://application:,,,/Pages";
        //    Uri urur = new Uri(path);
        //    //DirectoryInfo dir = new DirectoryInfo(urur.);
        //    MessageBox.Show(urur.Normalize().GetPath());
        //    //map.MapBrowser.Source = new Uri(urur.AbsolutePath);

        //    List<Placemark> lines = new List<Placemark>();
        //    ExtractPlacemarks(((Kml)file.Root).Feature, lines);

        //    return map;
        //}

        private void EditFile()
        {
            KmlFile outputFile;
            if (Setts.SpareCopy == Setting.Yes)
            {
                outputFile = KmlFile.Create(ChosenFile.Root.Clone(), true);
                ((Kml)outputFile.Root).Feature.Name = ((Kml)outputFile.Root).Feature.Name.Replace(".kml", "(Copy).kml");
                AddToList(outputFile);
                FileButton<KmlFile> butt = Stack.Children[Stack.Children.Count - 1] as FileButton<KmlFile>;
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
                    FileButton<KmlFile> butt = Stack.Children[Stack.Children.Count - 1] as FileButton<KmlFile>;
                    butt.IsEnabled = false;
                }
            }
            EditWindow editWindow = new EditWindow();
            Kml kml = ChosenFile.Root as Kml;
            List<Placemark> placemarks = new List<Placemark>();
            ExtractPlacemarks(kml.Feature, placemarks);
            foreach (var place in placemarks)
            {
                TextBlock placeBox = new TextBlock();
                placeBox.TextAlignment = TextAlignment.Center;
                placeBox.Text = place.Name;
                placeBox.Height = 50;
                placeBox.FontSize = 30;
                editWindow.Stack.Children.Add(placeBox);
                int iter = 0;
                foreach (var i in ((LineString)place.Geometry).Coordinates)
                {
                    editWindow.Stack.Children.Add(new EditPointControl(i, ++iter));
                }
            }

            editWindow.Show();
        }

        private void OpenSection(Section section)
        {
            SectionReadWindow sectionReadWindow = new SectionReadWindow(section, true);
            sectionReadWindow.ShowDialog();
        }

        //private void ShowFile(KmlFile inputFile)
        //{
        //    Map MapWindow = CreateMap(inputFile);

        //    MapWindow.Show();
        //}

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
                if (cont != null)
                {
                    foreach (var i in cont.Features)
                    {
                        ExtractPlacemarks(i, placemarks);
                    }
                }
            }
        }

        private void SaveKmls()
        {
            foreach (KmlFile i in KMLCollection)
            {
                try
                {
                    FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                    i.Save(str);
                    MessageBox.Show("File saved");
                }
                catch
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml");
                        FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                        i.Save(str);
                        MessageBox.Show("File saved");
                    }
                    catch
                    {
                        MessageBox.Show("Save failed");
                    }
                }
            }
        }

        private void SaveSections()
        {
            foreach (Section i in SectionCollection)
            {
                try
                {
                    if (i.Name.Contains(".jpg"))
                        i.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections\" + i.Name);
                    else
                        i.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections\" + i.Name + ".jpg");
                }
                catch
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections");
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        private void ChangeButtons(bool to)
        {
            //this.ShowButton.IsEnabled = to;
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
            catch
            {
                return;
            }
        }

        private void OnLoadSectionClick(object sender, RoutedEventArgs e)
        {
            Stream str = null;
            OpenFileDialog LoadWindow = new OpenFileDialog()
            {
                InitialDirectory = "c:\\Pulpit",
                Filter = "All files (*.*)|*.*|JPG (*.jpg)|*.jpg",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            try
            {
                LoadWindow.ShowDialog();
                str = LoadWindow.OpenFile();
                if (str != null)
                {
                    Bitmap img = new Bitmap(str);

                    StringPackage pac = new StringPackage("");
                    NewName newName = new NewName(pac);
                    newName.ShowDialog();
                    Section sec = new Section(pac.Content, img);
                    AddToList(sec);
                }
            }
            catch
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
                SectionStack.Children.Clear();
                SectionCollection.Clear();
            }
            ChangeButtons(false);
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FileButton<KmlFile> butt = sender as FileButton<KmlFile>;

            if (!butt.IsClicked)
            {
                butt.IsClicked = true;
                ChangeButtons(true);
                butt.Background = new SolidColorBrush(Colors.DarkGray);
                ChosenFile = butt.File;

                foreach (FileButton<KmlFile> i in Stack.Children)
                {
                    if (!i.IsClicked)
                        i.IsEnabled = false;
                }
                try
                {
                    ChosenFile.Save(new FileStream("Actual.kml", FileMode.Open));
                }
                catch (Exception ex)
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

                foreach (FileButton<KmlFile> i in Stack.Children)
                {
                    i.IsEnabled = true;
                }
            }
        }

        //private void OnShowButtonClick(object sender, RoutedEventArgs e)
        //{
        //    ShowFile(ChosenFile);
        //    //try
        //    //{
        //    //    ShowFile(ChosenFile);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //}
        //}

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            EditFile();
        }

        private void OnNewNameButtonClick(object sender, RoutedEventArgs e)
        {
            StringPackage pac = new StringPackage("");
            NewName newName = new NewName(pac);
            newName.ShowDialog();
            string na = pac.Content;

            if (!na.Contains(".kml"))
                na += ".kml";

            foreach (FileButton<KmlFile> i in Stack.Children)
            {
                if (i.IsClicked)
                {
                    i.Content = na;
                    break;
                }
            }

            ((Kml)ChosenFile.Root).Feature.Name = na;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveKmls();
            SaveSections();
        }

        private void OnLoadAllFilesClick(object sender, RoutedEventArgs e)
        {
            //loading kmls
            DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml");
            FileInfo[] files = di.GetFiles("*.kml");
            foreach (FileInfo file in files)
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

            //loading sections
            di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections");
            files = di.GetFiles("*.jpg");
            foreach (FileInfo file in files)
            {
                Section section = new Section();
                try
                {
                    section.Image = Bitmap.FromStream(file.Open(FileMode.Open)) as Bitmap;
                    section.Name = file.Name;
                    AddToList(section);
                }
                catch
                {
                }
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            KMLCollection.Remove(ChosenFile);
            foreach (FileButton<KmlFile> i in Stack.Children)
            {
                if (i.IsClicked)
                {
                    Stack.Children.Remove(i);
                    break;
                }
            }
            foreach (FileButton<KmlFile> i in Stack.Children)
            {
                i.IsEnabled = true;
            }
        }

        private void OnMakeProfileButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}