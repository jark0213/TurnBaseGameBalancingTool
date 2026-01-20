using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 시뮬레이션 설정 패널
    /// 반복 횟수, 최대 턴, 코스트 시스템 등 설정
    /// </summary>
    public class SimulationSettingsPanel : MonoBehaviour
    {
        [Header("Basic Settings")]
        [SerializeField] private TMP_InputField iterationsInput;
        [SerializeField] private TMP_InputField maxTurnsInput;
        [SerializeField] private TMP_InputField maxActionsPerTurnInput;

        [Header("Cost System")]
        [SerializeField] private Toggle useCostSystemToggle;
        [SerializeField] private TMP_InputField maxCostInput;
        [SerializeField] private TMP_InputField recoveryAmountInput;

        [Header("Speed System")]
        [SerializeField] private Toggle useSpeedSystemToggle;
        [SerializeField] private TMP_Dropdown speedTiebreakDropdown;    // Speed 동점 시 처리
        [SerializeField] private TMP_Dropdown tiebreakStatDropdown;     // 타이브레이크 스탯 선택
        [SerializeField] private TMP_InputField customStatNameInput;    // 커스텀 스탯 이름

        [Header("Turn Order (when Speed OFF)")]
        [SerializeField] private TMP_Dropdown firstTurnDropdown;

        // 이벤트
        public event System.Action<bool> OnCostSystemChanged;
        public event System.Action<bool> OnSpeedSystemChanged;

        // Parent GameObjects (자동 찾기)
        private GameObject maxCostInputParent;
        private GameObject recoveryAmountInputParent;
        private GameObject firstTurnDropdownParent;
        private GameObject speedTiebreakDropdownParent;
        private GameObject tiebreakStatDropdownParent;
        private GameObject customStatNameInputParent;

        private void Awake()
        {
            // InputField의 부모 GameObject 자동 찾기
            if (maxCostInput != null)
                maxCostInputParent = maxCostInput.transform.parent.gameObject;

            if (recoveryAmountInput != null)
                recoveryAmountInputParent = recoveryAmountInput.transform.parent.gameObject;

            if (firstTurnDropdown != null)
                firstTurnDropdownParent = firstTurnDropdown.transform.parent.gameObject;

            if (speedTiebreakDropdown != null)
                speedTiebreakDropdownParent = speedTiebreakDropdown.transform.parent.gameObject;

            if (tiebreakStatDropdown != null)
                tiebreakStatDropdownParent = tiebreakStatDropdown.transform.parent.gameObject;

            if (customStatNameInput != null)
                customStatNameInputParent = customStatNameInput.transform.parent.gameObject;
        }

        private void Start()
        {
            InitializeDefaults();

            // 코스트 토글 변경 시 이벤트 발생 및 UI 업데이트
            if (useCostSystemToggle != null)
            {
                useCostSystemToggle.onValueChanged.AddListener(OnCostToggleChanged);
                // 초기 상태 전파
                OnCostToggleChanged(useCostSystemToggle.isOn);
            }

            // 스피드 토글 변경 시 이벤트 발생 및 UI 업데이트
            if (useSpeedSystemToggle != null)
            {
                useSpeedSystemToggle.onValueChanged.AddListener(OnSpeedToggleChanged);
                // 초기 상태 전파
                OnSpeedToggleChanged(useSpeedSystemToggle.isOn);
            }

            // 타이브레이크 드롭다운 변경 시
            if (speedTiebreakDropdown != null)
            {
                speedTiebreakDropdown.onValueChanged.AddListener(OnTiebreakMethodChanged);
                OnTiebreakMethodChanged(speedTiebreakDropdown.value);
            }

            // 타이브레이크 스탯 드롭다운 변경 시
            if (tiebreakStatDropdown != null)
            {
                tiebreakStatDropdown.onValueChanged.AddListener(OnTiebreakStatChanged);
                OnTiebreakStatChanged(tiebreakStatDropdown.value);
            }

            // 커스텀 스탯 이름 입력 완료 시 검증
            if (customStatNameInput != null)
            {
                customStatNameInput.onEndEdit.AddListener(ValidateCustomStatName);
            }
        }

        private void OnCostToggleChanged(bool isOn)
        {
            // 자신의 MaxCost/RecoveryAmount 필드 토글
            if (maxCostInputParent != null)
                maxCostInputParent.SetActive(isOn);

            if (recoveryAmountInputParent != null)
                recoveryAmountInputParent.SetActive(isOn);

            // 외부(UnitSettingsPanel)에 이벤트 전파
            OnCostSystemChanged?.Invoke(isOn);
        }

        private void OnSpeedToggleChanged(bool isOn)
        {
            // FirstTurn dropdown은 Speed가 OFF일 때만 표시
            if (firstTurnDropdownParent != null)
                firstTurnDropdownParent.SetActive(!isOn);

            // Tiebreak dropdown은 Speed가 ON일 때만 표시
            if (speedTiebreakDropdownParent != null)
                speedTiebreakDropdownParent.SetActive(isOn);

            // 외부(UnitSettingsPanel)에 이벤트 전파
            OnSpeedSystemChanged?.Invoke(isOn);
        }

        private void OnTiebreakMethodChanged(int value)
        {
            SpeedTiebreakOption option = (SpeedTiebreakOption)value;
            
            // UseStat 선택 시만 tiebreakStatDropdown 표시
            bool showStatDropdown = (option == SpeedTiebreakOption.UseStat);
            if (tiebreakStatDropdownParent != null)
                tiebreakStatDropdownParent.SetActive(showStatDropdown);

            // UseStat 아니면 customStatNameInput도 숨김 (Parent 끄기)
            if (!showStatDropdown && customStatNameInputParent != null)
                customStatNameInputParent.SetActive(false);
        }

        private void OnTiebreakStatChanged(int value)
        {
            TiebreakStatOption option = (TiebreakStatOption)value;
            
            // Custom 선택 시만 customStatNameInput 표시 (Parent 켜기)
            bool showCustomInput = (option == TiebreakStatOption.Custom);
            if (customStatNameInputParent != null)
            {
                customStatNameInputParent.SetActive(showCustomInput);
                
                // Custom 아니면 InputField 색상 리셋
                if (!showCustomInput && customStatNameInput != null)
                {
                    ResetInputFieldColors(customStatNameInput);
                }
            }
        }

        private void ValidateCustomStatName(string statName)
        {
            if (customStatNameInput == null) return;

            // 비어있으면 경고 (오렌지)
            if (string.IsNullOrEmpty(statName))
            {
                SetInputFieldColors(customStatNameInput, new Color(1f, 0.5f, 0f)); // 오렌지 텍스트
                return;
            }

            // 스탯 존재 여부 확인
            bool isValid = IsValidStatName(statName);

            if (isValid)
            {
                // 초록색 (유효)
                SetInputFieldColors(customStatNameInput, Color.green);
            }
            else
            {
                // 빨간색 (오류)
                SetInputFieldColors(customStatNameInput, Color.red);
            }
        }

        private bool IsValidStatName(string statName)
        {
            // DefaultUnit의 기본 속성들
            string[] validStats = { "Defense", "Evasion", "CritRate", "CritMultiplier", "Speed", "MaxHp", "CurrentHp" };
            
            foreach (var validStat in validStats)
            {
                if (statName.Equals(validStat, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // CustomData는 나중에 실제 유닛 검증 필요
            // 지금은 일단 경고만 표시
            return false;
        }

        /// <summary>
        /// InputField의 텍스트 색상과 테두리 색상 변경
        /// </summary>
        private void SetInputFieldColors(TMP_InputField inputField, Color color)
        {
            if (inputField == null) return;

            // 1. 입력 텍스트 색상 변경
            if (inputField.textComponent != null)
            {
                inputField.textComponent.color = color;
            }

            // 2. 테두리(배경) 색상 변경
            var image = inputField.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                // 약간 투명한 색상으로 테두리 표시
                image.color = new Color(color.r, color.g, color.b, 0.3f);
            }
        }

        /// <summary>
        /// InputField 색상 리셋 (기본 상태로)
        /// </summary>
        private void ResetInputFieldColors(TMP_InputField inputField)
        {
            if (inputField == null) return;

            // 텍스트 색상 리셋
            if (inputField.textComponent != null)
            {
                inputField.textComponent.color = Color.white; // 기본 흰색
            }

            // 테두리 색상 리셋
            var image = inputField.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.color = new Color(1f, 1f, 1f, 0.1f); // 기본 반투명 흰색
            }
        }

        /// <summary>
        /// TiebreakStatOption의 표시 라벨 가져오기
        /// TODO: Localization 적용 시 수정
        /// </summary>
        private string GetTiebreakStatLabel(TiebreakStatOption stat)
        {
            switch (stat)
            {
                case TiebreakStatOption.Defense:
                    return "방어력";
                case TiebreakStatOption.Evasion:
                    return "회피율";
                case TiebreakStatOption.CritRate:
                    return "치명타율";
                case TiebreakStatOption.Speed:
                    return "속도";
                case TiebreakStatOption.Custom:
                    return "커스텀";
                default:
                    return stat.ToString();
            }
        }

        /// <summary>
        /// 기본값 초기화
        /// </summary>
        private void InitializeDefaults()
        {
            if (iterationsInput) iterationsInput.text = "1000";
            if (maxTurnsInput) maxTurnsInput.text = "100";
            if (maxActionsPerTurnInput) maxActionsPerTurnInput.text = "1";
            if (useCostSystemToggle) useCostSystemToggle.isOn = true;
            if (maxCostInput) maxCostInput.text = "3";
            if (recoveryAmountInput) recoveryAmountInput.text = "3";
            if (useSpeedSystemToggle) useSpeedSystemToggle.isOn = false;

            // FirstTurn Dropdown 초기화 (Player First, Enemy First, Random)
            if (firstTurnDropdown)
            {
                firstTurnDropdown.ClearOptions();
                firstTurnDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Player First",
                    "Enemy First",
                    "Random"
                });
                firstTurnDropdown.value = 0; // Player First 기본
            }

            // Speed Tiebreak Dropdown 초기화
            if (speedTiebreakDropdown)
            {
                speedTiebreakDropdown.ClearOptions();
                speedTiebreakDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Random",
                    "Player First",
                    "Enemy First",
                    "Use Stat"
                });
                speedTiebreakDropdown.value = 0; // Random 기본
            }

            // Tiebreak Stat Dropdown 초기화 (Enum에서 자동 생성)
            if (tiebreakStatDropdown)
            {
                tiebreakStatDropdown.ClearOptions();
                
                // Enum의 모든 값에 대해 라벨 생성
                var labels = new System.Collections.Generic.List<string>();
                foreach (TiebreakStatOption stat in System.Enum.GetValues(typeof(TiebreakStatOption)))
                {
                    labels.Add(GetTiebreakStatLabel(stat));
                }
                
                tiebreakStatDropdown.AddOptions(labels);
                tiebreakStatDropdown.value = 0; // Defense 기본
            }

            // 초기 상태: tiebreakStatDropdown과 customStatNameInput 숨김
            if (tiebreakStatDropdownParent != null)
                tiebreakStatDropdownParent.SetActive(false);
            
            if (customStatNameInputParent != null)
                customStatNameInputParent.SetActive(false);
        }

        /// <summary>
        /// UI에서 설정값을 수집하여 SimulationSettings 객체 반환
        /// </summary>
        public SimulationSettings GetSettings()
        {
            var settings = new SimulationSettings();

            // Basic Settings
            if (iterationsInput && int.TryParse(iterationsInput.text, out int iterations))
                settings.Iterations = iterations;

            if (maxTurnsInput && int.TryParse(maxTurnsInput.text, out int maxTurns))
                settings.MaxTurns = maxTurns;

            if (maxActionsPerTurnInput && int.TryParse(maxActionsPerTurnInput.text, out int maxActions))
                settings.MaxActionsPerTurn = maxActions;

            // Cost System
            if (useCostSystemToggle)
                settings.UseCostSystem = useCostSystemToggle.isOn;

            if (maxCostInput && int.TryParse(maxCostInput.text, out int maxCost))
                settings.MaxCost = maxCost;

            if (recoveryAmountInput && int.TryParse(recoveryAmountInput.text, out int recovery))
                settings.RecoveryAmount = recovery;

            // Speed System
            if (useSpeedSystemToggle)
                settings.UseSpeedSystem = useSpeedSystemToggle.isOn;

            // First Turn (Speed OFF 시)
            if (firstTurnDropdown)
                settings.FirstTurn = (FirstTurnOption)firstTurnDropdown.value;

            // Speed Tiebreak (Speed ON 시)
            if (speedTiebreakDropdown)
                settings.SpeedTiebreak = (SpeedTiebreakOption)speedTiebreakDropdown.value;

            if (tiebreakStatDropdown)
                settings.TiebreakStat = (TiebreakStatOption)tiebreakStatDropdown.value;

            if (customStatNameInput)
                settings.CustomTiebreakStatName = customStatNameInput.text;

            return settings;
        }

        /// <summary>
        /// SimulationSettings 객체로 UI 업데이트
        /// </summary>
        public void SetSettings(SimulationSettings settings)
        {
            if (iterationsInput) iterationsInput.text = settings.Iterations.ToString();
            if (maxTurnsInput) maxTurnsInput.text = settings.MaxTurns.ToString();
            if (maxActionsPerTurnInput) maxActionsPerTurnInput.text = settings.MaxActionsPerTurn.ToString();
            if (useCostSystemToggle) useCostSystemToggle.isOn = settings.UseCostSystem;
            if (maxCostInput) maxCostInput.text = settings.MaxCost.ToString();
            if (recoveryAmountInput) recoveryAmountInput.text = settings.RecoveryAmount.ToString();
            if (useSpeedSystemToggle) useSpeedSystemToggle.isOn = settings.UseSpeedSystem;
            if (firstTurnDropdown) firstTurnDropdown.value = (int)settings.FirstTurn;
            if (speedTiebreakDropdown) speedTiebreakDropdown.value = (int)settings.SpeedTiebreak;
            if (tiebreakStatDropdown) tiebreakStatDropdown.value = (int)settings.TiebreakStat;
            if (customStatNameInput) customStatNameInput.text = settings.CustomTiebreakStatName;
        }
    }
}
