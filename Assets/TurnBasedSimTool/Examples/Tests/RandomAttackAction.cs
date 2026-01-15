using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Examples.Tests
{
    public class RandomAttackAction : IBattleAction
    {
        public string ActionName => "RandomAttack";
        public int GetCost(IBattleState state) => 0;
        public bool CanExecute(IBattleState state) => true;

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 1~3 사이의 랜덤 데미지
            int damage = UnityEngine.Random.Range(1, 4);
            defender.CurrentHp -= damage;
        }

        public IBattleAction Clone() => new RandomAttackAction();
    }
}
