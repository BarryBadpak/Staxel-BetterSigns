using Plukit.Base;

namespace BetterSigns.Staxel.Components
{
	/// <summary>
	/// Represents the values contained within the "betterSign" tile config
	/// </summary>
	public class SignComponent
	{
		public Vector3D offsetFromCenter { get; private set; }
		public Vector2F textRegionSize { get; private set; }

		public SignComponent(Blob config)
		{
			this.offsetFromCenter = config.Contains("offsetFromCenter") ? config.GetBlob("offsetFromCenter").GetVector3D() : Vector3D.Zero;
			this.textRegionSize = config.Contains("textRegionSize") ? config.GetBlob("textRegionSize").GetVector2F() : Vector2F.One;
		}
	}
}
