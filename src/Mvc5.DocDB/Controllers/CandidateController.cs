using AutoMapper;
using Microsoft.AspNet.Identity;
using Mvc5.DocDB.Infrastructure.Alerts;
using Mvc5.DocDB.Infrastructure.Repository;
using Mvc5.DocDB.Infrastructure.Repository.DocumentDB;
using Mvc5.DocDB.Models.Dating;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc5.DocDB.Controllers
{
    [Authorize]
    public class CandidateController : BaseController
    {
        private ICandidateRepository repository;

        public CandidateController(ICandidateRepository _repository)
        {
            repository = _repository;
        }

        [HttpGet]
        public ActionResult Create()
        {
            return this.View(new ViewModel_CreateCandidateProfile());
        }

        [HttpPost]
        public async Task<ActionResult> Create(ViewModel_CreateCandidateProfile model)
        {
            var candidate = Mapper.Map<ViewModel_CreateCandidateProfile, Candidate>(model);

            // set id & email 
            candidate.Id = User.Identity.GetUserId();
            candidate.Email = User.Identity.GetUserName();
            candidate.Views = 0;
            candidate.Revisions = 0;
            candidate.Interests = model.Interests2.StringToList();

            // create documentasync
            var result = await repository.Create(candidate);
            if (result.StatusCode == System.Net.HttpStatusCode.Created) return RedirectToAction("Index", "Home")
                                                               .WithSuccess("User " + candidate.Email + " successfully added.");

            return this.View(model).WithError("User " + candidate.Email + " failed to add :(");
        }



        [HttpGet]
        public ActionResult Update()
        {
            var id = User.Identity.GetUserId();
            var candidate = repository.Get(id);
            var viewModel = Mapper.Map<Candidate, ViewModel_UpdateCandidateProfile>(candidate);
            viewModel.Interests2 = candidate.Interests.ListToString();
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(ViewModel_UpdateCandidateProfile model)
        {
            Candidate candidate = Mapper.Map<ViewModel_UpdateCandidateProfile, Candidate>(model);
            candidate.Interests = model.Interests2.StringToList();

            var result = await repository.Update(candidate);
            if (result.StatusCode == System.Net.HttpStatusCode.OK ||
                result.StatusCode == System.Net.HttpStatusCode.NoContent ) 
                return RedirectToAction("Index", "Home").WithSuccess("User " + candidate.Email + " successfully updated.");

            return this.View(model).WithError("User " + candidate.Email + " failed to update :(");
        }




        [HttpGet]
        public ActionResult Match()
        {
            return this.View(new ViewModel_MatchCandidate());
        }

        [HttpPost]
        public ActionResult Match(ViewModel_MatchCandidate model)
        {
            var id = User.Identity.GetUserId();
            var results = repository.Match(id, model.Species, model.Gender, model.CurrentLocation, model.PoliticalAffiliation);
            return this.View("MatchResults", results);
        }


        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            var candidate = repository.Get(id);

            var attachment = await repository.ReadAttachment(id);
            if (attachment != null) { 
                    candidate.ProfilePicUrl = Convert.ToBase64String(attachment);
            }

            ViewBag.Interests = candidate.Interests.ListToString();   // hehe, cheating a little here
            return this.View(candidate);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CreateCandidateUpdateTrigger(string id)
        {
            string filePath = Server.MapPath(Url.Content("~/Scripts/"));
            await repository.CreateCandidateTrigger(filePath);
            return RedirectToAction("Index", "Home").WithSuccess("Trigger successfully created!");
        }


        [HttpGet]
        public ActionResult Upload()
        {
            return View("Upload");
        }


        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase photo)
        {
            if (photo != null && photo.ContentLength > 0)
            {
                var supportedTypes = new[] { "jpg", "jpeg" };
                var fileExt = Path.GetExtension(photo.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg) are supported.");
                    return View();
                }

                // Read bytes from http input stream
                await repository.SaveAttachment(User.Identity.GetUserId(), photo.InputStream);

                return RedirectToAction("Index", "Home").WithSuccess("Profile Picture successfully created!");
            }

            return this.View().WithError("Attachment failed to add :(");
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

    }
}