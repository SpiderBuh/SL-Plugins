using CommandSystem;
using LabApi.Features.Console;
using RedRightHand;
using RedRightHand.Commands;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace CustomCommands.Features.TestingFeatures.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class AI : ICustomCommand
	{
		public string Command => "dummyai";

		public string[] Aliases => null;

		public string Description => "Basic AI for dummies";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (!CustomCommandsPlugin.Config.EnableDebugTests)
			{
				response = "This command is disabled";
				return false;
			}

			if (sender is PlayerCommandSender pSender)
			{
				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						if (!dummyHub.gameObject.TryGetComponent<NavMeshAgent>(out var agent))
						{
							agent = dummyHub.gameObject.AddComponent<NavMeshAgent>();

							agent.baseOffset = 0.98f;
							agent.updateRotation = true;
							agent.angularSpeed = 360;
							agent.acceleration = 600;
							agent.radius = 0.1f;
							agent.areaMask = 1;
							agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
						}

						if (!dummyHub.gameObject.TryGetComponent<DummyAI>(out var _))
							dummyHub.gameObject.AddComponent<DummyAI>().Init(dummyHub, agent);


						dummyHub.gameObject.TryGetComponent<DummyAI>(out var ai);
						ai.SetDestination(pSender.ReferenceHub.transform.position);

						foreach (var corner in agent.path.corners)
						{
							Logger.Info($"{corner} + {pSender.ReferenceHub.transform.position}");
						}

						response = $"Path set with {agent.path.corners.Length} corners";
					}
				}
			}

			//response = $"Path set to";
			return true;
		}
	}
}
