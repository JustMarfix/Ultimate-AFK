using System;
using System.Linq;
using UnityEngine;
using Exiled.API.Features;
using PlayerRoles;
using Exiled.API.Features.Roles;

namespace UltimateAFK
{
    public class AFKComponent : MonoBehaviour
    {
        public MainClass plugin;

        public bool disabled = false;

        Player ply;

        public Vector3 AFKLastPosition;
        public Vector3 AFKLastAngle;

        public int AFKTime = 0;
        public int AFKCount = 0;
        private float timer = 0.0f;

        // Do not change this delay. It will screw up the detection
        public float delay = 1.0f;

        void Awake()
        {
            ply = Player.Get(gameObject);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                timer = 0f;
                if (!this.disabled)
                {
                    try
                    {
                        AFKChecker();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        // Called every 1 second according to the player's Update function. This is way more efficient than the old way of doing a forloop for every player.
        // Also, since the gameObject for the player is deleted when they disconnect, we don't need to worry about cleaning any variables :) 
        private void AFKChecker()
        {
            //Log.Info($"AFK Time: {this.AFKTime} AFK Count: {this.AFKCount}");
            if (this.ply.Role.Team == Team.Dead || Player.List.Count() <= plugin.Config.MinPlayers || (plugin.Config.IgnoreTut && this.ply.Role.Type == RoleTypeId.Tutorial))
            {
                Log.Debug($"Player {this.ply.Nickname} is not AFK because of the reasons above.");
                return;
            }

            bool isScp079 = (this.ply.Role.Type == RoleTypeId.Scp079);
            bool scp096TryNotToCry = false;

            // When SCP096 is in the state "TryNotToCry" he cannot move or it will cancel,
            // therefore, we don't want to AFK check 096 while he's in this state.
            if (this.ply.Role.Type == RoleTypeId.Scp096)
            {
                Scp096Role scp096 = this.ply.Role.As<Scp096Role>();
                scp096TryNotToCry = (scp096.AbilityState == PlayerRoles.PlayableScps.Scp096.Scp096AbilityState.TryingNotToCry);
                Log.Debug($"Player {this.ply.Nickname} is not AFK because of crying.");
            }

            Vector3 CurrentPos = this.ply.Position;
            Vector3 CurrentAngle = (isScp079) ? this.ply.CameraTransform.position : this.ply.CameraTransform.forward;
            Log.Debug($"Angle: {CurrentAngle}");

            if (CurrentPos != this.AFKLastPosition || CurrentAngle != this.AFKLastAngle || scp096TryNotToCry)
            {
                Log.Debug($"Current pos: {CurrentPos}, last pos: {this.AFKLastPosition}");
                Log.Debug($"Current AFK time: {this.AFKTime}");
                this.AFKLastPosition = CurrentPos;
                this.AFKLastAngle = CurrentAngle;
                this.AFKTime = 0;
                return;
            }

            // The player hasn't moved past this point.
            this.AFKTime++;

            // If the player hasn't reached the time yet don't continue.
            if (this.AFKTime < plugin.Config.AfkTime) return;

            // Check if we're still in the "grace" period
            int secondsuntilspec = (plugin.Config.AfkTime + plugin.Config.GraceTime) - this.AFKTime;
            if (secondsuntilspec > 0)
            {
                string warning = plugin.Config.MsgGrace;
                warning = warning.Replace("%timeleft%", secondsuntilspec.ToString());

                this.ply.ClearBroadcasts();
                this.ply.Broadcast(1, $"{plugin.Config.MsgPrefix} {warning}");
                return;
            }

            // The player is AFK and action will be taken.
            Log.Info($"{this.ply.Nickname} ({this.ply.UserId}) was detected as AFK!");
            this.AFKTime = 0;

            if (this.ply.Role.Team == Team.Dead) return;
            ForceToSpec(this.ply);

            // If it's -1 we won't be kicking at all.
            if (plugin.Config.NumBeforeKick != -1)
            {
                // Increment AFK Count
                this.AFKCount++;
                if (this.AFKCount >= plugin.Config.NumBeforeKick)
                {
                    // Since this.AFKCount is greater than the config we're going to kick that player for being AFK too many times in one match.
                    ServerConsole.Disconnect(this.gameObject, plugin.Config.MsgKick);
                }
            }
        }

        private void ForceToSpec(Player hub)
        {
            hub.Role.Set(RoleTypeId.Spectator);
            hub.Broadcast(30, $"{plugin.Config.MsgPrefix} {plugin.Config.MsgFspec}");
        }
    }
}
