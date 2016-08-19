using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Serves as a base-class for all classes that implement the <see cref="IShape3D"/> interface.
    /// </summary>
    public abstract class Shape3D : IShape3D
    {
        #region [====== PropertyChanged ======]

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/>-event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        /// <param name="regenerateGeometry">
        /// Indicates whether or not the specified <paramref name="propertyName"/> is a property that is part of
        /// the geometry description of this shape and thus requires the geometry to be re-generated on the next access.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName, bool regenerateGeometry = true)
        {            
            if (regenerateGeometry)
            {
                _geometry = null;

                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

                if (string.IsNullOrEmpty(propertyName))
                {
                    return;
                }                
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Geometry)));
            }
            else
            {                
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged.Raise(this, e);
        }

        #endregion

        #region [====== Basic Shape Properties ======]

        private bool _shareVertices;

        /// <summary>
        /// Indicates whether or not the vertices should be shared between adjacent triangles.
        /// </summary>
        public bool ShareVertices
        {
            get { return _shareVertices; }
            set
            {
                if (_shareVertices != value)
                {
                    _shareVertices = value;

                    OnPropertyChanged(nameof(ShareVertices));
                }
            }
        }        

        private MeshGeometry3D _geometry;

        /// <inheritdoc />
        public MeshGeometry3D Geometry
        {
            get
            {
                if (_geometry == null)
                {
                    _geometry = GenerateGeometry();                        
                }
                return _geometry;
            }
        }

        /// <summary>
        /// Generates the <see cref="MeshGeometry3D" /> of this shape.
        /// </summary>
        /// <returns>The <see cref="MeshGeometry3D" /> of this shape.</returns>
        protected abstract MeshGeometry3D GenerateGeometry();        

        #endregion      
    }
}
