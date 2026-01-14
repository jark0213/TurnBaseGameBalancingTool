using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSim.Core {
    public class MonteCarloRunner {
        private readonly FlexibleBattleSimulator _simulator;

        public MonteCarloRunner(FlexibleBattleSimulator simulator) {
            _simulator = simulator;
        }

        public MonteCarloReport RunSimulation(IBattleUnit player, IBattleUnit enemy, int iterations) {
            List<SimulationResult> results = new List<SimulationResult>(iterations);

            for (int i = 0; i < iterations; i++) {
                // 핵심: 원본 데이터가 변하지 않도록 내부에서 Clone은 시뮬레이터가 처리함
                var result = _simulator.Run(player, enemy);
                results.Add(result);
            }

            return new MonteCarloReport(results);
        }
    }

    // 통계 결과를 담는 구조체
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