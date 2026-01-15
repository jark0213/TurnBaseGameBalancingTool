using System.Collections.Generic;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Examples.Tests
{
    public class TestActionPhase : ActionPhaseBase
    {
        // 시뮬레이터 툴에서 동적으로 담을 액션 리스트
        private readonly List<IBattleAction> _actions = new List<IBattleAction>();
        private int _actionIndex = 0;

        public TestActionPhase(string name, bool isPlayer) : base(name, isPlayer) { }

        // 외부(SimUIManager)에서 액션을 주입하기 위한 메서드
        public void AddAction(IBattleAction action) => _actions.Add(action);

        // 페이즈 시작 시 인덱스 초기화
        public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context)
        {
            _actionIndex = 0;
            base.Execute(player, enemy, context);
        }

        protected override bool HasAvailableActions(IBattleUnit unit, BattleContext context)
        {
            return _actionIndex < _actions.Count;
        }

        protected override IBattleAction GetNextAction(IBattleUnit unit, BattleContext context)
        {
            if (_actionIndex < _actions.Count)
            {
                return _actions[_actionIndex++];
            }
            return null;
        }
    }
}
