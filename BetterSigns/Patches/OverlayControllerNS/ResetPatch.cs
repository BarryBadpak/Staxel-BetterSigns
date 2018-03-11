using Harmony;
using Staxel;
using Staxel.Client;

namespace BetterSigns.Patches.OverlayControllerNS
{
	[HarmonyPatch(typeof(OverlayController), "Reset")]
	class ResetPatch
	{
		/// <summary>
		/// Make sure that we hide our UI if it is globally called on OverlayController
		/// </summary>
		[HarmonyPrefix]
		static void PreReset()
		{
			BetterSigns.Instance.SignController.Hide();
			BetterSigns.Instance.SignController.Reset();
		}
	}
}
