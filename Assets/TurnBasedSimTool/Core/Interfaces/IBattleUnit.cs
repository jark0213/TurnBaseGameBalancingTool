namespace TurnBasedSimTool.Core
{
    /// <summary>
    /// 최소한의 전투 유닛 인터페이스
    /// 이름, HP, 생존 여부만 정의
    /// 추가 기능은 구현체에서 선택적으로 추가
    /// </summary>
    public interface IBattleUnit
    {
        string Name { get; set; }
        int MaxHp { get; set; }
        int CurrentHp { get; set; }
        bool IsDead { get; }

        IBattleUnit Clone();
    }
}