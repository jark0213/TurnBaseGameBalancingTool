namespace TurnBasedSim.Core {
    public interface IBattlePhase {
        string PhaseName { get; }
        // 이 단계에서 어떤 유닛이 행동하는지, 혹은 시스템적인 처리인지 정의
        void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context);
    }
}