using _VictorDev.DoTweenUtils;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃裡設備RuLayout列表
    public class RackRuLayoutDeviceList : MonoBehaviour
    {
        #region Variables

        [Label("[資料項]"), SerializeField] private RackRevitAssetData rackRevitAssetData;

        [Foldout("[組件]"), SerializeField] private TextDotweener
            txtAmountOfServer, txtAmountOfRouter, txtAmountOfSwitch;

        #endregion

        public void ReceiveData(RackRevitAssetData data)
        {
            rackRevitAssetData = data;
            UpdateUI();
        }

        private void UpdateUI()
        {
            int amountOfServer = 0, amountOfRouter = 0, amountOfSwitch = 0;
            foreach (DeviceRevitAssetData device in rackRevitAssetData.Containers)
            {
                switch (device.DeviceKind)
                {
                    case EnumDeviceKind.Server: amountOfServer++; break;
                    case EnumDeviceKind.Router: amountOfRouter++; break;
                    case EnumDeviceKind.Switch: amountOfSwitch++; break;
                }
            }

            txtAmountOfServer.text = amountOfServer.ToString();
            txtAmountOfRouter.text = amountOfRouter.ToString();
            txtAmountOfSwitch.text = amountOfSwitch.ToString();
        }
    }
}