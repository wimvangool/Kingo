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
    /// Represents a cube.
    /// </summary>
    public sealed class Cube3D : Shape3DDecorator<Cuboid3D>
    {
        #region [====== Size ======]        

        /// <summary>
        /// Gets or sets the size of the cube.
        /// </summary>
        public Distance Size
        {
            get { return Shape.Width; }
            set
            {
                if (Size == value)
                {
                    return;
                }
                using (ChangeProperty(nameof(Size)))
                {
                    Shape.Width = value;
                    Shape.Height = value;
                    Shape.Depth = value;
                }
            }
        }

        #endregion

        /// <inheritdoc />
        protected override Cuboid3D CreateShape()
        {
            return new Cuboid3D();
        }
    }
}
