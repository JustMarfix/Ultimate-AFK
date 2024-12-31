using System;
using Exiled.API.Features;
using ExiledHandlers = Exiled.Events.Handlers;

namespace UltimateAFK
{
    public class UltimateAfk : Plugin<Config>
    {
        public static UltimateAfk Instance { get; private set; }
        
        public override string Author => "Thomasjosif + rewrite by JustMarfix";
        public override string Name => "Ultimate AFK";
        public override string Prefix => "uAFK";
        
        public override Version Version { get; } = new Version(4, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 1, 1);

        public override void OnEnabled()
        {
            try
            {
                Instance = this;
                
                ExiledHandlers.Player.Verified += PlayerEvents.OnPlayerVerified;
                ExiledHandlers.Player.ChangingRole += PlayerEvents.OnSetClass;
                ExiledHandlers.Player.Shooting += PlayerEvents.OnPlayerShoot;
                ExiledHandlers.Player.InteractingDoor += PlayerEvents.OnDoorInteract;
                ExiledHandlers.Scp914.Activating += PlayerEvents.On914Activate;
                ExiledHandlers.Scp914.ChangingKnobSetting += PlayerEvents.On914Change;
                ExiledHandlers.Player.InteractingLocker += PlayerEvents.OnLockerInteract;
                ExiledHandlers.Player.DroppedItem += PlayerEvents.OnDropItem;
                ExiledHandlers.Scp079.GainingExperience += PlayerEvents.OnSCP079Exp;
            }
            catch (Exception e)
            {
                Log.Error($"There was an error loading the plugin: {e}");
            }
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            ExiledHandlers.Player.Verified -= PlayerEvents.OnPlayerVerified;
            ExiledHandlers.Player.ChangingRole -= PlayerEvents.OnSetClass;
            ExiledHandlers.Player.Shooting -= PlayerEvents.OnPlayerShoot;
            ExiledHandlers.Player.InteractingDoor -= PlayerEvents.OnDoorInteract;
            ExiledHandlers.Scp914.Activating -= PlayerEvents.On914Activate;
            ExiledHandlers.Scp914.ChangingKnobSetting -= PlayerEvents.On914Change;
            ExiledHandlers.Player.InteractingLocker -= PlayerEvents.OnLockerInteract;
            ExiledHandlers.Player.DroppedItem -= PlayerEvents.OnDropItem;
            ExiledHandlers.Scp079.GainingExperience -= PlayerEvents.OnSCP079Exp;

            Instance = null;
            base.OnDisabled();
        }
    }
}