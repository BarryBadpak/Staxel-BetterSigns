using Harmony;
using Staxel.Client;

namespace BetterSigns.Patches.InterruptionOverlayControllerNS
{
	[HarmonyPatch(typeof(InterruptionOverlayController), "ShowNightTransition")]
	class ShowNightTransitionPatch
	{
		/// <summary>
		/// Make sure that whenever the player faints we close the UI if its open
		/// </summary>
		[HarmonyPrefix]
		static void PreShowNightTransition()
		{
			if(BetterSigns.Instance.SignController.IsVisible())
			{
				BetterSigns.Instance.SignController.Hide();
			}
		}
	}
}
