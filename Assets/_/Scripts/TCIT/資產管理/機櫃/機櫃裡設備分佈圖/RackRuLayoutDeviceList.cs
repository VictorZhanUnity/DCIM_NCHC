using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃裡設備RuLayout列表
    public class RackRuLayoutDeviceList : MonoBehaviour
    {
        public UnityEvent<bool> isOnEvent;
        
        public void SetIsOn(bool isOn)
        {
            isOnEvent.Invoke(isOn);
            gameObject.SetActive(isOn);
        }
    }
}