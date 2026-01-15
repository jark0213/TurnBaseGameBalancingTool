using System;
using TurnBasedSimTool.Core.Logic;

namespace TurnBasedSimTool.Core {
    public class BattleContext : IBattleState { // 인터페이스 구현 추가
        public int CurrentTurn;
        public bool IsFinished;
        public bool PlayerWon;
        public string ResultMessage;

        // [범용 제약 수치들 직접 추가]
        public bool UseCostSystem { get; set; } = true;
        public int MaxActionsPerTurn { get; set; } = 1; // 기본은 1턴 1행동
        public ICostHandler Cost { get; set; } 

        // IBattleState 인터페이스 구현 (시뮬레이터가 참조하기 위함)
        int IBattleState.TurnCount => CurrentTurn;
        bool IBattleState.IsBattleOver => IsFinished;

        public Action<IBattleUnit, IBattleUnit, object> OnEffectApplied;

        public void TriggerEffectApplied(IBattleUnit attacker, IBattleUnit target, object effectData) {
            OnEffectApplied?.Invoke(attacker, target, effectData);
        }
    }
}