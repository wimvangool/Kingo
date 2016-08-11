using System.Security.Permissions;
using System.Windows.Threading;

namespace Kingo.Windows
{
    internal static class DispatcherOperations
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void ProcessMessageQueue()
        {
            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public static object ExitFrame(object frame)
        {
            var dispatcherFrame = frame as DispatcherFrame;
            if (dispatcherFrame != null)
            {
                dispatcherFrame.Continue = false;
            }
            return null;
        }
    }
}
