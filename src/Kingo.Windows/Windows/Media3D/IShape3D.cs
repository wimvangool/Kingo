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
    /// When implemented by a class, represents a shape in 3D-space.
    /// </summary>
    public interface IShape3D : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates whether or not adjacent triangles share their vertices.
        /// </summary>
        bool ShareVertices
        {
            get;
            set;
        }

        /// <summary>
        /// Geometry describing the body of the shape.
        /// </summary>
        MeshGeometry3D Geometry
        {
            get;
        }
    }
}
