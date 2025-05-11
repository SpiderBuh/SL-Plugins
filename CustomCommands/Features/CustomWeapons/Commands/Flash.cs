using CommandSystem;
using LabApi.Features.Wrappers;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.CustomWeapons.Commands
{
	[CommandHandler(typeof(CustomWeaponParent))]
	public class Flash : ICustomCommand
	{
		public string Command => "flash";

		public string[] Aliases { get; } = { "fl" };

		public string Description => "Launches flashbangs when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (!CustomWeapons.AvailableWeapons.TryGetValue(CustomWeapons.WeaponType.Flashbang, out var customWeaponBase))
			{
				response = $"Unable to find weapon with type {CustomWeapons.WeaponType.Flashbang}";
				return false;
			}

			bool enabled = customWeaponBase.ToggleWeapon(plr);


			response = $"Flashbang launcher {(enabled ? "enabled" : "disabled")}.";
			return true;
		}
	}
}
