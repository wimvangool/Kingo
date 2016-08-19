using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a cuboid with a certain width, height and depth.
    /// </summary>
    public sealed class Cuboid3D : Shape3D
    {
        #region [====== Width ======]

        private Distance _width = Distance.Unit;

        /// <summary>
        /// Gets or sets the width of the cubiod, corresponding to the x-value in world-coordinates.
        /// </summary>
        public Distance Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;

                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        #endregion

        #region [====== Height ======]

        private Distance _height = Distance.Unit;

        /// <summary>
        /// Gets or sets the height of this cuboid, corresponding to the y-value in world-coordinates.
        /// </summary>
        public Distance Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;

                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        #endregion

        #region [====== Depth ======]

        private Distance _depth = Distance.Unit;

        /// <summary>
        /// Gets or sets the depth of this cuboid, corresponding to the z-value in world-coordinates.
        /// </summary>
        public Distance Depth
        {
            get { return _depth; }
            set
            {
                if (_depth != value)
                {
                    _depth = value;

                    OnPropertyChanged(nameof(Depth));
                }
            }
        }

        #endregion

        /// <inheritdoc />
        protected override MeshGeometry3D GenerateGeometry()
        {
            var builder = new MeshGeometry3DBuilder();
            var x = Width / 2;
            var y = Height / 2;
            var z = Depth / 2;

            AddFrontFace(builder, x, y, z);
            AddBackFace(builder, x, y, z);

            AddLeftFace(builder, x, y, z);
            AddRightFace(builder, x, y, z);

            AddTopFace(builder, x, y, z);
            AddBottomFace(builder, x, y, z);

            return builder.BuildMeshGeometry(ShareVertices);
        }     
        
        private static void AddFrontFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(-x, -y, z);
            var bottomRight = builder.AddVertex(x, -y, z);
            var topRight = builder.AddVertex(x, y, z);
            var topLeft = builder.AddVertex(-x, y, z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }

        private static void AddBackFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(x, -y, -z);
            var bottomRight = builder.AddVertex(-x, -y, -z);
            var topRight = builder.AddVertex(-x, y, -z);
            var topLeft = builder.AddVertex(x, y, -z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }

        private static void AddRightFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(x, -y, z);
            var bottomRight = builder.AddVertex(x, -y, -z);
            var topRight = builder.AddVertex(x, y, -z);
            var topLeft = builder.AddVertex(x, y, z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }        

        private static void AddLeftFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(-x, -y, -z);
            var bottomRight = builder.AddVertex(-x, -y, z);
            var topRight = builder.AddVertex(-x, y, z);
            var topLeft = builder.AddVertex(-x, y, -z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }

        private static void AddTopFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(-x, y, z);
            var bottomRight = builder.AddVertex(x, y, z);
            var topRight = builder.AddVertex(x, y, -z);
            var topLeft = builder.AddVertex(-x, y, -z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }

        private static void AddBottomFace(MeshGeometry3DBuilder builder, double x, double y, double z)
        {
            var bottomLeft = builder.AddVertex(-x , -y, -z);
            var bottomRight = builder.AddVertex(x, -y, -z);
            var topRight = builder.AddVertex(x, -y, z);
            var topLeft = builder.AddVertex(-x, -y, z);

            AddFace(builder, bottomLeft, bottomRight, topRight, topLeft);
        }

        private static void AddFace(MeshGeometry3DBuilder builder, int bottomLeft, int bottomRight, int topRight, int topLeft)
        {
            builder.AddTriangle(bottomLeft, bottomRight, topRight);
            builder.AddTriangle(bottomLeft, topRight, topLeft);
        }
    }
}
