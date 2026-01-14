using System.Collections.Generic;

namespace TurnBasedSim.Core 
{
    public class FlexibleBattleSimulator 
    {
        private List<IBattlePhase> _phases = new List<IBattlePhase>();

        public void AddPhase(IBattlePhase phase) {
            _phases.Add(phase);
        }

        public SimulationResult Run(IBattleUnit player, IBattleUnit enemy, int maxTurns = 100) {
            var p = player.Clone();
            var e = enemy.Clone();
            var context = new BattleContext { CurrentTurn = 0, IsFinished = false };

            while (context.CurrentTurn < maxTurns && !context.IsFinished) {
                context.CurrentTurn++;

                foreach (var phase in _phases) {
                    phase.Execute(p, e, context);
                    
                    if (p.IsDead || e.IsDead || context.IsFinished) {
                        context.IsFinished = true;
                        context.PlayerWon = !p.IsDead;
                        context.ResultMessage = p.IsDead ? "Player Defeated" : "Enemy Slain";
                        break;
                    }
                }
            }

            return new SimulationResult(context.PlayerWon, context.CurrentTurn, p.CurrentHp, context.ResultMessage);
        }
        
        public void ClearPhases()
        {
            _phases.Clear();
        }
    }
}