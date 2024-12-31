using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Roles;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using UnityEngine;
using Scp096Role = Exiled.API.Features.Roles.Scp096Role;

namespace UltimateAFK
{
    public class AfkComponent : MonoBehaviour
    {
        public bool disabled;
        public Vector3 afkLastPosition;
        public Vector3 afkLastAngle;
        public int afkTime;
        public int afkCount;
        private float _timer;
        private Player _ply;

        // Do not change this delay. It will screw up the detection
        public float delay = 1.0f;

        private void Awake()
        {
            _ply = Player.Get(gameObject);
        }

        private void Update()
        {
            if (disabled) return;
            _timer += Time.deltaTime;
            if (_timer > delay)
            {
                _timer = 0f;
                try
                {
                    AfkChecker();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        // Called every 1 second according to the player's Update function. This is way more efficient than the old way of doing a forloop for every player.
        // Also, since the gameObject for the player is deleted when they disconnect, we don't need to worry about cleaning any variables :) 
        private void AfkChecker()
        {
            if (_ply.Role.Team == Team.Dead || Player.List.Count <= UltimateAfk.Instance.Config.MinPlayers || (UltimateAfk.Instance.Config.IgnoreTut && _ply.Role.Type == RoleTypeId.Tutorial))
            {
                Log.Debug($"Player {_ply.Nickname} is not AFK because of the reasons above.");
                return;
            }

            var isScp079 = _ply.Role.Type == RoleTypeId.Scp079;
            var scp096TryNotToCry = false;

            // When SCP096 is in the state "TryNotToCry" he cannot move, or it will cancel,
            // therefore, we don't want to AFK check 096 while he's in this state.
            if (_ply.Role.Type == RoleTypeId.Scp096)
            {
                var scp096 = _ply.Role.As<Scp096Role>();
                scp096TryNotToCry = scp096.AbilityState == Scp096AbilityState.TryingNotToCry;
                Log.Debug($"Player {_ply.Nickname} is not AFK because of crying.");
            }

            var currentPos = _ply.Position;
            var currentAngle = isScp079 ? _ply.Role.As<Scp079Role>().CameraPosition : _ply.CameraTransform.forward;
            Log.Debug($"Angle: {currentAngle}");

            if (currentPos != afkLastPosition || currentAngle != afkLastAngle || scp096TryNotToCry)
            {
                Log.Debug($"Current pos: {currentPos}, last pos: {afkLastPosition}");
                Log.Debug($"Current AFK time: {afkTime}");
                afkLastPosition = currentPos;
                afkLastAngle = currentAngle;
                afkTime = 0;
                return;
            }

            // The player hasn't moved past this point.
            afkTime++;

            // If the player hasn't reached the time yet don't continue.
            if (afkTime < UltimateAfk.Instance.Config.AfkTime) return;

            // Check if we're still in the "grace" period
            var secondsuntilspec = UltimateAfk.Instance.Config.AfkTime + UltimateAfk.Instance.Config.GraceTime - afkTime;
            if (secondsuntilspec > 0)
            {
                var warning = UltimateAfk.Instance.Config.MsgGrace;
                warning = warning.Replace("%timeleft%", secondsuntilspec.ToString());

                _ply.ClearBroadcasts();
                _ply.Broadcast(1, $"{UltimateAfk.Instance.Config.MsgPrefix} {warning}");
                return;
            }

            // The player is AFK and action will be taken.
            Log.Info($"{_ply.Nickname} ({_ply.UserId}) was detected as AFK!");
            afkTime = 0;

            if (_ply.Role.Team == Team.Dead) return;
            ForceToSpec(_ply);

            // If it's -1 we won't be kicking at all.
            if (UltimateAfk.Instance.Config.NumBeforeKick != -1)
            {
                // Increment AFK Count
                afkCount++;
                if (afkCount >= UltimateAfk.Instance.Config.NumBeforeKick)
                {
                    // Since this.AFKCount is greater than the config we're going to kick that player for being AFK too many times in one match.
                    ServerConsole.Disconnect(gameObject, UltimateAfk.Instance.Config.MsgKick);
                }
            }
        }

        private static void ForceToSpec(Player hub)
        {
            switch (hub.Role.Type)
            {
                case RoleTypeId.Scp049:
                    Cassie.CustomScpTermination("0 4 9", new DamageHandler());
                    break;
                case RoleTypeId.Scp079:
                    Cassie.CustomScpTermination("0 7 9", new DamageHandler());
                    break;
                case RoleTypeId.Scp096:
                    Cassie.CustomScpTermination("0 9 6", new DamageHandler());
                    break;
                case RoleTypeId.Scp106:
                    Cassie.CustomScpTermination("1 0 6", new DamageHandler());
                    break;
                case RoleTypeId.Scp173:
                    Cassie.CustomScpTermination("1 7 3", new DamageHandler());
                    break;
                case RoleTypeId.Scp939:
                    Cassie.CustomScpTermination("9 3 9", new DamageHandler());
                    break;
            }
            hub.Kill(DamageType.Falldown);
            hub.Broadcast(30, $"{UltimateAfk.Instance.Config.MsgPrefix} {UltimateAfk.Instance.Config.MsgFspec}");
        }
    }
}
