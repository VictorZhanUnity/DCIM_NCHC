using _VictorDev.ApiExtensions;
using _VictorDev.ObjectUtils;
using NaughtyAttributes;
using UnityEngine;
using Debug = _VictorDev.MediatorUtils.Debug;

namespace _VictorDev.Advanced
{
    /// 把UI跟隨鼠標移動
    [RequireComponent(typeof(RectTransform))]
    public class FollowMouseUI : MonoBehaviour
    {
        #region Variables

        [Label("與滑鼠的偏移"), SerializeField] private Vector2 offset = new Vector2(20f, 20f);
        [Foldout("[組件]"), SerializeField] private RectTransform uiTarget;
        [Foldout("[組件]"), SerializeField] private Canvas canvas;

        private RectTransform canvasRectTransform;
        
        #endregion

        #region Initialized
        private void Awake() => canvasRectTransform = canvas.transform as RectTransform;

        private void OnValidate()
        {
            uiTarget ??= transform as RectTransform;
            if (canvas == null && transform.TryGetComponentInParent(out canvas) == false) 
                Debug.LogError("FollowMouseUI 必須放在 Canvas 子物件中！");
        }
        #endregion

        private void Update()
        {
            if (uiTarget == null || canvas == null) return;

            // 取得滑鼠座標 (螢幕座標)
            Vector2 mousePos = Input.mousePosition;

            // 轉換為 UI 的座標
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, mousePos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localPoint
            );

            // 加上偏移
            localPoint += offset;

            // 將座標設置到 UI 上
            uiTarget.localPosition = UiHelper.ClampUIToScreen(localPoint, uiTarget, canvasRectTransform);
        }
    }
}