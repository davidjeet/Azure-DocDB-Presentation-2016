using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5.DocDB.Models.Dating
{
    using System.ComponentModel.DataAnnotations;

    public class ViewModel_MatchCandidate
    {
        public string Species { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Current Location")]
        public string CurrentLocation { get; set; }

        public string PoliticalAffiliation { get; set; }
    }
}