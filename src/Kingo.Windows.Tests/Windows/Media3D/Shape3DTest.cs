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
    public sealed class Shape3DTest
    {
        [TestMethod]
        public void OnPropertyChanged_OnlyRaisesPropertyChangedForSpecifiedProperty_IfPropertyNameIsSpecified_And_RegenerateGeometryIsFalse()
        {
            const string propertyName = "NonGeometryProperty";
            var properties = new List<string>();
            var shape = new Shape3DSpy();

            shape.PropertyChanged += (s, e) => properties.Add(e.PropertyName);
            shape.RaisePropertyChanged(propertyName, false);

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(propertyName, properties[0]);
        }

        [TestMethod]
        public void OnPropertyChanged_OnlyRaisesPropertyChangedForSpecifiedProperty_IfPropertyNameIsSpecified_And_RegenerateGeometryIsTrue()
        {
            const string propertyName = "GeometryProperty";
            var properties = new List<string>();
            var shape = new Shape3DSpy();

            shape.PropertyChanged += (s, e) => properties.Add(e.PropertyName);
            shape.RaisePropertyChanged(propertyName, true);

            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual(propertyName, properties[0]);
            Assert.AreEqual(nameof(shape.Geometry), properties[1]);
        }

        [TestMethod]
        public void OnPropertyChanged_OnlyRaisesPropertyChangedForSpecifiedProperty_IfPropertyNameIsNotSpecified_And_RegenerateGeometryIsFalse()
        {            
            var properties = new List<string>();
            var shape = new Shape3DSpy();

            shape.PropertyChanged += (s, e) => properties.Add(e.PropertyName);
            shape.RaisePropertyChanged(null, false);

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(null, properties[0]);            
        }

        [TestMethod]
        public void OnPropertyChanged_OnlyRaisesPropertyChangedForSpecifiedPropCauseGeerty_IfPropertyNameIsNotSpecified_And_RegenerateGeometryIsTrue()
        {
            var properties = new List<string>();
            var shape = new Shape3DSpy();

            shape.PropertyChanged += (s, e) => properties.Add(e.PropertyName);
            shape.RaisePropertyChanged(null, true);

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(null, properties[0]);
        }

        [TestMethod]
        public void OnPropertyChanged_DoesNotCauseGeometryToBeRegenerated_IfRegenerateGeometryIsFalse()
        {
            var geometries = new List<Geometry3D>();
            var shape = new Shape3DSpy();
            
            geometries.Add(shape.Geometry);
            shape.RaisePropertyChanged(null, false);
            geometries.Add(shape.Geometry);

            shape.AssertGenerateGeometryCountIs(1);
            
            Assert.AreSame(geometries[0], geometries[1]);       
        }

        [TestMethod]
        public void OnPropertyChanged_CausesGeometryToBeRegenerated_IfRegenerateGeometryIsTrue()
        {
            var geometries = new List<Geometry3D>();
            var shape = new Shape3DSpy();

            geometries.Add(shape.Geometry);
            shape.RaisePropertyChanged(null, true);
            geometries.Add(shape.Geometry);

            shape.AssertGenerateGeometryCountIs(2);

            Assert.AreNotSame(geometries[0], geometries[1]);
        }
    }
}
