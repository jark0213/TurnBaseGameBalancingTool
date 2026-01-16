# ExternalEffectAdapter 사용 가이드

## 목적
자신의 게임에서 사용 중인 이펙트 시스템을 시뮬레이터와 연결합니다.

## 사용 시나리오
- **DSL 기반 이펙트 시스템** (예: 던전다이스의 DSLKeywordEffect)
- **ScriptableObject 기반 스킬 시스템**
- **스프레드시트 데이터 기반 능력치 계산**
- **커스텀 이펙트 엔진**

---

## 구현 방법

### 1. ExternalEffectAdapter 상속

```csharp
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Core.Adapters;

public class MyGameEffectAdapter : ExternalEffectAdapter
{
    private MyEffect _effect;
    private string _effectData;

    public MyGameEffectAdapter(string effectData)
    {
        _effectData = effectData;
        _effect = MyEffectParser.Parse(effectData);
    }

    public override string ActionName => _effect.Name;

    protected override void ExecuteExternalEffect(
        IBattleUnit attacker,
        IBattleUnit defender,
        BattleContext context)
    {
        // 여기에 자신의 이펙트 시스템 호출
        _effect.Apply(attacker, defender);
    }

    public override IBattleAction Clone()
    {
        // 상태 초기화를 위해 새 인스턴스 생성
        return new MyGameEffectAdapter(_effectData);
    }
}
```

---

## 던전다이스 DSL 통합 예시

던전다이스 프로젝트에서 사용할 구체적인 구현 예시:

```csharp
// Assets/DungeonDice/SimulationIntegration/DSLEffectAdapter.cs
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Core.Adapters;
using DungeonDice.Effects; // 던전다이스의 이펙트 시스템

public class DSLEffectAdapter : ExternalEffectAdapter
{
    private IEffect _compiledEffect;
    private string _dslCode;

    public DSLEffectAdapter(string dsl)
    {
        _dslCode = dsl;
        // 기존 던전다이스의 파서 활용
        _compiledEffect = CompiledEffectCache.GetOrParse(dsl);
    }

    public override string ActionName => _compiledEffect.EffectName;

    // DSL에 코스트 정보가 있다면 파싱
    public override int GetCost(IBattleState state)
    {
        return _compiledEffect.HasCost ? _compiledEffect.Cost : 0;
    }

    protected override void ExecuteExternalEffect(
        IBattleUnit attacker,
        IBattleUnit defender,
        BattleContext context)
    {
        // 던전다이스의 DeepCloner 활용
        var runtimeEffect = EffectDeepCloner.Clone(_compiledEffect);

        // 기존 Execute 로직 그대로 사용
        runtimeEffect.Execute(attacker, defender);
    }

    public override IBattleAction Clone()
    {
        // DSL 코드로부터 새 인스턴스 생성
        return new DSLEffectAdapter(_dslCode);
    }
}
```

### 던전다이스 프로젝트에서 사용하기

```csharp
// 시뮬레이터 설정 시
var playerActions = new List<IBattleAction>
{
    new DSLEffectAdapter("damage 10 to enemy"),
    new DSLEffectAdapter("heal 5 to self"),
    new DSLEffectAdapter("poison 3 for 2turns to enemy")
};

playerPhase.SetActions(playerActions);
```

---

## 코스트 시스템 통합

코스트를 사용하는 경우 `GetCost`와 `CanExecute`를 오버라이드하세요:

```csharp
public override int GetCost(IBattleState state)
{
    // 이펙트 데이터에서 코스트 추출
    return _effect.ManaCost;
}

public override bool CanExecute(IBattleState state)
{
    // 조건 체크 (예: 특정 버프가 있을 때만 실행)
    return state.UseCostSystem
        ? state.Cost.CanAfford(GetCost(state))
        : true;
}
```

---

## 전후처리 훅 활용

액션 실행 전후로 로깅이나 애니메이션이 필요한 경우:

```csharp
protected override void OnBeforeExecute(
    IBattleUnit attacker,
    IBattleUnit defender,
    BattleContext context)
{
    Debug.Log($"{attacker.Name} uses {ActionName}!");
}

protected override void OnAfterExecute(
    IBattleUnit attacker,
    IBattleUnit defender,
    BattleContext context)
{
    // 이펙트 트리거 알림
    context.TriggerEffectApplied(attacker, defender, _effect);
}
```

---

## 주의사항

### ⚠️ Clone() 구현 필수
몬테카를로 시뮬레이션은 수천 번 반복 실행되므로, **상태 오염을 방지**하기 위해 반드시 깊은 복사를 구현하세요.

```csharp
// ❌ 잘못된 예 - 같은 인스턴스 반환
public override IBattleAction Clone()
{
    return this; // 상태 오염 발생!
}

// ✅ 올바른 예 - 새 인스턴스 생성
public override IBattleAction Clone()
{
    return new MyEffectAdapter(_originalData);
}
```

### 🔍 디버깅 팁
- `OnBeforeExecute`에 로그를 추가하여 액션 실행 순서 확인
- `Clone()`이 제대로 작동하는지 테스트 (인스턴스 비교)
- 코스트 시스템을 사용하지 않는다면 `GetCost`는 0 반환

---

## 참고 자료
- `Assets/TurnBasedSimTool/Examples/DungeonDicePlugin/` - 구현 예시
- `Assets/TurnBasedSimTool/Standard/IntervalActionAdapter.cs` - 어댑터 패턴 예시
- `Assets/TurnBasedSimTool/Core/Interfaces/IBattleAction.cs` - 인터페이스 정의
