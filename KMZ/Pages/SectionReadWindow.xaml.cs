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
        LastPoint,
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
        public Point LastPoint { get; set; }
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

                    case PointingState.LastPoint:
                        CommandBlock.Text = "Click on the low-right point";
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
    }
}