using System.Collections.Generic;

namespace Project.Scripts.Analytics
{
    public interface IAnalyticsProvider
    {
        void Initialize();
        void SendEvent(string eventName, IReadOnlyDictionary<string, object> parameters);
    }
}