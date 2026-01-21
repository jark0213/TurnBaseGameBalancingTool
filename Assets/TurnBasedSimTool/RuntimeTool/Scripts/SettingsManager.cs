using System.IO;
using UnityEngine;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 시뮬레이션 설정 저장/불러오기 관리
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        private const string SETTINGS_FILE_NAME = "last_settings.json";

        /// <summary>
        /// 설정 파일 경로
        /// </summary>
        private static string SettingsFilePath
        {
            get
            {
                string directory = Application.persistentDataPath;
                return Path.Combine(directory, SETTINGS_FILE_NAME);
            }
        }

        /// <summary>
        /// 설정 저장
        /// </summary>
        public static void SaveSettings(SimulationSettings settings)
        {
            try
            {
                string json = settings.ToJson();
                File.WriteAllText(SettingsFilePath, json);
                Debug.Log($"[SettingsManager] Settings saved to: {SettingsFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SettingsManager] Failed to save settings: {e.Message}");
            }
        }

        /// <summary>
        /// 설정 불러오기
        /// </summary>
        public static SimulationSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    SimulationSettings settings = SimulationSettings.FromJson(json);
                    Debug.Log($"[SettingsManager] Settings loaded from: {SettingsFilePath}");
                    return settings;
                }
                else
                {
                    Debug.Log("[SettingsManager] No saved settings found. Using defaults.");
                    return null;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SettingsManager] Failed to load settings: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 저장된 설정 파일이 있는지 확인
        /// </summary>
        public static bool HasSavedSettings()
        {
            return File.Exists(SettingsFilePath);
        }

        /// <summary>
        /// 저장된 설정 삭제
        /// </summary>
        public static void DeleteSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    File.Delete(SettingsFilePath);
                    Debug.Log("[SettingsManager] Settings deleted.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SettingsManager] Failed to delete settings: {e.Message}");
            }
        }
    }
}
