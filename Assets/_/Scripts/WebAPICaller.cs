using System.Collections;
using System.Collections.Generic;
using System.Text;
using _VictorDev.ApiExtensions;
using _VictorDev.Net.WebAPI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace _VictorDev.Framework.WebAPI
{
    public class WebAPICaller : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event] - 是否在Loading")] public UnityEvent<bool> onLoadingEvent;
        [Foldout("[Event] - Response內容")] public UnityEvent<string> onResponseSuccessEvent, onResponseErrorEvent;
        [Foldout("[連線設定]"), SerializeField] private string url = "http://192.168.0.107:5080/api/Auth/login";
        [Foldout("[連線設定]"), SerializeField] private int timeoutSeconds = 10;
        [Foldout("[連線設定]"), SerializeField] private EnumHttpMethod httpMethod = EnumHttpMethod.POST;
        [Foldout("[連線設定]"), SerializeField, ShowIf(nameof(IsNotGetMethod))] private EnumBody sendBodyType = EnumBody.FormData;
        [Foldout("[認證設定]"), SerializeField]
        private bool useAuthorization = false;

        [Foldout("[認證設定]"), SerializeField, ShowIf(nameof(useAuthorization))]
        private string bearerToken;
        
        private bool IsNotGetMethod => httpMethod != EnumHttpMethod.GET;
        private bool IsGetOrFormData => sendBodyType == EnumBody.FormData || httpMethod == EnumHttpMethod.GET;
        private bool IsSendJson => sendBodyType == EnumBody.RawJson;

        [Foldout("[連線設定]"), SerializeField, ShowIf(nameof(IsGetOrFormData))]
        private List<KeyValueData<string, string>> paramsSetting;

        [Foldout("[連線設定]"), SerializeField, ShowIf(nameof(IsSendJson))]
        private string sendBodyJson;

        #endregion

        [Button]
        public void CallAPI()
        {
            onLoadingEvent?.Invoke(true);
            StartCoroutine(CoroutineHandler());
        }

        IEnumerator CoroutineHandler()
        {
            UnityWebRequest request = null;

            switch (httpMethod)
            {
                case EnumHttpMethod.GET:
                    request = BuildGetRequest();
                    break;

                case EnumHttpMethod.POST:
                    request = BuildPostRequest();
                    break;

                case EnumHttpMethod.PATCH:
                    request = BuildPatchRequest();
                    break;
            }

            if (request == null)
            {
                onResponseErrorEvent?.Invoke("Request build failed");
                onLoadingEvent?.Invoke(false);
                yield break;
            }
            request.timeout = timeoutSeconds; // timeout時會觸發：UnityWebRequest.Result.ConnectionError
            if (useAuthorization && !string.IsNullOrEmpty(bearerToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");
            }
            yield return request.SendWebRequest();

            string msg;
            if (request.result != UnityWebRequest.Result.Success)
            {
                msg = $"[{request.error}]\n{request.downloadHandler.text}";
                Debug.LogError(msg);
                onResponseErrorEvent?.Invoke(msg);
            }
            else
            {
                msg = request.downloadHandler.text.ToJsonFormat();
                Debug.Log(msg);
                onResponseSuccessEvent?.Invoke(msg);
            }

            request.Dispose();
            onLoadingEvent?.Invoke(false);
        }

        private UnityWebRequest BuildGetRequest()
        {
            string finalUrl = url;

            if (paramsSetting != null && paramsSetting.Count > 0)
            {
                List<string> query = new();
                foreach (var kv in paramsSetting)
                {
                    query.Add($"{UnityWebRequest.EscapeURL(kv.Key)}={UnityWebRequest.EscapeURL(kv.Value)}");
                }
                finalUrl += "?" + string.Join("&", query);
            }
            return UnityWebRequest.Get(finalUrl);
        }

        UnityWebRequest BuildPostRequest()
        {
            switch (sendBodyType)
            {
                case EnumBody.FormData:
                    WWWForm form = new WWWForm();
                    foreach (var kv in paramsSetting)
                    {
                        form.AddField(kv.Key, kv.Value);
                    }
                    return UnityWebRequest.Post(url, form);
                case EnumBody.RawJson:
                    var request = new UnityWebRequest(url, "POST");
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(sendBodyJson);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    return request;
            }

            return null;
        }

        UnityWebRequest BuildPatchRequest()
        {
            var request = new UnityWebRequest(url, "PATCH");

            switch (sendBodyType)
            {
                case EnumBody.RawJson:
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(sendBodyJson);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                    break;
                case EnumBody.FormData:
                    WWWForm form = new WWWForm();
                    foreach (var kv in paramsSetting)
                    {
                        form.AddField(kv.Key, kv.Value);
                    }

                    request.uploadHandler = new UploadHandlerRaw(form.data);
                    // ⚠ PATCH + FormData 視後端而定，必要時改用 JSON
                    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    break;
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }
    }
}