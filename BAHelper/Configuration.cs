using BAHelper.Utility;
using Dalamud.Configuration;
using ECommons.Configuration;

namespace BAHelper;

public class Configuration : IPluginConfiguration
{
    public void Save() => EzConfig.Save();
    public int Version { get; set; } = 1;

    public bool OnlyShowStanceOn = false;

    public bool IsCNMoogleDCPlayer = false;
    public bool UsePartyChannel = true;

    public int ShieldRemainingTimeThreshold = 15; // minutes

    public bool ElementLevelReminderEnabled = true;
    public bool PlayerHighlightEnabled = false;
    public float PlayerHighlightDistance = 50f;
    public float PlayerHighlightCircleRadius = 0.8f;
    public bool PlayerHighlightShowName = true;
    public bool PlayerHighlightShowMatchedRules = true;
    public bool PlayerHighlightHeroEnabled = true;
    public bool PlayerHighlightPerceptionEnabled = true;
    public bool PlayerHighlightIncenseEnabled = true;
    public bool PlayerHighlightTankStanceEnabled = true;
    public bool PlayerHighlightMissingShellEnabled = true;
    public uint PlayerHighlightHeroColor = Color.Yellow;
    public uint PlayerHighlightPerceptionColor = Color.Cyan;
    public uint PlayerHighlightIncenseColor = Color.Orange;
    public uint PlayerHighlightTankStanceColor = Color.Magenta;
    public uint PlayerHighlightMissingShellColor = Color.Blue;

    public bool AdvancedModeEnabled = false;
    public float TrapViewDistance = 100f;
    public bool DrawRecordedTraps = false;
    public bool DrawTrapBlastCircle = false;
    public bool DrawTrapBlastCircleOnlyWhenApproaching = false;
    public bool DrawTrap15m = false;
    public bool DrawTrap15mOnlyWhenApproaching = false;
    public bool DrawTrap15mExceptRevealed = false;
    public bool DrawTrap36m = false;
    public bool DrawTrap36mOnlyWhenApproaching = false;
    public bool DrawTrap36mExceptRevealed = false;
    public bool DrawRecommendedScanningSpots = false;
    public bool DrawScanningSpot15m = false;
    public bool DrawScanningSpot36m = false;
    public bool DrawMobViews = true;
    public uint TrapBigBombColor = Color.Red;
    public uint TrapSmallBombColor = Color.Orange;
    public uint TrapPortalColor = Color.Green;
    public uint RevealedTrapColor = Color.TransBlack;
    public uint Trap15mCircleColor = Color.LightCyan;
    public uint Trap36mCircleColor = Color.DarkCyan;
    public uint ScanningSpotColor = Color.Cyan;
    public uint ScanningSpot15mCircleColor = Color.White;
    public uint ScanningSpot36mCircleColor = Color.White;
    public uint NormalAggroColor = Color.Brown;
    public uint SoundAggroColor = Color.Magenta;
}
