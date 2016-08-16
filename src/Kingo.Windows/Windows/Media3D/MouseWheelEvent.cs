using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents an action of the mousewheel.
    /// </summary>
    [Flags]
    public enum MouseWheelEvents
    {
        /// <summary>
        /// Represents the absence of a mouse wheel event.
        /// </summary>
        None,

        /// <summary>
        /// Represents one rotation of the wheel away from the user.
        /// </summary>
        ScrollUp,

        /// <summary>
        /// Represents one rotation of the wheel towards the user.
        /// </summary>
        ScrollDown
    }
}
