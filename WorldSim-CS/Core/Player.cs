using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldSimCS
{
	public enum PlayerId : int { }

	public enum ActionType
	{
		None,
		Move,
		Attack,
		Trade,
		Wait
	}

	public struct PlayerAction
	{
		public PlayerId PlayerId;
		public ActionType Type;
		public Vector2 Target;
		public int EntityId;

		public static PlayerAction NoneAction = new PlayerAction { PlayerId = (PlayerId)0, Type = ActionType.None };
	}

	public class PlayerState
	{
		public PlayerId Id { get; }
		public List<int> EntityIds { get; }

		public PlayerState(PlayerId id)
		{
			Id = id;
			EntityIds = new List<int>();
		}
	}

	public struct PlayerInputFrame
	{
		public int Tick;
		public List<PlayerAction> Actions;

		public PlayerInputFrame(int tick)
		{
			Tick = tick;
			Actions = new List<PlayerAction>(64);
		}
	}
}

