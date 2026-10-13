using System;
using System.Collections.Generic;
using Project.Scripts.Analytics;
using Project.Scripts.Gameplay.Boost;
using Project.Scripts.Gameplay.Economy;
using Project.Scripts.Gameplay.Loop;
using Project.Scripts.Gameplay.Machines;
using Project.Scripts.Gameplay.Offline;
using Project.Scripts.Progress;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Bootstrappers
{
    public class GameplayBootstrapper : IInitializable, ITickable, IDisposable
    {
        private const float SaveInterval = 5f;

        private readonly IEconomyService _economy;
        private readonly IMachineService _machines;
        private readonly IBoostService _boost;
        private readonly IOfflineProgressService _offline;
        private readonly ISaveLoadService _saveLoad;
        private readonly IPersistentProgress _progress;
        private readonly IAnalyticsService _analytics;
        private readonly IGameLoopService _loop;

        private float _saveTimer;

        public GameplayBootstrapper(
            IEconomyService economy,
            IMachineService machines,
            IBoostService boost,
            IOfflineProgressService offline,
            ISaveLoadService saveLoad,
            IPersistentProgress progress,
            IAnalyticsService analytics,
            IGameLoopService loop)
        {
            _economy = economy;
            _machines = machines;
            _boost = boost;
            _offline = offline;
            _saveLoad = saveLoad;
            _progress = progress;
            _analytics = analytics;
            _loop = loop;
        }

        public void Initialize()
        {
            var now = DateTime.UtcNow.Ticks;

            _boost.RestoreFromProgress(now);

            var result = _offline.ApplyOffline(
                _progress.Progress.LastExitUtcTicks,
                now,
                _machines.GetTotalProduction());

            if (result.Amount > 0)
            {
                _economy.Add(result.Amount);
                
                _analytics.TrackEvent(AnalyticsEvents.OfflineIncomeApplied, new Dictionary<string, object>
                {
                    { "seconds", result.Seconds },
                    { "amount", result.Amount }
                });
            }

            _progress.Progress.LastExitUtcTicks = now;

            _machines.OnMachineUnlocked += OnMachineUnlocked;
            _machines.OnMachineUpgraded += OnMachineUpgraded;
            _boost.OnBoostStarted += OnBoostStarted;
            _boost.OnBoostFinished += OnBoostFinished;

            Application.focusChanged += OnFocusChanged;

            _analytics.TrackEvent(AnalyticsEvents.GameStarted);
        }

        public void Tick()
        {
            _loop.Tick(Time.deltaTime);
            _saveTimer += Time.unscaledDeltaTime;
            
            if (_saveTimer >= SaveInterval)
            {
                _saveTimer = 0f;
                SaveNow();
            }
        }

        public void Dispose()
        {
            _machines.OnMachineUnlocked -= OnMachineUnlocked;
            _machines.OnMachineUpgraded -= OnMachineUpgraded;
            _boost.OnBoostStarted -= OnBoostStarted;
            _boost.OnBoostFinished -= OnBoostFinished;
            Application.focusChanged -= OnFocusChanged;

            SaveNow();
        }

        private void OnMachineUnlocked(MachineProgress state)
        {
            _analytics.TrackEvent(AnalyticsEvents.MachineUnlocked, new Dictionary<string, object>
            {
                { "id", state.Id },
                { "cost", _machines.GetUnlockCost(state.Id) },
                { "balance", _economy.Balance }
            });
        }

        private void OnMachineUpgraded(MachineProgress state)
        {
            _analytics.TrackEvent(AnalyticsEvents.MachineUpgraded, new Dictionary<string, object>
            {
                { "id", state.Id },
                { "level", state.Level },
                { "balance", _economy.Balance }
            });
        }

        private void OnBoostStarted()
            => _analytics.TrackEvent(AnalyticsEvents.BoostStarted);

        private void OnBoostFinished()
            => _analytics.TrackEvent(AnalyticsEvents.BoostFinished);

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveNow();
                return;
            }

            var now = DateTime.UtcNow.Ticks;
            var result = _offline.ApplyOffline(
                _progress.Progress.LastExitUtcTicks,
                now,
                _machines.GetTotalProduction());

            if (result.Amount > 0)
            {
                _economy.Add(result.Amount);
                
                _analytics.TrackEvent(AnalyticsEvents.OfflineIncomeApplied, new Dictionary<string, object>
                {
                    { "seconds", result.Seconds },
                    { "amount", result.Amount }
                });
            }

            _progress.Progress.LastExitUtcTicks = now;
        }

        private void SaveNow()
        {
            _progress.Progress.LastExitUtcTicks = DateTime.UtcNow.Ticks;
            _saveLoad.SaveProgress();
        }
    }
}