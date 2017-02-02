﻿import EventEmitter from 'event-emitter';
import * as JudgeApi from '../api/judgeApi'
import Dispatcher from '../dispatcher';

class JudgeStore extends EventEmitter {
    constructor(){
        super();
        this.judges = [];
    }
}

const judgeStore = new JudgeStore;

judgeStore.setJudges = function(_judges){
    judgeStore.judges = _judges;
    judgeStore.emit("change");
};

judgeStore.getContestJudges = function(){
    return this.judges;
};

judgeStore.loadContestJudges = function(contestId){
    JudgeApi.getContestJudges(contestId, judgeStore.setJudges);
};

judgeStore.getAll = function(){ //TODO probably remove this function
    return this.judges;
};

judgeStore.loadAllJudges = function(){
    JudgeApi.getAll(judgeStore.setJudges);
};

judgeStore.get = function(id){
    var judge = null;

    for (var i = 0; i < this.judges.length; i++){
        var currentJudge = this.judges[i];
        if(currentJudge.Id == id){
            judge = currentJudge;
            break;
        }
    }

    return judge;
};

judgeStore.add = function(judge){
    JudgeApi.add(judge, function(result){
        judgeStore.loadAllJudges();
    });
};

judgeStore.handleAction = function(action){
    switch(action.type){
        case "ADD_JUDGE":
            judgeStore.add(action.data);
            break;
        case "LOAD_ALL_JUDGES":
            judgeStore.loadAllJudges();
            break;
        case "LOAD_CONTEST_JUDGES":
            judgeStore.loadContestJudges(action.contestId);
            break;

    }
};

Dispatcher.register(judgeStore.handleAction.bind(judgeStore));

export default judgeStore;