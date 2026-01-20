using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 최저 HP 타겟팅 전략
    /// 가장 HP가 낮은 적을 우선 공격합니다 (처치 우선)
    /// </summary>
    public class LowestHpTargeting : ITargetingStrategy
    {
        public IBattleUnit SelectTarget(List<IBattleUnit> aliveEnemies, IBattleUnit attacker, BattleContext context)
        {
            if (aliveEnemies == null || aliveEnemies.Count == 0)
            {
                return null;
            }

            // 현재 HP가 가장 낮은 유닛 선택
            return aliveEnemies.OrderBy(e => e.CurrentHp).First();
        }
    }
}
