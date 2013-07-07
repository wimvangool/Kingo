
namespace YellowFlare.MessageProcessing.Clocks
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
