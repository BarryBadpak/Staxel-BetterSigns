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
				GameContext.DebugGraphics.Enabled = true;

				BulletinBoardTileStateEntityLogic logic = entity.TileStateEntityLogic as BulletinBoardTileStateEntityLogic;
				if (logic != null)
				{
					this._position = logic.GetInitialDrawPosition();

					if (this._position != Vector3D.Zero)
					{
						this._rotation = (logic.GetVariation() >> 10) & 3;
						this._initialised = true;

						Color[] colors = new Color[] { Color.AliceBlue, Color.Azure, Color.Bisque, Color.Blue, Color.Coral, Color.DarkGreen, Color.Firebrick, Color.Green, Color.Honeydew, Color.IndianRed, Color.LightSalmon, Color.Maroon, Color.Olive, Color.Orange, Color.Purple };
						BmFontAlign[] align = new BmFontAlign[] { BmFontAlign.Center | BmFontAlign.Middle, BmFontAlign.Left, BmFontAlign.Bottom, BmFontAlign.Right | BmFontAlign.Middle };

						int alignIdx = GameContext.RandomSource.Next(align.Length);
						int colorIdx = GameContext.RandomSource.Next(colors.Length);
						float scale = GameContext.RandomSource.NextFloat(0.5f, 2.5f);
						this.WorldTextRenderer.DrawString("test banaan bot ad bots knoert bakker aapje boop bam 123", new Vector2F(1f, 1f), align[alignIdx], this.BoardOffset, this._position, scale, this._rotation, colors[colorIdx]);

						//GameContext.DebugGraphics.DrawLine(delta - (this.BoardOffset * scale * 0.5f), delta + (this.BoardOffset * scale * 0.5f), Color.Azure, 10000);
					}
				}
			}
			//this.WorldTextRenderer.Purge();
		}

		public Vector3D RotatePosition(uint rotation, Vector3D vec)
		{
			switch (rotation)
			{
				case 0u:
					return vec;
				case 1u:
					return new Vector3D(vec.Z, vec.Y, 0.0 - vec.X);
				case 2u:
					return new Vector3D(0.0 - vec.X, vec.Y, 0.0 - vec.Z);
				case 3u:
					return new Vector3D(0.0 - vec.Z, vec.Y, vec.X);
				default:
					throw new Exception();
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
					// USE RotatePosition everywhere
					Vector3D rotatedOffset = this.RotatePosition(this._rotation, this.BoardOffset);
					Vector3D rotatedRadius = this.RotatePosition(this._rotation, new Vector3D(0.5, 0.5, 0));
					GameContext.DebugGraphics.DrawBoxCentered(this._position + rotatedOffset, rotatedRadius, Color.Yellow, -1);
					//GameContext.DebugGraphics.DrawBoxCentered(this._position+this.BoardOffset, new Vector3D(0.5, 0.5, 0), Color.Yellow, -1);
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
