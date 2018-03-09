using Harmony;
using Staxel;
using Staxel.Client;

namespace BulletinBoard.Patches
{
	[HarmonyPatch(typeof(OverlayController), "UpdateModalModes")]
	class UpdateModalModesPatch
	{
		/// <summary>
		/// Make sure that we AcquireInputControl whenever our menu opens
		/// </summary>
		[HarmonyPostfix]
		static void AfterUpdateModalModes()
		{
			if(!ClientContext.WebOverlayRenderer.HasInputControl())
			{
				bool capture = BulletinBoardManager.Instance.SignController.CaptureInput();
				if(capture)
				{
					ClientContext.WebOverlayRenderer.AcquireInputControl();
				}
				else
				{
					ClientContext.WebOverlayRenderer.ReleaseInputControl();
				}
			}
		}
	}
}
