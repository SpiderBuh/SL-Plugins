using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHand.DataStores
{
	public class FFDStore : CustomDataStore
	{
		public FFDStore(Player owner) : base(owner)
		{
			TriggerCount = 0;
			LastTrigger = DateTime.MinValue;
			PreviousRole = RoleTypeId.None;
			BlockLog = false;
		}

		public int TriggerCount { get; set; }
		public DateTime LastTrigger { get; set; }
		public RoleTypeId PreviousRole { get; set; }

		public bool ReverseFFEnabled {  get; set; }
		public bool BlockLog { get; set; }
	}
}
