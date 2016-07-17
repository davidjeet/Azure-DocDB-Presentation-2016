using System;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly string endpointUrl = ConfigurationManager.AppSettings.Get("endpointUrl");
        private static readonly string authorizationKey = ConfigurationManager.AppSettings.Get("authorizationKey");

        private static readonly string databaseId = "ScottsDemoDB";
        private static readonly string collectionId = "Families";

        static void Main(string[] args)
        {
            ScottGuDemo().Wait();
            Console.WriteLine("Finished! (press any key to end session)");
            Console.ReadKey();
        }

        private static async Task ScottGuDemo()
        {
            using (var client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
                var database = new Database { Id = databaseId };
                database = await client.CreateDatabaseAsync(database);

                var collection = new DocumentCollection { Id = collectionId };
                collection = await client.CreateDocumentCollectionAsync(database.SelfLink, collection);

                //DocumentDB supports strongly typed POCO objects and also dynamic objects
                dynamic sonmezFamily = GetFamily1();
                dynamic mariotaFamily = GetFamily2();

                //persist the documents in DocumentDB
                await client.CreateDocumentAsync(collection.SelfLink, sonmezFamily);
                await client.CreateDocumentAsync(collection.SelfLink, mariotaFamily);

                //very simple query returning the full JSON document matching a simple WHERE clause
                //A. Get all the pets in the Sonmez family
                var  query = client.CreateDocumentQuery(collection.SelfLink, "SELECT * FROM Families f WHERE f.id = 'SonmezFamily'");
                var family = query.AsEnumerable().FirstOrDefault();
                Console.WriteLine("The Sonmez family have the following pets:");

                foreach (var person in family.Children)
                {
                    foreach (var pet in person.Pets)
                    {
                        Console.WriteLine(person.FirstName + " has a pet: " + pet.GivenName);
                    }
                }


                //B. Select JUST the child record out of the Family record where the child's gender is male
                query = client.CreateDocumentQuery(collection.DocumentsLink, "SELECT c.FirstName, c.Grade FROM c IN Families.Children WHERE c.Gender='male'");
                var child = query.AsEnumerable().FirstOrDefault();

                Console.WriteLine("The Sonmez's have a son named {0} in grade {1} ", child.FirstName, child.Grade);

                //cleanup test database
                #if DEBUG
                await client.DeleteDatabaseAsync(database.SelfLink);
                #endif

            }

        }

        #region  Helpers for Fake Data

        static Family GetFamily1()
        {
            var SonmezFamily = new Family
            {
                Id = "SonmezFamily",
                Parents = new [] {
                    new Parent { FirstName = "Enrique", FamilyName = "Sonmez" },
                    new Parent { FirstName = "Mary Sue", FamilyName = "Sonmez" }
                    },
                 Children = new [] {
                        new Child
                        { 
                            FirstName = "John", 
                            Gender = "male", 
                            Grade = 5,  
                            Pets = new [] {
                                new Pet { GivenName = "Fluffy" } ,
                                new Pet { GivenName = "Toodles" } 
                            }
                        } 
                    },
                            Address = new Address { State = "WA", County = "King", City = "Seattle" },
                            IsRegistered = true
                        };

                        return SonmezFamily;
                    }


         static Family GetFamily2()
           {
            var MariotaFamily = new Family
            {
                Id = "MariotaFamily",
                Parents = new[] {
                    new Parent { FamilyName = "Mariota", FirstName = "Desmond" },
                    new Parent { FamilyName = "Mariota", FirstName = "Kahlan" }
            },
                Children = new [] {
                    new Child {
                        FamilyName = "Mariota", 
                        FirstName = "Isaac", 
                        Gender = "male", 
                        Grade = 8,
                        Pets= new Pet[] {
                            new Pet { GivenName= "Pikachu" },
                            new Pet { GivenName= "Shadow" }
                        }
                    },
                    new Child {
                        FamilyName = "Mariota", 
                        FirstName = "Marcus", 
                        Gender = "male", 
                        Grade = 1
                    }
                },
                    Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                    IsRegistered = false
            };

            return MariotaFamily;
        }

        #endregion
    }
}
