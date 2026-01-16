using TurnBasedSimTool.Core;
using UnityEngine;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// 랜덤 범위 데미지 액션 (예: 1-5 데미지)
    /// </summary>
    public class RangedDamageAction : IBattleAction
    {
        public string ActionName { get; set; } = "RangedAttack";
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int Cost { get; set; } = 0;

        public int GetCost(IBattleState state) => Cost;
        public bool CanExecute(IBattleState state) => true;

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 랜덤 데미지 계산 (MinDamage ~ MaxDamage 포함)
            int damage = Random.Range(MinDamage, MaxDamage + 1);
            defender.CurrentHp -= damage;
        }

        public IBattleAction Clone()
        {
            return new RangedDamageAction
            {
                ActionName = this.ActionName,
                MinDamage = this.MinDamage,
                MaxDamage = this.MaxDamage,
                Cost = this.Cost
            };
        }
    }
}
