using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MusicSearch.Models;
using System.Reflection;
using System.Linq;

namespace MusicSearch.Data
{
    public class MusicProcessor
    {
        readonly int numConsumers;
        readonly Predicate<Track> predicate;
        readonly BlockingCollection<Track> collection = new BlockingCollection<Track>(new ConcurrentQueue<Track>());
        readonly Action<Track> matches;
        CancellationToken cancellationToken;

        public MusicProcessor(int numConsumers, Predicate<Track> predicate, Action<Track> matches)
        {
            this.numConsumers = numConsumers;
            this.predicate = predicate;
            this.matches = matches;
        }

        public Task Start(CancellationToken token)
        {
            cancellationToken = token;

            List<Task> allTasks = new List<Task> {
                Task.Run(new Action(GenerateTracks), cancellationToken)
            };
            allTasks.AddRange(Enumerable.Range(0, numConsumers).Select(n => Task.Run(new Action(ConsumeTracks), cancellationToken)));

            return Task.WhenAll(allTasks);
        }

        private void GenerateTracks()
        {
            try 
            {
                foreach (var item in ReadMusicTracks())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    collection.Add(item, cancellationToken);
                }
            }
            finally
            {
                collection.CompleteAdding();
            }
        }

        private void ConsumeTracks()
        {
            foreach (var item in collection.GetConsumingEnumerable(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (predicate(item))
                	matches(item);
            }
        }

        IEnumerable<Track> ReadMusicTracks()
        {
            // Read in from a resource file, but could be HTTP, etc.
            const string MusicDataResource = "MusicSearch.Data.music.csv";
            
            using (var musicData = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(MusicDataResource))
            using (StreamReader sr = new StreamReader(musicData))
            {
                sr.ReadLine();

                string line;
                while ((line = sr.ReadLine()) != null)
                    yield return new Track(line);
            }
        }
    }
}