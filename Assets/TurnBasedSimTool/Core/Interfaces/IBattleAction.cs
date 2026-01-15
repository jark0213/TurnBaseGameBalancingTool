namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 전투 행동을 나타내는 인터페이스
    /// 외부 이펙트 시스템 연동을 위해서는 ExternalEffectAdapter를 상속받으세요
    /// </summary>
    public interface IBattleAction
    {
        string ActionName { get; }

        /// <summary>
        /// 코스트 시스템에서 사용할 비용 반환 (코스트 미사용 시 0 반환)
        /// </summary>
        int GetCost(IBattleState state);

        /// <summary>
        /// 현재 상태에서 이 액션을 실행 가능한지 체크
        /// </summary>
        bool CanExecute(IBattleState state);

        /// <summary>
        /// 액션 실행
        /// </summary>
        void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context);

        /// <summary>
        /// 몬테카를로 시뮬레이션을 위한 깊은 복사
        /// 상태를 가진 액션은 반드시 새 인스턴스를 반환해야 함
        /// </summary>
        IBattleAction Clone();
    }
}