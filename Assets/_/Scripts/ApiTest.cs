using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ApiTest : MonoBehaviour
{
    public UnityEvent<bool> onLoadingEvent;
    public UnityEvent<string> onSuccess, onFailure;
    
    public TMP_InputField inputURL;

    [Button]
    public void Login()
    {
        onLoadingEvent?.Invoke(true);
        StartCoroutine(SendLogin());
    }

    IEnumerator SendLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("account", "2025TCIT");
        form.AddField("pw", "TCIT2080@");

        using (UnityWebRequest request = UnityWebRequest.Post(inputURL.text.Trim(), form))
        {
            // ❗不要自己亂加 Content-Type
            // Unity 會自動產生 multipart/form-data + boundary

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Login Failed: {request.error}");
                Debug.LogError(request.downloadHandler.text);
                onFailure?.Invoke(request.error);
            }
            else
            {
                Debug.Log("Login Success");
                Debug.Log(request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
        }
        onLoadingEvent?.Invoke(false);
    }
}