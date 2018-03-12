using Harmony;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Logic;
using Staxel.Rendering;
using Sunbeam.Staxel.Rendering.BitmapFont;
using System.Reflection;

namespace BetterSigns
{
	public class UIController
	{
		private bool _open;
		private bool _active;
		private string _data;
		private EntityId _currentSignId;

		private static readonly FieldInfo CapturedInput = AccessTools.Field(typeof(OverlayController), "_capturedInput");

		public UIController()
		{
			this.Bind();
		}

		public bool IsOpen()
		{
			return this._open;
		}

		public bool CaptureInput()
		{
			if (!this._open)
			{
				return this._active;
			}
			return true;
		}

		public bool PauseGame()
		{
			if (!this._open)
			{
				return this._active;
			}
			return true;
		}

		public bool IsVisible()
		{
			if (this._open)
			{
				return this._active;
			}
			return false;
		}

		/// <summary>
		/// Bind events from the UI
		/// </summary>
		private void Bind()
		{
			WebOverlayRenderer overlay = ClientContext.WebOverlayRenderer;
			overlay.Bind("onShowSignInfo", this.OnShow);
			overlay.Bind("onHideSignInfo", this.OnHide);
			overlay.Bind("onChangeSignInfo", this.OnChange);
		}

		/// <summary>
		/// Setup the UI to show a specific sign's information
		/// </summary>
		/// <param name="message"></param>
		/// <param name="color"></param>
		/// <param name="scale"></param>
		public void Setup(EntityId entityId, string message, Color color, float scale, BmFontAlign align)
		{
			this._currentSignId = entityId;

			Blob blob = BlobAllocator.Blob(true);
			blob.SetLong("id", entityId.Id);
			blob.SetString("message", message);
			blob.FetchBlob("color").SetVector3F(new Vector3F(color.R, color.G, color.B));
			blob.SetDouble("scale", scale);
			blob.SetLong("align", (long)align);

			this._data = blob.ToString();
			Blob.Deallocate(ref blob);
		}

		/// <summary>
		/// Open up the sign UI
		/// </summary>
		public void Show()
		{
			if (!this._open)
			{
				this._open = true;
				WebOverlayRenderer overlay = ClientContext.WebOverlayRenderer;
				overlay.Call("showSignInfo", this._data, null, null, null, null, null);
			}
		}

		/// <summary>
		/// Called from the UI whenever it's shown
		/// </summary>
		/// <param name="arg"></param>
		public void OnShow(string arg)
		{
			this._active = true;
		}

		/// <summary>
		/// Close the sign UI
		/// </summary>
		public void Hide()
		{
			if (this._open)
			{
				this._open = false;
				WebOverlayRenderer overlay = ClientContext.WebOverlayRenderer;
				overlay.Call("hideSignInfo", null, null, null, null, null, null);
			}
		}

		private void OnHide(string arg)
		{
			if (this._active)
			{
				this._open = false;
				this._active = false;

				// We release it manually here, there is no easy way to do this with the PostFix patch without affecting other UI's
				// But we only release it whenever the OverlayController has not captured input, this makes sure that for instance
				// when we pass out we do not reset the input control and cause the user to get stuck on the pause menu
				bool OverlayControllerCapturedInput = (bool)UIController.CapturedInput.GetValue(ClientContext.OverlayController);
				if (!OverlayControllerCapturedInput)
				{
					ClientContext.WebOverlayRenderer.ReleaseInputControl();
				}
			}
		}

		private void OnChange(string args)
		{
			Blob blob = BlobAllocator.Blob(true);
			blob.ReadJson(args);

			string message = blob.GetString("message");
			float scale = (float)blob.GetDouble("scale");
			Vector3I colorVec = blob.FetchBlob("color").GetVector3I();
			Color color = new Color(colorVec.X, colorVec.Y, colorVec.Z);
			BmFontAlign align = (BmFontAlign)blob.GetLong("align");

			Blob.Deallocate(ref blob);

			Blob cmd = BlobAllocator.Blob(true);
			cmd.SetString("action", "changeSign");
			cmd.SetLong("id", this._currentSignId.Id);
			cmd.SetString("message", message);
			cmd.SetLong("color", color.PackedValue);
			cmd.SetDouble("scale", scale);
			cmd.SetLong("align", (long)align);

			ClientContext.OverlayController.AddCommand(cmd);

			this.Hide();
		}

		public void Refresh()
		{
			bool open = this._open;
			this.Reset();
			if (open)
			{
				this.Show();
			}
		}

		public void Reset()
		{
			this._open = false;
			this._active = false;
		}
	}
}
