using UnityEngine;

namespace VictorDev.Configs
{
    public class PlayerPrefStringHandler : PlayerPrefParent<string>
    {
        private void Start() => onValueLoaded?.Invoke(PlayerPrefs.GetString(TagName, DefaultValue));

        public override void Receive(string value) => PlayerPrefs.SetString(TagName, value);
    }
}