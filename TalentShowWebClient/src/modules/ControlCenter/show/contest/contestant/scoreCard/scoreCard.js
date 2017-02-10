﻿import React from 'react';
import ScoreCardStore from '../../../../../../data/stores/scoreCardStore';
import * as ScoreCardActions from '../../../../../../data/actions/scoreCardActions';
import * as ScoreCardUtil from './scoreCardUtil';
import ScorableCriteria from './scorableCriteria';
import PageContent from '../../../../../../common/pageContent';

class ScoreCardPage extends React.Component {
    constructor(props) {
        super(props);
        this.getState = this.getState.bind(this);
        this.storeChanged = this.storeChanged.bind(this);
        this.getScoreCard = this.getScoreCard.bind(this);
        this.getScoreCardId = this.getScoreCardId.bind(this);
        this.handleScorableCriteriaChange = this.handleScorableCriteriaChange.bind(this);
        this.state = this.getState();
    }

    componentWillMount(){
        ScoreCardStore.on("change", this.storeChanged);
        ScoreCardActions.loadScoreCard(this.getScoreCardId());
    }

    componentWillUnmount(){
        ScoreCardStore.off("change", this.storeChanged);
    }

    storeChanged(){
        this.setState(this.getState());
    }

    getState(){
        return { scoreCard: this.getScoreCard() };
    }

    getScoreCard() {
        return ScoreCardStore.get(this.getScoreCardId());
    }

    getScoreCardId() {
        return this.props.params.scoreCardId;
    }

    handleScorableCriteriaChange() {
        ScoreCardActions.updateScoreCard(this.state.scoreCard);  
    }

    render() {
        var scoreCard = this.state.scoreCard;
        
        if (!scoreCard){
            return (
                <PageContent title="Loading" description="The score card's details are loading, please wait."></PageContent>
            );
        }

        return (
            <PageContent title={ScoreCardUtil.getName(scoreCard)} description="">
                <ScorableCriteria scoreCardId={scoreCard.Id} onChange={this.handleScorableCriteriaChange}/>
            </PageContent>
        );
   }
}

export default ScoreCardPage;