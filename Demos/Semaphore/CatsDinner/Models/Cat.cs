using System;
using System.Threading.Tasks;
using System.Threading;

namespace CatsDinner.Models
{
	public class Cat 
	{
		static readonly Random Rnd = new Random();
		readonly Fork firstFork;
		readonly Fork secondFork;

		// TODO: Step 2: add semaphore
		//SemaphoreSlim bowlAvailable;

		public enum State 
		{ 
			Sleeping, 
			Hungry, 
			Eating 
		};

		public string Name { get; private set; }
		public int Bites { get; private set; }
		public State CurrentState { get; private set; }

		// TODO: Step 2: add semaphore
		public Cat(string name, Fork left, Fork right, SemaphoreSlim bowlAvailable)
		{
			//this.bowlAvailable = bowlAvailable;
			Name = name;
			CurrentState = State.Sleeping;
			firstFork = (left.Id < right.Id) ? left : right;
			secondFork = (left.Id < right.Id) ? right : left;
		}

		public void Run()
		{
			while (true) 
			{
				TakeNap();
				Eat();
			}
		}

		void TakeNap()
		{
			CurrentState = State.Sleeping;
			// TODO: can remove delay so there's a lot of contention.
			// This will have more impact on multi-core machines.
			Task.Delay(Rnd.Next(0, 1)).Wait();
			CurrentState = State.Hungry;
		}

		void Eat()
		{
			// TODO: Step 3 - wait on semaphore
			// This will ensure that only half the cats grab for the forks
			// which cuts down the contention and will balance out the 
			// bites across the cats. On single-core machines, this will also
			// vastly improve the concurrency allowed because threads are being blocked
			// more efficiently here.
			//bowlAvailable.Wait();

			lock (firstFork)
			{
				if (firstFork.GrabBy(this))
				{
					lock (secondFork)
					{
						// TODO: Step 3 - release the semaphore once we
						// have both locks.
						//bowlAvailable.Release();

						if (secondFork.GrabBy(this))
						{
							TakeABite ();
							secondFork.Drop(this);
						}
					}
					firstFork.Drop(this);
				}
			}
		}

		void TakeABite ()
		{
			CurrentState = State.Eating;
			if (Rnd.Next(10) == 5)
				Task.Delay(1).Wait();
			Bites++;
		}
	}
}
