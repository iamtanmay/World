using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldSimCS
{
	public interface ISystem
	{
		void Initialize();
		void Update(Entity[] entities, List<PlayerAction> actions);
	}

	public class InventorySystem : ISystem
	{
		public void Initialize()
		{
		}
		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
		}
	}

	public class TradeSystem : ISystem
	{
		public void Initialize()
		{
		}
		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
		}
	}

	public class QuestSystem : ISystem
	{
		public void Initialize()
		{
		}
		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
		}
	}

	public class DialogueSystem : ISystem
	{
		public void Initialize()
		{
		}
		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
		}
	}

	public class LevellingSystem : ISystem
	{
		public void Initialize()
		{
		}
		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
		}
	}
		
	public class MovementSystem : ISystem
	{
		private Entity[] _entities;

		public void Initialize()
		{
			_entities = new Entity[0];
		}

		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				var entity = entities[i];
				if (!entity.IsAlive) continue;

				var newPos = entity.Position2D.Add(entity.Velocity2D);
				entity.Position2D = newPos;
			}
		}
	}

	public class CombatSystem : ISystem
	{
		public void Initialize() { }

		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				if (action.Type == ActionType.Attack)
				{
					Entity attacker = null;
					for (int e = 0; e < entities.Length; e++)
					{
						if (entities[e].Id == action.EntityId)
						{
							attacker = entities[e];
							break;
						}
					}

					if (attacker != null && attacker.IsAlive)
					{
						for (int j = 0; j < entities.Length; j++)
						{
							var target = entities[j];
							if (target.IsAlive && target.Id != attacker.Id)
							{
								var dist = Distance(attacker.Position2D, target.Position2D);
								if (dist <= FixedPoint.FromInt(2))
								{
									target.Health -= 10;
								}
							}
						}
					}
				}
			}
		}

		private long Distance(Vector2 a, Vector2 b)
		{
			long dx = FixedPoint.Subtract(a.X, b.X);
			long dy = FixedPoint.Subtract(a.Y, b.Y);
			return FixedPoint.Sqrt(dx * dx + dy * dy);
		}
	}

	public class StateUpdateSystem : ISystem
	{
		public void Initialize() { }

		public void Update(Entity[] entities, List<PlayerAction> actions)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				var entity = entities[i];
				if (!entity.IsAlive) continue;

				for (int j = 0; j < actions.Count; j++)
				{
					var action = actions[j];
					if (action.Type == ActionType.Move && action.EntityId == entity.Id)
					{
						entity.Position2D = action.Target;
						break;
					}
				}
			}
		}
	}
}

