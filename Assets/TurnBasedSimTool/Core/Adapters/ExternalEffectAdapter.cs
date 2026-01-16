namespace TurnBasedSimTool.Core.Adapters
{
    /// <summary>
    /// 외부 이펙트 시스템을 시뮬레이터와 연결하기 위한 어댑터 베이스 클래스
    ///
    /// 사용법:
    /// 1. 이 클래스를 상속받습니다
    /// 2. ExecuteExternalEffect 메서드에 자신의 이펙트 시스템 호출 로직을 작성합니다
    /// 3. Clone() 메서드를 구현합니다 (상태 초기화 필요 시)
    ///
    /// 예시:
    /// <code>
    /// public class DSLEffectAdapter : ExternalEffectAdapter
    /// {
    ///     private IEffect _compiledEffect;
    ///     private string _dslCode;
    ///
    ///     public DSLEffectAdapter(string dsl)
    ///     {
    ///         _dslCode = dsl;
    ///         _compiledEffect = CompiledEffectCache.GetOrParse(dsl);
    ///     }
    ///
    ///     public override string ActionName => _compiledEffect.EffectName;
    ///
    ///     protected override void ExecuteExternalEffect(
    ///         IBattleUnit attacker,
    ///         IBattleUnit defender,
    ///         BattleContext context)
    ///     {
    ///         var runtimeEffect = EffectDeepCloner.Clone(_compiledEffect);
    ///         runtimeEffect.Execute(attacker, defender);
    ///     }
    ///
    ///     public override IBattleAction Clone()
    ///     {
    ///         return new DSLEffectAdapter(_dslCode);
    ///     }
    /// }
    /// </code>
    /// </summary>
    public abstract class ExternalEffectAdapter : IBattleAction
    {
        public abstract string ActionName { get; }

        /// <summary>
        /// 코스트 시스템을 사용하지 않는 경우 기본값 0 반환
        /// 코스트를 사용하는 경우 오버라이드하세요
        /// </summary>
        public virtual int GetCost(IBattleState state) => 0;

        /// <summary>
        /// 조건부 실행이 필요하면 오버라이드하세요
        /// 기본값은 항상 실행 가능
        /// </summary>
        public virtual bool CanExecute(IBattleState state) => true;

        /// <summary>
        /// 외부 이펙트 시스템을 호출하는 메서드
        /// 반드시 구현해야 합니다
        ///
        /// 이 메서드에서 자신의 게임 프로젝트의 이펙트 시스템을 호출하세요
        /// 예: DSL 파서, ScriptableObject 스킬 시스템, 스프레드시트 데이터 등
        /// </summary>
        protected abstract void ExecuteExternalEffect(
            IBattleUnit attacker,
            IBattleUnit defender,
            BattleContext context
        );

        /// <summary>
        /// 액션 실행 (템플릿 메서드 패턴)
        /// 전처리 -> 외부 시스템 호출 -> 후처리 순으로 실행됩니다
        /// </summary>
        public void Execute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 전처리 (공통 로직)
            OnBeforeExecute(attacker, defender, context);

            // 외부 시스템 호출
            ExecuteExternalEffect(attacker, defender, context);

            // 후처리 (공통 로직)
            OnAfterExecute(attacker, defender, context);
        }

        /// <summary>
        /// 액션 실행 전 처리
        /// 필요시 오버라이드하여 로깅, 애니메이션 등을 추가할 수 있습니다
        /// </summary>
        protected virtual void OnBeforeExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 기본 구현 없음 (옵션)
        }

        /// <summary>
        /// 액션 실행 후 처리
        /// 필요시 오버라이드하여 로깅, 애니메이션 등을 추가할 수 있습니다
        /// </summary>
        protected virtual void OnAfterExecute(IBattleUnit attacker, IBattleUnit defender, BattleContext context)
        {
            // 기본 구현 없음 (옵션)
        }

        /// <summary>
        /// 몬테카를로 시뮬레이션을 위한 깊은 복사
        /// 반드시 구현해야 합니다
        ///
        /// 주의: 상태를 가진 액션은 반드시 새 인스턴스를 반환해야 합니다
        /// </summary>
        public abstract IBattleAction Clone();
    }
}
