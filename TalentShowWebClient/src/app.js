﻿import React from 'react';
import { render } from 'react-dom';
import { Router, Route, hashHistory, IndexRoute, Link  } from 'react-router';
import about from './modules/about';
import login from './modules/login';
import judges from './modules/judges';
import shows from './modules/ControlCenter/shows';
import show from './modules/ControlCenter/show/show';
import contest from './modules/ControlCenter/show/contest/contest';
import contestant from './modules/ControlCenter/show/contest/contestant/contestant';

var Menu = React.createClass({
    render: function () {
        return (
            <div>
                <nav className="navbar navbar-inverse navbar-static-top">
                    <div className="container-fluid">
                        <div className="navbar-header">
                            <span className="navbar-brand">
                                Control Center
                            </span>
                        </div>
                        <ul className="nav navbar-nav">
                            <li><Link to="/shows">Shows</Link></li>
                            <li><Link to="/judges">Judges</Link></li>
                            <li><Link to="/about">About</Link></li>
                        </ul>
                    </div>
                </nav>
                <div style={{margin: "20px"}}>{this.props.children}</div>
            </div>
        );
    }
});

function requireAuth(nextState, replaceState){
    if(!getToken()){
        replaceState({ nextPathname: nextState.location.pathname }, '/login');
    }
}

function getToken(){
    return sessionStorage.getItem("token");
}

render((
    <Router history={hashHistory}>
        <Route path="/">
            <IndexRoute component={login} />
            <Route onEnter={requireAuth} component={Menu}>
                <Route path="/shows" component={shows} />
                <Route path="/show/:showId" component={show} />
                <Route path="/show/:showId/contest/:contestId" component={contest} />
                <Route path="/show/:showId/contest/:contestId/contestant/:contestantId" component={contestant} />
                <Route path="/about" component={about} />
                <Route path="/judges" component={judges} />
            </Route>
        </Route>
     </Router>
), document.getElementById('app'));