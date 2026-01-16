# Runtime UI Setup Guide

Unity Editor에서 BattleSimDashboard.unity Scene을 설정하는 가이드입니다.

---

## 전체 UI 구조

```
Canvas
├── LeftPanel (ScrollView - 게임 요소)
│   └── Viewport
│       └── Content
│           ├── PlayerSettingsPanel (GameObject + UnitSettingsPanel.cs)
│           └── EnemySettingsPanel (GameObject + UnitSettingsPanel.cs)
│
├── RightPanel (ScrollView - 시뮬레이션)
│   └── Viewport
│       └── Content
│           ├── SimulationSettingsPanel (GameObject + SimulationSettingsPanel.cs)
│           ├── RunButton (Button)
│           └── SimulationResultPanel (GameObject + SimulationResultPanel.cs)
│
└── SimUIManager (Empty GameObject + SimUIManager.cs)
```

---

## 1단계: 좌우 분할 레이아웃 만들기

### 1. Canvas 설정
- Canvas Scaler: Scale With Screen Size 권장
- Reference Resolution: 1920x1080 (프로젝트에 맞게 조정)

### 2. LeftPanel (ScrollView) 생성
1. Hierarchy 우클릭 → UI → Scroll View
2. 이름: `LeftPanel`
3. RectTransform 설정:
   - Anchor: Left-Stretch
   - Pos X: 0
   - Width: 960 (화면의 절반)
   - Height: Stretch (Top: 0, Bottom: 0)
4. Scrollbar 설정:
   - Horizontal Scrollbar 삭제 (불필요)
   - Vertical Scrollbar만 유지

### 3. RightPanel (ScrollView) 생성
1. Hierarchy 우클릭 → UI → Scroll View
2. 이름: `RightPanel`
3. RectTransform 설정:
   - Anchor: Right-Stretch
   - Pos X: 0
   - Width: 960 (화면의 절반)
   - Height: Stretch (Top: 0, Bottom: 0)
4. Scrollbar 설정:
   - Horizontal Scrollbar 삭제
   - Vertical Scrollbar만 유지

---

## 2단계: Left Panel - 게임 요소 UI

### PlayerSettingsPanel 만들기

```
LeftPanel/Viewport/Content
└── PlayerSettingsPanel (GameObject)
    ├── TitleText (TextMeshPro - "Player Settings")
    ├── StatsGroup (Vertical Layout Group)
    │   ├── HPRow (Horizontal Layout Group)
    │   │   ├── Label (TextMeshPro - "HP:")
    │   │   └── HPInput (TMP_InputField)
    │   └── DamageRow (Horizontal Layout Group)
    │       ├── Label (TextMeshPro - "Damage:")
    │       └── DamageInput (TMP_InputField)
    ├── ActionsGroup
    │   ├── ActionsLabel (TextMeshPro - "Actions:")
    │   ├── AddActionButton (Button - "+")
    │   └── ActionListContent (Vertical Layout Group)
    └── UnitSettingsPanel (Script Component)
```

#### UnitSettingsPanel.cs Inspector 연결:
- `hpInput` → HPInput
- `damageInput` → DamageInput
- `addActionButton` → AddActionButton
- `actionListContent` → ActionListContent
- `actionItemPrefab` → ActionItemPrefab (다음 단계에서 생성)

### EnemySettingsPanel 만들기
- PlayerSettingsPanel을 복제 (Ctrl+D)
- 이름만 `EnemySettingsPanel`로 변경
- TitleText를 "Enemy Settings"로 변경

### Content 설정
- LeftPanel/Viewport/Content에 Vertical Layout Group 추가
- Child Force Expand: Width만 체크
- Spacing: 20

---

## 3단계: Right Panel - 시뮬레이션 UI

### SimulationSettingsPanel 만들기

```
RightPanel/Viewport/Content
└── SimulationSettingsPanel (GameObject)
    ├── TitleText (TextMeshPro - "Simulation Settings")
    ├── BasicSettings (Vertical Layout Group)
    │   ├── IterationsRow
    │   │   ├── Label ("Iterations:")
    │   │   └── IterationsInput (TMP_InputField - default: "1000")
    │   ├── MaxTurnsRow
    │   │   ├── Label ("Max Turns:")
    │   │   └── MaxTurnsInput (TMP_InputField - default: "100")
    │   └── MaxActionsRow
    │       ├── Label ("Max Actions/Turn:")
    │       └── MaxActionsInput (TMP_InputField - default: "1")
    ├── CostSystemGroup
    │   ├── UseCostToggle (Toggle - "Use Cost System" - default: true)
    │   ├── MaxCostRow
    │   │   ├── Label ("Max Cost:")
    │   │   └── MaxCostInput (TMP_InputField - default: "3")
    │   └── RecoveryRow
    │       ├── Label ("Recovery Amount:")
    │       └── RecoveryInput (TMP_InputField - default: "3")
    └── SimulationSettingsPanel (Script Component)
```

#### SimulationSettingsPanel.cs Inspector 연결:
- `iterationsInput` → IterationsInput
- `maxTurnsInput` → MaxTurnsInput
- `maxActionsPerTurnInput` → MaxActionsInput
- `useCostSystemToggle` → UseCostToggle
- `maxCostInput` → MaxCostInput
- `recoveryAmountInput` → RecoveryInput

### RunButton 추가
```
RightPanel/Viewport/Content
└── RunButton (Button)
    └── Text (TextMeshPro - "Run Simulation")
```
- 버튼 크기: Height 60~80 정도
- 색상: 강조 색상 (초록/파랑 등)

### SimulationResultPanel 만들기

```
RightPanel/Viewport/Content
└── SimulationResultPanel (GameObject)
    ├── TitleText (TextMeshPro - "Results")
    ├── ResultText (TextMeshPro)
    │   └── 설정:
    │       - Alignment: Center or Left
    │       - Font Size: 18~24
    │       - 초기 텍스트: "No results yet"
    ├── ButtonGroup (Horizontal Layout Group)
    │   ├── ExportButton (Button - "Export CSV")
    │   └── ClearButton (Button - "Clear")
    └── SimulationResultPanel (Script Component)
```

#### SimulationResultPanel.cs Inspector 연결:
- `resultText` → ResultText
- `exportButton` → ExportButton
- `clearButton` → ClearButton

### Content 설정
- RightPanel/Viewport/Content에 Vertical Layout Group 추가
- Child Force Expand: Width만 체크
- Spacing: 20

---

## 4단계: ActionItemPrefab 만들기

Prefabs 폴더에 저장할 Prefab입니다.

### 구조
```
ActionItemPrefab (GameObject)
├── Background (Image)
├── SelectToggle (Toggle)
├── NameInputField (TMP_InputField - placeholder: "Action Name")
├── ValueInputField (TMP_InputField - placeholder: "Value")
├── IntervalInputField (TMP_InputField - placeholder: "Interval")
└── ActionItemUI (Script Component)
```

### ActionItemUI.cs Inspector 연결:
- `selectToggle` → SelectToggle
- `nameInput` → NameInputField
- `valueInput` → ValueInputField
- `intervalInput` → IntervalInputField

### Prefab 저장 위치:
`Assets/TurnBasedSimTool/RuntimeTool/Prefabs/ActionItemPrefab.prefab`

---

## 5단계: SimUIManager 설정

### SimUIManager GameObject 생성
1. Canvas 아래에 Empty GameObject 생성
2. 이름: `SimUIManager`
3. SimUIManager.cs 스크립트 추가

### Inspector 연결:
```
Panels:
- playerPanel → LeftPanel/.../PlayerSettingsPanel
- enemyPanel → LeftPanel/.../EnemySettingsPanel
- settingsPanel → RightPanel/.../SimulationSettingsPanel
- resultPanel → RightPanel/.../SimulationResultPanel

Control:
- runButton → RightPanel/.../RunButton
```

### UnitSettingsPanel들에 Prefab 연결:
- PlayerSettingsPanel의 `actionItemPrefab` → ActionItemPrefab
- EnemySettingsPanel의 `actionItemPrefab` → ActionItemPrefab

---

## 6단계: Layout 세부 조정

### ScrollView Content 설정
- Content Fitter 추가: Vertical Fit = Preferred Size
- 이렇게 하면 내용물에 맞춰 자동으로 높이 조절됨

### Spacing 추천값:
- Panel 간 간격: 20~30
- 같은 Panel 내 요소 간격: 10
- Input Field 간격: 5~10

### 색상 추천:
- Background Panel: 약간 어두운 회색 (구분용)
- Input Field: 흰색 배경
- Button: 강조 색상
- Title Text: 굵게, 크기 20~24

---

## 7단계: 테스트

### 테스트 절차:
1. Play Mode 진입
2. Player/Enemy HP, Damage 입력
3. 각각 [+] 버튼으로 액션 추가
4. 액션 이름, 데미지, 인터벌 입력
5. Simulation Settings 확인/조정
6. [Run Simulation] 클릭
7. Results에 Win Rate, Avg Turns 표시 확인

### 예상 결과:
```
Results
Win Rate: 65.30%
Avg Turns: 8.2
Total Simulations: 1000
Player Wins: 653
Enemy Wins: 347
```

---

## 추가 개선 아이디어 (나중에)

### Left Panel:
- 버프/디버프 시스템
- 추가 스탯 (방어력, 회피율 등)
- 장비/아이템 시스템

### Right Panel:
- 결과 그래프 (승률 변화, 턴 분포 등)
- 시뮬레이션 히스토리
- A/B 비교 기능
- 상세 로그 보기

---

## 문제 해결

### 스크롤이 안 될 때:
- Content의 RectTransform Height가 Viewport보다 커야 함
- Content Fitter의 Vertical Fit = Preferred Size 확인

### 버튼이 안 눌릴 때:
- Canvas에 Graphic Raycaster 있는지 확인
- EventSystem GameObject 있는지 확인

### Panel이 안 보일 때:
- Canvas Render Mode 확인 (Screen Space - Overlay 권장)
- Panel의 Image 컴포넌트 있는지 확인 (투명해도 Raycast Target용)

---

## 완료 체크리스트

- [ ] Canvas 설정 완료
- [ ] LeftPanel ScrollView 생성
- [ ] RightPanel ScrollView 생성
- [ ] PlayerSettingsPanel 생성 및 스크립트 연결
- [ ] EnemySettingsPanel 생성 및 스크립트 연결
- [ ] SimulationSettingsPanel 생성 및 스크립트 연결
- [ ] RunButton 생성
- [ ] SimulationResultPanel 생성 및 스크립트 연결
- [ ] ActionItemPrefab 생성 및 저장
- [ ] SimUIManager 생성 및 모든 참조 연결
- [ ] Play Mode 테스트 성공
