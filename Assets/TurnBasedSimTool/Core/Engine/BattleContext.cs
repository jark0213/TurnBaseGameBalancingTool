using System;

namespace TurnBasedSim.Core {
    public class BattleContext {
        public int CurrentTurn;
        public bool IsFinished;
        public bool PlayerWon;
        public string ResultMessage;

        // [추가] 타겟당 효과 적용 시 발생하는 이벤트
        // <공격자, 타겟, 효과 데이터(데미지 등)>
        public Action<IBattleUnit, IBattleUnit, object> OnEffectApplied;

        // 이벤트를 안전하게 호출하기 위한 헬퍼
        public void TriggerEffectApplied(IBattleUnit attacker, IBattleUnit target, object effectData) {
            OnEffectApplied?.Invoke(attacker, target, effectData);
        }
    }
}