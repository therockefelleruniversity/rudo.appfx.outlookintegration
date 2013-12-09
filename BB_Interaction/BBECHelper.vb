Imports Blackbaud.AppFx.WebAPI
Imports Blackbaud.AppFx.UIModeling.DataFormWebHost
Imports Blackbaud.AppFx.XmlTypes.DataForms
Imports System.Collections
Imports Blackbaud.AppFx.XmlTypes
Imports Blackbaud.AppFx.WebAPI.ServiceProxy

Friend NotInheritable Class BBECHelper

    Public Const applicationTitle As String = "Blackbaud CRM"

    Protected Friend Const serviceUrlBasePath As String = ""
    Protected Friend Const databaseName As String = ""

    Protected Friend Const dbUser As String = ""
    Protected Friend Const dbPwd As String = ""

    Private Sub New()
    End Sub

    Private Shared _provider As AppFxWebServiceProvider

    Friend Shared Function GetProvider() As AppFxWebServiceProvider

        If _provider Is Nothing Then
            _provider = New Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider
            _provider.Url = String.Concat(serviceUrlBasePath, "appfxwebservice.asmx")
            _provider.Database = databaseName
            _provider.Credentials = New System.Net.NetworkCredential(dbUser, dbPwd, Environment.UserDomainName)
        End If

        Return _provider

    End Function

    Friend Shared Sub AddInteraction(ByVal item As Outlook.MailItem, ByVal constituentId As Guid)
        Dim intOwner As Guid = GetFundRaiserId()
        Dim vInt As String = ""
        Dim vReponse As Integer
        Dim ccCount As Integer = 0
        Dim participants As List(Of ParticipantGUIDStruct) = New List(Of ParticipantGUIDStruct)
        Dim vComment As String = ""

        Try
            If item Is Nothing Then Return
            If constituentId = Guid.Empty Then Return

            _provider = Nothing

            Dim provider = GetProvider()
            Dim form = BBECHelper.GetDataFormWebHostDialog()
            Dim formData As New DataFormFieldValueSet

            form.DataFormInstanceId = New Guid("723ad883-f995-4c40-afed-6a7914b536e3")
            form.ContextRecordId = constituentId.ToString()
            formData.Add(New DataFormFieldValue("FUNDRAISERID", intOwner))

            If (item.Sent) Then
                formData.SetValue("ACTUALDATE", item.SentOn.ToShortDateString)
            Else
                formData.SetValue("ACTUALDATE", item.ReceivedTime.ToShortDateString)
            End If

            'Build Comment field
            'Mail Sender + Mail Recipients + Mail Body
            vComment = BuildNiceHeader(item.Recipients, item) + vbCrLf + item.Subject + vbCrLf + vbCrLf + item.Body

            formData.SetValue("COMMENT", vComment)
            formData.SetValue("OBJECTIVE", Mid(item.Subject, 1, 100))
            formData.SetValue("STATUSCODE", 2)
            formData.SetValue("INTERACTIONTYPECODEID", Blackbaud.AppFx.Constituent.Catalog.WebApiClient.CodeTables.InteractionType.GetId(provider, "Email"))

            'CREATE PARTICIPANTS
            'Step 1 - Set count of possible participants from TO field
            If (item.To.Split(";").Count() > 1) Then
                ccCount = item.To.Split(";").Count() - 1
            End If

            'Step 2 - Set count of possible participants from CC field
            If Not (item.CC Is Nothing) Then
                ccCount = ccCount + item.CC.Split(";").Count()
            End If

            'Step 3 - Loop through possible participants, create list of GUIDs
            '         Use CreateParticipants Function
            If ccCount > 0 Then
                participants = CreateParticipants(item, ccCount)
            End If

            If (participants.Count > 0) Then
                formData.SetValue("PARTICIPANTS", CollectionToDataFormFieldValue(participants, "PARTICIPANTS", constituentId, intOwner))
            End If

            Dim dfi As New DataFormItem
            dfi.Values = formData
            form.DefaultValues = dfi
            form.ShowDialog()

            If String.IsNullOrEmpty(form.RecordId) Then
                MsgBox("No Interaction was created", MsgBoxStyle.Information, applicationTitle)
            Else
                vReponse = MsgBox("An Interaction Was Created. Do you want to view the created interaction?", MsgBoxStyle.YesNoCancel)
                If vReponse = 6 Then
                    vInt = form.RecordId
                    Dim vForm = BBECHelper.GetDataFormWebHostDialog()
                    vForm.DataFormInstanceId = New Guid("e3574968-1684-4b51-9752-3599be1b4ec4")
                    vForm.RecordId = vInt
                    vForm.ShowDialog()
                End If
            End If



        Catch ex As Exception
            BBECHelper.HandleException("There was an error showing the Add Interaction form", ex)

        Finally
            _provider = Nothing

        End Try

    End Sub
    Friend Shared Sub AddProspect(ByVal item As Outlook.MailItem, ByVal constituentId As Guid, ByVal intChoice As Integer)
        Dim intOwner As Guid = GetFundRaiserId()
        Dim intOwnerExists As Boolean = False
        Dim vInt As String = ""
        Dim vReponse As Integer
        Dim ccCount As Integer = 0
        Dim participants As List(Of ParticipantGUIDStruct) = New List(Of ParticipantGUIDStruct)
        Dim participantsMod As List(Of ParticipantGUIDStruct) = New List(Of ParticipantGUIDStruct)
        Dim vComment As String = ""
        Dim planDialog As ProspectPlanList
        Dim myResult As System.Windows.Forms.DialogResult

        Try
            If item Is Nothing Then Return
            If constituentId = Guid.Empty Then Return

            _provider = Nothing
            Dim provider = GetProvider()

            planDialog = New ProspectPlanList(_provider)
            planDialog._prospectID = constituentId.ToString()
            planDialog._intChoice = intChoice

            If intChoice = 3 Then
                planDialog.Height = 350
                planDialog.lvPlanSteps.Height = 97
                planDialog.BTN_CANCEL.Top = 275
                planDialog.BTN_STEP.Top = 275
                planDialog.lvPlanSteps.Visible = True
                planDialog.BTN_SELECTED.Visible = False
                planDialog.BTN_STEP.Visible = True
                planDialog.cmbStepChoice.Visible = True
                planDialog.lblProspectPlan.Text = "Select a Prospect Plan to Edit a Step"
            End If

            planDialog.ShowDialog()

            If Len(planDialog._prospectPlanID) = 0 Then
                MsgBox("No Prospect Plan Was Selected")
                Exit Sub
            End If

            Dim form = BBECHelper.GetDataFormWebHostDialog()
            Dim formData As New DataFormFieldValueSet

            If intChoice = 2 Then
                form.DataFormInstanceId = New Guid("B6DC6978-CBB4-49CA-9A33-43E0FF635671")
                form.ContextRecordId = planDialog._prospectPlanID
                formData.SetValue("OBJECTIVE", Left(item.Subject, 100))
            ElseIf intChoice = 3 Then
                form.DataFormInstanceId = New Guid("4131F838-FE6E-4796-B7C1-2307366BDF0D")
                form.RecordId = planDialog._planStepID
            End If


            'Check if Current Owner matches Logged in User
            'If no, set Owner to Logged in User
            If (planDialog._prospectFundraisers.Count > 0) Then
                For Each row In planDialog._prospectFundraisers
                    If intOwner.ToString.Equals(row) Then
                        intOwnerExists = True
                    End If
                Next
                Dim emptyCount As Integer = 0
                For Each row In planDialog._prospectFundraisers
                    If (String.IsNullOrEmpty(row)) Then
                        emptyCount += 1
                    End If
                Next
                If emptyCount.Equals(planDialog._prospectFundraisers.Count) Then
                    intOwnerExists = True
                End If
            End If


            If ((planDialog._prospectFundraisers.Count > 0) And (intOwnerExists = False)) Then
                intOwner = New System.Guid(planDialog._prospectFundraisers.ElementAt(0))
            End If

            'Set Owner only on Add Prospect Plan Step action OR on update if Owner is NULL
            If (intChoice = 2) Or (intChoice = 3 And planDialog._prospectStepOwner = "") Then
                formData.Add(New DataFormFieldValue("FUNDRAISERID", intOwner))
                formData.Add(New DataFormFieldValue("OWNERID", intOwner))
            End If

            formData.SetValue("STATUSCODE", 2)

            If (item.Sent) Then
                formData.SetValue("ACTUALDATE", item.SentOn.ToShortDateString)
            Else
                formData.SetValue("ACTUALDATE", item.ReceivedTime.ToShortDateString)
            End If

            'Retreive GUIDs for CC'd emails and To addresses other than first
            If (item.To.Split(";").Count() > 1) Then
                ccCount = item.To.Split(";").Count() - 1
            End If

            If Not (item.CC Is Nothing) Then
                ccCount = ccCount + item.CC.Split(";").Count()
            End If

            If ccCount > 0 Then
                participants = CreateParticipants(item, ccCount)
            End If

            If (participants.Count > 0 And intChoice = 2) Then
                formData.SetValue("PARTICIPANTS", CollectionToDataFormFieldValue(participants, "PARTICIPANTS", constituentId, intOwner))
            ElseIf (participants.Count > 0 And intChoice = 3) Then
                If planDialog._prospectParticipants.Count > 0 Then
                    For Each row In participants
                        participantsMod.Add(row)
                        For Each row2 In planDialog._prospectParticipants
                            If (row2.CompareTo(row.PartGuid) = 0) Then
                                participantsMod.Remove(row)
                            End If
                        Next
                    Next
                    If participantsMod.Count > 0 Then
                        formData.SetValue("PARTICIPANTS", CollectionToDataFormFieldValue(participantsMod, "PARTICIPANTS", constituentId, intOwner))
                    End If
                End If
            End If


            vComment = BuildNiceHeader(item.Recipients, item) + vbCrLf + item.Subject + vbCrLf + vbCrLf + item.Body

            formData.SetValue("COMMENT", vComment)
            formData.SetValue("STATUSCODE", 2)
            formData.SetValue("INTERACTIONTYPECODEID", Blackbaud.AppFx.Constituent.Catalog.WebApiClient.CodeTables.InteractionType.GetId(provider, "Email"))
            Dim dfi As New DataFormItem
            dfi.Values = formData
            form.DefaultValues = dfi
            myResult = form.ShowDialog()

            If intChoice = 2 Then
                If String.IsNullOrEmpty(form.RecordId) Then
                    MsgBox("No Prospect Plan Step was created", MsgBoxStyle.Information, applicationTitle)
                Else
                    vReponse = MsgBox("A Prospect Plan Step Was Created. Do you want to view the created prospect plan step?", MsgBoxStyle.YesNoCancel)
                    If vReponse = 6 Then
                        vInt = form.RecordId
                        Dim vForm = BBECHelper.GetDataFormWebHostDialog()
                        vForm.DataFormInstanceId = New Guid("849327fb-b458-4f14-a731-0cad5de0eba2")
                        vForm.RecordId = vInt
                        vForm.ShowDialog()
                    End If
                End If
            End If

            If intChoice = 3 Then
                If myResult = System.Windows.Forms.DialogResult.Cancel Then
                    MsgBox("The Prospect Plan Step Was Not Updated")
                Else
                    vReponse = MsgBox("The Prospect Plan Step Was Updated. Do you want to view the updated prospect plan step?", MsgBoxStyle.YesNoCancel)
                    If vReponse = 6 Then
                        vInt = form.RecordId
                        Dim vForm = BBECHelper.GetDataFormWebHostDialog()
                        vForm.DataFormInstanceId = New Guid("849327fb-b458-4f14-a731-0cad5de0eba2")
                        vForm.RecordId = vInt
                        vForm.ShowDialog()
                    End If
                End If
            End If



        Catch ex As Exception
            BBECHelper.HandleException("There was an error showing the Add Prospect Plan Step form", ex)

        Finally
            _provider = Nothing

        End Try

    End Sub
    Friend Shared Function SearchConstituent(ByVal intName As String) As Guid
        Dim vConstituentID As New Guid()
        Dim nameArray As Array
        Dim fName As String = ""
        Dim lName As String = ""

        Try
            _provider = Nothing
            Dim provider = GetProvider()
            Dim form = BBECHelper.GetSearchFormWebHostDialog()
            form.SearchListId = New Guid("23c5c603-d7d8-4106-aecc-65392b563887")
            Dim formData As New DataFormFieldValueSet
            nameArray = SplitCustName(intName)
            fName = nameArray.GetValue(0)
            lName = nameArray.GetValue(1)
            formData.SetValue("FIRSTNAME", fName)
            formData.SetValue("KEYNAME", lName)
            Dim dfi As New DataFormItem
            dfi.Values = formData
            form.SetSearchCriteria(dfi)
            form.ShowDialog()
            vConstituentID = New Guid(form.SelectedRecordId)
            Return vConstituentID

        Catch ex As Exception
            'Swallow error

        Finally
            _provider = Nothing

        End Try

    End Function
    Friend Shared Function SearchProspect(ByVal intName As String) As Guid
        Dim vProspectID As New Guid()
        Dim nameArray As Array
        Dim fName As String = ""
        Dim lName As String = ""

        Try
            _provider = Nothing
            Dim provider = GetProvider()
            Dim form = BBECHelper.GetSearchFormWebHostDialog()
            form.SearchListId = New Guid("DF763CBC-6F79-4E50-8849-E34E71BD5250")
            Dim formData As New DataFormFieldValueSet
            nameArray = SplitCustName(intName)
            fName = nameArray.GetValue(0)
            lName = nameArray.GetValue(1)
            formData.SetValue("FIRSTNAME", fName)
            formData.SetValue("KEYNAME", lName)
            Dim dfi As New DataFormItem
            dfi.Values = formData
            form.SetSearchCriteria(dfi)
            form.ShowDialog()
            vProspectID = New Guid(form.SelectedRecordId)
            Return vProspectID

        Catch ex As Exception
            'Swallow error

        Finally
            _provider = Nothing

        End Try

    End Function
    Friend Shared Function GetConstituentId(ByVal item As Outlook.MailItem) As Guid

        Dim emailAddress = ResolveEmailAddress(item)
        Return GetConstituentId(emailAddress)

    End Function

    Friend Shared Function GetConstituentId(ByVal emailAddress As String) As Guid
        Dim consGUID As New System.Guid

        consGUID = Guid.Empty

        Try
            Dim filter = New Blackbaud.AppFx.Constituent.Catalog.WebApiClient.SearchLists.Constituent.ConstituentSearchFilterData
            filter.EMAILADDRESS = emailAddress
            filter.EXACTMATCHONLY = True
            filter.INCLUDEGROUPS = False
            filter.INCLUDEORGANIZATIONS = False

            If _provider Is Nothing Then
                _provider = GetProvider()
            End If

            Dim provider = _provider

            Dim IDs() As String = Blackbaud.AppFx.Constituent.Catalog.WebApiClient.SearchLists.Constituent.ConstituentSearch.GetIDs(provider, filter)

            If IDs.Length = 1 Then
                consGUID = New System.Guid(IDs(0))
                'Return New System.Guid(IDs(0))
            ElseIf (IDs.Length = 0 And emailAddress.Contains("@rockefeller.edu")) Then
                consGUID = GetConstituentId(Replace(emailAddress, "@rockefeller.edu", "@mail.rockefeller.edu"))
            End If

        Catch ex As Exception
            HandleException("There was an error getting the constituent ID", ex)

        Finally
            _provider = Nothing

        End Try

        Return consGUID

    End Function

    Friend Shared Function GetFundRaiserId() As Guid

        Dim emailAddress = String.Concat(Environment.UserName, "@rockefeller.edu")
        Return GetConstituentId(emailAddress)

    End Function

    Friend Shared Function ResolveEmailAddress(ByVal item As Outlook.MailItem) As String
        Dim currentUser As Outlook.AddressEntry = item.Session.CurrentUser.AddressEntry
        Dim mailAddress As String

        Try
            'Determine if RU Exchange Address
            If String.Equals(item.SenderEmailType, "EX", StringComparison.OrdinalIgnoreCase) Then
                mailAddress = GetSMTPSenderRecipient(item.Recipients, item)
                Return mailAddress  'Return RU Exchange Email
            End If

        Catch ex As Exception
            'eat the error
        End Try

        'Return SMTP address of external address
        Return Replace(item.SenderEmailAddress, "@mail.rockefeller.edu", "@rockefeller.edu")

    End Function
    Friend Shared Function GetSMTPSenderRecipient(ByVal item As Outlook.Recipients, ByVal item2 As Outlook.MailItem) As String
        Dim exUser As Outlook.ExchangeUser
        Dim intEmail As String = Nothing
        Dim done = False

        If Not (IsNothing(item2.ReceivedByName)) Then
            If item2.Sender.DisplayType = Outlook.OlDisplayType.olUser Or item2.Sender.DisplayType = Outlook.OlDisplayType.olRemoteUser Then
                If item2.Sender.Address.ToUpper.Contains("/O=RUEXCHMAIL") Then
                    exUser = item2.Sender.GetExchangeUser()
                    intEmail = exUser.PrimarySmtpAddress
                Else
                    intEmail = item2.SenderEmailAddress
                End If
            End If
        Else
            For Each olRecipient As Outlook.Recipient In item
                If (olRecipient.Type = Outlook.OlMailRecipientType.olTo) Then
                    If olRecipient.DisplayType = Outlook.OlDisplayType.olUser Or olRecipient.DisplayType = Outlook.OlDisplayType.olRemoteUser Then
                        If olRecipient.Address.ToUpper.Contains("/O=RUEXCHMAIL") Then
                            exUser = olRecipient.AddressEntry.GetExchangeUser()
                            intEmail = exUser.PrimarySmtpAddress
                            done = True
                        Else
                            intEmail = olRecipient.Address
                            done = True
                        End If
                    End If
                End If
                If done Then Exit For
            Next
        End If

        Return intEmail

    End Function
    Friend Shared Function GetInteractionName(ByVal item As Outlook.MailItem) As String
        Dim intName As String
        Dim currentUser As Outlook.AddressEntry = item.Session.CurrentUser.AddressEntry

        intName = ""

        Try
            'Determine if RU Exchange Address
            If String.Equals(item.SenderEmailAddress, currentUser.Address, StringComparison.OrdinalIgnoreCase) Then
                'MsgBox("Sent")
                intName = item.Recipients(1).Name
            Else
                'MsgBox("Received")
                intName = item.SenderName
            End If

        Catch ex As Exception
            BBECHelper.HandleException("Can't determine the current user", ex)
        End Try

        Return intName

    End Function

    Friend Shared Function GetDataFormWebHostDialog() As DataFormWebHostDialog

        Dim form = New DataFormWebHostDialog
        form.ServiceUrlBasePath = serviceUrlBasePath
        form.DatabaseName = databaseName
        form.ApplicationTitle = applicationTitle
        form.Credentials = New System.Net.NetworkCredential(dbUser, dbPwd, Environment.UserDomainName)
        Return form

    End Function

    Friend Shared Function GetSearchFormWebHostDialog() As SearchFormWebHostDialog

        Dim form = New SearchFormWebHostDialog
        form.ServiceUrlBasePath = serviceUrlBasePath
        form.DatabaseName = databaseName
        form.ApplicationTitle = applicationTitle
        form.Credentials = New System.Net.NetworkCredential(dbUser, dbPwd, Environment.UserDomainName)
        Return form

    End Function

    Friend Shared Sub ConstituentExists(ByVal item As Outlook.MailItem, ByVal inType As Integer)

        Dim intReturnValue As Integer
        Dim intChoiceValue As Integer
        Dim vConstituentID As New Guid()
        Dim constituentID As String = ""
        Dim constDialog As ConstituentDialog = New ConstituentDialog
        Dim intProsDialog As InterProspChoice = New InterProspChoice
        If item Is Nothing Then Return
        If item IsNot Nothing Then constituentID = GetConstituentId(item).ToString

        intChoiceValue = inType
        Dim myChoice As String = "Nothing"
        Dim myChoiceHeader As String = "Nothing"

        If intChoiceValue = 1 Then 'Interaction
            myChoice = "an Interaction"
            myChoiceHeader = "Create Interaction"
        ElseIf intChoiceValue = 2 Then 'New Prospect Plan Step
            myChoice = "a Prospect Plan Step"
            myChoiceHeader = "Create Prospect Plan Step"
        ElseIf intChoiceValue = 3 Then 'Update Prospect Plan Step
            myChoice = "a Prospect Plan Step"
            myChoiceHeader = "Edit Prospect Plan Step"
        End If

        'Set Constituent Name from email
        'If email is received, name is sender
        'If email is sent, name is first recipient
        Dim intName As String
        intName = BBECHelper.GetInteractionName(item)

        'Add an Interaction
        If intChoiceValue = 1 Then
            If String.Equals(constituentID, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase) Then
                'Get Consituent ID from Constituent Name
                vConstituentID = BBECHelper.SearchConstituent(intName)
                If String.IsNullOrEmpty(vConstituentID.ToString) Then Return
                BBECHelper.AddInteraction(item, vConstituentID)
            Else
                constDialog.Text = myChoiceHeader
                constDialog.lblConstituent.Text = String.Concat("Do you want to create ", myChoice, " for ", intName, ", or search for another constituent?")
                intReturnValue = constDialog.ShowDialog
                'Create for Prompted Constituent
                If intReturnValue = 6 Then
                    If String.IsNullOrEmpty(constituentID) Then Return
                    BBECHelper.AddInteraction(item, New Guid(constituentID))
                    'Search for Constituent
                ElseIf intReturnValue = 7 Then
                    vConstituentID = BBECHelper.SearchConstituent(intName)
                    If String.Equals(vConstituentID.ToString, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase) Then Return
                    BBECHelper.AddInteraction(item, vConstituentID)
                End If
            End If

            'Add a Prospect Plan Step 
        ElseIf intChoiceValue = 2 Or intChoiceValue = 3 Then
            vConstituentID = BBECHelper.SearchProspect(intName)
            If String.IsNullOrEmpty(vConstituentID.ToString) Then Return
            BBECHelper.AddProspect(item, vConstituentID, intChoiceValue)
        End If

    End Sub

    Friend Shared Sub TagConstituent(ByVal item As Outlook.MailItem, ByVal constituentId As Guid, ByVal constituentName As String)

        If item Is Nothing Then Return

        Try
            Dim provider = GetProvider()

            Dim req = DataFormServices.CreateDataFormSaveRequest(provider, New Guid("d286b10f-2d65-4603-991e-bf322f37a9a6"))
            req.ContextRecordID = item.EntryID

            Dim dfi = New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            dfi.SetValue("CONSTITUENTID", constituentId)
            req.DataFormItem = dfi

            DataFormServices.SaveData(provider, req)

            MsgBox(String.Format("{0} was tagged on this email.", constituentName), vbInformation, applicationTitle)

        Catch ex As Exception
            HandleException("There was an error tagging this constituent", ex)

        End Try

    End Sub

    Friend Shared Function SplitCustName(ByVal strNameIn As String) As Array
        Dim strParts() As String
        Dim intParts As Integer
        Dim strP1() As String
        Dim nameOut As Array = Array.CreateInstance(GetType(String), 2)
        Dim strWork As String
        Dim i As Integer
        Dim j As Integer
        Dim strFNameOut As String
        Dim strLNameOut As String

        'Initialize output fields
        strFNameOut = String.Empty
        strLNameOut = String.Empty
        nameOut.SetValue(strFNameOut, 0)
        nameOut.SetValue(strLNameOut, 1)

        'Set initial letters to uppercase
        'strNameIn = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strNameIn)
        'Remove all extraneous spaces
        '	30 is just an arbitrary number.  There shouldn't
        '	be that many spaces in the field
        For i = 1 To 30
            strNameIn = strNameIn.Replace("  ", " ")
        Next
        'remove characters that you don't need
        strNameIn = strNameIn.Replace(",", String.Empty)
        strNameIn = strNameIn.Replace(".", String.Empty)
        strNameIn = strNameIn.Replace("/", String.Empty)

        'split the name on the sapces
        strParts = strNameIn.Split(" "c)
        intParts = strParts.Length
        'This field is used later to identify which parts of strParts
        '	belong in the first name and which in the last name
        '	value of "F" is first name, value of "L" is last name
        ReDim strP1(intParts - 1)

        'if it's an obvious company name,
        '	put it all in the last name field and exit.
        Select Case UCase(strParts(intParts - 1))
            Case "INC", "INCORPORATED", "LLC", "CORP", "CORPORATION", "STORE", "STORES", "CO", "COMPANY", "SHOP", "SHOPS"
                For i = 0 To intParts - 1
                    strLNameOut += strParts(i) & " "
                Next
                nameOut.SetValue(strFNameOut, 0)
                nameOut.SetValue(strLNameOut, 1)
                Return nameOut
        End Select

        'if it's a one word name, it goes into the last name field
        If intParts = 1 Then
            strP1(0) = "L"
        Else
            strP1(0) = "F"
            If intParts = 2 Then
                '2-part names automatically go into first and last
                strP1(1) = "L"
            End If
        End If

        'check for common last name parts and suffixes and
        '	format name accordingly
        If intParts > 2 Then
            'set all parts to first name
            For i = 0 To intParts - 1
                strP1(i) = "F"
            Next
            'search for last name parts and suffixes
            Select Case UCase(strParts(intParts - 1))
                Case "MD", "PHD", "JR", "SR", "I", "II", "III", "IV", "V", "VI", "VII", "VIII"
                    strP1(intParts - 1) = "L"
                    strP1(intParts - 2) = "L"
                    Select Case UCase(strParts(intParts - 3))
                        Case "DEL", "DE", "VAN", "DER"
                            strP1(intParts - 3) = "L"
                    End Select
                Case Else
                    strP1(intParts - 1) = "L"
                    Select Case UCase(strParts(intParts - 2))
                        Case "DEL", "DE", "VAN", "DER"
                            strP1(intParts - 2) = "L"
                    End Select
            End Select
            For i = 0 To intParts - 2
                If UCase(strParts(i)) = "VAN" And UCase(strParts(i + 1)) = "DER" Then
                    For j = i To intParts - 1
                        strP1(j) = "L"
                    Next
                    Exit For
                End If
            Next
        End If
        For i = 0 To intParts - 1
            Select Case strP1(i)
                Case "F"
                    strFNameOut += strParts(i) & " "
                Case "L"
                    strLNameOut += strParts(i) & " "
            End Select
        Next

        nameOut.SetValue(strFNameOut, 0)
        nameOut.SetValue(strLNameOut, 1)

        Return nameOut

    End Function

    Friend Shared Function CreateParticipants(ByVal item As Outlook.MailItem, ByVal ccCount As Integer) As List(Of ParticipantGUIDStruct)
        Dim participants As List(Of ParticipantGUIDStruct) = New List(Of ParticipantGUIDStruct)
        Dim ccName As List(Of ParticipantNameStruct) = New List(Of ParticipantNameStruct)
        Dim counter As Integer = 0

        Try
            If ccCount >= 1 Then
                ccName = GetEmailAddressFromName(item.Recipients, ccCount)
                For Each pair In ccName
                    If Not (pair.PartEmail Is Nothing) Then
                        participants.Add(New ParticipantGUIDStruct(pair.PartName, GetConstituentId(pair.PartEmail.ToString)))
                    End If
                    counter += 1
                Next
            End If

        Catch ex As Exception
            Return participants
        End Try

        Return participants
    End Function

    Friend Shared Function GetEmailAddressFromName(ByVal olNames As Outlook.Recipients, ByVal ccCount As Integer) As List(Of ParticipantNameStruct)
        Dim ccName As New List(Of ParticipantNameStruct)
        Dim counter As Integer = 0
        Dim exUser As Outlook.ExchangeUser

        Try
            If ccCount >= 1 Then
                For Each olRecipient As Outlook.Recipient In olNames
                    If (olRecipient.Type = Outlook.OlMailRecipientType.olCC Or olRecipient.Type = Outlook.OlMailRecipientType.olTo) Then
                        If olRecipient.DisplayType = Outlook.OlDisplayType.olUser Then
                            If olRecipient.Address.Contains("/O=RUEXCHMAIL") Then
                                exUser = olRecipient.AddressEntry.GetExchangeUser()
                                ccName.Add(New ParticipantNameStruct(exUser.Name, exUser.PrimarySmtpAddress))
                            Else
                                ccName.Add(New ParticipantNameStruct(olRecipient.Name, Replace(olRecipient.Address, "@mail.rockefeller.edu", "@rockefeller.edu")))
                            End If
                        End If
                        counter += 1
                    End If
                Next
            End If
        Catch ex As Exception
            'swallow error
        End Try

        Return ccName

    End Function

    Friend Shared Function CollectionToDataFormFieldValue(ByVal inParticipants As List(Of ParticipantGUIDStruct), ByVal CollectionKey As String, ByVal anyConstituent As Guid, ByVal anyOwner As Guid) As Blackbaud.AppFx.XmlTypes.DataForms.DataFormItemArrayValue
        Try
            Dim myParticipants As New List(Of ParticipantGUIDStruct)
            Dim processedrows As New Generic.List(Of Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem)
            Dim count As Integer = inParticipants.Count
            Dim myCounter As Integer = 0
            Dim dfi As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi1 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi2 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi3 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi4 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi5 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi6 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi7 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi8 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi9 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi10 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi11 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi12 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi13 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi14 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi15 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi16 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi17 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi18 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim dfi19 As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
            Dim currGuid As String
            Dim lastGuid As String

            myParticipants = inParticipants
            lastGuid = "00000000-0000-0000-0000-000000000000"

            For Each row In myParticipants
                currGuid = row.PartGuid.ToString
                If Not (String.Equals(currGuid, lastGuid, StringComparison.OrdinalIgnoreCase)) Then
                    If Not (String.Equals(row.PartGuid.ToString, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase)) Then
                        If Not (String.Equals(row.PartGuid.ToString, anyConstituent.ToString, StringComparison.OrdinalIgnoreCase)) Then
                            If Not (String.Equals(row.PartGuid.ToString, anyOwner.ToString, StringComparison.OrdinalIgnoreCase)) Then
                                Select Case myCounter
                                    Case 0
                                        dfi.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi)
                                    Case 1
                                        dfi1.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi1)
                                    Case 2
                                        dfi2.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi2)
                                    Case 3
                                        dfi3.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi3)
                                    Case 4
                                        dfi4.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi4)
                                    Case 5
                                        dfi5.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi5)
                                    Case 6
                                        dfi6.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi6)
                                    Case 7
                                        dfi7.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi7)
                                    Case 8
                                        dfi8.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi8)
                                    Case 9
                                        dfi9.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi9)
                                    Case 10
                                        dfi10.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi10)
                                    Case 11
                                        dfi11.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi11)
                                    Case 12
                                        dfi12.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi12)
                                    Case 13
                                        dfi13.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi13)
                                    Case 14
                                        dfi14.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi14)
                                    Case 15
                                        dfi15.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi15)
                                    Case 16
                                        dfi16.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi16)
                                    Case 17
                                        dfi17.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi17)
                                    Case 18
                                        dfi18.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi18)
                                    Case 19
                                        dfi19.Values.Add("CONSTITUENTID", row.PartGuid, row.PartName)
                                        processedrows.Add(dfi19)
                                End Select
                            End If
                        End If
                    End If
                End If
                myCounter = myCounter + 1
                lastGuid = currGuid
            Next

            Dim dfiav = New DataForms.DataFormItemArrayValue
            dfiav.Items = processedrows.ToArray()
            Return dfiav

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Friend Shared Function BuildNiceHeader(ByVal item As Outlook.Recipients, ByVal item2 As Outlook.MailItem) As String
        Dim exUser As Outlook.ExchangeUser
        Dim exDist As Outlook.ExchangeDistributionList
        Dim niceHeader As String = "Email From: "

        If item2.Sender.DisplayType = Outlook.OlDisplayType.olUser Or item2.Sender.DisplayType = Outlook.OlDisplayType.olRemoteUser Then
            If item2.Sender.Address.ToUpper.Contains("/O=RUEXCHMAIL") Then
                exUser = item2.Sender.GetExchangeUser()
                niceHeader = niceHeader + exUser.Name + " <" + exUser.PrimarySmtpAddress + ">; "
            Else
                niceHeader = niceHeader + item2.SenderName + " <" + item2.SenderEmailAddress + ">; "
            End If
        End If

        niceHeader = niceHeader + vbCrLf + vbCrLf + "Email Sent To: "

        For Each olRecipient As Outlook.Recipient In item
            If (olRecipient.Type = Outlook.OlMailRecipientType.olTo) Then
                If olRecipient.DisplayType = Outlook.OlDisplayType.olUser Or olRecipient.DisplayType = Outlook.OlDisplayType.olRemoteUser Then
                    If olRecipient.Address.Contains("/O=RUEXCHMAIL") Then
                        exUser = olRecipient.AddressEntry.GetExchangeUser()
                        niceHeader = niceHeader + exUser.Name + " <" + exUser.PrimarySmtpAddress + ">; "
                    Else
                        niceHeader = niceHeader + olRecipient.Name + " <" + olRecipient.Address + ">; "
                    End If
                ElseIf olRecipient.DisplayType = Outlook.OlDisplayType.olDistList Then
                    exDist = olRecipient.AddressEntry.GetExchangeDistributionList()
                    niceHeader = niceHeader + exDist.Name + " <" + exDist.PrimarySmtpAddress + ">; "
                End If
            End If
        Next

        niceHeader = niceHeader.Substring(0, niceHeader.Length - 2) + vbCrLf

        niceHeader = niceHeader + vbCrLf + "Email CC:  "

        For Each olRecipient As Outlook.Recipient In item
            If (olRecipient.Type = Outlook.OlMailRecipientType.olCC) Then
                If olRecipient.DisplayType = Outlook.OlDisplayType.olUser Or olRecipient.DisplayType = Outlook.OlDisplayType.olRemoteUser Then
                    If olRecipient.Address.Contains("/O=RUEXCHMAIL") Then
                        exUser = olRecipient.AddressEntry.GetExchangeUser()
                        niceHeader = niceHeader + exUser.Name + " <" + exUser.PrimarySmtpAddress + ">; "
                    Else
                        niceHeader = niceHeader + olRecipient.Name + " <" + olRecipient.Address + ">; "
                    End If
                ElseIf olRecipient.DisplayType = Outlook.OlDisplayType.olDistList Then
                    exDist = olRecipient.AddressEntry.GetExchangeDistributionList()
                    niceHeader = niceHeader + exDist.Name + " <" + exDist.PrimarySmtpAddress + ">; "
                End If
            End If
        Next

        niceHeader = niceHeader.Substring(0, niceHeader.Length - 2) + vbCrLf

        Return niceHeader
    End Function

    Friend Shared Sub HandleException(ByVal msg As String, ByVal ex As Exception)
        MsgBox(String.Format("{0}:  {1}", msg, ex.Message), MsgBoxStyle.Information, applicationTitle)
    End Sub

    Structure ParticipantNameStruct
        Public PartName As String
        Public PartEmail As String

        Public Sub New(ByVal inName As String, ByVal inEmail As String)
            Me.PartName = inName
            Me.PartEmail = inEmail
        End Sub
    End Structure

    Structure ParticipantGUIDStruct
        Public PartName As String
        Public PartGuid As Guid

        Public Sub New(ByVal inName As String, ByVal inGuid As Guid)
            Me.PartName = inName
            Me.PartGuid = inGuid
        End Sub
    End Structure

End Class
