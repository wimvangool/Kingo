
namespace System.ComponentModel.Messaging
{
    internal interface IClockContext
    {
        IClock CurrentClock
        {
            get;
            set;
        }
    }
}
