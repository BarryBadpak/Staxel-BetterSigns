using Harmony;
using Staxel.Client;

namespace BetterSigns.Patches.OverlayControllerNS
{
	[HarmonyPatch(typeof(OverlayController), "OnHideAll")]
	class OnHideAllPatch
	{
		/// <summary>
		/// Make sure that we cancel the method from connector.js Connections.pressBack since we 
		/// do not register our js module within it
		/// </summary>
		[HarmonyPrefix]
		static bool PreOnHideAll()
		{
			if(BetterSigns.Instance.SignController.IsVisible())
			{
				return false;
			}

			return true;
		}
	}
}
