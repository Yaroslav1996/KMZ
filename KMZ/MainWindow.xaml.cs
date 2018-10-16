using KMZ.Classes;
using KMZ.Controls;
using KMZ.Pages;
using Microsoft.Win32;
using SharpKml.Dom;
using SharpKml.Engine;
using Coord = SharpKml.Base;
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
            ProfileCollection = new List<Profile>();
            LoadSettings();
        }

        public List<KmlFile> KMLCollection { get; }
        public List<Section> SectionCollection { get; }
        public List<Profile> ProfileCollection { get; }
        public KmlFile ChosenFile;
        public Settings Setts;

        public void SaveSettings()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            StreamWriter writer = new StreamWriter("setts.xml");
            xmlSerializer.Serialize(writer, Setts);
            writer.Close();
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
            catch
            {
                MessageBox.Show("Unable to create settings file");
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

        public void AddToList(Profile file)
        {
            FileButton<Profile> fileButton = new FileButton<Profile>(file)
            {
                Margin = new Thickness(10, 5, 5, 10),
                Content = file.Name,
            };
            fileButton.Click += OnProfileButtonClick;
            ProfilesStack.Children.Add(fileButton);
            ProfileCollection.Add(file);
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
        
        private void SaveKmls()
        {
            foreach (KmlFile i in KMLCollection)
            {
                try
                {
                    FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                    i.Save(str);
                }
                catch
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml");
                        FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                        i.Save(str);
                    }
                    catch
                    {
                        MessageBox.Show("Kml save failed");
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
                        MessageBox.Show("Sections save failed");
                    }
                }
            }
        }
        
        private void ChangeButtons(bool to)
        {
            //this.ShowButton.IsEnabled = to;
            this.MakeProfileButton.IsEnabled = to;
            this.EditButton.IsEnabled = to;
            this.ChangeName.IsEnabled = to;
            this.DeleteButton.IsEnabled = to;
        }

        private void OnManualClick(object sender, RoutedEventArgs e)
        {
            List<Coord.Vector> coorList = new List<Coord.Vector>();
            NewFileWindow newFileWindow = new NewFileWindow(this);
            newFileWindow.ShowDialog();
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

        private void OnProfileButtonClick(object sender, RoutedEventArgs e)
        {
            FileButton<Profile> fileButton = sender as FileButton<Profile>;
            SectionReadWindow sectionReadWindow = new SectionReadWindow(fileButton.File.ProfSection, false);
            sectionReadWindow.ShowDialog();

            Kml file = (Kml)fileButton.File.ProfKmlFile.Root;
            List<Placemark> lineList = new List<Placemark>();
            ExtractPlacemarks(file.Feature, lineList);
            LineString line = new LineString();
            

            try
            {
                line = lineList[0].Geometry as LineString;
            }
            catch
            {
                MessageBox.Show("Kml doesn't include a linestring");
            }

            List<Coord.Vector> points = new List<Coord.Vector>();

            foreach (Coord.Vector i in line.Coordinates)
            {
                points.Add(i);
            }

            Coord.Vector startCoord = points[0];
            Coord.Vector endCoord = points[points.Count - 1];

            double profLength = CoordinatesCalculator.CalculateDistance(endCoord.Latitude, endCoord.Longitude, startCoord.Latitude, startCoord.Longitude);
            double sectionLength = sectionReadWindow.LastPoint.X - sectionReadWindow.ZeroPoint.X;
            double scale = profLength / sectionLength;
            double maxDepth = sectionReadWindow.LastPoint.Y - sectionReadWindow.ZeroPoint.Y;
            int pointsAmount = sectionReadWindow.Points.Count;

            List<System.Windows.Point> secPoints = sectionReadWindow.Points;

            Document document = new Document();

            for (int i = 0; i < pointsAmount; i++)
            {
                int depth;
                if(i == 0 || i == pointsAmount - 1)
                {
                    depth = (int)(secPoints[i].Y / maxDepth * 255);
                }
                else
                {
                    depth = (int)((secPoints[i].Y + secPoints[i - 1].Y) / 2 / maxDepth * 255);
                }

                double dist = (secPoints[i].X - secPoints[i - 1].X) * scale;
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
            #region Loading KML files

            try
            {
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
            }
            catch
            {
            }

            #endregion Loading KML files

            #region Loading sections

            try
            {
                DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections");
                FileInfo[] files = di.GetFiles("*.jpg");
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
            catch
            { }
            #endregion Loading sections
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
            Profile profile = new Profile();
            profile.ProfKmlFile = ChosenFile;
            ChooseSectionWindow chooseSectionWindow = new ChooseSectionWindow(profile);

            foreach (Section i in SectionCollection)
            {
                FileButton<Section> button = new FileButton<Section>(i);
                button.Margin = new Thickness(10, 5, 5, 10);
                button.Content = i.Name;
                button.Click += chooseSectionWindow.OnButtClick;
                chooseSectionWindow.Stack.Children.Add(button);
            }

            chooseSectionWindow.ShowDialog();
            StringPackage pac = new StringPackage("");
            NewName newName = new NewName(pac);
            newName.ShowDialog();
            profile.Name = pac.Content;
            AddToList(profile);
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
                if (cont != null)
                {
                    foreach (var i in cont.Features)
                    {
                        ExtractPlacemarks(i, placemarks);
                    }
                }
            }
        }

        

        public void AddSingleLineToContainer(Coord.Vector start, Coord.Vector end, int color, Container container)
        {
            SharpKml.Dom.Style style = new SharpKml.Dom.Style();

            style.Id = "LineStyle";
            style.Line = new LineStyle();
            style.Line.Color = new Coord.Color32(color);

            Placemark placemark = new Placemark();
            placemark.StyleUrl = new Uri("#LineStyle", UriKind.Relative);

            LineString lineString = new LineString();
            lineString.Coordinates.Add(start);
            lineString.Coordinates.Add(end);

            placemark.Geometry = lineString;
            container.AddFeature(placemark);
            container.AddStyle(style);
        }
        
    }
}