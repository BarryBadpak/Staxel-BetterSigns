using BetterSigns.Staxel.Rendering;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;

namespace BetterSigns.Staxel.TileState
{
	class BetterSignTileStateEntityPainter : EntityPainter
	{
		private WorldTextRenderer WorldTextRenderer = new WorldTextRenderer();
		private bool _textInitialized = false;

		private Vector3D _position;
		private uint _rotation;

		private string _message;
		private Color _color;
		private float _scale;
		private BmFontAlign _align;

		/// <summary>
		/// Clear out any old pending draw calls pre-render
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="renderOrigin"></param>
		/// <param name="entity"></param>
		/// <param name="avatarController"></param>
		/// <param name="renderTimestep"></param>
		public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep)
		{
			WorldTextRenderer.Init(graphics);

			if (!this._textInitialized)
			{
				//GameContext.DebugGraphics.Enabled = true;

				BetterSignTileStateEntityLogic logic = entity.TileStateEntityLogic as BetterSignTileStateEntityLogic;
				if (logic != null)
				{
					this._position = logic.GetCenterPosition();
					if (this._position != Vector3D.Zero)
					{
						this._rotation = (logic.GetVariation() >> 10) & 3;
						this._message = logic.GetMessage();
						this._color = logic.GetColor();
						this._scale = logic.GetScale();
						this._align = logic.GetAlign();

						this.WorldTextRenderer.DrawString(
							this._message,
							logic.GetTextRegionSize(),
							this._align,
							Vector3D.Zero,
							this._position,
							this._scale,
							this._rotation,
							this._color
						);

						this._textInitialized = true;
					}
				}
			}
		}

		public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode)
		{
			if (renderMode == RenderMode.Normal)
			{
				this.WorldTextRenderer.Draw(graphics, renderOrigin, matrix);

				if (GameContext.DebugGraphics.Enabled)
				{
					BetterSignTileStateEntityLogic logic = entity.TileStateEntityLogic as BetterSignTileStateEntityLogic;
					if (logic != null)
					{
						Vector2F size = logic.GetTextRegionSize();
						Vector3D rotatedRadius = VectorHelper.RotatePosition(this._rotation, new Vector3D(size.X, size.Y, 0) * 0.5);
						GameContext.DebugGraphics.DrawBoxCentered(this._position, rotatedRadius, Color.Yellow, 1);
					}
				}
			}
		}

		public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade, int updateSteps) { }
		public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }
		public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade)
		{
			BetterSignTileStateEntityLogic logic = entity.TileStateEntityLogic as BetterSignTileStateEntityLogic;
			if (logic != null)
			{
				string message = logic.GetMessage();
				Color color = logic.GetColor();
				float scale = logic.GetScale();
				BmFontAlign align = logic.GetAlign();

				if (message != this._message || color != this._color || scale != this._scale || align != this._align)
				{
					this.WorldTextRenderer.Purge();

					// If the message is not blank add it to the WorldTextRenderer
					if (message != "")
					{
						this._textInitialized = false;
					}
				}
			}
		}

		public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) { }
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.WorldTextRenderer != null)
			{
				this.WorldTextRenderer.Purge();
			}
		}
	}
}
