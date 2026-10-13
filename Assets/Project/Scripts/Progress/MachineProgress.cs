using System;

namespace Project.Scripts.Progress
{
    [Serializable]
    public class MachineProgress
    {
        public string Id;
        public bool Unlocked;
        public int Level;
    }
}