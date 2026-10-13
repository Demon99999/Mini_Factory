using Project.Scripts.Gameplay.Boost;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Gameplay.Machines;

namespace Project.Scripts.Gameplay.Loop
{
    public class GameLoopService : IGameLoopService
    {
        private readonly IMachineService _machines;
        private readonly IEconomyService _economy;
        private readonly IBoostService _boost;

        public GameLoopService(IMachineService machines, IEconomyService economy, IBoostService boost)
        {
            _machines = machines;
            _economy = economy;
            _boost = boost;
        }

        public void Tick(float deltaTime)
        {
            _boost.Tick(deltaTime);
            var production = _machines.GetTotalProduction() * _boost.Multiplier * deltaTime;
            
            if (production > 0) _economy.Add(production);
        }
    }
}