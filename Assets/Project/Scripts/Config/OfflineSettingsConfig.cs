using UnityEngine;

namespace Project.Scripts.Config
{
    [CreateAssetMenu(menuName = "MiniFactory/Offline Settings", fileName = "OfflineSettings")]
    public class OfflineSettingsConfig : ScriptableObject
    {
        [SerializeField] private OfflineConfig _offline = new OfflineConfig();
        
        public OfflineConfig Offline => _offline;
    }
}