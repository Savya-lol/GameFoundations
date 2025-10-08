using Darkmatter.Core.Services.DataPresistanceService.Interfaces;
using Darkmatter.Core.Utils;
using UnityEngine;

namespace Darkmatter.Core.Services.DataPresistanceService
{
    public class PlayerPrefsPresistanceService : IDataPresistanceService
    {
        public void SaveData<T>(string key, T data)
        {
            string jsonData = JsonUtility.ToJson(data);
            jsonData = EncryptionUtils.Encrypt(jsonData);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }

        public T LoadData<T>(string key, T defaultValue = default)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string jsonData = UnityEngine.PlayerPrefs.GetString(key);
                jsonData = EncryptionUtils.Decrypt(jsonData);       
                return JsonUtility.FromJson<T>(jsonData);
            }
            return defaultValue;
        }
    }
}