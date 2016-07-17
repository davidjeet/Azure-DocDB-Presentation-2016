using Newtonsoft.Json;

namespace ConsoleApplication1
{
    public class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public Parent[] Parents { get; set; }

        public Child[] Children { get; set; }

        [JsonProperty(PropertyName = "isRegistered")]
        public bool IsRegistered { get; set; }

        public Address Address { get; set; }
    }

    public class Parent
    {
        public string FirstName { get; set; }

        public string FamilyName { get; set; }

    }

    public class Child
    {
        public string FirstName { get; set; }

        public string FamilyName { get; set; }
        
        public string Gender { get; set; }

        public int Grade { get; set; }

        public Pet[] Pets { get; set; }

    }

    public class Pet
    {
        public string GivenName { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }
}
