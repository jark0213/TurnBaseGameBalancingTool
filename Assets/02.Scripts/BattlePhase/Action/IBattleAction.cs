namespace TurnBasedSim.Core {
    public interface IBattleAction {
        string ActionName { get; }
        // 실제 효과를 실행하는 메서드
        void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context);
    }
}