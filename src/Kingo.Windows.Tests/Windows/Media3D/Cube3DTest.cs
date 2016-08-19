using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class Cube3DTest
    {
        private List<string> _changedProperties;
        private Cube3D _cube;

        [TestInitialize]
        public void Setup()
        {
            _changedProperties = new List<string>();
            _cube = new Cube3D();
            _cube.PropertyChanged += (s, e) => _changedProperties.Add(e.PropertyName);
        }

        #region [====== ShareVertices ======]

        [TestMethod]
        public void SetSharedVertices_HasNoEffect_IfValueIsNotChanged()
        {
            _cube.ShareVertices = false;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetSharedVertices_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cube.ShareVertices = true;

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cube.ShareVertices), _changedProperties[0]);
            Assert.AreEqual(nameof(_cube.Geometry), _changedProperties[1]);
        }

        #endregion

        #region [====== Size ======]

        [TestMethod]
        public void SetSize_HasNoEffect_IfValueIsNotChanged()
        {
            _cube.Size = Distance.Unit;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetSize_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cube.Size = new Distance(4);

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cube.Size), _changedProperties[0]);
            Assert.AreEqual(nameof(_cube.Geometry), _changedProperties[1]);
        }

        #endregion
    }
}
