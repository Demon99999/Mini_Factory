using UnityEngine;

namespace Project.Scripts.Config
{
    [CreateAssetMenu(menuName = "MiniFactory/Boost Settings", fileName = "BoostSettings")]
    public class BoostSettingsConfig : ScriptableObject
    {
        [SerializeField] private BoostConfig _boost = new BoostConfig();
        
        public BoostConfig Boost => _boost;
    }
}