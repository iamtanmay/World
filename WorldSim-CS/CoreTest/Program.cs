using System;
using System.Collections.Generic;
using WorldSimCS;

namespace WorldSimCS.Tests
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("==================================================");
			Console.WriteLine("        WORLDCON-CS DETERMINISTIC SIM TEST        ");
			Console.WriteLine("==================================================");

			// 1. Initialize a simulation with 2 players and 3 entities
			int maxPlayers = 2;
			int entityCount = 3;
			Sim sim = Game.CreateSim(maxPlayers, entityCount);

			Console.WriteLine($"[Init] Simulation created for {maxPlayers} max players.");
			Console.WriteLine($"[Init] Total registered entities in world: {entityCount}");
			PrintWorldState(sim);

			// ==========================================
			// TICK 0: Both players submit valid inputs
			// ==========================================
			Console.WriteLine("\n--- Processing Tick 0 (Both players active) ---");

			var frameTick0 = new PlayerInputFrame
			{
				Tick = 0,
				Actions = new List<PlayerAction>
				{
					new PlayerAction { PlayerId = (PlayerId)0, Type = ActionType.Move, Target = new Vector2(FixedPoint.FromInt(5), FixedPoint.FromInt(5)), EntityId = 1 },
					new PlayerAction { PlayerId = (PlayerId)1, Type = ActionType.Attack, Target = Vector2.Zero, EntityId = 2 }
				}
			};

			sim.SubmitInput(frameTick0);
			sim.ProcessTick(); // Executes systems and advances clock
			PrintWorldState(sim);


			// ==========================================
			// TICK 1: Network Gating Test (Player 1 misses their turn)
			// ==========================================
			Console.WriteLine("\n--- Processing Tick 1 (Player 1 is missing, blank injection test) ---");

			var frameTick1 = new PlayerInputFrame
			{
				Tick = 1,
				Actions = new List<PlayerAction>
				{
					// Only Player 0 submits an action. Player 1 is completely absent.
					new PlayerAction { PlayerId = (PlayerId)0, Type = ActionType.Move, Target = new Vector2(FixedPoint.FromInt(10), FixedPoint.FromInt(10)), EntityId = 1 }
				}
			};

			sim.SubmitInput(frameTick1);
			sim.ProcessTick(); // Expect Player 1 to automatically fall back to 'Wait' type
			PrintWorldState(sim);

			Console.WriteLine("\n==================================================");
			Console.WriteLine("              TEST EXECUTION COMPLETE             ");
			Console.WriteLine("==================================================");
		}

		/// <summary>
		/// Reads directly from your static EntityManager registry to print 100% accurate ground-truth positions.
		/// </summary>
		private static void PrintWorldState(Sim sim)
		{
			var entities = EntityManager.Instance.GetEntitiesSortedById();
			// Use the verified CurrentTick property from your Sim instance
			Console.WriteLine($"[Ground Truth] Current World Sim Tick: {sim.CurrentTick}");

			foreach (var entity in entities)
			{
				Console.WriteLine($"  -> Entity ID {entity.Id} | Position: ({entity.Position.X}, {entity.Position.Y}) | Health: {entity.Health}");
			}
		}
	}
}

