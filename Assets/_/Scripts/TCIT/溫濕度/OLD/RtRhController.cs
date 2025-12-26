using System.Collections.Generic;
using System.Linq;
using _VictorDev.ApiExtensions;
using _VictorDev.Configs;
using _VictorDev.MediatorUtils;
using _VictorDev.TCIT.DCIM.EnvironmentModule.Old.Old;
using NaughtyAttributes;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM.EnvironmentModule.Old
{
    public class RtRhController : MonoBehaviour
    {
        #region Vairalbes

        [Label("[機櫃群]"), SerializeField] private List<RevitAssetDataHolder> dataHolders;
        [Foldout("[組件]"), SerializeField] private BoxCollider area;
        [Foldout("[地標組件]"), SerializeField] private RtRhLandmark environmentLandmarkPrefab;
        [Foldout("[地標組件]"), SerializeField] private Transform rtRhLandmarkContainer;

        private float averageRt, averageRh;

        #endregion

        [Button]
        private void CreateLandmark()
        {
            environmentLandmarkPrefab = ObjectHelper.Instantiate(environmentLandmarkPrefab, rtRhLandmarkContainer);
            environmentLandmarkPrefab.name += $" - {name}";
            environmentLandmarkPrefab.SetTargetModel(transform);
        }

        [Button]
        public void GetRtRhValue()
        {
            averageRt = dataHolders.Average(holder => holder.RackRevitData.RT);
            averageRh = dataHolders.Average(holder => holder.RackRevitData.RH);
            environmentLandmarkPrefab.SetRtValue(averageRt);
            environmentLandmarkPrefab.SetRhValue(averageRh);
        }
        
        private void OnEnable() => GetRtRhValue();

        [Button]
        private void FindRacksInArea()
        {
            dataHolders = Physics.OverlapBox(area.bounds.center, area.bounds.extents, transform.rotation).ToList()
                .FilterByNameForKeywords(EnumSearchType.Include, "Rack")
                .FilterByNameForKeywords(EnumSearchType.Exclude, "RackUnitGrid")
                .Select(target => target.GetComponent<RevitAssetDataHolder>()).ToList();
        }

        [Button]
        private void FindComponents() => area = GetComponent<BoxCollider>();

        private void Reset() => FindComponents();
    }
}