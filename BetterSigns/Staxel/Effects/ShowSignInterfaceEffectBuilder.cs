using Microsoft.Xna.Framework;
using Plukit.Base;
using Staxel;
using Staxel.Draw;
using Staxel.Effects;
using Staxel.Logic;
using Staxel.Rendering;
using Sunbeam.Staxel.Rendering.BitmapFont;
using System;

namespace BetterSigns.Staxel.Effects
{
	public sealed class ShowSignInterfaceEffectBuilder : IEffectBuilder, IDisposable
	{
		public sealed class EffectInstance : IEffect, IDisposable
		{
			private bool _completed;
			private long _entityId;
			private string _message;
			private Color _color;
			private float _scale;
			private BmFontAlign _align;

			private EntityUniverseFacade _entityUniverseFacade;

			public EffectInstance(Timestep step, Entity entity, EntityPainter painter, EntityUniverseFacade facade, Blob data)
			{
				this._entityId = data.GetLong("id");
				this._message = data.GetString("message");
				this._color = new Color() { PackedValue = (uint)data.GetLong("color") };
				this._scale = (float)data.GetDouble("scale");
				this._align = (BmFontAlign)data.GetLong("align");
			}

			public void Pause() { }
			public void Resume() { }
			public void Stop() { }
			public void Dispose() { }

			public bool Completed()
			{
				return this._completed;
			}

			/// <summary>
			/// Abuse the render function to trigger our UI
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="painter"></param>
			/// <param name="renderTimestep"></param>
			/// <param name="graphics"></param>
			/// <param name="matrix"></param>
			/// <param name="renderOrigin"></param>
			/// <param name="position"></param>
			/// <param name="renderMode"></param>
			public void Render(Entity entity, EntityPainter painter, Timestep renderTimestep, DeviceContext graphics, Matrix4F matrix, Vector3D renderOrigin, Vector3D position, RenderMode renderMode)
			{
				if (!this._completed && renderMode == RenderMode.Normal)
				{
					this._completed = true;
					if (ClientContext.PlayerFacade.IsLocalPlayer(entity) && !ClientContext.OverlayController.IsOpen())
					{
						BetterSigns.Instance.SignController.Setup(this._entityId, this._message, this._color, this._scale, this._align);
						BetterSigns.Instance.SignController.Show();
					}
				}
			}
		}

		public static string KindCode()
		{
			return "mods.bulletinboard.effect.ShowSignInterface";

		}

		public string Kind()
		{
			return ShowSignInterfaceEffectBuilder.KindCode();
		}

		public void Dispose() { }
		public void Load() { }

		public IEffect Instance(Timestep step, Entity entity, EntityPainter painter, EntityUniverseFacade facade, Blob data, EffectDefinition definition, EffectMode mode)
		{
			return new EffectInstance(step, entity, painter, facade, data);
		}

		/// <summary>
		/// Static BuildTrigger function to trigger the effect
		/// </summary>
		/// <returns></returns>
		public static EffectTrigger BuildTrigger(Blob data)
		{
			return new EffectTrigger(ShowSignInterfaceEffectBuilder.KindCode(), data);
		}
	}
}
