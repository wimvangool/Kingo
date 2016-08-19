using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a set of vertices that form a triangle inside a mesh.
    /// </summary>
    public struct Triangle3DInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle3DInfo" /> structure.
        /// </summary>
        /// <param name="vertexA">Vertex A.</param>
        /// <param name="vertexB">Vertex B.</param>
        /// <param name="vertexC">Vertex C.</param>
        public Triangle3DInfo(int vertexA, int vertexB, int vertexC)
        {
            VertexA = vertexA;
            VertexB = vertexB;
            VertexC = vertexC;
        }

        /// <summary>
        /// Vertex A.
        /// </summary>
        public int VertexA
        {
            get;
        }

        /// <summary>
        /// Vertex B.
        /// </summary>
        public int VertexB
        {
            get;
        }

        /// <summary>
        /// Vertex C.
        /// </summary>
        public int VertexC
        {
            get;
        }

        internal Triangle3DInfo DuplicateSharedVertices(IList<Point3D> source, IList<Point3D> destination)
        {
            var vertexA = AddVertex(source, destination, VertexA);
            var vertexB = AddVertex(source, destination, VertexB);
            var vertexC = AddVertex(source, destination, VertexC);

            return new Triangle3DInfo(vertexA, vertexB, vertexC);
        }

        private static int AddVertex(IList<Point3D> source, IList<Point3D> destination, int vertex)
        {
            destination.Add(source[vertex]);
            return destination.Count - 1;
        }
    }
}
