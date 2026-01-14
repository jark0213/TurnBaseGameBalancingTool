using TurnBasedSim.Core;

public class StatusEffectMiddleware : IBattleMiddleware {
    public bool OnPreExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        // 공격자가 행동하기 전, 가지고 있는 모든 상태 효과의 'OnAction' 실행
        // (예: 여기서 공격력 버프 계산이나 화상 데미지 처리)
        foreach (var status in attacker.StatusEffects) {
            status.OnAction(attacker, context);
        }
        
        return !attacker.IsDead; // 행동 전 데미지로 죽었다면 행동 취소
    }

    public void OnPostExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        // 행동 후 처리 (예: 공격 시마다 스택 감소 등)
    }
}