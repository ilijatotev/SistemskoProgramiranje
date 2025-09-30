using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SysProgProjekat_TreciDeo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string APIKey = "Lp1I3MkxI-WRBHRRzZFX5ZaNsrCiKmmQ1x95yDgIEodLQuGzfQiSgB3IB0Zlcx_lWtpcjP3ZoNIVuLQ1Xb5l65JNclJfGjU0a5cuT-f3_jj4_N7uOZsUlZHoo0rNaHYx";
            YelpClient yclient = new YelpClient(APIKey);
            string city; string isopen; bool open;

            Console.WriteLine("Unesite grad:");
            city = Console.ReadLine();
            Console.WriteLine("Otvoren restoran? [Y/N]");
            isopen = Console.ReadLine();
            open = false;
            if (isopen == "Y" || isopen == "y")
                open = true;

            List<Business> bussines_list = await yclient.Businesses_search(city, open);

            var reviewService = new ReviewService();

            bussines_list.ToObservable()
                .Select(business =>
                    Observable.FromAsync(async () =>
                    {
                        var reviews = await yclient.Test_reviews_search();
                        var scores = reviews.Select(r => reviewService.AnalyzeReview(r.Description).Prediction ? 1 : 0);
                        var averageScore = scores.Any() ? scores.Average() : 0;

                        return new
                        {
                            business = business,
                            AverageScore = averageScore
                        };
                    })
                    .SubscribeOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
                )
                .Merge()
                .Subscribe(result =>
                {
                    Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Restoran: {result.business.name}, Average Sentiment: {result.AverageScore:F2}");
                });

            Console.ReadLine();
        }
    }
}
