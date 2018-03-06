using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Staxel.Draw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBoard.Rendering
{
	class WorldTextRenderer
	{
		private static float Units = 0.06f / 36f;
		private static Texture2D FontTexture;
		private static FontFile FontFile;
		private static Dictionary<char, FontChar> CharacterMap = new Dictionary<char, FontChar>();

		private List<BmFontDrawCall> _drawCalls = new List<BmFontDrawCall>();

		/// <summary>
		/// Clear out any pending draw calls
		/// </summary>
		public void Purge()
		{
			this._drawCalls.Clear();
		}

		/// <summary>
		/// Draw all of the drawcalls to the DeviceContext
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="renderOrigin"></param>
		/// <param name="matrix"></param>
		public void Draw(DeviceContext graphics, Vector3D renderOrigin, Matrix4F matrix)
		{
			if (this._drawCalls.Count > 0)
			{
				graphics.PushShader();
				graphics.PushRenderState();
				graphics.SetShader(graphics.GetShader("NameTag"));
				graphics.SetTexture(WorldTextRenderer.FontTexture);
				graphics.SetRasterizerState(CullMode.None);

				foreach (BmFontDrawCall drawCall in this._drawCalls)
				{
					TextureVertexDrawable drawable = drawCall.Drawable;
					Vector3F delta = (drawCall.Location - renderOrigin).ToVector3F();
					float rotation = ((float)drawCall.Rotation + 2f) * 3.14159274f * 0.5f;

					drawable.Render(graphics, Matrix4F.Identity
						.Translate(drawCall.Offset.ToVector3F())
						.Rotate(rotation, Vector3F.UnitY)
						.Translate(delta)
						.Multiply(matrix));
					//float spacer = 0f;
					//int i = 0;

					//foreach (char c in drawCall.Message)
					//{
					//	if (WorldTextRenderer.Drawables.TryGetValue(c, out TextureVertexDrawable textureVertexdrawable) && WorldTextRenderer.CharacterMap.TryGetValue(c, out FontChar fontchar))
					//	{
					//		Color col = colors.Length > i ? colors[i] : Color.White;
					//		textureVertexdrawable = this.BuildDrawable(fontchar, col);

					//		Vector3D charOffset = new Vector3D((double)((float)fontchar.XOffset * WorldTextRenderer.Units * 2f), 0.0, 0.0);
					//		Vector3D spacingOffset = new Vector3D((double)spacer + (float)fontchar.Width * WorldTextRenderer.Units, 0.0, 0.0);

					//		Vector3F delta = (drawCall.Location - renderOrigin).ToVector3F();
					//		Vector2F flatDelta = new Vector2F(delta.X, delta.Z);
					//		float numberRotate = flatDelta.ToAngle() + 3.14159274f;
					//		float rotation = ((float)drawCall.rotation + 2f) * 3.14159274f * 0.5f;
					//		textureVertexdrawable.Render(graphics, Matrix4F.Identity
					//			.Translate(WorldTextRenderer.BoardOffset)
					//			.Translate(spacingOffset.ToVector3F())
					//			.Translate(charOffset.ToVector3F())
					//			//.Translate(offset.ToVector3F())
					//			//.Translate(worldCharOffset)
					//			//.Translate(worldSpacingOffset)
					//			.Rotate(rotation, Vector3F.UnitY)
					//			.Translate(delta)
					//			.Multiply(matrix));

					//		spacer += (float)fontchar.Width * WorldTextRenderer.Units * 2f;
					//		i++;
					//	}
					//}
				}

				graphics.SetRasterizerState(CullMode.CullCounterClockwiseFace);
				graphics.PopShader();
				graphics.PopRenderState();
			}
		}

		/// <summary>
		/// Draw text
		/// </summary>
		/// <param name="v"></param>
		/// <param name="location"></param>
		/// <param name="rotation"></param>
		/// <param name="color"></param>
		public void DrawString(string text, Vector3D location, Vector3D offset, float scale, uint rotation, Color color)
		{
			this._drawCalls.Add(new BmFontDrawCall(this.BuildDrawable(text, color, scale), location, offset, rotation));
		}

		/// <summary>
		/// Draw text contained within a region
		/// </summary>
		/// <param name="text"></param>
		/// <param name="location"></param>
		/// <param name="rotation"></param>
		/// <param name="color"></param>
		public void DrawString(string text, Vector4F textBox, BmFontAlign alignment, Vector3D offset, Vector3D location, float scale, uint rotation, Color color)
		{
			this._drawCalls.Add(new BmFontDrawCall(this.BuildDrawable(text, color, scale), location, offset, rotation));
		}

		private TextureVertexDrawable BuildDrawable(string text, Color color, float scale = 1f)
		{
			int verticesCount = text.Length * 4;
			TextureVertex[] vertices = new TextureVertex[verticesCount];

			float x = 0f;
			int i = 0;
			foreach (char l in text)
			{
				if (WorldTextRenderer.CharacterMap.TryGetValue(l, out FontChar fontChar))
				{
					Vector4F UVPos = this.CharPositionToUV(fontChar.GetVector4I());
					float UnitScale = WorldTextRenderer.Units * scale;

					float xOffset = fontChar.XOffset * UnitScale;
					float yOffset = fontChar.YOffset * UnitScale;
					float xAdvance = fontChar.XAdvance * UnitScale;
					float width = fontChar.Width * UnitScale;
					float height = fontChar.Height * UnitScale;

					if(fontChar.ID == 32)
					{
						xAdvance *= 2;
					}

					vertices[i].Color = color;
					vertices[i + 1].Color = color;
					vertices[i + 2].Color = color;
					vertices[i + 3].Color = color;

					vertices[i].Position = new Vector3F(x + xOffset, 0f - height, 0f);
					vertices[i + 1].Position = new Vector3F(x + xOffset, height, 0f);
					vertices[i + 2].Position = new Vector3F(x + xOffset + width, height, 0f);
					vertices[i + 3].Position = new Vector3F(x + xOffset + width, 0f - height, 0f);

					vertices[i].Texture = new Vector2F(UVPos.X, UVPos.W);
					vertices[i + 1].Texture = new Vector2F(UVPos.X, UVPos.Y);
					vertices[i + 2].Texture = new Vector2F(UVPos.Z, UVPos.Y);
					vertices[i + 3].Texture = new Vector2F(UVPos.Z, UVPos.W); 

					x += xAdvance;
					i += 4;
				}
			}

			return new TextureVertexDrawable(vertices, verticesCount);
		}

        private TextureVertexDrawable buildDrawable(string text, Vector4F textBox, BmFontAlign alignment, Color color, float scale)
        {
            int verticesCount = text.Length * 4;
            TextureVertex[] vertices = new TextureVertex[verticesCount];
            List<int[]> lines = new List<int[]>();

            float x = 0f;
            int i = 0;
            foreach(char l in text)
            {

            }


            return new TextureVertexDrawable(vertices, verticesCount);
        }

		/// <summary>
		/// Converts a font char Vector4I to uv coordinates
		/// </summary>
		/// <param name="charPosition"></param>
		/// <returns></returns>
		private Vector4F CharPositionToUV(Vector4I charPosition)
		{
			return new Vector4F(
				(float)charPosition.X / (float)WorldTextRenderer.FontTexture.Width,
				((float)charPosition.Y / (float)WorldTextRenderer.FontTexture.Height),
				(float)(charPosition.X + charPosition.Z) / (float)WorldTextRenderer.FontTexture.Width,
				((float)(charPosition.Y + charPosition.W) / (float)WorldTextRenderer.FontTexture.Height)
			);
		}

		/// <summary>
		/// Initialize the WorldTextRenderer
		/// across any possible instances
		/// </summary>
		/// <param name="graphics"></param>
		public static void Init(DeviceContext graphics)
		{
			// Do nothing if both are initialized
			if (WorldTextRenderer.FontTexture != null && !WorldTextRenderer.FontTexture.IsDisposed)
			{
				return;
			}

			if (WorldTextRenderer.FontTexture != null && WorldTextRenderer.FontTexture.IsDisposed)
			{
				WorldTextRenderer.CharacterMap.Clear();
			}

			WorldTextRenderer.FontFile = FontLoader.Load(BulletinBoardManager.Instance.GetModDirectory() + "/Font/plixel.fnt");
			WorldTextRenderer.FontTexture = graphics.GetTexture("mods/BulletinBoard/Font/plixel_0");

			foreach (FontChar fChar in WorldTextRenderer.FontFile.Chars)
			{
				char c = (char)fChar.ID;
				WorldTextRenderer.CharacterMap.Add(c, fChar);
			}
		}
	}
}
