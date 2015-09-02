
namespace Kingo.BuildingBlocks.Clocks
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
