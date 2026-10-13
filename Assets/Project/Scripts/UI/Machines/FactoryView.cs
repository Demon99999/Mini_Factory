using Project.Scripts.Gameplay.Machines;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Machines
{
    public class FactoryView : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private MachineView _machineViewPrefab;

        private IMachineService _machines;

        [Inject]
        public void Construct(IMachineService machines)
        {
            _machines = machines;
        }

        private void Start()
        {
            foreach (var state in _machines.States)
            {
                var view = Instantiate(_machineViewPrefab, _container);
                view.Construct(_machines, state.Id);
            }
        }
    }
}