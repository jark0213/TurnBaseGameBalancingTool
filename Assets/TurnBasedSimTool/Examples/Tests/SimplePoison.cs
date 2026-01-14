using TurnBasedSim.Core;
using UnityEngine;

public class SimplePoison : IStatusEffect {
    public string EffectName => "Poison";
    public int Duration { get; set; }

    public SimplePoison(int duration) {
        Duration = duration;
    }

    public void OnTurnStart(IBattleUnit owner, BattleContext context) {
        owner.CurrentHp -= 1; // 매 턴 시작 시 체력 1 감소
        Debug.Log($"{owner.Name}이 독 데미지를 입었습니다. 남은 HP: {owner.CurrentHp}");
    }

    public void OnAction(IBattleUnit owner, BattleContext context) { }
    public void OnTurnEnd(IBattleUnit owner, BattleContext context) {
        Duration--; // 턴 종료 시 지속시간 감소
    }

    public IStatusEffect Clone() => new SimplePoison(Duration);
}