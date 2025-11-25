using System.Linq;
using _VictorDev.MediatorUtils.ScrollRectUtils;

namespace _VictorDev.TCIT.DCIM
{
    /// 上架設備列表 
    public class UploadDeviceList : BaseScrollRectList<UploadDeviceRevitAssetData>
    {
        protected override void UpdateUI()
        {
            DataList = DataList.OrderBy(data=>data.DeviceName).ToList();
            base.UpdateUI();
        }
    }
}