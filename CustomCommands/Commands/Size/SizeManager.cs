using LabApi.Features.Wrappers;
using Mirror;

using System.Reflection;

namespace CustomCommands.Features.Players.Size
{
	public static class SizeManager
	{
		public static void SetSize(this Player plr, float x, float y, float z)
		{
			var svrPlrs = Player.List;

			var nId = plr.ReferenceHub.networkIdentity;
			plr.ReferenceHub.gameObject.transform.localScale = new UnityEngine.Vector3(1 * x, 1 * y, 1 * z);

			foreach (var player in svrPlrs)
			{
				NetworkConnection nConn = player.ReferenceHub.connectionToClient;

				typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { nId, nConn });
			}
		}

		public static void ResetSize(this Player plr) => plr.SetSize(1, 1, 1);
	}
}
