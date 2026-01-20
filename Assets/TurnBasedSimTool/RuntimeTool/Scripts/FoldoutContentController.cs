using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// Foldout 시 Content 접기/펼치기
    /// Unity Layout System이 자동으로 크기를 재계산합니다.
    ///
    /// 사용법:
    /// 1. Toggle 오브젝트에 이 컴포넌트 추가
    /// 2. contentArea 할당 (접을 영역)
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class FoldoutContentController : MonoBehaviour
    {
        [SerializeField] private GameObject contentArea;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void Start()
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(OnToggleChanged);

                // 초기 상태 적용
                if (contentArea != null)
                {
                    contentArea.SetActive(_toggle.isOn);
                }
            }
        }

        private void OnToggleChanged(bool isExpanded)
        {
            if (contentArea != null)
            {
                contentArea.SetActive(isExpanded);
            }
        }
    }
}
