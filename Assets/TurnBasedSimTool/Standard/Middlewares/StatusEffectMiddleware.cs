using System.Linq;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard {
    public class StatusEffectMiddleware : IBattleMiddleware {
        public bool OnPreExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
            // 1. 공격자의 행동 전 상태 처리 (예: 기절 체크, 공격력 버프)
            // 리스트 복사본을 만들어 순회 (순회 중 리스트 수정 대비)
            var effects = attacker.StatusEffects.ToArray();
            foreach (var status in effects) {
                status.OnAction(attacker, context);
            }
            return !attacker.IsDead;
        }

        public void OnPostExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
            // 2. 행동 후 처리 (필요 시 추가)
        }

        // 참고: 만약 턴 시작/종료 시점의 처리가 필요하다면 
        // 이 미들웨어를 TestStartTurnPhase나 TestEndTurnPhase에도 등록해서 사용하면 됩니다.
    }
}