using System.Collections.Generic;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// Phase 기반 전투 시뮬레이터
    /// Phase들을 순회하며 전투를 진행합니다
    /// </summary>
    public class FlexibleBattleSimulator
    {
        private List<IBattlePhase> _phases = new List<IBattlePhase>();

        public void AddPhase(IBattlePhase phase) => _phases.Add(phase);

        /// <summary>
        /// 전투 시뮬레이션 실행
        /// </summary>
        public SimulationResult Run(IBattleUnit player, IBattleUnit enemy, BattleContext context, int maxTurns = 100) {
            var p = player.Clone();
            var e = enemy.Clone();

            // 초기 세팅
            if (context.UseCostSystem) context.Cost.OnTurnStart();

            while (context.CurrentTurn < maxTurns && !context.IsFinished) {
                context.CurrentTurn++;

                // 각 Phase 실행 (PlayerPhase, EnemyPhase 등)
                foreach (var phase in _phases) {
                    phase.Execute(p, e, context);

                    // 전투 종료 체크
                    if (p.IsDead || e.IsDead) {
                        context.IsFinished = true;
                        break;
                    }

                    if (context.IsFinished) break;
                }

                // 턴 종료 처리
                if (context.UseCostSystem) {
                    context.Cost.OnTurnEnd();
                    context.Cost.OnTurnStart(); // 다음 턴 준비
                }
            }

            context.PlayerWon = !p.IsDead;
            return new SimulationResult(context.PlayerWon, context.CurrentTurn, p.CurrentHp, context.ResultMessage);
        }

        public void ClearPhases() => _phases.Clear();
    }
}