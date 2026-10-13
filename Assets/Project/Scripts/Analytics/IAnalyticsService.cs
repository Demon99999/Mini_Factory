using System.Collections.Generic;

namespace Project.Scripts.Analytics
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);
    }
}