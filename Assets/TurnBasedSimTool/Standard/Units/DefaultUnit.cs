using System.Collections.Generic;
using System.Linq;
using TurnBasedSim.Core;
using UnityEngine;

public class DefaultUnit : IBattleUnit
{
    public string Name { get; set; }
    public int MaxHp { get; set; }
    private int _currentHp;
    public int CurrentHp 
    { 
        get => _currentHp; 
        set => _currentHp = Mathf.Clamp(value, 0, MaxHp); 
    }

    public bool IsDead => CurrentHp <= 0;

    // 상태 효과 리스트
    public List<IStatusEffect> StatusEffects { get; private set; } = new List<IStatusEffect>();

    public void AddStatus(IStatusEffect effect)
    {
        StatusEffects.Add(effect);
    }

    public void RemoveStatus(IStatusEffect effect)
    {
        StatusEffects.Remove(effect);
    }

    // 몬테카를로 시뮬레이션을 위한 핵심 메서드
    public IBattleUnit Clone()
    {
        var clone = new DefaultUnit
        {
            Name = this.Name,
            MaxHp = this.MaxHp,
            CurrentHp = this.CurrentHp,
            // 상태 효과들도 각각 복제해서 새 리스트에 담아야 함 (중요!)
            StatusEffects = this.StatusEffects.Select(s => CloneStatus(s)).ToList()
        };
        return clone;
    }

    // 상태 효과 복제를 위한 헬퍼 (각 상태 효과 클래스도 복제 기능을 가지면 좋음)
    private IStatusEffect CloneStatus(IStatusEffect source)
    {
        // 실제로는 IStatusEffect에도 Clone() 인터페이스를 추가하는 것이 가장 깔끔합니다.
        // 지금은 단순화를 위해 소속 데이터를 넘기는 식으로 가정하거나 
        // 인터페이스에 Clone을 추가하는 방향을 추천합니다.
        return source; 
    }
}