using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// 기본 데미지 액션
    /// </summary>
    public class GenericAction : IBattleAction
    {
        public string ActionName { get; set; } = "GenericAttack";
        public int Damage { get; set; }

        public int GetCost(IBattleState state) => 0; // 기본은 코스트 없음
        public bool CanExecute(IBattleState state) => true;

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            defender.CurrentHp -= Damage;
        }

        public IBattleAction Clone()
        {
            return new GenericAction
            {
                ActionName = this.ActionName,
                Damage = this.Damage
            };
        }
    }
}