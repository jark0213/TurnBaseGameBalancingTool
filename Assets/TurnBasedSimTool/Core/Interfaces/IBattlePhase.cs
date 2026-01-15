namespace TurnBasedSimTool.Core {
    public interface IBattlePhase {
        string PhaseName { get; }
        void Execute(IBattleUnit p, IBattleUnit e, BattleContext context);
    }
}