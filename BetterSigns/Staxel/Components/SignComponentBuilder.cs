using Plukit.Base;
using Staxel.Core;

namespace BetterSigns.Staxel.Components
{
	/// <summary>
	/// Builder for the "betterSign" key in the tile config
	/// </summary>
	public class SignComponentBuilder: IComponentBuilder
	{
		public string Kind()
		{
			return "betterSign";
		}

		public object Instance(Blob config)
		{
			return new SignComponent(config);
		}
	}
}
