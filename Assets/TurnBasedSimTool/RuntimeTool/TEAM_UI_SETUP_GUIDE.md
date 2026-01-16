# Team-Based UI Setup Guide

새로운 팀 기반 UI 구조로 업그레이드되었습니다.
**1v1** 에서 **NvM (다대다)** 시뮬레이션으로 확장 가능합니다.

---

## 📋 구조 개요

```
TeamSettingsPanel (Player Team / Enemy Team)
├─ Team Foldout Toggle (접기/펼치기)
├─ [Add Character] Button
└─ Character List (Vertical Layout)
    ├─ CharacterSettingsPanel 1
    │   ├─ Character Name
    │   ├─ Foldout Toggle
    │   ├─ [Delete] Button
    │   └─ Stats + Actions
    ├─ CharacterSettingsPanel 2
    └─ ...
```

---

## 🛠️ 1단계: CharacterSettingsPanel Prefab 생성

### **Hierarchy 구조:**
```
CharacterSettingsPanel (GameObject)
├─ Header (Horizontal Layout)
│   ├─ DeleteButton (Button)
│   ├─ CharacterNameInput (TMP_InputField)
│   └─ FoldoutToggle (Toggle)
├─ ContentArea (Vertical Layout) ← 접었다 펼 영역
│   ├─ StatsSection
│   │   ├─ HP Input
│   │   ├─ Defense Input
│   │   ├─ Evasion Input
│   │   ├─ CritRate Input
│   │   └─ Speed Input
│   └─ ActionsSection
│       ├─ [Add Action] Button
│       └─ ActionList (Scroll View)
```

### **Inspector 설정:**
- **CharacterSettingsPanel.cs** 컴포넌트 추가
- 각 필드 연결:
  - `characterNameInput` → CharacterNameInput
  - `foldoutToggle` → FoldoutToggle
  - `deleteButton` → DeleteButton
  - `contentArea` → ContentArea GameObject
  - `hpInput`, `defenseInput`, `evasionInput`, `critRateInput`, `speedInput`
  - `addActionButton`, `actionListContent`, `actionItemPrefab`

### **Prefab 저장:**
`Assets/TurnBasedSimTool/RuntimeTool/Prefabs/CharacterSettingsPanel.prefab`

---

## 🛠️ 2단계: TeamSettingsPanel 생성

### **Hierarchy 구조:**
```
TeamSettingsPanel (GameObject)
├─ TeamHeader (Horizontal Layout)
│   ├─ TeamNameText (TMP_Text) "Player Team"
│   └─ TeamFoldoutToggle (Toggle)
├─ TeamContentArea (Vertical Layout) ← 팀 전체 접기/펼치기
│   ├─ [Add Character] Button
│   ├─ DefeatConditionDropdown (TMP_Dropdown)
│   └─ CharacterListContent (Vertical Layout) ← 캐릭터들이 생성될 위치
```

### **Inspector 설정:**
- **TeamSettingsPanel.cs** 컴포넌트 추가
- 필드 연결:
  - `teamName` → "Player" or "Enemy"
  - `teamFoldoutToggle` → TeamFoldoutToggle
  - `teamContentArea` → TeamContentArea GameObject
  - `addCharacterButton` → Add Character Button
  - `characterListContent` → CharacterListContent Transform
  - `characterPanelPrefab` → CharacterSettingsPanel.prefab
  - `defeatConditionDropdown` → DefeatConditionDropdown

---

## 🛠️ 3단계: SimulationSettingsPanel 업데이트

### **추가 필드:**
```
Speed System Section:
├─ UseSpeedSystemToggle (Toggle)
└─ FirstTurnDropdown (TMP_Dropdown)
    - Options: "Player First", "Enemy First", "Random"
```

### **Inspector 설정:**
- `useSpeedSystemToggle` → UseSpeedSystemToggle
- `firstTurnDropdown` → FirstTurnDropdown

---

## 🛠️ 4단계: Scene 구성

### **BattleSimDashboard Scene:**
```
Canvas
├─ LeftPanel (Scroll View) ← 게임 설정
│   ├─ PlayerTeamPanel (TeamSettingsPanel)
│   └─ EnemyTeamPanel (TeamSettingsPanel)
├─ RightPanel (Scroll View) ← 시뮬레이션 설정/결과
│   ├─ SimulationSettingsPanel
│   ├─ [Run Simulation] Button
│   └─ SimulationResultPanel
└─ SimUIManager (GameObject)
    - playerTeam → PlayerTeamPanel
    - enemyTeam → EnemyTeamPanel
    - settingsPanel → SimulationSettingsPanel
    - resultPanel → SimulationResultPanel
    - runButton → Run Simulation Button
```

---

## ✅ 5단계: 동작 확인

### **테스트 순서:**
1. **Play 모드 진입**
2. **Player Team에 Character 자동 추가됨 (기본 1개)**
3. **[Add Character] 버튼 클릭** → 캐릭터 추가
4. **Foldout Toggle** → 접기/펼치기 동작 확인
5. **[Delete] 버튼** → 캐릭터 삭제 (최소 1개 유지)
6. **Speed Toggle ON** → Speed 필드만 표시
7. **Speed Toggle OFF** → FirstTurn Dropdown 표시
8. **[Run Simulation]** → 시뮬레이션 실행

---

## 📝 주요 변경사항

### **리네이밍:**
- `UnitSettingsPanel` → `CharacterSettingsPanel`
- `CreateUnit()` → `CreateCharacter()`

### **새 기능:**
- ✅ 여러 캐릭터 관리 (Add/Delete)
- ✅ Team Foldout (팀 단위 접기/펼치기)
- ✅ Character Foldout (캐릭터별 접기/펼치기)
- ✅ Character Name 입력
- ✅ Speed System (턴 순서 제어)
- ✅ First Turn Option (선공권)
- ✅ Defeat Condition (패배 조건)

### **호환성:**
- 현재는 **1v1** 시뮬레이션만 동작 (첫 번째 캐릭터 사용)
- **NvM** 지원은 BattleSimulator 확장 후 가능

---

## 🔧 다음 단계

1. **Unity에서 Prefab 생성** (CharacterSettingsPanel)
2. **Scene 재구성** (TeamSettingsPanel 배치)
3. **Inspector 연결** (모든 SerializeField)
4. **테스트 실행**
5. **BattleSimulator 확장** (팀 기반 전투)

---

## 💡 팁

### **빠른 설정:**
1. 기존 PlayerPanel/EnemyPanel GameObject를 복사
2. TeamSettingsPanel 컴포넌트로 교체
3. CharacterSettingsPanel Prefab 생성
4. Inspector 연결

### **Prefab 변형:**
- CharacterSettingsPanel Prefab을 변형하여 게임별 커스터마이징 가능
- Stats 추가, ActionItem 구조 변경 등

### **확장 포인트:**
- DefeatCondition: 주요 캐릭터 선택 UI 추가
- 팀 편성 제약: 최소/최대 인원 설정
- Position System: 다키스트던전 스타일 위치 시스템
