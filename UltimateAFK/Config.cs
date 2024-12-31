using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace UltimateAFK
{
    public sealed class Config : IConfig
    {
        [Description("Is the plugin enabled? (bool)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Is the debug mode enabled? (bool)")]
        public bool Debug { get; set; } = false;

        [Description("Minimum required players for uAFK to be active. (int)")]
        public int MinPlayers { get; set; } = 2;
        
        [Description("Should Tutorials be ignored? (bool)")]
        public bool IgnoreTut { get; private set; } = true;

        [Description("Use Exiled permissions to prevent AntiAFK from working? If false, will user  (bool)")]
        public bool UseExiledPermissions { get; private set; } = true;

        [Description("List of players who are not affected by AntiAFK system. Used if UseExiledPermissions = false. (List<string>)")]
        public List<string> WhitelistedPlayers { get; set; } = new List<string>()
        {
            "id@steam"
        };
        
        [Description("How long can player not move in seconds? (int)")]
        public int AfkTime { get; private set; } = 30;
        
        [Description("How long (in seconds) to wait before player gets kicked after getting a warning for not moving? (int)")]
        public int GraceTime { get; private set; } = 15;
        
        [Description("After how many changes to spectator for AFK should player get kicked? (int)")]
        public int NumBeforeKick { get; private set; } = 2;
        
        [Description("Don't touch this if you do not understand the repercussions! - Ignore Perm and IP Checks. (bool)")]
        public bool IgnorePermissionsAndIP { get; private set; } = false;
        
        public string MsgPrefix { get; private set; } = "<color=white>[</color><color=green>uAFK</color><color=white>]</color>";
        
        public string MsgGrace { get; private set; } = "<color=red>You will be moved to spectator in</color> <color=white>%timeleft% seconds</color><color=red> if you do not move!</color>";
        
        public string MsgFspec { get; private set; } = "You were detected as AFK and automatically moved to spectator!";
        
        public string MsgKick { get; private set; } = "[Kicked by uAFK] You were AFK for too long!";
    }
}
