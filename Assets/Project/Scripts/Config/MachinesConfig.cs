using UnityEngine;

namespace Project.Scripts.Config
{
    [CreateAssetMenu(menuName = "MiniFactory/Machines Config", fileName = "MachinesConfig")]
    public class MachinesConfig : ScriptableObject
    {
        [SerializeField] private MachineConfig[] _machines;
        
        public MachineConfig[] Machines => _machines;
    }
}