using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Examples.Tests
{
    public class DrainAction : IBattleAction
    {
        public string ActionName => "DrainAttack";
        public int GetCost(IBattleState state) => 0;
        public bool CanExecute(IBattleState state) => true;

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            defender.CurrentHp -= 2;
            attacker.CurrentHp += 2;
        }

        public IBattleAction Clone() => new DrainAction();
    }
}
