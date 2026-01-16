using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// 회피 카운트 상태 효과 예제
    ///
    /// 동작:
    /// - 확률이 아닌 무조건 회피
    /// - 회피 시 카운트 1 감소
    /// - 카운트가 0이 되면 자동 제거
    ///
    /// 사용 예시:
    /// <code>
    /// var dodge = new DodgeChargesEffect(2); // 2회 무조건 회피
    /// unit.AddStatus(dodge);
    /// </code>
    ///
    /// 주의:
    /// - 이 예제는 개념 설명용입니다
    /// - 실제 회피 처리는 IBattleAction이나 IBattleMiddleware에서 구현 필요
    /// - 회피 시 ConsumeCharge() 메서드를 외부에서 호출해야 함
    /// </summary>
    public class DodgeChargesEffect : IStatusEffect
    {
        public string EffectName => "DodgeCharges";
        public int Duration { get; set; }
        public int Charges { get; set; }

        /// <param name="charges">회피 가능 횟수</param>
        public DodgeChargesEffect(int charges)
        {
            Charges = charges;
            Duration = -1; // 무한 지속 (카운트로만 소진)
        }

        /// <summary>
        /// 턴 시작 시 호출
        /// </summary>
        public void OnTurnStart(IBattleUnit owner, BattleContext context)
        {
            // 회피 카운트는 턴과 무관
        }

        /// <summary>
        /// 액션 시 호출
        /// </summary>
        public void OnAction(IBattleUnit owner, BattleContext context)
        {
            // 회피 카운트는 액션 시 영향 없음
        }

        /// <summary>
        /// 턴 종료 시 호출
        /// </summary>
        public void OnTurnEnd(IBattleUnit owner, BattleContext context)
        {
            // 회피 카운트는 턴 종료 시 영향 없음
        }

        /// <summary>
        /// 회피 가능 여부 확인
        /// </summary>
        public bool CanDodge()
        {
            return Charges > 0;
        }

        /// <summary>
        /// 회피 카운트 소모 (외부에서 호출)
        /// </summary>
        /// <returns>회피 성공 여부</returns>
        public bool ConsumeCharge(IBattleUnit owner)
        {
            if (Charges <= 0)
                return false;

            Charges--;
            Debug.Log($"{owner.Name}이(가) 공격을 회피했습니다! (남은 회피 횟수: {Charges})");

            // 카운트 소진 시 즉시 제거 플래그
            if (Charges == 0)
            {
                Duration = 0;
                Debug.Log($"{owner.Name}의 회피 카운트가 모두 소진되었습니다.");
            }

            return true;
        }

        /// <summary>
        /// Deep Clone
        /// </summary>
        public IStatusEffect Clone()
        {
            return new DodgeChargesEffect(this.Charges)
            {
                Duration = this.Duration
            };
        }

        public override string ToString()
        {
            return $"{EffectName}(x{Charges})";
        }
    }
}
