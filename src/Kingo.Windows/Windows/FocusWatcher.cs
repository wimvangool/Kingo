using System;

namespace Kingo.Windows
{
    internal abstract class FocusWatcher : Disposable
    {
        #region [====== LostFocus ======]

        public event EventHandler LostFocus;

        protected virtual void OnLostFocus()
        {
            LostFocus.Raise(this);
        }

        #endregion

        #region [====== GotFocus ======]

        public event EventHandler GotFocus;

        protected virtual void OnGotFocus()
        {
            GotFocus.Raise(this);
        }

        #endregion
    }
}
