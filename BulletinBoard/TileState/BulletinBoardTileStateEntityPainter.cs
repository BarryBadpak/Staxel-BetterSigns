using BulletinBoard.Rendering;
using Plukit.Base;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Logic;
using Staxel.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBoard.TileState
{
	class BulletinBoardTileStateEntityPainter : EntityPainter
	{ 
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
			//this.WorldTextRenderer.Purge();
		}

		public override void Render(DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Entity entity, AvatarController avatarController, Timestep renderTimestep, RenderMode renderMode)
		{
			if (renderMode == RenderMode.Normal)
			{
				this.WorldTextRenderer.Draw(graphics, renderOrigin, matrix);
			}
		}

		public override void RenderUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade, int updateSteps) {}

		public override void ClientPostUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) { }
		public override void ClientUpdate(Timestep timestep, Entity entity, AvatarController avatarController, EntityUniverseFacade facade) {

			if (!this._initialised)
			{
				BulletinBoardTileStateEntityLogic logic = entity.TileStateEntityLogic as BulletinBoardTileStateEntityLogic;
				if (logic != null)
				{
					this._position = logic.GetInitialDrawPosition();

					if (this._position != Vector3D.Zero)
					{
						this._rotation = (logic.GetVariation() >> 10) & 3;
						this._initialised = true;

						this.WorldTextRenderer.DrawString("De kat krabt de krullen van de trap.", this._position, this._rotation);
					}
				}
			}
		}

		public override void StartEmote(Entity entity, Timestep renderTimestep, EmoteConfiguration emote) {}
		protected override void Dispose(bool disposing) {

			if (disposing && this.WorldTextRenderer != null)
			{
				//this.WorldTextRenderer.Dispose();
			}
		}
	}
}
