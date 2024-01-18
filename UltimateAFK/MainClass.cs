using System;
using Handlers = Exiled.Events.Handlers;
using Exiled.API.Features;

namespace UltimateAFK
{
    public class MainClass : Plugin<Config>
    {
        public override string Author { get; } = "Thomasjosif + 2024 Exiled rewrite by JustMarfix";
        public override string Name { get; } = "Ultimate AFK";
        public override string Prefix { get; } = "uAFK";
        public override Version Version { get; } = new Version(3, 1, 6);
        public override Version RequiredExiledVersion { get; } = new Version(8, 7, 0);
        public PlayerEvents PlayerEvents;

        public override void OnEnabled()
        {
            base.OnEnabled();
            try
            {
                PlayerEvents = new PlayerEvents(this);

                Handlers.Player.Verified += PlayerEvents.OnPlayerVerified;
                Handlers.Player.ChangingRole += PlayerEvents.OnSetClass;
                Handlers.Player.Shooting += PlayerEvents.OnPlayerShoot;
                Handlers.Player.InteractingDoor += PlayerEvents.OnDoorInteract;
                Handlers.Scp914.Activating += PlayerEvents.On914Activate;
                Handlers.Scp914.ChangingKnobSetting += PlayerEvents.On914Change;
                Handlers.Player.InteractingLocker += PlayerEvents.OnLockerInteract;
                Handlers.Player.DroppedItem += PlayerEvents.OnDropItem;
                Handlers.Scp079.GainingExperience += PlayerEvents.OnSCP079Exp;
            }
            catch (Exception e)
            {
                Log.Error($"There was an error loading the plugin: {e}");
            }

        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            Handlers.Player.Verified -= PlayerEvents.OnPlayerVerified;
            Handlers.Player.ChangingRole -= PlayerEvents.OnSetClass;
            Handlers.Player.Shooting -= PlayerEvents.OnPlayerShoot;
            Handlers.Player.InteractingDoor -= PlayerEvents.OnDoorInteract;
            Handlers.Scp914.Activating -= PlayerEvents.On914Activate;
            Handlers.Scp914.ChangingKnobSetting -= PlayerEvents.On914Change;
            Handlers.Player.InteractingLocker -= PlayerEvents.OnLockerInteract;
            Handlers.Player.DroppedItem -= PlayerEvents.OnDropItem;
            Handlers.Scp079.GainingExperience -= PlayerEvents.OnSCP079Exp;

            PlayerEvents = null;
        }
    }
}