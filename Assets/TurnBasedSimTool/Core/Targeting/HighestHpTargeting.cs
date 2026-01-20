using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 최고 HP 타겟팅 전략
    /// 가장 HP가 높은 적을 우선 공격합니다 (탱커 우선)
    /// </summary>
    public class HighestHpTargeting : ITargetingStrategy
    {
        public IBattleUnit SelectTarget(List<IBattleUnit> aliveEnemies, IBattleUnit attacker, BattleContext context)
        {
            if (aliveEnemies == null || aliveEnemies.Count == 0)
            {
                return null;
            }

            // 현재 HP가 가장 높은 유닛 선택
            return aliveEnemies.OrderByDescending(e => e.CurrentHp).First();
        }
    }
}
