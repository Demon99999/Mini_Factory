using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, Object> _loadedAssets = new Dictionary<string, Object>();

        public TAsset Load<TAsset>(string path) where TAsset : Object
        {
            if (!_loadedAssets.TryGetValue(path, out var asset))
            {
                asset = Resources.Load<TAsset>(path);
                _loadedAssets[path] = asset;
            }
            
            return asset as TAsset;
        }

        public TAsset[] LoadAll<TAsset>(string folderPath) where TAsset : Object
        {
            var assets = Resources.LoadAll<TAsset>(folderPath);
            
            foreach (var asset in assets)
                _loadedAssets[asset.name] = asset;
            
            return assets;
        }
    }
}
