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
		private List<TextDrawCall3D> _drawCalls = new List<TextDrawCall3D>();
		private static float Units = 0.03f / 36f;
		private static Vector3F BoardOffset = new Vector3F(0.25f, 0f, 0.01f);
		private static Texture2D FontTexture;
		private static FontFile FontFile;
		private static Dictionary<char, FontChar> CharacterMap = new Dictionary<char, FontChar>();
		private static Dictionary<char, TextureVertexDrawable> Drawables = new Dictionary<char, TextureVertexDrawable>();
		
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
			if(this._drawCalls.Count > 0)
			{
				this.Init(graphics);

				graphics.PushShader();
				graphics.PushRenderState();
				graphics.SetShader(graphics.GetShader("NameTag"));
				graphics.SetTexture(WorldTextRenderer.FontTexture);
				graphics.SetRasterizerState(CullMode.None);

				Color[] colors = new Color[] { Color.AliceBlue, Color.Azure, Color.Bisque, Color.Blue, Color.Coral, Color.DarkGreen, Color.Firebrick, Color.Green, Color.Honeydew, Color.IndianRed, Color.LightSalmon, Color.Maroon, Color.Olive, Color.Orange, Color.Purple };

				foreach (TextDrawCall3D drawCall in this._drawCalls)
				{
					float spacer = 0f;
					int i = 0;

					foreach (char c in drawCall.Message)
					{
						if (WorldTextRenderer.Drawables.TryGetValue(c, out TextureVertexDrawable textureVertexdrawable) && WorldTextRenderer.CharacterMap.TryGetValue(c, out FontChar fontchar))
						{
							Color col = colors.Length > i ? colors[i] : Color.White;
							textureVertexdrawable = this.BuildDrawable(fontchar, col);

							Vector3D charOffset = new Vector3D((double)((float)fontchar.XOffset * WorldTextRenderer.Units * 2f), 0.0, 0.0);
							Vector3D spacingOffset = new Vector3D((double)spacer + (float)fontchar.Width * WorldTextRenderer.Units, 0.0, 0.0);

							Vector3F delta = (drawCall.Location - renderOrigin).ToVector3F();
							Vector2F flatDelta = new Vector2F(delta.X, delta.Z);
							float numberRotate = flatDelta.ToAngle() + 3.14159274f;
							float rotation = ((float)drawCall.rotation + 2f) * 3.14159274f * 0.5f;
							textureVertexdrawable.Render(graphics, Matrix4F.Identity
								.Translate(WorldTextRenderer.BoardOffset)
								.Translate(spacingOffset.ToVector3F())
								.Translate(charOffset.ToVector3F())
								//.Translate(offset.ToVector3F())
								//.Translate(worldCharOffset)
								//.Translate(worldSpacingOffset)
								.Rotate(rotation, Vector3F.UnitY)
								.Translate(delta)
								.Multiply(matrix));

							spacer += (float)fontchar.Width * WorldTextRenderer.Units * 2f;
							i++;
						}
					}
				}

				graphics.SetRasterizerState(CullMode.CullCounterClockwiseFace);
				graphics.PopShader();
				graphics.PopRenderState();
			}
		}

		/// <summary>
		/// Adds a new text draw call
		/// They need to be actually drawn using a call to draw
		/// </summary>
		/// <param name="message"></param>
		/// <param name="location"></param>
		public void DrawString(string message, Vector3D location, uint rotation)
		{
			this._drawCalls.Add(new TextDrawCall3D(message, location, rotation));
		}

		private TextureVertexDrawable BuildDrawable(FontChar fontChar, Color col)
		{
			Vector4F vec = new Vector4F( // Pixel coords to uv coords
				(float)fontChar.X / (float)WorldTextRenderer.FontTexture.Width,
				((float)fontChar.Y / (float)WorldTextRenderer.FontTexture.Height),
				(float)(fontChar.X + fontChar.Width) / (float)WorldTextRenderer.FontTexture.Width,
				((float)(fontChar.Y + fontChar.Height) / (float)WorldTextRenderer.FontTexture.Height)
			);

			float fWidth = (float)fontChar.Width * WorldTextRenderer.Units;
			float fHeight = (float)fontChar.Height * WorldTextRenderer.Units;

			TextureVertex[] vertices = new TextureVertex[4];
			vertices[0].Color = col;
			vertices[1].Color = col;
			vertices[2].Color = col;
			vertices[3].Color = col;

			vertices[0].Position = new Vector3F(0f - fWidth, 0f - fHeight, 0f);
			vertices[1].Position = new Vector3F(0f - fWidth, fHeight, 0f);
			vertices[2].Position = new Vector3F(fWidth, fHeight, 0f);
			vertices[3].Position = new Vector3F(fWidth, 0f - fHeight, 0f);

			vertices[0].Texture = new Vector2F(vec.X, vec.W);
			vertices[1].Texture = new Vector2F(vec.X, vec.Y);
			vertices[2].Texture = new Vector2F(vec.Z, vec.Y);
			vertices[3].Texture = new Vector2F(vec.Z, vec.W);

			return new TextureVertexDrawable(vertices, 4);
		}

		/// <summary>
		/// Initialize the WorldTextRenderer
		/// across any possible instances
		/// </summary>
		/// <param name="graphics"></param>
		private void Init(DeviceContext graphics)
		{
			// Do nothing if both are initialized
			if(WorldTextRenderer.Drawables.Count > 0 && WorldTextRenderer.FontTexture != null && !WorldTextRenderer.FontTexture.IsDisposed)
			{
				return;
			}

			if (WorldTextRenderer.FontTexture != null && WorldTextRenderer.FontTexture.IsDisposed)
			{
				WorldTextRenderer.Drawables.Clear();
				WorldTextRenderer.CharacterMap.Clear();
			}

			WorldTextRenderer.FontFile = FontLoader.Load(BulletinBoardManager.Instance.GetModDirectory() + "/Font/plixel.fnt");
			WorldTextRenderer.FontTexture = graphics.GetTexture("mods/BulletinBoard/Font/plixel_0");

			foreach (FontChar fChar in WorldTextRenderer.FontFile.Chars)
			{
				char c = (char)fChar.ID;
				WorldTextRenderer.CharacterMap.Add(c, fChar);
				WorldTextRenderer.Drawables.Add(c, this.BuildDrawable(fChar, Color.White));
			}
		}
	}
}
