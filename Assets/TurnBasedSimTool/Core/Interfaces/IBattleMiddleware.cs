namespace TurnBasedSim.Core {
    public interface IBattleMiddleware {
        // 미들웨어를 만들고 행동 전후로 
        
        // 행동 전 실행: false를 반환하면 해당 행동(Phase)을 스킵함
        bool OnPreExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context);
        // 행동 후 실행: 인챈트 트리거, 반격, 로그 기록 등
        void OnPostExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context);
    }
}