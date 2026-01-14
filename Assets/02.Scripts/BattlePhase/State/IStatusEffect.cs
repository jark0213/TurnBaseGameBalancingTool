namespace TurnBasedSim.Core {
    public interface IStatusEffect {
        string EffectName { get; }
        int Duration { get; set; }
        
        IStatusEffect Clone();
        // 상태 이상이 유닛에게 미치는 영향 (미들웨어에서 호출할 지점들)
        void OnTurnStart(IBattleUnit owner, BattleContext context);
        void OnAction(IBattleUnit owner, BattleContext context);
        void OnTurnEnd(IBattleUnit owner, BattleContext context);
    }
}