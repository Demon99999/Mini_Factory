using Project.Scripts.Gameplay.Boost;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Gameplay.Machines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI
{
    public class TopBarView : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _productionText;

        [Header("Boost")]
        [SerializeField] private Button _boostButton;
        [SerializeField] private TMP_Text _boostStatusText;

        private IEconomyService _economy;
        private IMachineService _machines;
        private IBoostService _boost;

        private float _pollTimer;
        private const float PollInterval = 0.1f;

        [Inject]
        public void Construct(IEconomyService economy, IMachineService machines, IBoostService boost)
        {
            _economy = economy;
            _machines = machines;
            _boost = boost;
        }

        private void Start()
        {
            _boostButton.onClick.AddListener(OnBoostClicked);
            _boost.OnBoostStarted += Refresh;
            _boost.OnBoostFinished += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (_boost == null) return;
            _boost.OnBoostStarted -= Refresh;
            _boost.OnBoostFinished -= Refresh;
        }

        private void Update()
        {
            _pollTimer += Time.unscaledDeltaTime;
            if (_pollTimer < PollInterval) return;
            _pollTimer = 0f;

            _balanceText.text = $"Balance: {FormatNumber(_economy.Balance)}";
            _productionText.text = $"Production: {FormatNumber(_machines.GetTotalProduction() * _boost.Multiplier)}/s";

            _boostStatusText.text = _boost.IsActive
                ? $"Active: {_boost.RemainingSeconds:0.0}s"
                : "Ready";
        }

        private void OnBoostClicked() => _boost.TryActivate();

        private void Refresh()
        {
            _boostButton.interactable = !_boost.IsActive;
        }

        private static string FormatNumber(double value)
        {
            if (value >= 1_000_000_000) return (value / 1_000_000_000d).ToString("0.##") + "B";
            if (value >= 1_000_000) return (value / 1_000_000d).ToString("0.##") + "M";
            if (value >= 1_000) return (value / 1_000d).ToString("0.##") + "K";
            return value.ToString("0.##");
        }
    }
}