using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Kingo.Resources;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a builder that can be used to build a shape's <see cref="MeshGeometry3D"/>.
    /// </summary>
    public sealed class MeshGeometry3DBuilder
    {        
        private const int _DefaultCapacity = 64;
        private readonly List<Point3D> _vertices;
        private readonly List<Triangle3DInfo> _triangles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometry3DBuilder" /> class.
        /// </summary>
        public MeshGeometry3DBuilder()
            : this(new List<Point3D>(_DefaultCapacity), new List<Triangle3DInfo>(_DefaultCapacity)) { }

        private MeshGeometry3DBuilder(List<Point3D> vertices, List<Triangle3DInfo> triangles)
        {
            _vertices = vertices;
            _triangles = triangles;
        }

        /// <summary>
        /// Adds a new vertex to the mesh with the specified points and returns its index.
        /// </summary>
        /// <param name="x">X-location of the vertex.</param>
        /// <param name="y">Y-location of the vertex.</param>
        /// <param name="z">Z-location of the vertex.</param>
        /// <returns>The index of the vertex.</returns>
        public int AddVertex(double x, double y, double z)
        {
            return AddVertex(new Point3D(x, y, z));
        }

        /// <summary>
        /// Adds a new vertex to the mesh and returns its index.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        /// <returns>The index of the vertex.</returns>
        public int AddVertex(Point3D vertex)
        {
            var vertexIndex = _vertices.IndexOf(vertex);
            if (vertexIndex < 0)
            {
                _vertices.Add(vertex);

                return _vertices.Count - 1;
            }
            return vertexIndex;          
        }

        /// <summary>
        /// Adds a new triangle to the mesh with the specified indices.
        /// </summary>
        /// <param name="vertexA">Index of the first vertex.</param>
        /// <param name="vertexB">Index of the second vertex.</param>
        /// <param name="vertexC">Index of the third vertex.</param>
        /// <returns>The added triangle.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="vertexA"/>, <paramref name="vertexB"/> or <paramref name="vertexC"/> does not point
        /// to an existing vertex.
        /// </exception>
        public Triangle3DInfo AddTriangle(int vertexA, int vertexB, int vertexC)
        {
            if (NotExists(vertexA))
            {
                throw NewInvalidVertexException(vertexA, nameof(vertexA));
            }
            if (NotExists(vertexB))
            {
                throw NewInvalidVertexException(vertexB, nameof(vertexB));
            }
            if (NotExists(vertexC))
            {
                throw NewInvalidVertexException(vertexC, nameof(vertexC));
            }
            var triangle = new Triangle3DInfo(vertexA, vertexB, vertexC);

            _triangles.Add(triangle);

            return triangle;
        }        

        private bool NotExists(int vertex)
        {
            return !(0 <= vertex && vertex < _vertices.Count);
        }

        private static ArgumentOutOfRangeException NewInvalidVertexException(int vertex, string paramName)
        {            
            var messageFormat = ExceptionMessages.MeshGeometry3DBuilder_InvalidVertex;
            var message = string.Format(messageFormat, vertex);
            return new ArgumentOutOfRangeException(paramName, message);            
        }

        /// <summary>
        /// Creates and returns a new <see cref="MeshGeometry3D"/> based on all added vertices and triangles.
        /// </summary>
        /// <param name="shareVertices">
        /// Indicates whether or not the vertices should be shared amongst adjacent triangles.
        /// </param>
        /// <returns>A new <see cref="MeshGeometry3D"/>-instance containing all vertices and triangles.</returns>
        public MeshGeometry3D BuildMeshGeometry(bool shareVertices)
        {
            if (shareVertices)
            {
                return BuildMeshGeometry();
            }
            return DuplicateSharedVertices().BuildMeshGeometry();
        }

        private MeshGeometry3D BuildMeshGeometry()
        {
            var mesh = new MeshGeometry3D();

            foreach (var vertex in _vertices)
            {
                mesh.Positions.Add(vertex);
            }
            foreach (var triangle in _triangles)
            {
                mesh.TriangleIndices.Add(triangle.VertexA);
                mesh.TriangleIndices.Add(triangle.VertexB);
                mesh.TriangleIndices.Add(triangle.VertexC);
            }
            return mesh;
        }

        private MeshGeometry3DBuilder DuplicateSharedVertices()
        {
            var vertices = new List<Point3D>(_vertices.Count);
            var triangles = new List<Triangle3DInfo>(_triangles.Count);

            foreach (var triangle in _triangles)
            {
                triangles.Add(triangle.DuplicateSharedVertices(_vertices, vertices));
            }
            return new MeshGeometry3DBuilder(vertices, triangles);
        }
    }
}
