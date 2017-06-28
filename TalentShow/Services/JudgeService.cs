﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentShow.CrossReferences;
using TalentShow.Repos;

namespace TalentShow.Services
{
    public class JudgeService
    {
        private readonly IRepo<Judge> JudgeRepo;
        private readonly IRepo<PersonName> PersonNameRepo;
        private readonly IRepo<Organization> OrganizationRepo;
        private readonly ICrossReferenceRepo<ContestJudge> ContestJudgeRepo;

        public JudgeService(IRepo<Judge> judgeRepo, IRepo<PersonName> personNameRepo, IRepo<Organization> organizationRepo, ICrossReferenceRepo<ContestJudge> contestJudgeRepo)
        {
            if (judgeRepo == null)
                throw new ApplicationException("A JudgeService cannot be constructed without a JudgeRepo.");
            if (personNameRepo == null)
                throw new ApplicationException("A JudgeService cannot be constructed without a PersonNameRepo.");
            if (organizationRepo == null)
                throw new ApplicationException("A JudgeService cannot be constructed without an OrganizationRepo.");
            if (contestJudgeRepo == null)
                throw new ApplicationException("A JudgeService cannot be constructed without an ContestJudgeRepo.");

            JudgeRepo = judgeRepo;
            PersonNameRepo = personNameRepo;
            OrganizationRepo = organizationRepo;
            ContestJudgeRepo = contestJudgeRepo;
        }

        public ICollection<Judge> GetContestJudges(int contestId)
        {
            var contestJudgeCollection = ContestJudgeRepo.GetMatchingOn(contestId);
            var judges = new List<Judge>();

            foreach (var cj in contestJudgeCollection)
            {
                if (JudgeRepo.Exists(cj.JudgeId))
                    judges.Add(JudgeRepo.Get(cj.JudgeId));
            }

            return judges;
        }

        public void AddContestJudge(int contestId, Judge judge)
        {
            Add(judge);
            ContestJudgeRepo.Add(new ContestJudge(contestId, judge.Id));
        }

        public ICollection<Judge> GetAll()
        {
            return JudgeRepo.GetAll();
        }

        public bool Exists(int id)
        {
            return JudgeRepo.Exists(id);
        }

        public Judge Get(int id)
        {
            return JudgeRepo.Get(id);
        }

        public void Add(Judge judge)
        {
            AddNameAndAffiliation(judge);
            JudgeRepo.Add(judge);
        }

        public void Update(Judge judge)
        {
            AddNameAndAffiliation(judge);
            UpdateNameAndAffiliation(judge);
            JudgeRepo.Update(judge);
        }

        public void Delete(int id)
        {
            JudgeRepo.Delete(id);
        }

        public void Delete(Judge judge)
        {
            JudgeRepo.Delete(judge);
        }

        public void DeleteAll()
        {
            JudgeRepo.DeleteAll();
        }

        private void AddNameAndAffiliation(Judge judge)
        {
            if (judge.Name != null && !PersonNameRepo.Exists(judge.Name.Id))
                PersonNameRepo.Add(judge.Name);
            if (judge.Affiliation != null && !OrganizationRepo.Exists(judge.Affiliation.Id))
                OrganizationRepo.Add(judge.Affiliation);
        }

        private void UpdateNameAndAffiliation(Judge judge)
        {
            PersonNameRepo.Update(judge.Name);
            OrganizationRepo.Update(judge.Affiliation);
        }
    }
}
