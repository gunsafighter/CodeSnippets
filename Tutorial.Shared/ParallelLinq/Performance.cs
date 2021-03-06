﻿namespace Tutorial.ParallelLinq
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Xml.Linq;

#if NETFX
    using Microsoft.ConcurrencyVisualizer.Instrumentation;
#endif

    using static Tutorial.LinqToXml.Modeling;

    using static Functions;

    using Stopwatch = System.Diagnostics.Stopwatch;

    internal static partial class Performance
    {
        private static void OrderBy(int count, int run, Func<int, int> keySelector)
        {
            int[] source = EnumerableX.RandomInt32().Take(count).ToArray();
            Stopwatch stopwatch = Stopwatch.StartNew();
            Enumerable.Range(0, run).ForEach(_ =>
            {
                int[] sequential = source.OrderBy(keySelector).ToArray();
            });
            stopwatch.Stop();
            $"Sequential:{stopwatch.ElapsedMilliseconds}".WriteLine();

            stopwatch.Restart();
            Enumerable.Range(0, run).ForEach(_ =>
            {
                int[] parallel1 = source.AsParallel().OrderBy(keySelector).ToArray();
            });
            stopwatch.Stop();
            $"Parallel:{stopwatch.ElapsedMilliseconds}".WriteLine();
        }
    }

    internal static partial class Performance
    {
        internal static void OrderBy()
        {
            OrderBy(5, 10_000, value => value); // Sequential:11    Parallel:1422
            OrderBy(5_000, 100, value => value); // Sequential:114   Parallel:107
            OrderBy(500_000, 100, value => value); // Sequential:18210 Parallel:8204

            OrderBy(Environment.ProcessorCount, 10, value => value + Compute()); // Sequential:1605  Parallel:737
        }
    }

    internal static partial class Performance
    {
        private static void Download(string[] uris)
        {
            uris.Visualize(uri =>
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.GetByteArrayAsync(uri).Wait();
                }
            });

            uris.AsParallel()
                .WithDegreeOfParallelism(10)
                .Visualize(uri =>
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.GetByteArrayAsync(uri).Wait();
                    }
                });

            using (Markers.EnterSpan(-1, nameof(ParallelEnumerableX.ForceParallel)))
            {
                MarkerSeries markerSeries = Markers.CreateMarkerSeries(nameof(ParallelEnumerableX.ForceParallel));
                uris.ForceParallel(
                    uri =>
                    {
                        using (markerSeries.EnterSpan(Thread.CurrentThread.ManagedThreadId, uri))
                        using (HttpClient httpClient = new HttpClient())
                        {
                            httpClient.GetByteArrayAsync(uri).Wait();
                        }
                    },
                    10);
            }
        }

        internal static void DownloadSmallFiles()
        {
            string[] thumbnails = 
                LoadXDocument("https://www.flickr.com/services/feeds/photos_public.gne?id=64715861@N07&format=rss2")
                .Descendants((XNamespace)"http://search.yahoo.com/mrss/" + "thumbnail")
                .Attributes("url")
                .Select(uri => (string)uri)
                .ToArray();
            Download(thumbnails);
        }

        internal static void DownloadLargeFiles()
        {
            string[] contents = 
                LoadXDocument("https://www.flickr.com/services/feeds/photos_public.gne?id=64715861@N07&format=rss2")
                .Descendants((XNamespace)"http://search.yahoo.com/mrss/" + "content")
                .Attributes("url")
                .Select(uri => (string)uri)
                .ToArray();
            Download(contents);
        }

        internal static void ReadFiles()
        {
            string coreLibraryPath = typeof(object).GetTypeInfo().Assembly.Location;
            string coreLibraryDirectory = Path.GetDirectoryName(coreLibraryPath);
            string[] files = Directory.GetFiles(coreLibraryDirectory);

            files.Visualize(file => File.ReadAllBytes(file));
            files.AsParallel().Visualize(file => File.ReadAllBytes(file));
        }
    }
}
