using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldSimCS
{
    public class Sim
    {
        private readonly int _maxPlayers;
        private readonly Dictionary<int, PlayerState> _players;
        private readonly List<ISystem> _systems;
        private readonly Queue<PlayerInputFrame> _pendingInputs;
        private readonly Dictionary<int, PlayerInputFrame> _collectedInputs;
        private int _currentTick;
        private readonly object _lock = new object();

        public Sim(int maxPlayers)
        {
            _maxPlayers = maxPlayers;
            _players = new Dictionary<int, PlayerState>();
            _systems = new List<ISystem>();
            _pendingInputs = new Queue<PlayerInputFrame>();
            _collectedInputs = new Dictionary<int, PlayerInputFrame>();
            _currentTick = 0;

            for (int i = 0; i < maxPlayers; i++)
            {
				_players[i] = new PlayerState((PlayerId)i);
            }

            RegisterSystem(new MovementSystem());
            RegisterSystem(new CombatSystem());
			RegisterSystem(new InventorySystem());
			RegisterSystem(new TradeSystem());
			RegisterSystem(new QuestSystem());
			RegisterSystem(new DialogueSystem());
			RegisterSystem(new LevellingSystem());
            RegisterSystem(new StateUpdateSystem());
        }

        public void RegisterSystem(ISystem system)
        {
            _systems.Add(system);
            system.Initialize();
        }

        public void SubmitInput(PlayerInputFrame frame)
        {
            lock (_lock)
            {
                _pendingInputs.Enqueue(frame);
            }
        }

        public void ProcessTick()
        {
            lock (_lock)
            {
                var actions = CollectPlayerActions(_currentTick);
                ExecuteSystems(actions);
                _currentTick++;
            }
        }

        private List<PlayerAction> CollectPlayerActions(int tick)
        {
            _collectedInputs.Clear();

            while (_pendingInputs.Count > 0 && _pendingInputs.Peek().Tick == tick)
            {
                var frame = _pendingInputs.Dequeue();
                foreach (var action in frame.Actions)
                {
                    _collectedInputs[action.PlayerId.GetHashCode()] = frame;
                }
            }

            var allActions = new List<PlayerAction>(_maxPlayers * 8);

			for (int playerId = 0; playerId < _maxPlayers; playerId++)
            {
                int hash = playerId;

				PlayerInputFrame inputFrame;
                if (_collectedInputs.TryGetValue(hash, out inputFrame))
                {
                    foreach (var action in inputFrame.Actions)
                    {
                        allActions.Add(action);
                    }
                }
                else
                {
                    var blankAction = new PlayerAction
                    {
                        PlayerId = (PlayerId)playerId,
                        Type = ActionType.Wait,
                        Target = Vector2.Zero,
                        EntityId = -1
                    };
                    allActions.Add(blankAction);
                }
            }

            return allActions;
        }

		private void ExecuteSystems(List<PlayerAction> actions)
		{
			var entityManager = EntityManager.Instance;

			var allEntities = entityManager.GetEntitiesSortedById();

			for (int sysIdx = 0; sysIdx < _systems.Count; sysIdx++)
			{
				var system = _systems[sysIdx];
				system.Update(allEntities, actions);
			}
		}			

        public int CurrentTick => _currentTick;

        public void AddEntity(Entity entity)
        {
            EntityManager.Instance.AddEntity(entity);
        }

        public void RemoveEntity(int entityId)
        {
            EntityManager.Instance.RemoveEntity(entityId);
        }
    }

    public static class Game
    {
        public static Sim CreateSim(int maxPlayers, int entityCount)
        {
			Sim loopManager = new Sim(maxPlayers);

            for (int i = 0; i < entityCount; i++)
            {
				var entity = new Entity(i + 1, EntityType.NPC);
                entity.Position2D = new Vector2(
                    FixedPoint.FromInt(i % 10),
                    FixedPoint.FromInt(i / 10)
                );
                entity.Velocity2D = Vector2.Zero;
                entity.Health = 100;
                loopManager.AddEntity(entity);
            }

            return loopManager;
        }
    }
}