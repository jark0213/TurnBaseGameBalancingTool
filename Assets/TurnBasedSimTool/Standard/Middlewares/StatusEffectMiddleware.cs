using System.Linq;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// 상태 효과 미들웨어
    ///
    /// DefaultUnit을 사용하는 경우 상태 효과를 자동으로 처리
    /// 다른 유닛 타입은 무시하고 통과
    /// </summary>
    public class StatusEffectMiddleware : IBattleMiddleware
    {
        public bool OnPreExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // DefaultUnit인 경우에만 상태 효과 처리
            if (attacker is DefaultUnit defaultAttacker)
            {
                // 리스트 복사본을 만들어 순회 (순회 중 리스트 수정 대비)
                var effects = defaultAttacker.StatusEffects.ToArray();
                foreach (var status in effects)
                {
                    status.OnAction(defaultAttacker, context);
                }
            }

            return !attacker.IsDead;
        }

        public void OnPostExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 행동 후 처리 (필요 시 추가)
        }
    }
}