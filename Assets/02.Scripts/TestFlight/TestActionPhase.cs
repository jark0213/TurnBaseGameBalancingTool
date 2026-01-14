using UnityEngine;

namespace TurnBasedSim.Core {
    public class TestActionPhase : BattlePhaseBase {
        public TestActionPhase(string name, bool isPlayer) : base(name, isPlayer) { }

        // 액션단위 실행으로 변경해야됨
        public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context) {
            var (attacker, defender) = GetUnits(player, enemy);

            // 슬스파나 던전다이스처럼 "할 수 있는 행동"이 없을 때까지 반복
            /*while (HasAvailableActions(attacker)) {
                // 1. 구체적인 액션 결정 (카드 선택 혹은 주사위 결과)
                var action = GetNextAction(attacker); 

                // 2. 이 액션에 대한 전처리 (미들웨어)
                foreach (var mw in Middlewares) mw.OnPreExecute(attacker, defender, context);

                // 3. 액션 실행 (실제 데미지 발생)
                action.Execute(attacker, defender);

                // 4. 이 액션에 대한 후처리 (미들웨어 - 인챈트 트리거 등)
                foreach (var mw in Middlewares) mw.OnPostExecute(attacker, defender, context);
        
                // 사망 체크 등 흐름 제어
                if (defender.IsDead) break;
            }*/
        }
    }
}