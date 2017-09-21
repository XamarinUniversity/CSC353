using MusicSearch.Models;
using System.Threading;

namespace MusicSearch.ViewModels
{
    public class TrackViewModel
    {
        static long count = 0;
        readonly Track track;

        public TrackViewModel(Track track)
        {
            this.track = track;
            Index = Interlocked.Increment(ref count);
            IsOddRow = Index % 2 != 0;
        }

        public long Index { get; private set; }
        public bool IsOddRow { get; private set; }
        public string Title { get { return track.Name; }}
        public string Details {
            get { return string.Format("{0} - {1} [{2}]", track.Artist, track.Album, track.Year); }
        }
    }
}

