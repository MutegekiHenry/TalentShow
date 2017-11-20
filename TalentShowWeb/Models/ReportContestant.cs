﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalentShowWeb.Models
{
    public class ReportContestant
    {
        public string Name { get; private set; }
        public string PerformanceDescription { get; private set; }
        public TimeSpan PerformanceDuration { get; private set; }
        public double TotalScore { get; private set; }
        public double PenaltyPoints { get; private set; }
        public double FinalScore { get; private set; }
        public double NumberOfScoreCards { get; private set; }
        public double NumberOfJudges { get; private set; }

        public ReportContestant(string Name, string PerformanceDescription, TimeSpan PerformanceDuration, 
            double TotalScore, double PenaltyPoints, double FinalScore, double NumberOfScoreCards, double NumberOfJudges)
        {
            this.Name = Name;
            this.PerformanceDescription = PerformanceDescription;
            this.PerformanceDuration = PerformanceDuration;
            this.TotalScore = TotalScore;
            this.PenaltyPoints = PenaltyPoints;
            this.FinalScore = FinalScore;
            this.NumberOfScoreCards = NumberOfScoreCards;
            this.NumberOfJudges = NumberOfJudges;

        }
    }
}