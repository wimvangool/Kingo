using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Threading;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents an element that can be used to show an image that is rendered directly by DirectX.
    /// </summary>
    public sealed class DirectXElement : FrameworkElement
    {
        #region [====== State ======]

        private abstract class State
        {
            public abstract bool IsRendering
            {
                get;
            }

            protected abstract DirectXElement Element
            {
                get;
            }

            protected abstract UIElement Content
            {
                get;
            }

            public virtual IEnumerator LogicalChildren
            {
                get { yield return Content; }
            }

            public virtual int VisualChildrenCount
            {
                get { return 1; }
            }

            public virtual Visual GetVisualChild(int index)
            {
                if (index == 0)
                {
                    return Content;
                }
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            public virtual Size MeasureOverride(Size availableSize)
            {
                Content.Measure(availableSize);

                return Content.DesiredSize;
            }

            public virtual Size ArrangeOverride(Size finalSize)
            {
                Content.Arrange(new Rect(finalSize));

                return Content.RenderSize;
            }

            internal virtual void Enter()
            {
                var children = LogicalChildren;

                while (children.MoveNext())
                {
                    Element.AddLogicalChild(children.Current);                    
                }                  
                for (int index = 0; index < VisualChildrenCount; index++)
                {
                    Element.AddVisualChild(GetVisualChild(index));
                }
                Element.InvalidateVisual();
            }

            public virtual void StartRendering(Func<DirectXImage> imageFactory)
            {
                throw NewInvalidOperationException(nameof(StartRendering));
            }

            public virtual void StopRendering()
            {
                throw NewInvalidOperationException(nameof(StopRendering));
            }

            private Exception NewInvalidOperationException(string operationName)
            {
                return new InvalidOperationException($"Operation '{operationName}' is not allowed when {nameof(DirectXElement)} is in the {GetType().Name}.");
            }

            internal virtual void Exit()
            {
                var children = LogicalChildren;

                while (children.MoveNext())
                {
                    Element.RemoveLogicalChild(children.Current);
                }
                for (int index = 0; index < VisualChildrenCount; index++)
                {
                    Element.RemoveVisualChild(GetVisualChild(index));
                }
            }
        }

        #endregion        

        #region [====== NotRenderingState ======]

        private sealed class NotRenderingState : State
        {            
            public NotRenderingState(DirectXElement element)
            {
                Element = element;
                Content = new Border()
                {
                    BorderThickness = new Thickness(5.0),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 10, 10))
                };
            }

            public override bool IsRendering
            {
                get { return false; }
            }

            protected override DirectXElement Element
            {
                get;
            }

            protected override UIElement Content
            {
                get;
            }

            public override void StartRendering(Func<DirectXImage> imageFactory)
            {
                Element.MoveToState(new RenderingState(Element, imageFactory));
            }
        }

        #endregion        

        #region [====== RenderingState ======]

        private sealed class RenderingState : State
        {
            private readonly DispatcherTimer _resizeDelayTimer;
            private Size _availableSize;
            
            private readonly Func<DirectXImage> _imageFactory;
            private readonly WindowsFormsHost _host;
            private DirectXImage _image;           

            public RenderingState(DirectXElement element, Func<DirectXImage> imageFactory)
            {
                Element = element;

                _resizeDelayTimer = CreateResizeDelayTimer();
                _availableSize = new Size(element.ActualWidth, element.ActualHeight);
                
                _imageFactory = imageFactory;
                _host = new WindowsFormsHost()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = _availableSize.Width,
                    Height = _availableSize.Height
                };
            }

            public override bool IsRendering
            {
                get { return true; }
            }

            protected override DirectXElement Element
            {
                get;
            }

            protected override UIElement Content
            {
                get { return _host; }
            }

            private DispatcherTimer CreateResizeDelayTimer()
            {
                var timer = new DispatcherTimer(DispatcherPriority.Render);
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += HandleResizeTimerElapsed;
                return timer;
            }

            private void HandleResizeTimerElapsed(object sender, EventArgs e)
            {
                _resizeDelayTimer.Stop();
                _host.Width = _availableSize.Width;
                _host.Height = _availableSize.Height;
            }

            public override Size MeasureOverride(Size availableSize)
            {
                ResetResizeDelayTimer();

                Content.Measure(availableSize);

                return _availableSize = availableSize;
            }

            public override Size ArrangeOverride(Size finalSize)
            {
                Content.Arrange(new Rect(finalSize));

                return finalSize;
            }

            private void ResetResizeDelayTimer()
            {
                _resizeDelayTimer.Stop();
                _resizeDelayTimer.Start();
            }

            internal override void Enter()
            {
                base.Enter();

                _image = _imageFactory.Invoke();
                _image.RenderingStarted += (s, e) => _host.Child = e.Control;
                _image.RenderingStopped += (s, e) => _host.Child = null;
                _image.StartRendering();              
            }            

            public override void StopRendering()
            {
                Element.MoveToState(new NotRenderingState(Element));
            }

            internal override void Exit()
            {                
                _image.StopRendering();
                _image.Dispose();
                _image = null;                                           

                base.Exit();
            }
        }

        #endregion        

        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXElement" /> class.
        /// </summary>
        public DirectXElement()
        {
            _state = new NotRenderingState(this);                             
            _state.Enter();

            Unloaded += HandleUnloaded;
        }        

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            if (_state.IsRendering)
            {
                _state.StopRendering();
            }
        }

        private void MoveToState(State newState)
        {
            var oldState = Interlocked.Exchange(ref _state, newState);

            oldState.Exit();
            newState.Enter();           
        }

        #region [====== FrameworkElement ======]

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get { return _state.LogicalChildren; }
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount
        {
            get { return _state.VisualChildrenCount; }
        }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return _state.GetVisualChild(index);
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            return _state.MeasureOverride(availableSize);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            return _state.ArrangeOverride(finalSize);
        }

        #endregion

        #region [====== Start & Stop Rendering ======]        

        /// <summary>
        /// Starts rendering the image that is created using the specified <paramref name="imageFactory"/>.
        /// </summary>
        /// <param name="imageFactory">Delegate that is used to create the image to be rendered.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageFactory"/> is <c>null</c>.
        /// </exception>
        public void StartRendering(Func<DirectXImage> imageFactory)
        {
            if (imageFactory == null)
            {
                throw new ArgumentNullException(nameof(imageFactory));
            }
            _state.StartRendering(imageFactory);
        }

        /// <summary>
        /// Stops rendering the image.
        /// </summary>
        public void StopRendering()
        {
            _state.StopRendering();
        }

        #endregion        
    }
}
