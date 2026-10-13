using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Project.Scripts.Analytics
{
    public class ConsoleAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize()
        {
            Debug.Log("[Analytics] Console provider initialized");
        }

        public void SendEvent(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            var sb = new StringBuilder("[Analytics] ").Append(eventName);

            if (parameters != null && parameters.Count > 0)
            {
                sb.Append(" { ");
                
                foreach (var kv in parameters)
                    sb.Append(kv.Key).Append('=').Append(kv.Value).Append(' ');
                
                sb.Append('}');
            }

            Debug.Log(sb.ToString());
        }
    }
}