﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InteractionChannelList.ascx.cs" Inherits="RockWeb.Blocks.Crm.InteractionChannelList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-random"></i> Interactions Channels List</h1>
            </div>
            <div class="panel-body">
                <div class="grid grid-panel">
                    <Rock:GridFilter ID="gfFilter" runat="server" OnApplyFilterClick="gfFilter_ApplyFilterClick" OnDisplayFilterValue="gfFilter_DisplayFilterValue">
                        <Rock:DefinedValuePicker ID="ddlMediumValue" runat="server" Label="Medium Type" Help="The Medium Type that identify the Content Channel." />
                    </Rock:GridFilter>
                    <ul class="list-group margin-all-md">
                        <asp:Repeater ID="rptChannel" runat="server">
                            <ItemTemplate>
                                <li class="list-group-item margin-b-md" style="background-color: #edeae6;">
                                    <asp:Literal ID="lContent" runat="server" Text='<%# Eval("ChannelHtml") %>'></asp:Literal>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>