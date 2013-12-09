Public Class ThisAddIn

    Private _btnAddInteraction As Office.CommandBarButton

    Private Sub ThisAddIn_Startup() Handles Me.Startup

    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown

    End Sub

    Private Sub Application_ContextMenuClose(ContextMenu As Microsoft.Office.Interop.Outlook.OlContextMenu) Handles Application.ContextMenuClose

        Try
            If _btnAddInteraction IsNot Nothing Then
                RemoveHandler _btnAddInteraction.Click, AddressOf AddInteractionClickHandler
            End If
            _btnAddInteraction = Nothing

        Catch ex As Exception
            BBECHelper.HandleException("Error occurred closing the context menu", ex)

        End Try

    End Sub

    Private Sub Application_ItemContextMenuDisplay(CommandBar As Microsoft.Office.Core.CommandBar, Selection As Microsoft.Office.Interop.Outlook.Selection) Handles Application.ItemContextMenuDisplay
        If Selection.Count > 1 Then Return
        Dim item = TryCast(Selection.Item(1), Outlook.MailItem)
        If item Is Nothing Then Return

        Try
            Dim constituentId = BBECHelper.GetConstituentId(item)
            Dim constituentIdString = constituentId.ToString()
            Dim enabled = (constituentId <> Guid.Empty)

            Dim key = "Blackbaud.CRM"
            Dim cmdCtrl = DirectCast(CommandBar.FindControl(Tag:=key, Recursive:=True), Office.CommandBarPopup)
            If cmdCtrl Is Nothing Then
                cmdCtrl = CommandBar.Controls.Add(Type:=Office.MsoControlType.msoControlPopup, Parameter:=key, Temporary:=True)
                cmdCtrl.Caption = "Blackbaud CRM"
                cmdCtrl.Tag = key
                cmdCtrl.BeginGroup = True
            End If

            If _btnAddInteraction Is Nothing Then
                _btnAddInteraction = cmdCtrl.Controls.Add(Office.MsoControlType.msoControlButton)
                _btnAddInteraction.Caption = "Add an interaction"
                _btnAddInteraction.Tag = constituentIdString
                _btnAddInteraction.Enabled = True
                AddHandler _btnAddInteraction.Click, AddressOf AddInteractionClickHandler
            End If

        Catch ex As Exception
            BBECHelper.HandleException("Error occurred building the context menu", ex)

        End Try

    End Sub

    Private Sub AddInteractionClickHandler(ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean)
        'Dim intReturnValue As Integer
        'Dim vConstituentID As New Guid()
        Dim item = TryCast(Me.Application.ActiveExplorer.Selection.Item(1), Outlook.MailItem)
        If item Is Nothing Then Return
        BBECHelper.ConstituentExists(item, 1)

        'Dim intName As String
        'intName = BBECHelper.GetInteractionName(item)

        'Dim constituentId = ctrl.Tag
        ''MsgBox(constituentId.ToString)
        'If String.Equals(constituentId, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase) Then
        '    vConstituentID = BBECHelper.SearchConstituent()
        '    If String.IsNullOrEmpty(vConstituentID.ToString) Then Return
        '    BBECHelper.AddInteraction(item, vConstituentID)
        'Else
        '    intReturnValue = MsgBox(String.Concat("Do you want to search for a constituent other than ", intName, "?"), MsgBoxStyle.YesNoCancel)
        '    If intReturnValue = 7 Then
        '        If String.IsNullOrEmpty(constituentId) Then Return
        '        BBECHelper.AddInteraction(item, New Guid(constituentId))
        '    ElseIf intReturnValue = 6 Then
        '        vConstituentID = BBECHelper.SearchConstituent()
        '        If String.Equals(vConstituentID.ToString, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase) Then Return
        '        BBECHelper.AddInteraction(item, vConstituentID)
        '    End If
        'End If
    End Sub

End Class
