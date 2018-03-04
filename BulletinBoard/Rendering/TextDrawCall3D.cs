using Microsoft.Xna.Framework;
using Plukit.Base;

namespace BulletinBoard.Rendering
{
	public struct TextDrawCall3D
	{
		public readonly string Message;
		public readonly Vector3D Location;
		public readonly uint rotation;

		public TextDrawCall3D(string message, Vector3D location, uint rotation)
		{
			this.Message = message;
			this.Location = location;
			this.rotation = rotation;
		}
	}
}
