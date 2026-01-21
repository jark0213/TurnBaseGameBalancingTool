using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TurnBasedSimTool.Core;
using Newtonsoft.Json;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 팀 데이터 저장용 래퍼 클래스
    /// </summary>
    [System.Serializable]
    public class TeamData
    {
        public List<IBattleUnit> Units;
        public Dictionary<int, List<IBattleAction>> Actions; // 유닛 인덱스 -> 액션 리스트
    }

    /// <summary>
    /// 캐릭터 데이터 저장/불러오기 관리
    /// Newtonsoft.Json을 사용하여 복잡한 타입도 자동 직렬화
    ///
    /// 사용법:
    /// 1. 캐릭터 클래스에 [System.Serializable] 속성 추가
    /// 2. 저장하고 싶은 필드를 public으로 선언
    /// 3. ScriptableObject나 Unity Object 참조는 ID로 변환하여 저장
    ///
    /// 예시:
    /// <code>
    /// [System.Serializable]
    /// public class DungeonDiceUnit : DefaultUnit
    /// {
    ///     public List&lt;int&gt; diceIds;      // 주사위 ID들
    ///     public List&lt;int&gt; abilityIds;   // 능력 ID들
    /// }
    ///
    /// // 저장
    /// CharacterSaveManager.SaveTeam("PlayerTeam", playerUnits);
    ///
    /// // 불러오기
    /// var units = CharacterSaveManager.LoadTeam("PlayerTeam");
    /// </code>
    /// </summary>
    public static class CharacterSaveManager
    {
        private const string SAVE_FOLDER = "Teams";

        /// <summary>
        /// 팀 데이터 폴더 경로
        /// </summary>
        private static string SaveFolderPath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// JSON 직렬화 설정
        /// TypeNameHandling.Auto: 인터페이스/추상 클래스의 실제 타입 정보를 자동으로 포함
        /// </summary>
        private static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        /// <summary>
        /// 팀 데이터 저장 (유닛만, 호환성용)
        /// </summary>
        /// <param name="teamName">팀 이름 (파일명으로 사용됨)</param>
        /// <param name="units">저장할 유닛 리스트</param>
        public static void SaveTeam(string teamName, List<IBattleUnit> units)
        {
            SaveTeamWithActions(teamName, units, null);
        }

        /// <summary>
        /// 팀 데이터 저장 (유닛 + 액션)
        /// </summary>
        /// <param name="teamName">팀 이름 (파일명으로 사용됨)</param>
        /// <param name="units">저장할 유닛 리스트</param>
        /// <param name="actionMap">유닛별 액션 맵 (선택 사항)</param>
        public static void SaveTeamWithActions(string teamName, List<IBattleUnit> units, Dictionary<IBattleUnit, List<IBattleAction>> actionMap)
        {
            try
            {
                string fileName = $"{teamName}.json";
                string filePath = Path.Combine(SaveFolderPath, fileName);

                // 액션 맵을 인덱스 기반으로 변환
                var teamData = new TeamData
                {
                    Units = units,
                    Actions = new Dictionary<int, List<IBattleAction>>()
                };

                if (actionMap != null)
                {
                    for (int i = 0; i < units.Count; i++)
                    {
                        if (actionMap.ContainsKey(units[i]))
                        {
                            teamData.Actions[i] = actionMap[units[i]];
                        }
                    }
                }

                string json = JsonConvert.SerializeObject(teamData, SerializerSettings);
                File.WriteAllText(filePath, json);

                Debug.Log($"[CharacterSaveManager] Team '{teamName}' saved to: {filePath} ({units.Count} units)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CharacterSaveManager] Failed to save team '{teamName}': {e.Message}");
            }
        }

        /// <summary>
        /// 팀 데이터 불러오기 (유닛만, 호환성용)
        /// </summary>
        /// <param name="teamName">팀 이름</param>
        /// <returns>저장된 유닛 리스트, 실패 시 null</returns>
        public static List<IBattleUnit> LoadTeam(string teamName)
        {
            var teamData = LoadTeamWithActions(teamName);
            return teamData?.Units;
        }

        /// <summary>
        /// 팀 데이터 불러오기 (유닛 + 액션)
        /// </summary>
        /// <param name="teamName">팀 이름</param>
        /// <returns>저장된 팀 데이터, 실패 시 null</returns>
        public static TeamData LoadTeamWithActions(string teamName)
        {
            try
            {
                string fileName = $"{teamName}.json";
                string filePath = Path.Combine(SaveFolderPath, fileName);

                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[CharacterSaveManager] Team file not found: {fileName}");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                var teamData = JsonConvert.DeserializeObject<TeamData>(json, SerializerSettings);

                Debug.Log($"[CharacterSaveManager] Team '{teamName}' loaded from: {filePath}");
                return teamData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CharacterSaveManager] Failed to load team '{teamName}': {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 저장된 팀이 있는지 확인
        /// </summary>
        public static bool HasSavedTeam(string teamName)
        {
            string fileName = $"{teamName}.json";
            string filePath = Path.Combine(SaveFolderPath, fileName);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 저장된 팀 삭제
        /// </summary>
        public static void DeleteTeam(string teamName)
        {
            try
            {
                string fileName = $"{teamName}.json";
                string filePath = Path.Combine(SaveFolderPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"[CharacterSaveManager] Team '{teamName}' deleted.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CharacterSaveManager] Failed to delete team '{teamName}': {e.Message}");
            }
        }

        /// <summary>
        /// 저장된 모든 팀 이름 목록 가져오기
        /// </summary>
        public static List<string> GetSavedTeamNames()
        {
            try
            {
                var teamNames = new List<string>();
                var files = Directory.GetFiles(SaveFolderPath, "*.json");

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    teamNames.Add(fileName);
                }

                return teamNames;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CharacterSaveManager] Failed to get team list: {e.Message}");
                return new List<string>();
            }
        }
    }
}
