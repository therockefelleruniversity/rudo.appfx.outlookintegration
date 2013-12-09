<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProspectPlanList
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lvPlans = New System.Windows.Forms.ListView()
        Me.BTN_SELECTED = New System.Windows.Forms.Button()
        Me.BTN_CANCEL = New System.Windows.Forms.Button()
        Me.lblProspectPlan = New System.Windows.Forms.Label()
        Me.lvPlanSteps = New System.Windows.Forms.ListView()
        Me.BTN_STEP = New System.Windows.Forms.Button()
        Me.cmbStepChoice = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'lvPlans
        '
        Me.lvPlans.Location = New System.Drawing.Point(39, 41)
        Me.lvPlans.Name = "lvPlans"
        Me.lvPlans.Size = New System.Drawing.Size(586, 97)
        Me.lvPlans.TabIndex = 0
        Me.lvPlans.UseCompatibleStateImageBehavior = False
        '
        'BTN_SELECTED
        '
        Me.BTN_SELECTED.Enabled = False
        Me.BTN_SELECTED.Location = New System.Drawing.Point(157, 157)
        Me.BTN_SELECTED.Name = "BTN_SELECTED"
        Me.BTN_SELECTED.Size = New System.Drawing.Size(114, 23)
        Me.BTN_SELECTED.TabIndex = 1
        Me.BTN_SELECTED.Text = "Use Selected Plan"
        Me.BTN_SELECTED.UseVisualStyleBackColor = True
        '
        'BTN_CANCEL
        '
        Me.BTN_CANCEL.Location = New System.Drawing.Point(389, 157)
        Me.BTN_CANCEL.Name = "BTN_CANCEL"
        Me.BTN_CANCEL.Size = New System.Drawing.Size(75, 23)
        Me.BTN_CANCEL.TabIndex = 2
        Me.BTN_CANCEL.Text = "Cancel"
        Me.BTN_CANCEL.UseVisualStyleBackColor = True
        '
        'lblProspectPlan
        '
        Me.lblProspectPlan.AutoSize = True
        Me.lblProspectPlan.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProspectPlan.Location = New System.Drawing.Point(153, 9)
        Me.lblProspectPlan.Name = "lblProspectPlan"
        Me.lblProspectPlan.Size = New System.Drawing.Size(311, 20)
        Me.lblProspectPlan.TabIndex = 3
        Me.lblProspectPlan.Text = "Select a Prospect Plan To Add a Step"
        '
        'lvPlanSteps
        '
        Me.lvPlanSteps.Location = New System.Drawing.Point(39, 168)
        Me.lvPlanSteps.Name = "lvPlanSteps"
        Me.lvPlanSteps.Size = New System.Drawing.Size(586, 10)
        Me.lvPlanSteps.TabIndex = 4
        Me.lvPlanSteps.UseCompatibleStateImageBehavior = False
        Me.lvPlanSteps.Visible = False
        '
        'BTN_STEP
        '
        Me.BTN_STEP.Enabled = False
        Me.BTN_STEP.Location = New System.Drawing.Point(157, 157)
        Me.BTN_STEP.Name = "BTN_STEP"
        Me.BTN_STEP.Size = New System.Drawing.Size(114, 23)
        Me.BTN_STEP.TabIndex = 5
        Me.BTN_STEP.Text = "Edit Selected Step"
        Me.BTN_STEP.UseVisualStyleBackColor = True
        Me.BTN_STEP.Visible = False
        '
        'cmbStepChoice
        '
        Me.cmbStepChoice.CausesValidation = False
        Me.cmbStepChoice.Enabled = False
        Me.cmbStepChoice.FormattingEnabled = True
        Me.cmbStepChoice.Items.AddRange(New Object() {"Planned Steps", "Completed Steps"})
        Me.cmbStepChoice.Location = New System.Drawing.Point(39, 143)
        Me.cmbStepChoice.Name = "cmbStepChoice"
        Me.cmbStepChoice.Size = New System.Drawing.Size(201, 21)
        Me.cmbStepChoice.TabIndex = 6
        Me.cmbStepChoice.Visible = False
        '
        'ProspectPlanList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(673, 196)
        Me.Controls.Add(Me.cmbStepChoice)
        Me.Controls.Add(Me.lvPlanSteps)
        Me.Controls.Add(Me.lblProspectPlan)
        Me.Controls.Add(Me.BTN_CANCEL)
        Me.Controls.Add(Me.BTN_SELECTED)
        Me.Controls.Add(Me.lvPlans)
        Me.Controls.Add(Me.BTN_STEP)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ProspectPlanList"
        Me.ShowIcon = False
        Me.Text = "Prospect Plan List"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvPlans As System.Windows.Forms.ListView
    Friend WithEvents BTN_SELECTED As System.Windows.Forms.Button
    Friend WithEvents BTN_CANCEL As System.Windows.Forms.Button
    Friend WithEvents lblProspectPlan As System.Windows.Forms.Label
    Friend WithEvents lvPlanSteps As System.Windows.Forms.ListView
    Friend WithEvents BTN_STEP As System.Windows.Forms.Button
    Friend WithEvents cmbStepChoice As System.Windows.Forms.ComboBox
End Class
