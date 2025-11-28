using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _VictorDev.InterfaceUtils;
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
        [Foldout("[組件]"), SerializeField] private TMP_Dropdown dpRevitAssetKind, dpManufacture;
        [Foldout("[組件]"), SerializeField] private DeviceSearchBar deviceSearchBar;

        /// 所有設備
        private List<DeviceRevitAssetData> allDeviceRevitAssets;
        
        #endregion

        public void ReceiveData(List<RackRevitAssetData> data)
        {
            rackData = data;
            
        }
        
        private void OnDpManufactureChanged(int selectedIndex)
        {
        }

        private void OnDpRevitAssetKindChanged(int selectedIndex)
        {
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
               //僅設備
               allDeviceRevitAssets ??= rackData.SelectMany(rack => rack.Containers).ToList();
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
        
        private void OnSelectedItem(DeviceRevitAssetData data) => onSelectedItemEvent?.Invoke(data.Model);

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