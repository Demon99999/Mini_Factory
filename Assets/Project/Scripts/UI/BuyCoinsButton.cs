using Project.Scripts.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI
{
    public class BuyCoinsButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _labelFormat = "BUY {0}";

        private PurchaseCoordinator _coordinator;

        [Inject]
        public void Construct(PurchaseCoordinator coordinator)
        {
            _coordinator = coordinator;
        }

        private void Start()
        {
            _label.text = string.Format(_labelFormat, IapConstants.CoinsPackSmallAmount);
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            BuyAsync().Forget();
        }

        private async Cysharp.Threading.Tasks.UniTaskVoid BuyAsync()
        {
            var result = await _coordinator.BuyCoinsPackAsync();
            Debug.Log($"[IAP] Purchase result: {result}");
        }
    }
}