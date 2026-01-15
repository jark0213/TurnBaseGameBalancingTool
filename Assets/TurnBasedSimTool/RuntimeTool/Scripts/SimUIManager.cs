using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 런타임 시뮬레이션 UI 매니저
    /// UI에서 설정된 값들을 기반으로 몬테카를로 시뮬레이션을 실행합니다
    /// </summary>
    public class SimUIManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] TMP_InputField playerHpInput;
        [SerializeField] TMP_InputField playerDmgInput;
        [SerializeField] Button playerActionAddButton;
        [SerializeField] Transform playerActionContent;

        [Header("Enemy Settings")]
        [SerializeField] TMP_InputField enemyHpInput;
        [SerializeField] TMP_InputField enemyDmgInput;
        [SerializeField] Button enemyActionAddButton;
        [SerializeField] Transform enemyActionContent;

        [Header("Simulation Settings")]
        [SerializeField] TMP_InputField iterationsInput;
        [SerializeField] TMP_InputField maxTurnsInput;
        [SerializeField] TMP_InputField maxActionsPerTurnInput;
        [SerializeField] Toggle useCostSystemToggle;
        [SerializeField] TMP_InputField maxCostInput;
        [SerializeField] TMP_InputField recoveryAmountInput;

        [Header("UI Controls")]
        [SerializeField] Button runButton;
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] GameObject actionItemPrefab;

        private List<MonteCarloReport> _reportHistory = new List<MonteCarloReport>();
        private FlexibleBattleSimulator _simulator;
        private MonteCarloRunner _runner;

        void Start()
        {
            _simulator = new FlexibleBattleSimulator();
            _runner = new MonteCarloRunner(_simulator);
            InitializeDefaultValues();
            ClearButtonsActions();
            AddButtonAction();
        }

        /// <summary>
        /// UI 기본값 초기화
        /// </summary>
        private void InitializeDefaultValues()
        {
            if (iterationsInput) iterationsInput.text = "1000";
            if (maxTurnsInput) maxTurnsInput.text = "100";
            if (maxActionsPerTurnInput) maxActionsPerTurnInput.text = "1";
            if (useCostSystemToggle) useCostSystemToggle.isOn = true;
            if (maxCostInput) maxCostInput.text = "3";
            if (recoveryAmountInput) recoveryAmountInput.text = "3";
        }

        private void OnRunClick()
        {
            RunMonteCarlo();
        }

        /// <summary>
        /// 몬테카를로 시뮬레이션 실행
        /// </summary>
        public void RunMonteCarlo()
        {
            // 1. SimulationSettings 수집
            var settings = new SimulationSettings
            {
                Iterations = int.Parse(iterationsInput.text),
                MaxTurns = int.Parse(maxTurnsInput.text),
                MaxActionsPerTurn = int.Parse(maxActionsPerTurnInput.text),
                UseCostSystem = useCostSystemToggle.isOn,
                MaxCost = int.Parse(maxCostInput.text),
                RecoveryAmount = int.Parse(recoveryAmountInput.text)
            };

            // 2. 유닛 생성
            var player = new DefaultUnit
            {
                Name = "Player",
                MaxHp = int.Parse(playerHpInput.text),
                CurrentHp = int.Parse(playerHpInput.text)
            };
            var enemy = new DefaultUnit
            {
                Name = "Enemy",
                MaxHp = int.Parse(enemyHpInput.text),
                CurrentHp = int.Parse(enemyHpInput.text)
            };

            // 3. Phase 준비
            _simulator.ClearPhases();

            var playerPhase = new ManualActionPhase("PlayerTurn", true);
            var enemyPhase = new ManualActionPhase("EnemyTurn", false);

            playerPhase.SetActions(CollectActionsFromUI(playerActionContent));
            enemyPhase.SetActions(CollectActionsFromUI(enemyActionContent));

            _simulator.AddPhase(playerPhase);
            _simulator.AddPhase(enemyPhase);

            // 4. 시뮬레이션 실행
            MonteCarloReport report = _runner.RunSimulation(player, enemy, settings);

            // 5. 결과 저장 및 표시
            _reportHistory.Add(report);
            DisplaySimulationResult(report);
        }

        /// <summary>
        /// 시뮬레이션 결과 표시
        /// </summary>
        private void DisplaySimulationResult(MonteCarloReport report)
        {
            resultText.text = $"Win Rate: {report.WinRate:F2}%\nAvg Turns: {report.AvgTurns:F1}";
        }

        /// <summary>
        /// UI에서 액션 리스트 수집
        /// </summary>
        private List<IBattleAction> CollectActionsFromUI(Transform content)
        {
            List<IBattleAction> actions = new List<IBattleAction>();
            ActionItemUI[] items = content.GetComponentsInChildren<ActionItemUI>();

            foreach (var item in items)
            {
                if (item.IsSelected)
                {
                    // 1. 기본 액션 생성
                    IBattleAction baseAction = new GenericAction
                    {
                        ActionName = item.ActionName,
                        Damage = item.ActionValue
                    };

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

        void ClearButtonsActions()
        {
            playerActionAddButton.onClick.RemoveAllListeners();
            enemyActionAddButton.onClick.RemoveAllListeners();
            runButton.onClick.RemoveAllListeners();
        }

        void AddButtonAction()
        {
            playerActionAddButton.onClick.AddListener(() => AddActionItem(true));
            enemyActionAddButton.onClick.AddListener(() => AddActionItem(false));
            runButton.onClick.AddListener(OnRunClick);
        }

        public void AddActionItem(bool isPlayer)
        {
            Transform parent = isPlayer ? playerActionContent : enemyActionContent;
            Instantiate(actionItemPrefab, parent);
        }
    }
}
