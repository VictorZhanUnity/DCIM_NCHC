using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JsonDataGetter : MonoBehaviour
{
    public UnityEvent<string> invokeJsonDataEvent;

    private Coroutine coroutine;
    private string dataString;

    public void ReceiveJsonString(string jsonString)
    {
        if (Application.isPlaying)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(CoroutineHandler());
        }
        else
            DataHandler(jsonString);

        IEnumerator CoroutineHandler()
        {
            yield return new WaitForEndOfFrame();
            DataHandler(jsonString);
        }
    }
    private void DataHandler(string jsonString)
    {
        JObject root = JObject.Parse(jsonString);
        dataString = root["data"]?.ToString();
        invokeJsonDataEvent?.Invoke(dataString);
    }
}