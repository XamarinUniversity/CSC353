using MusicSearch.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Concurrent;
using XamarinUniversity.Infrastructure;
using System.Threading.Tasks;
using XamarinUniversity.Interfaces;

namespace MusicSearch.ViewModels
{
    public class MainViewModel : SimpleViewModel
    {
        private CancellationTokenSource cancellationTokenSource;

        public IList<SearchCriteriaViewModel> SearchCriteria { get; private set; }
        IList<TrackViewModel> results;
        public IList<TrackViewModel> Results {
            get { return results; }
            private set { SetPropertyValue(ref results, value); }
        }

        string resultsTime;
        public string ResultsTime {
            get { return resultsTime; }
            private set { SetPropertyValue(ref resultsTime, value); }
        }

        int numberOfConsumers = 1;
        private IDependencyService serviceLocator;

        public int NumberOfConsumers {
            get { return numberOfConsumers; }
            set { SetPropertyValue(ref numberOfConsumers, value);}
        }

        public IAsyncDelegateCommand Search { get; private set; }
        public IDelegateCommand CancelSearch { get; private set; }
        public IAsyncDelegateCommand<SearchCriteriaViewModel> EditSearchCriteria { get; private set; }
        public IAsyncDelegateCommand AddSearchCriteria { get; private set; }

        public MainViewModel(IDependencyService serviceLocator)
        {
            this.serviceLocator = serviceLocator;

            SearchCriteria = new ObservableCollection<SearchCriteriaViewModel>() {
                new SearchCriteriaViewModel {
                    Field = "Year",
                    Operator = ComparisonOperation.GreaterEqual,
                    Value = "2000"
                }
            };

            Search = new AsyncDelegateCommand(OnSearch, () => SearchCriteria.Count > 0);
            EditSearchCriteria = new AsyncDelegateCommand<SearchCriteriaViewModel>(OnEditSearchCriteria);
            CancelSearch = new DelegateCommand(OnCancelSearch, () => cancellationTokenSource != null);
            AddSearchCriteria = new AsyncDelegateCommand(OnAddSearchCriteria);

            MessagingCenter.Subscribe<SearchCriteriaViewModel>(this, "Delete", async scvm => {
                SearchCriteria.Remove(scvm) ;
                if (SearchCriteria.Count == 0)
                    Search.RaiseCanExecuteChanged();
                await serviceLocator.Get<INavigationService>().GoBackAsync();
            });
        }

        private Task OnAddSearchCriteria()
        {
            var sc = new SearchCriteriaViewModel();
            SearchCriteria.Add(sc);
            if (SearchCriteria.Count == 1)
                Search.RaiseCanExecuteChanged();

            return OnEditSearchCriteria(sc);
        }

        private async Task OnEditSearchCriteria(SearchCriteriaViewModel item)
        {
            await serviceLocator.Get<INavigationService>()
               .NavigateAsync(AppPages.EditSearch, item);
        }

        async Task OnSearch()
        {
            OnCancelSearch();

            Results = null;
            ResultsTime = "Running..";
            cancellationTokenSource = new CancellationTokenSource();
            CancelSearch.RaiseCanExecuteChanged();

            serviceLocator.Get<INavigationService>()
                    .NavigateAsync(AppPages.Results, this).IgnoreResult();

            ConcurrentBag<TrackViewModel> foundTracks = new ConcurrentBag<TrackViewModel>();

            int numConsumers = NumberOfConsumers;
            MusicProcessor processor = new MusicProcessor(numConsumers, 
                                           t => SearchCriteria.All(sc => sc.Test(t)),
                                           t => foundTracks.Add(new TrackViewModel(t)));

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                await processor.Start(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                sw.Stop();

                ResultsTime = string.Format("{0} consumer took {1} for {2} records.",
                    numConsumers, sw.Elapsed, foundTracks.Count);
                Results = foundTracks.Distinct().OrderBy(t => t.Index).ToList();
                cancellationTokenSource = null;
                CancelSearch.RaiseCanExecuteChanged();
            }
        }

        void OnCancelSearch()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
        }

    }
}

