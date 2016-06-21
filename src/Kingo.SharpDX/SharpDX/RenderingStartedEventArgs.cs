using System;
using System.Windows.Forms;

namespace Kingo.SharpDX
{    
    internal sealed class RenderingStartedEventArgs : EventArgs
    {
        public RenderingStartedEventArgs(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            Control = control;
        }

        public Control Control
        {
            get;
            private set;
        }
    }
}
