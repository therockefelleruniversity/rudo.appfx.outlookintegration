Imports Microsoft.Office.Tools.Ribbon
Imports Blackbaud.AppFx.XmlTypes.DataForms

Public Class BBECRibbon
    Private _constituentId As Guid
    Dim intOwner As Object
    Private Sub BBECRibon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

        'Dim item = GetCurrentItem()
        'If item IsNot Nothing Then _constituentId = BBECHelper.GetConstituentId(item)

        'Dim enabled = (_constituentId <> Guid.Empty)
        'Me.btnConstituentSummary.Enabled = enabled
        'Me.btnAddInteraction.Enabled = True

    End Sub
    Private Function GetCurrentItem() As Outlook.MailItem
        Dim ThisOutlookSession As Outlook.Application = New Outlook.Application
        Dim NS As Outlook.NameSpace = ThisOutlookSession.Session
        Dim objsel As Object

        If TypeName(ThisOutlookSession.ActiveWindow) = "Inspector" Then
            objsel = ThisOutlookSession.ActiveInspector.CurrentItem
            Return TryCast(objsel, Outlook.MailItem)
        Else
            objsel = ThisOutlookSession.ActiveExplorer.Selection.Item(1)
            Return TryCast(objsel, Outlook.MailItem)
        End If

    End Function

    Private Sub btnConstituentSummary_Click(sender As System.Object, e As Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs) Handles btnConstituentSummary.Click
        Dim item = GetCurrentItem()
        If item IsNot Nothing Then _constituentId = BBECHelper.GetConstituentId(item)



        'Show Error Message If No Match By Email Address
        If _constituentId = Guid.Empty Then
            ConstituentNoExist(item)
            Return
        End If

        Try
            Dim form = BBECHelper.GetDataFormWebHostDialog()
            form.DataFormInstanceId = New Guid("1f4fea31-5779-44db-bcce-ec5afa36d82b")
            form.RecordId = _constituentId.ToString()
            form.ShowDialog()

        Catch ex As Exception
            BBECHelper.HandleException("There was an showing the constituent summary", ex)

        End Try
    End Sub

    Private Sub btnAddInteraction_Click(sender As System.Object, e As Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs) Handles btnAddInteraction.Click
        Dim item = GetCurrentItem()
        If item Is Nothing Then Return
        BBECHelper.ConstituentExists(item, 1)

    End Sub

    Private Sub ConstituentNoExist(item As Outlook.MailItem)
        Dim intName As String

        intName = BBECHelper.GetInteractionName(item)
        MsgBox(String.Concat("There is no constituent record for ", intName, "."), MsgBoxStyle.Information)

    End Sub

    Private Sub btnAddProspectPlan_Click(ByVal sender As System.Object, ByVal e As Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs) Handles btnAddProspectPlan.Click
        Dim item = GetCurrentItem()
        If item Is Nothing Then Return
        BBECHelper.ConstituentExists(item, 2)

    End Sub

    Private Sub btnEditProspectPlan_Click(ByVal sender As System.Object, ByVal e As Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs) Handles btnEditProspectPlan.Click
        Dim item = GetCurrentItem()
        If item Is Nothing Then Return
        BBECHelper.ConstituentExists(item, 3)

    End Sub
End Class
