using System.Collections.Generic;

namespace granny.classy
{
    public interface IRepository
    {
        IEnumerable<int> GetMatchingOrderLineIds(int physicalZoneId, int accessDeviceId);
    }
}