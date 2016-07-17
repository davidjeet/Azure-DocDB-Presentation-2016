using AutoMapper;
using Mvc5.DocDB.Models.Dating;

namespace Mvc5.DocDB
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<ViewModel_CreateCandidateProfile, Candidate>();

            Mapper.CreateMap<ViewModel_UpdateCandidateProfile, Candidate>();
            Mapper.CreateMap<Candidate, ViewModel_UpdateCandidateProfile>();
        }
    }
}
