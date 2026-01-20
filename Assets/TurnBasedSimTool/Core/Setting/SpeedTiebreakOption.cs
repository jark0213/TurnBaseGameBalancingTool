namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// Speed 동점 시 순서 결정 방식
    /// </summary>
    public enum SpeedTiebreakOption
    {
        Random = 0,           // 무작위
        PlayerFirst = 1,      // 플레이어 팀 우선
        EnemyFirst = 2,       // 적 팀 우선
        UseStat = 3           // 추가 스탯으로 비교
    }

    /// <summary>
    /// 타이브레이크에 사용할 기본 스탯 옵션
    /// 순서대로 0, 1, 2, 3, 4로 연속된 값 (드롭다운 인덱스와 일치)
    /// </summary>
    public enum TiebreakStatOption
    {
        Defense = 0,
        Evasion = 1,
        CritRate = 2,
        Speed = 3,
        Custom = 4            // 항상 마지막에 배치
    }
}
