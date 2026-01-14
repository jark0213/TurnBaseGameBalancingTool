using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedSim.Core;

namespace TurnBasedSim.Standard {
    public class DefaultUnit : IBattleUnit {
        public string Name { get; set; }
        public int MaxHp { get; set; }
        private int _currentHp;
        
        public int CurrentHp {
            get => _currentHp;
            set => _currentHp = Mathf.Clamp(value, 0, MaxHp);
        }

        public bool IsDead => CurrentHp <= 0;

        // 상태 효과 리스트
        public List<IStatusEffect> StatusEffects { get; private set; } = new List<IStatusEffect>();

        public void AddStatus(IStatusEffect effect) {
            StatusEffects.Add(effect);
        }

        public void RemoveStatus(IStatusEffect effect) {
            StatusEffects.Remove(effect);
        }

        // 몬테카를로 시뮬레이션을 위한 핵심: Deep Clone
        public IBattleUnit Clone() {
            var clone = new DefaultUnit {
                Name = this.Name,
                MaxHp = this.MaxHp,
                CurrentHp = this.CurrentHp,
                // 중요: 리스트만 복사하는 게 아니라 내부의 Effect들도 각각 Clone()해야 함
                StatusEffects = this.StatusEffects.Select(s => s.Clone()).ToList()
            };
            return clone;
        }
    }
}