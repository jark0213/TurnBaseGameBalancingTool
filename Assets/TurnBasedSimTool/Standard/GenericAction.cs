using TurnBasedSim.Core;

public class GenericAction : IBattleAction
{
    public string ActionName { get; set; } = "GenericAttack";
    public int Damage { get; set; }

    public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
    {
        // 원본 SimpleAttackAction의 로직과 동일하게 구현
        defender.CurrentHp -= Damage;
    }
}