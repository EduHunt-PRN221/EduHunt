namespace Eduhunt.AppSettings
{
    public class CloudinarySetting
    {
        public const string CloudinarySettingName = "CloudinarySetting";
        public string? CloudName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? CloudPreset { get; set; }
    }
}
