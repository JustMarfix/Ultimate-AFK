# Ultimate-AFK
Handles AFK Checking in SCP:SL.

# Features
- Detects AFK Players via in-game movement, camera movement, and in-game interactions
- Moves players to spectator after a determined AFK Time and grace period
- (Optional) Kick players from the server after repeated AFK detections!
- Custom broadcasts to AFK Players to indicate to them if they are AFK. 
- Works with SCP-079 by checking camera angle, and experience interactions
- Configurable and user-specific (if you want to prevent AntiAFK from working at specific player, you can either give them a `uafk.ignore` permission or add an entry to the whitelist)

# Default config:
```yaml
uafk_enable: true
# The time is the time in seconds of non-movement before the player is detected as AFK.
uafk_time: 30
# The grace period is the time in seconds that the player has after the AFK Time where a message is displayed via broadcast.
uafk_grace_period: 15
uafk_prefix: <color=white>[</color><color=green>uAFK</color><color=white>]</color>
uafk_grace_period_message: <color=red>You will be moved to spec in</color> <color=white>%timeleft% seconds</color><color=red> if you do not move!</color>
uafk_fspec_message: You were detected as AFK and automatically moved to spectator!
# Fspec count before player is kicked. (Set to -1 to disable kicking)
uafk_kick_count: 2
```
# Installation

**[Exiled](https://github.com/Exiled-Team/EXILED) must be installed for this to work.**

Place the "UltimateAFK.dll" file in your Plugins folder.
