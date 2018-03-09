using BulletinBoard.Rendering;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;
using System;

namespace BulletinBoard.TileState
{
	class BulletinBoardTileStateEntityPainter : EntityPainter
	{
		private Vector3D BoardOffset = new Vector3D(0.25, 0, -0.03);
        private Vector3D BoardRegion = new Vector3D(2.4, 1, 0);

        private WorldTextRenderer WorldTextRenderer = new WorldTextRenderer();
		private BillboardNumberRenderer BillboardRenderer = new BillboardNumberRenderer();
		private bool _initialised = false;
		private Vector3D _position;
		private uint _rotation;

		/// <summary>
		/// Clear out any old pending draw calls pre-render
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="renderOrigin"></param>
		/// <param name="entity"></param>
		/// <param name="avatarController"></param>
		/// <param name="renderTimestep"></param>
		public override void BeforeRender(DeviceContext graphics, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep) {

			WorldTextRenderer.Init(graphics);

			if (!this._initialised)
			{
				//GameContext.DebugGraphics.Enabled = true;

				BulletinBoardTileStateEntityLogic logic = entity.TileStateEntityLogic as BulletinBoardTileStateEntityLogic;
				if (logic != null)
				{
					this._position = logic.GetCenterPosition();

					if (this._position != Vector3D.Zero)
					{
						this._rotation = (logic.GetVariation() >> 10) & 3;
                        this._initialised = true;

						float scale = GameContext.RandomSource.NextFloat(0.5f, 2.5f);
						this.WorldTextRenderer.DrawString("test banaan bot ad bots knoert bakker aapje boop bam 123", new Vector2F((float)this.BoardRegion.X, (float)this.BoardRegion.Y), BmFontAlign.Center | BmFontAlign.Middle, Vector3D.Zero, this._position, scale, this._rotation, Color.Azure);
                    }
				}
			}
		}

		public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode)
		{
			if (renderMode == RenderMode.Normal)
			{
				this.WorldTextRenderer.Draw(graphics, renderOrigin, matrix);

				BulletinBoardTileStateEntityLogic logic = entity.TileStateEntityLogic as BulletinBoardTileStateEntityLogic;
				if (logic != null)
				{
					Vector3D rotatedRadius = VectorHelper.RotatePosition(this._rotation, BoardRegion * 0.5);
                    GameContext.DebugGraphics.DrawBoxCentered(this._position, rotatedRadius, Color.Yellow, -1);
				}
			}
		}

		public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade, int updateSteps) {}
		public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }
		public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {}

		public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) {}
		protected override void Dispose(bool disposing) {

			if (disposing && this.WorldTextRenderer != null)
			{
				//this.WorldTextRenderer.Dispose();
			}
		}
	}
}
