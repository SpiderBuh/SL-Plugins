using CommandSystem;
using GameCore;
using Mirror;
using PlayerRoles.FirstPersonControl;
using LabApi.Features.Wrappers;
using LabApi.Features;
using RedRightHand;
using RedRightHand.Commands;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using NetworkManagerUtils.Dummies;
using Logger = LabApi.Features.Console.Logger;
using CustomCommands.Core;
using LabApi.Events.Arguments.PlayerEvents;
using DrawableLine;
using Interactables;
using Interactables.Verification;
using RelativePositioning;
using MEC;

namespace CustomCommands.Features.TestingFeatures
{
	public class TestingDummies : CustomFeature
	{
		public static DummyAI CreateNewSmartDummy(ReferenceHub hub)
		{
			if (!hub.gameObject.TryGetComponent<NavMeshAgent>(out var agent))
			{
				agent = hub.gameObject.AddComponent<NavMeshAgent>();

				agent.baseOffset = 0.98f;
				agent.updateRotation = true;
				agent.angularSpeed = 360;
				agent.acceleration = 600;
				agent.radius = 0.1f;
				agent.areaMask = 1;
				agent.stoppingDistance = 0.2f;
				agent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
			}

			if (!hub.gameObject.TryGetComponent<DummyAI>(out var ai))
				hub.gameObject.AddComponent<DummyAI>().Init(hub, agent);

			return hub.gameObject.GetComponent<DummyAI>();
		}

		public TestingDummies(bool configSetting) : base(configSetting)
		{
		}

		public override void OnServerWaitingForPlayers()
		{
			if (CustomCommandsPlugin.Config.EnableSteve)
			{
				var steve = DummyUtils.SpawnDummy("Steve");
				Round.IsLocked = true;
			}
		}

		public override void OnServerRoundStarted()
		{
			Timing.CallDelayed(0.2f, () =>
			{
				foreach (var dummyHub in ReferenceHub.AllHubs)
				{
					if (dummyHub.IsDummy)
					{
						var dummyAI = CreateNewSmartDummy(dummyHub);
						dummyAI.SetDestination(dummyHub.GetPosition());
					}
				}
			});
		}

		public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		{
			Logger.Debug("BLEH");

			if (ev.Player.IsDummy)
			{
				Logger.Debug($"BLEH 2 {ev.Player.ReferenceHub.gameObject.TryGetComponent<DummyAI>(out _)}");

				if (ev.Player.ReferenceHub.gameObject.TryGetComponent<DummyAI>(out var agnet))
				{
					Logger.Debug($"BLEH 3");
					agnet.SetDestination(ev.Attacker.Position);
				}
			}
		}
	}
}
