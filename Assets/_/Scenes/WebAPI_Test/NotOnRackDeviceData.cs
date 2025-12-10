using System;

namespace _VictorDev.TCIT.DCIM.DeviceConfigurationModuel
{
    [Serializable]
    public class NotOnRackDeviceData
    {
        public string DeviceCode { get; private set; }
        public string Code { get; private set; }
        public string System { get; private set; }
        public string Type { get; private set; }
        public string Description { get; private set; }
        public string DeviceId { get; private set; }
        public string Manufacturer { get; private set; }
        public string ModelNumber { get; private set; }
        public string Urn { get; private set; }
        public string Status { get; private set; }
    }
}