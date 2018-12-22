using KMZ.Classes;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

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
        }

        public List<Point> Points { get; set; }
        public List<Point> Markers { get; set; }
        public Point ZeroPoint { get; set; } //point on the 0,0 coords
        public Point LastPoint { get; set; }

        private bool canDraw = false;
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

        private Point GetCoordsImage(MouseButtonEventArgs e)
        {
            return e.GetPosition(SectionImage);
        }

        private Point GetCoordsCanvas(MouseButtonEventArgs e)
        {
            return e.GetPosition(Cnv);
        }

        private void OnSectionClick(object sender, MouseButtonEventArgs e)
        {
            Point imagePoint = GetCoordsImage(e);
            Point cnvPoint = GetCoordsCanvas(e);
            ClickProcessing(imagePoint);

            if (canDraw)
                DrawCross(cnvPoint);
        }

        private void DrawCross(Point center)
        {
            int length = 20;

            Point point = new Point(center.X + ZeroPoint.X, center.Y - ZeroPoint.Y);

            Line verticalLine = new Line()
            {
                Margin = new Thickness(0),
                Visibility = Visibility.Visible,
                StrokeThickness = 3,
                Stroke = System.Windows.Media.Brushes.Black,
                X1 = center.X,
                X2 = center.X,
                Y1 = center.Y - length,
                Y2 = center.Y + length 
            };
            Line horizontalLine = new Line()
            {
                Margin = new Thickness(0),
                Visibility = Visibility.Visible,
                StrokeThickness = 3,
                Stroke = System.Windows.Media.Brushes.Black,
                Y1 = center.Y,
                Y2 = center.Y,
                X1 = center.X - length,
                X2 = center.X + length
            };

            Cnv.Children.Add(verticalLine);
            Cnv.Children.Add(horizontalLine);
        }
        
        private void DrawMarker(Point point)
        {
            int wid = 15;
            int hei = 30;

            Line rightLine = new Line()
            {
                Margin = new Thickness(0),
                Visibility = Visibility.Visible,
                StrokeThickness = 3,
                Stroke = System.Windows.Media.Brushes.Red,
                X1 = point.X,
                X2 = point.X + wid,
                Y1 = point.Y,
                Y2 = point.Y - hei
            };

            Line leftLine = new Line()
            {
                Margin = new Thickness(0),
                Visibility = Visibility.Visible,
                StrokeThickness = 3,
                Stroke = System.Windows.Media.Brushes.Red,
                X1 = point.X,
                X2 = point.X - wid,
                Y1 = point.Y,
                Y2 = point.Y - hei
            };

            Cnv.Children.Add(rightLine);
            Cnv.Children.Add(leftLine);
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
                        canDraw = true;
                        break;

                    default:
                        break;
                }
            }
        }

        private void OnRightClick(object sender, MouseButtonEventArgs e)
        {
            if (State == PointingState.LayerPoints)
            {
                Point imagePoint = GetCoordsImage(e);
                Point cnvPoint = GetCoordsCanvas(e);
                RightClickProcessing(imagePoint);

                if (canDraw)
                    DrawMarker(cnvPoint);
            }
        }

        private void RightClickProcessing(Point point)
        {
            Markers.Add(point);
        }
    }
}