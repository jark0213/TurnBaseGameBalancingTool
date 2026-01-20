using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 시뮬레이션 결과 표시 패널
    /// 승률, 평균 턴 등 통계 표시
    /// </summary>
    public class SimulationResultPanel : MonoBehaviour
    {
        [Header("Result Display")]
        [SerializeField] private TextMeshProUGUI resultText;

        [Header("Actions (Optional)")]
        [SerializeField] private Button exportButton;
        [SerializeField] private Button clearButton;

        private void Start()
        {
            if (clearButton)
            {
                clearButton.onClick.RemoveAllListeners();
                clearButton.onClick.AddListener(Clear);
            }

            Clear();
        }

        /// <summary>
        /// 시뮬레이션 결과 표시
        /// </summary>
        public void DisplayResult(MonteCarloReport report)
        {
            if (resultText)
            {
                resultText.text = FormatResult(report);
            }
        }

        /// <summary>
        /// 결과를 포맷팅된 문자열로 변환
        /// </summary>
        private string FormatResult(MonteCarloReport report)
        {
            string result = "=== Simulation Results ===\n\n";

            // 기본 통계
            result += $"<b>전체 통계</b>\n";
            result += $"  총 시행 횟수: {report.TotalCount}\n";
            result += $"  승리: {report.WinCount} | 패배: {report.LoseCount}\n";
            result += $"  승률: <color={(report.WinRate >= 50 ? "#00FF00" : "#FF6666")}>{report.WinRate:F1}%</color>\n";
            result += $"  평균 턴 수: {report.AvgTurns:F1}\n\n";

            // Player 팀 통계
            result += $"<b><color=#4DA6FF>플레이어 팀 통계</color></b>\n";
            result += FormatTeamStats(report.PlayerStats);
            result += "\n";

            // Enemy 팀 통계
            result += $"<b><color=#FF6666>적 팀 통계</color></b>\n";
            result += FormatTeamStats(report.EnemyStats);

            return result;
        }

        /// <summary>
        /// 팀 통계를 포맷팅
        /// </summary>
        private string FormatTeamStats(TeamStatistics stats)
        {
            string result = "";
            result += $"  평균 생존자: {stats.AvgSurvivors:F2}명\n";
            result += $"  생존자 범위: {stats.MinSurvivors} ~ {stats.MaxSurvivors}명\n";
            result += $"  평균 잔여 HP: {stats.AvgRemainingHp:F1}\n";
            result += $"  평균 HP 비율: {stats.AvgHpPercentage:F1}%\n";
            return result;
        }

        /// <summary>
        /// 결과 텍스트 초기화
        /// </summary>
        public void Clear()
        {
            if (resultText)
            {
                resultText.text = "Run simulation to see results...";
            }
        }

        /// <summary>
        /// CSV 파일로 내보내기 (향후 확장)
        /// </summary>
        public void ExportToCSV(MonteCarloReport report)
        {
            // TODO: 향후 구현
            Debug.Log("CSV Export: Not implemented yet");
        }
    }
}
