using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class Shape3DSpy : Shape3D
    {
        private int _generateGeometryCount;        
             
        public void RaisePropertyChanged(string propertyName, bool regenerateGeometry)
        {
            OnPropertyChanged(propertyName, regenerateGeometry);
        }

        public void AssertGenerateGeometryCountIs(int count)
        {
            Assert.AreEqual(count, _generateGeometryCount);
        }

        protected override MeshGeometry3D GenerateGeometry()
        {
            _generateGeometryCount++;

            return new MeshGeometry3D();
        }       
    }
}
