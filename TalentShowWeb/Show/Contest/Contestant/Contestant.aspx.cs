﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TalentShow;
using TalentShow.Services;
using TalentShowDataStorage;
using TalentShowWeb.CustomControls.Models;
using TalentShowWeb.CustomControls.Renderers;
using TalentShowWeb.Utils;

namespace TalentShowWeb.Show.Contest.Contestant
{
    public partial class Contestant : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contestantId = Convert.ToInt32(Request.QueryString["contestantId"]);
            var contestant = ServiceFactory.ContestantService.Get(contestantId);
            var performers = ServiceFactory.PerformerService.GetContestantPerformers(contestant.Id);

            labelPageTitle.Text = "Contestant: " + GetContestantHeadingText(contestant);
            labelPageDescription.Text = GetContestantDescriptionText(contestant);

            var performerItems = new List<HyperlinkListPanelItem>();

            foreach (var performer in performers)
            {
                var url = "~";
                var heading = GetPerformerHeadingText(performer);
                var text = GetPerformerDescriptionText(performer);

                performerItems.Add(new HyperlinkListPanelItem(url, heading, text));
            }

            HyperlinkListPanelRenderer.Render(performersList, new HyperlinkListPanelConfig("Performers", performerItems, ButtonAddPerformerClick));
        }

        private string GetContestantHeadingText(TalentShow.Contestant contestant)
        {
            var performers = ServiceFactory.PerformerService.GetContestantPerformers(contestant.Id);

            bool isFirst = true;

            string text = "";

            foreach (var performer in performers)
            {
                text += (!isFirst ? ", " : "") + performer.Name.FirstName + " " + performer.Name.LastName;
                isFirst = false;
            }

            return text;
        }

        private string GetContestantDescriptionText(TalentShow.Contestant contestant)
        {
            return contestant.Performance.Description;
        }

        private string GetPerformerHeadingText(Performer performer)
        {
            return performer.Name.FirstName + " " + performer.Name.LastName;
        }

        private string GetPerformerDescriptionText(Performer performer)
        {
            return "Division: " + performer.Division.Name + " Affiliation: " + performer.Affiliation.Name;
        }

        protected void ButtonAddPerformerClick(object sender, EventArgs evnt)
        {
            Response.Redirect("~/About.aspx");
        }
    }
}