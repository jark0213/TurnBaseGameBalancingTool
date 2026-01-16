namespace TurnBasedSim.Standard
{
    using System.Collections.Generic;
    using System.Linq;
    using TurnBasedSimTool.Core;

    // 수천 번의 SimulationResult를 모아서 통계를 내는 클래스
    public class SimulationSummary
    {
        public int TotalCount;
        public int WinCount;
        public float WinRate => (float)WinCount / TotalCount * 100f;
        
        public float AvgTurns;
        public float AvgRemainingHp;
        public Dictionary<string, int> ReasonStats = new Dictionary<string, int>();

        public SimulationSummary(List<SimulationResult> results)
        {
            TotalCount = results.Count;
            WinCount = results.Count(r => r.IsPlayerWin);
            AvgTurns = (float)results.Average(r => r.TotalTurns);
            AvgRemainingHp = (float)results.Average(r => r.RemainingHp);

            // 종료 사유별 통계 (예: 타임아웃으로 끝난 판이 얼마나 되는지)
            foreach (var r in results)
            {
                if (!ReasonStats.ContainsKey(r.EndReason)) ReasonStats[r.EndReason] = 0;
                ReasonStats[r.EndReason]++;
            }
        }
    }
}