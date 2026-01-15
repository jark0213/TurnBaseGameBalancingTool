using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Examples.Tests
{
    public class SmashAction : IBattleAction
    {
        public string ActionName => "SmashAttack";
        public int GetCost(IBattleState state) => 0;
        public bool CanExecute(IBattleState state) => true;

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            defender.CurrentHp -= 5;
            attacker.CurrentHp -= 1; // 반동 데미지
        }

        public IBattleAction Clone() => new SmashAction();
    }
}
