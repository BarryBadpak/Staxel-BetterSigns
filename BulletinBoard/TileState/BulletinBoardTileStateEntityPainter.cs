using BulletinBoard.Rendering;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
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
		private Vector3D BoardOffset = new Vector3D(0.25, 0, 0.01);
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
				BulletinBoardTileStateEntityLogic logic = entity.TileStateEntityLogic as BulletinBoardTileStateEntityLogic;
				if (logic != null)
				{
					this._position = logic.GetInitialDrawPosition();

					if (this._position != Vector3D.Zero)
					{
						this._rotation = (logic.GetVariation() >> 10) & 3;
						this._initialised = true;

						Color[] colors = new Color[] { Color.AliceBlue, Color.Azure, Color.Bisque, Color.Blue, Color.Coral, Color.DarkGreen, Color.Firebrick, Color.Green, Color.Honeydew, Color.IndianRed, Color.LightSalmon, Color.Maroon, Color.Olive, Color.Orange, Color.Purple };
						BmFontAlign[] align = new BmFontAlign[] { BmFontAlign.Center, BmFontAlign.Left, BmFontAlign.Right };

						int alignIdx = GameContext.RandomSource.Next(align.Length);
						int colorIdx = GameContext.RandomSource.Next(colors.Length);
						float scale = GameContext.RandomSource.NextFloat(0.5f, 2.5f);
						this.WorldTextRenderer.DrawString("test banaan bot ad bots knoert bakker aapje boop bam 123", new Vector2F(0.5f, 1f), align[alignIdx], this.BoardOffset, this._position, scale, this._rotation, colors[colorIdx]);
					}
				}
			}
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
