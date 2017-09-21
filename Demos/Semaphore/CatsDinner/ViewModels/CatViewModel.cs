using CatsDinner.Models;
using XamarinUniversity.Infrastructure;

namespace CatsDinner.ViewModels
{
    public class CatViewModel : SimpleViewModel
    {
        readonly Cat cat;
        int previousBites;
        string text;
        private Cat.State lastState;

        public Cat.State State
        {
            get { return lastState; }
            private set { SetPropertyValue(ref lastState, value); }
        }

        public string Text 
        { 
            get { return text; } 
            private set { SetPropertyValue(ref text, value); }
        }

        public CatViewModel(Cat cat )
        {
            this.cat = cat;
            Update();
        }

        public void Run()
        {
            cat.Run();
        }

        public void Update() {
            int delta = cat.Bites - previousBites;
            previousBites = cat.Bites;
            State = cat.CurrentState;
            Text = string.Format("{0} ({1}):{2}", cat.Name, State, delta);
        }
    }
}
