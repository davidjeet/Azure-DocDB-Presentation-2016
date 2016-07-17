using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5.DocDB.Models.Dating
{
    public class ViewModel_CreateCandidateProfile : Candidate
    {
        public HttpPostedFileBase ProfilePic { get; set; }

        public string Interests2 { get; set; }   
    }
}