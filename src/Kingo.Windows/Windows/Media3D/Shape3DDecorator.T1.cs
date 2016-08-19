using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a class, represents a shape that encapsulates another, typically more elaborate shape.
    /// </summary>
    /// <typeparam name="TShape">Type of the decorated shape.</typeparam>
    public abstract class Shape3DDecorator<TShape> where TShape : class, IShape3D
    {
        #region [====== ChangePropertyScope ======]

        private sealed class ChangePropertyScope : Disposable
        {
            private readonly Shape3DDecorator<TShape> _decorator;
            private readonly string _propertyName;
            private readonly bool _regenerateGeometry;

            public ChangePropertyScope(Shape3DDecorator<TShape> decorator, string propertyName, bool regenerateGeometry)
            {
                _decorator = decorator;
                _decorator.Shape.PropertyChanged -= _decorator.HandleShapePropertyChanged;

                _propertyName = propertyName;
                _regenerateGeometry = regenerateGeometry;
            }

            protected override void DisposeManagedResources()
            {
                _decorator.Shape.PropertyChanged += _decorator.HandleShapePropertyChanged;
                _decorator.OnPropertyChanged(_propertyName, _regenerateGeometry);

                base.DisposeManagedResources();
            }
        }

        #endregion

        #region [====== PropertyChanged ======]

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;        

        /// <summary>
        /// This method is called when one of the decorated shape's properties changes.
        /// </summary>
        /// <param name="sender">The decorated shape.</param>
        /// <param name="e">Arguments containing the name of the property that changed.</param>
        private void HandleShapePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        /// <param name="regenerateGeometry">
        /// Indicates whether or not the specified <paramref name="propertyName"/> is a property that is part of
        /// the geometry description of this shape and thus requires the geometry to be re-generated on the next access.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName, bool regenerateGeometry = true)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            if (regenerateGeometry)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Geometry)));
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged.Raise(this, e);
        }

        /// <summary>
        /// Creates and returns a scope that can be used to edit the properties of the decorated shape without raising the property-changed events.
        /// </summary>
        /// <param name="propertyName">Name of the parent property that is being editted.</param>
        /// <param name="regenerateGeometry">
        /// Indicates whether or not the specified <paramref name="propertyName"/> is a property that is part of
        /// the geometry description of this shape and thus requires the geometry to be re-generated on the next access.
        /// </param>
        /// <returns>A new edit scope.</returns>
        protected IDisposable ChangeProperty(string propertyName, bool regenerateGeometry = true)
        {
            return new ChangePropertyScope(this, propertyName, regenerateGeometry);
        }

        #endregion

        #region [====== Basic Shape Properties ======]

        /// <inheritdoc />
        public bool ShareVertices
        {
            get { return Shape.ShareVertices; }
            set { Shape.ShareVertices = value; }
        }

        /// <inheritdoc />
        public MeshGeometry3D Geometry => Shape.Geometry;

        #endregion

        #region [====== Shape ======]

        private TShape _shape;

        /// <summary>
        /// The decorated shape.
        /// </summary>
        protected TShape Shape
        {
            get
            {
                if (_shape == null)
                {
                    _shape = CreateShape();
                    _shape.PropertyChanged += HandleShapePropertyChanged;
                }
                return _shape;
            }
        }

        /// <summary>
        /// Creates and returns the decorated shape.
        /// </summary>
        /// <returns>The decorated shape.</returns>
        protected abstract TShape CreateShape();        

        #endregion
    }
}
