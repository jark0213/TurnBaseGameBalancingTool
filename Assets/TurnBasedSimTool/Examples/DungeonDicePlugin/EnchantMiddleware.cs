using TurnBasedSim.Core;

public class EnchantMiddleware : IBattleMiddleware {
    public bool OnPreExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        // 이펙트가 타겟에 박힐 때마다 실행될 함수 등록
        context.OnEffectApplied += TriggerEnchantLogic;
        return true;
    }

    private void TriggerEnchantLogic(IBattleUnit attacker, IBattleUnit target, object effectData) {
        // 여기서 실제 던전다이스의 코드를 호출!
        // 예: EnchantSystem.Process(attacker, target, effectData);
    }

    public void OnPostExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
        // 메모리 누수 방지를 위한 해제
        context.OnEffectApplied -= TriggerEnchantLogic;
    }
}