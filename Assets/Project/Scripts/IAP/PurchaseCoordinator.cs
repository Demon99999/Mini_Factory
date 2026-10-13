using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Scripts.Analytics;
using Project.Scripts.Gameplay.Economy;
using Zenject;

namespace Project.Scripts.IAP
{
    public class PurchaseCoordinator : IInitializable, IDisposable
    {
        private readonly IIAPSService _iiaps;
        private readonly IEconomyService _economy;
        private readonly IAnalyticsService _analytics;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // Prevents double-granting the same product within a session
        private readonly HashSet<string> _grantedTransactions = new HashSet<string>();

        public PurchaseCoordinator(IIAPSService iiaps, IEconomyService economy, IAnalyticsService analytics)
        {
            _iiaps = iiaps;
            _economy = economy;
            _analytics = analytics;
        }

        public void Initialize()
        {
            _iiaps.OnPurchaseCompleted += OnPurchaseCompleted;
            InitializeIapAsync(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _iiaps.OnPurchaseCompleted -= OnPurchaseCompleted;
            _cts.Cancel();
            _cts.Dispose();
        }

        public async UniTask<PurchaseResult> BuyCoinsPackAsync()
        {
            return await _iiaps.PurchaseAsync(IapConstants.CoinsPackSmall, _cts.Token);
        }

        private async UniTaskVoid InitializeIapAsync(CancellationToken ct)
        {
            await _iiaps.InitializeAsync(ct);
        }

        private void OnPurchaseCompleted(string productId, PurchaseResult result)
        {
            if (productId != IapConstants.CoinsPackSmall) return;

            if (result == PurchaseResult.Success)
            {
                _economy.Add(IapConstants.CoinsPackSmallAmount);
            }

            var evt = result switch
            {
                PurchaseResult.Success => AnalyticsEvents.PurchaseSucceeded,
                PurchaseResult.Failed => AnalyticsEvents.PurchaseFailed,
                PurchaseResult.Cancelled => AnalyticsEvents.PurchaseFailed,
                PurchaseResult.Deferred => AnalyticsEvents.PurchaseFailed,
                _ => AnalyticsEvents.PurchaseFailed
            };

            _analytics.TrackEvent(evt, new Dictionary<string, object>
            {
                { "product_id", productId },
                { "result", result.ToString() }
            });
        }
    }
}