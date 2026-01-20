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
    /// 팀별 상세 통계
    /// </summary>
    public struct TeamStatistics {
        public float AvgSurvivors;          // 평균 생존자 수
        public float AvgRemainingHp;        // 평균 잔여 HP
        public float AvgHpPercentage;       // 평균 HP 비율 (%)
        public int MaxSurvivors;            // 최대 생존자 수
        public int MinSurvivors;            // 최소 생존자 수

        public TeamStatistics(List<SimulationResult> results, bool isPlayerTeam) {
            if (results == null || results.Count == 0) {
                AvgSurvivors = 0;
                AvgRemainingHp = 0;
                AvgHpPercentage = 0;
                MaxSurvivors = 0;
                MinSurvivors = 0;
                return;
            }

            var teamStates = isPlayerTeam
                ? results.Select(r => r.PlayerTeamState).ToList()
                : results.Select(r => r.EnemyTeamState).ToList();

            AvgSurvivors = (float)teamStates.Average(s => s.SurvivorCount);
            AvgRemainingHp = (float)teamStates.Average(s => s.TotalRemainingHp);

            // HP 비율 계산 (TotalMaxHp가 0인 경우 방지)
            var hpPercentages = teamStates
                .Where(s => s.TotalMaxHp > 0)
                .Select(s => (float)s.TotalRemainingHp / s.TotalMaxHp * 100f);
            AvgHpPercentage = hpPercentages.Any() ? hpPercentages.Average() : 0f;

            MaxSurvivors = teamStates.Max(s => s.SurvivorCount);
            MinSurvivors = teamStates.Min(s => s.SurvivorCount);
        }
    }

    /// <summary>
    /// 몬테카를로 시뮬레이션 결과 통계 (NvM 지원)
    /// </summary>
    public struct MonteCarloReport {
        // 기본 통계
        public int TotalCount;
        public int WinCount;
        public int LoseCount;
        public float WinRate;
        public float AvgTurns;

        // 팀별 상세 통계
        public TeamStatistics PlayerStats;
        public TeamStatistics EnemyStats;

        public MonteCarloReport(List<SimulationResult> results) {
            TotalCount = results.Count;
            WinCount = results.Count(r => r.IsPlayerWin);
            LoseCount = TotalCount - WinCount;
            WinRate = (float)WinCount / TotalCount * 100f;
            AvgTurns = (float)results.Average(r => r.TotalTurns);

            // 팀별 통계 생성
            PlayerStats = new TeamStatistics(results, true);
            EnemyStats = new TeamStatistics(results, false);
        }
    }
}