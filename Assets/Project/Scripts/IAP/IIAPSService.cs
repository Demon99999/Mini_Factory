using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Project.Scripts.IAP
{
    public interface IIAPSService
    {
        bool IsInitialized { get; }
        event Action<string, PurchaseResult> OnPurchaseCompleted;

        UniTask InitializeAsync(CancellationToken ct);
        UniTask<PurchaseResult> PurchaseAsync(string productId, CancellationToken ct);
    }
}