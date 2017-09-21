using System;
using System.Globalization;
using System.Diagnostics;

namespace MusicSearch.Models
{
    public class Track
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public int? Year { get; set; }
        public long? TotalTime { get; set; }
        public int? TrackNumber { get; set; }
        public string Composer { get; set; }
        public int? BitRate { get; set; }

        public Track(string csvData)
        {
            // name,artist,album,genre,year,total-time,track-number,composer,bit-rate
            string[] items = csvData.Split(',');

            try 
            {
                Name = items[0];
                Artist = items[1];
                Album = items[2];
                Genre = items[3];
                Year = string.IsNullOrEmpty(items[4]) ? (int?)null : int.Parse(items[4], NumberFormatInfo.InvariantInfo);
                TotalTime = string.IsNullOrEmpty(items[5]) ? (long?)null : long.Parse(items[5], NumberFormatInfo.InvariantInfo);
                TrackNumber = string.IsNullOrEmpty(items[6]) ? (int?)null : int.Parse(items[6], NumberFormatInfo.InvariantInfo);
                Composer = items[7];
                BitRate = string.IsNullOrEmpty(items[8]) ? (int?)null : int.Parse(items[8], NumberFormatInfo.InvariantInfo);
            }
            catch (FormatException)
            {
                Debug.WriteLine("Problem parsing: " + csvData);
            }
        }
    }
}
