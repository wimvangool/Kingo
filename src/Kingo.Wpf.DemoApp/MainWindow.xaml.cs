using System.Windows.Input;

namespace Kingo.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Angle _Angle = Angle.FromDegrees(5);
        private const float _Distance = 0.01f;        

        public MainWindow()
        {
            InitializeComponent();
        }

        private ProjectionCameraController CameraController
        {
            get { return (ProjectionCameraController) _viewPort.Resources[nameof(CameraController)]; }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    CameraController.Move(_Distance);
                    return;
                case Key.Z:
                    CameraController.Move(-_Distance);
                    return;

                case Key.Left:
                    CameraController.MoveHorizontal(-_Distance);
                    return;
                case Key.Right:
                    CameraController.MoveHorizontal(_Distance);
                    return;

                case Key.Up:
                    CameraController.MoveVertical(_Distance);
                    return;
                case Key.Down:
                    CameraController.MoveVertical(-_Distance);
                    return;

                case Key.NumPad8:
                    CameraController.Pitch(_Angle.Invert());
                    return;
                case Key.NumPad2:
                    CameraController.Pitch(_Angle);
                    return;

                case Key.NumPad4:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        CameraController.Yaw(_Angle.Invert());
                    }
                    else
                    {
                        CameraController.Roll(_Angle.Invert());
                    }
                    return;
                case Key.NumPad6:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        CameraController.Yaw(_Angle);
                    }
                    else
                    {
                        CameraController.Roll(_Angle);
                    }
                    return;                
            }
        }
    }
}
