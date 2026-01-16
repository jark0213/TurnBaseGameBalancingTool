using System;

namespace TurnBasedSimTool.Core.Logic
{
    public interface ICostHandler
    {
        int CurrentCost { get; }
        void OnTurnStart();
        void OnTurnEnd();
        bool CanAfford(int cost);
        void Consume(int cost);
    }

    public class CostHandler : ICostHandler
    {
        public int MaxCost = 3;
        public int RecoveryAmount = 3;
        public int FixedRetention = 0;   // 고정 이월 (유물 등)
        public float RetentionRate = 0f; // 비율 이월 (0~1)
        public int MaxRetention = 99;    // 최대 이월 제한

        public int CurrentCost { get; private set; }

        public void OnTurnStart()
        {
            CurrentCost = Math.Min(MaxCost, CurrentCost + RecoveryAmount);
        }

        public void OnTurnEnd()
        {
            int totalRetained = Math.Max(FixedRetention, (int)(CurrentCost * RetentionRate));
            CurrentCost = Math.Min(Math.Min(totalRetained, CurrentCost), MaxRetention);
        }

        public bool CanAfford(int cost) => CurrentCost >= cost;

        public void Consume(int cost)
        {
            CurrentCost = Math.Max(0, CurrentCost - cost);
        }
    }
}