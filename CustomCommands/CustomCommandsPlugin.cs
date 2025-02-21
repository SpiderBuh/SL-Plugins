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
using LabApi.Features.Console;
using CustomCommands.Features.CustomSettings;
using CustomCommands.Features.SCPSwap;

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


	public enum VoteType
	{
		NONE = 0,
		AdminVote = 1,
		AutoEventVote = 2,
	}

	public class CustomCommandsPlugin : CustomPluginCore<Config>
	{
		public static Config Config;

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
				new Features.DoorLocking.DoorLocking(Config.EnableDoorLocking),
				//new Features.Events.GlobalEvents(),
				//new Features.Humans.Disarming.DisarmingEvents(), #DISABLED
				new Features.LateJoin.LateJoin(Config.EnableLateJoin && (Config.LateJoinTime > 0)),
				new Features.LateSpawn.LateSpawn(Config.EnableLateSpawn && (Config.LateSpawnTime > 0)),
				//new Features.Items.Weapons.WeaponEvents(),
				new Features.SurfaceLightingFix.SurfaceLightingFix(Config.EnableAdditionalSurfaceLighting),
				new Features.DamageAnnouncements.DamageAnnouncements(Config.EnableDamageAnnouncements),
				//new Features.SCPs.SCP079Removal.RemovalEvents(), #DISABLED
				//new Features.SCPs.SCP3114.SCP3114Overhaul(), #DISABLED
				new Features.SCPSwap.SCPSwap(Config.EnableScpSwap),
				//new Features.Voting.VotingEvents(),
				//new Features.Events.WeeklyEvents.EventManager(),
				//new Features.Events.WeeklyEvents.Events(),
				//new Features.Map.RollingBlackouts.BlackoutEvents(), #DISABLED
			};

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

		}

		public override void Disable()
		{
			foreach(var a in features)
			{
				if (a.isEnabled)
					a.OnDisabled();
			}
		}
	}
}
