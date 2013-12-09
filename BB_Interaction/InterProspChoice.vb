Public Class InterProspChoice

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub BTN_INTERACTION_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTN_INTERACTION.Click

        Me.DialogResult = Windows.Forms.DialogResult.Yes
        Me.Close()

    End Sub

    Private Sub BTN_PROSPECT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTN_PROSPECT.Click

        Me.DialogResult = Windows.Forms.DialogResult.No
        Me.Close()

    End Sub

End Class