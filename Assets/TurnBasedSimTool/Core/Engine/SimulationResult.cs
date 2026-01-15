namespace TurnBasedSimTool.Core {
    public struct SimulationResult {
        public bool IsPlayerWin;
        public int TotalTurns;
        public int RemainingHp;
        public string EndReason; // 예: "Enemy Slain", "Turn Limit Exceeded"

        public SimulationResult(bool win, int turns, int hp, string reason) {
            IsPlayerWin = win;
            TotalTurns = turns;
            RemainingHp = hp;
            EndReason = reason;
        }
    }
}