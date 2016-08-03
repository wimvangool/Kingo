using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When attached as a behavior to a <see cref="ProjectionCamera"/>, adds camera navigation such
    /// as moving, panning, zooming and orbiting the camera.
    /// </summary>
    [ContentProperty(nameof(ControlModes))]
    public sealed class ProjectionCameraControllerBehavior : Behavior<ProjectionCamera>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraControllerBehavior" /> class.
        /// </summary>
        public ProjectionCameraControllerBehavior()
            : this(new ProjectionCameraController()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraControllerBehavior" /> class.
        /// </summary>
        /// <param name="controller">The controller that is used to control the associated <see cref="ProjectionCamera" />.</param>
        public ProjectionCameraControllerBehavior(IProjectionCameraController controller)
        {
            Controller = controller;
            ControlModes = new ObservableCollection<ControlMode>();
            ControlModes.CollectionChanged += HandleControlModesChanged;
        }

        #region [====== Controller ======]

        /// <summary>
        /// Backing-field of the <see cref="Controller"/>-property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IProjectionCameraController), typeof(ProjectionCameraControllerBehavior), new FrameworkPropertyMetadata(HandleControllerChanged));

        /// <summary>
        /// Gets or sets the controller that is used to control the associated <see cref="ProjectionCamera" />.
        /// </summary>
        public IProjectionCameraController Controller
        {
            get { return (IProjectionCameraController) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        private static void HandleControllerChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void AttachController(IProjectionCameraController controller)
        {
            
        }

        private void DetachController(IProjectionCameraController controller)
        {
            
        }

        #endregion

        #region [====== InputSource ======]

        /// <summary>
        /// Backing-field of the <see cref="InputSource"/>-property.
        /// </summary>
        public static readonly DependencyProperty InputSourceProperty =
            DependencyProperty.Register(nameof(InputSource), typeof(UIElement), typeof(ProjectionCameraControllerBehavior), new FrameworkPropertyMetadata(HandleInputSourceChanged));

        /// <summary>
        /// Gets or sets the <see cref="UIElement" /> that is used to attach all eventhandlers that control the navigation to..
        /// </summary>
        public UIElement InputSource
        {
            get { return (UIElement) GetValue(InputSourceProperty); }
            set { SetValue(InputSourceProperty, value); }
        }

        private static void HandleInputSourceChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            
        }        

        #endregion

        #region [====== ControlModes ======]

        /// <summary>
        /// Returns the collection of <see cref="ControlMode">ControlModes</see> that have been defined on this behavior.
        /// </summary>
        public ObservableCollection<ControlMode> ControlModes
        {
            get;
        }

        private void HandleControlModesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        #endregion

        #region [====== Attaching and Detaching ======]

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        #endregion
    }
}
