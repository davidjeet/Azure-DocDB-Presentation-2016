using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Mvc5.DocDB.Models.Dating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLoader.DocDB
{
    using System.Diagnostics;

    class Program
    {
        private static string endpointUrl = @"https://docujeet.documents.azure.com:443/";
        private static string authorizationKey = @"8xvwgeqFb8iRBPtx0oK2MeMmhv9CZZrE94RuIKc54C0yrcCmcHA7IxJ1z+pkCj52dUE0FtKL5UKSGNMkmXvVwg==";

        private static DocumentClient client;
        private static Database database;
        private static DocumentCollection collection;

        private static readonly string DATABASEID = "CodeCampDemo";
        private static readonly string COLLECTIONID = "candidates";


        private static readonly int NUM_CANDIDATES = 50;

        private static readonly string[] politicalAffiliation = { "", "The Rebellion", "Empire", "Green Party", "Independent" };
        private static readonly string[] species = { "", "Human", "Alien", "Droid", "Other" };
        private static readonly string[] locations = { "Death Star", "Planet: Hoth", "Jar-Jar land", "Moon: Endor", "Planet: New Alderaan", "Planet: Dagobah", "Cloud City", "Planet: Tatooine", "Planet: Coruscant" };

        private static readonly string[] interests =
            {
                "playing baachi with a wookie", "shooting womprats wth a T-16", "sunset walks", "fencing", "documentdb", "eating", "smoking", "gambling",
                "dev ops", "programming", "judo", "tennis", "badminton", "skiiing", "water sports", "fantasy football", "running", "collecting stamps",
                "dancing for Jabba", "playing xbox", "reading fiction", "lifting weights", "gardening", "sleeping"
            };
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            DoWork().Wait();
            sw.Stop();
            Console.WriteLine("The whole thing took {0} minutes", sw.Elapsed.TotalMinutes);
        }

        private static async Task DoWork()
        {
            using (var ctx = new AdventureWorksLT2012_DataEntities1())
            {
                //0. set up document db
                // 1 . Read using EF 6
                List<Customer> customers;
                try
                {
                    customers = ctx.Customers.OrderBy(x => x.CustomerID).Take(NUM_CANDIDATES).ToList();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }

                using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
                {
                    database = new Database { Id = DATABASEID };
                    database = await client.CreateDatabaseAsync(database);

                    collection = new DocumentCollection { Id = COLLECTIONID };
                    collection = await client.CreateDocumentCollectionAsync(database.SelfLink, collection);


                    var count = 0;
                    foreach (var customer in customers)
                    {
                        // 2. Create the randomized candidate
                        var candidate = CreateCandidate(customer);

                        // 3. Insert into the database
                        await client.CreateDocumentAsync(collection.SelfLink, candidate);
                        count++;

                        Console.WriteLine(
                            "candidate {0} ({1} {2}) added.",
                            count + ":" + candidate.Id,
                            candidate.FirstName,
                            candidate.LastName);
                    }
                }// //dispose dobdb client
            } //dispose EF context
        }

        private static Candidate CreateCandidate(Customer customer)
        {
            return new Candidate
                           {
                               Id = customer.CustomerID.ToString(),
                               FirstName = customer.FirstName,
                               LastName = customer.LastName,
                               Email = customer.EmailAddress,
                               Gender = (customer.Title == "Mr.") ? "Male" : "Female",
                               PoliticalAffiliation = politicalAffiliation[RandomNumber(0, politicalAffiliation.Length - 1)],
                               Species = species[RandomNumber(0, species.Length - 1)],
                               Views = RandomNumber(0, 15000),
                               CurrentLocation = locations[RandomNumber(0, locations.Length - 1)],
                               AccountInformation = new AccountInfo() { Username = customer.EmailAddress, Password = customer.PasswordHash },
                               Interests = GetInterests(),
                               ShortBio = LoremIpsum(50, 200, 5, 10, 3),
                               ProfilePicUrl = null,
                               Messages = new List<Message>()
                           };

        }


        private static List<string> GetInterests()
        {
            var list = new HashSet<string>();
            var size = RandomNumber(1, 6);
            for (var i = 0; i < size; i++)
            {
                list.Add(interests[RandomNumber(0, interests.Length - 1)]);
            }
            return list.ToList();
        }

        static string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer","adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                               "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            int numSentences = RandomNumber(0, maxSentences - minSentences)
                + minSentences + 1;
            int numWords = RandomNumber(0, maxWords - minWords) + minWords + 1;

            string result = string.Empty;

            for (int p = 0; p < numParagraphs; p++)
            {
                result += "<p>";
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result += " "; }
                        result += words[RandomNumber(0, words.Length)];
                    }
                    result += ". ";
                }
                result += "</p>";
            }

            return result;
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }


}
