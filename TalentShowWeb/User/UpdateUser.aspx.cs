﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TalentShowWeb.Account.Util;
using TalentShowWeb.Models;
using TalentShowWeb.Utils;

namespace TalentShowWeb.User
{
    public partial class UpdateUser : System.Web.UI.Page
    {
        private AccountUtil accountUtil;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectUtil.RedirectUnauthenticatedUserToLoginPage();
            RedirectUtil.RedirectNonAdminUserToHomePage();

            BreadCrumbUtil.DataBind(Page, new List<BreadCrumb>()
            {
                new BreadCrumb(NavUtil.GetHomePageUrl(), "Home"),
                new BreadCrumb(NavUtil.GetUsersPageUrl(), "Users"),
                new BreadCrumb(NavUtil.GetUpdateUserPageUrl(GetUserId()), "Update User", IsActive: true),
            });

            this.accountUtil = new AccountUtil(Context);

            labelPageTitle.Text = "Update the User";
            labelPageDescription.Text = "Use the form below to update the user.";

            userForm.GetSubmitButton().Click += new EventHandler(btnUpdateUser_Click);
            userForm.GetCancelButton().Click += new EventHandler(btnCancel_Click);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {        
            var user = accountUtil.GetUser(GetUserId());
            userForm.GetUserNameLabel().Text = user.UserName;
            userForm.GetEmailTextBox().Text = user.Email;
            userForm.GetIsAdminCheckBox().Checked = accountUtil.IsUserAnAdmin(user.Id);
            userForm.GetIsSuperuserCheckBox().Checked = accountUtil.IsUserASuperuser(user.Id);

            if (!accountUtil.IsUserASuperuser())
                userForm.GetIsSuperuserCheckBox().Enabled = false;
        }

        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                //TODO
                return;
            }

            var userId = GetUserId();
            accountUtil.SetEmail(userId, userForm.GetEmailTextBox().Text.Trim());

            if (userForm.GetIsAdminCheckBox().Checked)
                accountUtil.AddToAdminRole(userId);
            else
                accountUtil.RemoveFromAdminRole(userId);

            if (userForm.GetIsSuperuserCheckBox().Checked)
                accountUtil.AddToSuperuserRole(userId);
            else
                accountUtil.RemoveFromSuperuserRole(userId);

            GoToUsersPage();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GoToUsersPage();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            accountUtil.DeleteUser(GetUserId());
            GoToUsersPage();
        }

        private void GoToUsersPage()
        {
            NavUtil.GoToUsersPage(Response);
        }

        private string GetUserId()
        {
            return Convert.ToString(Request.QueryString["userId"]);
        }
    }
}