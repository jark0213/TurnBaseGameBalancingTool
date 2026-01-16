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

        [Header("Turn Order (when Speed OFF)")]
        [SerializeField] private TMP_Dropdown firstTurnDropdown;

        // 이벤트
        public event System.Action<bool> OnCostSystemChanged;
        public event System.Action<bool> OnSpeedSystemChanged;

        // Parent GameObjects (자동 찾기)
        private GameObject maxCostInputParent;
        private GameObject recoveryAmountInputParent;
        private GameObject firstTurnDropdownParent;

        private void Awake()
        {
            // InputField의 부모 GameObject 자동 찾기
            if (maxCostInput != null)
                maxCostInputParent = maxCostInput.transform.parent.gameObject;

            if (recoveryAmountInput != null)
                recoveryAmountInputParent = recoveryAmountInput.transform.parent.gameObject;

            if (firstTurnDropdown != null)
                firstTurnDropdownParent = firstTurnDropdown.transform.parent.gameObject;
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

            // 외부(UnitSettingsPanel)에 이벤트 전파
            OnSpeedSystemChanged?.Invoke(isOn);
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
        }
    }
}
