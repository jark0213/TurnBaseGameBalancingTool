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
            return $"=== Simulation Results ===\n\n" +
                   $"Total Runs: {report.TotalCount}\n" +
                   $"Wins: {report.WinCount}\n" +
                   $"Win Rate: {report.WinRate:F2}%\n" +
                   $"Avg Turns: {report.AvgTurns:F1}\n";
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
