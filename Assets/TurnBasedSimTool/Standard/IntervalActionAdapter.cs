using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// N턴마다 1번 실행되는 액션 어댑터
    /// 예: interval=2이면 2턴마다 한 번 실행
    /// </summary>
    public class IntervalActionAdapter : IBattleAction
    {
        private readonly IBattleAction _baseAction;
        private readonly int _interval;
        private int _currentTurnCount = 0;

        public string ActionName => _baseAction.ActionName;

        public IntervalActionAdapter(IBattleAction baseAction, int interval)
        {
            _baseAction = baseAction;
            _interval = interval < 1 ? 1 : interval;
        }

        public int GetCost(IBattleState state) => _baseAction.GetCost(state);
        public bool CanExecute(IBattleState state) => _baseAction.CanExecute(state);

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            _currentTurnCount++;

            // 인터벌 주기에 도달했을 때만 실제 액션 실행
            if (_currentTurnCount >= _interval)
            {
                _baseAction.Execute(attacker, defender, context);
                _currentTurnCount = 0; // 카운트 초기화
            }
        }

        /// <summary>
        /// 몬테카를로 시뮬레이션을 위한 Clone (상태 초기화)
        /// </summary>
        public IBattleAction Clone()
        {
            return new IntervalActionAdapter(_baseAction.Clone(), _interval);
        }
    }
}