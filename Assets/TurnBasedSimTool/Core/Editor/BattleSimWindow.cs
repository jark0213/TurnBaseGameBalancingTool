using UnityEngine;
using UnityEditor;
using TurnBasedSimTool.Core;
using TurnBasedSimTool.Standard;

public class BattleSimWindow : EditorWindow
{
    private int simulationCount = 10000;
    private int playerHp = 20;
    private int enemyHp = 20;

    [MenuItem("Window/TurnBasedSim/Battle Simulator")]
    public static void ShowWindow()
    {
        GetWindow<BattleSimWindow>("Battle Sim Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Battle Simulation Settings", EditorStyles.boldLabel);

        playerHp = EditorGUILayout.IntField("Player HP", playerHp);
        enemyHp = EditorGUILayout.IntField("Enemy HP", enemyHp);
        simulationCount = EditorGUILayout.IntField("Simulation Count", simulationCount);

        if (GUILayout.Button("Run Simulation"))
        {
            RunSim();
        }
    }

    private void RunSim()
    {
        // 여기서 기존 SimTest의 로직을 에디터 값에 맞춰 실행
        Debug.Log($"Running {simulationCount} simulations...");
    }
}