﻿using BulletinBoard.Actions;
using BulletinBoard.Rendering;
using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Notifications;
using Staxel.Tiles;
using Staxel.TileStates;
using System;

namespace BulletinBoard.TileState
{
	public class BulletinBoardTileStateEntityLogic : TileStateEntityLogic
	{
		private TileConfiguration _configuration;
		private bool _isRemoved;
		private bool _serverMode;
		private uint _variant;
		private Vector3D _centerPos = Vector3D.Zero;
		private Vector3D _centerPosOffset = new Vector3D(0, 0.30, -0.2);

		private string _message = "";
		private Color _color = Color.White;
		private float _scale = 1f;

		public BulletinBoardTileStateEntityLogic(Entity entity, bool serverMode) : base(entity)
		{
			this._serverMode = serverMode;
			this.Entity.Physics.PriorityChunkRadius(0, false);
		}

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="timestep"></param>
		/// <param name="universe"></param>
		public override void Update(Timestep timestep, EntityUniverseFacade universe)
		{
			// Following check is taken from Daemons mod
			// Don't let this logic do anything if tile does not exist. Might be accessed after removal of tile.
			if (!universe.ReadTile(Location, TileAccessFlags.None, out Tile tile))
			{
				return;
			}

			// Following check is taken from Daemons mod
			// If the tile/variant has changed, or it doesn't have a Sprinkler Component set this logic to be removed.
			if ((tile.Configuration != this._configuration) || this._variant != tile.Variant())
			{
				this._isRemoved = true;

				if (tile.Configuration == this._configuration)
				{
					universe.RemoveTile(base.Entity, base.Location, TileAccessFlags.None);
				}

				return;
			}

			// If the centerPos is not set lets set it
			if (this._centerPos == Vector3D.Zero)
			{
				uint rotation = tile.Configuration.Rotation(tile.Variant());
				Vector3D offset = VectorHelper.RotatePosition(rotation, this._centerPosOffset);
				this._centerPos = tile.Configuration.TileCenter(this.Location, tile.Variant()) + offset;

				Vector3F tileOffset = default(Vector3F);
				if (universe.TileOffset(base.Location, TileAccessFlags.None, out tileOffset))
				{
					this._centerPos.Y += (double)tileOffset.Y;
				}
			}
		}

		/// <summary>
		/// Returns the centerPos
		/// </summary>
		/// <returns></returns>
		public Vector3D GetCenterPosition()
		{
			return this._centerPos;
		}

		/// <summary>
		/// Returns the variant
		/// </summary>
		/// <returns></returns>
		public uint GetVariation()
		{
			return this._variant;
		}

		/// <summary>
		/// Set temp save data
		/// </summary>
		public override void Store()
		{
			base._blob.FetchBlob("location").SetVector3I(this.Location);
			base._blob.SetLong("variant", this._variant);
			base._blob.SetString("tile", this._configuration.Code);

			base._blob.FetchBlob("centerPos").SetVector3D(this._centerPos);
			base._blob.SetBool("isRemoved", this._isRemoved);

			base._blob.SetString("message", this._message);
			base._blob.SetLong("color", this._color.PackedValue);
			base._blob.SetDouble("scale", this._scale);
		}

		/// <summary>
		/// Restore entity from temp save data
		/// </summary>
		public override void Restore()
		{
			base.Restore();

			this.Location = base._blob.FetchBlob("location").GetVector3I();
			this._variant = (uint)base._blob.GetLong("variant");
			this._configuration = GameContext.TileDatabase.GetTileConfiguration(base._blob.GetString("tile"));

			this._centerPos = base._blob.GetBlob("centerPos").GetVector3D();
			this._isRemoved = base._blob.GetBool("isRemoved");

			this._message = base._blob.GetString("message");
			this._color = new Color() { PackedValue = (uint)base._blob.GetLong("color") };
			this._scale = (float)base._blob.GetDouble("scale");
		}

		/// <summary>
		/// Called whenever the tile entity is created. Setup time!
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="entityUniverseFacade"></param>
		public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade)
		{
			this._configuration = GameContext.TileDatabase.GetTileConfiguration(arguments.GetString("tile"));
			this.Location = arguments.FetchBlob("location").GetVector3I();
			this._variant = (uint)arguments.GetLong("variant");

			this.Entity.Physics.Construct(arguments.FetchBlob("position").GetVector3D(), Vector3D.Zero);
			this.Entity.Physics.MakePhysicsless();
		}

		/// <summary>
		/// Set the persistent save data
		/// </summary>
		/// <param name="data"></param>
		public override void StorePersistenceData(Blob data)
		{
			base.StorePersistenceData(data);

			Blob constructData = data.FetchBlob("constructData");
			constructData.SetString("tile", this._configuration.Code);
			constructData.FetchBlob("location").SetVector3I(this.Location);
			constructData.SetLong("variant", this._variant);
			constructData.FetchBlob("position").SetVector3D(this.Entity.Physics.Position);

			data.SetBool("isRemoved", this._isRemoved);
			data.FetchBlob("centerPos").SetVector3D(this._centerPos);

			data.SetString("message", this._message);
			data.SetLong("color", this._color.PackedValue);
			data.SetDouble("scale", this._scale);
		}

		/// <summary>
		/// Restore from persistent save data
		/// </summary>
		/// <param name="data"></param>
		/// <param name="facade"></param>
		public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade)
		{
			base.RestoreFromPersistedData(data, facade);
			Entity.Construct(data.GetBlob("constructData"), facade);

			this._centerPos = data.GetBlob("centerPos").GetVector3D();
			this._isRemoved = data.GetBool("isRemoved");

			this._message = data.GetString("message");
			this._color = new Color() { PackedValue = (uint)data.GetLong("color") };
			this._scale = (float)data.GetDouble("scale");

			this.Store();
		}

		public override void Bind() { }
		public override void BeingLookedAt(Entity entity) { }
		public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) {

			Notification notif2;
			if (alt.DownClick)
			{
				entity.Logic.ActionFacade.NextAction(ExamineSignEntityAction.KindCode());
				notif2 = GameContext.NotificationDatabase.CreateNotificationFromCode("staxel.notifications.feeder.Empty", entity.Step, NotificationParams.EmptyParams, false);
				entity.PlayerEntityLogic.ShowNotification(notif2);
			}
		}

		public override void KeepAlive() { }
		public override void PostUpdate(Timestep timestep, EntityUniverseFacade universe)
		{
			//If this has been set. Remove this entity.
			if (this._isRemoved)
			{
				universe.RemoveEntity(Entity.Id);
			}
		}

		public override void PreUpdate(Timestep timestep, EntityUniverseFacade entityUniverseFacade) { }

		public override bool IsPersistent()
		{
			return true;
		}

		public override bool IsLingering()
		{
			return this._isRemoved;
		}

		public override bool IsBeingLookedAt()
		{
			return false;
		}

		public override bool Interactable()
		{
			return true;
		}

		public override bool CanChangeActiveItem()
		{
			return false;
		}
	}
}
