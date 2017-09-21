using Xamarin.Forms;

namespace CatsDinner.Extensions
{
    /// <summary>
    /// OnIdiom for XAML which supports Desktop.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OnIdiom<T>
    {
        public T Phone { get; set; }
        public T Tablet { get; set; }
        public T Desktop { get; set; }


        public static implicit operator T(OnIdiom<T> onIdiom)
        {
            switch (Device.Idiom)
            {
                case TargetIdiom.Tablet:
                    return onIdiom.Tablet;
                case TargetIdiom.Desktop:
                    return onIdiom.Desktop;
                case TargetIdiom.Phone:
                default:
                    return onIdiom.Phone;
            }
        }
    }
}
