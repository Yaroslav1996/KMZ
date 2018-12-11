using KMZ.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;
using System.Windows.Controls;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for SectionReadWindow.xaml
    /// </summary>

    public enum PointingState
    {
        ZeroPoint,
        LastPoint,
        LayerPoints
    }

    public partial class SectionReadWindow : Window
    {
        public SectionReadWindow(Section section, bool showing)
        {
            InitializeComponent();
            Points = new List<Point>();
            Markers = new List<Point>();
            SectionName.Text = section.Name;
            SectionImage.Source = BitmapConversion.ToWpfBitmap(section.Image);
            IsOnlyShowing = showing;
            if (showing)
                CommandBlock.Visibility = Visibility.Hidden;

            Cnv.Height = SectionImage.Height;
            Cnv.Width = SectionImage.Width;
        }

        public List<Point> Points { get; set; }
        public List<Point> Markers { get; set; }
        public Point ZeroPoint { get; set; } //point on the 0,0 coords
        public Point LastPoint { get; set; }
        private PointingState _State = PointingState.ZeroPoint;
        public PointingState State
        {
            get => _State;
            set
            {
                switch (value)
                {
                    case PointingState.ZeroPoint:
                        CommandBlock.Text = "Zaznacz punkt w lewym-górnym rogu profilu";
                        _State = value;
                        break;

                    case PointingState.LastPoint:
                        CommandBlock.Text = "Zaznacz punkt w prawym-dolnym rogu profilu";
                        _State = value;
                        break;

                    case PointingState.LayerPoints:
                        CommandBlock.Text = "Zaznaczaj punty wzdłuż wybranej warstwy";
                        _State = value;
                        break;

                    default:
                        break;
                }
            }
        }
        public bool IsOnlyShowing { get; set; }

        private Point GetCoords(MouseButtonEventArgs e)
        {
            return e.GetPosition(SectionImage);
        }

        private void OnSectionClick(object sender, MouseButtonEventArgs e)
        {
            Point point = GetCoords(e);
            ClickProcessing(point);

            DropImage(point, Properties.Resources.crosss);
        }

        private void DropImage(Point point, Bitmap bitmap)
        {
            Image img = new Image();
            img.Source = BitmapConversion.ToWpfBitmap(BitmapConversion.ResizeImage(bitmap, 53, 30));

            Cnv.Children.Add(img);
            Canvas.SetLeft(img, point.X + 200);
            Canvas.SetTop(img, point.Y - 30);
        }

        private void ClickProcessing(Point point)
        {
            if (!IsOnlyShowing)
            {
                switch (State)
                {
                    case PointingState.ZeroPoint:
                        ZeroPoint = point;
                        State++;
                        break;

                    case PointingState.LastPoint:
                        LastPoint = point;
                        State++;
                        break;

                    case PointingState.LayerPoints:
                        Points.Add(point);
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnRightClick(object sender, MouseButtonEventArgs e)
        {
            Point point = GetCoords(e);
            RightClickProcessing(point);

            DropImage(point, Properties.Resources.marker);
        }

        private void RightClickProcessing(Point point)
        {
            Markers.Add(point);
        }
    }
}