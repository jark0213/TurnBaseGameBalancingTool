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
        /// 몬테카를로 시뮬레이션 실행 (NvM 지원)
        /// </summary>
        public void RunMonteCarlo()
        {
            // 1. 패널로부터 설정 수집
            var settings = settingsPanel.GetSettings();

            // 2. 팀 데이터 생성 (UI → Core 변환)
            BattleTeam playerBattleTeam = playerTeam.CreateBattleTeam();
            BattleTeam enemyBattleTeam = enemyTeam.CreateBattleTeam();

            // 검증 및 디버그
            Debug.Log($"[Setup] Player Team: {playerBattleTeam.Units?.Count ?? 0} units");
            if (playerBattleTeam.Units == null || playerBattleTeam.Units.Count == 0)
            {
                Debug.LogError("Player team is empty!");
                return;
            }

            Debug.Log($"[Setup] Enemy Team: {enemyBattleTeam.Units?.Count ?? 0} units");
            if (enemyBattleTeam.Units == null || enemyBattleTeam.Units.Count == 0)
            {
                Debug.LogError("Enemy team is empty!");
                return;
            }

            // 3. 액션 수집 (같은 유닛 객체를 인자로 전달)
            var playerActions = playerTeam.CollectAllActions(playerBattleTeam.Units);
            var enemyActions = enemyTeam.CollectAllActions(enemyBattleTeam.Units);

            // BattleTeam에 액션 설정 (인덱스 기반)
            playerBattleTeam.ActionsPerUnit = new List<List<IBattleAction>>();
            foreach (var kvp in playerActions)
            {
                playerBattleTeam.ActionsPerUnit.Add(kvp.Value);
            }

            enemyBattleTeam.ActionsPerUnit = new List<List<IBattleAction>>();
            foreach (var kvp in enemyActions)
            {
                enemyBattleTeam.ActionsPerUnit.Add(kvp.Value);
            }

            // 디버그 로그
            Debug.Log($"[Setup] Player Actions: {playerBattleTeam.ActionsPerUnit.Count} units with actions");
            for (int i = 0; i < playerBattleTeam.Units.Count; i++)
            {
                int actionCount = i < playerBattleTeam.ActionsPerUnit.Count ? playerBattleTeam.ActionsPerUnit[i].Count : 0;
                Debug.Log($"  - Unit[{i}] '{playerBattleTeam.Units[i].Name}' (HP:{playerBattleTeam.Units[i].MaxHp}): {actionCount} actions");
            }

            Debug.Log($"[Setup] Enemy Actions: {enemyBattleTeam.ActionsPerUnit.Count} units with actions");
            for (int i = 0; i < enemyBattleTeam.Units.Count; i++)
            {
                int actionCount = i < enemyBattleTeam.ActionsPerUnit.Count ? enemyBattleTeam.ActionsPerUnit[i].Count : 0;
                Debug.Log($"  - Unit[{i}] '{enemyBattleTeam.Units[i].Name}' (HP:{enemyBattleTeam.Units[i].MaxHp}): {actionCount} actions");
            }

            // 4. Phase 준비 (Speed 시스템 ON/OFF에 따라 분기)
            _simulator.ClearPhases();
            
            if (settings.UseSpeedSystem)
            {
                // Speed 기반: 모든 유닛을 Speed 순으로 정렬하여 행동
                Debug.Log("[Setup] Using Speed-based combat system");
                _simulator.AddPhase(new SpeedBasedCombatPhase("SpeedBasedCombat", new RandomTargeting(), settings));
            }
            else
            {
                // 팀 기반: Player → Enemy 순서 (또는 FirstTurn 옵션에 따라)
                Debug.Log($"[Setup] Using Team-based combat system (FirstTurn: {settings.FirstTurn})");
                
                if (settings.FirstTurn == FirstTurnOption.PlayerFirst)
                {
                    _simulator.AddPhase(new TeamActionPhase("PlayerTurn", true, new RandomTargeting()));
                    _simulator.AddPhase(new TeamActionPhase("EnemyTurn", false, new RandomTargeting()));
                }
                else if (settings.FirstTurn == FirstTurnOption.EnemyFirst)
                {
                    _simulator.AddPhase(new TeamActionPhase("EnemyTurn", false, new RandomTargeting()));
                    _simulator.AddPhase(new TeamActionPhase("PlayerTurn", true, new RandomTargeting()));
                }
                else // Random
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        _simulator.AddPhase(new TeamActionPhase("PlayerTurn", true, new RandomTargeting()));
                        _simulator.AddPhase(new TeamActionPhase("EnemyTurn", false, new RandomTargeting()));
                    }
                    else
                    {
                        _simulator.AddPhase(new TeamActionPhase("EnemyTurn", false, new RandomTargeting()));
                        _simulator.AddPhase(new TeamActionPhase("PlayerTurn", true, new RandomTargeting()));
                    }
                }
            }

            // 4. NvM 시뮬레이션 실행
            Debug.Log($"[Simulation] Starting {settings.Iterations} iterations...");
            MonteCarloReport report = _runner.RunTeamSimulation(playerBattleTeam, enemyBattleTeam, settings);

            // 5. 결과 저장 및 표시
            _reportHistory.Add(report);
            resultPanel.DisplayResult(report);

            Debug.Log($"[Result] Win Rate: {report.WinRate:F1}% ({report.WinCount}/{report.TotalCount}), Avg Turns: {report.AvgTurns:F1}");
        }
    }
}
