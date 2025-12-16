using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using _VictorDev.ApiExtensions;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃資料
    [Serializable]
    public class RackRevitAssetData : RevitAssetData
    {
        /// 設備列表
        [JsonProperty]
        [field: SerializeField]
        public List<DeviceRevitAssetData> Containers { get; private set; }

        #region 額外增加的變數
        
        /// 機櫃編號
        public string RackNo;

        /// 設備已使用總電力
        public int UsageWatt => Containers.Sum(deviceData => deviceData.Watt);
        /// 設備已使用總重量
        public int UsageWeight => Containers.Sum(deviceData => deviceData.Weight);
        /// 設備已使用總U層數
        public int UsageHeightU => Containers.Sum(deviceData => deviceData.HeightU);
        
        /// 總電力 (+3000 For Demo)
        public int MaxWatt => Information.watt + 3000;
        /// 總負重
        public int MaxWeight => Information.weight + 300;
        /// 總U層數
        public int MaxHeightU => Information.heightU;
        
        /// 可供電力
        public int AvailableWatt => Mathf.Clamp(MaxWatt - UsageWatt, 0, MaxWatt);
        /// 可供重量
        public int AvailableWeight => Mathf.Clamp(MaxWeight - UsageWeight, 0, MaxWeight);
        /// 可供U層數
        public int AvailableHeightU => Mathf.Clamp(MaxHeightU - UsageHeightU, 0, MaxHeightU);

        #region 可供U層數的尺吋
        /// 可供U層數的尺吋
        public List<int> AvailableUSize
        {
            get
            {
                Array.Clear(OccupiedU, 0, OccupiedU.Length);
                MarkOccupied();
                return GetAvailableDeviceSegments();
            }
        }
        private const int RackTotalU = 42;
        /// 各U層的佔用情況
        public bool[] OccupiedU { get; private set; } = new bool[RackTotalU];// index 0 = U1
        /// 計算佔用U層
        private void MarkOccupied()
        {
            foreach (var device in Containers)
            {
                int startIndex = device.RackLocation - 1;
                for (int i = 0; i < device.HeightU; i++)
                {
                    int index = startIndex + i;

                    if (index < 0 || index >= OccupiedU.Length)
                    {
                        throw new IndexOutOfRangeException(
                            $"[Rack Occupy Error]\n" +
                            $"Device: {device.DevicePath}\n" +
                            $"RackLocation: {device.RackLocation}\n" +
                            $"HeightU: {device.HeightU}\n" +
                            $"CalculatedIndex: {index}\n" +
                            $"RackSizeU: {OccupiedU.Length}"
                        );
                    }

                    OccupiedU[index] = true;
                }
            }
        }
        /// 取得空閒U區段 (各種可用U尺吋)
        private List<int> GetAvailableDeviceSegments()
        {
            List<int> segments = new();
            int currentEmpty = 0;
            for (int i = 0; i < OccupiedU.Length; i++)
            {
                if (OccupiedU[i] == false)
                    currentEmpty++;
                else if (currentEmpty > 0)
                {
                    segments.Add(currentEmpty);
                    currentEmpty = 0;
                }
            }
            if (currentEmpty > 0)
                segments.Add(currentEmpty);
            segments = segments.Distinct().OrderBy(value=>value).ToList();
            return segments;
        }
        #endregion 

        /// 取得哪些「起始 U」可以放設備
        public List<int> GetAvailableStartU(DeviceRevitAssetData deviceData)
        {
            Array.Clear(OccupiedU, 0, OccupiedU.Length);
            MarkOccupied();

            List<int> result = new();
            int height = deviceData.HeightU;

            int currentEmpty = 0;
            int segmentStartIndex = -1;

            for (int i = 0; i < OccupiedU.Length; i++)
            {
                if (!OccupiedU[i])
                {
                    if (currentEmpty == 0)
                        segmentStartIndex = i;

                    currentEmpty++;

                    if (currentEmpty >= height)
                    {
                        // i 是目前段的結尾
                        int startIndex = i - height + 1;
                        result.Add(startIndex + 1); // 轉回 U (1-based)
                    }
                }
                else
                {
                    currentEmpty = 0;
                    segmentStartIndex = -1;
                }
            }

            return result;
        }

        /// 最佳放置策略（減少碎片）
        public int? GetBestFitStartU(DeviceRevitAssetData deviceData)
        {
            Array.Clear(OccupiedU, 0, OccupiedU.Length);
            MarkOccupied();

            int height = deviceData.HeightU;

            int bestStartU = -1;
            int minRemaining = int.MaxValue;

            int currentEmpty = 0;
            int segmentStart = -1;

            for (int i = 0; i < OccupiedU.Length; i++)
            {
                if (!OccupiedU[i])
                {
                    if (currentEmpty == 0)
                        segmentStart = i;

                    currentEmpty++;
                }
                else
                {
                    EvaluateSegment();
                    currentEmpty = 0;
                }
            }

            EvaluateSegment();

            return bestStartU == -1 ? null : bestStartU + 1;

            void EvaluateSegment()
            {
                if (currentEmpty >= height)
                {
                    int remaining = currentEmpty - height;
                    if (remaining < minRemaining)
                    {
                        minRemaining = remaining;
                        bestStartU = segmentStart;
                    }
                }
            }
        }
        
        /// 百分比：可供電力 
        public float AvailableWattPercentage01 => (float)AvailableWatt / (MaxWatt);
        /// 百分比：可供重量
        public float AvailableWeightPercentage01 => (float)AvailableWeight / MaxWeight;
        /// 百分比：可供U層數
        public float AvailableHeightUPercentage01 => (float)AvailableHeightU / MaxHeightU;
        
        /// 即時溫度
        public float RT => Random.Range(14f, 30f);

        /// 即時濕度
        public float RH => Random.Range(55f, 85f);

        #endregion

        /// 是否放的下設備高度
        public bool IsAbleToPlaceDevice(int startU, int deviceHeightU)
        {
            Array.Clear(OccupiedU, 0, OccupiedU.Length);
            MarkOccupied();

            int startIndex = startU - 1;

            for (int i = 0; i < deviceHeightU; i++)
            {
                int index = startIndex + i;

                // 超出機櫃範圍 → 一定不行
                if (index < 0 || index >= OccupiedU.Length)
                    return false;

                // 這一 U 已被佔用 → 不行
                if (OccupiedU[index])
                    return false;
            }

            return true;
        }
        
        /// 設備是否適放於至機櫃
        public bool IsDeviceSuitable(UploadDeviceRevitAssetData deviceData)
        {
            bool isWattSuitable = deviceData.Watt < AvailableWatt;
            bool isWeightSuitable = deviceData.Weight < AvailableWeight;
            List<int> availableUSegments = GetAvailableDeviceSegments();
            bool isHeightSuitable = availableUSegments.Any(size=> size > deviceData.HeightU);
            return isWattSuitable && isWeightSuitable && isHeightSuitable;
        }

        public void AddDevice(DeviceRevitAssetData deviceData) => Containers.Add(deviceData);
        public void RemoveDevice(DeviceRevitAssetData deviceData) => Containers.Remove(deviceData);
        
        /// 在JSON解析後處理 (需子類別自行解析，override函式需加上[OnDeserialized])
        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            ParseDeviceNameAndCode();
            RackNo = DeviceNameAndCode.Split("+")[1].Trim();
        }
    }
}