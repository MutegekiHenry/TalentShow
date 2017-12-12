﻿<%@ Page Title="Judge Sheet Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JudgeSheetReport.aspx.cs" Inherits="TalentShowWeb.Show.JudgeSheetReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><small>Show:</small> <asp:Label runat="server" ID="labelPageTitle" /></h2>
    <p><asp:Label runat="server" ID="labelPageDescription" /></p>
    <hr />
    <%  foreach (var contest in contests)
        {
                foreach (var contestant in GetReportContestants(contest))
                { %>
                    <p style='overflow:hidden;page-break-before:always;'></p>
                    <img src='<%= Page.ResolveUrl("~/Images/JudgeSheetHeader.png") %>' />
                    <br />
                    <div class="row">
                        <div class="col-md-6">
                            <h3><small>Contest:</small> <% Response.Write(contest.Name); %></h3>
                        </div>
                        <div class="col-md-6">
                            <h3><small>Contestant:</small> <% Response.Write(contestant.Name); %></h3>
                        </div>
                    </div>    
                    <%  foreach (var scoreCard in contestant.ScoreCards)
                        { %>
                        <h3><small>Judge:</small> <% Response.Write(GetJudgeUserName(scoreCard.Judge.UserId)); %></h3>
                        <table class="table table-bordered table-condensed table-striped">
                            <thead>
                                <tr>
                                    <%  foreach (var scorableCriterion in scoreCard.ScorableCriteria)
                                        { %>
                                            <th>
                                                <%= scorableCriterion.ScoreCriterion.CriterionDescription %>
                                            </th>
                                    <%  } %>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <%  foreach (var scorableCriterion in scoreCard.ScorableCriteria)
                                        { %>
                                            <td>
                                                <% Response.Write(scorableCriterion.Score); %>
                                            </td>
                                    <%  } %>
                                </tr>
                                <tr>
                                    <%  foreach (var scorableCriterion in scoreCard.ScorableCriteria)
                                        { %>
                                            <td>
                                                <% Response.Write(String.IsNullOrWhiteSpace(scorableCriterion.Comment) ? "--" : scorableCriterion.Comment); %>
                                            </td>
                                    <%  } %>
                                </tr>
                            </tbody>     
                        </table>
                    <% } %>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="well">
                                <table style="width:98%;">
                                    <tr>
                                        <td class="pull-left">
                                            <h4>Allowed Duration:</h4>
                                        </td>
                                        <td class="pull-right">
                                            <h4><% Response.Write(contest.MaxDuration.Hours.ToString("00") + ":" + contest.MaxDuration.Minutes.ToString("00") + ":" + contest.MaxDuration.Seconds.ToString("00")); %></h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="pull-left">
                                            <h4>Actual Duration:</h4>
                                        </td>
                                        <td class="pull-right">
                                            <h4><% Response.Write(contestant.PerformanceDuration.Hours.ToString("00") + ":" + contestant.PerformanceDuration.Minutes.ToString("00") + ":" + contestant.PerformanceDuration.Seconds.ToString("00")); %></h4>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="well">
                                <table style="width:98%;">
                                    <tr>
                                        <td class="pull-left">
                                            <h4>Total Score:</h4>
                                        </td>
                                        <td class="pull-right">
                                            <h4><% Response.Write(contestant.TotalScore); %></h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="pull-left">
                                            <h4>Penalty Points:</h4>
                                        </td>
                                        <td class="pull-right">
                                            <h4><% Response.Write(contestant.PenaltyPoints); %></h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="pull-left">
                                            <h4>Lowest Score:</h4>
                                        </td>
                                        <td class="pull-right">
                                            <h4><% Response.Write(contestant.LowestScore); %></h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="pull-left">
                                            <h3>Final Score:</h3>
                                        </td>
                                        <td class="pull-right">
                                            <h3><% Response.Write(contestant.FinalScore); %></h3>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>       
                <br />
            <%  } %>
    <%  } %>
    <p style='overflow:hidden;page-break-before:always;'></p>
</asp:Content>