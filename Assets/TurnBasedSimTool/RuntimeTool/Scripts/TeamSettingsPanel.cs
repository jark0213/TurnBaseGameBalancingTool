using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 팀 설정 패널 (여러 캐릭터 관리)
    /// Player Team 또는 Enemy Team을 담당
    /// </summary>
    public class TeamSettingsPanel : MonoBehaviour
    {
        [Header("Team Info")]
        [SerializeField] private string teamName = "Team";

        [Header("Character Management")]
        [SerializeField] private Button addCharacterButton;
        [SerializeField] private Transform characterListContent;
        [SerializeField] private GameObject characterPanelPrefab; // CharacterSettingsPanel Prefab

        [Header("Defeat Condition")]
        [SerializeField] private TMP_Dropdown defeatConditionDropdown;
        [SerializeField] private TMP_Dropdown mainCharacterDropdown; // 주캐 선택
        [SerializeField] private GameObject mainCharacterSelector; // Dropdown 포함한 Parent

        private List<CharacterSettingsPanel> characters = new List<CharacterSettingsPanel>();

        private void Start()
        {
            // Add Character 버튼
            if (addCharacterButton)
            {
                addCharacterButton.onClick.RemoveAllListeners();
                addCharacterButton.onClick.AddListener(AddCharacter);
            }

            // Team Foldout 토글은 FoldoutContentController가 처리
            // teamFoldoutToggle.isOn은 Inspector에서 설정 (기본 true)

            // Defeat Condition Dropdown 초기화
            if (defeatConditionDropdown)
            {
                defeatConditionDropdown.ClearOptions();
                defeatConditionDropdown.AddOptions(new List<string>
                {
                    "All Dead (전멸)",
                    "Main Character Dead (주캐)"
                });
                defeatConditionDropdown.value = 0; // 기본: 전멸
                defeatConditionDropdown.onValueChanged.AddListener(OnDefeatConditionChanged);
            }

            // 초기 상태: Main Character Selector 숨김
            if (mainCharacterSelector != null)
            {
                mainCharacterSelector.SetActive(false);
            }

            // 기본 캐릭터 1개 추가
            if (characters.Count == 0)
            {
                AddCharacter();
            }
        }

        // OnTeamFoldoutToggled 메서드 제거됨 (FoldoutContentController가 처리)

        /// <summary>
        /// Defeat Condition 변경 시 Main Character Selector 표시/숨김
        /// </summary>
        private void OnDefeatConditionChanged(int value)
        {
            if (mainCharacterSelector == null)
                return;

            // 1 = Main Character Dead
            bool showSelector = (value == 1);
            mainCharacterSelector.SetActive(showSelector);

            // 주캐 선택 Dropdown 업데이트
            if (showSelector)
            {
                UpdateMainCharacterDropdown();
            }
        }

        /// <summary>
        /// Main Character Dropdown 옵션 업데이트
        /// </summary>
        private void UpdateMainCharacterDropdown()
        {
            if (mainCharacterDropdown == null)
                return;

            mainCharacterDropdown.ClearOptions();

            List<string> characterNames = new List<string>();
            for (int i = 0; i < characters.Count; i++)
            {
                // CharacterSettingsPanel에서 이름 가져오기 (나중에 public 프로퍼티 추가 필요)
                characterNames.Add($"Character {i + 1}");
            }

            mainCharacterDropdown.AddOptions(characterNames);
            mainCharacterDropdown.value = 0; // 기본: 첫 번째 캐릭터
        }

        /// <summary>
        /// SimulationSettingsPanel과 연결하여 시스템 토글 동기화
        /// </summary>
        public void ConnectToSimulationSettings(SimulationSettingsPanel simPanel)
        {
            if (simPanel != null)
            {
                simPanel.OnCostSystemChanged += (active) =>
                {
                    foreach (var character in characters)
                    {
                        character.ConnectToSimulationSettings(simPanel);
                    }
                };

                simPanel.OnSpeedSystemChanged += (active) =>
                {
                    foreach (var character in characters)
                    {
                        character.ConnectToSimulationSettings(simPanel);
                    }
                };

                // 기존 캐릭터들 연결
                foreach (var character in characters)
                {
                    character.ConnectToSimulationSettings(simPanel);
                }
            }
        }

        /// <summary>
        /// 캐릭터 추가
        /// </summary>
        public void AddCharacter()
        {
            if (characterPanelPrefab == null || characterListContent == null)
            {
                Debug.LogError("CharacterPanelPrefab or CharacterListContent is not assigned!");
                return;
            }

            GameObject characterObj = Instantiate(characterPanelPrefab, characterListContent);
            CharacterSettingsPanel characterPanel = characterObj.GetComponent<CharacterSettingsPanel>();

            if (characterPanel != null)
            {
                characters.Add(characterPanel);

                // Delete 이벤트 연결
                characterPanel.OnDeleteRequested += RemoveCharacter;

                // Main Character Dropdown 업데이트
                if (defeatConditionDropdown != null && defeatConditionDropdown.value == 1)
                {
                    UpdateMainCharacterDropdown();
                }

                Debug.Log($"Character added to {teamName}. Total: {characters.Count}");
            }
            else
            {
                Debug.LogError("CharacterSettingsPanel component not found on prefab!");
                Destroy(characterObj);
            }
        }

        /// <summary>
        /// 캐릭터 제거
        /// </summary>
        private void RemoveCharacter(CharacterSettingsPanel character)
        {
            if (characters.Count <= 1)
            {
                Debug.LogWarning("Cannot delete the last character!");
                return;
            }

            characters.Remove(character);
            Destroy(character.gameObject);

            // Main Character Dropdown 업데이트
            if (defeatConditionDropdown != null && defeatConditionDropdown.value == 1)
            {
                UpdateMainCharacterDropdown();
            }

            Debug.Log($"Character removed from {teamName}. Total: {characters.Count}");
        }

        /// <summary>
        /// 모든 캐릭터 제거
        /// </summary>
        public void ClearAllCharacters()
        {
            foreach (var character in characters)
            {
                if (character != null)
                {
                    Destroy(character.gameObject);
                }
            }

            characters.Clear();
        }

        /// <summary>
        /// 팀 생성 (모든 캐릭터 + 액션)
        /// </summary>
        public List<IBattleUnit> CreateTeam()
        {
            List<IBattleUnit> team = new List<IBattleUnit>();

            foreach (var characterPanel in characters)
            {
                if (characterPanel != null)
                {
                    IBattleUnit unit = characterPanel.CreateCharacter();
                    team.Add(unit);
                }
            }

            return team;
        }

        /// <summary>
        /// 팀의 모든 액션 수집 (유닛 객체와 매핑)
        /// </summary>
        /// <param name="units">CreateBattleTeam()으로 생성된 유닛 리스트</param>
        public Dictionary<IBattleUnit, List<IBattleAction>> CollectAllActions(List<IBattleUnit> units)
        {
            var actionMap = new Dictionary<IBattleUnit, List<IBattleAction>>();

            for (int i = 0; i < characters.Count && i < units.Count; i++)
            {
                List<IBattleAction> actions = characters[i].CollectActions();
                actionMap[units[i]] = actions;
            }

            return actionMap;
        }

        /// <summary>
        /// 패배 조건 가져오기
        /// </summary>
        public DefeatCondition GetDefeatCondition()
        {
            if (defeatConditionDropdown != null)
            {
                return (DefeatCondition)defeatConditionDropdown.value;
            }

            return DefeatCondition.AllDead;
        }

        /// <summary>
        /// 메인 캐릭터 인덱스 (주캐 사망 조건일 때)
        /// </summary>
        public int GetMainCharacterIndex()
        {
            if (mainCharacterDropdown != null)
            {
                return mainCharacterDropdown.value;
            }

            // 기본값: 첫 번째 캐릭터
            return 0;
        }

        /// <summary>
        /// UI 입력을 BattleTeam 데이터로 변환
        /// 시뮬레이터에 전달하기 위한 팀 데이터 생성
        /// </summary>
        public BattleTeam CreateBattleTeam()
        {
            var team = new BattleTeam();

            // 1. 유닛 생성
            team.Units = CreateTeam();

            // 2. 패배 조건 설정
            team.DefeatCondition = GetDefeatCondition();

            // 3. 메인 캐릭터 설정 (주캐 사망 조건일 때만)
            if (team.DefeatCondition == DefeatCondition.MainCharacterDead)
            {
                team.MainCharacterIndex = GetMainCharacterIndex();
            }
            else
            {
                team.MainCharacterIndex = -1; // 전멸 조건에서는 사용 안 함
            }

            return team;
        }
    }

}
