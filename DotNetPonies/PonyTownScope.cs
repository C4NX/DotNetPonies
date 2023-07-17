namespace DotNetPonies
{
    /// <summary>
    /// A class containing all scopes of this library. (magic numbers, regex, ect...)
    /// </summary>
    public class PonyTownScope
    {
        public string RegexApiVersion = "const Ew=\"([^\"]*)";
        public int MagicStatusBit = 80;
        public int MagicOfflineAndRestrictAndNotificationBit = 213;
        public int MagicOnlineBit = 50071;
        public string? DefaultApiVersion = null;
    }
}