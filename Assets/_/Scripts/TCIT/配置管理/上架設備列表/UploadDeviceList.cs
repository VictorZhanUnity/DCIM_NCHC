using System.Linq;
using _VictorDev.Framework.ScrollRectUtils;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備列表 
    public class UploadDeviceList : BaseScrollRectList<UploadDeviceRevitAssetData>
    {
        protected override void UpdateUI(Transform container = null)
        {
            DataList = DataList.OrderBy(data=>data.DeviceName).ToList();
            
            base.UpdateUI(container);
        }
    }
}