using UnityEngine;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 시뮬레이션 설정 데이터
    /// UI에서 설정한 값들을 담아 MonteCarloRunner에 전달합니다
    /// </summary>
    [System.Serializable]
    public class SimulationSettings
    {
        public int Iterations = 1000;           // 시뮬레이션 반복 횟수
        public int MaxTurns = 100;              // 최대 턴 수
        public int MaxActionsPerTurn = 1;       // 턴당 최대 행동 횟수
        public bool UseCostSystem = true;       // 코스트 시스템 사용 여부
        public int MaxCost = 3;                 // 최대 코스트
        public int RecoveryAmount = 3;          // 턴 시작 시 회복량
        public bool UseSpeedSystem = false;     // 속도 기반 턴 순서 사용 여부
        public FirstTurnOption FirstTurn = FirstTurnOption.PlayerFirst; // 선공권 (Speed OFF 시)
        
        // Speed 타이브레이크 (Speed 동점 시 순서 결정)
        public SpeedTiebreakOption SpeedTiebreak = SpeedTiebreakOption.Random;
        public TiebreakStatOption TiebreakStat = TiebreakStatOption.Defense;
        public string CustomTiebreakStatName = ""; // TiebreakStat이 Custom일 때 사용

        // JSON 직렬화
        public string ToJson() => JsonUtility.ToJson(this, true);
        public static SimulationSettings FromJson(string json) => JsonUtility.FromJson<SimulationSettings>(json);
    }

    /// <summary>
    /// 선공권 옵션 (Speed System이 OFF일 때)
    /// </summary>
    public enum FirstTurnOption
    {
        PlayerFirst = 0,  // 플레이어 선공
        EnemyFirst = 1,   // 적 선공
        Random = 2        // 무작위 (50/50)
    }
}