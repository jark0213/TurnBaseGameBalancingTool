using System.Collections.Generic;

namespace TurnBasedSimTool.Core {
    public abstract class BattlePhaseBase : IBattlePhase {
        public string PhaseName { get; }
        public bool IsPlayerPhase { get; }

        // 전/후처리 미들웨어 리스트
        protected List<IBattleMiddleware> Middlewares = new List<IBattleMiddleware>();

        protected BattlePhaseBase(string name, bool isPlayer) {
            PhaseName = name;
            IsPlayerPhase = isPlayer;
        }

        public void AddMiddleware(IBattleMiddleware middleware) {
            Middlewares.Add(middleware);
        }

        public abstract void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context);

        // 공격자와 방어자를 IsPlayerPhase에 따라 자동으로 매칭해주는 헬퍼
        protected (IBattleUnit attacker, IBattleUnit defender) GetUnits(IBattleUnit player, IBattleUnit enemy) {
            return IsPlayerPhase ? (player, enemy) : (enemy, player);
        }
    }
}