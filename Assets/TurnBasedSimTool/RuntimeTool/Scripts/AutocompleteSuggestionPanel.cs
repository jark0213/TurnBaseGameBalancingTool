using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// InputField 아래에 나타나는 자동완성 제안 패널
    /// </summary>
    public class AutocompleteSuggestionPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject suggestionItemPrefab;
        [SerializeField] private Transform suggestionListContent;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject backgroundPanel; // 외부 클릭 감지용 배경

        [Header("Settings")]
        [SerializeField] private int maxSuggestions = 7;
        [SerializeField] private int canvasSortOrder = 100; // 다른 UI 위에 표시

        private List<string> _allOptions = new List<string>();
        private TMP_InputField _targetInputField;
        private System.Action<string> _onSuggestionSelected;
        private Canvas _overlayCanvas;
        private Camera _uiCamera;

        private bool _backgroundSetup = false; // 한 번만 설정

        private void Awake()
        {
            // Overlay Canvas를 먼저 설정 (Initialize 전에도 작동)
            SetupOverlayCanvas();

            // Background Panel 클릭 이벤트 설정 (한 번만)
            if (!_backgroundSetup)
            {
                SetupBackgroundPanel();
                _backgroundSetup = true;
            }
        }

        /// <summary>
        /// Background Panel 클릭 시 패널 닫기
        /// </summary>
        private void SetupBackgroundPanel()
        {
            if (backgroundPanel == null)
            {
                Debug.LogWarning("[Autocomplete] backgroundPanel is null!");
                return;
            }

            Debug.Log("[Autocomplete] Setting up background panel: " + backgroundPanel.name);

            // Button 컴포넌트로 클릭 감지
            var button = backgroundPanel.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("[Autocomplete] Button found, adding onClick listener");
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log("[Autocomplete] Background clicked!");
                    Hide();
                });
            }
            else
            {
                Debug.LogWarning("[Autocomplete] Button component not found on backgroundPanel!");
            }

            // GraphicRaycaster가 있는지 확인
            var raycaster = backgroundPanel.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogWarning("[Autocomplete] GraphicRaycaster not found on backgroundPanel!");
            }
        }

        /// <summary>
        /// 자동완성 패널 초기화
        /// </summary>
        public void Initialize(List<string> options, TMP_InputField targetInput, System.Action<string> onSelected)
        {
            _allOptions = options ?? new List<string>();
            _targetInputField = targetInput;
            _onSuggestionSelected = onSelected;

            // UI Camera 찾기
            _uiCamera = FindUICamera();

            // 초기에는 Background와 패널 모두 숨김
            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // 패널이 활성화되어 있을 때만 위치 업데이트
            if (gameObject.activeSelf && _targetInputField != null)
            {
                UpdatePosition();
            }
        }

        /// <summary>
        /// Overlay Canvas 설정 (다른 UI 위에 표시)
        /// </summary>
        private void SetupOverlayCanvas()
        {
            // 이미 Canvas가 있는지 확인
            _overlayCanvas = GetComponent<Canvas>();

            if (_overlayCanvas == null)
            {
                _overlayCanvas = gameObject.AddComponent<Canvas>();
            }

            // Overlay 모드로 설정
            _overlayCanvas.overrideSorting = true;
            _overlayCanvas.sortingOrder = canvasSortOrder;

            // GraphicRaycaster 추가 (클릭 이벤트 처리)
            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        /// <summary>
        /// UI Camera 찾기
        /// </summary>
        private Camera FindUICamera()
        {
            // InputField의 Canvas에서 Camera 가져오기
            if (_targetInputField != null)
            {
                Canvas parentCanvas = _targetInputField.GetComponentInParent<Canvas>();
                if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    return parentCanvas.worldCamera;
                }
            }

            return null; // Screen Space Overlay인 경우 null
        }

        /// <summary>
        /// 스크린 좌표 기반으로 InputField 아래에 패널 위치 업데이트
        /// GridLayoutGroup 등 동적 레이아웃에도 대응
        /// </summary>
        private void UpdatePosition()
        {
            if (_targetInputField == null)
                return;

            RectTransform panelRect = GetComponent<RectTransform>();
            RectTransform inputRect = _targetInputField.GetComponent<RectTransform>();

            if (panelRect == null || inputRect == null)
                return;

            // InputField의 부모 GameObject (Label + InputField)
            RectTransform inputParentRect = _targetInputField.transform.parent.GetComponent<RectTransform>();
            RectTransform targetRect = inputParentRect != null ? inputParentRect : inputRect;

            // 월드 좌표의 모서리 가져오기
            Vector3[] worldCorners = new Vector3[4];
            targetRect.GetWorldCorners(worldCorners);

            // 왼쪽 아래 모서리 (worldCorners[0])
            Vector3 bottomLeft = worldCorners[0];

            // 패널은 별도 Canvas이므로 직접 스크린 좌표를 World Position으로 사용
            panelRect.position = bottomLeft + new Vector3(0, -5f, 0); // 5px 아래

            // 너비는 InputField 부모와 동일하게
            float targetWidth = targetRect.rect.width;
            panelRect.sizeDelta = new Vector2(targetWidth, 200f); // 높이 200px

            // Pivot 설정 (왼쪽 위 기준)
            panelRect.pivot = new Vector2(0, 1);
        }

        /// <summary>
        /// 입력값에 따라 제안 목록 업데이트
        /// </summary>
        public void UpdateSuggestions(string inputText)
        {
            // 기존 항목 제거
            ClearSuggestions();

            // 필터링된 옵션 가져오기
            List<string> filteredOptions = FilterOptions(inputText);

            if (filteredOptions.Count == 0)
            {
                // Background도 함께 숨김
                if (backgroundPanel != null)
                    backgroundPanel.SetActive(false);
                gameObject.SetActive(false);
                return;
            }

            // 제안 항목 생성
            foreach (var option in filteredOptions.Take(maxSuggestions))
            {
                CreateSuggestionItem(option);
            }

            // Background 먼저 표시 (뒤에 배치)
            if (backgroundPanel != null)
                backgroundPanel.SetActive(true);

            // 패널 표시
            gameObject.SetActive(true);

            // 위치 즉시 업데이트 (Update 대기 없이)
            UpdatePosition();

            // ScrollRect를 맨 위로 리셋
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        /// <summary>
        /// 옵션 필터링 (대소문자 무시, 부분 일치)
        /// </summary>
        private List<string> FilterOptions(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                // 입력이 비어있으면 모든 옵션 반환
                return _allOptions.ToList();
            }

            string lowerInput = inputText.ToLower();

            // 시작 일치 우선, 그 다음 부분 일치
            var startsWith = _allOptions.Where(o => o.ToLower().StartsWith(lowerInput)).ToList();
            var contains = _allOptions.Where(o => o.ToLower().Contains(lowerInput) && !startsWith.Contains(o)).ToList();

            return startsWith.Concat(contains).ToList();
        }

        /// <summary>
        /// 제안 항목 생성
        /// </summary>
        private void CreateSuggestionItem(string optionText)
        {
            if (suggestionItemPrefab == null || suggestionListContent == null)
                return;

            GameObject item = Instantiate(suggestionItemPrefab, suggestionListContent);

            // 버튼에 텍스트 설정
            var textComponent = item.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = optionText;
            }

            // 버튼 클릭 이벤트 연결
            var button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnSuggestionSelected(optionText));
            }
        }

        /// <summary>
        /// 제안 항목 선택 시
        /// </summary>
        private void OnSuggestionSelected(string selectedText)
        {
            // InputField에 텍스트 설정
            if (_targetInputField != null)
            {
                _targetInputField.text = selectedText;
                _targetInputField.Select(); // 다시 포커스
            }

            // 콜백 호출
            _onSuggestionSelected?.Invoke(selectedText);

            // 패널 숨기기
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 모든 제안 항목 제거
        /// </summary>
        private void ClearSuggestions()
        {
            if (suggestionListContent == null)
                return;

            foreach (Transform child in suggestionListContent)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// 패널 숨기기 (Background도 함께 숨김)
        /// </summary>
        public void Hide()
        {
            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 패널 표시 여부
        /// </summary>
        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }
    }
}
