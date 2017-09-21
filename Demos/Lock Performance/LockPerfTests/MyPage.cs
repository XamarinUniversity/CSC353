using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LockPerfTests
{
    public class MyPage : ContentPage
    {
        IList<object> data = new ObservableCollection<object>();

        public MyPage()
        {
            Padding = new Thickness(5, 20);

            Button button = new Button { Text = "Run Tests" };
            button.Clicked += OnStartTest;

            Content = new ListView {
                ItemsSource = data,
                Header = button,
                ItemTemplate = new DataTemplate(() => {
                    var cell = new TextCell();
                    cell.SetBinding(TextCell.TextProperty, "LockType");
                    cell.SetBinding(TextCell.DetailProperty, "Elapsed");
                    return cell;
                })
            };
        }

        void OnStartTest(object sender, EventArgs e)
        {
            Task.Run(() => 
                LockTestLogic.TestLocks(
                    (ts, exclusive, lockType) => {
                        object value = new {
                            Elapsed = ts.ToString(),
                            LockType = string.Format("{0}: {1}", lockType, exclusive ? "Exclusive" : "Shared"),
                        };
                        data.Add(value);
                    }));
        }
    }
}


