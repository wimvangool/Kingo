using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class MeshGeometryBuilderTest
    {
        private MeshGeometry3DBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new MeshGeometry3DBuilder();
        }

        #region [====== AddVertex ======]

        [TestMethod]
        public void AddVertex_ReturnsExpectedIndex_IfVertexIsNewVertex()
        {
            Assert.AreEqual(0, _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate()));
            Assert.AreEqual(1, _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate()));
            Assert.AreEqual(2, _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate()));
        }

        [TestMethod]
        public void AddVertex_ReturnsExpectedIndex_IfVertexIsExistingVertex()
        {
            var x = RandomCoordinate();
            var y = RandomCoordinate();
            var z = RandomCoordinate();

            Assert.AreEqual(0, _builder.AddVertex(x, y, z));
            Assert.AreEqual(0, _builder.AddVertex(x, y, z));            
        }

        #endregion

        #region [====== AddTriangle ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddTriangle_Throws_IfIndexAIsNotValid()
        {
            var indexA = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexB = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexC = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            _builder.AddTriangle(indexA - 1, indexB, indexC);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddTriangle_Throws_IfIndexBIsNotValid()
        {
            var indexA = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexB = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexC = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            _builder.AddTriangle(indexA, indexB + 2, indexC);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddTriangle_Throws_IfIndexCIsNotValid()
        {
            var indexA = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexB = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexC = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            _builder.AddTriangle(indexA, indexB, indexC + 1);
        }

        [TestMethod]        
        public void AddTriangle_AddsTheSpecifiedTriangle_IfAllIndicesAreValid()
        {
            var indexA = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexB = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var indexC = _builder.AddVertex(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            var triangle = _builder.AddTriangle(indexA, indexB, indexC);

            Assert.AreEqual(indexA, triangle.VertexA);
            Assert.AreEqual(indexB, triangle.VertexB);
            Assert.AreEqual(indexC, triangle.VertexC);
        }

        #endregion

        #region [====== BuildMeshGeometry ======]

        [TestMethod]
        public void BuildMeshGeometry_ReturnsEmptyGeometry_IfNoVerticesAndTrianglesWereAdded_And_ShareVerticesIsTrue()
        {
            var mesh = _builder.BuildMeshGeometry(true);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(0, mesh.Positions.Count);            
            Assert.AreEqual(0, mesh.TriangleIndices.Count);            
            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        [TestMethod]
        public void BuildMeshGeometry_ReturnsEmptyGeometry_IfNoVerticesAndTrianglesWereAdded_And_ShareVerticesIsFalse()
        {
            var mesh = _builder.BuildMeshGeometry(false);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(0, mesh.Positions.Count);
            Assert.AreEqual(0, mesh.TriangleIndices.Count);
            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        [TestMethod]
        public void BuildMeshGeometry_ReturnsExpectedGeometry_IfGeometryIsTriangle_And_ShareVerticesIsTrue()
        {
            var vertexA = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexB = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexC = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            var indexA = _builder.AddVertex(vertexA);
            var indexB = _builder.AddVertex(vertexB);
            var indexC = _builder.AddVertex(vertexC);

            _builder.AddTriangle(indexA, indexB, indexC);

            var mesh = _builder.BuildMeshGeometry(true);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(3, mesh.Positions.Count);
            Assert.AreEqual(vertexA, mesh.Positions[indexA]);
            Assert.AreEqual(vertexB, mesh.Positions[indexB]);
            Assert.AreEqual(vertexC, mesh.Positions[indexC]);

            Assert.AreEqual(3, mesh.TriangleIndices.Count);
            Assert.AreEqual(indexA, mesh.TriangleIndices[0]);
            Assert.AreEqual(indexB, mesh.TriangleIndices[1]);
            Assert.AreEqual(indexC, mesh.TriangleIndices[2]);

            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        [TestMethod]
        public void BuildMeshGeometry_ReturnsExpectedGeometry_IfGeometryIsTriangle_And_ShareVerticesIsFalse()
        {
            var vertexA = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexB = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexC = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            var indexA = _builder.AddVertex(vertexA);
            var indexB = _builder.AddVertex(vertexB);
            var indexC = _builder.AddVertex(vertexC);

            _builder.AddTriangle(indexA, indexB, indexC);

            var mesh = _builder.BuildMeshGeometry(false);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(3, mesh.Positions.Count);
            Assert.AreEqual(vertexA, mesh.Positions[indexA]);
            Assert.AreEqual(vertexB, mesh.Positions[indexB]);
            Assert.AreEqual(vertexC, mesh.Positions[indexC]);

            Assert.AreEqual(3, mesh.TriangleIndices.Count);
            Assert.AreEqual(indexA, mesh.TriangleIndices[0]);
            Assert.AreEqual(indexB, mesh.TriangleIndices[1]);
            Assert.AreEqual(indexC, mesh.TriangleIndices[2]);

            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        [TestMethod]
        public void BuildMeshGeometry_ReturnsExpectedGeometry_IfGeometryIsRectangle_And_ShareVerticesIsTrue()
        {
            var vertexA = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexB = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexC = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexD = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            var indexA = _builder.AddVertex(vertexA);
            var indexB = _builder.AddVertex(vertexB);
            var indexC = _builder.AddVertex(vertexC);
            var indexD = _builder.AddVertex(vertexD);

            _builder.AddTriangle(indexA, indexB, indexC);
            _builder.AddTriangle(indexA, indexC, indexD);

            var mesh = _builder.BuildMeshGeometry(true);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(4, mesh.Positions.Count);
            Assert.AreEqual(vertexA, mesh.Positions[indexA]);
            Assert.AreEqual(vertexB, mesh.Positions[indexB]);
            Assert.AreEqual(vertexC, mesh.Positions[indexC]);
            Assert.AreEqual(vertexD, mesh.Positions[indexD]);

            Assert.AreEqual(6, mesh.TriangleIndices.Count);
            Assert.AreEqual(indexA, mesh.TriangleIndices[0]);
            Assert.AreEqual(indexB, mesh.TriangleIndices[1]);
            Assert.AreEqual(indexC, mesh.TriangleIndices[2]);
            Assert.AreEqual(indexA, mesh.TriangleIndices[3]);
            Assert.AreEqual(indexC, mesh.TriangleIndices[4]);
            Assert.AreEqual(indexD, mesh.TriangleIndices[5]);

            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        [TestMethod]
        public void BuildMeshGeometry_ReturnsExpectedGeometry_IfGeometryIsRectangle_And_ShareVerticesIsFalse()
        {
            var vertexA = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexB = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexC = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());
            var vertexD = new Point3D(RandomCoordinate(), RandomCoordinate(), RandomCoordinate());

            var indexA = _builder.AddVertex(vertexA);
            var indexB = _builder.AddVertex(vertexB);
            var indexC = _builder.AddVertex(vertexC);
            var indexD = _builder.AddVertex(vertexD);

            _builder.AddTriangle(indexA, indexB, indexC);
            _builder.AddTriangle(indexA, indexC, indexD);

            var mesh = _builder.BuildMeshGeometry(false);

            Assert.IsNotNull(mesh);
            Assert.AreEqual(6, mesh.Positions.Count);
            Assert.AreEqual(vertexA, mesh.Positions[0]);
            Assert.AreEqual(vertexB, mesh.Positions[1]);
            Assert.AreEqual(vertexC, mesh.Positions[2]);
            Assert.AreEqual(vertexA, mesh.Positions[3]);
            Assert.AreEqual(vertexC, mesh.Positions[4]);
            Assert.AreEqual(vertexD, mesh.Positions[5]);

            Assert.AreEqual(6, mesh.TriangleIndices.Count);
            Assert.AreEqual(0, mesh.TriangleIndices[0]);
            Assert.AreEqual(1, mesh.TriangleIndices[1]);
            Assert.AreEqual(2, mesh.TriangleIndices[2]);
            Assert.AreEqual(3, mesh.TriangleIndices[3]);
            Assert.AreEqual(4, mesh.TriangleIndices[4]);
            Assert.AreEqual(5, mesh.TriangleIndices[5]);

            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.TextureCoordinates.Count);
        }

        #endregion

        private static readonly Random _Random = new Random();

        private static double RandomCoordinate()
        {
            lock (_Random)
            {
                return _Random.Next(-100, 100);
            }
        }
    }
}
