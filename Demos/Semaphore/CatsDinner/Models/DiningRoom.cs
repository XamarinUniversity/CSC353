using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;

namespace CatsDinner.Models
{
    public class DiningRoom
    {
        static readonly string[] Names = {
          "Garfield", "Sylvester", "Tom", "Muffin", "Bob"  
        };

        public List<Cat> Cats { get; private set; }
        public event Action<Cat,Cat,Fork> CatFightDetected = delegate {};

        public DiningRoom()
        {
            int count = Names.Length;

            Fork[] forks = new Fork[count];
            for (int n = 0; n < count; n++)
                forks[n] = new Fork(n, this);

            // TODO: Step 1 - add semaphore, pass to Cat.
            SemaphoreSlim bowlAvailable = new SemaphoreSlim(count / 2);

            Cats = Enumerable.Range(0, count)
			    .Select(n => new Cat(Names[n], forks[n], forks[(n + 1) % count], bowlAvailable))
                .ToList();
        }

        public void StartDinnerParty()
        {
            foreach (var c in Cats)
            {
                Task.Run(() => c.Run());
            }
        }

        public void RaiseCatfight(Cat currentOwner, Cat otherCat, Fork fork)
        {
            CatFightDetected(currentOwner, otherCat, fork);
        }
    }
}