using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Project.Scripts.IAP
{
    public class UnityAPSService : IIAPSService
    {
        private StoreController _storeController;
        private List<ProductDefinition> _productDefinitions;
        
        private readonly Dictionary<string, UniTaskCompletionSource<PurchaseResult>> _purchaseTcsMap = new Dictionary<string, UniTaskCompletionSource<PurchaseResult>>();
        
        private readonly HashSet<string> _processedTransactionIds = new HashSet<string>();

        public bool IsInitialized { get; private set; }
        public event Action<string, PurchaseResult> OnPurchaseCompleted;

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            _storeController = UnityIAPServices.StoreController();
            
            SubscribeToEvents();
            
            var connectTcs = new UniTaskCompletionSource<bool>();

            void OnConnected() => connectTcs.TrySetResult(true);
            void OnDisconnected(StoreConnectionFailureDescription failure) => connectTcs.TrySetResult(false);

            _storeController.OnStoreConnected += OnConnected;
            _storeController.OnStoreDisconnected += OnDisconnected;

            _storeController.Connect();

            try
            {
                var connected = await connectTcs.Task.AttachExternalCancellation(ct);

                _storeController.OnStoreConnected -= OnConnected;
                _storeController.OnStoreDisconnected -= OnDisconnected;

                if (!connected)
                {
                    Debug.LogError("[IAP] Failed to connect to store");
                    IsInitialized = false;
                    return;
                }
                
                _productDefinitions = new List<ProductDefinition>
                {
                    new ProductDefinition(IapConstants.CoinsPackSmall, ProductType.Consumable)
                };
                
                var productsTcs = new UniTaskCompletionSource<bool>();

                void OnProductsFetched(List<Product> products) => productsTcs.TrySetResult(true);
                void OnProductsFetchFailed(ProductFetchFailed failure) => productsTcs.TrySetResult(false);

                _storeController.OnProductsFetched += OnProductsFetched;
                _storeController.OnProductsFetchFailed += OnProductsFetchFailed;

                _storeController.FetchProducts(_productDefinitions);

                var productsFetched = await productsTcs.Task.AttachExternalCancellation(ct);

                _storeController.OnProductsFetched -= OnProductsFetched;
                _storeController.OnProductsFetchFailed -= OnProductsFetchFailed;

                IsInitialized = productsFetched;
                
                if (IsInitialized)
                {
                    _storeController.FetchPurchases();
                }

                Debug.Log($"[IAP] Initialized: {IsInitialized}");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[IAP] Initialization cancelled");
                IsInitialized = false;
            }
        }

        public async UniTask<PurchaseResult> PurchaseAsync(string productId, CancellationToken ct)
        {
            if (!IsInitialized) return PurchaseResult.NotInitialized;
            
            var product = _storeController.GetProductById(productId);
            
            if (product == null)
                return PurchaseResult.ProductNotFound;
            
            var tcs = new UniTaskCompletionSource<PurchaseResult>();
            _purchaseTcsMap[productId] = tcs;
            
            _storeController.PurchaseProduct(product);

            try
            {
                return await tcs.Task.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                _purchaseTcsMap.Remove(productId);
                return PurchaseResult.Cancelled;
            }
        }

        private void SubscribeToEvents()
        {
            _storeController.OnPurchasePending += OnPurchasePending;
            _storeController.OnPurchaseFailed += OnPurchaseFailed;
            _storeController.OnPurchaseDeferred += OnPurchaseDeferred;
            _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            
            _storeController.OnPurchasesFetched += OnPurchasesFetched;
            _storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        }

        private void OnPurchasePending(PendingOrder order)
        {
            var product = order.CartOrdered.Items().FirstOrDefault()?.Product;
            if (product == null) return;

            var productId = product.definition.id;
            var transactionId = order.Info?.TransactionID;
            
            if (!string.IsNullOrEmpty(transactionId) && _processedTransactionIds.Contains(transactionId))
            {
                Debug.Log($"[IAP] Duplicate transaction skipped: {transactionId}");
                _storeController.ConfirmPurchase(order);
                return;
            }
            
            if (_purchaseTcsMap.TryGetValue(productId, out var tcs))
            {
                tcs.TrySetResult(PurchaseResult.Success);
                _purchaseTcsMap.Remove(productId);
            }
            
            if (!string.IsNullOrEmpty(transactionId))
                _processedTransactionIds.Add(transactionId);
            
            OnPurchaseCompleted?.Invoke(productId, PurchaseResult.Success);
            
            _storeController.ConfirmPurchase(order);
        }

        private void OnPurchaseFailed(FailedOrder failedOrder)
        {
            var product = failedOrder.CartOrdered.Items().FirstOrDefault()?.Product;
            var productId = product?.definition.id ?? "unknown";
            
            if (_purchaseTcsMap.TryGetValue(productId, out var tcs))
            {
                tcs.TrySetResult(PurchaseResult.Failed);
                _purchaseTcsMap.Remove(productId);
            }

            Debug.LogError($"[IAP] Purchase failed: {productId} - {failedOrder.FailureReason}");
            OnPurchaseCompleted?.Invoke(productId, PurchaseResult.Failed);
        }

        private void OnPurchaseDeferred(DeferredOrder order)
        {
            var product = order.CartOrdered.Items().FirstOrDefault()?.Product;
            var productId = product?.definition.id ?? "unknown";

            Debug.Log($"[IAP] Purchase deferred: {productId}");
            OnPurchaseCompleted?.Invoke(productId, PurchaseResult.Deferred);
        }

        private void OnPurchaseConfirmed(Order order)
        {
            switch (order)
            {
                case ConfirmedOrder confirmedOrder:
                    Debug.Log($"[IAP] Purchase confirmed: {confirmedOrder.Info?.TransactionID}");
                    break;
                case FailedOrder failedOrder:
                    Debug.LogError($"[IAP] Confirmation failed: {failedOrder.FailureReason} - {failedOrder.Details}");
                    break;
            }
        }

        private void OnPurchasesFetched(Orders orders)
        {
            if (orders.PendingOrders.Count > 0)
            {
                Debug.Log($"[IAP] Found {orders.PendingOrders.Count} pending orders");
                foreach (var order in orders.PendingOrders)
                {
                    var product = order.CartOrdered.Items().FirstOrDefault()?.Product;
                    if (product != null)
                    {
                        OnPurchasePending(order);
                    }
                }
            }
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            Debug.LogError($"[IAP] Failed to fetch purchases: {failure.Message}");
        }
    }
}