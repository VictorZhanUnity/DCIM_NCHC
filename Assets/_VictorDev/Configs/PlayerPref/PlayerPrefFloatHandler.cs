using UnityEngine;

namespace VictorDev.Configs
{
    public class PlayerPrefFloatHandler : PlayerPrefParent<float>
    {
        private void Start() => onValueLoaded?.Invoke(PlayerPrefs.GetFloat(TagName, DefaultValue));

        public override void Receive(float value) => PlayerPrefs.SetFloat(TagName, value);
    }
}