using System.Collections;
using System.Collections.Generic;
using TurnBasedSim.Core;
using UnityEngine;

public class TestRunner : MonoBehaviour
{
// TestRunner.cs (임시 테스트용)
    void Start() {
        var sim = new BattleSimulator();
        var player = new DefaultUnit { Name = "Warrior", MaxHp = 10 };
        var enemy = new DefaultUnit { Name = "Slime", MaxHp = 5 };

        var result = sim.Run(player, enemy);
        Debug.Log($"결과: {(result.IsPlayerWin ? "승리" : "패배")}, 소요 턴: {result.TotalTurns}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
