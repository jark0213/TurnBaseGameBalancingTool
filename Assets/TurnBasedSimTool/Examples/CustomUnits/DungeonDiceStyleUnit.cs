using UnityEngine;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Examples
{
    /// <summary>
    /// DungeonDice 스타일 유닛 예제
    ///
    /// 특징:
    /// - 각 상태 이상마다 전용 카운터 필드 (NowBleedingCount, NowPoisonCount 등)
    /// - 명시적인 상태 접근 가능 (unit.NowBleedingCount += 1)
    /// - 각 게임에 특화된 구현 방식
    ///
    /// 장점:
    /// - 코드 자동완성으로 접근 가능
    /// - 타입 안정성 (int만 사용)
    /// - 빠른 성능 (Dictionary 오버헤드 없음)
    ///
    /// 단점:
    /// - 새 상태 추가 시 클래스 수정 필요
    /// - 범용성 낮음 (게임별로 다시 작성)
    ///
    /// 사용 예시:
    /// <code>
    /// var unit = new DungeonDiceStyleUnit
    /// {
    ///     Name = "Warrior",
    ///     MaxHp = 100,
    ///     NowBleedingCount = 3,
    ///     NowPoisonCount = 2
    /// };
    ///
    /// // 출혈 데미지 적용
    /// unit.CurrentHp -= unit.NowBleedingCount;
    /// </code>
    /// </summary>
    public class DungeonDiceStyleUnit : IBattleUnit
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

        // === 게임 특화 상태 카운터 ===
        /// <summary>출혈 카운트 (턴 종료 시 이 값만큼 데미지)</summary>
        public int NowBleedingCount { get; set; } = 0;

        /// <summary>독 카운트 (턴 종료 시 이 값만큼 데미지)</summary>
        public int NowPoisonCount { get; set; } = 0;

        /// <summary>화상 카운트 (턴 종료 시 이 값만큼 데미지)</summary>
        public int NowBurnCount { get; set; } = 0;

        /// <summary>방어력 버프</summary>
        public int DefenseBuffCount { get; set; } = 0;

        /// <summary>공격력 버프</summary>
        public int AttackBuffCount { get; set; } = 0;

        // === 선택적 전투 스탯 ===
        public int Defense { get; set; } = 0;
        public int Evasion { get; set; } = 0;

        /// <summary>
        /// 턴 종료 시 호출 - 모든 상태 이상 데미지 적용
        /// </summary>
        public void ProcessEndOfTurnEffects()
        {
            // 출혈 데미지
            if (NowBleedingCount > 0)
            {
                CurrentHp -= NowBleedingCount;
                Debug.Log($"{Name}이(가) 출혈로 {NowBleedingCount} 데미지를 받았습니다.");
            }

            // 독 데미지
            if (NowPoisonCount > 0)
            {
                CurrentHp -= NowPoisonCount;
                Debug.Log($"{Name}이(가) 독으로 {NowPoisonCount} 데미지를 받았습니다.");
            }

            // 화상 데미지
            if (NowBurnCount > 0)
            {
                CurrentHp -= NowBurnCount;
                Debug.Log($"{Name}이(가) 화상으로 {NowBurnCount} 데미지를 받았습니다.");
            }
        }

        /// <summary>
        /// 턴 시작 시 호출 - 버프/디버프 카운트 감소
        /// </summary>
        public void ProcessStartOfTurnEffects()
        {
            // 출혈 카운트 감소 (예: 매 턴 1씩 감소)
            if (NowBleedingCount > 0)
                NowBleedingCount = Mathf.Max(0, NowBleedingCount - 1);

            // 독 카운트 감소
            if (NowPoisonCount > 0)
                NowPoisonCount = Mathf.Max(0, NowPoisonCount - 1);

            // 화상 카운트 감소
            if (NowBurnCount > 0)
                NowBurnCount = Mathf.Max(0, NowBurnCount - 1);
        }

        // === Deep Clone (몬테카를로 시뮬레이션용) ===
        public IBattleUnit Clone()
        {
            return new DungeonDiceStyleUnit
            {
                Name = this.Name,
                MaxHp = this.MaxHp,
                CurrentHp = this.CurrentHp,
                Defense = this.Defense,
                Evasion = this.Evasion,

                // 상태 카운터 복사
                NowBleedingCount = this.NowBleedingCount,
                NowPoisonCount = this.NowPoisonCount,
                NowBurnCount = this.NowBurnCount,
                DefenseBuffCount = this.DefenseBuffCount,
                AttackBuffCount = this.AttackBuffCount
            };
        }
    }
}
