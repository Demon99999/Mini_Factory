using System.Collections.Generic;

namespace Project.Scripts.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly List<IAnalyticsProvider> _providers;

        public AnalyticsService(IEnumerable<IAnalyticsProvider> providers)
        {
            _providers = new List<IAnalyticsProvider>(providers);
            
            foreach (var p in _providers) p.Initialize();
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            var payload = (IReadOnlyDictionary<string, object>)parameters
                          ?? new Dictionary<string, object>();

            foreach (var p in _providers)
                p.SendEvent(eventName, payload);
        }
    }
}