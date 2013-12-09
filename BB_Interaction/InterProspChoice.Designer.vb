<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InterProspChoice
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BTN_INTERACTION = New System.Windows.Forms.Button()
        Me.BTN_PROSPECT = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(35, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(423, 19)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Would you like to create an Interaction or Prospect Plan Step?"
        '
        'BTN_INTERACTION
        '
        Me.BTN_INTERACTION.Location = New System.Drawing.Point(57, 74)
        Me.BTN_INTERACTION.Name = "BTN_INTERACTION"
        Me.BTN_INTERACTION.Size = New System.Drawing.Size(110, 23)
        Me.BTN_INTERACTION.TabIndex = 1
        Me.BTN_INTERACTION.Text = "Interaction"
        Me.BTN_INTERACTION.UseVisualStyleBackColor = True
        '
        'BTN_PROSPECT
        '
        Me.BTN_PROSPECT.Location = New System.Drawing.Point(187, 74)
        Me.BTN_PROSPECT.Name = "BTN_PROSPECT"
        Me.BTN_PROSPECT.Size = New System.Drawing.Size(110, 23)
        Me.BTN_PROSPECT.TabIndex = 2
        Me.BTN_PROSPECT.Text = "Propect Plan Step"
        Me.BTN_PROSPECT.UseVisualStyleBackColor = True
        '
        'Cancel_Button
        '
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(315, 74)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(110, 23)
        Me.Cancel_Button.TabIndex = 3
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = True
        '
        'InterProspChoice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(489, 117)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.BTN_PROSPECT)
        Me.Controls.Add(Me.BTN_INTERACTION)
        Me.Controls.Add(Me.Label1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "InterProspChoice"
        Me.ShowIcon = False
        Me.Text = "Interaction or Prospect Plan"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents BTN_INTERACTION As System.Windows.Forms.Button
    Friend WithEvents BTN_PROSPECT As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
End Class
