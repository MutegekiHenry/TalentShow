﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoreCriterionForm.ascx.cs" Inherits="TalentShowWeb.Show.Contest.ScoreCriterion.ScoreCriterionForm" %>

<div class="form-group">
    <asp:Label runat="server" Text="Description" AssociatedControlID="txtDescription" CssClass="control-label" />
    <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
</div>
<div class="form-group">
    <asp:Label runat="server" Text="Minimum Score" AssociatedControlID="txtMinScore" CssClass="control-label" />
    <asp:TextBox runat="server" ID="txtMinScore" CssClass="form-control" />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMinScore" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMinScore" CssClass="text-danger" ErrorMessage="Only Numbers Allowed" ValidationExpression="\d+" Display="Dynamic" />
</div>
<div class="form-group">
    <asp:Label runat="server" Text="Maximum Score" AssociatedControlID="txtMaxScore" CssClass="control-label" />
    <asp:TextBox runat="server" ID="txtMaxScore" CssClass="form-control" />
    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMaxScore" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMaxScore" CssClass="text-danger" ErrorMessage="Only Numbers Allowed" ValidationExpression="\d+" Display="Dynamic" />
</div>
<br />
<div class="form-group">
    <asp:Button runat="server" ID="btnSubmit" Text="Submit"  CausesValidation="true" CssClass="btn btn-primary" />
    <asp:Button runat="server" ID="btnCancel" Text="Cancel"  CausesValidation="false" CssClass="btn btn-danger" />
</div>