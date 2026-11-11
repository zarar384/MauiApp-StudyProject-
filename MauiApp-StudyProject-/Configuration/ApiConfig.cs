namespace MauiApp_StudyProject_.Configuration
{
    public static class ApiConfig
    {
        public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5072"
            : "http://localhost:5072";
    }
}
