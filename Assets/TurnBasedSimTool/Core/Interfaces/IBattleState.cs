using TurnBasedSimTool.Core.Logic;

namespace TurnBasedSimTool.Core
{
    public interface IBattleState
    {
        // 시스템 설정
        bool UseCostSystem { get; }
        int MaxActionsPerTurn { get; } // 한 턴 최대 행동 횟수 (1이면 JRPG, 99면 무한)
        
        // 전투 상태
        int TurnCount { get; }
        bool IsBattleOver { get; } // 적 혹은 아군의 HP가 0인지 체크
        
        ICostHandler Cost { get; }
    }
}