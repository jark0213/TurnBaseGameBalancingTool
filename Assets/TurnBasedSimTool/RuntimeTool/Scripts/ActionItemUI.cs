using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurnBasedSimTool.Runtime
{
    public class ActionItemUI : MonoBehaviour
    {
        [Header("UI References")]
        public Toggle includeToggle;
        public TMP_InputField nameInput;
        public TMP_InputField valueInput;
        [SerializeField] private TMP_InputField intervalInput;
        [SerializeField] private TMP_InputField costInput;

        [Header("Ranged Damage (Optional)")]
        [SerializeField] private TMP_InputField minDamageInput;
        [SerializeField] private TMP_InputField maxDamageInput;
        [SerializeField] private Toggle useRangedDamageToggle;

        // 자동으로 찾을 Parent GameObjects
        private GameObject valueInputParent;
        private GameObject minDamageInputParent;
        private GameObject maxDamageInputParent;
        private GameObject costInputParent;

        private void Awake()
        {
            // InputField의 부모 GameObject를 자동으로 찾기
            if (valueInput != null)
                valueInputParent = valueInput.transform.parent.gameObject;

            if (minDamageInput != null)
                minDamageInputParent = minDamageInput.transform.parent.gameObject;

            if (maxDamageInput != null)
                maxDamageInputParent = maxDamageInput.transform.parent.gameObject;

            if (costInput != null)
                costInputParent = costInput.transform.parent.gameObject;
        }

        private void Start()
        {
            // 토글 변경 시 필드 활성화/비활성화
            if (useRangedDamageToggle != null)
            {
                useRangedDamageToggle.onValueChanged.AddListener(OnRangedDamageToggleChanged);
                // 초기 상태 설정
                OnRangedDamageToggleChanged(useRangedDamageToggle.isOn);
            }
        }

        private void OnRangedDamageToggleChanged(bool isOn)
        {
            // 랜덤 데미지 ON → Min/Max 활성화, 고정 데미지 비활성화
            // 랜덤 데미지 OFF → 고정 데미지 활성화, Min/Max 비활성화

            if (valueInputParent != null)
                valueInputParent.SetActive(!isOn);

            if (minDamageInputParent != null)
                minDamageInputParent.SetActive(isOn);

            if (maxDamageInputParent != null)
                maxDamageInputParent.SetActive(isOn);
        }

        /// <summary>
        /// 코스트 필드 활성화/비활성화 (시뮬레이션 설정에서 코스트 시스템 사용 여부에 따라)
        /// </summary>
        public void SetCostFieldActive(bool active)
        {
            if (costInputParent != null)
                costInputParent.SetActive(active);
        }

        public int IntervalValue
        {
            get
            {
                // 입력이 없거나 숫자가 아니면 기본값 1(매 턴) 반환
                if (intervalInput != null && int.TryParse(intervalInput.text, out int result))
                    return result > 0 ? result : 1;
                return 1;
            }
        }

        public int CostValue
        {
            get
            {
                // 입력이 없거나 숫자가 아니면 기본값 0(코스트 없음) 반환
                if (costInput != null && int.TryParse(costInput.text, out int result))
                    return result >= 0 ? result : 0;
                return 0;
            }
        }

        public bool UseRangedDamage => useRangedDamageToggle != null && useRangedDamageToggle.isOn;

        public int MinDamage
        {
            get
            {
                if (minDamageInput != null && int.TryParse(minDamageInput.text, out int result))
                    return result;
                return 0;
            }
        }

        public int MaxDamage
        {
            get
            {
                if (maxDamageInput != null && int.TryParse(maxDamageInput.text, out int result))
                    return result;
                return 0;
            }
        }

        // 시뮬레이터가 데이터를 가져갈 때 사용할 프로퍼티
        public bool IsSelected => includeToggle != null && includeToggle.isOn;
        public string ActionName => nameInput != null ? nameInput.text : "Unknown";

        public int ActionValue
        {
            get
            {
                if (valueInput != null && int.TryParse(valueInput.text, out int result))
                    return result;
                return 0;
            }
        }

        // 나중에 데이터 로드 시 사용할 초기화 함수
        public void Setup(string actionName, int value, bool selected = true, int interval = 1, int cost = 0)
        {
            if (nameInput) nameInput.text = actionName;
            if (valueInput) valueInput.text = value.ToString();
            if (includeToggle) includeToggle.isOn = selected;
            if (intervalInput) intervalInput.text = interval.ToString();
            if (costInput) costInput.text = cost.ToString();
        }

        public void SetupRanged(string actionName, int minDmg, int maxDmg, bool selected = true, int interval = 1, int cost = 0)
        {
            if (nameInput) nameInput.text = actionName;
            if (minDamageInput) minDamageInput.text = minDmg.ToString();
            if (maxDamageInput) maxDamageInput.text = maxDmg.ToString();
            if (useRangedDamageToggle) useRangedDamageToggle.isOn = true;
            if (includeToggle) includeToggle.isOn = selected;
            if (intervalInput) intervalInput.text = interval.ToString();
            if (costInput) costInput.text = cost.ToString();
        }

        /// <summary>
        /// 액션 데이터 설정 (로드 시 사용)
        /// </summary>
        public void SetActionData(string actionName, int damage, int maxDamage, int cost, int interval, bool useRangedDamage)
        {
            if (useRangedDamage)
            {
                SetupRanged(actionName, damage, maxDamage, true, interval, cost);
            }
            else
            {
                Setup(actionName, damage, true, interval, cost);
            }
        }
    }
}
