using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysProgProjekat_TreciDeo
{
    internal class YelpClient
    {
        private HttpClient client;
        private List<Review> test_reviews;
        public YelpClient(string APIKey)
        {
            test_reviews = new List<Review>();
            InitializeTestReviews();
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIKey);

        }

        public async Task<List<Business>> Businesses_search(string city, bool open_now)
        {
            var endpoint = $"https://api.yelp.com/v3/businesses/search?location={city}&categories=&open_now={open_now.ToString().ToLower()}&sort_by=best_match&limit=50";

            try
            {
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var searchResponse = JsonConvert.DeserializeObject<YelpBusinessResponse>(jsonResponse);
                    return searchResponse?.businesses ?? new List<Business>();
                }
                else
                {
                    Console.WriteLine($"Greška: {response.StatusCode}");
                    return new List<Business>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<Business>();
        }

        public async Task<List<Review>> Reviews_search(string business_id)
        {
            var endpoint = $"https://api.yelp.com/v3/businesses/{business_id}/reviews?limit=20&sort_by=yelp_sort";

            try
            {
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var searchResponse = JsonConvert.DeserializeObject<YelpReviewResponse>(jsonResponse);
                    return searchResponse?.reviews ?? new List<Review>();
                }
                else
                {
                    Console.WriteLine($"Greška: {response.StatusCode}");
                    return new List<Review>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<Review>();
        }

        public async Task<List<Review>> Test_reviews_search()
        {
            var random = new Random();

            var randomIndices = new HashSet<int>();
            while (randomIndices.Count < 10)
            {
                randomIndices.Add(random.Next(0,test_reviews.Count));
            }

            var randomReviews = randomIndices.Select(i => test_reviews[i]).ToList();

            return randomReviews;
        }

        public void InitializeTestReviews()
        {

            test_reviews.Add(new Review { Description = "Service was terrible and slow." });
            test_reviews.Add(new Review { Description = "Loved the atmosphere and the dessert!" });
            test_reviews.Add(new Review { Description = "Food was cold and tasteless." });
            test_reviews.Add(new Review { Description = "The staff was very friendly and helpful." });
            test_reviews.Add(new Review { Description = "I waited too long for my meal." });
            test_reviews.Add(new Review { Description = "Best dining experience I’ve had in years!" });
            test_reviews.Add(new Review { Description = "The drinks were overpriced and watered down." });
            test_reviews.Add(new Review { Description = "Highly recommend this restaurant!" });
            test_reviews.Add(new Review { Description = "The waiter was rude and inattentive." });
            test_reviews.Add(new Review { Description = "The portions were generous and tasty." });
            test_reviews.Add(new Review { Description = "Food arrived cold and undercooked." });
            test_reviews.Add(new Review { Description = "Everything was perfect from start to finish." });
            test_reviews.Add(new Review { Description = "I will never come back here again." });
            test_reviews.Add(new Review { Description = "Lovely place, very cozy and clean." });
            test_reviews.Add(new Review { Description = "The music was too loud and annoying." });
            test_reviews.Add(new Review { Description = "The pizza was fresh and hot." });
            test_reviews.Add(new Review { Description = "The burger was dry and overcooked." });
            test_reviews.Add(new Review { Description = "Amazing desserts, especially the chocolate cake." });
            test_reviews.Add(new Review { Description = "The coffee was burnt and bitter." });
            test_reviews.Add(new Review { Description = "Fast service and friendly staff." });
            test_reviews.Add(new Review { Description = "The chairs were uncomfortable and sticky." });
            test_reviews.Add(new Review { Description = "I had a wonderful evening, everything top-notch." });
            test_reviews.Add(new Review { Description = "The menu was limited and unappetizing." });
            test_reviews.Add(new Review { Description = "Excellent value for money." });
            test_reviews.Add(new Review { Description = "Terrible experience, left hungry and angry." });
            test_reviews.Add(new Review { Description = "The steak was juicy and perfectly cooked." });
            test_reviews.Add(new Review { Description = "The salad was wilted and tasteless." });
            test_reviews.Add(new Review { Description = "Great music, nice vibe, and tasty cocktails." });
            test_reviews.Add(new Review { Description = "I got food poisoning after eating here." });
            test_reviews.Add(new Review { Description = "The atmosphere was cozy and relaxing." });
            test_reviews.Add(new Review { Description = "The service was slow and unprofessional." });
            test_reviews.Add(new Review { Description = "The waiter was attentive and polite." });
            test_reviews.Add(new Review { Description = "The dessert was too sweet for my taste." });
            test_reviews.Add(new Review { Description = "Loved the appetizers and presentation." });
            test_reviews.Add(new Review { Description = "The soup was bland and watery." });
            test_reviews.Add(new Review { Description = "Perfect spot for a romantic dinner." });
            test_reviews.Add(new Review { Description = "Overpriced menu and small portions." });
            test_reviews.Add(new Review { Description = "Staff went above and beyond to help us." });
            test_reviews.Add(new Review { Description = "Food lacked flavor and was disappointing." });
            test_reviews.Add(new Review { Description = "The ambiance was delightful and cozy." });
            test_reviews.Add(new Review { Description = "Worst dining experience ever." });
            test_reviews.Add(new Review { Description = "Highly satisfied, will come back soon." });
            test_reviews.Add(new Review { Description = "The kitchen made several mistakes with our order." });

        }
    }
}
