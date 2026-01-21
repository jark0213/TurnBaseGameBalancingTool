using System.Collections.Generic;
using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// CharacterSaveManager 테스트 스크립트
    /// Inspector에서 버튼으로 저장/불러오기 테스트 가능
    /// </summary>
    public class CharacterSaveTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private string testTeamName = "TestTeam";

        [ContextMenu("Save Test Team")]
        public void SaveTestTeam()
        {
            // 테스트 유닛 생성
            var units = new List<IBattleUnit>
            {
                new DefaultUnit
                {
                    Name = "Knight",
                    MaxHp = 100,
                    CurrentHp = 100,
                    Defense = 10,
                    Speed = 8,
                    Evasion = 5
                },
                new DefaultUnit
                {
                    Name = "Archer",
                    MaxHp = 80,
                    CurrentHp = 80,
                    Defense = 5,
                    Speed = 12,
                    CritRate = 20
                },
                new DefaultUnit
                {
                    Name = "Mage",
                    MaxHp = 60,
                    CurrentHp = 60,
                    Defense = 3,
                    Speed = 10,
                    CritRate = 15,
                    CritMultiplier = 200
                }
            };

            CharacterSaveManager.SaveTeam(testTeamName, units);
            Debug.Log($"[Test] Saved {units.Count} units to '{testTeamName}'");
        }

        [ContextMenu("Load Test Team")]
        public void LoadTestTeam()
        {
            var units = CharacterSaveManager.LoadTeam(testTeamName);

            if (units == null)
            {
                Debug.LogWarning($"[Test] Failed to load '{testTeamName}'");
                return;
            }

            Debug.Log($"[Test] Loaded {units.Count} units from '{testTeamName}':");
            foreach (var unit in units)
            {
                Debug.Log($"  - {unit.Name}: HP={unit.CurrentHp}/{unit.MaxHp}");

                if (unit is DefaultUnit defaultUnit)
                {
                    Debug.Log($"    Defense={defaultUnit.Defense}, Speed={defaultUnit.Speed}, Evasion={defaultUnit.Evasion}");
                }
            }
        }

        [ContextMenu("Check Saved Teams")]
        public void CheckSavedTeams()
        {
            var teamNames = CharacterSaveManager.GetSavedTeamNames();
            Debug.Log($"[Test] Found {teamNames.Count} saved teams:");
            foreach (var name in teamNames)
            {
                Debug.Log($"  - {name}");
            }
        }

        [ContextMenu("Delete Test Team")]
        public void DeleteTestTeam()
        {
            CharacterSaveManager.DeleteTeam(testTeamName);
            Debug.Log($"[Test] Deleted '{testTeamName}'");
        }
    }
}
