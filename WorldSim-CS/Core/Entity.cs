using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldSimCS
{
	public enum EntityType
	{
		NPC,
		Beast,
		Vegetation,
		DynamicItem,
		Item
	}

	public class EntityManager
	{
		private static readonly EntityManager _instance = new EntityManager();
		public static EntityManager Instance => _instance;

		private readonly Dictionary<int, Entity> _entities;
		private readonly List<int> _entityIds;
		private readonly HashSet<int> _removedIds;
		private int _nextEntityId;

		private EntityManager()
		{
			_entities = new Dictionary<int, Entity>();
			_entityIds = new List<int>();
			_removedIds = new HashSet<int>();
			_nextEntityId = 1;
		}

		public int CreateEntity(EntityType _type)
		{
			int id = _nextEntityId++;
			var entity = new Entity(id, _type);
			_entities[id] = entity;
			_entityIds.Add(id);
			return id;
		}

		public void AddEntity(Entity entity)
		{
			if (!_entities.ContainsKey(entity.Id))
			{
				_entities[entity.Id] = entity;
				_entityIds.Add(entity.Id);
			}
		}

		public void RemoveEntity(int entityId)
		{
			if (_entities.Remove(entityId))
			{
				_removedIds.Add(entityId);
			}
		}

		public Entity[] GetEntitiesSortedById()
		{
			var removedSet = _removedIds;
			var entities = new Entity[_entityIds.Count];
			int writeIdx = 0;
			for (int i = 0; i < _entityIds.Count; i++)
			{
				int id = _entityIds[i];
				if (!removedSet.Contains(id))
				{
					entities[writeIdx++] = _entities[id];
				}
			}
			Array.Resize(ref entities, writeIdx);
			Array.Sort(entities, (a, b) => a.Id.CompareTo(b.Id));
			return entities;
		}

		public Entity GetEntity(int id)
		{
			Entity entity;
			_entities.TryGetValue(id, out entity);
			return entity;
		}

		public void Clear()
		{
			_entities.Clear();
			_entityIds.Clear();
			_removedIds.Clear();
			_nextEntityId = 1;
		}
	}

	public class Entity
	{
		public int Id { get; }
		public EntityType Type { get; set; }
		public Vector2 Position2D { get; set; }
		public Vector3 Position { get; set; }
		public Vector2 Velocity2D { get; set; }
		public Vector3 Velocity { get; set; }
		public int Health { get; set; }
		public int OwnerPlayerId { get; set; }
		public bool IsAlive => Health > 0;

		public Entity(int id, EntityType type)
		{
			Id = id;
			Type = type;
			Position2D = Vector2.Zero;
			Velocity2D = Vector2.Zero;
			Health = 100;
			OwnerPlayerId = -1;
		}
	}
}

