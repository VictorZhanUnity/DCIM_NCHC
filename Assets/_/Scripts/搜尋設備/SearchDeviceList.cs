using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.InterfaceUtils;
using _VictorDev.ScrollRectUtils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _VictorDev.TCIT.DCIM
{
    public class SearchDeviceList : MonoBehaviour, IReceiveData<List<RackRevitAssetData>>
    {
        #region Variables
        [Label("[資料項] - 設備資產"), SerializeField] private List<RackRevitAssetData> rackData;
        [Foldout("[Event]")] public UnityEvent<bool> isSearchingEvent;
        [Foldout("[Event] - SelectedItem")] public UnityEvent<Transform> onSelectedItemEvent;
        [Foldout("[組件]"), SerializeField] private DeviceSearchBar deviceSearchBar;
        [Foldout("[組件]"), SerializeField] private TMP_Dropdown dpRevitAssetKind, dpManufacture;
        [Foldout("[組件]"), SerializeField] private DeviceAssetList deviceList;

        /// 所有設備
        private List<DeviceRevitAssetData> allDeviceRevitAssets;
        /// 條件過濾後的設備
        private List<DeviceRevitAssetData> filteredDeviceRevitAssets;
        #endregion

        private void Start()
        {
            //僅設備
            allDeviceRevitAssets ??= rackData.SelectMany(rack => rack.Containers).ToList();
            OnDpRevitAssetKindChanged(0);
        }

        public void ReceiveData(List<RackRevitAssetData> data)
        {
            rackData = data;
        }

        /// 當選擇Asset類型時
        private void OnDpRevitAssetKindChanged(int selectedIndex)
        {
            filteredDeviceRevitAssets = allDeviceRevitAssets.Where(device=> device.RevitAssetKind == (EnumRevitAssetKind)(selectedIndex+2)).ToList();
            string[] manufactureList = filteredDeviceRevitAssets.GroupBy(device => device.Manufacturer)
                .Select(group => group.First().Manufacturer).OrderBy(manufacturer => manufacturer).ToArray();

            if (manufactureList.Length == 0) manufactureList = new[] { "無資料" };
            dpManufacture.SetOptions(manufactureList);
            OnDpManufactureChanged(0);
        }
        
        /// 當選擇廠牌時
        private void OnDpManufactureChanged(int selectedIndex)
        {
            List<DeviceRevitAssetData> result = filteredDeviceRevitAssets
                .Where(device => device.Manufacturer.Equals(dpManufacture.CurrentSelectedText())).ToList();
            deviceList.ReceiveData(result);
        }
        
        private Coroutine searchCoroutine;
        
        /// 搜尋設備財產編號
        private void OnSubmitSearch(string keywordCompanyAssetNo)
        {
            //機櫃與設備
            //allRevitAssets ??= rackData.SelectMany(a => new RevitAssetData[] { a }.Concat(a.Containers)).ToList();
            
            //精確比對
           /*List<DeviceRevitAssetData> findResult = allDeviceRevitAssets
               .Where(x => x.CompanyAssetNo.Equals(keywordCompanyAssetNo, StringComparison.OrdinalIgnoreCase))   
               .ToList();*/
           
           if(searchCoroutine != null) StopCoroutine(searchCoroutine);
           searchCoroutine = StartCoroutine(SearchHandler());
           
           IEnumerator SearchHandler()
           {
               isSearchingEvent?.Invoke(true);
               yield return new WaitForSeconds(0.5f);
               
               // 模糊搜尋
               List<DeviceRevitAssetData> findResult = allDeviceRevitAssets
                   .Where(x => x.DevicePath.Contains(keywordCompanyAssetNo, StringComparison.OrdinalIgnoreCase) == true)
                   .Take(50)
                   .ToList();
               deviceSearchBar.ReceiveData(findResult);
               isSearchingEvent?.Invoke(false);
               Debug.Log($"findResult: {findResult.Count}");
           }
        }
        
        public void OnSelectedItem(DeviceRevitAssetData data) => onSelectedItemEvent?.Invoke(data.Model);

        #region EventListener
        private void OnEnable()
        {
            dpRevitAssetKind.onValueChanged.AddListener(OnDpRevitAssetKindChanged);
            dpManufacture.onValueChanged.AddListener(OnDpManufactureChanged);
            deviceSearchBar.onSubmitEvent.AddListener(OnSubmitSearch);
            deviceSearchBar.onSelectedItemEvent.AddListener(OnSelectedItem);
        }

        private void OnDisable()
        {
            dpRevitAssetKind.onValueChanged.RemoveListener(OnDpRevitAssetKindChanged);
            dpManufacture.onValueChanged.RemoveListener(OnDpManufactureChanged);
            deviceSearchBar.onSubmitEvent.RemoveListener(OnSubmitSearch);
            deviceSearchBar.onSelectedItemEvent.RemoveListener(OnSelectedItem);
        }
        #endregion
       
        private void Reset()
        {
            var dpResult = GetComponentsInChildren<TMP_Dropdown>(true);
            dpRevitAssetKind = dpResult.First();
            dpManufacture = dpResult.Last();
            deviceSearchBar = GetComponentInChildren<DeviceSearchBar>(true);
        }
    }
}