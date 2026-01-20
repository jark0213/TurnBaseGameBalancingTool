using System.Collections.Generic;
using TurnBasedSimTool.Core;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// NvM 팀 전투용 Phase
    /// 팀의 모든 살아있는 유닛이 순차적으로 액션을 실행합니다
    /// </summary>
    public class TeamActionPhase : BattlePhaseBase
    {
        private ITargetingStrategy _targetingStrategy;
        private bool _isPlayerTeam; // true: Player팀 공격, false: Enemy팀 공격

        public TeamActionPhase(string name, bool isPlayerTeam, ITargetingStrategy targetingStrategy = null)
            : base(name, isPlayerTeam)
        {
            _isPlayerTeam = isPlayerTeam;
            _targetingStrategy = targetingStrategy ?? new RandomTargeting(); // 기본값: 랜덤 타겟팅
        }

        /// <summary>
        /// 타겟팅 전략 변경
        /// </summary>
        public void SetTargetingStrategy(ITargetingStrategy strategy)
        {
            _targetingStrategy = strategy;
        }

        public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context)
        {
            // NvM에서는 Context를 통해 팀 접근
            if (context.PlayerTeam == null || context.EnemyTeam == null)
            {
                // Fallback: 1v1 모드 (하위 호환성)
                ExecuteSingleUnit(player, enemy, context);
                return;
            }

            // NvM 모드: 팀 단위 실행
            ExecuteTeamBattle(context);
        }

        /// <summary>
        /// NvM 팀 전투 실행 (인덱스 기반)
        /// </summary>
        private void ExecuteTeamBattle(BattleContext context)
        {
            // 공격 팀과 방어 팀 결정
            BattleTeam attackerTeam = _isPlayerTeam ? context.PlayerTeam : context.EnemyTeam;
            BattleTeam defenderTeam = _isPlayerTeam ? context.EnemyTeam : context.PlayerTeam;

            // 인덱스 기반으로 유닛과 액션 처리
            for (int i = 0; i < attackerTeam.Units.Count && i < attackerTeam.ActionsPerUnit.Count; i++)
            {
                IBattleUnit attacker = attackerTeam.Units[i];

                // 죽은 유닛은 스킵
                if (attacker.IsDead)
                    continue;

                List<IBattleAction> actions = attackerTeam.ActionsPerUnit[i];

                foreach (var action in actions)
                {
                    // 살아있는 방어자 목록 갱신 (이전 공격으로 죽었을 수 있음)
                    List<IBattleUnit> aliveDefenders = defenderTeam.GetAliveUnits();

                    if (aliveDefenders.Count == 0)
                    {
                        // 방어 팀 전멸 -> 전투 종료
                        return;
                    }

                    // 타겟 선택
                    IBattleUnit target = _targetingStrategy.SelectTarget(aliveDefenders, attacker, context);

                    if (target != null)
                    {
                        // 액션 실행 - 여기서 attacker의 현재 스탯이 참조됨
                        action.Execute(attacker, target, context);

                        // 방어 팀 패배 체크
                        if (defenderTeam.IsDefeated())
                        {
                            return;
                        }
                    }
                }

                // 공격자 팀이 패배했는지 체크 (반격 등으로 죽었을 수 있음)
                if (attackerTeam.IsDefeated())
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 1v1 모드 실행 (하위 호환성) - 사용 안 함
        /// </summary>
        private void ExecuteSingleUnit(IBattleUnit player, IBattleUnit enemy, BattleContext context)
        {
            // 1v1 모드는 더 이상 지원하지 않음
            // NvM 전투만 사용
        }
    }
}
