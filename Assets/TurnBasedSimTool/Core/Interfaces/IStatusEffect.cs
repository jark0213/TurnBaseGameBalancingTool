// Core/Interfaces/IStatusEffect.cs
namespace TurnBasedSim.Core {
    public interface IStatusEffect {
        string EffectName { get; }
        int Duration { get; set; }
        
        void OnTurnStart(IBattleUnit owner, BattleContext context);
        void OnAction(IBattleUnit owner, BattleContext context);
        void OnTurnEnd(IBattleUnit owner, BattleContext context);

        IStatusEffect Clone();
    }
}