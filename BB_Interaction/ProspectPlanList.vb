Imports System.Windows.Forms
Imports Blackbaud.AppFx.WebAPI.ServiceProxy
Imports Blackbaud.AppFx.XmlTypes.DataForms

Public Class ProspectPlanList
    Private _appFx As Blackbaud.AppFx.WebAPI.ServiceProxy.AppFxWebService
    Private _myCred As System.Net.ICredentials
    Private _clientAppInfoHeader As Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
    Private _prospectLoaded As Boolean = False
    Private _stepLoaded As Boolean = False
    Protected Friend _prospectPlanID As String = ""
    Protected Friend _planStepID As String = ""
    Protected Friend _prospectID As String = ""
    Protected Friend _prospectParticipants As List(Of Guid) = New List(Of Guid)
    Protected Friend _prospectFundraisers As List(Of String) = New List(Of String)
    Protected Friend _prospectStepOwner As String = ""
    Private _provider As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider
    Protected Friend _intChoice As Integer = 2

    Private Function GetServiceProvider() As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider

        _clientAppInfoHeader = New Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
        _clientAppInfoHeader.ClientAppName = "CustomEventManager"

        Dim mySP As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider

        mySP = New Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider(BBECHelper.serviceUrlBasePath, BBECHelper.databaseName, _clientAppInfoHeader.ClientAppName)

        mySP.Credentials = GetNetworkCredentials()

        Return mySP

    End Function
   
    Private Function GetNetworkCredentials() As System.Net.ICredentials
        Dim securelyStoredUserName, securelyStoredPassword, securelyStoredDomain As String

        securelyStoredUserName = BBECHelper.dbUser
        securelyStoredPassword = BBECHelper.dbPwd
        securelyStoredDomain = Environment.UserDomainName

        Dim NetworkCredential As New System.Net.NetworkCredential(securelyStoredUserName, securelyStoredPassword, securelyStoredDomain)

        Return NetworkCredential

    End Function
    Private Sub LoadPlans()

        Dim prospectID As String = _prospectID
        Dim Req As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadRequest
        Dim Reply As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadReply

        Req.IncludeMetaData = True

        Dim Filter As New Blackbaud.AppFx.MajorGiving.Catalog.WebApiClient.DataLists.Constituent.ProspectPlanListFilterData

        Filter.INCLUDEINACTIVEPLANS = False

        Req = Blackbaud.AppFx.MajorGiving.Catalog.WebApiClient.DataLists.Constituent.ProspectPlanList.CreateRequest(_provider, prospectID, Filter)
        Req.IncludeMetaData = True
        Reply = Blackbaud.AppFx.MajorGiving.Catalog.WebApiClient.DataLists.Constituent.ProspectPlanList.LoadResults(_provider, Req)

        DisplayDataListReplyRowsInListView(Reply, lvPlans)

    End Sub
    Private Sub LoadSteps()
        Dim prospectPlan As String = _prospectPlanID
        Dim Req As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadRequest
        Dim Reply As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadReply

        _clientAppInfoHeader = New Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
        _clientAppInfoHeader.ClientAppName = "CustomEventManager"
        _clientAppInfoHeader.REDatabaseToUse = BBECHelper.databaseName

        With Req
            .ClientAppInfo = _clientAppInfoHeader

            If cmbStepChoice.SelectedItem = "Planned Steps" Then
                .DataListID = New Guid("cef6accb-ab60-4e93-8bb3-2098519458d6")
            ElseIf cmbStepChoice.SelectedItem = "Completed Steps" Then
                .DataListID = New Guid("376023AC-0557-4049-89FC-6105E1F5F534")
            Else
                .DataListID = New Guid("cef6accb-ab60-4e93-8bb3-2098519458d6")
            End If

            .ContextRecordID = prospectPlan
            .IncludeMetaData = True
        End With

        Reply = _provider.CreateAppFxWebService.DataListLoad(Req)

        DisplayDataListReplyRowsInListView(Reply, lvPlanSteps)

    End Sub

    Private Sub BTN_CANCEL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTN_CANCEL.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        _prospectPlanID = ""
        Me.Close()

    End Sub

    Private Sub BTN_SELECTED_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTN_SELECTED.Click

        If _prospectLoaded = True Then

            GetProspectPlanID()
            GetProspectFundRaisers()

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        Else
            MsgBox("No Prospect Plans Have Been Loaded")
            Me.Close()
        End If

    End Sub

    Private Sub BTN_STEP_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTN_STEP.Click

        If _stepLoaded = True Then
            GetProspectPlanStepID()
            getStepProspects()
            GetProspectPlanStepOwner()
            GetProspectFundRaisers()
            Me.Close()
        Else
            MsgBox("No Prospect Plan Steps Have Been Loaded")
            Me.Close()
        End If


    End Sub

    Private Sub GetProspectPlanID()

        Dim item As ListViewItem

        _prospectPlanID = ""

        Dim SelectedPlan As ListView.SelectedListViewItemCollection = Me.lvPlans.SelectedItems
        For Each item In SelectedPlan
            _prospectPlanID = item.SubItems(0).Text.ToString
        Next
        If _prospectPlanID = "" Then
            Exit Sub
        End If

    End Sub

    Private Sub GetProspectPlanStepID()

        Dim item As ListViewItem

        _planStepID = ""

        Dim SelectedPlanStep As ListView.SelectedListViewItemCollection = Me.lvPlanSteps.SelectedItems
        For Each item In SelectedPlanStep
            _planStepID = item.SubItems(0).Text.ToString
        Next
        If _planStepID = "" Then
            Exit Sub
        End If

    End Sub
    Private Sub GetProspectPlanStepOwner()

        Dim prospectPlanStep As String = _planStepID
        Dim Req As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadRequest
        Dim Reply As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadReply

        _clientAppInfoHeader = New Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
        _clientAppInfoHeader.ClientAppName = "CustomEventManager"
        _clientAppInfoHeader.REDatabaseToUse = BBECHelper.databaseName

        With Req
            .ClientAppInfo = _clientAppInfoHeader
            .DataListID = New Guid("5b15ec2b-8c15-4a76-b88a-222c0c399a69")
            .ContextRecordID = prospectPlanStep
            .IncludeMetaData = False
        End With

        Reply = _provider.CreateAppFxWebService.DataListLoad(Req)

        If (Reply.Rows IsNot Nothing) Then
            For Each row As Blackbaud.AppFx.WebAPI.ServiceProxy.DataListResultRow In Reply.Rows
                If (row.Values.GetValue(0) IsNot Nothing) Then
                    _prospectStepOwner = row.Values.GetValue(0).ToString
                End If
            Next
        End If

    End Sub

    Private Sub ProspectPlanList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadPlans()
    End Sub

    Private Sub DisplayDataListReplyRowsInListView(ByVal Reply As Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadReply,
                                                   ByVal ListView As System.Windows.Forms.ListView)
        Try
            'Display hourglass during appfx web service calls
            Cursor.Current = Cursors.WaitCursor
            Cursor.Show()

            With ListView
                .View = View.Details
                .FullRowSelect = True
                .Clear()
            End With

            If (Reply.Rows IsNot Nothing) Then
                For Each f As Blackbaud.AppFx.XmlTypes.DataListOutputFieldType In Reply.MetaData.OutputDefinition.OutputFields
                    If f.IsHidden = True Then
                        ListView.Columns.Add(f.FieldID, f.Caption, 0)
                    Else
                        ListView.Columns.Add(f.FieldID, f.Caption)
                    End If
                Next

                For Each row As Blackbaud.AppFx.WebAPI.ServiceProxy.DataListResultRow In Reply.Rows
                    ListView.Items.Add(New ListViewItem(row.Values))
                Next

                ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
                For Each f As Blackbaud.AppFx.XmlTypes.DataListOutputFieldType In Reply.MetaData.OutputDefinition.OutputFields
                    If f.IsHidden = True Then
                        ListView.Columns(f.FieldID).Width = 0
                    End If
                Next

                If ListView.Equals(lvPlans) Then
                    _prospectLoaded = True
                ElseIf ListView.Equals(lvPlanSteps) Then
                    _stepLoaded = True
                End If

            End If

        Catch ex As Exception
            MsgBox(ex.Message.ToString)

        Finally
            'Hide hourglass after api call
            Cursor.Current = Cursors.Default
            Cursor.Show()
        End Try
    End Sub

    Public Sub New(ByVal inProvider As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider)

        ' This call is required by the designer.
        InitializeComponent()
        _provider = inProvider

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub lvPlans_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lvPlans.MouseMove

        If lvPlans.SelectedItems.Count > 0 Then
            Me.BTN_SELECTED.Enabled = True
        Else
            Me.BTN_SELECTED.Enabled = False
        End If

    End Sub

    Private Sub lvPlans_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvPlans.SelectedIndexChanged

        If lvPlans.SelectedItems.Count > 0 Then
            If _intChoice = 3 Then
                GetProspectPlanID()
                lblProspectPlan.Text = "Select a Prospect Plan Step to Edit"
                cmbStepChoice.SelectedItem = "Planned Steps"
                cmbStepChoice.Enabled = True
                LoadSteps()
            End If
        End If

    End Sub

    Private Sub lvPlanSteps_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lvPlanSteps.MouseMove

        If lvPlanSteps.SelectedItems.Count > 0 Then
            Me.BTN_STEP.Enabled = True
        Else
            Me.BTN_STEP.Enabled = False
        End If

    End Sub


    Private Sub cmbStepChoice_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbStepChoice.SelectedValueChanged

        LoadSteps()

    End Sub

    Private Sub getStepProspects()

        Dim prospectPlanStep As String = _planStepID
        Dim Req As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadRequest
        Dim Reply As New Blackbaud.AppFx.WebAPI.ServiceProxy.DataListLoadReply

        _clientAppInfoHeader = New Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
        _clientAppInfoHeader.ClientAppName = "CustomEventManager"
        _clientAppInfoHeader.REDatabaseToUse = BBECHelper.databaseName

        With Req
            .ClientAppInfo = _clientAppInfoHeader
            .DataListID = New Guid("65a4f80f-95f7-4164-b27b-b6327fce96c2")
            .ContextRecordID = prospectPlanStep
            .IncludeMetaData = False
        End With

        Reply = _provider.CreateAppFxWebService.DataListLoad(Req)

        If (Reply.Rows IsNot Nothing) Then
            For Each row As Blackbaud.AppFx.WebAPI.ServiceProxy.DataListResultRow In Reply.Rows
                _prospectParticipants.Add(New System.Guid(row.Values.GetValue(0).ToString))
            Next
        End If


    End Sub

    Private Sub GetProspectFundRaisers()

        Dim Req As New SimpleDataListLoadRequest
        Dim Reply As New SimpleDataListLoadReply

        _clientAppInfoHeader = New Blackbaud.AppFx.WebAPI.ServiceProxy.ClientAppInfoHeader
        _clientAppInfoHeader.ClientAppName = "CustomEventManager"
        _clientAppInfoHeader.REDatabaseToUse = BBECHelper.databaseName

        With Req
            .DataListID = New Guid("5ec35496-8035-4fb7-a565-33c90f71ae75")
            .Parameters = New DataFormItem
            .Parameters.SetValue("PLANID", _prospectPlanID)
            .ClientAppInfo = _clientAppInfoHeader
        End With

        Reply = _provider.CreateAppFxWebService.SimpleDataListLoad(Req)

        If (Reply.Rows IsNot Nothing) Then
            For Each row As SimpleDataListResultRow In Reply.Rows
                _prospectFundraisers.Add(row.Value)
            Next
        End If

    End Sub


End Class