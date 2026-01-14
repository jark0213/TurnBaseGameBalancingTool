using TurnBasedSim.Core;

public class TestActionPhase : ActionPhaseBase
{
    private bool _hasAttacked = false;

    public TestActionPhase(string name, bool isPlayer) : base(name, isPlayer) { }

    // 페이즈가 시작될 때마다 공격 여부를 초기화해야 합니다.
    public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context)
    {
        _hasAttacked = false; // 공격 기회 초기화
        base.Execute(player, enemy, context);
    }

    protected override bool HasAvailableActions(IBattleUnit unit, BattleContext context)
    {
        // 턴당 딱 한 번만 공격하고 루프를 빠져나가게 함
        return !_hasAttacked;
    }

    protected override IBattleAction GetNextAction(IBattleUnit unit, BattleContext context)
    {
        _hasAttacked = true;
    
        // 50% 확률로 강타, 50% 확률로 흡혈 사용 (랜덤 패턴 테스트)
        if (UnityEngine.Random.value > 0.5f)
            return new SmashAction();
        else
            return new DrainAction();
    }
}

// 테스트를 위한 간단한 공격 액션 클래스
public class SimpleAttackAction : IBattleAction
{
    public string ActionName => "BasicAttack";

    public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
    {
        // 실제로 상대방의 피를 깎음
        int damage = 2; 
        defender.CurrentHp -= damage;
        
        // 유니티 콘솔에서 확인하고 싶다면 (10,000번 돌릴 땐 주석 처리 권장)
        // UnityEngine.Debug.Log($"{attacker.Name}이 {defender.Name}에게 {damage} 데미지를 입힘!");
    }
}