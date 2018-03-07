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
		private static BmFontFile FontFile;
		private static Dictionary<char, BmFontChar> CharacterMap = new Dictionary<char, BmFontChar>();

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
		public void DrawString(string text, Vector2F textBox, BmFontAlign alignment, Vector3D offset, Vector3D location, float scale, uint rotation, Color color)
		{
			this._drawCalls.Add(new BmFontDrawCall(this.BuildDrawable(text, textBox, alignment, color, scale), location, offset, rotation));
		}

		/// <summary>
		/// Build a TextureVertexDrawable from the text
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		private TextureVertexDrawable BuildDrawable(string text, Color color, float scale = 1f)
		{
			float UnitScale = WorldTextRenderer.Units * scale;
			int verticesCount = text.Length * 4;
			TextureVertex[] vertices = new TextureVertex[verticesCount];

			float x = 0f;
			int i = 0;
			foreach (char l in text)
			{
				if (WorldTextRenderer.CharacterMap.TryGetValue(l, out BmFontChar fontChar))
				{
					Vector4F UVPos = this.CharPositionToUV(fontChar.GetVector4I());

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

		/// <summary>
		/// Build a TextureVertexDrawable from the text where it is contained within a region
		/// </summary>
		/// <param name="text"></param>
		/// <param name="textBox"></param>
		/// <param name="alignment"></param>
		/// <param name="color"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
        private TextureVertexDrawable BuildDrawable(string text, Vector2F textBox, BmFontAlign alignment, Color color, float scale)
        {
			float UnitScale = WorldTextRenderer.Units * scale;
			int verticesCount = text.Length * 4;
            TextureVertex[] vertices = new TextureVertex[verticesCount];
			List<BmFontCharInfo> charInfo = new List<BmFontCharInfo>();
			List<float> lineWidths = new List<float>();

			// Textbox edge locations
			float textBoxLeft = 0f;
			float textBoxRight = textBox.X;
			float textBoxBottom = -textBox.Y * 0.5f;
			float textBoxTop = textBox.Y * 0.5f;

			// Keep track of offset positions
			int vIdx = 0; // Vertex index
			float x = textBoxLeft;
			float y = textBoxTop;
			float lineHeight = WorldTextRenderer.FontFile.Info.Size * UnitScale;
			float lineWidth = 0f;
			int lineNumber = 1;
			int wordNumber = 1;

			// First we build everything, afterward we reposition vertices according to the BmFontCharinfo and alignment
			// Should be slightly more efficient as to realign all previous characters on the line as soon as we add a new one
			foreach (char c in text)
            {
				BmFontChar fontChar = WorldTextRenderer.CharacterMap.GetOrDefault(c);
				float xOffset = fontChar.XOffset * UnitScale;
				float yOffset = fontChar.YOffset * UnitScale;
				float xAdvance = fontChar.XAdvance * UnitScale;
				float width = fontChar.Width * UnitScale;
				float height = fontChar.Height * UnitScale;

				// Outside of bottom boundary; just stop
				if(y - yOffset - height < textBoxBottom)
				{
					break;
				}

				// Newline supplied or needed
				if(c == '\r' || c == '\n' || (lineWidth + width >= textBox.X))
				{
					x = textBoxLeft;
					y -= lineHeight;

					// If we exceed the textbox and are not on the first word
					// we move the last word down a line
					if((lineWidth + width >= textBox.X) && wordNumber != 1)
					{
						float previousLineWidth = lineWidth;
						lineWidth = 0f;

						for (int i = 0; i < charInfo.Count; i++)
						{
							if(charInfo[i].LineNumber == lineNumber && charInfo[i].WordNumber == wordNumber)
							{
								charInfo[i].LineNumber++;
								charInfo[i].WordNumber = 1;

								vertices[charInfo[i].FirstVertexIndex].Position.X = x + charInfo[i].XOffset;
								vertices[charInfo[i].FirstVertexIndex].Position.Y = y - charInfo[i].Height;
								vertices[charInfo[i].FirstVertexIndex + 1].Position.X = x + charInfo[i].XOffset;
								vertices[charInfo[i].FirstVertexIndex + 1].Position.Y = y;
								vertices[charInfo[i].FirstVertexIndex + 2].Position.X = x + charInfo[i].XOffset + charInfo[i].Width;
								vertices[charInfo[i].FirstVertexIndex + 2].Position.Y = y;
								vertices[charInfo[i].FirstVertexIndex + 3].Position.X = x + charInfo[i].XOffset + charInfo[i].Width;
								vertices[charInfo[i].FirstVertexIndex + 3].Position.Y = y - charInfo[i].Height;

								x += charInfo[i].XAdvance;
								lineWidth += charInfo[i].Width;
							}
						}

						lineWidths.Add(previousLineWidth - lineWidth);
					}
					else
					{
						lineWidths.Add(lineWidth);
						lineWidth = 0f;
					}

					wordNumber = 1;
					lineNumber++;
				}

				// Ignore newlines / tab
				if(c == '\r' || c == '\n' || c == '\t')
				{
					continue;
				}

				Vector4F UVPos = this.CharPositionToUV(fontChar.GetVector4I());

				vertices[vIdx].Color = color;
				vertices[vIdx + 1].Color = color;
				vertices[vIdx + 2].Color = color;
				vertices[vIdx + 3].Color = color;

				vertices[vIdx].Position = new Vector3F(x + xOffset, y - height, 0f);
				vertices[vIdx + 1].Position = new Vector3F(x + xOffset, y, 0f);
				vertices[vIdx + 2].Position = new Vector3F(x + xOffset + width, y, 0f);
				vertices[vIdx + 3].Position = new Vector3F(x + xOffset + width, y - height, 0f);

				vertices[vIdx].Texture = new Vector2F(UVPos.X, UVPos.W);
				vertices[vIdx + 1].Texture = new Vector2F(UVPos.X, UVPos.Y);
				vertices[vIdx + 2].Texture = new Vector2F(UVPos.Z, UVPos.Y);
				vertices[vIdx + 3].Texture = new Vector2F(UVPos.Z, UVPos.W);

				if(c == ' ')
				{
					wordNumber++;
				}

				charInfo.Add(new BmFontCharInfo(c, vIdx, xOffset, yOffset, width, height, xAdvance, lineNumber, wordNumber));

				if (c == ' ')
				{
					wordNumber++;
				}

				x += xAdvance;
				lineWidth += width;
				vIdx += 4;
			}

			// Add last line, this never creates a new line so we need to manually add it
			lineWidths.Add(lineWidth);

			for (int line = 0; line < lineWidths.Count; line++)
			{
				lineWidth = lineWidths[line];
				float offsetX = 0;

				if(alignment == BmFontAlign.Center)
				{
					offsetX = (textBox.X - lineWidth) / 2;
				}

				if(alignment == BmFontAlign.Right)
				{
					offsetX = textBox.X - lineWidth;
				}

				for(int j = 0; j < charInfo.Count; j++)
				{
					if (charInfo[j].LineNumber == (line + 1))
					{
						vertices[charInfo[j].FirstVertexIndex].Position.X += offsetX;
						vertices[charInfo[j].FirstVertexIndex + 1].Position.X += offsetX;
						vertices[charInfo[j].FirstVertexIndex + 2].Position.X += offsetX;
						vertices[charInfo[j].FirstVertexIndex + 3].Position.X += offsetX;
					}
				}
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

			WorldTextRenderer.FontFile = BmFontLoader.Load(BulletinBoardManager.Instance.GetModDirectory() + "/Font/plixel.fnt");
			WorldTextRenderer.FontTexture = graphics.GetTexture("mods/BulletinBoard/Font/plixel_0");

			foreach (BmFontChar fChar in WorldTextRenderer.FontFile.Chars)
			{
				char c = (char)fChar.ID;
				WorldTextRenderer.CharacterMap.Add(c, fChar);
			}
		}
	}
}
