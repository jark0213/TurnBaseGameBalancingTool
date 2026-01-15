using UnityEngine;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;
using TurnBasedSimTool.Examples.Tests;

namespace TurnBasedSimTool.Examples.Tests
{
    public class SimTest : MonoBehaviour
    {
        void Start()
        {
            var fbSim = new FlexibleBattleSimulator();
            var runner = new MonteCarloRunner(fbSim);

            // 2. 캐릭터 설정 (Warrior vs Slime)
            var player = new DefaultUnit { Name = "Warrior", MaxHp = 10, CurrentHp = 10 };
            var enemy = new DefaultUnit { Name = "Slime", MaxHp = 8, CurrentHp = 8 };

            // 1. 플레이어용 페이즈들
            var pStart = new TestStartTurnPhase("P_Start", true);
            var pAction = new TestActionPhase("P_Action", true);
            var pEnd = new TestEndTurnPhase("P_End", true);

            // 2. 적용 페이즈들
            var eStart = new TestStartTurnPhase("E_Start", false);
            var eAction = new TestActionPhase("E_Action", false);
            var eEnd = new TestEndTurnPhase("E_End", false);

            // 3. 미들웨어 주입 (예: 상태 이상 시스템)
            var statusMW = new StatusEffectMiddleware();
            pAction.AddMiddleware(statusMW); // 플레이어 행동 시 상태 체크
            eAction.AddMiddleware(statusMW); // 적 행동 시 상태 체크

            // 2. 적에게 독 부여 (테스트용)
            enemy.AddStatus(new SimplePoison(5));

            // 4. 시뮬레이터에 순서대로 등록
            fbSim.AddPhase(pStart);
            fbSim.AddPhase(pAction);
            fbSim.AddPhase(pEnd);
            fbSim.AddPhase(eStart);
            fbSim.AddPhase(eAction);
            fbSim.AddPhase(eEnd);


            // 3. 10,000번 시뮬레이션 실행
            Debug.Log("시뮬레이션 시작...");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var settings = new SimulationSettings { Iterations = 10000 };
            var report = runner.RunSimulation(player, enemy, settings);

            stopwatch.Stop();

            // 4. 결과 출력
            Debug.Log($"[결과] 총 테스트: {report.TotalCount}회");
            Debug.Log($"[결과] 승리: {report.WinCount}회 (승률: {report.WinRate:F2}%)");
            Debug.Log($"[결과] 평균 소요 턴: {report.AvgTurns:F1}턴");
            Debug.Log($"[성능] 계산 소요 시간: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
