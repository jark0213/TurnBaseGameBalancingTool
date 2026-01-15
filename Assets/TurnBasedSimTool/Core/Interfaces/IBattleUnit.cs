using System.Collections.Generic;

namespace TurnBasedSimTool.Core {
    public interface IBattleUnit {
        string Name { get; }
        int MaxHp { get; }
        int CurrentHp { get; set; }
        bool IsDead { get; }
        
        // [추가] 범용 상태 리스트
        List<IStatusEffect> StatusEffects { get; }
        void AddStatus(IStatusEffect effect);
        void RemoveStatus(IStatusEffect effect);

        IBattleUnit Clone();
    }
}