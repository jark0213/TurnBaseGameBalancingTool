using TurnBasedSim.Core;

public class SmashAction : IBattleAction
{
    public string ActionName => "SmashAttck";

    public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        defender.CurrentHp -= 5;
        attacker.CurrentHp -= 1; // 반동 데미지
    }
}