using System;

namespace Project.Scripts.Config
{
    [Serializable]
    public class BoostConfig
    {
        public bool Enabled = true;
        public float DurationSeconds = 30f;
        public float Multiplier = 2f;
    }
}