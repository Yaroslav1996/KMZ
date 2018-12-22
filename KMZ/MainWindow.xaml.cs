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
using Coord = SharpKml.Base;
using System.Linq;
using System.Diagnostics;

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
        private FileButton<Profile> current;
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
            KMLCollection.Add(file);
        }

        public void AddToList(Section file)
        {
            SectionCollection.Add(file);
        }

        public void AddToList(Profile file)
        {
            FileButton<Profile> fileButton = new FileButton<Profile>(file)
            {
                Margin = new Thickness(10, 5, 5, 10),
                Height = 100,
                Width = 150,
                Content = file.Name
            };
            fileButton.Click += OnProfileButtonClick;
            ProfilesStack.Children.Add(fileButton);
            ProfileCollection.Add(file);
        }
        
        private void EditFile()
        {
            EditWindow editWindow = new EditWindow();
            Kml kml = current.File.ProfKmlFile.Root as Kml;
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

            editWindow.ShowDialog();
        }

        private void OpenSection(Section section)
        {
            SectionReadWindow sectionReadWindow = new SectionReadWindow(section, true);
            sectionReadWindow.ShowDialog();
        }
        
        private void SaveKmls(params KmlFile[] kmls)
        {
            foreach (KmlFile i in kmls)
            {
                try
                {
                    FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                    i.Save(str);
                    //MessageBox.Show("Pliki KML zapisane");
                }
                catch
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml");
                        FileStream str = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml\" + ((Kml)i.Root).Feature.Name, FileMode.OpenOrCreate);
                        i.Save(str);
                        //MessageBox.Show("Pliki KML zapisane");
                    }
                    catch
                    {
                        //MessageBox.Show("Kml save failed");
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
                    MessageBox.Show("Przekroje zapisane");
                }
                catch
                {
                    try
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections");
                        if (i.Name.Contains(".jpg"))
                            i.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections\" + i.Name);
                        else
                            i.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\sections\" + i.Name + ".jpg");
                        MessageBox.Show("Przekroje zapisane");
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
            EditButt.IsEnabled = to;
            Analize.IsEnabled = to;
        }

        private KmlFile LoadKml()
        {
            try
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
                        return file;
                    }
                }
                catch
                {
                    return null;
                }
                return null;
            }
            catch
            {
                MessageBox.Show("Kml not loaded");
                return null;
            }
        }

        private Section LoadSection()
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
                    return sec;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        private KmlFile CreateKml()
        {
            NewFileWindow newFileWindow = new NewFileWindow();
            newFileWindow.ShowDialog();
            return newFileWindow.OutFile;
        }

        private void OnManualClick(object sender, RoutedEventArgs e)
        {
            try
            {
                KmlFile kml = CreateKml();
                Section sec = LoadSection();
                Profile profile = new Profile(sec, kml);
                AddToList(profile);
            }
            catch
            {
                MessageBox.Show("Błąd wczytywania plików");
            }


        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                KmlFile kml = LoadKml();
                Section sec = LoadSection();
                Profile profile = new Profile(sec, kml);
                AddToList(profile);
            }
            catch
            {
                MessageBox.Show("Unable to create profile");
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
                ProfilesStack.Children.Clear();
                ProfileCollection.Clear();
                KMLCollection.Clear();
                SectionCollection.Clear();
            }
            ChangeButtons(false);
        }

        private void DoAnalize(FileButton<Profile> butt)
        {
            SectionReadWindow sectionReadWindow = new SectionReadWindow(butt.File.ProfSection, false);
            sectionReadWindow.ShowDialog();

            Kml file = (Kml)butt.File.ProfKmlFile.Root;
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

            foreach (Coord.Vector j in line.Coordinates)
            {
                points.Add(j);
            }

            Coord.Vector startCoord = points[0];
            Coord.Vector endCoord = points[points.Count - 1];
            List<System.Windows.Point> secPoints = sectionReadWindow.Points;
            List<Coord.Vector> vectors = new List<Coord.Vector>();

            int pointsAmount = sectionReadWindow.Points.Count;
            double profLength = CoordinatesCalculator.CalculateDistance(startCoord.Latitude, startCoord.Longitude, endCoord.Latitude, endCoord.Longitude);
            double sectionLength = sectionReadWindow.LastPoint.X - sectionReadWindow.ZeroPoint.X;
            double scale = profLength / sectionLength;
            double maxDepth = sectionReadWindow.LastPoint.Y - sectionReadWindow.ZeroPoint.Y;
            double azimuth = CoordinatesCalculator.CalculateBearing(startCoord, endCoord);
            var pp = from t in secPoints
                     orderby t.X ascending
                     select t;

            secPoints = pp.ToList<System.Windows.Point>();

            List<byte> depths = new List<byte>();
            byte depth = new byte();

            for (int i = 0; i < pointsAmount; i++)
            {
                depth = (byte)(secPoints[i].Y / maxDepth * 255);
                double dist;

                if (i == 0)
                {
                    dist = secPoints[i].X * scale;
                    vectors.Add(startCoord);
                    depths.Add((byte)(sectionReadWindow.ZeroPoint.Y / maxDepth * 255));
                }
                else
                {
                    dist = (secPoints[i].X - secPoints[i - 1].X) * scale;
                }

                Coord.Vector vector = CoordinatesCalculator.CalculatePoint(vectors[vectors.Count - 1], dist, azimuth);
                vectors.Add(vector);
                depths.Add(depth);
            }

            vectors.RemoveAt(vectors.Count - 1);
            depths.RemoveAt(depths.Count - 1);
            vectors.RemoveAt(vectors.Count - 1);
            depths.RemoveAt(depths.Count - 1);

            vectors.Add(endCoord);
            depths.Add(depths[depths.Count - 1]);

            Document document = new Document();
            document.Name = butt.File.Name + ".kml";

            for (int i = 1; i < vectors.Count; i++)
            {
                AddSingleLineToContainer(vectors[i - 1], vectors[i], (byte)((depths[i] + depths[i - 1]) / 2), document, i);
            }

            Kml kml = new Kml();
            kml.Feature = document;
            KmlFile kmlFile = KmlFile.Create(kml, true);

            AddToList(kmlFile);
            SaveKmls(kmlFile);
            FileButton<KmlFile> button = new FileButton<KmlFile>(kmlFile)
            {
                Height = 40,
                Margin = new Thickness(10),
                Content = document.Name
            };
            button.Click += OnOutFileButtonClick;

            OutStack.Children.Add(button);
        }

        private void OnAnalizeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DoAnalize(current);
            }
            catch
            {
                MessageBox.Show("Nie podano żadnego punktu");
            }
        }

        private void OnProfileButtonClick(object sender, RoutedEventArgs e)
        {
            FileButton<Profile> fileButton = sender as FileButton<Profile>;
            if(current == null)
            {
                current = fileButton;
                ChangeButtons(true);
                fileButton.Background = new System.Windows.Media.SolidColorBrush(Colors.DarkGray);
            }
            else
            {
                current = null;
                ChangeButtons(false);
                fileButton.Background = new System.Windows.Media.SolidColorBrush(Colors.LightGray);
            }

        }

        private void OnOutFileButtonClick(object sender, RoutedEventArgs e)
        {

            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kmz\data\kml");
        }

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

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        public void AddSingleLineToContainer(Coord.Vector start, Coord.Vector end, byte color, Container container, int number)
        {
            SharpKml.Dom.Style style = new SharpKml.Dom.Style();

            style.Id = "LineStyle" + number.ToString();
            style.Line = new LineStyle();
            style.Line.Color = new Coord.Color32(255, 0, (byte)(255 - color), color);
            style.Line.Width = 3;

            Placemark placemark = new Placemark();
            placemark.StyleUrl = new Uri("LineStyle" + number.ToString(), UriKind.Relative);

            LineString lineString = new LineString();
            lineString.Coordinates = new CoordinateCollection();
            lineString.Coordinates.Add(start);
            lineString.Coordinates.Add(end);

            placemark.Geometry = lineString;
            container.AddFeature(placemark);
            container.AddStyle(style);
        }

    }
}