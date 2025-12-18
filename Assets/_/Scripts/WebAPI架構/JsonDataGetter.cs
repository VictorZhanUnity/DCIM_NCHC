using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JsonDataGetter : MonoBehaviour
{
   public UnityEvent<string> invokeJsonDataEvent;
   
   public void ReceiveJsonString(string jsonString)
   {
      JObject root = JObject.Parse(jsonString);
      string dataString = root["data"]?.ToString();
      invokeJsonDataEvent?.Invoke(dataString);
   }
}
