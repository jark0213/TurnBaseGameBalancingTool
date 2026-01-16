using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// 독 상태 효과 예제
    ///
    /// 동작:
    /// - 턴 종료 시 매턴 고정 데미지
    /// - 지속시간 무한 (Duration = -1)
    /// - 제거하려면 명시적으로 RemoveStatus 호출 필요
    ///
    /// 사용 예시:
    /// <code>
    /// var poison = new PoisonStatusEffect(2); // 매 턴 2 데미지 (무한 지속)
    /// unit.AddStatus(poison);
    /// </code>
    /// </summary>
    public class PoisonStatusEffect : IStatusEffect
    {
        public string EffectName => "Poison";
        public int Duration { get; set; }
        public int DamagePerTurn { get; private set; }

        public PoisonStatusEffect(int damagePerTurn, int duration = -1)
        {
            DamagePerTurn = damagePerTurn;
            Duration = duration; // -1 = 무한 지속
        }

        /// <summary>
        /// 턴 시작 시 호출 - 지속시간 감소 (무한이면 유지)
        /// </summary>
        public void OnTurnStart(IBattleUnit owner, BattleContext context)
        {
            if (Duration > 0)
            {
                Duration--;
                Debug.Log($"{owner.Name}의 독 지속시간이 1 감소했습니다. (남은 턴: {Duration})");
            }
        }

        /// <summary>
        /// 액션 시 호출
        /// </summary>
        public void OnAction(IBattleUnit owner, BattleContext context)
        {
            // 독은 액션 시 영향 없음
        }

        /// <summary>
        /// 턴 종료 시 호출 - 독 데미지 적용
        /// </summary>
        public void OnTurnEnd(IBattleUnit owner, BattleContext context)
        {
            if (Duration != 0) // 0이 아니면 적용 (무한 또는 남은 턴이 있음)
            {
                owner.CurrentHp -= DamagePerTurn;
                Debug.Log($"{owner.Name}이(가) 독으로 {DamagePerTurn} 데미지를 받았습니다. (남은 HP: {owner.CurrentHp})");
            }
        }

        /// <summary>
        /// Deep Clone
        /// </summary>
        public IStatusEffect Clone()
        {
            return new PoisonStatusEffect(this.DamagePerTurn, this.Duration);
        }

        public override string ToString()
        {
            string durationText = Duration == -1 ? "무한" : $"{Duration}턴";
            return $"{EffectName}({DamagePerTurn}dmg, {durationText})";
        }
    }
}
