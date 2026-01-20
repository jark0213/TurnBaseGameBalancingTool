using System.Collections.Generic;
using System.Linq;
using TurnBasedSimTool.Core;
using UnityEngine;

namespace TurnBasedSimTool.Standard
{
    /// <summary>
    /// Speed 기반 전투 Phase
    /// 양쪽 팀의 모든 유닛을 Speed 순으로 정렬하여 순차 행동
    /// </summary>
    public class SpeedBasedCombatPhase : BattlePhaseBase
    {
        private ITargetingStrategy _targetingStrategy;
        private SimulationSettings _settings;

        public SpeedBasedCombatPhase(string name, ITargetingStrategy targetingStrategy, SimulationSettings settings)
            : base(name, true) // isPlayerTurn은 의미 없음 (Speed 순서로 결정)
        {
            _targetingStrategy = targetingStrategy ?? new RandomTargeting();
            _settings = settings;
        }

        public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context)
        {
            // NvM 전투만 지원
            if (context.PlayerTeam == null || context.EnemyTeam == null)
                return;

            ExecuteSpeedBasedCombat(context);
        }

        private void ExecuteSpeedBasedCombat(BattleContext context)
        {
            // 1. 모든 유닛을 Speed 순으로 정렬
            var sortedUnits = GetSortedUnits(context);

            // 2. 순서대로 각 유닛 행동
            foreach (var unitInfo in sortedUnits)
            {
                // 죽은 유닛은 스킵
                if (unitInfo.Unit.IsDead)
                    continue;

                // 유닛이 속한 팀 확인
                BattleTeam myTeam = unitInfo.IsPlayerTeam ? context.PlayerTeam : context.EnemyTeam;
                BattleTeam enemyTeam = unitInfo.IsPlayerTeam ? context.EnemyTeam : context.PlayerTeam;

                // 유닛의 액션 실행
                ExecuteUnitActions(unitInfo, myTeam, enemyTeam, context);

                // 전투 종료 체크
                if (context.PlayerTeam.IsDefeated() || context.EnemyTeam.IsDefeated())
                {
                    context.IsFinished = true;
                    return;
                }
            }
        }

        private void ExecuteUnitActions(UnitInfo unitInfo, BattleTeam myTeam, BattleTeam enemyTeam, BattleContext context)
        {
            // 유닛의 인덱스로 액션 찾기
            if (unitInfo.UnitIndex < 0 || unitInfo.UnitIndex >= myTeam.ActionsPerUnit.Count)
                return;

            List<IBattleAction> actions = myTeam.ActionsPerUnit[unitInfo.UnitIndex];

            foreach (var action in actions)
            {
                // 살아있는 적 찾기
                List<IBattleUnit> aliveEnemies = enemyTeam.GetAliveUnits();
                if (aliveEnemies.Count == 0)
                    return;

                // 타겟 선택
                IBattleUnit target = _targetingStrategy.SelectTarget(aliveEnemies, unitInfo.Unit, context);

                if (target != null)
                {
                    // 액션 실행
                    action.Execute(unitInfo.Unit, target, context);

                    // 적 팀 패배 체크
                    if (enemyTeam.IsDefeated())
                        return;
                }
            }
        }

        private List<UnitInfo> GetSortedUnits(BattleContext context)
        {
            List<UnitInfo> allUnits = new List<UnitInfo>();

            // Player 팀 유닛 추가
            for (int i = 0; i < context.PlayerTeam.Units.Count; i++)
            {
                allUnits.Add(new UnitInfo
                {
                    Unit = context.PlayerTeam.Units[i],
                    UnitIndex = i,
                    IsPlayerTeam = true
                });
            }

            // Enemy 팀 유닛 추가
            for (int i = 0; i < context.EnemyTeam.Units.Count; i++)
            {
                allUnits.Add(new UnitInfo
                {
                    Unit = context.EnemyTeam.Units[i],
                    UnitIndex = i,
                    IsPlayerTeam = false
                });
            }

            // Speed 기준 정렬 (높은 Speed가 먼저)
            allUnits.Sort((a, b) =>
            {
                int speedA = GetSpeed(a.Unit);
                int speedB = GetSpeed(b.Unit);

                // Speed 비교
                if (speedA != speedB)
                    return speedB.CompareTo(speedA); // 내림차순

                // Speed 동점 시 타이브레이크
                return ResolveTiebreak(a, b);
            });

            return allUnits;
        }

        private int GetSpeed(IBattleUnit unit)
        {
            if (unit is DefaultUnit defaultUnit)
                return defaultUnit.Speed;

            // CustomData에서 Speed 찾기
            if (unit is DefaultUnit du && du.CustomData.ContainsKey("Speed"))
            {
                if (du.CustomData["Speed"] is int speed)
                    return speed;
            }

            return 10; // 기본값
        }

        private int ResolveTiebreak(UnitInfo a, UnitInfo b)
        {
            switch (_settings.SpeedTiebreak)
            {
                case SpeedTiebreakOption.Random:
                    return Random.Range(0, 2) == 0 ? -1 : 1;

                case SpeedTiebreakOption.PlayerFirst:
                    if (a.IsPlayerTeam && !b.IsPlayerTeam) return -1;
                    if (!a.IsPlayerTeam && b.IsPlayerTeam) return 1;
                    return 0;

                case SpeedTiebreakOption.EnemyFirst:
                    if (a.IsPlayerTeam && !b.IsPlayerTeam) return 1;
                    if (!a.IsPlayerTeam && b.IsPlayerTeam) return -1;
                    return 0;

                case SpeedTiebreakOption.UseStat:
                    return ResolveStatTiebreak(a.Unit, b.Unit);

                default:
                    return 0;
            }
        }

        private int ResolveStatTiebreak(IBattleUnit unitA, IBattleUnit unitB)
        {
            int statA = GetTiebreakStat(unitA);
            int statB = GetTiebreakStat(unitB);

            if (statA != statB)
                return statB.CompareTo(statA); // 높은 스탯이 먼저

            // 그래도 같으면 무작위
            return Random.Range(0, 2) == 0 ? -1 : 1;
        }

        private int GetTiebreakStat(IBattleUnit unit)
        {
            if (!(unit is DefaultUnit defaultUnit))
                return 0;

            switch (_settings.TiebreakStat)
            {
                case TiebreakStatOption.Defense:
                    return defaultUnit.Defense;

                case TiebreakStatOption.Evasion:
                    return defaultUnit.Evasion;

                case TiebreakStatOption.CritRate:
                    return defaultUnit.CritRate;

                case TiebreakStatOption.Speed:
                    return defaultUnit.Speed;

                case TiebreakStatOption.Custom:
                    return GetCustomStat(defaultUnit, _settings.CustomTiebreakStatName);

                default:
                    return 0;
            }
        }

        private int GetCustomStat(DefaultUnit unit, string statName)
        {
            if (string.IsNullOrEmpty(statName))
                return 0;

            // CustomData에서 찾기
            if (unit.CustomData.ContainsKey(statName))
            {
                if (unit.CustomData[statName] is int intValue)
                    return intValue;
            }

            // 속성 리플렉션으로 찾기
            var property = typeof(DefaultUnit).GetProperty(statName);
            if (property != null && property.PropertyType == typeof(int))
            {
                return (int)property.GetValue(unit);
            }

            return 0;
        }

        private class UnitInfo
        {
            public IBattleUnit Unit;
            public int UnitIndex;
            public bool IsPlayerTeam;
        }
    }
}
