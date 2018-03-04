using Sunbeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletinBoard
{
    public class BulletinBoardManager: SunbeamMod
    {
		public override string ModIdentifier => "BulletinBoard";
		public static BulletinBoardManager Instance { get; private set; }

		public BulletinBoardManager()
		{
			BulletinBoardManager.Instance = this;
		}

		/// <summary>
		/// Returns the mod directory
		/// </summary>
		/// <returns></returns>
		public string GetModDirectory()
		{
			return this.AssetLoader.ModDirectory;
		}
    }
}
