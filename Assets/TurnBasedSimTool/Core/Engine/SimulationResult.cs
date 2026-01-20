using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSimTool.Core {
    /// <summary>
    /// 개별 유닛의 전투 종료 상태
    /// </summary>
    public struct UnitEndState {
        public string UnitName;
        public int FinalHp;
        public int MaxHp;
        public bool IsDead;

        public UnitEndState(string name, int finalHp, int maxHp, bool isDead) {
            UnitName = name;
            FinalHp = finalHp;
            MaxHp = maxHp;
            IsDead = isDead;
        }
    }

    /// <summary>
    /// 팀 전체의 전투 종료 상태
    /// </summary>
    public struct TeamEndState {
        public List<UnitEndState> UnitStates;
        public int SurvivorCount;
        public int TotalRemainingHp;
        public int TotalMaxHp;

        public TeamEndState(List<UnitEndState> unitStates) {
            UnitStates = unitStates ?? new List<UnitEndState>();
            SurvivorCount = UnitStates.Count(u => !u.IsDead);
            TotalRemainingHp = UnitStates.Sum(u => u.FinalHp);
            TotalMaxHp = UnitStates.Sum(u => u.MaxHp);
        }

        public static TeamEndState FromBattleTeam(BattleTeam team) {
            if (team == null || team.Units == null || team.Units.Count == 0) {
                return new TeamEndState(new List<UnitEndState>());
            }

            var unitStates = team.Units.Select(u => new UnitEndState(
                u.Name,
                u.CurrentHp,
                u.MaxHp,
                u.IsDead
            )).ToList();

            return new TeamEndState(unitStates);
        }
    }

    /// <summary>
    /// 시뮬레이션 결과 (1v1 및 NvM 모두 지원)
    /// </summary>
    public struct SimulationResult {
        // 기본 결과 정보
        public bool IsPlayerWin;
        public int TotalTurns;
        public string EndReason; // 예: "Enemy Slain", "Turn Limit Exceeded"

        // 하위 호환성을 위한 1v1 결과
        public int RemainingHp;

        // NvM 결과 (팀 단위)
        public TeamEndState PlayerTeamState;
        public TeamEndState EnemyTeamState;

        // 1v1 생성자 (하위 호환)
        public SimulationResult(bool win, int turns, int hp, string reason) {
            IsPlayerWin = win;
            TotalTurns = turns;
            RemainingHp = hp;
            EndReason = reason;
            PlayerTeamState = new TeamEndState(new List<UnitEndState>());
            EnemyTeamState = new TeamEndState(new List<UnitEndState>());
        }

        // NvM 생성자 (팀 기반)
        public SimulationResult(bool win, int turns, TeamEndState playerState, TeamEndState enemyState, string reason) {
            IsPlayerWin = win;
            TotalTurns = turns;
            RemainingHp = playerState.TotalRemainingHp; // 하위 호환성
            EndReason = reason;
            PlayerTeamState = playerState;
            EnemyTeamState = enemyState;
        }
    }
}