using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class Cuboid3DTest
    {
        private List<string> _changedProperties;
        private Cuboid3D _cuboid;

        [TestInitialize]
        public void Setup()
        {
            _changedProperties = new List<string>();
            _cuboid = new Cuboid3D();
            _cuboid.PropertyChanged += (s, e) => _changedProperties.Add(e.PropertyName);
        }

        #region [====== ShareVertices ======]

        [TestMethod]
        public void SetSharedVertices_HasNoEffect_IfValueIsNotChanged()
        {
            _cuboid.ShareVertices = false;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetSharedVertices_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cuboid.ShareVertices = true;

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cuboid.ShareVertices), _changedProperties[0]);
            Assert.AreEqual(nameof(_cuboid.Geometry), _changedProperties[1]);
        }

        #endregion

        #region [====== Width ======]

        [TestMethod]
        public void SetWidth_HasNoEffect_IfValueIsNotChanged()
        {
            _cuboid.Width = Distance.Unit;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetWidth_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cuboid.Width = new Distance(4);

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cuboid.Width), _changedProperties[0]);
            Assert.AreEqual(nameof(_cuboid.Geometry), _changedProperties[1]);
        }

        #endregion

        #region [====== Height ======]

        [TestMethod]
        public void SetHeight_HasNoEffect_IfValueIsNotChanged()
        {
            _cuboid.Height = Distance.Unit;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetHeight_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cuboid.Height = new Distance(4);

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cuboid.Height), _changedProperties[0]);
            Assert.AreEqual(nameof(_cuboid.Geometry), _changedProperties[1]);
        }

        #endregion

        #region [====== Depth ======]

        [TestMethod]
        public void SetDepth_HasNoEffect_IfValueIsNotChanged()
        {
            _cuboid.Depth = Distance.Unit;

            Assert.AreEqual(0, _changedProperties.Count);
        }

        [TestMethod]
        public void SetDepth_RaisesPropertyChanged_IfValueIsChanged()
        {
            _cuboid.Depth = new Distance(4);

            Assert.AreEqual(2, _changedProperties.Count);
            Assert.AreEqual(nameof(_cuboid.Depth), _changedProperties[0]);
            Assert.AreEqual(nameof(_cuboid.Geometry), _changedProperties[1]);
        }

        #endregion
    }
}
