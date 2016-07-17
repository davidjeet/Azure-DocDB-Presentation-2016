using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Mvc5.DocDB.Models.Dating
{
    using System.ComponentModel.DataAnnotations;

    public class Message
    {
        public string FromCandidateId { get; set; }

        public string FromCandidateName { get; set; }

        public DateTime SentOn { get; set; }

        //[UIHint("tinymce_david")]
        [System.Web.Mvc.AllowHtml]
        [Required(ErrorMessage = "Cannot send an empty message")]
        public string Text { get; set; }

    }


}