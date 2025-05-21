using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles;
using RedRightHand;
using RedRightHand.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Commands.Gaslighting
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Capyman : ICustomCommand
    {
        public string Command => "capyfy";

        public string[] Aliases => null;

        public string Description => "Attaches a Capybara to the target players' heads";

        public string[] Usage => ["%player%", "invisible to SCPs? (y/n)"];

        public PlayerPermissions? Permission => PlayerPermissions.Effects;

        public string PermissionString => string.Empty;

        public bool RequirePlayerSender => false;

        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out var plrs, out _)) return false;

            response = "Calling the Capys...\n";

            List<uint> capyIds = new List<uint>();

            foreach (var plr in plrs.ToList())
            {
                try
                {
                    LabApi.Features.Wrappers.CapybaraToy capy = LabApi.Features.Wrappers.CapybaraToy.Create(plr.Camera.position, plr.Camera.rotation, plr.Camera.transform, true);
                    capy.CollidersEnabled = false;
                    capy.Spawn();
                    capyIds.Add(capy.Base.netId);
                }
                catch (Exception ex)
                {
                    response += $"Failed to add Capy to player ID {plr.PlayerId}\n";
                    plrs.Remove(plr);
                }
            }

            for (int i = 0; i < plrs.Count; i++)
                plrs.ElementAt(i).Connection.Send(new ObjectHideMessage { netId = capyIds.ElementAt(i) });

            if (arguments.At(1).ToLower().Equals("y"))
                foreach (var scp in LabApi.Features.Wrappers.Player.List.Where(x => x.ReferenceHub.IsSCP(false)))
                    foreach (var capyID in capyIds)
                        scp.Connection.Send(new ObjectHideMessage { netId = capyID });
            if (plrs.Count == 0 && capyIds.Count == 0)
                response += "The Capys couldn't come :(\n(could not add to any players)";
            else if (plrs.Count == 1 && capyIds.Count == 1)
                response += $"The Capy has arrived. If you're a monster, destroy them with \'DESTROYTOY {capyIds.First()}\'";
            else if (plrs.Count == capyIds.Count)
            {
                response += $"The Capys have arrived. If you're a monster, destroy them with \'DESTROYTOY [Capy ID]\'\nPlayer ID - Capy ID:\n";
                for (int i = 0; i < plrs.Count; i++)
                {
                    response += $"{plrs.ElementAt(i)}\t{capyIds.ElementAt(i)}\n";
                }
            }
            else 
                response += $"The Capys have arrived. If you're a monster, destroy them (individually) with:\n\'DESTROYTOY {string.Join("/", capyIds)}\'";
            return true;
        }
    }
}
