using System;
using UnityEngine;
using TurnBasedSim.Core;

public class SimTest : MonoBehaviour 
{
    void Start() 
    {
        var fbSim = new FlexibleBattleSimulator();
        var runner = new MonteCarloRunner(fbSim);

        // 2. 캐릭터 설정 (Warrior vs Slime)
        var player = new DefaultUnit { Name = "Warrior", MaxHp = 10, CurrentHp = 10 };
        var enemy = new DefaultUnit { Name = "Slime", MaxHp = 8, CurrentHp = 8 };
        
        // 1. 플레이어 페이즈들
        fbSim.AddPhase(new TestStartTurnPhase("P_Start", true));
        fbSim.AddPhase(new TestActionPhase("P_Action", true));
        fbSim.AddPhase(new TestEndTurnPhase("P_End", true));

        // 2. 적 페이즈들
        fbSim.AddPhase(new TestStartTurnPhase("E_Start", false));
        fbSim.AddPhase(new TestActionPhase("E_Action", false));
        fbSim.AddPhase(new TestEndTurnPhase("E_End", false));
        

        // 3. 10,000번 시뮬레이션 실행
        Debug.Log("시뮬레이션 시작...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var report = runner.RunSimulation(player, enemy, 10000);
        
        stopwatch.Stop();

        // 4. 결과 출력
        Debug.Log($"[결과] 총 테스트: {report.TotalCount}회");
        Debug.Log($"[결과] 승리: {report.WinCount}회 (승률: {report.WinRate:F2}%)");
        Debug.Log($"[결과] 평균 소요 턴: {report.AvgTurns:F1}턴");
        Debug.Log($"[성능] 계산 소요 시간: {stopwatch.ElapsedMilliseconds}ms");
    }
}