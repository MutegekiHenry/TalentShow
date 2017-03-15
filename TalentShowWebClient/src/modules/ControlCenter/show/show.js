﻿import React from 'react';
import * as Nav from '../../../routing/navigation';
import ContestsBox from './contests';
import ShowStore from '../../../data/stores/showStore';
import * as ShowActions from '../../../data/actions/showActions';
import * as ContestActions from '../../../data/actions/contestActions';
import PageContent from '../../../common/pageContent';
import Button from '../../../common/button';

class ShowPage extends React.Component {
    constructor(props) {
        super(props);
        this.getState = this.getState.bind(this);
        this.storeChanged = this.storeChanged.bind(this);
        this.getShow = this.getShow.bind(this);
        this.getShowId = this.getShowId.bind(this);
        this.handleEditShowClick = this.handleEditShowClick.bind(this);
    }

    shouldComponentUpdate(nextProps, nextState){
        return true;
    }

    componentWillMount(){
        ShowStore.on("change", this.storeChanged);
        var showId = this.getShowId();
        ShowActions.loadShow(showId);
        ContestActions.loadShowContests(showId);
        ContestActions.joinHubGroup(showId);
    }

    componentWillUnmount(){
        ShowStore.off("change", this.storeChanged);
        ContestActions.leaveHubGroup(this.getShowId());
    }

    storeChanged(){
        this.setState(this.getState());
    }

    getState(){
        return { show: this.getShow() };
    }

    getShow() {
        return ShowStore.get(this.getShowId());
    }

    getShowId() {
        return this.props.params.showId;
    }

    handleEditShowClick(e){
        e.preventDefault();
        Nav.goToEditShow(this.getShowId());
    }

    render() {
        if (!this.state || !this.state.show){
            return (
                <PageContent title="Loading" description="The show's details are loading, please wait."></PageContent>
            );
        }

        var show = this.state.show;

        var editShowButton = ( <Button key="editShow" type="primary" authorizedRoles={["admin"]} name="editShow" value="Edit" onClick={this.handleEditShowClick} /> );

        return (
            <PageContent title={show.Name} description={show.Description} buttons={[editShowButton]}>
                <ContestsBox showId={show.Id} />
            </PageContent>
        );
    }
}

export default ShowPage;