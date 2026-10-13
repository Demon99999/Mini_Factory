using Newtonsoft.Json;
using UnityEngine;

namespace Project.Scripts.Progress
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string Key = "MiniFactory.Progress";

        private readonly IPersistentProgress _progress;

        public SaveLoadService(IPersistentProgress progress)
        {
            _progress = progress;
        }

        public PlayerProgress LoadProgress()
        {
            var json = PlayerPrefs.GetString(Key, string.Empty);
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                return JsonConvert.DeserializeObject<PlayerProgress>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoad] Load failed: {e}");
                return null;
            }
        }

        public void SaveProgress()
        {
            try
            {
                PlayerPrefs.SetString(Key, JsonConvert.SerializeObject(_progress.Progress));
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoad] Save failed: {e}");
            }
        }
    }
}