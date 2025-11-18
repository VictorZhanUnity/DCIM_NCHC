using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _VictorDev.TextUtils.EditableTextComponent
{
    /// 可編輯文字項
    public class EditableText : MonoBehaviour
    {
        #region Variables

        [field: SerializeField] public bool IsEditable { get; private set; } = true;

        [Foldout("[組件]"), SerializeField] private TextMeshProUGUI txtTitle;
        [Foldout("[組件]"), SerializeField] private TMP_InputField inputField;

        public string Title => txtTitle.text.Trim();
        public string Text => inputField.text.Trim();
        
        #endregion

        /// 設定標題
        public void SetTitle(string title) => txtTitle.text = title.Trim();
        /// 設定內容
        public void SetText(string txt) => inputField.text = txt.Trim();
        /// 設定可編輯
        public void SetIsEnableEdit(bool isEnableEdit)
        {
            IsEditable = isEnableEdit;
            inputField.interactable = IsEditable;
        }

        private void OnValidate() => SetIsEnableEdit(IsEditable);

        [Button]
        private void Reset()
        {
            txtTitle = GetComponentInChildren<TextMeshProUGUI>();
            inputField = GetComponentInChildren<TMP_InputField>();
        }
    }
}
