Partial Class BBECRibbon
    Inherits Microsoft.Office.Tools.Ribbon.OfficeRibbon

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        If (container IsNot Nothing) Then
            container.Add(Me)
        End If

    End Sub

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tabBBEC = New Microsoft.Office.Tools.Ribbon.RibbonTab()
        Me.grpInteractions = New Microsoft.Office.Tools.Ribbon.RibbonGroup()
        Me.btnConstituentSummary = New Microsoft.Office.Tools.Ribbon.RibbonButton()
        Me.btnAddInteraction = New Microsoft.Office.Tools.Ribbon.RibbonButton()
        Me.btnAddProspectPlan = New Microsoft.Office.Tools.Ribbon.RibbonButton()
        Me.btnEditProspectPlan = New Microsoft.Office.Tools.Ribbon.RibbonButton()
        Me.tabBBEC.SuspendLayout()
        Me.grpInteractions.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabBBEC
        '
        Me.tabBBEC.Groups.Add(Me.grpInteractions)
        Me.tabBBEC.Label = "Blackbaud CRM"
        Me.tabBBEC.Name = "tabBBEC"
        '
        'grpInteractions
        '
        Me.grpInteractions.Items.Add(Me.btnConstituentSummary)
        Me.grpInteractions.Items.Add(Me.btnAddInteraction)
        Me.grpInteractions.Items.Add(Me.btnAddProspectPlan)
        Me.grpInteractions.Items.Add(Me.btnEditProspectPlan)
        Me.grpInteractions.Label = "Constituent Interactions"
        Me.grpInteractions.Name = "grpInteractions"
        '
        'btnConstituentSummary
        '
        Me.btnConstituentSummary.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btnConstituentSummary.Image = Global.BB_Interaction.My.Resources.Resources.individual
        Me.btnConstituentSummary.Label = "Constituent summary"
        Me.btnConstituentSummary.Name = "btnConstituentSummary"
        Me.btnConstituentSummary.ShowImage = True
        '
        'btnAddInteraction
        '
        Me.btnAddInteraction.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btnAddInteraction.Image = Global.BB_Interaction.My.Resources.Resources.interactions
        Me.btnAddInteraction.Label = "Add interaction"
        Me.btnAddInteraction.Name = "btnAddInteraction"
        Me.btnAddInteraction.ShowImage = True
        '
        'btnAddProspectPlan
        '
        Me.btnAddProspectPlan.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btnAddProspectPlan.Image = Global.BB_Interaction.My.Resources.Resources.interactions
        Me.btnAddProspectPlan.Label = "Add new prospect plan step"
        Me.btnAddProspectPlan.Name = "btnAddProspectPlan"
        Me.btnAddProspectPlan.ShowImage = True
        '
        'btnEditProspectPlan
        '
        Me.btnEditProspectPlan.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btnEditProspectPlan.Image = Global.BB_Interaction.My.Resources.Resources.interactions
        Me.btnEditProspectPlan.Label = "Update prospect plan step"
        Me.btnEditProspectPlan.Name = "btnEditProspectPlan"
        Me.btnEditProspectPlan.ShowImage = True
        '
        'BBECRibbon
        '
        Me.Name = "BBECRibbon"
        Me.RibbonType = "Microsoft.Outlook.Explorer, Microsoft.Outlook.Mail.Read"
        Me.Tabs.Add(Me.tabBBEC)
        Me.tabBBEC.ResumeLayout(False)
        Me.tabBBEC.PerformLayout()
        Me.grpInteractions.ResumeLayout(False)
        Me.grpInteractions.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabBBEC As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents grpInteractions As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents btnConstituentSummary As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents btnAddInteraction As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents btnAddProspectPlan As Microsoft.Office.Tools.Ribbon.RibbonButton
    Friend WithEvents btnEditProspectPlan As Microsoft.Office.Tools.Ribbon.RibbonButton
End Class

Partial Class ThisRibbonCollection
    Inherits Microsoft.Office.Tools.Ribbon.RibbonReadOnlyCollection

    <System.Diagnostics.DebuggerNonUserCode()> _
    Friend ReadOnly Property Ribbon1() As BBECRibbon
        Get
            Return Me.GetRibbon(Of BBECRibbon)()
        End Get
    End Property
End Class
