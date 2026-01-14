using System.Collections;
using System.Collections.Generic;
using TurnBasedSim.Core;
using UnityEngine;

public class TestStartTurnPhase : BattlePhaseBase
{
    public TestStartTurnPhase(string name, bool isPlayer) : base(name, isPlayer) { }

    public override void Execute(IBattleUnit player, IBattleUnit enemy, BattleContext context)
    {
        // 시작 단계 로직 (예: 버프 감소 등)
        // 지금은 비워두어도 에러가 나지 않도록 구현만 해둡니다.
    }
}
