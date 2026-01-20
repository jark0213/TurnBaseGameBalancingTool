using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// Phase 기반 전투 시뮬레이터
    /// 1v1 및 NvM 전투를 지원합니다
    /// </summary>
    public class FlexibleBattleSimulator
    {
        private List<IBattlePhase> _phases = new List<IBattlePhase>();

        public void AddPhase(IBattlePhase phase) => _phases.Add(phase);

        /// <summary>
        /// NvM 전투 시뮬레이션 실행
        /// </summary>
        public SimulationResult RunTeamBattle(BattleTeam playerTeam, BattleTeam enemyTeam, BattleContext context, int maxTurns = 100)
        {
            // 팀 복제 (원본 데이터 보호)
            var pTeam = playerTeam.Clone();
            var eTeam = enemyTeam.Clone();

            // Context에 팀 정보 저장
            context.PlayerTeam = pTeam;
            context.EnemyTeam = eTeam;

            // 초기 세팅
            if (context.UseCostSystem) context.Cost.OnTurnStart();

            while (context.CurrentTurn < maxTurns && !context.IsFinished)
            {
                context.CurrentTurn++;

                // 각 Phase 실행
                foreach (var phase in _phases)
                {
                    phase.Execute(null, null, context); // 팀 전투에서는 context를 통해 접근

                    // 전투 종료 체크 (패배 조건 기반)
                    if (pTeam.IsDefeated() || eTeam.IsDefeated())
                    {
                        context.IsFinished = true;
                        break;
                    }

                    if (context.IsFinished) break;
                }

                // 턴 종료 처리
                if (context.UseCostSystem)
                {
                    context.Cost.OnTurnEnd();
                    context.Cost.OnTurnStart(); // 다음 턴 준비
                }
            }

            // 승패 결정
            context.PlayerWon = !pTeam.IsDefeated();

            // 결과 생성 (플레이어 팀의 총 HP 합산)
            int totalPlayerHp = pTeam.Units.Sum(u => u.CurrentHp);
            return new SimulationResult(context.PlayerWon, context.CurrentTurn, totalPlayerHp, context.ResultMessage);
        }

        /// <summary>
        /// 1v1 전투 시뮬레이션 실행 (하위 호환성)
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