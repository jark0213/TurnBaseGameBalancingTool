using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 캐릭터 설정 패널 (HP, 스탯, 액션 리스트)
    /// 한 명의 캐릭터 설정을 담당
    /// TeamSettingsPanel에서 여러 개 관리됨
    /// </summary>
    public class CharacterSettingsPanel : MonoBehaviour
    {
        [Header("Character Info")]
        [SerializeField] private TMP_InputField characterNameInput;
        [SerializeField] private Toggle foldoutToggle;
        [SerializeField] private Button deleteButton;
        [SerializeField] private GameObject contentArea; // 접었다 펼 영역

        public event System.Action<CharacterSettingsPanel> OnDeleteRequested;
        [Header("Basic Stats")]
        [SerializeField] private TMP_InputField hpInput;
        [SerializeField] private TMP_InputField damageInput;

        [Header("Optional Stats (DefaultUnit)")]
        [SerializeField] private TMP_InputField defenseInput;
        [SerializeField] private TMP_InputField evasionInput;
        [SerializeField] private TMP_InputField critRateInput;
        [SerializeField] private TMP_InputField speedInput;

        [Header("Action List")]
        [SerializeField] private Button addActionButton;
        [SerializeField] private Transform actionListContent;
        [SerializeField] private GameObject actionItemPrefab;

        [Header("Scroll Control (Optional)")]
        [SerializeField] private AutoScrollController actionScrollController;

        private void Start()
        {
            // Add Action 버튼
            if (addActionButton)
            {
                addActionButton.onClick.RemoveAllListeners();
                addActionButton.onClick.AddListener(AddActionItem);
            }

            // Delete 버튼
            if (deleteButton)
            {
                deleteButton.onClick.RemoveAllListeners();
                deleteButton.onClick.AddListener(() => OnDeleteRequested?.Invoke(this));
            }

            // Foldout 토글
            if (foldoutToggle)
            {
                foldoutToggle.onValueChanged.RemoveAllListeners();
                foldoutToggle.onValueChanged.AddListener(OnFoldoutToggled);
                foldoutToggle.isOn = true; // 기본 펼침
            }

            // 기본값 설정
            if (characterNameInput && string.IsNullOrEmpty(characterNameInput.text))
                characterNameInput.text = "Character";
            if (hpInput && string.IsNullOrEmpty(hpInput.text))
                hpInput.text = "100";
            if (damageInput && string.IsNullOrEmpty(damageInput.text))
                damageInput.text = "10";
            if (defenseInput && string.IsNullOrEmpty(defenseInput.text))
                defenseInput.text = "0";
            if (evasionInput && string.IsNullOrEmpty(evasionInput.text))
                evasionInput.text = "0";
            if (critRateInput && string.IsNullOrEmpty(critRateInput.text))
                critRateInput.text = "0";
            if (speedInput && string.IsNullOrEmpty(speedInput.text))
                speedInput.text = "10";
        }

        /// <summary>
        /// Foldout 토글 시 Content 영역 접기/펼치기
        /// </summary>
        private void OnFoldoutToggled(bool isExpanded)
        {
            if (contentArea != null)
            {
                contentArea.SetActive(isExpanded);
            }
        }

        /// <summary>
        /// SimulationSettingsPanel과 연결하여 코스트/스피드 시스템 토글 동기화
        /// </summary>
        public void ConnectToSimulationSettings(SimulationSettingsPanel simPanel)
        {
            if (simPanel != null)
            {
                simPanel.OnCostSystemChanged += SetCostFieldsActive;
                simPanel.OnSpeedSystemChanged += SetSpeedFieldActive;
            }
        }

        /// <summary>
        /// 모든 ActionItem의 코스트 필드 활성화/비활성화
        /// </summary>
        private void SetCostFieldsActive(bool active)
        {
            if (actionListContent == null)
                return;

            ActionItemUI[] items = actionListContent.GetComponentsInChildren<ActionItemUI>(true);
            foreach (var item in items)
            {
                item.SetCostFieldActive(active);
            }
        }

        /// <summary>
        /// Speed 필드 활성화/비활성화
        /// </summary>
        private void SetSpeedFieldActive(bool active)
        {
            if (speedInput != null && speedInput.transform.parent != null)
            {
                speedInput.transform.parent.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 입력된 설정으로 캐릭터 생성
        /// </summary>
        public IBattleUnit CreateCharacter()
        {
            // 캐릭터 이름
            string characterName = "Character";
            if (characterNameInput && !string.IsNullOrEmpty(characterNameInput.text))
                characterName = characterNameInput.text;
            int hp = 100;
            if (hpInput && int.TryParse(hpInput.text, out int parsedHp))
                hp = parsedHp;

            int defense = 0;
            if (defenseInput && int.TryParse(defenseInput.text, out int parsedDefense))
                defense = parsedDefense;

            int evasion = 0;
            if (evasionInput && int.TryParse(evasionInput.text, out int parsedEvasion))
                evasion = parsedEvasion;

            int critRate = 0;
            if (critRateInput && int.TryParse(critRateInput.text, out int parsedCritRate))
                critRate = parsedCritRate;

            int speed = 10;
            if (speedInput && int.TryParse(speedInput.text, out int parsedSpeed))
                speed = parsedSpeed;

            return new DefaultUnit
            {
                Name = characterName,
                MaxHp = hp,
                CurrentHp = hp,
                Defense = defense,
                Evasion = evasion,
                CritRate = critRate,
                Speed = speed
            };
        }

        /// <summary>
        /// 액션 리스트에서 선택된 액션들을 수집
        /// </summary>
        public List<IBattleAction> CollectActions()
        {
            List<IBattleAction> actions = new List<IBattleAction>();

            if (actionListContent == null)
                return actions;

            ActionItemUI[] items = actionListContent.GetComponentsInChildren<ActionItemUI>();

            foreach (var item in items)
            {
                if (item.IsSelected)
                {
                    IBattleAction baseAction;

                    // 1. 랜덤 데미지 사용 여부 확인
                    if (item.UseRangedDamage)
                    {
                        baseAction = new RangedDamageAction
                        {
                            ActionName = item.ActionName,
                            MinDamage = item.MinDamage,
                            MaxDamage = item.MaxDamage,
                            Cost = item.CostValue
                        };
                    }
                    else
                    {
                        // 고정 데미지
                        baseAction = new GenericAction
                        {
                            ActionName = item.ActionName,
                            Damage = item.ActionValue,
                            Cost = item.CostValue
                        };
                    }

                    // 2. 인터벌이 1보다 크면 어댑터로 감싸기
                    int interval = item.IntervalValue;
                    if (interval > 1)
                    {
                        baseAction = new IntervalActionAdapter(baseAction, interval);
                    }

                    actions.Add(baseAction);
                }
            }

            return actions;
        }

        /// <summary>
        /// 액션 아이템 추가
        /// </summary>
        public void AddActionItem()
        {
            if (actionItemPrefab && actionListContent)
            {
                Instantiate(actionItemPrefab, actionListContent);

                // 스크롤 상태 업데이트
                if (actionScrollController != null)
                {
                    actionScrollController.OnContentChanged();
                }
            }
        }

        /// <summary>
        /// 모든 액션 아이템 제거
        /// </summary>
        public void ClearActions()
        {
            if (actionListContent == null)
                return;

            foreach (Transform child in actionListContent)
            {
                Destroy(child.gameObject);
            }

            // 스크롤 상태 업데이트
            if (actionScrollController != null)
            {
                actionScrollController.OnContentChanged();
            }
        }
    }
}
