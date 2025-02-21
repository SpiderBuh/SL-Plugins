using System;
using System.Collections.Generic;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using RedRightHand.CustomPlugin;

namespace StatTracker
{
    public class StatTrackerPlugin : CustomPluginCore<Config>
    {
		public static Config config;

		public override string Name => "Stat Tracker";

		public override string Description => "Tracks player stats";

		public override Version Version => new(1,0,0);

		public override string ConfigFileName => "StatTracker.yml";

		public override void Enable()
		{
			Events = new Events();
			CustomHandlersManager.RegisterEventsHandler(Events);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}

		public class Stats
		{
			public Stats(Player plr)
			{
				UserID = plr.UserId;
				DNT = plr.DoNotTrack;
				Jointime = DateTime.UtcNow;
			}

			public string UserID; 
			public bool DNT = true;
			public int MedicalItems = 0; 
			public bool Escaped = false; //Has the player escaped
			public bool RoundWon = false; //Is the player alive as the winning team when the round ends
			public int SecondsPlayed = 0; //How many seconds the player has played for
			public int PlayersDisarmed = 0; //How many people has the player disarmed
			public int DamageDealt = 0; //How much damage the player has dealt
			public int DamageTaken = 0; //How much damage the player has taken
			public Dictionary<int, int> Spawns = new Dictionary<int, int>(); //How many times this player has spawned as a role
			public Dictionary<int, int> Kills = new Dictionary<int, int>(); //How many kills this player has as this role
			public Dictionary<int, int> Killed = new Dictionary<int, int>(); //How many times this player has killed this role
			public Dictionary<int, int> Deaths = new Dictionary<int, int>(); //How many times this player has died as this role
			public DateTime Jointime;
		}
	}
}
