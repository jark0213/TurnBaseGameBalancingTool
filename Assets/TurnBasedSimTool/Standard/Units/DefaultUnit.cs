using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// 확장 가능한 기본 유닛 구현
    ///
    /// 사용 가능한 기능:
    /// - 필수: Name, MaxHp, CurrentHp (IBattleUnit)
    /// - 선택적 스탯: Defense, Evasion, CritRate, CritMultiplier
    /// - 상태 효과: StatusEffects 리스트
    /// - 범용 확장: CustomData Dictionary
    ///
    /// 사용 예시:
    /// <code>
    /// var unit = new DefaultUnit
    /// {
    ///     Name = "Knight",
    ///     MaxHp = 100,
    ///     Defense = 10,
    ///     Evasion = 5
    /// };
    ///
    /// // CustomData 사용 (고급)
    /// unit.CustomData["BleedingCount"] = 3;
    /// unit.CustomData["Speed"] = 10;
    /// </code>
    /// </summary>
    public class DefaultUnit : IBattleUnit
    {
        // === 필수 속성 (IBattleUnit) ===
        public string Name { get; set; }
        public int MaxHp { get; set; }
        private int _currentHp;

        public int CurrentHp
        {
            get => _currentHp;
            set => _currentHp = Mathf.Clamp(value, 0, MaxHp);
        }

        public bool IsDead => CurrentHp <= 0;

        // === 선택적 전투 스탯 ===
        /// <summary>방어력 (사용하지 않으면 0으로 유지)</summary>
        public int Defense { get; set; } = 0;

        /// <summary>회피율 (0-100, 사용하지 않으면 0으로 유지)</summary>
        public int Evasion { get; set; } = 0;

        /// <summary>크리티컬 확률 (0-100, 사용하지 않으면 0으로 유지)</summary>
        public int CritRate { get; set; } = 0;

        /// <summary>크리티컬 배율 (기본 150 = 1.5배)</summary>
        public int CritMultiplier { get; set; } = 150;

        /// <summary>속도 (턴 순서 결정, 기본 10)</summary>
        public int Speed { get; set; } = 10;

        // === 상태 효과 시스템 (선택적) ===
        /// <summary>상태 효과 리스트 (독, 출혈 등)</summary>
        public List<IStatusEffect> StatusEffects { get; private set; } = new List<IStatusEffect>();

        public void AddStatus(IStatusEffect effect)
        {
            StatusEffects.Add(effect);
        }

        public void RemoveStatus(IStatusEffect effect)
        {
            StatusEffects.Remove(effect);
        }

        // === 범용 확장 포인트 (고급) ===
        /// <summary>
        /// 커스텀 데이터 저장소
        /// 게임별로 필요한 추가 속성을 자유롭게 저장
        /// 예: CustomData["Speed"] = 10, CustomData["BleedingCount"] = 3
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();

        // === 몬테카를로 시뮬레이션용 Deep Clone ===
        public IBattleUnit Clone()
        {
            var clone = new DefaultUnit
            {
                // 필수
                Name = this.Name,
                MaxHp = this.MaxHp,
                CurrentHp = this.CurrentHp,

                // 선택적 스탯
                Defense = this.Defense,
                Evasion = this.Evasion,
                CritRate = this.CritRate,
                CritMultiplier = this.CritMultiplier,
                Speed = this.Speed,

                // 상태 효과 (Deep Copy)
                StatusEffects = this.StatusEffects.Select(s => s.Clone()).ToList(),

                // CustomData (Deep Copy)
                CustomData = new Dictionary<string, object>(this.CustomData)
            };
            return clone;
        }
    }
}