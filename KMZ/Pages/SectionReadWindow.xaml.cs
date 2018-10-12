using KMZ.Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for SectionReadWindow.xaml
    /// </summary>

    public enum PointingState
    {
        ZeroPoint,
        DistPoint,
        DepthPoint,
        LayerPoints
    }

    public partial class SectionReadWindow : Window
    {
        public SectionReadWindow(Section section, bool showing)
        {
            InitializeComponent();
            Points = new List<Point>();
            SectionName.Text = section.Name;
            SectionImage.Source = BitmapConversion.ToWpfBitmap(section.Image);
            IsOnlyShowing = showing;
            if (showing)
                CommandBlock.Visibility = Visibility.Hidden;
        }

        public List<Point> Points { get; set; }
        public Point ZeroPoint { get; set; } //point on the 0,0 coords
        public double Dist { get; set; } //distance unit
        public double Depth { get; set; } //depth unit
        private PointingState _State;
        public PointingState State
        {
            get => _State;
            set
            {
                switch (value)
                {
                    case PointingState.ZeroPoint:
                        CommandBlock.Text = "Click on the 0,0 point";
                        _State = value;
                        break;

                    case PointingState.DistPoint:
                        CommandBlock.Text = "Click on the point 1 unit along the profile";
                        _State = value;
                        break;

                    case PointingState.DepthPoint:
                        CommandBlock.Text = "Click on the point 1 unit deep.";
                        _State = value;
                        break;

                    case PointingState.LayerPoints:
                        CommandBlock.Text = "Click on the points along the layer";
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
            ClickProcessing(GetCoords(e));
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

                    case PointingState.DistPoint:
                        Dist = point.X;
                        State++;
                        break;

                    case PointingState.DepthPoint:
                        Depth = point.Y;
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
    }
}