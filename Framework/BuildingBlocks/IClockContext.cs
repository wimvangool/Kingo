
namespace Kingo.BuildingBlocks
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
