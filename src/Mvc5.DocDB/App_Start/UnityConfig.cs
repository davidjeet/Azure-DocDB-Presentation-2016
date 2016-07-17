using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using Mvc5.DocDB.Infrastructure.Repository;
using Mvc5.DocDB.Infrastructure.Repository.DocumentDB;
using Mvc5.DocDB.Controllers;

namespace Mvc5.DocDB
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterType<ICandidateRepository, CandidateRepository>();
            container.RegisterType<AccountController>(new InjectionConstructor());
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}