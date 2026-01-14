using TurnBasedSim.Core;

public class RandomAttackAction : IBattleAction
{
    public string ActionName => "RandomAttack";

    public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
    {
        // 1~3 사이의 랜덤 데미지 (Random.Range의 최대값은 exclusive하므로 4로 설정)
        int damage = UnityEngine.Random.Range(1, 4); 
        defender.CurrentHp -= damage;
    }
}