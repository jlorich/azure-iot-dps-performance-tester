namespace azure_iot_dps_timing
{

    public class RegistrationOperationStatusResult
    {
        public string operationId { get; set; }
        public DeviceRegistrationResult registrationState { get; set; }
        public string status { get; set; }

    }
}
