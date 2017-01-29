﻿using TalentShow.Repos;

namespace TalentShow.CrossReferences
{
    public class ContestJudge : IIdentity
    {
        public int Id { get; private set; }
        public int ContestId { get; private set; }
        public int JudgeId { get; private set; }

        public ContestJudge(int id, int contestId, int judgeId)
        {
            Init(id, contestId, judgeId);
        }

        public ContestJudge(int contestId, int judgeId)
        {
            Init(0, contestId, judgeId);
        }

        private void Init(int id, int contestId, int judgeId)
        {
            Id = id;
            ContestId = contestId;
            JudgeId = judgeId;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
