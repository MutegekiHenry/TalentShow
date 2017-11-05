﻿<%@ Page Title="Contest" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contest.aspx.cs" Inherits="TalentShowWeb.Show.Contest.Contest" %>
<%@ Register TagPrefix="custom" TagName="HyperlinkListPanel" Src="~/CustomControls/HyperlinkListPanel.ascx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label runat="server" ID="labelPageTitle" /></h2>
    <p><asp:Label runat="server" ID="labelPageDescription" /></p>
    <div class="form-group">
        <asp:Button runat="server" ID="btnEdit" Text="Edit" OnClick="btnEdit_Click" CssClass="btn btn-sm btn-primary" />
        <asp:Button runat="server" ID="btnDelete" Text="Delete" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this contest?');" CssClass="btn btn-sm btn-danger" />
    </div>
    <hr />
    <custom:HyperlinkListPanel runat="server" ID="contestantsList" />
    <br />
    <custom:HyperlinkListPanel runat="server" ID="judgesList" />
    <br />
    <custom:HyperlinkListPanel runat="server" ID="scoreCriteriaList" />
</asp:Content>
