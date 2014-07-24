
namespace YellowFlare.MessageProcessing
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
