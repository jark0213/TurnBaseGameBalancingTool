using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TurnBasedSim.Core;
using TurnBasedSim.Standard;

public class SimUIManager : MonoBehaviour
{
    // 업로드 테스트
    
    [Header("Player Settings")]
    public TMP_InputField playerHpInput;
    public TMP_InputField playerDmgInput;

    [Header("Enemy Settings")]
    public TMP_InputField enemyHpInput;
    public TMP_InputField enemyDmgInput;

    [Header("Sim Settings")]
    public TMP_InputField countInput;
    public Button runButton;
    public TextMeshProUGUI resultText;

    private FlexibleBattleSimulator _simulator;
    private MonteCarloRunner _runner;

    void Start()
    {
        _simulator = new FlexibleBattleSimulator();
        _runner = new MonteCarloRunner(_simulator);
        runButton.onClick.AddListener(OnRunClick);
    }

    private void OnRunClick()
    {
        // 1. 유닛 준비
        var player = new DefaultUnit { 
            Name = "Player", 
            MaxHp = int.Parse(playerHpInput.text), 
            CurrentHp = int.Parse(playerHpInput.text) 
        };
        var enemy = new DefaultUnit { 
            Name = "Enemy", 
            MaxHp = int.Parse(enemyHpInput.text), 
            CurrentHp = int.Parse(enemyHpInput.text) 
        };

        // 2. 시뮬레이터 구성 (Phase 초기화 로직은 Simulator에 List.Clear() 추가 필요)
        // 현재 FlexibleBattleSimulator에 Clear 기능이 없다면 새로 생성하여 교체
        _simulator = new FlexibleBattleSimulator(); 
        
        var pPhase = new TestActionPhase("PlayerTurn", true);
        pPhase.AddAction(new GenericAction { ActionName = "Atk", Damage = int.Parse(playerDmgInput.text) });

        var ePhase = new TestActionPhase("EnemyTurn", false);
        ePhase.AddAction(new GenericAction { ActionName = "Atk", Damage = int.Parse(enemyDmgInput.text) });

        _simulator.AddPhase(pPhase);
        _simulator.AddPhase(ePhase);

        // 3. 실행 및 리포트 (깃허브의 MonteCarloReport 사용)
        int iterations = int.Parse(countInput.text);
        MonteCarloReport report = _runner.RunSimulation(player, enemy, iterations);

        // 4. 결과 출력
        resultText.text = $"Win Rate: {report.WinRate * 100:F2}%\nAvg Turns: {report.AvgTurns:F1}";
    }
}