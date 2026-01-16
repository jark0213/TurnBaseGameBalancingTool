using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 런타임 시뮬레이션 UI 매니저
    /// 각 패널을 조율하여 몬테카를로 시뮬레이션을 실행합니다
    /// </summary>
    public class SimUIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private TeamSettingsPanel playerTeam;
        [SerializeField] private TeamSettingsPanel enemyTeam;
        [SerializeField] private SimulationSettingsPanel settingsPanel;
        [SerializeField] private SimulationResultPanel resultPanel;

        [Header("Control")]
        [SerializeField] private Button runButton;

        private FlexibleBattleSimulator _simulator;
        private MonteCarloRunner _runner;
        private List<MonteCarloReport> _reportHistory = new List<MonteCarloReport>();

        void Start()
        {
            _simulator = new FlexibleBattleSimulator();
            _runner = new MonteCarloRunner(_simulator);

            // Panel 간 연결
            ConnectPanels();

            if (runButton)
            {
                runButton.onClick.RemoveAllListeners();
                runButton.onClick.AddListener(RunMonteCarlo);
            }
        }

        /// <summary>
        /// 패널들을 서로 연결 (코스트 시스템 토글 동기화 등)
        /// </summary>
        private void ConnectPanels()
        {
            if (settingsPanel != null)
            {
                // Player와 Enemy 팀을 Simulation Settings와 연결
                if (playerTeam != null)
                    playerTeam.ConnectToSimulationSettings(settingsPanel);

                if (enemyTeam != null)
                    enemyTeam.ConnectToSimulationSettings(settingsPanel);
            }
        }

        /// <summary>
        /// 몬테카를로 시뮬레이션 실행
        /// TODO: 팀 기반 시뮬레이션으로 확장 필요
        /// </summary>
        public void RunMonteCarlo()
        {
            // 1. 패널로부터 설정 수집
            var settings = settingsPanel.GetSettings();

            // 2. 팀 생성
            List<IBattleUnit> playerUnits = playerTeam.CreateTeam();
            List<IBattleUnit> enemyUnits = enemyTeam.CreateTeam();

            // 임시: 1v1 호환을 위해 첫 번째 캐릭터만 사용
            // TODO: NvM 지원으로 확장
            var player = playerUnits.Count > 0 ? playerUnits[0] : null;
            var enemy = enemyUnits.Count > 0 ? enemyUnits[0] : null;

            if (player == null || enemy == null)
            {
                Debug.LogError("Player or Enemy team is empty!");
                return;
            }

            // 3. Phase 준비
            _simulator.ClearPhases();

            var playerPhase = new ManualActionPhase("PlayerTurn", true);
            var enemyPhase = new ManualActionPhase("EnemyTurn", false);

            // 첫 번째 캐릭터의 액션 수집
            var playerActions = playerTeam.CollectAllActions();
            var enemyActions = enemyTeam.CollectAllActions();

            if (playerActions.ContainsKey(player))
                playerPhase.SetActions(playerActions[player]);
            if (enemyActions.ContainsKey(enemy))
                enemyPhase.SetActions(enemyActions[enemy]);

            _simulator.AddPhase(playerPhase);
            _simulator.AddPhase(enemyPhase);

            // 4. 시뮬레이션 실행
            MonteCarloReport report = _runner.RunSimulation(player, enemy, settings);

            // 5. 결과 저장 및 표시
            _reportHistory.Add(report);
            resultPanel.DisplayResult(report);

            Debug.Log($"Simulation completed: {report.WinRate:F1}% Player Win Rate");
        }
    }
}
