using System.Collections.Generic;
using TurnBasedSim.Core;
using UnityEngine;

public class SimUIManager : MonoBehaviour
{
    // ... UI 참조들 ...

    // 핵심: 툴이 실행될 때 어떤 액션들을 선택지에 넣을지 리스트로 관리
    private List<IBattleAction> _availableActions = new List<IBattleAction>();

    public void Init(List<IBattleAction> actions)
    {
        _availableActions = actions;
        // 드롭다운이나 버튼 리스트를 _availableActions 기반으로 생성
        //UpdateActionSelectionUI();
    }

    public void OnRunClick()
    {
        // 1. UI에서 유닛 스탯 읽기
        // 2. 선택된 액션들로 페이즈 구성
        // 3. MonteCarloRunner 실행
    }
}