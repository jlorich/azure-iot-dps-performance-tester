using System;

namespace azure_iot_dps_timing
{

    public class DeviceRegistrationResult
    {
        public string assignedHub { get; set; }
        public string deviceId { get; set; }
        public DateTime createdDateTimeUtc { get; set; }
        public string registrationId { get; set; }
        public string status { get; set; }
        public string substatus { get; set; }

    }
}
