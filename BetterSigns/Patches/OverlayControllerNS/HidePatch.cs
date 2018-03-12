using Harmony;
using Staxel;
using Staxel.Client;

namespace BetterSigns.Patches.OverlayControllerNS
{
	[HarmonyPatch(typeof(OverlayController), "Hide")]
	class HidePatch
	{
		/// <summary>
		/// Make sure that we hide our UI if it is globally called on OverlayController
		/// </summary>
		[HarmonyPrefix]
		static void PreHide()
		{
			BetterSigns.Instance.SignController.Hide();
		}
	}
}
