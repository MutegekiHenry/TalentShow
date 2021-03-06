﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentShow.CrossReferences;
using TalentShow.Repos;

namespace TalentShow.Services
{
    public class ContestService
    {
        private readonly IRepo<Contest> ContestRepo;
        private readonly ICrossReferenceRepo<ShowContest> ShowContestRepo;

        public ContestService(IRepo<Contest> contestRepo, ICrossReferenceRepo<ShowContest> showContestRepo)
        {
            if (contestRepo == null)
                throw new ApplicationException("A ContestService cannot be constructed without a ContestRepo.");
            if (showContestRepo == null)
                throw new ApplicationException("A ContestService cannot be constructed without a ShowContestRepo.");

            ContestRepo = contestRepo;
            ShowContestRepo = showContestRepo;
        }

        public ICollection<Contest> GetShowContests(int showId)
        {
            return SortByName
            (
                ContestRepo.GetWhereParentForeignKeyIs(showId)
            );
        }

        public ICollection<Contest> GetAll()
        {
            return SortByName
            (
                ContestRepo.GetAll()
            );
        }

        public bool Exists(int id)
        {
            return ContestRepo.Exists(id);
        }

        public Contest Get(int id)
        {
            return ContestRepo.Get(id);
        }

        public void Add(Contest contest)
        {
            ContestRepo.Add(contest);
        }

        public void Update(Contest contest)
        {
            ContestRepo.Update(contest);
        }

        public void Delete(int id)
        {
            ContestRepo.Delete(id);
        }

        public void Delete(Contest contest)
        {
            ContestRepo.Delete(contest);
        }

        public void AddShowContest(int showId, Contest contest)
        {
            Add(contest);
            ShowContestRepo.Add(new ShowContest(showId, contest.Id));
        }

        public void DeleteAll()
        {
            ContestRepo.DeleteAll();
        }

        private ICollection<Contest> SortByName(ICollection<Contest> contests)
        {
            var results =
                contests
                    .Where
                    (
                        c =>
                            IsDouble
                            (
                                GetFirstWord(c.Name)
                            )
                    )
                    .OrderBy
                    (
                        c =>       
                            GetFirstNumber
                            (
                                GetFirstWord
                                (   
                                    c.Name
                                )
                            )       
                    )
                    .ThenBy
                    (
                        c =>
                            GetSecondNumber
                            (
                                GetFirstWord
                                (
                                    c.Name
                                )
                            )
                    )
                    .ToList();

            results
                .AddRange
                (
                    contests
                        .Where
                        (
                            c =>
                            {
                                return !IsDouble
                                (
                                    GetFirstWord(c.Name)
                                );
                            }
                        )
                        .OrderBy
                        (   c => 
                                c.Name
                        )
                        .ToList()
                );


            return results;
        }

        private bool IsInt(string value)
        {
            int num;
            return int.TryParse(value, out num);
        }

        private bool IsDouble(string value)
        {
            double num;
            return double.TryParse(value, out num);
        }

        private string GetFirstWord(string value)
        {
            try
            {
                return value.Split(' ')[0];
            }
            catch
            {
                return value;
            }
        }

        private int GetFirstNumber(string value)
        {
            try
            {
                return Convert.ToInt32(value.Split('.')[0]);
            }
            catch
            {
                return 0;
            }
        }

        private int GetSecondNumber(string value)
        {
            try
            {
                return Convert.ToInt32(value.Split('.')[1]);
            }
            catch
            {
                return 0;
            }
        }
    }
}
