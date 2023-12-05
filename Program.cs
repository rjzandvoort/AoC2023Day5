using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day5
{
    internal class Program
    {
        private class SeedTo
        {
            public string Name;
            public long Id;
            public SeedTo Destination;
            public SeedTo Source;
            public List<long> Path = new List<long>();
            public Dictionary<long,long> PathD= new Dictionary<long,long>();
        }

        private class SeedB
        {
            public string Name;
            public long Id;
            public SeedTo Destination;
            public SeedTo Source;            
            public Dictionary<long, long> PathB = new Dictionary<long, long>();
        }
        static void Main(string[] args)
        {            
            Day5bNaive();
        }

        private class SeedRange
        {
            public long Start; 
            public long End;
        }

        private class MapLevel
        {
            public List<Mapper> mappers = new List<Mapper>();

            public long Map(long val)
            {
                long mval = val;

                foreach(var map in mappers)
                {
                    if(val >= map.Start && val <= map.End)
                    {
                        mval = val + map.Modifier;
                        return mval;
                    }
                }
                return mval;
            }

            public long BackMap(long val)
            {
                long mval = val;

                foreach (var map in mappers)
                {
                    if (val >= map.Start + map.Modifier && val <= map.End + map.Modifier)
                    {
                        mval = val - map.Modifier;
                        return mval;
                    }
                }
                return mval;
            }
        }

        private class Mapper
        {
            public string Name;
            public long Start;
            public long End;
            public long Modifier;

            public override string ToString()
            {
                return Name + " : " + Start + ", " + End + ", " + Modifier;
            }
        }

        private static Dictionary<int,MapLevel> mapperLevels = new Dictionary<int,MapLevel>();
        private static List<SeedRange> seedRanges = new List<SeedRange>();

        static void Day5bNaive()
        {
            var txt = File.ReadAllText("input.txt");
            txt = txt.Replace("\r", "");
            var seeds = new List<SeedB>();
            var mappers = new List<Mapper>();
            string curCat = "";
            int catId = 0;
            
            foreach (var line in txt.Split('\n'))
            {
                if (line.StartsWith("seeds:"))
                {
                    var sps = line.Replace("seeds: ", "").Split(' ');
                    for(int i=0;i< sps.Length; i += 2)
                    {
                        long startS = long.Parse(sps[i]);
                        long rangeS = long.Parse((sps[i + 1]));
                        var seedRange = new SeedRange();
                        seedRange.Start = startS;
                        seedRange.End = startS + rangeS - 1;
                        seedRanges.Add(seedRange);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        // New Category coming up
                        curCat = "";
                    }
                    else if (curCat == "" && line.Contains(":"))
                    {
                        // New category
                        curCat = line.Replace(" map:", "").Split('-')[2];
                        catId++;                        
                        mapperLevels.Add(catId, new MapLevel()) ;                        
                    }
                    else
                    {
                        // Numbers
                        var lps = line.Split(' ');
                        var from = long.Parse(lps[1]);
                        var to = long.Parse(lps[0]);
                        var range = long.Parse(lps[2]);
                        var curMapper = new Mapper();
                        curMapper.Start = from;
                        curMapper.End = from + range - 1;
                        curMapper.Modifier = to - from;

                        mapperLevels[catId].mappers.Add(curMapper);
                        
                    }
                }
            }

            foreach(var ml in mapperLevels.Values)
            {
                foreach(var mapper in ml.mappers)
                {
                    Console.WriteLine(mapper.ToString());
                }
                Console.WriteLine("----");
            }

            
            long lastMap = 1;
            bool found = false;
            var oMap = lastMap;

            // Brute force; start with location 1 and Map Back to see if there is a valid location 
            while (!found)
            {
                oMap = lastMap;
                for (int i = mapperLevels.Count; i >= 1; i--)
                {
                    lastMap = mapperLevels[i].BackMap(lastMap);
                }
                foreach(var rng in seedRanges)
                {
                    if(lastMap >= rng.Start && lastMap <= rng.End)
                    {
                        // FOUND IT!
                        Console.WriteLine("Lowest Location with a seed : " + oMap);
                        found = true;
                        break;
                    }
                }
                lastMap = oMap + 1;
            }
            Console.ReadLine();
        }

        static void Day5a()
        {
            var txt = File.ReadAllText("input.txt");
            txt = txt.Replace("\r", "");
            var seeds = new List<SeedTo>();
            
            string curCat = "";
            int catId = 0;
            foreach (var line in txt.Split('\n'))
            {
                if (line.StartsWith("seeds:"))
                {
                    var sps = line.Replace("seeds: ", "").Split(' ');
                    foreach (var s in sps)
                    {
                        var seed = new SeedTo() { Id = long.Parse(s), Name = "Seed" };
                        seed.Path.Add(seed.Id);
                        seeds.Add(seed);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        // New Category coming up
                        curCat = "";
                    }
                    else if (curCat == "" && line.Contains(":"))
                    {
                        // New category
                        curCat = line.Replace(" map:", "").Split('-')[2];
                        catId++;
                        foreach (var s in seeds)
                        {
                            s.Path.Add(s.Path[catId - 1]);
                        }
                    }
                    else
                    {
                        // Numbers
                        var lps = line.Split(' ');
                        var from = long.Parse(lps[1]);
                        var to = long.Parse(lps[0]);
                        var range = long.Parse(lps[2]);
                        foreach (var s in seeds)
                        {
                            var snr = s.Path[catId - 1];
                            if (snr >= from && snr < from + range)
                            {
                                // Valid dest 
                                s.Path[catId] = (snr - from) + to;
                            }
                        }
                    }
                }
            }
            var loc = seeds.OrderBy(s => s.Path.Last()).First();
            Console.WriteLine("Lowest location " + loc.Path.Last());
            Console.ReadLine();
        }
    }
}
