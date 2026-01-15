using System.Collections.Generic;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// 액션 리스트를 순차적으로 실행하는 Phase
    /// UI에서 등록한 액션들을 순서대로 실행합니다
    /// </summary>
    public class ManualActionPhase : ActionPhaseBase
    {
        private List<IBattleAction> _actionList = new List<IBattleAction>();
        private int _currentIndex = 0;

        public ManualActionPhase(string name, bool isPlayer) : base(name, isPlayer) { }

        /// <summary>
        /// UI에서 준비한 액션 리스트를 주입
        /// </summary>
        public void SetActions(List<IBattleAction> actions)
        {
            _actionList = new List<IBattleAction>(actions);
        }

        public override void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            _currentIndex = 0; // 전투 회차마다 인덱스 초기화
            base.Execute(attacker, defender, context);
        }

        protected override bool HasAvailableActions(IBattleUnit unit, BattleContext context)
        {
            return _currentIndex < _actionList.Count;
        }

        protected override IBattleAction GetNextAction(IBattleUnit unit, BattleContext context)
        {
            if (_currentIndex < _actionList.Count)
            {
                return _actionList[_currentIndex++];
            }
            return null;
        }
    }
}