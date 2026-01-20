using System.Collections.Generic;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 첫 번째 타겟팅 전략
    /// 항상 목록의 첫 번째 적을 공격합니다 (순서대로)
    /// </summary>
    public class FirstTargeting : ITargetingStrategy
    {
        public IBattleUnit SelectTarget(List<IBattleUnit> aliveEnemies, IBattleUnit attacker, BattleContext context)
        {
            if (aliveEnemies == null || aliveEnemies.Count == 0)
            {
                return null;
            }

            // 첫 번째 유닛 선택
            return aliveEnemies[0];
        }
    }
}
