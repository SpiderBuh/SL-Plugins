using CustomCommands.Features.Humans;
using CustomCommands.Features.Map;
using CustomCommands.Features.Map.RollingBlackouts;
using CustomCommands.Features.SCPs;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using RedRightHand.CustomSettings;
using System.Linq;
using UserSettings.ServerSpecific;
using RedRightHand.CustomPlugin;
using System;
using CustomCommands.Features;
using CustomCommands.Core;

namespace CustomCommands
{
	//An enum containing every single custom setting ID that we will be using.
	//To keep it simple and easier to track, IDs need to be prefixed with what they are being used for.
	//SCP is used for CustomSCPSettings
	//Human is used for CustomHumanSettings
	public enum SettingsIDs
	{
		SCP_SwapToHuman = 0,
		SCP_SwapFromHuman = 1,
		SCP_NeverSCP = 2,
		SCP_ZombieSuicide = 3,
		Human_HealOther = 4,
		Human_Suicide = 5,
	}

	public enum EventType
	{
		NONE = 0,

		Infection = 1,
		Battle = 2,
		Hush = 3,
		SnowballFight = 4 // This event is christmas-exclusive.
	}
	public enum VoteType
	{
		NONE = 0,
		AdminVote = 1,
		AutoEventVote = 2,
	}

	public class CustomCommandsPlugin : CustomPluginCore<Config>
	{
		public static Config Config;

		public static bool EventInProgress => CurrentEvent != EventType.NONE;
		public static EventType CurrentEvent = EventType.NONE;

		public override string ConfigFileName => "CustomCommandsConfig.yml";

		public override string Name => "Custom Commands";

		public override string Description => "Simple plugin for custom commands";

		public override Version Version => new(2, 0, 0);

		public CustomFeature[] features;

		public override void Enable()
		{
			base.Enable();

			Harmony harmony = new Harmony("CC-Patching-Phegg");
			harmony.PatchAll();

			features = new CustomFeature[]
			{
				new Features.DoorLocking.DoorLocking(),
				new Features.Events.GlobalEvents(),
				new Features.Events.Infection.InfectionEvents(),
				new Features.Humans.Disarming.DisarmingEvents(),
				new Features.Humans.LateJoin.LateJoinEvents(),
				new Features.Humans.LateSpawn.LateSpawnEvents(),
				new Features.Humans.TutorialFix.TutorialEvents(),
				new Features.Items.Weapons.WeaponEvents(),
				new Features.Map.SurfaceLightFix.LightFixEvents(),
				new Features.SCPs.DamageAnnouncements.AnnouncementEvents(),
				new Features.SCPs.SCP079Removal.RemovalEvents(),
				new Features.SCPs.SCP3114.SCP3114Overhaul(),
				new Features.SCPs.Swap.SwapEvents(),
				new Features.Voting.VotingEvents(),
				new Features.Events.WeeklyEvents.EventManager(),
				new Features.Events.WeeklyEvents.Events(),
				new Features.Map.RollingBlackouts.BlackoutEvents(),
			};

		}

		public override void Disable()
		{
			foreach(var a in features)
			{
				if (a.isEnabled)
					a.OnDisabled();
			}
		}

		public void OnPluginStart()
		{
			

			Logger.Info($"Plugin is loading...");

			EventManager.RegisterEvents<Carpincho>(this);

			if (Config.EnableDoorLocking)
			{
				EventManager.RegisterEvents<Features.DoorLocking.DoorLocking>(this);
			}

			if (Config.EnableEvents)
			{
				EventManager.RegisterEvents<Features.Events.GlobalEvents>(this);
				EventManager.RegisterEvents<Features.Events.Infection.InfectionEvents>(this);
			}

			if (Config.EnableBetterDisarming)
			{
				EventManager.RegisterEvents<Features.Humans.Disarming.DisarmingEvents>(this);
			}

			if (Config.EnableLateJoin)
			{
				EventManager.RegisterEvents<Features.Humans.LateJoin.LateJoinEvents>(this);
			}

			if (Config.EnableLateSpawn)
			{
				EventManager.RegisterEvents<Features.Humans.LateSpawn.LateSpawnEvents>(this);
			}

			if (Config.EnableTutorialFixes)
			{
				EventManager.RegisterEvents<Features.Humans.TutorialFix.TutorialEvents>(this);
			}

			if (Config.EnableSpecialWeapons)
			{
				EventManager.RegisterEvents<Features.Items.Weapons.WeaponEvents>(this);
			}

			if (Config.EnableAdditionalSurfaceLighting)
			{
				EventManager.RegisterEvents<Features.Map.SurfaceLightFix.LightFixEvents>(this);
			}

			if (Config.EnableDamageAnnouncements)
			{
				EventManager.RegisterEvents<Features.SCPs.DamageAnnouncements.AnnouncementEvents>(this);
			}

			if (Config.EnableScp079Removal)
			{
				EventManager.RegisterEvents<Features.SCPs.SCP079Removal.RemovalEvents>(this);
				EventManager.RegisterEvents<Features.SCPs.SCP3114.SCP3114Overhaul>(this);
			}

			if (Config.EnableScpSwap)
			{
				EventManager.RegisterEvents<Features.SCPs.Swap.SwapEvents>(this);
			}

			if (Config.EnableDebugTests)
			{
				//EventManager.RegisterEvents<Features.Testing.Navigation.NavigationEvents>(this);
				//EventManager.RegisterEvents<Features.Testing.TestingDummies>(this);
			}

			if (Config.EnablePlayerVoting)
			{
				EventManager.RegisterEvents<Features.Voting.VotingEvents>(this);
			}

			if (Config.EnableWeeklyEvents)
			{
				EventManager.RegisterEvents<Features.Events.WeeklyEvents.EventManager>(this);
				EventManager.RegisterEvents<Features.Events.WeeklyEvents.Events>(this);
			}

			if (Config.EnableBlackout)
			{
				EventManager.RegisterEvents<BlackoutEvents>(this);
				new BlackoutManager();
			}

			if (ServerSpecificSettingsSync.DefinedSettings == null)
				ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[0];

			var settings = new CustomSettingsBase[]
			{
				new CustomSCPSettings(),
				new CustomHumanSettings(),
			};

			ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Concat(CustomSettingsManager.ActivateAllSettings(settings)).ToArray();
			ServerSpecificSettingsSync.SendToAll();

			RagdollManager.OnRagdollSpawned += Features.Ragdoll.PocketRagdollHandler.RagdollManager_OnRagdollSpawned;

			EventManager.RegisterEvents<Features.Players.Size.SizeEvents>(this);
		}


	}
}
