using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM;
using _VictorDev.TCIT.DCIM.EnvironmentModule;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnvironmentRankingList : MonoBehaviour
{
   public UnityEvent<Transform> onItemClickedEvent;
   public UnityEvent onItemCancelEvent;
   public UnityEvent<RackRevitAssetData> onItemClickedDataEvent;
   
   public Transform rtRankContainer, rhRankContainer;
   public EnvironmentValueDisplay rtDisplayPrefab, rhDisplayPrefab;
   public EnvironmentDataManager environmentDataManager;

   public ToggleGroup toggleGroup;
   
   public void GetRankingList()
   {
      GetRTRankingList();
      GetRHRankingList();
   }
   private void GetRTRankingList()
   {
      rtRankContainer.RemoveAllChildren();
      
      List<EnvironmentDataHolder> result;
      result = environmentDataManager.EnvironmentDataHolders.Where(holder=> holder.EnvData.RTValue >=0).OrderByDescending(holder=> holder.EnvData.RTValue).Take(10).ToList();
      result.ForEach(dataHolder =>
      {
         EnvironmentValueDisplay item = ObjectHelper.Instantiate(rtDisplayPrefab, rtRankContainer);
         item.SetEnvData(dataHolder);
         item.ToggleComp.onValueChanged.AddListener((isOn)=>OnToggleValueChanged(isOn, dataHolder));
      });
   }

   private void GetRHRankingList()
   {
      rhRankContainer.RemoveAllChildren();
      
      List<EnvironmentDataHolder> result;
      result = environmentDataManager.EnvironmentDataHolders.Where(holder=> holder.EnvData.RHValue >=0).OrderByDescending(holder=> holder.EnvData.RHValue).Take(10).ToList();
      result.ForEach(dataHolder =>
      {
         EnvironmentValueDisplay item = ObjectHelper.Instantiate(rhDisplayPrefab, rhRankContainer);
         item.SetEnvData(dataHolder);
         item.ToggleComp.group = toggleGroup;
         item.ToggleComp.onValueChanged.AddListener((isOn)=>OnToggleValueChanged(isOn, dataHolder));
      });
   }
   
   private void OnToggleValueChanged(bool isOn, EnvironmentDataHolder target)
   {
      if (isOn)
      {
         onItemClickedEvent?.Invoke(target.RackData.Model);
         onItemClickedDataEvent?.Invoke(target.RackData);
      }
      else
      {
         onItemCancelEvent?.Invoke();
      }
   }

   public void RemoveAllItems()
   {
      rtRankContainer.RemoveAllChildren();
      rhRankContainer.RemoveAllChildren();
   }
}
