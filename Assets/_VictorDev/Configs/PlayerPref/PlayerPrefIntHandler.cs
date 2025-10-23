using System;
using UnityEngine;

namespace VictorDev.Configs
{
    public class PlayerPrefParent : PlayerPrefParent<int>
    {
        private void Start() => onValueLoaded?.Invoke(PlayerPrefs.GetInt(TagName, DefaultValue));
        public override void Receive(int value) => PlayerPrefs.SetInt(TagName, value);
    }
}