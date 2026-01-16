using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// 방어력 버프 상태 효과 예제
    ///
    /// 동작:
    /// - DefaultUnit의 Defense 속성을 증가시킴
    /// - 턴 시작 시 지속시간 감소
    /// - 지속시간이 0이 되면 버프 해제
    ///
    /// 사용 예시:
    /// <code>
    /// var unit = new DefaultUnit { Name = "Knight", MaxHp = 100, Defense = 10 };
    /// var defenseBuff = new DefenseBuffEffect(5, 3); // +5 방어력, 3턴 지속
    /// unit.AddStatus(defenseBuff);
    /// </code>
    ///
    /// 주의:
    /// - DefaultUnit을 사용하는 경우에만 Defense 속성 사용 가능
    /// - 다른 유닛 타입에선 CustomData["DefenseBuff"] 같은 방식으로 대체 구현
    /// </summary>
    public class DefenseBuffEffect : IStatusEffect
    {
        public string EffectName => "DefenseBuff";
        public int Duration { get; set; }
        public int BuffAmount { get; private set; }

        private bool _isApplied = false;

        public DefenseBuffEffect(int buffAmount, int duration)
        {
            BuffAmount = buffAmount;
            Duration = duration;
        }

        /// <summary>
        /// 턴 시작 시 호출 - 버프 적용 및 지속시간 감소
        /// </summary>
        public void OnTurnStart(IBattleUnit owner, BattleContext context)
        {
            // 버프 적용 (첫 턴)
            if (!_isApplied)
            {
                ApplyBuff(owner);
            }

            // 지속시간 감소
            if (Duration > 0)
            {
                Duration--;
                Debug.Log($"{owner.Name}의 방어력 버프 지속시간이 1 감소했습니다. (남은 턴: {Duration})");

                if (Duration == 0)
                {
                    RemoveBuff(owner);
                }
            }
        }

        /// <summary>
        /// 액션 시 호출
        /// </summary>
        public void OnAction(IBattleUnit owner, BattleContext context)
        {
            // 방어력 버프는 액션 시 영향 없음
        }

        /// <summary>
        /// 턴 종료 시 호출
        /// </summary>
        public void OnTurnEnd(IBattleUnit owner, BattleContext context)
        {
            // 턴 종료 시 특별한 동작 없음
        }

        private void ApplyBuff(IBattleUnit owner)
        {
            if (_isApplied)
                return;

            // DefaultUnit인 경우 Defense 증가
            if (owner is DefaultUnit defaultUnit)
            {
                defaultUnit.Defense += BuffAmount;
                Debug.Log($"{owner.Name}의 방어력이 {BuffAmount} 증가했습니다! (현재: {defaultUnit.Defense})");
                _isApplied = true;
            }
            else
            {
                Debug.LogWarning($"{owner.Name}은 DefaultUnit이 아니므로 방어력 버프를 적용할 수 없습니다.");
            }
        }

        private void RemoveBuff(IBattleUnit owner)
        {
            if (!_isApplied)
                return;

            // DefaultUnit인 경우 Defense 원상복구
            if (owner is DefaultUnit defaultUnit)
            {
                defaultUnit.Defense -= BuffAmount;
                Debug.Log($"{owner.Name}의 방어력 버프가 종료되었습니다. (현재: {defaultUnit.Defense})");
                _isApplied = false;
            }
        }

        /// <summary>
        /// Deep Clone
        /// </summary>
        public IStatusEffect Clone()
        {
            return new DefenseBuffEffect(this.BuffAmount, this.Duration)
            {
                _isApplied = this._isApplied
            };
        }

        public override string ToString()
        {
            return $"{EffectName}(+{BuffAmount}, {Duration}턴)";
        }
    }
}
