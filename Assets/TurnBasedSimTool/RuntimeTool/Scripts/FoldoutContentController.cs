using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// Foldout 시 Content 크기 자동 조절
    ///
    /// 사용 케이스:
    /// - 3단계 이상 중첩 Foldout (Team > Character > Actions)
    /// - Scroll View 안에 Scroll View
    /// - Dynamic Add/Remove Content
    /// - Content Size Fitter 충돌 해결
    ///
    /// 사용법:
    /// 1. Toggle 오브젝트에 이 컴포넌트 추가
    /// 2. contentArea 할당 (접을 영역)
    /// 3. autoCalculateHeight = true (권장)
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class FoldoutContentController : MonoBehaviour
    {
        [SerializeField] private GameObject contentArea;
        [SerializeField] private LayoutElement contentLayoutElement; // 자동 찾기됨

        [Header("Size Settings")]
        [Tooltip("자동으로 Content 높이 계산 (권장)")]
        [SerializeField] private bool autoCalculateHeight = true;

        [Tooltip("수동 설정 시 펼쳤을 때 높이")]
        [SerializeField] private float manualExpandedHeight = 300f;

        [SerializeField] private float collapsedHeight = 0f;

        [Header("Nested Layout Support")]
        [Tooltip("부모 Layout을 몇 단계까지 업데이트할지 (중첩 대응)")]
        [SerializeField] private int parentUpdateDepth = 2;

        private Toggle _toggle;
        private RectTransform _rectTransform;
        private float _calculatedHeight = -1f;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _rectTransform = transform as RectTransform;

            // contentArea의 LayoutElement 자동 찾기/추가
            if (contentArea != null && contentLayoutElement == null)
            {
                contentLayoutElement = contentArea.GetComponent<LayoutElement>();

                if (contentLayoutElement == null)
                {
                    contentLayoutElement = contentArea.AddComponent<LayoutElement>();
                }
            }
        }

        private void Start()
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(OnToggleChanged);

                // 초기 상태: 펼쳐진 상태에서 높이 계산
                if (autoCalculateHeight && _toggle.isOn)
                {
                    // 다음 프레임에 계산 (Layout 완성 후)
                    StartCoroutine(CalculateHeightNextFrame());
                }
                else
                {
                    OnToggleChanged(_toggle.isOn);
                }
            }
        }

        private System.Collections.IEnumerator CalculateHeightNextFrame()
        {
            yield return null; // 1프레임 대기
            yield return null; // 2프레임 대기 (Layout 완성)

            CalculateExpandedHeight();
            OnToggleChanged(_toggle.isOn);
        }

        private void OnToggleChanged(bool isExpanded)
        {
            if (contentArea != null)
            {
                contentArea.SetActive(isExpanded);
            }

            // LayoutElement로 크기 제어
            if (contentLayoutElement != null)
            {
                float targetHeight = isExpanded ? GetExpandedHeight() : collapsedHeight;
                contentLayoutElement.preferredHeight = targetHeight;
                contentLayoutElement.minHeight = isExpanded ? -1 : collapsedHeight;
            }

            // 중첩 Layout 업데이트
            UpdateParentLayouts();
        }

        /// <summary>
        /// 펼쳤을 때 높이 반환
        /// </summary>
        private float GetExpandedHeight()
        {
            if (autoCalculateHeight)
            {
                // 자동 계산된 높이가 있으면 사용
                return _calculatedHeight > 0 ? _calculatedHeight : manualExpandedHeight;
            }
            else
            {
                // 수동 설정값 사용
                return manualExpandedHeight;
            }
        }

        /// <summary>
        /// 펼쳤을 때 높이를 자동 계산
        /// </summary>
        public void CalculateExpandedHeight()
        {
            if (!autoCalculateHeight || contentArea == null)
                return;

            RectTransform contentRect = contentArea.GetComponent<RectTransform>();
            if (contentRect != null)
            {
                Canvas.ForceUpdateCanvases();
                _calculatedHeight = LayoutUtility.GetPreferredHeight(contentRect);

                Debug.Log($"[{gameObject.name}] Calculated Height: {_calculatedHeight}");
            }
        }

        /// <summary>
        /// 부모 Layout들을 재귀적으로 업데이트
        /// </summary>
        private void UpdateParentLayouts()
        {
            Canvas.ForceUpdateCanvases();

            Transform current = transform;
            for (int i = 0; i < parentUpdateDepth && current != null; i++)
            {
                current = current.parent;
                if (current != null)
                {
                    RectTransform rectParent = current as RectTransform;
                    if (rectParent != null)
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rectParent);
                    }
                }
            }
        }

        /// <summary>
        /// 외부에서 강제로 높이 재계산 (Content 변경 시)
        /// </summary>
        public void RecalculateHeight()
        {
            if (autoCalculateHeight)
            {
                CalculateExpandedHeight();
                OnToggleChanged(_toggle != null && _toggle.isOn);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Calculate Height Now")]
        private void DebugCalculateHeight()
        {
            CalculateExpandedHeight();
            Debug.Log($"Calculated: {_calculatedHeight}, Manual: {manualExpandedHeight}");
        }
#endif
    }
}
