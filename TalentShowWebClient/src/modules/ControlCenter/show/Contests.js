﻿import React from 'react';
import { hashHistory } from 'react-router';
import { ListPanel, ListPanelItem } from '../../../common/listPanel';
import Button from '../../../common/button';
import ContestStore from '../../../data/stores/contestStore';
import * as ContestActions from '../../../data/actions/contestActions';

class ContestsBox extends React.Component {

    constructor(props) {
        super(props);
        this.getState = this.getState.bind(this);
        this.storeChanged = this.storeChanged.bind(this);
        this.handleAddContestClick = this.handleAddContestClick.bind(this); 
        this.getShowId = this.getShowId.bind(this);
        this.state = this.getState();
    }

    componentWillMount(){
        ContestStore.on("change", this.storeChanged);
        var showId = this.getShowId();
        ContestActions.loadShowContests(showId);
        ContestActions.joinHubGroup(showId);
    }

    componentWillUnmount(){
        ContestStore.off("change", this.storeChanged);
        var showId = this.getShowId();
        ContestActions.leaveHubGroup(showId);
    }

    storeChanged(){
        this.setState(this.getState());
    }

    getState(){
        return { contests: ContestStore.getShowContests() };
    }

    getShowId(){
        return this.props.showId;
    }

    handleAddContestClick(e){
        e.preventDefault();
        hashHistory.push('show/' + this.getShowId() + '/contests/add');
    }

    render() {
        var showId = this.getShowId();
        var contests = this.state.contests.map(function (contest) {
            return (
                <ListPanelItem 
                    key={contest.Id} 
                    name={contest.Name} 
                    description={contest.Description} 
                    pathname={ '/show/' + showId + '/contest/' + contest.Id } />
            );
        });

        var addContestButton = ( <Button type="primary" authorizedRoles={["admin"]} name="addContest" value="Add" onClick={this.handleAddContestClick} /> );

        return ( <ListPanel title="Contests" items={contests} button={addContestButton}/> );
    }
}

export default ContestsBox;