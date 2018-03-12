using Staxel.Browser;
using Sunbeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterSigns
{
    public class BetterSigns: SunbeamMod
    {
		public override string ModIdentifier => "BetterSigns";
		public static BetterSigns Instance { get; private set; }

		public UIController SignController;

		private string HTMLAsset { get; set; }
		private string JSAsset { get; set; }
		private string CSSAsset { get; set; }

		public BetterSigns()
		{
			BetterSigns.Instance = this;

			this.HTMLAsset = this.AssetLoader.ReadFileContent("UI/index.min.html").Replace('"', '\'');
			this.JSAsset = this.AssetLoader.ReadFileContent("UI/main.min.js");
			this.CSSAsset = this.AssetLoader.ReadFileContent("UI/style.min.css");
		}

		/// <summary>
		/// Returns the mod directory
		/// </summary>
		/// <returns></returns>
		public string GetModDirectory()
		{
			return this.AssetLoader.ModDirectory;
		}

		/// <summary>
		/// We can only instantiate the controller after the ClientContext is initialised
		/// otherwise the WeboverlayRenderer is not available
		/// </summary>
		public override void ClientContextInitializeBefore()
		{
			this.SignController = new UIController();
		}

		/// <summary>
		/// Inject the UI contents
		/// </summary>
		public override void IngameOverlayUILoaded(BrowserRenderSurface surface)
		{
			surface.CallPreparedFunction("(() => { const el = document.createElement('style'); el.type = 'text/css'; el.appendChild(document.createTextNode('" + this.CSSAsset + "')); document.head.appendChild(el); })();");
			surface.CallPreparedFunction("$('body').append(\"" + this.HTMLAsset + "\");");
			surface.CallPreparedFunction("try{ "+this.JSAsset+" } catch(e) { alert(e.toString()); }");

			this.SignController.Reset();
		}
	}
}
