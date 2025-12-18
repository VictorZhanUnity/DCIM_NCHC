using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _VictorDev.ApiExtensions;
using _VictorDev.Net.WebAPI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using VictorDev.Net.WebAPI;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.Framework.WebAPI
{
    public class WebAPICaller : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event] - 是否在Loading")] public UnityEvent<bool> onLoadingEvent;
        [Foldout("[Event] - Response內容"), HideIf(nameof(IsResponseBinary))] public UnityEvent<string> onResponseSuccessEvent;
        [Foldout("[Event] - Response內容"), ShowIf(nameof(IsResponseBinary))] public UnityEvent<Byte[]> onResponseSuccessBinaryEvent;
        [Foldout("[Event] - Response內容")] public UnityEvent<string> onResponseErrorEvent;
        
        [Foldout("[連線設定]"), SerializeField] private string url = "http://192.168.0.107:5080/api/Auth/login";
        [Foldout("[連線設定]"), SerializeField] private EnumHttpMethod httpMethod = EnumHttpMethod.POST;
        [Foldout("[連線設定]"), SerializeField, ShowIf(nameof(IsNotGetMethod))] private EnumBody sendBodyType = EnumBody.FormData;
        [Foldout("[連線設定]"), SerializeField, ShowIf(nameof(IsGetOrFormData))] private List<KeyValueData<string, string>> paramsSetting;
        [Foldout("[連線設定]"), SerializeField, ResizableTextArea, ShowIf(nameof(IsSendJson))] private string sendBodyJson;
        [Foldout("[連線設定]"), SerializeField] private EnumResponseDataType responseDataType = EnumResponseDataType.Json;
        [Foldout("[連線設定]"), Label("逾時秒數"), SerializeField] private int timeoutSeconds = 60;
        [Foldout("[連線設定]"), Label("Authorization (選填)"), SerializeField] private WebApiAuthorizationSO authorization;
        
        #endregion

        /// 設置SendBody - Params
        public void SetParams(List<KeyValueData<string, string>> data) => SetFormData(data);
        
        /// 設置SendBody - FormData
        public void SetFormData(List<KeyValueData<string, string>> data) => paramsSetting = data;
        
        /// 設置SendBody -  JSON字串
        public void SetBodyJson(List<KeyValueData<string, string>> data) => sendBodyJson = data.ToJsonFormat();

        [Button]
        public void CallAPI() => CallAPI(null, null);

        public void CallAPI(UnityEvent<string> onSuccess, UnityEvent<string> onError)
        {
            onLoadingEvent?.Invoke(true);
            StartCoroutine(CoroutineHandler(onSuccess, onError));
        }

        /// 呼叫WebAPI流程
        private IEnumerator CoroutineHandler(UnityEvent<string> onSuccess, UnityEvent<string> onError)
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
            if (authorization != null)
            {
                request.SetRequestHeader("Authorization", $"{authorization.AuthorizationType} {authorization.Token}");
            }
            yield return request.SendWebRequest();

            string msg;
            if (request.result != UnityWebRequest.Result.Success)
            {
                // 失敗
                msg = $"[{request.error}]\n{request.downloadHandler.text}";
                Debug.LogError($"onResponseErrorEvent\n{msg}", this);
                onResponseErrorEvent?.Invoke(msg);
                onError?.Invoke(msg);
            }
            else
            {
                // 成功時
                Debug.Log($"ResponseHeader Content-Type: {request.GetResponseHeader("Content-Type")}");
                switch (responseDataType)
                {
                    
                    case EnumResponseDataType.Json:
                    case EnumResponseDataType.Text:
                        msg = request.downloadHandler.text.ToJsonFormat();
                        Debug.Log($"onResponseSuccessEvent\n{msg}", this);
                        onResponseSuccessEvent?.Invoke(msg);
                        onSuccess?.Invoke(msg);
                        break;
                    case EnumResponseDataType.Binary:
                        byte[] bytes = request.downloadHandler.data;
                        Debug.Log($"onResponseSuccessEvent\nbytes lenght:{bytes.Length}", this);
                        onResponseSuccessBinaryEvent?.Invoke(bytes);
                        onSuccess?.Invoke("");
                        break;
                }
            }
            request.Dispose();
            onLoadingEvent?.Invoke(false);
        }

        #region 依HttpMethod的不同，建立WebRequest
        private UnityWebRequest BuildGetRequest()
        {
            string finalUrl = url;

            if (paramsSetting is { Count: > 0 })
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
        private UnityWebRequest BuildPostRequest()
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
        private UnityWebRequest BuildPatchRequest()
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
        #endregion
        
        private void OnValidate()
        {
            if (name == gameObject.SetNameHeader(httpMethod.ToString())) return;
            name = gameObject.SetNameHeader(httpMethod.ToString());
        }
        
        #region Variables for NaughtyAttribute

        private bool IsNotGetMethod => httpMethod != EnumHttpMethod.GET;
        private bool IsGetOrFormData => sendBodyType == EnumBody.FormData || httpMethod == EnumHttpMethod.GET;
        private bool IsSendJson => sendBodyType == EnumBody.RawJson && httpMethod != EnumHttpMethod.GET ;
        private bool IsResponseBinary => responseDataType == EnumResponseDataType.Binary;

        #endregion
    }
}