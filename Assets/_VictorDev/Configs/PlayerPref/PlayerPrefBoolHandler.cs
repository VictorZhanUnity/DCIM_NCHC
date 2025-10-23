using UnityEngine;

namespace VictorDev.Configs
{
    public class PlayerPrefBoolHandler : PlayerPrefParent<bool>
    {
        private void Start() => onValueLoaded?.Invoke(PlayerPrefs.GetInt(TagName, DefaultValue ? 1 : 0) == 1);

        public override void Receive(bool value) => PlayerPrefs.SetInt(TagName, value ? 1 : 0);
    }
}