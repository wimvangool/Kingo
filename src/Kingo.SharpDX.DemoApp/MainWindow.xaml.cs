using System.Windows;

namespace Kingo.SharpDX.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        #region [====== HandleLoaded ======]

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            Top = CalculateTop();
            Left = CalculateLeft();
        }

        private double CalculateTop()
        {
            return (SystemParameters.PrimaryScreenHeight / 2) - (Height / 2);
        }

        private double CalculateLeft()
        {
            return (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
        }

        #endregion

        #region [====== HandleUnloaded ======]

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region [====== Start & Stop Rendering ======]

        private void HandleStartRenderingClicked(object sender, RoutedEventArgs e)
        {
            _surface.StartRendering(() => new CubeImage());
        }

        private void HandleStopRenderingClicked(object sender, RoutedEventArgs e)
        {
            _surface.StopRendering();
        }

        #endregion
    }
}
