using System;

namespace Project.Scripts.Config
{
    [Serializable]
    public class OfflineConfig
    {
        public float MaxOfflineSeconds = 4 * 60 * 60;
    }
}