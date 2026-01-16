using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// 쉴드 상태 효과 예제 (슬레이 더 스파이어 Block 방식)
    ///
    /// 동작:
    /// - 피격 시 HP 대신 쉴드가 먼저 데미지 흡수
    /// - 쉬드가 0이 되면 자동 제거
    /// - Duration으로 지속 시간 제어:
    ///   - Duration = 1: 한 턴만 (슬더스 Block)
    ///   - Duration = -1: 전투 동안 유지 (데미지로만 소진)
    ///   - Duration = 3: 3턴 지속 OR 데미지로 소진
    ///
    /// 사용 예시:
    /// <code>
    /// // 슬더스 Block 방식 (1턴만)
    /// var block = new ShieldEffect(10, 1);
    /// unit.AddStatus(block);
    ///
    /// // 전투 동안 유지되는 쉴드
    /// var permanentShield = new ShieldEffect(50, -1);
    /// unit.AddStatus(permanentShield);
    /// </code>
    ///
    /// 주의:
    /// - 이 예제는 개념 설명용입니다
    /// - 실제 데미지 감소 로직은 IBattleAction이나 BattleSimulator에서 구현 필요
    /// - OnBeforeDefend 같은 훅이 필요하면 IBattleMiddleware 사용
    /// </summary>
    public class ShieldEffect : IStatusEffect
    {
        public string EffectName => "Shield";
        public int Duration { get; set; }
        public int ShieldAmount { get; set; }

        private int _initialAmount;

        /// <param name="shieldAmount">흡수할 데미지 총량</param>
        /// <param name="duration">지속 시간 (1=한턴, -1=무한, 3=3턴 등)</param>
        public ShieldEffect(int shieldAmount, int duration)
        {
            ShieldAmount = shieldAmount;
            _initialAmount = shieldAmount;
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
                Debug.Log($"{owner.Name}의 쉴드 지속시간이 1 감소했습니다. (남은 턴: {Duration}, 쉴드: {ShieldAmount})");

                // 슬더스 Block 방식: 턴 종료 시 쉴드 0으로 리셋
                if (Duration == 0)
                {
                    Debug.Log($"{owner.Name}의 쉴드가 만료되었습니다.");
                }
            }
        }

        /// <summary>
        /// 액션 시 호출
        /// </summary>
        public void OnAction(IBattleUnit owner, BattleContext context)
        {
            // 쉴드는 액션 시 영향 없음
        }

        /// <summary>
        /// 턴 종료 시 호출
        /// </summary>
        public void OnTurnEnd(IBattleUnit owner, BattleContext context)
        {
            // 턴 종료 시 특별한 동작 없음
            // (슬더스에서 Block을 턴 종료 시 제거하려면 여기서 Duration = 0 설정)
        }

        /// <summary>
        /// 데미지 흡수 메서드 (외부에서 호출)
        /// </summary>
        /// <param name="incomingDamage">받을 데미지</param>
        /// <returns>쉴드를 뚫고 나온 남은 데미지</returns>
        public int AbsorbDamage(int incomingDamage)
        {
            if (ShieldAmount <= 0)
                return incomingDamage;

            int absorbed = Mathf.Min(ShieldAmount, incomingDamage);
            ShieldAmount -= absorbed;
            int remainingDamage = incomingDamage - absorbed;

            Debug.Log($"쉴드가 {absorbed} 데미지를 흡수했습니다. (남은 쉴드: {ShieldAmount}, 관통 데미지: {remainingDamage})");

            // 쉴드 소진 시 즉시 제거 플래그
            if (ShieldAmount <= 0)
            {
                Duration = 0;
                Debug.Log("쉴드가 완전히 소진되었습니다.");
            }

            return remainingDamage;
        }

        /// <summary>
        /// Deep Clone
        /// </summary>
        public IStatusEffect Clone()
        {
            return new ShieldEffect(this.ShieldAmount, this.Duration)
            {
                _initialAmount = this._initialAmount
            };
        }

        public override string ToString()
        {
            string durationText = Duration == -1 ? "무한" : Duration == 0 ? "만료" : $"{Duration}턴";
            return $"{EffectName}({ShieldAmount}/{_initialAmount}, {durationText})";
        }
    }
}
