// ---- AngelCode BmFont XML serializer ----------------------
// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
// ---- There's no license restrictions, use as you will. ----
// ---- Credits to http://www.angelcode.com/ -----------------

using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Plukit.Base;

namespace BulletinBoard.Rendering
{
	[Serializable]
	[XmlRoot("font")]
	public class BmFontFile
	{
		[XmlElement("info")]
		public BmFontInfo Info
		{
			get;
			set;
		}

		[XmlElement("common")]
		public BmFontCommon Common
		{
			get;
			set;
		}

		[XmlArray("pages")]
		[XmlArrayItem("page")]
		public List<BmFontPage> Pages
		{
			get;
			set;
		}

		[XmlArray("chars")]
		[XmlArrayItem("char")]
		public List<BmFontChar> Chars
		{
			get;
			set;
		}

		[XmlArray("kernings")]
		[XmlArrayItem("kerning")]
		public List<BmFontKerning> Kernings
		{
			get;
			set;
		}
	}

	[Serializable]
	public class BmFontInfo
	{
		[XmlAttribute("face")]
		public String Face
		{
			get;
			set;
		}

		[XmlAttribute("size")]
		public Int32 Size
		{
			get;
			set;
		}

		[XmlAttribute("bold")]
		public Int32 Bold
		{
			get;
			set;
		}

		[XmlAttribute("italic")]
		public Int32 Italic
		{
			get;
			set;
		}

		[XmlAttribute("charset")]
		public String CharSet
		{
			get;
			set;
		}

		[XmlAttribute("unicode")]
		public Int32 Unicode
		{
			get;
			set;
		}

		[XmlAttribute("stretchH")]
		public Int32 StretchHeight
		{
			get;
			set;
		}

		[XmlAttribute("smooth")]
		public Int32 Smooth
		{
			get;
			set;
		}

		[XmlAttribute("aa")]
		public Int32 SuperSampling
		{
			get;
			set;
		}

		private Rectangle _Padding;
		[XmlAttribute("padding")]
		public String Padding
		{
			get
			{
				return _Padding.X + "," + _Padding.Y + "," + _Padding.Width + "," + _Padding.Height;
			}
			set
			{
				String[] padding = value.Split(',');
				_Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]), Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
			}
		}

		private Point _Spacing;
		[XmlAttribute("spacing")]
		public String Spacing
		{
			get
			{
				return _Spacing.X + "," + _Spacing.Y;
			}
			set
			{
				String[] spacing = value.Split(',');
				_Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
			}
		}

		[XmlAttribute("outline")]
		public Int32 OutLine
		{
			get;
			set;
		}
	}

	[Serializable]
	public class BmFontCommon
	{
		[XmlAttribute("lineHeight")]
		public Int32 LineHeight
		{
			get;
			set;
		}

		[XmlAttribute("base")]
		public Int32 Base
		{
			get;
			set;
		}

		[XmlAttribute("scaleW")]
		public Int32 ScaleW
		{
			get;
			set;
		}

		[XmlAttribute("scaleH")]
		public Int32 ScaleH
		{
			get;
			set;
		}

		[XmlAttribute("pages")]
		public Int32 Pages
		{
			get;
			set;
		}

		[XmlAttribute("packed")]
		public Int32 Packed
		{
			get;
			set;
		}

		[XmlAttribute("alphaChnl")]
		public Int32 AlphaChannel
		{
			get;
			set;
		}

		[XmlAttribute("redChnl")]
		public Int32 RedChannel
		{
			get;
			set;
		}

		[XmlAttribute("greenChnl")]
		public Int32 GreenChannel
		{
			get;
			set;
		}

		[XmlAttribute("blueChnl")]
		public Int32 BlueChannel
		{
			get;
			set;
		}
	}

	[Serializable]
	public class BmFontPage
	{
		[XmlAttribute("id")]
		public Int32 ID
		{
			get;
			set;
		}

		[XmlAttribute("file")]
		public String File
		{
			get;
			set;
		}
	}

	[Serializable]
	public class BmFontChar
	{
		[XmlAttribute("id")]
		public Int32 ID
		{
			get;
			set;
		}

		[XmlAttribute("x")]
		public Int32 X
		{
			get;
			set;
		}

		[XmlAttribute("y")]
		public Int32 Y
		{
			get;
			set;
		}

		[XmlAttribute("width")]
		public Int32 Width
		{
			get;
			set;
		}

		[XmlAttribute("height")]
		public Int32 Height
		{
			get;
			set;
		}

		[XmlAttribute("xoffset")]
		public Int32 XOffset
		{
			get;
			set;
		}

		[XmlAttribute("yoffset")]
		public Int32 YOffset
		{
			get;
			set;
		}

		[XmlAttribute("xadvance")]
		public Int32 XAdvance
		{
			get;
			set;
		}

		[XmlAttribute("page")]
		public Int32 Page
		{
			get;
			set;
		}

		[XmlAttribute("chnl")]
		public Int32 Channel
		{
			get;
			set;
		}

		/// <summary>
		/// Returns the location & width,height as a Vector4I
		/// </summary>
		/// <returns></returns>
		public Vector4I GetVector4I()
		{
			return new Vector4I(this.X, this.Y, this.Width, this.Height);
		}
	}

	[Serializable]
	public class BmFontKerning
	{
		[XmlAttribute("first")]
		public Int32 First
		{
			get;
			set;
		}

		[XmlAttribute("second")]
		public Int32 Second
		{
			get;
			set;
		}

		[XmlAttribute("amount")]
		public Int32 Amount
		{
			get;
			set;
		}
	}

	public class BmFontLoader
	{
		public static BmFontFile Load(String filename)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(BmFontFile));
			TextReader textReader = new StreamReader(filename);
			BmFontFile file = (BmFontFile)deserializer.Deserialize(textReader);
			textReader.Close();
			return file;
		}
	}
}