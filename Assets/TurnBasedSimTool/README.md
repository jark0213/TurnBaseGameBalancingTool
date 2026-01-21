# Turn-Based Battle Simulation Tool

범용 턴제 전투 밸런싱 툴입니다. N vs M 전투 시뮬레이션을 통해 캐릭터 밸런스를 테스트할 수 있습니다.

## 📋 목차
- [기능 소개](#기능-소개)
- [시작하기](#시작하기)
- [사용 방법](#사용-방법)
- [확장 방법](#확장-방법)
- [저장 시스템](#저장-시스템)
- [요구사항](#요구사항)

---

## 기능 소개

### ✨ 핵심 기능
- **N vs M 전투 시뮬레이션**: 플레이어 팀 vs 적 팀의 몬테카를로 시뮬레이션
- **코스트 시스템**: 행동력(Cost) 기반 전투
- **속도 시스템**: 스피드 스탯 기반 턴 순서 결정
- **커스텀 스탯**: 게임별 고유 스탯 추가 가능
- **저장/불러오기**: 시뮬레이션 설정 및 팀 구성 저장

### 🎯 이 툴은 무엇인가?
- **범용 단일 전투 계산기**
- 턴제 전투의 승률, 평균 턴 수, 생존율 등을 분석
- 게임 프로젝트에 패키지로 임포트하여 사용

### ❌ 이 툴이 아닌 것
- 스테이지 시스템, 덱/주사위 메커닉 등은 **게임 프로젝트에서 구현**
- 게임별 특수 메커닉은 **Adapter 패턴**으로 확장

---

## 시작하기

### 패키지 설치

**방법 1: Package Manager (Git URL)**
1. Unity Package Manager 열기
2. `+ 버튼` → `Add package from git URL`
3. 입력: `https://github.com/[your-repo]/TurnBasedSimTool.git`
4. `Add` 클릭

**방법 2: manifest.json 수정**
```json
{
  "dependencies": {
    "com.milestone.turnbasedsimtool": "https://github.com/[your-repo]/TurnBasedSimTool.git",
    ...
  }
}
```

### 자동 설치되는 의존성
패키지 설치 시 다음 라이브러리가 자동으로 설치됩니다:
- **Newtonsoft.Json** (v3.2.1) - JSON 직렬화
- **TextMeshPro** (v3.0.6) - UI 텍스트
- **StandaloneFileBrowser** (포함됨) - 파일 다이얼로그

**수동 작업 불필요!** 모든 의존성이 자동 처리됩니다.

---

## 사용 방법

### 1. 시뮬레이션 설정

**Simulation Settings Panel**에서 전투 규칙을 설정합니다:

```
[Simulation Settings]
├─ Iterations: 1000          # 시뮬레이션 반복 횟수
├─ Max Turns: 100            # 최대 턴 수
├─ Cost System: ON/OFF       # 코스트 시스템 사용 여부
│  ├─ Max Cost: 3            # 최대 코스트
│  └─ Recovery: 3            # 턴당 회복량
├─ Speed System: ON/OFF      # 속도 시스템 사용 여부
│  ├─ First Turn: Player/Enemy/Random
│  ├─ Speed Tiebreak: Random/Defense/Custom
│  └─ Custom Stat Name       # 커스텀 스탯 이름
└─ [Save] [Load]             # 설정 저장/불러오기
```

**저장 위치**: `Application.persistentDataPath/last_settings.json`

### 2. 팀 구성

**Player Team / Enemy Team Panel**에서 캐릭터를 추가합니다:

```
[Player Team]
├─ [Save Team] [Load Team] [Open Folder]
├─ Defeat Condition: All Dead / Main Character Dead
├─ [Add Character]
└─ Character List:
    ├─ Character 1
    │  ├─ Name: "Knight"
    │  ├─ HP: 100
    │  ├─ Defense: 10
    │  ├─ Speed: 8
    │  └─ Actions: [Add Action]
    └─ Character 2
       └─ ...
```

### 3. 액션 추가

각 캐릭터의 **Actions** 섹션에서 행동을 추가합니다:

```
[Actions]
├─ [Add Action] 드롭다운
│  ├─ Generic Action        # 범용 공격
│  ├─ Ranged Damage Action  # 원거리 공격
│  └─ Interval Action       # 주기적 행동
└─ Action List:
    ├─ Attack (Cost: 2)
    │  ├─ Damage: 20
    │  ├─ Target: Single
    │  └─ [Delete]
    └─ Heavy Strike (Cost: 3)
       └─ ...
```

**코스트 시스템 OFF**: Cost 입력 필드가 자동으로 숨겨짐

### 4. 시뮬레이션 실행

1. **[Run Simulation]** 버튼 클릭
2. 결과 확인:
   - Player 승률
   - Enemy 승률
   - 평균 턴 수
   - 각 유닛의 생존율

---

## 확장 방법

### 커스텀 유닛 만들기

`DefaultUnit`을 상속하여 게임별 유닛을 만듭니다:

```csharp
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

[System.Serializable]
public class DungeonDiceUnit : DefaultUnit
{
    // 게임별 추가 데이터
    public List<int> diceIds;      // 주사위 ID들
    public List<int> abilityIds;   // 능력 ID들

    // Unity Object 참조는 저장 불가 → ID로 변환하여 저장
    [NonSerialized]
    public List<DiceAbility> abilities; // 런타임에 DSL에서 로드
}
```

**저장 시스템 자동 지원**: `[System.Serializable]` 속성만 추가하면 자동으로 저장됩니다.

### 커스텀 액션 만들기

`IBattleAction` 인터페이스를 구현합니다:

```csharp
using TurnBasedSimTool.Core;

public class BleedAction : IBattleAction
{
    public string Name => "Bleed Attack";
    public int Cost { get; set; } = 2;

    public void Execute(IBattleUnit user, List<IBattleUnit> targets, BattleContext context)
    {
        foreach (var target in targets)
        {
            // 데미지 + 출혈 상태 효과
            target.CurrentHp -= 10;
            target.AddStatus(new BleedingEffect());
        }
    }
}
```

### Adapter 패턴으로 확장

게임별 복잡한 메커닉은 Adapter로 구현합니다:

```csharp
// 예: 던전다이스 주사위 시스템
public class DiceSystemAdapter : MonoBehaviour
{
    public void ConvertDiceToActions(DungeonDiceUnit unit)
    {
        // DSL에서 주사위 능력 로드
        foreach (int diceId in unit.diceIds)
        {
            var diceData = DSLManager.LoadDice(diceId);
            // 주사위 → IBattleAction 변환
        }
    }
}
```

---

## 저장 시스템

### 시뮬레이션 설정 저장

**SettingsManager** 사용:

```csharp
// 저장
SimulationSettings settings = GetSettings();
SettingsManager.SaveSettings(settings);

// 불러오기
SimulationSettings settings = SettingsManager.LoadSettings();
```

**저장 위치**: `Application.persistentDataPath/last_settings.json`

**자동 로드**: 툴 시작 시 마지막 설정 자동 로드

### 팀 구성 저장

**CharacterSaveManager** 사용:

```csharp
// 저장
List<IBattleUnit> units = CreateTeam();
CharacterSaveManager.SaveTeam("PlayerTeam", units);

// 불러오기
List<IBattleUnit> units = CharacterSaveManager.LoadTeam("PlayerTeam");

// 저장된 팀 목록
List<string> teamNames = CharacterSaveManager.GetSavedTeamNames();
```

**저장 위치**: `Application.persistentDataPath/Teams/[팀이름].json`

**저장 폴더 열기**: UI의 **[Open Folder]** 버튼으로 파일 탐색기에서 직접 관리 가능

### 저장 가능한 데이터

✅ **가능:**
- 기본 타입 (int, string, float, bool)
- List, Dictionary
- 중첩된 클래스/구조체
- Enum
- 인터페이스/추상 클래스 (Newtonsoft.Json의 TypeNameHandling)

❌ **불가능:**
- ScriptableObject 참조 → **ID로 변환하여 저장**
- MonoBehaviour 참조
- Unity Object (Prefab, Sprite 등) → **리소스 경로로 저장**

### 저장 예시

```csharp
[System.Serializable]
public class MyCustomUnit : DefaultUnit
{
    // ✅ 저장됨
    public int level;
    public List<string> skills;
    public Dictionary<string, int> customStats;

    // ❌ 저장 안됨 (ScriptableObject 참조)
    public AbilityData abilityData;

    // ✅ 대안: ID로 저장
    public int abilityDataId; // 로드 시 ID로 ScriptableObject 찾기

    // ❌ 저장 안됨 (런타임 데이터)
    [NonSerialized]
    public Sprite icon;
}
```

---

## 요구사항

### Unity 버전
- Unity 2020.3 이상 권장
- Unity 2019.4 이상 호환

### 필수 패키지
- **Newtonsoft.Json**: `com.unity.nuget.newtonsoft-json`
  - 캐릭터 저장/불러오기 기능에 필요
  - Package Manager에서 설치

### 선택 사항
- **TextMeshPro**: UI에 사용 (Unity 기본 포함)

---

## 프로젝트 구조

```
TurnBasedSimTool/
├─ Core/                      # 핵심 인터페이스 및 시스템
│  ├─ Interfaces/
│  │  ├─ IBattleUnit.cs       # 유닛 인터페이스
│  │  ├─ IBattleAction.cs     # 액션 인터페이스
│  │  └─ IStatusEffect.cs     # 상태 효과 인터페이스
│  ├─ Settings/
│  │  └─ SimulationSettings.cs # 시뮬레이션 설정
│  └─ Simulator/
│     └─ BattleSimulator.cs   # 전투 시뮬레이터
├─ Standard/                  # 표준 구현
│  ├─ Units/
│  │  └─ DefaultUnit.cs       # 기본 유닛
│  └─ Actions/
│     ├─ GenericAction.cs     # 범용 공격
│     └─ RangedDamageAction.cs # 원거리 공격
├─ RuntimeTool/               # 런타임 UI 툴
│  ├─ Scripts/
│  │  ├─ SettingsManager.cs         # 설정 저장/로드
│  │  ├─ CharacterSaveManager.cs    # 캐릭터 저장/로드
│  │  ├─ SimulationSettingsPanel.cs # 시뮬레이션 설정 UI
│  │  ├─ TeamSettingsPanel.cs       # 팀 설정 UI
│  │  └─ CharacterSettingsPanel.cs  # 캐릭터 설정 UI
│  ├─ Prefabs/
│  └─ Scenes/
│     └─ BattleSimDashboard.unity   # 메인 씬
└─ Examples/                  # 예제 및 확장
   └─ CustomUnits/
      └─ DungeonDiceStyleUnit.cs    # 커스텀 유닛 예제
```

---

## FAQ

### Q: 주사위/덱 시스템을 추가하고 싶어요
A: **게임 프로젝트에서 Adapter로 구현**하세요. 이 툴은 범용 전투 계산기이므로, 게임별 메커닉은 별도로 구현합니다.

### Q: 저장 파일 위치가 어디인가요?
A: `Application.persistentDataPath` 경로입니다:
- Windows: `C:\Users\[user]\AppData\LocalLow\[company]\[product]\`
- macOS: `~/Library/Application Support/[company]/[product]/`
- **UI의 [Open Folder] 버튼**으로 쉽게 열 수 있습니다.

### Q: ScriptableObject를 저장할 수 없나요?
A: **직접 저장 불가능**합니다. 대신 **ID나 경로를 저장**하고, 로드 시 복원하세요:
```csharp
public int abilityId; // ScriptableObject의 ID 저장
[NonSerialized]
public AbilityData ability; // 로드 시 ID로 찾기
```

### Q: 다른 프로젝트로 팀 데이터를 복사할 수 있나요?
A: 가능합니다. **[Open Folder]** 버튼으로 저장 폴더를 열고, JSON 파일을 복사하여 다른 프로젝트의 같은 경로에 붙여넣으세요.

---

## 라이선스

MIT License

---

## 버전

**v1.0.0** - 2025.01
- 초기 릴리스
- N vs M 전투 시뮬레이션
- 코스트/속도 시스템
- 설정 및 팀 저장/불러오기
