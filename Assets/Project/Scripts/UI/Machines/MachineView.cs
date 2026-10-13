using System.Linq;
using Project.Scripts.Gameplay.Machines;
using Project.Scripts.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Machines
{
    public class MachineView : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _productionText;

        [Header("Unlock")]
        [SerializeField] private Button _unlockButton;
        [SerializeField] private TMP_Text _unlockCostText;

        [Header("Upgrade")]
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _upgradeCostText;

        private IMachineService _machines;
        private MachineProgress _state;
        private string _machineId;
        private float _pollTimer;
        private const float PollInterval = 0.1f;

        public void Construct(IMachineService machines, string machineId)
        {
            _machines = machines;
            _machineId = machineId;
            _state = machines.States.FirstOrDefault(s => s.Id == machineId);

            _unlockButton.onClick.AddListener(OnUnlockClicked);
            _upgradeButton.onClick.AddListener(OnUpgradeClicked);

            _machines.OnStateChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (_machines != null)
                _machines.OnStateChanged -= Refresh;
        }

        private void Update()
        {
            if (_state == null) return;

            _pollTimer += Time.unscaledDeltaTime;
            if (_pollTimer < PollInterval) return;
            _pollTimer = 0f;

            if (!_state.Unlocked)
                _unlockButton.interactable = _machines.CanUnlock(_machineId);
            else
                _upgradeButton.interactable = _machines.CanUpgrade(_machineId);
        }

        private void OnUnlockClicked() => _machines.TryUnlock(_machineId);
        private void OnUpgradeClicked() => _machines.TryUpgrade(_machineId);

        private void Refresh()
        {
            var config = _machines.GetConfig(_machineId);
            if (config == null) return;

            _nameText.text = config.DisplayName;

            if (!_state.Unlocked)
            {
                _statusText.text = "Locked";
                _statusText.color = new Color(1f, 0.5f, 0.5f);
                _levelText.text = "—";
                _productionText.text = "—";

                _unlockButton.gameObject.SetActive(true);
                _upgradeButton.gameObject.SetActive(false);

                _unlockCostText.text = $"Unlock {FormatNumber(_machines.GetUnlockCost(_machineId))}";
            }
            else
            {
                _statusText.text = "Unlocked";
                _statusText.color = new Color(0.6f, 1f, 0.6f);
                _levelText.text = $"Lvl {_state.Level}";
                _productionText.text = $"Produces {FormatNumber(_machines.GetProductionPerSecond(_state))}/s";

                _unlockButton.gameObject.SetActive(false);
                _upgradeButton.gameObject.SetActive(true);

                _upgradeCostText.text = $"Upgrade {FormatNumber(_machines.GetUpgradeCost(_state))}";
            }
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