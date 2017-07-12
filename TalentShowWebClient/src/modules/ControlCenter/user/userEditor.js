﻿'use strict';
import React from 'react';
import Clone from 'clone';
import * as Nav from '../../../routing/navigation';
import Input from '../../../common/input';
import Select from '../../../common/select';
import FormGroup from '../../../common/formGroup';
import OrganizationStore from '../../../data/stores/organizationStore';

var UserEditor = React.createClass({
    getInitialState: function() {
        return this.createInitialState();
    },
    componentWillMount: function(){
        OrganizationStore.on("change", this.storeChanged);
    },
    componentWillUnmount: function(){
        OrganizationStore.off("change", this.storeChanged);
    },
    storeChanged: function(){
        this.setState({ Organizations: OrganizationStore.getOrganizations() });
    },
    handleEmailChange: function (e) {
        this.setState({ Email: e.target.value.trim() });
    },
    handleFirstNameChange: function (e) {
        this.setState({ FirstName: e.target.value.trim() });
    },
    handleLastNameChange: function (e) {
        this.setState({ LastName: e.target.value.trim() });
    },
    handleAffiliationChange: function(selectedOption) {
        var organization = null;

        if(selectedOption && selectedOption.organization && selectedOption.organization.Name){      
            organization = selectedOption.organization;     
        }

        this.setState({Affiliation: organization});
    },
    getOrganizationOptions: function() {
        var organizations = this.state.Organizations;
        var options = [];

        for (var i = 0; i < organizations.length; i++) {
            var organization = organizations[i];
            options.push({
                value: organization.Name, 
                label: organization.Name,
                organization: organization
            });
        }

        return options;
    },
    handlePasswordChange: function (e) {
        this.setState({ Password: e.target.value.trim() });
    },
    handleConfirmPasswordChange: function (e) {
        this.setState({ ConfirmPassword: e.target.value.trim() });
    },
    handleSubmit: function (e) {
        e.preventDefault();
        if (!this.state.Email || !this.state.FirstName || !this.state.LastName || !this.state.Affiliation || !this.state.Password || !this.state.ConfirmPassword) {
            return;
        }

        this.props.onUserFormSubmit({ 
            Email: this.state.Email,
            FirstName: this.state.FirstName,
            LastName: this.state.LastName,
            OrganizationId: this.state.Affiliation.Id,
            Password: this.state.Password,  
            ConfirmPassword: this.state.ConfirmPassword 
        });

        this.setState(this.createInitialState());
    },
    createInitialState: function () {
        var user = this.props.user;
        if(user){
            return { 
                Email: user.Email, 
                FirstName:  user.FirstName, 
                LastName:  user.LastName, 
                Affiliation: { Id: 0, Name: "", Parent: null }, 
                Password: "", 
                ConfirmPassword: "",
                Organizations: OrganizationStore.getOrganizations() 
            };
        }
        return { 
            Email: "", 
            FirstName: "", 
            LastName: "", 
            Affiliation: { Id: 0, Name: "", Parent: null }, 
            Password: "", 
            ConfirmPassword: "",
            Organizations: OrganizationStore.getOrganizations() 
        };
    },
    render: function() {
        return (
            <form className="registerForm"  onSubmit={this.handleSubmit}>
                <Input 
                    name="email" 
                    type="text"
                    label="Email"
                    value={this.state.Email}
                    onChange={this.handleEmailChange} />

                <Input 
                    name="firstName" 
                    type="text"
                    label="First Name"
                    value={this.state.FirstName}
                    onChange={this.handleFirstNameChange} />

                <Input 
                    name="lastName" 
                    type="text"
                    label="Last Name"
                    value={this.state.LastName}
                    onChange={this.handleLastNameChange} />

                <Select
                    name="affiliation"
                    label="Affiliation"
                    value={this.state.Affiliation.Name}
                    options={this.getOrganizationOptions()}
                    onChange={this.handleAffiliationChange} />

                <Input 
                    name="password" 
                    type="password"
                    label="Password"
                    value={this.state.Password}
                    onChange={this.handlePasswordChange} />

                <Input 
                    name="confirmPassword" 
                    type="password"
                    label="Confirm Password"
                    value={this.state.ConfirmPassword}
                    onChange={this.handleConfirmPasswordChange} />

                <FormGroup>
                    <input className="btn btn-primary" type="submit" value="Submit" />
                </FormGroup>
            </form>
        );
    }
});

export default UserEditor;