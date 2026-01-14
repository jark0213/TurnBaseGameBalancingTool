using TurnBasedSim.Core;

public abstract class ActionPhaseBase : BattlePhaseBase 
{
    protected ActionPhaseBase(string name, bool isPlayer) : base(name, isPlayer) { }

    public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context) {
        var (attacker, defender) = GetUnits(player, enemy);

        // 1. 페이즈 전처리
        foreach (var mw in Middlewares) {
            if (!mw.OnPreExecute(attacker, defender, context)) return;
        }

        // 2. 액션 루프 (주사위나 카드 한 장 단위)
        while (HasAvailableActions(attacker, context)) {
            IBattleAction action = GetNextAction(attacker, context);
            
            // 액션 실행
            action.Execute(attacker, defender, context);

            if (defender.IsDead) break;
        }

        // 3. 페이즈 후처리
        foreach (var mw in Middlewares) {
            mw.OnPostExecute(attacker, defender, context);
        }
    }

    // [중요] 툴 사용자가 액션 내부에서 타겟에게 효과를 줄 때 호출해야 하는 표준 메서드
    protected void ApplyEffect(IBattleUnit attacker, IBattleUnit target, object effectData, BattleContext context) {
        // 여기에 타겟당 전처리(예: 보호막 감쇄)를 넣을 수 있습니다.

        // 실제 효과 적용 (이 부분은 나중에 IEffect 등과 연결)
        // ...
        
        // 타겟당 후처리 알림 (인챈트 트리거가 이 신호를 듣고 작동함)
        context.TriggerEffectApplied(attacker, target, effectData);
    }

    protected abstract bool HasAvailableActions(IBattleUnit unit, BattleContext context);
    protected abstract IBattleAction GetNextAction(IBattleUnit unit, BattleContext context);
}