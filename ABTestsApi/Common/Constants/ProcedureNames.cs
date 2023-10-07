namespace ABTestsApi.Common.Constants
{
    static class ProcedureNames
    {
        // Devices
        public static string GetDeviceByToken = "GetDeviceByToken";
        public static string CreateDevice = "CreateDevice";

        // Expereriments
        public static string GetAllExpts = "GetAllExperiments";

        // Experiment Options
        public static string GetOptsByExptId = "GetOptionsByExperimentId";
        public static string GetOptsWithDeviceCount = "GetOptionsWithDeviceCount";
        public static string GetOptValueByDeviceIdExptId = "GetOptionValueByDeviceIdExperimentId";

        // Device Experiment Options 
        public static string CreateDeviceExptOpt = "CreateDeviceExperimentOption";
    }
}
