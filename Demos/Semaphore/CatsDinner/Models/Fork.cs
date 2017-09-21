namespace CatsDinner.Models
{
    public class Fork
    {
        readonly DiningRoom room;
        Cat currentOwner;

        public int Id { get; private set; }

        public Fork(int id, DiningRoom room)
        {
            this.room = room;
            Id = id+1;
        }

        public bool GrabBy(Cat newOwner)
        {
			if (currentOwner != null) 
			{
				OnCatFightDetected (newOwner);
			}
            else
            {
                currentOwner = newOwner;
                return true;
            }
            return false;
        }

        public void Drop(Cat oldOwner)
        {
            if (currentOwner != oldOwner)
                OnCatFightDetected(oldOwner);
            currentOwner = null;
        }

        void OnCatFightDetected(Cat otherCat)
        {
            room.RaiseCatfight(currentOwner, otherCat, this);
        }
    }
}