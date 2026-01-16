using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedSimTool.Runtime
{
    /// <summary>
    /// 컨텐츠 크기에 따라 ScrollRect를 자동으로 활성화/비활성화
    /// 스크롤할 필요가 없으면 비활성화하여 부모 ScrollView에 터치 이벤트 전달
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScrollController : MonoBehaviour
    {
        private ScrollRect _scrollRect;
        private RectTransform _viewport;
        private RectTransform _content;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _viewport = _scrollRect.viewport;
            _content = _scrollRect.content;
        }

        private void Start()
        {
            // 초기 체크
            UpdateScrollState();
        }

        /// <summary>
        /// 컨텐츠 크기를 확인하고 스크롤 필요 여부 판단
        /// </summary>
        public void UpdateScrollState()
        {
            if (_content == null || _viewport == null || _scrollRect == null)
                return;

            // Canvas가 업데이트될 때까지 대기 (레이아웃 재계산 후)
            Canvas.ForceUpdateCanvases();

            float contentHeight = _content.rect.height;
            float viewportHeight = _viewport.rect.height;

            // 컨텐츠가 뷰포트보다 작으면 스크롤 비활성화
            _scrollRect.enabled = contentHeight > viewportHeight;
        }

        /// <summary>
        /// 외부에서 컨텐츠 변경 시 호출 (예: 아이템 추가/삭제)
        /// </summary>
        public void OnContentChanged()
        {
            // 다음 프레임에 레이아웃이 업데이트된 후 체크
            Invoke(nameof(UpdateScrollState), 0.1f);
        }
    }
}
