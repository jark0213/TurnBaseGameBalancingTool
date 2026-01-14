namespace TurnBasedSim.Core 
{
    // 이걸 던전 다이스에서 연결해서 사용하려면 IEffect를 통째로 감싸서 Execute시키는 어댑터패턴을 쓰는 클래스를 하나 만들어서 쓰거나 해야할듯?
    // 아래같은 느낌으로
    /*
     public class EffectActionAdapter : IBattleAction 
     {
        private IEffect _compiledEffect;

        public EffectActionAdapter(string dsl) {
            // 이미 있는 CompiledEffectCache와 DSLEffectParser를 활용해
            // 시뮬레이션 시작 시 딱 한 번만 파싱해서 캐싱해둡니다.
            _compiledEffect = CompiledEffectCache.GetOrParse(dsl);
        }

        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context) {
            // 기존 던전 다이스의 로직을 그대로 clone해서 사용
            // DeepCloner 던전다이스에서 이미 사용중이기 때문에 큰 문제는 없을거라 생각됨
            var runEffect = EffectDeepCloner.Clone(_compiledEffect);
            runEffect.Execute(attacker, defender);
        }
    }
     */
    public interface IBattleAction 
    {
        string ActionName { get; }
        // 실제 효과를 실행하는 메서드
        void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context);
    }
}