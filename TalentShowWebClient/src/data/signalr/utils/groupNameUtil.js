﻿var getContolCenterGroupName = function(){
    return "controlcenter";
};

var getShowGroupName = function(showId){
    return "show_" + showId;
};

var getContestGroupName = function(contestId){
    return "contest_" + contestId;
};

var getOrganizationsGroupName = function(){
    return "organizations";
};

var getUsersGroupName = function(){
    return "users";
};

var getContestantGroupName = function(contestantId){
    return "contestants_" + contestantId;
};

export {getContolCenterGroupName, getShowGroupName, getContestGroupName, getOrganizationsGroupName, getUsersGroupName, getContestantGroupName};