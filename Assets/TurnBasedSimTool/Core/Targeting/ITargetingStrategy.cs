using System.Collections.Generic;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 타겟 선택 전략 인터페이스
    /// NvM 전투에서 공격자가 여러 적 중 누구를 공격할지 결정합니다
    /// </summary>
    public interface ITargetingStrategy
    {
        /// <summary>
        /// 살아있는 적들 중에서 타겟 선택
        /// </summary>
        /// <param name="aliveEnemies">살아있는 적 목록</param>
        /// <param name="attacker">공격자 (상황에 따라 사용 가능)</param>
        /// <param name="context">전투 컨텍스트 (상황에 따라 사용 가능)</param>
        /// <returns>선택된 타겟</returns>
        IBattleUnit SelectTarget(List<IBattleUnit> aliveEnemies, IBattleUnit attacker, BattleContext context);
    }
}
