Imports System.Windows.Forms

Public Class ConstituentDialog


    Private Sub Create_Button_Click(sender As System.Object, e As System.EventArgs) Handles Create_Button.Click
        Me.DialogResult = Windows.Forms.DialogResult.Yes
        Me.Close()
    End Sub


    Private Sub Search_Button_Click(sender As System.Object, e As System.EventArgs) Handles Search_Button.Click
        Me.DialogResult = Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(sender As System.Object, e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
End Class
