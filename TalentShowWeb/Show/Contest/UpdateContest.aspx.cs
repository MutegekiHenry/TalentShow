﻿using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TalentShowWeb.Account.Util;
using TalentShowWeb.Models;
using TalentShowWeb.Utils;

namespace TalentShowWeb.Show.Contest
{
    public partial class UpdateContest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectUtil.RedirectUnauthenticatedUserToLoginPage();
            RedirectUtil.RedirectNonAdminUserToHomePage();

            BreadCrumbUtil.DataBind(Page, new List<BreadCrumb>()
            {
                new BreadCrumb(NavUtil.GetHomePageUrl(), "Home"),
                new BreadCrumb(NavUtil.GetShowsPageUrl(), "Shows"),
                new BreadCrumb(NavUtil.GetShowPageUrl(GetShowId()), "Show"),
                new BreadCrumb(NavUtil.GetContestPageUrl(GetShowId(), GetContestId()), "Contest"),
                new BreadCrumb(NavUtil.GetUpdateContestPageUrl(GetShowId(), GetContestId()), "Update Contest", IsActive: true),
            });

            labelPageTitle.Text = "Update the Contest";
            labelPageDescription.Text = "Use the form below to update the contest.";

            contestForm.GetSubmitButton().Click += new EventHandler(btnUpdateContest_Click);
            contestForm.GetCancelButton().Click += new EventHandler(btnCancel_Click);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var contest = ServiceFactory.ContestService.Get(GetContestId());
            contestForm.GetContestNameTextBox().Text = contest.Name;
            contestForm.GetDescriptionTextBox().Text = contest.Description;
            contestForm.GetMaxDurationTextBox().Text = Convert.ToString(contest.MaxDuration.Minutes);

            var timeKeepersDropDownList = contestForm.GetTimeKeepersDropDownList();

            var accountUtil = new AccountUtil(Context);
            var users = accountUtil.GetAllUsers();

            foreach (var user in users)
                timeKeepersDropDownList.Items.Add(new ListItem(user.Email, user.Id));

            timeKeepersDropDownList.Items.FindByValue(contest.TimeKeeperId).Selected = true;

            if(!String.IsNullOrWhiteSpace(contest.Status))
                contestForm.GetStatusDropDownList().Items.FindByValue(contest.Status).Selected = true;
        }

        protected void btnUpdateContest_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                //TODO
                return;
            }

            var contestName = contestForm.GetContestNameTextBox().Text.Trim();
            var description = contestForm.GetDescriptionTextBox().Text.Trim();
            var timeKeeper = contestForm.GetTimeKeepersDropDownList().SelectedValue.Trim();
            var maxDuration = new TimeSpan(0, Convert.ToInt32(contestForm.GetMaxDurationTextBox().Text.Trim()), 0);
            var status = contestForm.GetStatusDropDownList().SelectedValue.Trim();
            var contest = new TalentShow.Contest(GetContestId(), contestName, description, timeKeeper, maxDuration, status);
            ServiceFactory.ContestService.Update(contest);
            GoToContestPage();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GoToContestPage();
        }

        private void GoToContestPage()
        {
            NavUtil.GoToContestPage(Response, GetShowId(), GetContestId());
        }

        private int GetShowId()
        {
            return Convert.ToInt32(Request.QueryString["showId"]);
        }

        private int GetContestId()
        {
            return Convert.ToInt32(Request.QueryString["contestId"]);
        }
    }
}