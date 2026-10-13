using UnityEngine;

namespace Project.Scripts.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        TAsset Load<TAsset>(string path) where TAsset : Object;
        TAsset[] LoadAll<TAsset>(string folderPath) where TAsset : Object;
    }
}
