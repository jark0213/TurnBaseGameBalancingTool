using System.Collections.Generic;
using System.Linq;

namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// NvM 전투를 위한 팀 데이터 구조
    /// 여러 유닛과 패배 조건을 관리합니다
    /// </summary>
    public class BattleTeam
    {
        /// <summary>
        /// 팀에 속한 유닛들
        /// </summary>
        public List<IBattleUnit> Units { get; set; }

        /// <summary>
        /// 각 유닛의 액션 리스트 (Units와 같은 순서)
        /// Units[i]의 액션 = ActionsPerUnit[i]
        /// </summary>
        public List<List<IBattleAction>> ActionsPerUnit { get; set; }

        /// <summary>
        /// 팀의 패배 조건
        /// </summary>
        public DefeatCondition DefeatCondition { get; set; }

        /// <summary>
        /// 메인 캐릭터 인덱스 (DefeatCondition이 MainCharacterDead일 때 사용)
        /// -1이면 설정 안 됨
        /// </summary>
        public int MainCharacterIndex { get; set; } = -1;

        public BattleTeam()
        {
            Units = new List<IBattleUnit>();
            ActionsPerUnit = new List<List<IBattleAction>>();
            DefeatCondition = DefeatCondition.AllDead;
        }

        /// <summary>
        /// 팀이 패배했는지 체크
        /// </summary>
        public bool IsDefeated()
        {
            if (Units == null || Units.Count == 0)
                return true;

            if (DefeatCondition == DefeatCondition.AllDead)
            {
                // 전멸 조건: 모든 유닛이 사망
                return Units.All(u => u.IsDead);
            }
            else // MainCharacterDead
            {
                // 메인 캐릭터 사망 조건
                if (MainCharacterIndex >= 0 && MainCharacterIndex < Units.Count)
                {
                    return Units[MainCharacterIndex].IsDead;
                }

                // MainCharacterIndex가 유효하지 않으면 전멸 조건으로 Fallback
                return Units.All(u => u.IsDead);
            }
        }

        /// <summary>
        /// 살아있는 유닛들만 반환
        /// </summary>
        public List<IBattleUnit> GetAliveUnits()
        {
            if (Units == null)
                return new List<IBattleUnit>();

            return Units.Where(u => !u.IsDead).ToList();
        }

        /// <summary>
        /// 팀 전체를 복제 (시뮬레이션용)
        /// </summary>
        public BattleTeam Clone()
        {
            // 액션도 함께 복제
            var clonedActions = new List<List<IBattleAction>>();
            if (ActionsPerUnit != null)
            {
                foreach (var actionList in ActionsPerUnit)
                {
                    var clonedList = new List<IBattleAction>();
                    foreach (var action in actionList)
                    {
                        clonedList.Add(action.Clone());
                    }
                    clonedActions.Add(clonedList);
                }
            }

            return new BattleTeam
            {
                Units = Units?.Select(u => u.Clone()).ToList() ?? new List<IBattleUnit>(),
                ActionsPerUnit = clonedActions,
                DefeatCondition = DefeatCondition,
                MainCharacterIndex = MainCharacterIndex
            };
        }

        /// <summary>
        /// 메인 캐릭터 유닛 가져오기
        /// </summary>
        public IBattleUnit GetMainCharacter()
        {
            if (MainCharacterIndex >= 0 && MainCharacterIndex < Units?.Count)
            {
                return Units[MainCharacterIndex];
            }
            return null;
        }
    }
}
