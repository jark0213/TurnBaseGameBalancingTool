using System;
using System.Collections.Generic;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 랜덤 타겟팅 전략
    /// 살아있는 적들 중에서 무작위로 선택합니다
    /// </summary>
    public class RandomTargeting : ITargetingStrategy
    {
        private Random _random;

        public RandomTargeting()
        {
            _random = new Random();
        }

        /// <summary>
        /// 시드 지정 생성자 (테스트용)
        /// </summary>
        public RandomTargeting(int seed)
        {
            _random = new Random(seed);
        }

        public IBattleUnit SelectTarget(List<IBattleUnit> aliveEnemies, IBattleUnit attacker, BattleContext context)
        {
            if (aliveEnemies == null || aliveEnemies.Count == 0)
            {
                return null;
            }

            // 랜덤 인덱스 선택
            int randomIndex = _random.Next(aliveEnemies.Count);
            return aliveEnemies[randomIndex];
        }
    }
}
