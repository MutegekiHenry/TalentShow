﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TalentShow.Services;
using TalentShowDataStorage;
using TalentShowWeb.CustomControls.Models;
using TalentShowWeb.CustomControls.Renderers;
using TalentShowWeb.Utils;

namespace TalentShowWeb.Show
{
    public partial class Show : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var items = new List<HyperlinkListPanelItem>();
            var showId = GetShowId();
            var show = ServiceFactory.ShowService.Get(showId);

            labelPageTitle.Text = "Show: " + show.Name;
            labelPageDescription.Text = show.Description;

            var contests = ServiceFactory.ContestService.GetShowContests(showId);

            foreach (var contest in contests)
                items.Add(new HyperlinkListPanelItem(URL: NavUtil.GetContestPageUrl(showId, contest.Id), Heading: contest.Name, Text: contest.Description));

            HyperlinkListPanelRenderer.Render(contestsList, new HyperlinkListPanelConfig("Contests", items, ButtonAddContestClick));
        }

        protected void ButtonAddContestClick(object sender, EventArgs evnt)
        {
            NavUtil.GoToAddContestPage(Response, GetShowId());
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            NavUtil.GoToUpdateShowPage(Response, GetShowId());
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ServiceFactory.ShowService.Delete(GetShowId());
            NavUtil.GoToShowsPage(Response);
        }

        private int GetShowId()
        {
            return Convert.ToInt32(Request.QueryString["showId"]);
        }
    }
}