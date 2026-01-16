using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// 출혈 상태 효과 예제
    ///
    /// 동작:
    /// - 턴 종료 시 매턴 고정 데미지
    /// - 매 턴 시작 시 지속시간 감소
    ///
    /// 사용 예시:
    /// <code>
    /// var bleeding = new BleedingStatusEffect(3, 5); // 매턴 3 데미지, 5턴 지속
    /// unit.AddStatus(bleeding);
    /// </code>
    /// </summary>
    public class BleedingStatusEffect : IStatusEffect
    {
        public string EffectName => "Bleeding";
        public int Duration { get; set; }
        public int DamagePerTurn { get; private set; }

        public BleedingStatusEffect(int damagePerTurn, int duration)
        {
            DamagePerTurn = damagePerTurn;
            Duration = duration;
        }

        /// <summary>
        /// 턴 시작 시 호출 - 지속시간 감소
        /// </summary>
        public void OnTurnStart(IBattleUnit owner, BattleContext context)
        {
            if (Duration > 0)
            {
                Duration--;
                Debug.Log($"{owner.Name}의 출혈 지속시간이 1 감소했습니다. (남은 턴: {Duration})");
            }
        }

        /// <summary>
        /// 액션 시 호출
        /// </summary>
        public void OnAction(IBattleUnit owner, BattleContext context)
        {
            // 출혈은 액션 시 영향 없음
        }

        /// <summary>
        /// 턴 종료 시 호출 - 출혈 데미지 적용
        /// </summary>
        public void OnTurnEnd(IBattleUnit owner, BattleContext context)
        {
            if (Duration > 0)
            {
                owner.CurrentHp -= DamagePerTurn;
                Debug.Log($"{owner.Name}이(가) 출혈로 {DamagePerTurn} 데미지를 받았습니다. (남은 HP: {owner.CurrentHp})");
            }
        }

        /// <summary>
        /// Deep Clone
        /// </summary>
        public IStatusEffect Clone()
        {
            return new BleedingStatusEffect(this.DamagePerTurn, this.Duration);
        }

        public override string ToString()
        {
            return $"{EffectName}({DamagePerTurn}dmg, {Duration}턴)";
        }
    }
}
