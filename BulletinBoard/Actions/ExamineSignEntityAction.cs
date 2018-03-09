using BulletinBoard.Effects;
using Staxel.EntityActions;
using Staxel.Logic;

namespace BulletinBoard.Actions
{
	class ExamineSignEntityAction : EntityActionDriver
	{

		public static string KindCode()
		{
			return "mods.bulletinboard.entityAction.ExamineSign";
		}

		public override string Kind()
		{
			return ExamineSignEntityAction.KindCode();
		}

		public ExamineSignEntityAction()
		{
			base.RegisterState("Begin", this.Begin, false, null, false);
			//base.RegisterState("Idle", this.Idle, true, this.IdleLoop, false);
			base.RegisterState("End", this.End, false, null, false);
		}

		public override void Start(Entity entity, EntityUniverseFacade facade)
		{
			entity.Effects.Trigger(ShowSignInterfaceEffectBuilder.BuildTrigger());
			base.RunAnimation(entity, "staxel.emote.Wardrobe.open", "Begin");
		}

		public void Begin(Entity entity, EntityUniverseFacade facade)
		{
			base.RunAnimation(entity, "staxel.emote.Wardrobe.idle", "end");
		}

		public void Idle(Entity entity, EntityUniverseFacade facade)
		{
			base.RunAnimation(entity, "staxel.emote.Wardrobe.idle", "Idle");
		}

		/*private void IdleLoop(Entity entity, EntityUniverseFacade facade)
		{
			PlayerEntityLogic player = entity.PlayerEntityLogic;
			if (player.GetActionCookie("signDone", false))
			//if (player.GetActionCookie("totemDone", false))
			{
				base.RunAnimation(entity, "staxel.emote.Wardrobe.close", "End");
			}
		}*/

		public void End(Entity entity, EntityUniverseFacade facade)
		{
			entity.Logic.ActionFacade.NoNextAction();
			this.OnCancel(entity);
		}
	}
}
