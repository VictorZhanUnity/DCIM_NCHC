using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    public class UploadDeviceList : MonoBehaviour
    {
        #region Variables

        [Foldout("[Event]")] public UnityEvent<Transform> onClickDeviceListItemEvent;

        public void InvokeData(Transform model)
        {
            onClickDeviceListItemEvent?.Invoke(model);
        }
        #endregion
    }
}