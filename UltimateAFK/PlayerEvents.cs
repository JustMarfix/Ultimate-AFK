using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
using Exiled.Events.EventArgs.Scp079;

namespace UltimateAFK
{
    public static class PlayerEvents
    {
		public static void OnPlayerVerified(VerifiedEventArgs ev)
		{
			// Add a component to the player to check AFK status.
			ev.Player.GameObject.gameObject.AddComponent<AfkComponent>();
			Log.Debug($"Added component: {ev.Player.GameObject.gameObject.GetComponent<AfkComponent>()}");
		}

		// This check was moved here, because player's ranks are set AFTER OnPlayerJoin()
		public static void OnSetClass(ChangingRoleEventArgs ev)
		{
			try
			{
				if (ev.Player == null) return;
				var gotComponent = ev.Player.GameObject.gameObject.TryGetComponent<AfkComponent>(out var afkComponent);
				Log.Debug($"gotComponent: {gotComponent}");
				if (!gotComponent) return;
				
				if (!UltimateAfk.Instance.Config.IgnorePermissionsAndIP &&
				    (
					    (
						    ev.Player.CheckPermission("uafk.ignore") &&
						    UltimateAfk.Instance.Config.UseExiledPermissions
						) ||
					    (
						    UltimateAfk.Instance.Config.WhitelistedPlayers.Contains(ev.Player.UserId) &&
						    !UltimateAfk.Instance.Config.UseExiledPermissions
						) ||
					    ev.Player.IPAddress == "127.0.0.1")
				    )
				{
					Log.Debug("Disabled cuz of permissions or ip or whitelist");
					afkComponent.disabled = true;
				}
				
				Log.Debug("First check passed");
				if (ev.Player.IsNPC)
				{
					afkComponent.disabled = true;
					Log.Debug("Disabled cuz of npc");
				}
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnSetClass(): {e}");
			}
		}

		/*
		 * The following events are only here as additional AFK checks for some very basic player interactions
		 * I can add more interactions, but this seems good for now.
		 */
		public static void OnDoorInteract(InteractingDoorEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnDoorInteract(): {e}");
			}
		}

		public static void OnPlayerShoot(ShootingEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In ResetAFKTime(): {e}");
			}
		}
		public static void On914Activate(ActivatingEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In On914Activate(): {e}");
			}
		}
		public static void On914Change(ChangingKnobSettingEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnLockerInteract(): {e}");
			}
		}

		public static void OnLockerInteract(InteractingLockerEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnLockerInteract(): {e}");
			}
		}
		public static void OnDropItem(DroppedItemEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnDropItem(): {e}");
			}
		}

		public static void OnSCP079Exp(GainingExperienceEventArgs ev)
		{
			try
			{
				_resetAfkTime(ev.Player);
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In OnSCP079Exp(): {e}");
			}
		}

		/// <summary>
		/// Reset the AFK time of a player.
		/// Thanks iopietro!
		/// </summary>
		/// <param name="player"></param>
		private static void _resetAfkTime(Player player)
		{
			try
			{
				if (player == null) return;
				var afkComponent = player.GameObject.gameObject.GetComponent<AfkComponent>();
				if (afkComponent != null)
					afkComponent.afkTime = 0;
			}
			catch (Exception e)
			{
				Log.Error($"ERROR In ResetAFKTime(): {e}");
			}
		}
    }
}
