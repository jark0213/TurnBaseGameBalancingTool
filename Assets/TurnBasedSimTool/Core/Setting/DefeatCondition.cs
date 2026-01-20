namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 팀 패배 조건
    /// </summary>
    public enum DefeatCondition
    {
        /// <summary>
        /// 전멸 - 팀의 모든 유닛이 사망하면 패배
        /// </summary>
        AllDead = 0,

        /// <summary>
        /// 주요 캐릭터 사망 - 지정된 메인 캐릭터가 사망하면 패배
        /// </summary>
        MainCharacterDead = 1
    }
}
