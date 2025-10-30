using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using _VictorDev.ApiExtensions;
using _VictorDev.DebugUtils;

namespace _VictorDev.Frameworks
{
    /// 物件選取外框
    public class ObjectSelectionController : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event] - 當選取物件時")] public UnityEvent<Transform> onSelectObjectEvent;
        [Foldout("[Event] - 當取消選取時")] public UnityEvent unSelectObjectEvent;
        [Foldout("[設定]"), SerializeField] private Transform selectionBorder;
        [Foldout("[設定]"), SerializeField] private bool isBorderFollowTarget = true;
        [Foldout("[設定]"), SerializeField] private bool isSingleSelection = true;

        private Transform _currentSelectTarget;
        
        #endregion

        /// 選取物件
        public void SelectObject(Transform target)
        {
            if(isSingleSelection && _currentSelectTarget != null) DeselectObject();
            _currentSelectTarget = target;
            ObjectHelper.SetMatchSizeAndPosition(selectionBorder, target);
            selectionBorder.gameObject.SetActive(true);
            onSelectObjectEvent?.Invoke(target);
        }

        /// 取消選取物件
        public void DeselectObject()
        {
            selectionBorder.gameObject.SetActive(false);
            _currentSelectTarget = null;
            unSelectObjectEvent?.Invoke();
        }

        private void Update()
        {
            if (isBorderFollowTarget && _currentSelectTarget != null)
            {
                selectionBorder.FollowTarget(_currentSelectTarget);
            }
        }

        private void OnValidate()
        {
            selectionBorder ??= transform.GetChild(0);
            selectionBorder?.gameObject.SetActive(false);
        }

        private void Start() => OnValidate();
    }
}