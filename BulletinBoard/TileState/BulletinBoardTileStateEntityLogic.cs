using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace BulletinBoard.TileState
{
	public class BulletinBoardTileStateEntityLogic : TileStateEntityLogic
	{
		private TileConfiguration _configuration;
		private bool _isRemoved;
		private bool _serverMode;
		private Vector3D _topLeftPosition;
		private uint _variant;
		private uint _rotation;

		public BulletinBoardTileStateEntityLogic(Entity entity, bool serverMode) : base(entity)
		{
			this._serverMode = serverMode;
			this.Entity.Physics.PriorityChunkRadius(0, false);
		}

		/// <summary>
		/// Called whenever the tile entity is created. Setup time!
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="entityUniverseFacade"></param>
		public override void Construct(Blob arguments, EntityUniverseFacade entityUniverseFacade)
		{
			this.Location = arguments.FetchBlob("location").GetVector3I();
			this._configuration = GameContext.TileDatabase.GetTileConfiguration(arguments.GetString("tile"));
			this._variant = (uint)arguments.GetLong("variant");

			this.Entity.Physics.Construct(arguments.FetchBlob("position").GetVector3D(), arguments.FetchBlob("velocity").GetVector3D());
			this.Entity.Physics.MakePhysicsless();
		}

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

			//Logger.WriteLine("Rotation for "+ this._variant.ToString() +" -- "+tile.Configuration.Rotation(tile.Variant()).ToString());

			Vector3D coreOffset = default(Vector3D);
			uint rotation = tile.Configuration.Rotation(tile.Variant());
			
			CollisionBox bb = tile.Configuration.FetchBoundingBox(tile.Variant(), out coreOffset);
			this._topLeftPosition = this.Location.ToVector3D() 
				+ coreOffset + (bb.Max._0Y0() * 0.8);

			//Vector3D 3offsetZ = new Vector3D(0, 0, 1)
			//Vector3D 0ffsetX  = new Vector3D(1, 0, 0);
			//Vector3D 3offsetX = new Vector3D(1, 0, 0);
			//Vector3D 2offsetZ = new Vector3D(0, 0, 1);
			Vector3D offset = new Vector3D(rotation == 0 || rotation == 3 ? 1 : rotation == 1 ? 0.5 : 0, 0, rotation == 3 || rotation == 2 ? 1 : rotation == 0 ? 0.5 : 0);

			//Logger.WriteLine(bb.Min.ToString() + " || " + bb.Max.ToString());

			Vector3D size = bb.Min + (bb.Max - bb.Min);
			this._topLeftPosition += size * offset;

			// Worldposition _topLeftPostion + this.Location

			Vector3F tileOffset = default(Vector3F);
			if (universe.TileOffset(base.Location, TileAccessFlags.None, out tileOffset))
			{
				this._topLeftPosition.Y += (double)tileOffset.Y;
			}

			//Logger.WriteLine(this._topLeftPosition.ToString());
		}

		public Vector3D GetInitialDrawPosition()
		{
			return this._topLeftPosition;
		}

		public uint GetVariation()
		{
			return this._variant;
		}

		/// <summary>
		/// Set temp save data
		/// </summary>
		public override void Store()
		{
			this._blob.FetchBlob("location").SetVector3I(this.Location);
			base._blob.FetchBlob("centerPos").SetVector3D(this._topLeftPosition);
			this._blob.SetString("tile", this._configuration.Code);
			base._blob.SetLong("variant", this._variant);
			this._blob.SetBool("isRemoved", this._isRemoved);
		}

		/// <summary>
		/// Restore entity from temp save data
		/// </summary>
		public override void Restore()
		{
			base.Restore();

			this.Location = this._blob.FetchBlob("location").GetVector3I();
			this._topLeftPosition = base._blob.GetBlob("centerPos").GetVector3D();
			this._variant = (uint)base._blob.GetLong("variant");
			this._isRemoved = this._blob.GetBool("isRemoved");
			this._configuration = GameContext.TileDatabase.GetTileConfiguration(this._blob.GetString("tile"));
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
			constructData.FetchBlob("velocity").SetVector3D(Vector3D.Zero);

			data.SetBool("isRemoved", this._isRemoved);
			data.FetchBlob("centerPos").SetVector3D(this._topLeftPosition);
		}

		/// <summary>
		/// Restore from persistent save data
		/// </summary>
		/// <param name="data"></param>
		/// <param name="facade"></param>
		public override void RestoreFromPersistedData(Blob data, EntityUniverseFacade facade)
		{
			Entity.Construct(data.GetBlob("constructData"), facade);
			base.RestoreFromPersistedData(data, facade);

			this._topLeftPosition = data.GetBlob("centerPos").GetVector3D();
			this._isRemoved = data.GetBool("isRemoved");
			this.Store();
		}

		public override void Bind() { }
		public override void BeingLookedAt(Entity entity) { }
		public override void Interact(Entity entity, EntityUniverseFacade facade, ControlState main, ControlState alt) { }
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
			return false;
		}

		public override bool CanChangeActiveItem()
		{
			return false;
		}
	}
}
