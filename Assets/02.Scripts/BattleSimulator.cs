using System;

namespace TurnBasedSim.Core {
    public class BattleSimulator {
        public SimulationResult Run(IBattleUnit player, IBattleUnit enemy, int maxTurns = 100) {
            // 1. 전투 시작 전 클론 생성 (원본 데이터 보호)
            var p = player.Clone();
            var e = enemy.Clone();
            int currentTurn = 0;

            while (currentTurn < maxTurns && !p.IsDead && !e.IsDead) {
                currentTurn++;

                // [Step 1] 플레이어 공격 Phase
                ExecuteSimpleAttack(p, e);
                if (e.IsDead) 
                    return new SimulationResult(true, currentTurn, p.CurrentHp, "Enemy Slain");

                // [Step 2] 적 공격 Phase
                ExecuteSimpleAttack(e, p);
                if (p.IsDead) 
                    return new SimulationResult(false, currentTurn, p.CurrentHp, "Player Defeated");
            }

            return new SimulationResult(false, currentTurn, p.CurrentHp, "Time Over");
        }

        // 나중에 인터페이스 연결 전까지 사용할 기본 공격 로직
        private void ExecuteSimpleAttack(IBattleUnit attacker, IBattleUnit defender) {
            // TODO: 나중에 여기에 인챈트, 크리티컬, 상태이상 로직이 주입될 예정입니다.
            // 지금은 기본값인 '데미지 1'만 적용합니다.
            defender.CurrentHp -= 1;
            // Console.WriteLine 대신 사용할 수 있는 자체 로그 시스템이 필요할 수 있습니다.
        }
    }
}