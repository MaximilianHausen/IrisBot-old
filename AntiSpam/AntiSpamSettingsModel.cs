namespace AntiSpam;

public class AntiSpamSettingsModel
{
    public ulong? MuteRoleId { get; set; } = null;
    /// <summary> In minutes </summary>
    public ulong MuteDuration { get; set; } = 30;
    public bool AutoDelete { get; set; } = false;
    public bool AutoMute { get; set; } = false;
}
