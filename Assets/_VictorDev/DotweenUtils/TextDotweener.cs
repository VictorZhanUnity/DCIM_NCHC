using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace VictorDev.DoTweenUtils
{
    public class TextDotweener : MonoBehaviour
    {
        public void SetText(string str)
        {
            str = str.Trim();
            if(Application.isPlaying) DotweenHelper.ToBlink(TxtComp, str, duration, delay, isRandomDelay);
            else TxtComp.text = str;
        }

        public string text
        {
            set => SetText(value);
        }

        [Foldout("設定")] [SerializeField] private float duration = 0.15f;
        [Foldout("設定")] [SerializeField] private float delay = 0.3f;
        [Foldout("設定")] [SerializeField] private bool isRandomDelay = true;
        
        private TextMeshProUGUI TxtComp => _txt ??= GetComponent<TextMeshProUGUI>();
        [NonSerialized] private TextMeshProUGUI _txt;
    }
}