using System.Collections.Generic;
using System.Linq;
using TurnBasedSimTool.Core.Logic;

namespace TurnBasedSimTool.Core {
    /// <summary>
    /// 몬테카를로 시뮬레이션 실행기
    /// SimulationSettings를 기반으로 여러 판을 실행하고 통계를 생성합니다
    /// </summary>
    public class MonteCarloRunner {
        private readonly FlexibleBattleSimulator _simulator;

        public MonteCarloRunner(FlexibleBattleSimulator simulator) {
            _simulator = simulator;
        }

        /// <summary>
        /// NvM 팀 시뮬레이션 실행
        /// </summary>
        public MonteCarloReport RunTeamSimulation(BattleTeam playerTeam, BattleTeam enemyTeam, SimulationSettings settings) {
            List<SimulationResult> results = new List<SimulationResult>(settings.Iterations);

            for (int i = 0; i < settings.Iterations; i++) {
                // 매 판마다 독립적인 컨텍스트 생성 (데이터 오염 방지)
                var context = new BattleContext {
                    CurrentTurn = 0,
                    IsFinished = false,
                    UseCostSystem = settings.UseCostSystem,
                    MaxActionsPerTurn = settings.MaxActionsPerTurn,
                    Cost = new CostHandler {
                        MaxCost = settings.MaxCost,
                        RecoveryAmount = settings.RecoveryAmount
                    }
                };

                // 팀 시뮬레이터 실행
                var result = _simulator.RunTeamBattle(playerTeam, enemyTeam, context, settings.MaxTurns);
                results.Add(result);
            }

            return new MonteCarloReport(results);
        }

        /// <summary>
        /// 1v1 시뮬레이션 실행 (하위 호환성)
        /// </summary>
        public MonteCarloReport RunSimulation(IBattleUnit player, IBattleUnit enemy, SimulationSettings settings) {
            List<SimulationResult> results = new List<SimulationResult>(settings.Iterations);

            for (int i = 0; i < settings.Iterations; i++) {
                // 매 판마다 독립적인 컨텍스트 생성 (데이터 오염 방지)
                var context = new BattleContext {
                    CurrentTurn = 0,
                    IsFinished = false,
                    UseCostSystem = settings.UseCostSystem,
                    MaxActionsPerTurn = settings.MaxActionsPerTurn,
                    Cost = new CostHandler {
                        MaxCost = settings.MaxCost,
                        RecoveryAmount = settings.RecoveryAmount
                    }
                };

                // 시뮬레이터 실행
                var result = _simulator.Run(player, enemy, context, settings.MaxTurns);
                results.Add(result);
            }

            return new MonteCarloReport(results);
        }
    }

    /// <summary>
    /// 몬테카를로 시뮬레이션 결과 통계
    /// </summary>
    public struct MonteCarloReport {
        public int TotalCount;
        public int WinCount;
        public float WinRate;
        public float AvgTurns;

        public MonteCarloReport(List<SimulationResult> results) {
            TotalCount = results.Count;
            WinCount = results.Count(r => r.IsPlayerWin);
            WinRate = (float)WinCount / TotalCount * 100f;
            AvgTurns = (float)results.Average(r => r.TotalTurns);
        }
    }
}