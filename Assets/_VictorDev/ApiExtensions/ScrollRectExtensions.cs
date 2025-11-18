using DG.Tweening;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

namespace _VictorDev.ApiExtensions
{
    public static class ScrollRectExtensions
    {
        public static void ScrollToChild(this ScrollRect scrollRect, RectTransform target)
        {
            if (scrollRect == null || scrollRect.content == null || target == null)
                return;

            Canvas.ForceUpdateCanvases(); // 避免 Layout 尚未更新導致位置錯誤

            RectTransform content = scrollRect.content;

            // 1️⃣ 目標位置與 Content 的距離（世界座標 → 本地座標）
            Vector2 localPos = content.InverseTransformPoint(target.position);
            Vector2 contentPivotPos = content.InverseTransformPoint(content.position);

            float diffY = contentPivotPos.y - localPos.y;

            // 2️⃣ 把這個 offset 轉換成 normalizedPosition
            float scrollHeight = content.rect.height - scrollRect.viewport.rect.height;

            if (scrollHeight <= 0f)
            {
                scrollRect.verticalNormalizedPosition = 1f;
                return;
            }

            // Unity 的 normalizedPosition 垂直方向：1 = 上，0 = 下 （反直覺但就是這樣）
            float normalized = diffY / scrollHeight;

            // Clamp 避免超界
            normalized = Mathf.Clamp01(normalized - 0.08f); //位移4行

            scrollRect.verticalNormalizedPosition = 1 - normalized;
            Debug.Log($"normalized: {1-normalized} / {normalized}");
        }
        public static void ScrollToChild1(this ScrollRect scrollRect, RectTransform target)
        {
            Canvas.ForceUpdateCanvases(); // 確保 layout 更新完成

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            // 取得 target 在 content 座標中的位置
            Vector2 childLocalPos = content.InverseTransformPoint(target.position);
            Vector2 viewportLocalPos = content.InverseTransformPoint(viewport.position);

            float contentHeight = content.rect.height;
            float viewportHeight = viewport.rect.height;

            float childY = childLocalPos.y;
            float vpY = viewportLocalPos.y;

            // child 高度
            float childHeight = target.rect.height;

            // child 相對於 viewport 的位置
            float offset = childY - vpY;

            // 若 child 在 viewport 下方 -> 捲上去
            if (offset < -childHeight * 0.2f)
            {
                float normalized =  Mathf.Abs(childLocalPos.y - (viewportHeight * 0.5f)) / (contentHeight - viewportHeight);
                scrollRect.verticalNormalizedPosition =  Mathf.Clamp01(1 -normalized);
            Debug.Log($"scrollRect.verticalNormalizedPosition: {scrollRect.verticalNormalizedPosition} / {normalized}");
            }
            // 若 child 在 viewport 上方 -> 捲下來
            else if (offset > viewportHeight - childHeight * 0.8f)
            {
                float normalized =   Mathf.Abs (childLocalPos.y - (viewportHeight * 0.5f)) / (contentHeight - viewportHeight);
                scrollRect.verticalNormalizedPosition =Mathf.Clamp01(1 -normalized);
            Debug.Log($"scrollRect.verticalNormalizedPosition: {scrollRect.verticalNormalizedPosition} / {normalized}");
            }
            
        }
    }
}