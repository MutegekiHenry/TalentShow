﻿<%@ Page Title="Contest" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contestant.aspx.cs" Inherits="TalentShowWeb.Show.Contest.Contestant.Contestant" %>
<%@ Register TagPrefix="custom" TagName="HyperlinkListPanel" Src="~/CustomControls/HyperlinkListPanel.ascx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label runat="server" ID="labelPageTitle" /></h2>
    <p><asp:Label runat="server" ID="labelPageDescription" /></p>
    <%  if(IsUserAnAdmin())
        { %>
            <div class="form-group">
                <asp:Button runat="server" ID="btnEdit" Text="Edit" OnClick="btnEdit_Click" CssClass="btn btn-sm btn-primary" />
                <asp:Button runat="server" ID="btnDelete" Text="Delete" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this contestant?');" CssClass="btn btn-sm btn-danger" />
            </div>
    <%  } %>
    <hr />
    <%  if (IsAllowedToViewStopWatch())
        { %>
            <div class="panel panel-default">
                <div class="panel-heading clearfix">
                    <h3 class="panel-title pull-left">Performance Duration</h3>
                </div>
                <div class="panel-body">
                    <div class="stopwatch"></div>
                </div>
            </div>
            <br />
    <%  }
        if(IsUserAnAdmin())
        { %>
            <div class="panel panel-default">
                <div class="panel-heading clearfix">
                    <% var scoreCards = GetScoreCards();  %>
                    <h3 class="panel-title pull-left">Score Cards <span class="badge"><% Response.Write(scoreCards.Count()); %></span></h3>
                </div>
                <div class="panel-body">

                    <% foreach (var scoreCard in scoreCards)
                        { %>
                            <div class="panel panel-warning">
                                <div class="panel-heading clearfix">
                                    <h3 class="panel-title pull-left">Score Card by <% Response.Write(GetJudgeEmailAddress(scoreCard.Judge.UserId)); %></h3>
                                </div>
                                <div class="panel-body">
                                    <% foreach (var scorableCriterion in scoreCard.ScorableCriteria)
                                        { %>
                                            <h4><% Response.Write(scorableCriterion.ScoreCriterion.CriterionDescription); %></h4>
                                            <p><b>Score</b>: <% Response.Write(scorableCriterion.Score); %></p>
                                            <% if(!String.IsNullOrWhiteSpace(scorableCriterion.Comment))
                                                { %>
                                                    <p><b>Comment</b>: <% Response.Write(scorableCriterion.Comment); %></p>
                                            <% } %>
                                            <hr />
                                    <% } %>
                                    <h3>Card Total: <% Response.Write(scoreCard.TotalScore); %></h3>
                                </div>
                            </div>
                            <br />
                     <% } %>
                    <div class="well">
                        <table style="width:30%;">
                            <tr>
                                <td class="pull-left">
                                    <h3>Total Score:</h3>
                                </td>
                                <td class="pull-right">
                                    <h3><% Response.Write(GetTotalScore()); %></h3>
                                </td>
                            </tr>
                            <tr>
                                <td class="pull-left">
                                    <h3>Penalty Points:</h3>
                                </td>
                                <td class="pull-right">
                                    <h3><% Response.Write(GetPenaltyPoints()); %></h3>
                                </td>
                            </tr>
                            <tr>
                                <td class="pull-left">
                                    <h2>Final Score:</h2>
                                </td>
                                <td class="pull-right">
                                    <h2><% Response.Write(GetFinalScore()); %></h2>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <br />
            <custom:HyperlinkListPanel runat="server" ID="performersList" />
    <%  }
        if(IsAllowedToViewStopWatch())
        { %>
            <script>
                var Stopwatch = function(elem, options) {

                  var timer       = createTimer(),
                      startButton = createButton("start", start, "primary"),
                      stopButton  = createButton("stop", stop, "danger"),
                      resetButton = createButton("reset", reset, "default"),
                      offset,
                      clock,
                      interval;

                  // default options
                  options = options || {};
                  options.delay = options.delay || 1;

                  // append elements     
                  elem.appendChild(timer);
                  elem.appendChild(document.createElement("br"));
                  elem.appendChild(startButton);
                  elem.appendChild(stopButton);
                  elem.appendChild(resetButton);

                  // initialize
                  initClock();

                  // private functions
                  function createTimer() {
                    var span = document.createElement("span");
                    span.setAttribute("style", "font-size: 24pt; font-weight: bold;");
                    return span;
                  }

                  function createButton(action, handler, color) {
                    var button = document.createElement("button");
                    button.innerHTML = action;
                    button.classList.add("btn");
                    button.classList.add("btn-" + color);
                    button.setAttribute("style", "margin-right: 5px;");
                    button.addEventListener("click", function(event) {
                      handler();
                      event.preventDefault();
                    });
                    return button;
                  }

                  function start() {
                    if (!interval) {
                      offset   = Date.now();
                      interval = setInterval(update, options.delay);
                    }
                  }

                  function stop() {
                    if (interval) {
                      clearInterval(interval);
                      interval = null;
                      SetPerformanceDuration(clock);  
                    }
                  }

                  function reset() {
                    clock = 0;
                    SetPerformanceDuration(clock); 
                    render();
                  }

                  function initClock() {
                    clock = <%= contestant.Performance.Duration.TotalMilliseconds %>;
                    render();
                  }

                  function update() {
                    clock += delta();
                    render();
                  }

                  function render() {
                    timer.innerHTML = msToHMS(clock); 
                  }

                  function delta() {
                    var now = Date.now(),
                        d   = now - offset;

                    offset = now;
                    return d;
                  }

                  // public API
                  this.start  = start;
                  this.stop   = stop;
                  this.reset  = reset;
                };

                var elems = document.getElementsByClassName("stopwatch");

                for (var i=0, len=elems.length; i<len; i++) {
                    new Stopwatch(elems[i], {delay: 10});
                }

                function msToHMS( ms ) {
                    // 1- Convert to seconds:
                    var seconds = ms / 1000;
                    // 2- Extract hours:
                    var hours = parseInt( seconds / 3600 ); // 3,600 seconds in 1 hour
                    seconds = seconds % 3600; // seconds remaining after extracting hours
                    // 3- Extract minutes:
                    var minutes = parseInt( seconds / 60 ); // 60 seconds in 1 minute
                    // 4- Keep only seconds not extracted to minutes:
                    seconds = seconds % 60;

                    return formatNumberToTwoDigits(hours) + ":" + formatNumberToTwoDigits(minutes) + ":" + formatNumberToTwoDigits(parseInt(seconds));
                }

                function formatNumberToTwoDigits(num){
                    if(!num || num == 0) {
                        return "00";
                    }
                    var formattedNum = num + "";

                    while(formattedNum.length < 2 ){
                        formattedNum = "0" + formattedNum;
                    }

                    return formattedNum;
                }

                function SetPerformanceDuration(duration) {
                    var data = JSON.stringify({"contestantId": <%= contestant.Id %>, "duration": duration});
                    $.ajax({
                        type: 'POST',
                        url: '<%= ResolveUrl("~/Show/Contest/Contestant/Contestant.aspx/SetDuration") %>',
                        data: data,
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        success: function (msg) {     
                            location.reload();
                        },
                        error: function (jqXHR, exception) {
                            alert("The was a problem and the duration could not be saved.");
                        }      
                    });
                }
            </script>
    <%  } %>  
</asp:Content>