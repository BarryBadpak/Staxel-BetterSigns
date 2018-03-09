using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Logic;
using Staxel.Rendering;

namespace BulletinBoard
{
	public class SignController
	{
		private bool _open;
		private bool _active;
		private string _data;

		public SignController()
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
		public void Setup(EntityId entityId, string message, Color color, float scale)
		{
			Blob blob = BlobAllocator.Blob(true);
			blob.SetLong("id", entityId.Id);
			blob.SetString("message", message);
			blob.FetchBlob("color").SetVector3F(new Vector3F(color.R, color.G, color.B));
			blob.SetDouble("scale", scale);

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
			//ClientContext.PlayerFacade.SetCurrentlyViewedTotem(this._currentlyViewed);
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
				//ClientContext.PlayerFacade.CloseTotemRequirements();
			}
		}

		private void OnChange(string args)
		{
			Blob blob = BlobAllocator.Blob(true);
			blob.ReadJson(args);
			long id = blob.GetLong("id");
			string message = blob.GetString("message");
			float scale = (float)blob.GetDouble("scale");
			Vector3F colorVec = blob.FetchBlob("color").GetVector3F();
			Color color = new Color(new Vector3(colorVec.X, colorVec.Y, colorVec.Z));
			Blob.Deallocate(ref blob);
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
