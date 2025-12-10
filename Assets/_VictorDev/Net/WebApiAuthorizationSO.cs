using _VictorDev.Net.WebAPI;
using NaughtyAttributes;
using UnityEngine;

namespace VictorDev.Net.WebAPI
{
    /// WebAPI Authorization設定
    [CreateAssetMenu(fileName = "WebApiAuthorization", menuName = "VictorDev/Net/WebApiAuthorization")]
    public class WebApiAuthorizationSO : ScriptableObject
    {
        #region Variables

        [SerializeField] private EnumAuthorizationType authorizationTypeType = EnumAuthorizationType.Bearer;
        public bool IsHaveAuth => authorizationTypeType != EnumAuthorizationType.NoAuth;

        [TextArea(1, 23), ShowIf(nameof(IsHaveAuth)), SerializeField]
        private string token;

        public EnumAuthorizationType AuthorizationTypeType => authorizationTypeType;
        public string Token => token;

        #endregion

        public void SetToken(string data, EnumAuthorizationType authorizationType)
        {
            authorizationTypeType = authorizationType;
            SetToken(data);
        }
        public void SetToken(string data) => token = data;

        [Button]
        private void ClearToken()
        {
            authorizationTypeType = EnumAuthorizationType.NoAuth;
            token = string.Empty;
        }
    }
}