using Plukit.Base;
using Staxel.Logic;
using Staxel.Tiles;
using Staxel.TileStates;

namespace BetterSigns.Staxel.TileState
{
	class BetterSignTileStateBuilder : ITileStateBuilder
	{
		public void Load() { }
		public void Dispose() {}

		/// <summary>
		/// Return the tileState identifier for this tileStateBuilder
		/// </summary>
		/// <returns></returns>
		public string Kind()
		{
			return "mods.betterSigns.tileState.sign";
		}

		/// <summary>
		/// Spawns a new entity
		/// </summary>
		/// <param name="location"></param>
		/// <param name="tile"></param>
		/// <param name="universe"></param>
		/// <returns></returns>
		public Entity Instance(Vector3I location, Tile tile, Universe universe)
		{
			return BetterSignTileStateEntityBuilder.Spawn(universe, tile, location);
		}
	}
}
