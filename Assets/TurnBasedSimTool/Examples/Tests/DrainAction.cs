using TurnBasedSim.Core;

public class DrainAction : IBattleAction
{
    public string ActionName => "DrainAttack";

    public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        defender.CurrentHp -= 2;
        attacker.CurrentHp += 2;
    }
}