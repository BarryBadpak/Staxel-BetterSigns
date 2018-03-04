using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBoard.TileState
{
	class BulletinBoardTileStateEntityBuilder : IEntityPainterBuilder, IEntityLogicBuilder2
	{
		public string Kind { get { return KindCode; } }
		public static string KindCode { get { return "mods.bulletinboard.tileStateEntity.BulletinBoard"; } }

		public void Load() { }

		/// <summary>
		/// Renderer instance
		/// </summary>
		/// <returns></returns>
		EntityPainter IEntityPainterBuilder.Instance()
		{
			return new BulletinBoardTileStateEntityPainter();
		}

		/// <summary>
		/// Logic instance
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		EntityLogic IEntityLogicBuilder.Instance(Entity entity, bool server)
		{
			return new BulletinBoardTileStateEntityLogic(entity, server);
		}

		/// <summary>
		/// Statically construct a new BulletinBoard entity
		/// </summary>
		/// <param name="facade"></param>
		/// <param name="tile"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static Entity Spawn(EntityUniverseFacade facade, Tile tile, Vector3I location)
		{
			EntityId entityId = facade.AllocateNewEntityId();
			Entity entity = new Entity(entityId, false, KindCode, true);
			Blob blob = BlobAllocator.Blob(true);

			blob.SetString("tile", tile.Configuration.Code);
			blob.FetchBlob("location").SetVector3I(location);
			blob.SetLong("variant", tile.Variant());
			blob.FetchBlob("position").SetVector3D(location.ToTileCenterVector3D());
			blob.FetchBlob("velocity").SetVector3D(Vector3D.Zero);

			entity.Construct(blob, facade);
			Blob.Deallocate(ref blob);
			facade.AddEntity(entity);

			return entity;
		}

		public bool IsTileStateEntityKind()
		{
			return true;
		}
	}
}
