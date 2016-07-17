using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Mvc5.DocDB.Models.Dating
{
    public class Candidate
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicUrl { get; set; }

        [Display(Name = "Political Affiliation")]
        public string PoliticalAffiliation { get; set; }

        public string Species { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Current Location")]
        public string CurrentLocation { get; set; }

        //[UIHint("tinymce_david")]
        [Display(Name = "Short Bio")]
        [System.Web.Mvc.AllowHtml]
        [DataType(DataType.MultilineText)]
        public string ShortBio { get; set; }

        public AccountInfo AccountInformation { get; set; }

        public List<Message> Messages { get; set; }

        public List<string> Interests { get; set; }

        public int Views { get; set; }

        public int Revisions { get; set; }

    }

}