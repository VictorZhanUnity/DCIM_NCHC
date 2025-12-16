using System.Collections.Generic;
using _VictorDev.ApiExtensions;
using _VictorDev.ColorUtils;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace _VictorDev.TCIT.DCIM.DeviceConfigurationModuel
{
    /// [配置管理] - 機櫃條件過濾
    public class RackConditionFilter : MonoBehaviour
    {
        #region Variables

        [Foldout("[設定]"), SerializeField] private List<KeyValueData<float, Color>> rackColorRangeSetting;
        [Foldout("[設定]"), SerializeField] private Color rackSourceColor = ColorHelper.HexToColor(0x151620);
        [Foldout("[Tween設定]"), SerializeField] private float duration = 0.1f, delayRange = 0.1f;
        [Foldout("[Tween設定]"), SerializeField] private Ease easeType = Ease.OutQuad;
        [Foldout("[組件]"), SerializeField] private Toggle toggleWatt, toggleWeight, toggleHeightU;
        [Foldout("[組件]"), SerializeField] private RevitAssetDataManager revitAssetDataManager;

        private Sequence scaleSequence, colorSequence;
        
        private UploadDeviceRevitAssetData currentDeviceData;
        
        #endregion

        /// 設定選單選定欲上架的設備
        public void SetSelectedDeviceListItem(UploadDeviceRevitAssetData deviceData)
        {
            currentDeviceData = deviceData;
            OnToggleValueChanged(true);
        }
        
        private void OnToggleValueChanged(bool isOn)
        {
            if (scaleSequence != null)
            {
                scaleSequence.Kill();
                scaleSequence = null;
            }
            
            if (colorSequence != null)
            {
                colorSequence.Kill();
                colorSequence = null;
            }
            colorSequence = DOTween.Sequence();
            scaleSequence = DOTween.Sequence();

            revitAssetDataManager.Data.ForEach(rackData =>
            {
                // 是否適合
                bool isSuitable = rackData.IsDeviceSuitable(currentDeviceData);
                ChangeRackModelScale(rackData, isSuitable ? 1f : 0.001f);
                rackData.Model.GetComponentInChildren<RackUnitGrid>(true).gameObject.SetActive(isSuitable);

                Color rackConditionColor;
                // 計算百分比
                if (isSuitable)
                {
                    float totalPercent = 0f, counter = 0;
                    if (toggleWatt.isOn)
                    {
                        totalPercent += rackData.AvailableWattPercentage01;
                        counter++;
                    }

                    if (toggleWeight.isOn)
                    {
                        totalPercent += rackData.AvailableWeightPercentage01;
                        counter++;
                    }

                    if (toggleHeightU.isOn)
                    {
                        totalPercent += rackData.AvailableHeightUPercentage01;
                        counter++;
                    }
                    totalPercent /= counter;
                    rackConditionColor = counter == 0? rackSourceColor : GetColorByPercentage(totalPercent);
                }else rackConditionColor = rackSourceColor;
                
                ChangeRackModelColor(rackData, rackConditionColor);
            });
        }

        private Color GetColorByPercentage(float percentage)
        {
            for (int i = 0; i < rackColorRangeSetting.Count; i++)
            {
                if (percentage < rackColorRangeSetting[i].Key)
                    return rackColorRangeSetting[i].Value;
            }

            // 超過最大 threshold，回傳最後一個
            return rackColorRangeSetting[^1].Value;
        }

        /// 恢復機櫃屬性
        public void RestoreRackAttribute()
        {
            revitAssetDataManager.Data.ForEach(rackData =>
            {
                ChangeRackModelScale(rackData, 1);
                ChangeRackModelColor(rackData, rackSourceColor);
            });
            toggleWatt.isOn = false;
            toggleWeight.isOn = false;
            toggleHeightU.isOn = false;
        }


        /// 改變機櫃Model顏色
        private void ChangeRackModelColor(RackRevitAssetData rackData, Color targetColor)
        {
            Tween tween = rackData.ModelMeshRender.materials[0].DOColor(targetColor, duration).SetEase(easeType);
            if (delayRange > 0 && false)
            {
                //Dotween是在下一個Frame才會執行，所以可以這樣判斷後再加參數
                tween.SetDelay(Random.Range(0, delayRange));
            }
            colorSequence.Join(tween);
        }
        
        /// 改變機櫃Model Scale
        private void ChangeRackModelScale(RackRevitAssetData rackData, float scaleValue)
        {
            Tween tween = rackData.Model.transform.DOScaleY(scaleValue, duration).SetEase(easeType);
            if (delayRange > 0 && false)
            {
                //Dotween是在下一個Frame才會執行，所以可以這樣判斷後再加參數
                tween.SetDelay(Random.Range(0, delayRange));
            }
            scaleSequence.Join(tween);
        }

        #region Event Listener

        private void OnEnable()
        {
            toggleWatt.onValueChanged.AddListener(OnToggleValueChanged);
            toggleWeight.onValueChanged.AddListener(OnToggleValueChanged);
            toggleHeightU.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDisable()
        {
            toggleWatt.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggleWeight.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggleHeightU.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        #endregion

        private void OnValidate()
        {
            rackColorRangeSetting.Sort((a, b) => a.Key.CompareTo(b.Key));
        }
    }
}