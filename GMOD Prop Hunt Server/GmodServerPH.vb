Option Explicit On

Imports System
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions



Public Class GmodServerPH

    Dim filePath As String = CurDir()
    Dim serverCFG As String = filePath + "\garrysmod\cfg\"
    Dim maxPLYRS As String = "16"   ' use the power of 2 to set max number of players for your server.  No more than 128 players may be allowed in your server at one time.

    Dim selectedMap As String = ""  ' This is how we determin the default map selection.  This should be left blank.
    Dim useLAN As String = "0"  ' This chooses the LAN option.  This should be left default 0 (False).
    Dim usePort As String = "27015" ' This is the default port when using GMOD server. This does not tie to the Steam ports or servers.
    Dim rconPass As String = ""  '  This is for you to set your ownership of your server. You do not need to use one, but it is recommended. 
    Dim serverName As String = "Garry's Mod PH Server"
    Dim gameMode As String = "prop_hunt"
    Dim regionMode As String = "255"
    Dim svLoadingURL As String = ""
    Dim sv_downloadURL As String = ""
    Dim sv_allowdownload As String = "sv_allowdownload 0"
    Dim sv_allowupload As String = "sv_allowupload 0"

    Const quote As String = """"
    Dim capPercent As Double
    '\\ This is for testing //'
    Dim simPlayers As Integer = 1
    Dim dayPlayrs As Integer = 1
    Dim lifePlayers As Integer = 1

    Dim maxProps As Integer = 10
    Dim maxRagDolls As Integer = 10
    Dim maxNPCs As Integer = 10
    Dim maxBallons As Integer = 10
    Dim maxEffects As Integer = 10
    Dim maxDynamics As Integer = 10
    Dim maxLamps As Integer = 10
    Dim maxThrusters As Integer = 10
    Dim maxWheels As Integer = 10
    Dim maxHoverBalls As Integer = 10
    Dim maxVehicles As Integer = 10
    Dim maxButtons As Integer = 10
    Dim maxSents As Integer = 10
    Dim maxEmitters As Integer = 10
    Dim maxSpawners As Integer = 10
    Dim maxTurrets As Integer = 10

    Dim start_info As New ProcessStartInfo()
    Dim client As WebClient = New WebClient

    Private WithEvents MyProcess As Process
    Public Delegate Sub AppendOutputTextDelegate(ByVal text As String)



    Private Sub GmodServerPH_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        playersperday_lbl.Text = "0"
        playersinlifetime_lbl.Text = "0"
        Timer1.Start()

        If File.Exists(filePath & "srcds.exe") = True Then
            My.Computer.FileSystem.RenameFile(filePath & "srcds.exe", "gmodserver.exe")
        Else
            'return null
        End If


        sendCommand_btn.Enabled = False

        If Directory.Exists("C:\inetpub\wwwroot2") = False Then
            My.Computer.FileSystem.CreateDirectory("C:\inetpub\wwwroot2")
            Using addInfo = File.CreateText("C:\inetpub\wwwroot2\gmodVersion.txt")
                addInfo.WriteLine(Application.ProductVersion)
            End Using
        Else
            Using addInfo = File.CreateText("C:\inetpub\wwwroot2\gmodVersion.txt")
                addInfo.WriteLine(Application.ProductVersion)
            End Using
        End If

        Try
            If File.Exists(filePath & "/GMODDedicatedServerUpdate.exe") = True Then
                File.Delete(filePath & "/GMODDedicatedServerUpdate.exe")
            Else
                '
            End If
        Catch ex As Exception
            'Ignore this case as it may fail due to permission level.
        End Try





        sendCommand_btn.Enabled = False

        Try
            Dim strPath As String = System.IO.Path.GetDirectoryName(Application.ExecutablePath) & "\garrysmod\maps"
            Dim dirInfo As New IO.DirectoryInfo(strPath)

            For Each file As FileInfo In dirInfo.GetFiles("*.bsp", SearchOption.TopDirectoryOnly)
                selectedMap_cbx.Items.Add(file.Name)
            Next

            With My.Settings
                svrName_txt.Text = .ServerName
                selectedMap_cbx.SelectedValue = .StartupMap
                gMode_cbx.SelectedValue = .GameMode
                playersMAX_cbx.SelectedValue = .maxPlayers
                portsText_tbx.Text = .portnumber
                rPass_txt.Text = .RCONPassword
                regionList_cbx.SelectedValue = .Region
                lan_chbx.Checked = .LAN
                mProps_txt.Text = .mPropss
                mRagDolls_txt.Text = .mRagss
                mNPCs_txt.Text = .mNPCss
                mBallons_txt.Text = .mBallss
                mEffects_txt.Text = .mEffss
                mDynamics_txt.Text = .mDynss
                mLamps_txt.Text = .mLampss
                mThrusters_txt.Text = .mThrusts
                mWheels_txt.Text = .mWheess
                mHoverballs_txt.Text = .mHovess
                mVehicles_txt.Text = .mVehiss
                mButtons_txt.Text = .mButtss
                mSents_txt.Text = .mSentss
                mEmitters_txt.Text = .mEmitss
                gmodloadingURL_txt.Text = .LoadingURLss
                downloadMaps_txt.Text = .DownloadURLss
                'mSpawners_txt.Text =.mSpawnss(Depreciated)
                'mTurrets_txt.Text =.mTurrss  (Depreciated)

            End With
            mProps_txt.Text = maxProps
            mRagDolls_txt.Text = maxRagDolls
            mNPCs_txt.Text = maxNPCs
            mBallons_txt.Text = maxBallons
            mEffects_txt.Text = maxEffects
            mDynamics_txt.Text = maxDynamics
            mLamps_txt.Text = maxLamps
            mThrusters_txt.Text = maxThrusters
            mWheels_txt.Text = maxWheels
            mHoverballs_txt.Text = maxHoverBalls
            mVehicles_txt.Text = maxVehicles
            mButtons_txt.Text = maxButtons
            mSents_txt.Text = maxSents
            mEmitters_txt.Text = maxEmitters
            mSpawners_txt.Text = maxSpawners
            mTurrets_txt.Text = maxTurrets

        Catch ex As Exception
            MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Try
            Dim i As Integer = 0
            For i = 2 To 128
                playersMAX_cbx.Items.Add(i)
            Next
        Catch ex As Exception

        End Try
        selectedMap_cbx.SelectedIndex = 0
        playersMAX_cbx.SelectedIndex = 14
        gMode_cbx.SelectedIndex = 0
        regionList_cbx.SelectedIndex = 0
        portsText_tbx.Text = usePort
        'CapacityBar1.Maximum = maxPLYRS

        Call AppCheck()

    End Sub
    Public Sub beginSrvr_btn_Click(sender As Object, e As EventArgs) Handles beginSrvr_btn.Click

        beginSrvr_btn.Enabled = False
        closeSrvr_btn.Enabled = True
        resetServer_btn.Enabled = True

        ' Server Properties
        svrName_txt.Enabled = False
        selectedMap_cbx.Enabled = False
        portsText_tbx.Enabled = False
        playersMAX_cbx.Enabled = False
        regionList_cbx.Enabled = False
        lan_chbx.Enabled = False
        gMode_cbx.Enabled = False
        rPass_txt.Enabled = False
        mLamps_txt.Enabled = False
        mProps_txt.Enabled = False
        mRagDolls_txt.Enabled = False
        mNPCs_txt.Enabled = False
        mBallons_txt.Enabled = False
        mEffects_txt.Enabled = False
        mDynamics_txt.Enabled = False
        mThrusters_txt.Enabled = False
        mWheels_txt.Enabled = False
        mHoverballs_txt.Enabled = False
        mVehicles_txt.Enabled = False
        mButtons_txt.Enabled = False
        mSents_txt.Enabled = False
        mEmitters_txt.Enabled = False
        mSpawners_txt.Enabled = False
        mTurrets_txt.Enabled = False
        gmodloadingURL_txt.Enabled = False



        Try

            Using addInfo = File.CreateText(filePath + "\garrysmod\cfg\server.cfg")

                addInfo.WriteLine("Application version: " & Me.ProductVersion)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Server info generated by BLOODED CYBORG.")
                addInfo.WriteLine("log on")
                addInfo.WriteLine("")
                addInfo.WriteLine("hostname " & serverName)
                addInfo.WriteLine("rcon_password " & rconPass)
                addInfo.WriteLine("maxplayers " & maxPLYRS)
                addInfo.WriteLine("gamemode " & gameMode)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Internet, region, lobby setup")
                addInfo.WriteLine("hostport " & usePort)
                addInfo.WriteLine("sv_lan " & useLAN)
                addInfo.WriteLine("sv_region " & regionMode)
                addInfo.WriteLine(svLoadingURL)
                addInfo.WriteLine(sv_downloadURL)
                addInfo.WriteLine(sv_allowdownload)
                addInfo.WriteLine(sv_allowupload)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Game Settings")
                addInfo.WriteLine("")
                addInfo.WriteLine("sbox_godmode		0")
                addInfo.WriteLine("sbox_noclip		1")
                addInfo.WriteLine("")
                addInfo.WriteLine("sbox_maxprops " & maxProps)
                addInfo.WriteLine("sbox_maxragdolls	" & maxRagDolls)
                addInfo.WriteLine("sbox_maxnpcs " & maxNPCs)
                addInfo.WriteLine("sbox_maxballoons	" & maxBallons)
                addInfo.WriteLine("sbox_maxeffects " & maxEffects)
                addInfo.WriteLine("sbox_maxdynamite	" & maxDynamics)
                addInfo.WriteLine("sbox_maxlamps " & maxLamps)
                addInfo.WriteLine("sbox_maxthrusters " & maxThrusters)
                addInfo.WriteLine("sbox_maxwheels " & maxWheels)
                addInfo.WriteLine("sbox_maxhoverballs " & maxHoverBalls)
                addInfo.WriteLine("sbox_maxvehicles " & maxVehicles)
                addInfo.WriteLine("sbox_maxbuttons " & maxButtons)
                addInfo.WriteLine("sbox_maxsents " & maxSents)
                addInfo.WriteLine("sbox_maxemitters	" & maxEmitters)
                addInfo.WriteLine("sbox_maxspawners	" & maxSpawners)
                addInfo.WriteLine("sbox_maxturrets " & maxTurrets)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Network Tweaks - Increase network performance")
                addInfo.WriteLine("")
                addInfo.WriteLine("rate 10000")       '   //default 10000; Max bytes/sec the host can recieve data")
                addInfo.WriteLine("sv_minrate 15000") '   //default "5000"; Min bandwidth rate allowed on server, 0 = unlimited")
                addInfo.WriteLine("sv_maxrate 30000") '   //default "0";  Max bandwidth rate allowed On server, 0 = unlimited")
                addInfo.WriteLine("sv_mincmdrate 20") '   //Default 0; This sets the minimum value For cl_cmdrate. 0 = unlimited [cevo=67]")
                addInfo.WriteLine("sv_maxcmdrate 33") '   //Default 40; (If sv_mincmdrate Is > 0), this sets the maximum value For cl_cmdrate. [cevo= 101]")
                addInfo.WriteLine("decalfrequency 10")
                addInfo.WriteLine("net_maxfilesize 100")
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Bans")
                addInfo.WriteLine("//  execute banned.cfgs at server start. Optimally at launch commandline.")
                addInfo.WriteLine("//  Put writeip/wrtieid commands in the bottom of server.cfg AFTER banned.cfgs have loaded.")
                addInfo.WriteLine("")
                addInfo.WriteLine("exec banned_user.cfg ")      '//loads banned users' ids
                addInfo.WriteLine("exec banned_ip.cfg   ")      '//loads banned users' ips
                addInfo.WriteLine("writeip              ")      '//Save the ban list to banned_ip.cfg.
                addInfo.WriteLine("writeid              ")      '//Writes a list of permanently-banned user IDs to banned_user.cfg.
                addInfo.WriteLine("")
                addInfo.WriteLine("")
                addInfo.WriteLine("")

                ' addInfo.WriteLine()  'new addline template
            End Using


        Catch ex As Exception

        End Try


        If selectedMap = "" Then
            MessageBox.Show("No map was selected. Please Select a map To start.", "BSP Error.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        Else
            If portsText_tbx.Text > "65535" Then
                MessageBox.Show("Please use a valid port number. Using 27015 instead.", "Invalid port.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                usePort = "27015"
                portsText_tbx.Text = "27015"
            Else
                Try

                    sendCommand_btn.Enabled = True


                    Dim myProcess As Process = New Process
                    myProcess.StartInfo.FileName = "gmodserver"
                    myProcess.StartInfo.Arguments = ("-console -game garrysmod +map " & selectedMap & " +gamemode " & gameMode & " +maxplayers " & maxPLYRS)
                    myProcess.StartInfo.UseShellExecute = False
                    myProcess.StartInfo.RedirectStandardOutput = True
                    myProcess.StartInfo.RedirectStandardError = True
                    myProcess.StartInfo.CreateNoWindow = True
                    myProcess.EnableRaisingEvents = True
                    AddHandler myProcess.OutputDataReceived, AddressOf GotData
                    myProcess.Start()
                    myProcess.BeginOutputReadLine()

                Catch ex As Exception
                    MessageBox.Show(ex.Message & vbCrLf & "Either " & quote & "srcds.exe" & quote & " is not renamed as gmodserver, or " & quote & "gmodserver.exe" & quote & " is missing.", ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)

                End Try
            End If


        End If
    End Sub

    '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\  Textbox Output Console ////////////////////////////////////////////////////////////////////////
    Private Sub OutputTextBox_TextChanged(sender As Object, e As EventArgs) Handles OutputTextBox.TextChanged

        Dim sLines() As String, L As Long

        sLines = Split(OutputTextBox.Text, vbCrLf)

        Dim LastLine() As String

        LastLine = OutputTextBox.Lines

        LastLineViewer_lbl.Text = LastLine.Length

        OutputTextBox.Text += ""
        OutputTextBox.Select(OutputTextBox.Text.Length, 0)
        OutputTextBox.ScrollToCaret()


    End Sub

    Private Sub AppendOutputText(ByVal text As String)
        If OutputTextBox.InvokeRequired Then
            Dim myDelegate As New AppendOutputTextDelegate(AddressOf AppendOutputText)
            Me.Invoke(myDelegate, text)
        Else
            OutputTextBox.ScrollToCaret()
            OutputTextBox.AppendText(text)
        End If

    End Sub

    Private Sub GotData(sendingProcess As Object, outLine As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(outLine.Data) Then
            SetText(outLine.Data)
        End If
    End Sub
    Delegate Sub SetTextCallback(value As String)
    Private Sub SetText(ByVal value As String)

        If Me.OutputTextBox.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf SetText)
            Me.Invoke(d, New Object() {value})
        Else
            Label30.Text = value
            If value.Contains("STEAM USERID validated") Then
                Dim i As Integer
                Dim ti As Integer

                If Integer.TryParse(onlinePlayers_lbl.Text, i) Then
                    i += 1
                    simPlayers = i.ToString
                    If simPlayers > maxPLYRS Then
                        ' MessageBox.Show("The server is full.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        i -= 1
                        simPlayers = i.ToString
                        playersperday_lbl.Text = i.ToString
                        'playersinlifetime_lbl.Text = i.ToString
                    Else

                    End If
                End If

                If Integer.TryParse(playersperday_lbl.Text, ti) Then
                    ti += 1
                    dayPlayrs = ti.ToString
                    lifePlayers = ti.ToString

                End If

                onlinePlayers_lbl.Text = simPlayers
                CapacityBar1.Value = simPlayers

                playersperday_lbl.Text = dayPlayrs
                playersinlifetime_lbl.Text = lifePlayers
                'playersperday_lbl.Text += 1
                'playersinlifetime_lbl.Text += 1

                capPercent = simPlayers / maxPLYRS * 100

                If capPercent >= 75 And capPercent <= 99 Then
                    CapacityBar1.ForeColor = Color.Gold
                ElseIf capPercent >= 100 Then
                    CapacityBar1.ForeColor = Color.Red
                Else 'This is the general default color.
                    CapacityBar1.ForeColor = Color.Lime

                End If

            End If

            If value.Contains("disconnected") Then
                Dim i As Integer

                If Integer.TryParse(onlinePlayers_lbl.Text, i) Then
                    i -= 1
                    simPlayers = i.ToString
                    If simPlayers < 0 Then
                        ' MessageBox.Show("The server is Empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        i += 1
                        simPlayers = i.ToString
                    Else

                    End If
                End If
                onlinePlayers_lbl.Text = simPlayers
                CapacityBar1.Value = simPlayers
                capPercent = simPlayers / maxPLYRS * 100

                If capPercent >= 75 And capPercent <= 99 Then
                    CapacityBar1.ForeColor = Color.Gold
                ElseIf capPercent >= 100 Then
                    CapacityBar1.ForeColor = Color.Red
                Else 'This is the general default color.
                    CapacityBar1.ForeColor = Color.Lime

                End If
            End If

            'Started map "ph_motel_blacke_v3" (CRC "841c05c83d6986e7579c2049382cd87d")
            ' Dim strPath As String = System.IO.Path.GetDirectoryName(Application.ExecutablePath) & "\garrysmod\maps"
            ' Dim dirInfo As New IO.DirectoryInfo(strPath)
            '


            If value.Contains("Started Map ") Then
                If value.Contains(selectedMap) Then
                    showMap_lbl.Text = selectedMap
                End If
            End If

            If value.Contains("") Then

                If value.Contains("---- Host_Changelevel ----") Then

                    OutputTextBox.Text = ""

                End If
            End If
            Me.OutputTextBox.Text += value + Environment.NewLine
        End If



    End Sub

    '|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectedMap_cbx.SelectedIndexChanged
        Dim map As String = selectedMap_cbx.SelectedItem

        selectedMap = map
        showMap_lbl.Text = selectedMap

    End Sub
    Private Sub lan_chbx_CheckedChanged(sender As Object, e As EventArgs) Handles lan_chbx.CheckedChanged

        If lan_chbx.Checked = True Then
            useLAN = "1"
        Else
            useLAN = "0"
        End If

    End Sub
    Private Sub portsText_tbx_TextChanged(sender As Object, e As EventArgs) Handles portsText_tbx.TextChanged
        If portsText_tbx.Text = "" Or portsText_tbx.Text = "0" Then
            usePort = "27015"
            portsText_tbx.Text = "27015"
        ElseIf portsText_tbx.Text > "65535" Then
            '
        Else
            usePort = portsText_tbx.Text
        End If
    End Sub
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles playersMAX_cbx.SelectedIndexChanged
        Dim players As String = playersMAX_cbx.SelectedItem
        maxPLYRS = players
        maxPlyrs_lbl.Text = maxPLYRS
        'CapacityBar1.Value = 0
        'simPlayers = "0"
        CapacityBar1.Maximum = maxPLYRS

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles rPass_txt.TextChanged
        If rPass_txt.Text = "" Then
            rconPass = quote
        Else
            rconPass = rPass_txt.Text
        End If
    End Sub
    Private Sub svrName_txt_TextChanged(sender As Object, e As EventArgs) Handles svrName_txt.TextChanged
        If svrName_txt.Text = "" Then
            'MessageBox.Show("Your server name must not be blank.", "Server name error.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            svrName_txt.Text = "Garry's Mod Live Server"
        Else
            serverName = svrName_txt.Text

        End If
    End Sub
    Private Sub gMode_cbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gMode_cbx.SelectedIndexChanged
        Dim mode As String = gMode_cbx.SelectedIndex

        Select Case mode
            Case 0
                gameMode = "sandbox"    ' Sandbox.
            Case 1
                gameMode = "prop_hunt"  ' Prop Hunt.
            Case 2
                gameMode = "terrortown" ' Trouble in Terrorist Town.
                'Case 3
                '    gameMode = ""

        End Select




    End Sub

    '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ Key Presses ////////////////////////////////////////////////////////////////////////////////////////////
    Private Sub portsText_tbx_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles portsText_tbx.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub playersMAX_cbx_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles playersMAX_cbx.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub rPass_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rPass_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mProps_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mProps_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mRagDolls_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mRagDolls_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mNPCs_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mNPCs_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mBallons_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mBallons_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mEffects_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mEffects_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mDynamics_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mDynamics_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mLamps_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mLamps_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mThrusters_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mThrusters_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mHoverballs_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mHoverballs_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mVehicles_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mVehicles_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mButtons_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mButtons_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mSents_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mSents_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mEmitters_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mEmitters_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mSpawners_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mSpawners_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mTurrets_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mTurrets_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub mWheels_txt_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mWheels_txt.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    '------------------------------------------------------------ Process Control --------------------------------------------------------------------------------------
    Private Sub sendCommand_btn_Click(sender As Object, e As EventArgs) Handles sendCommand_btn.Click
        Try
            'Dim myProcess As Process = New Process
            Process.GetProcessesByName("gmodserver")
            For Each P In System.Diagnostics.Process.GetProcessesByName("gmodserver")
                'P.(InputTextBox.Text)

            Next

            'Process.WriteLine(InputTextBox.Text)

            'MyProcess.StandardInput.WriteLine(InputTextBox.Text)

            'MyProcess.I
            'MyProcess.StandardInput.
            'MyProcess.StandardInput.Flush()

        Catch ex As Exception
            MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        InputTextBox.Text = ""
    End Sub
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Dim processa As System.Diagnostics.Process = Nothing
        Dim psi As New ProcessStartInfo

        If Process.GetProcessesByName("gmodserver").Length <> 0 Then
            For Each P In System.Diagnostics.Process.GetProcessesByName("gmodserver")
                Try
                    Dim resp As MsgBoxResult

                    resp = MsgBox("Are you sure you want to close " & P.ProcessName & "?", MsgBoxStyle.YesNo, "Terminate gmodserver.exe?")

                    If resp = MsgBoxResult.Yes Then
                        closeSrvr_btn.Enabled = False
                        resetServer_btn.Enabled = False
                        P.Kill()
                        P.WaitForExit()

                    End If
                Catch Win32Exception As Exception
                    MsgBox(Win32Exception.Message, MsgBoxStyle.Critical)

                End Try

            Next
            MyProcess.Close()
        End If

    End Sub


    Private Sub closeSrvr_btn_Click(sender As Object, e As EventArgs) Handles closeSrvr_btn.Click
        onlinePlayers_lbl.Text = 0
        CapacityBar1.Value = 0




        Dim processa As System.Diagnostics.Process = Nothing
        Dim psi As New ProcessStartInfo

        If Process.GetProcessesByName("gmodserver").Length <> 0 Then
            For Each P In System.Diagnostics.Process.GetProcessesByName("gmodserver")
                Try
                    Dim resp As MsgBoxResult

                    resp = MsgBox("Terminate " & P.ProcessName & "?", MsgBoxStyle.YesNo, "Terminate gmodserver.exe?")

                    If resp = MsgBoxResult.Yes Then
                        beginSrvr_btn.Enabled = True
                        closeSrvr_btn.Enabled = False
                        resetServer_btn.Enabled = False
                        P.Kill()
                        P.WaitForExit()
                        OutputTextBox.Text = ""
                        playersperday_lbl.Text = "0"
                        playersinlifetime_lbl.Text = "0"
                    End If
                Catch Win32Exception As Exception
                    MsgBox(Win32Exception.Message, MsgBoxStyle.Critical)

                End Try

            Next
            resetServer_btn.Enabled = True
            closeSrvr_btn.Enabled = True
        End If

        beginSrvr_btn.Enabled = True
        closeSrvr_btn.Enabled = False
        resetServer_btn.Enabled = False


        svrName_txt.Enabled = True
        selectedMap_cbx.Enabled = True
        portsText_tbx.Enabled = True
        playersMAX_cbx.Enabled = True
        regionList_cbx.Enabled = True
        lan_chbx.Enabled = True
        gMode_cbx.Enabled = True
        rPass_txt.Enabled = True
        mLamps_txt.Enabled = True
        mProps_txt.Enabled = True
        mRagDolls_txt.Enabled = True
        mNPCs_txt.Enabled = True
        mBallons_txt.Enabled = True
        mEffects_txt.Enabled = True
        mDynamics_txt.Enabled = True
        mThrusters_txt.Enabled = True
        mWheels_txt.Enabled = True
        mHoverballs_txt.Enabled = True
        mVehicles_txt.Enabled = True
        mButtons_txt.Enabled = True
        mSents_txt.Enabled = True
        mEmitters_txt.Enabled = True
        mSpawners_txt.Enabled = True
        mTurrets_txt.Enabled = True
        gmodloadingURL_txt.Enabled = True


    End Sub
    Private Sub resetServer_btn_Click(sender As Object, e As EventArgs) Handles resetServer_btn.Click
        onlinePlayers_lbl.Text = 0
        CapacityBar1.Value = 0

        resetServer_btn.Enabled = False
        sendCommand_btn.Enabled = False

        Dim processa As System.Diagnostics.Process = Nothing
        Dim psi As New ProcessStartInfo

        If Process.GetProcessesByName("gmodserver").Length <> 0 Then
            For Each P In System.Diagnostics.Process.GetProcessesByName("gmodserver")
                Try
                    closeSrvr_btn.Enabled = False
                    P.Kill()
                    P.WaitForExit()
                    OutputTextBox.Text = ""
                    playersperday_lbl.Text = "0"
                    playersinlifetime_lbl.Text = "0"
                Catch Win32Exception As Exception
                    MsgBox(Win32Exception.Message, MsgBoxStyle.Critical)

                End Try

            Next
            beginSrvr_btn.Enabled = False
            closeSrvr_btn.Enabled = False
            resetServer_btn.Enabled = False
            Try
                selectedMap_cbx.Enabled = False
                portsText_tbx.Enabled = False
                lan_chbx.Enabled = False
                playersMAX_cbx.Enabled = False



                Dim myProcess As Process = New Process
                myProcess.StartInfo.FileName = "gmodserver"
                myProcess.StartInfo.Arguments = ("-console -game garrysmod +map " & selectedMap & " +gamemode " & gameMode & " +maxplayers " & maxPLYRS)
                myProcess.StartInfo.UseShellExecute = False
                myProcess.StartInfo.RedirectStandardOutput = True
                myProcess.StartInfo.RedirectStandardError = True
                myProcess.StartInfo.CreateNoWindow = True
                myProcess.EnableRaisingEvents = True
                AddHandler myProcess.OutputDataReceived, AddressOf GotData
                myProcess.Start()
                myProcess.BeginOutputReadLine()

            Catch ex As Exception
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)

            End Try
            beginSrvr_btn.Enabled = False
            closeSrvr_btn.Enabled = True
            resetServer_btn.Enabled = True
            sendCommand_btn.Enabled = True

        End If




    End Sub

    Private Sub regionList_cbx_SelectedIndexChanged(sender As Object, e As EventArgs) Handles regionList_cbx.SelectedIndexChanged
        Dim regionList As String = regionList_cbx.SelectedIndex

        Select Case regionList
            Case 0
                regionMode = "255" ' World (default)
            Case 1
                regionMode = "0"   ' US - EAST
            Case 2
                regionMode = "1"   ' US - WEST
            Case 3
                regionMode = "2"   ' South America
            Case 4
                regionMode = "3"   ' Europe
            Case 5
                regionMode = "4"   ' Asia
            Case 6
                regionMode = "5"   ' Australlia
            Case 7
                regionMode = "6"   ' Middle East
            Case 8
                regionMode = "7"   ' Africa                
        End Select
    End Sub
    Private Sub mProps_txt_TextChanged(sender As Object, e As EventArgs) Handles mProps_txt.TextChanged
        If mProps_txt.Text = "" Or mProps_txt.Text < 0 Then
            mProps_txt.Text = 0
            maxProps = "0"
        Else
            maxProps = mProps_txt.Text
        End If
    End Sub
    Private Sub mRagDolls_txt_TextChanged(sender As Object, e As EventArgs) Handles mRagDolls_txt.TextChanged
        If mRagDolls_txt.Text = "" Or mRagDolls_txt.Text < 0 Then
            mRagDolls_txt.Text = 0
            maxRagDolls = "0"
        Else
            maxRagDolls = mRagDolls_txt.Text
        End If
    End Sub
    Private Sub mNPCs_txt_TextChanged(sender As Object, e As EventArgs) Handles mNPCs_txt.TextChanged
        If mNPCs_txt.Text = "" Or mNPCs_txt.Text < 0 Then
            mNPCs_txt.Text = 0
            maxNPCs = "0"
        Else
            maxNPCs = mNPCs_txt.Text
        End If
    End Sub
    Private Sub mBallons_txt_TextChanged(sender As Object, e As EventArgs) Handles mBallons_txt.TextChanged
        If mBallons_txt.Text = "" Or mBallons_txt.Text < 0 Then
            mBallons_txt.Text = 0
            maxBallons = "0"
        Else
            maxBallons = mBallons_txt.Text
        End If
    End Sub
    Private Sub mEffects_txt_TextChanged(sender As Object, e As EventArgs) Handles mEffects_txt.TextChanged
        If mEffects_txt.Text = "" Or mEffects_txt.Text < 0 Then
            mEffects_txt.Text = 0
            maxEffects = "0"
        Else
            maxEffects = mEffects_txt.Text
        End If
    End Sub
    Private Sub mDynamics_txt_TextChanged(sender As Object, e As EventArgs) Handles mDynamics_txt.TextChanged
        If mDynamics_txt.Text = "" Or mDynamics_txt.Text < 0 Then
            mDynamics_txt.Text = 0
            maxDynamics = "0"
        Else
            maxDynamics = mDynamics_txt.Text
        End If
    End Sub
    Private Sub mLamps_txt_TextChanged(sender As Object, e As EventArgs) Handles mLamps_txt.TextChanged
        If mLamps_txt.Text = "" Or mLamps_txt.Text < 0 Then
            mLamps_txt.Text = 0
            maxLamps = "0"
        Else
            maxLamps = mLamps_txt.Text
        End If
    End Sub
    Private Sub mThrusters_txt_TextChanged(sender As Object, e As EventArgs) Handles mThrusters_txt.TextChanged
        If mThrusters_txt.Text = "" Or mThrusters_txt.Text < 0 Then
            mThrusters_txt.Text = 0
            maxThrusters = "0"
        Else
            maxThrusters = mThrusters_txt.Text
        End If
    End Sub
    Private Sub mWheels_txt_TextChanged(sender As Object, e As EventArgs) Handles mWheels_txt.TextChanged
        If mWheels_txt.Text = "" Or mWheels_txt.Text < 0 Then
            mWheels_txt.Text = 0
            maxWheels = "0"
        Else
            maxWheels = mWheels_txt.Text
        End If
    End Sub
    Private Sub mHoverballs_txt_TextChanged(sender As Object, e As EventArgs) Handles mHoverballs_txt.TextChanged
        If mHoverballs_txt.Text = "" Or mHoverballs_txt.Text < 0 Then
            mHoverballs_txt.Text = 0
            maxHoverBalls = "0"
        Else
            maxHoverBalls = mHoverballs_txt.Text
        End If
    End Sub
    Private Sub mVehicles_txt_TextChanged(sender As Object, e As EventArgs) Handles mVehicles_txt.TextChanged
        If mVehicles_txt.Text = "" Or mVehicles_txt.Text < 0 Then
            mVehicles_txt.Text = 0
            maxVehicles = "0"
        Else
            maxVehicles = mVehicles_txt.Text
        End If
    End Sub
    Private Sub mButtons_txt_TextChanged(sender As Object, e As EventArgs) Handles mButtons_txt.TextChanged
        If mButtons_txt.Text = "" Or mButtons_txt.Text < 0 Then
            mButtons_txt.Text = 0
            maxButtons = "0"
        Else
            maxButtons = mButtons_txt.Text
        End If
    End Sub
    Private Sub mSents_txt_TextChanged(sender As Object, e As EventArgs) Handles mSents_txt.TextChanged
        If mSents_txt.Text = "" Or mSents_txt.Text < 0 Then
            mSents_txt.Text = 0
            maxSents = "0"
        Else
            maxSents = mSents_txt.Text
        End If
    End Sub
    Private Sub mEmitters_txt_TextChanged(sender As Object, e As EventArgs) Handles mEmitters_txt.TextChanged
        If mEmitters_txt.Text = "" Or mEmitters_txt.Text < 0 Then
            mEmitters_txt.Text = 0
            maxEmitters = "0"
        Else
            maxEmitters = mEmitters_txt.Text
        End If
    End Sub
    Private Sub mSpawners_txt_TextChanged(sender As Object, e As EventArgs) Handles mSpawners_txt.TextChanged
        If mSpawners_txt.Text = "" Or mSpawners_txt.Text < 0 Then
            mSpawners_txt.Text = 0
            maxSpawners = "0"
        Else
            maxSpawners = mProps_txt.Text
        End If
    End Sub
    Private Sub mTurrets_txt_TextChanged(sender As Object, e As EventArgs) Handles mTurrets_txt.TextChanged
        If mTurrets_txt.Text = "" Or mTurrets_txt.Text < 0 Then
            mTurrets_txt.Text = 0
            maxTurrets = "0"
        Else
            maxTurrets = mTurrets_txt.Text
        End If
    End Sub


    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click

        Try
            My.Settings.ServerName = svrName_txt.Text
            My.Settings.StartupMap = selectedMap_cbx.SelectedValue
            My.Settings.GameMode = gMode_cbx.SelectedValue
            My.Settings.maxPlayers = playersMAX_cbx.SelectedValue
            My.Settings.portnumber = portsText_tbx.Text
            My.Settings.RCONPassword = rPass_txt.Text
            My.Settings.Region = regionList_cbx.SelectedValue
            My.Settings.LAN = lan_chbx.Checked
            My.Settings.mPropss = mProps_txt.Text
            My.Settings.mRagss = mRagDolls_txt.Text
            My.Settings.mNPCss = mNPCs_txt.Text
            My.Settings.mBallss = mBallons_txt.Text
            My.Settings.mEffss = mEffects_txt.Text
            My.Settings.mDynss = mDynamics_txt.Text
            My.Settings.mLampss = mLamps_txt.Text
            My.Settings.mThrusts = mThrusters_txt.Text
            My.Settings.mWheess = mWheels_txt.Text
            My.Settings.mHovess = mHoverballs_txt.Text
            My.Settings.mVehiss = mVehicles_txt.Text
            My.Settings.mButtss = mButtons_txt.Text
            My.Settings.mSentss = mSents_txt.Text
            My.Settings.mEmitss = mEmitters_txt.Text
            My.Settings.LoadingURLss = gmodloadingURL_txt.Text
            My.Settings.DownloadURLss = downloadMaps_txt.Text
            'My.Settings.mSpawnss = mSpawners_txt.Text  (Depreciated)
            'My.Settings.mTurrss = mTurrets_txt.Text    (Depreciated)
            My.Settings.Save()
            MessageBox.Show("Settings have been saved.", "Save Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
    Private Sub ExitToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem1.Click
        Try
            MyProcess.StandardInput.Flush()
            MyProcess.Close()
        Catch ex As Exception
            'Ignore
        End Try
        Me.Close()
    End Sub
    Private Sub RestoreDefaultsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestoreDefaultsToolStripMenuItem.Click
        maxProps = 10
        maxRagDolls = 10
        maxNPCs = 10
        maxBallons = 10
        maxEffects = 10
        maxDynamics = 10
        maxLamps = 10
        maxThrusters = 10
        maxWheels = 10
        maxHoverBalls = 10
        maxVehicles = 10
        maxButtons = 10
        maxSents = 10
        maxEmitters = 10
        maxSpawners = 10
        maxTurrets = 10
        usePort = "27015"
        serverName = "Garry's Mod Live Server"

        mProps_txt.Text = maxProps
        mRagDolls_txt.Text = maxRagDolls
        mNPCs_txt.Text = maxNPCs
        mBallons_txt.Text = maxBallons
        mEffects_txt.Text = maxEffects
        mDynamics_txt.Text = maxDynamics
        mLamps_txt.Text = maxLamps
        mThrusters_txt.Text = maxThrusters
        mWheels_txt.Text = maxWheels
        mHoverballs_txt.Text = maxHoverBalls
        mVehicles_txt.Text = maxVehicles
        mButtons_txt.Text = maxButtons
        mSents_txt.Text = maxSents
        mEmitters_txt.Text = maxEmitters
        mSpawners_txt.Text = maxSpawners
        mTurrets_txt.Text = maxTurrets

        playersMAX_cbx.SelectedIndex = 14
        gMode_cbx.SelectedIndex = 0
        regionList_cbx.SelectedIndex = 0
        portsText_tbx.Text = usePort
        svrName_txt.Text = serverName
        lan_chbx.Checked = False
        selectedMap_cbx.SelectedIndex = 0


    End Sub

    '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\  Application Update Checker //////////////////////////////////////////////////////////
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        weekDay_lbl.Text = WeekdayName(Weekday(Now))

        time_lbl.Text = TimeOfDay.ToString("h:mm:ss tt")
        'weekDay_lbl.Text = intDow
        If weekDay_lbl.Text = "Wednesday" Then
            If TimeOfDay.ToString("h:mm:ss tt") = "1:30:00 AM" Then
                Timer1.Stop()
                Try
                    'Process.Start("J:\Scheduled Tasks\Phantasy Star Online 2\pso2_bin\pso2launcher.exe")
                    'System.Threading.Thread.Sleep(1000)
                    Call AppCheck()
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
                Timer1.Start()
            End If
        End If

        If TimeOfDay.ToString("h:mm:ss tt") = "12:00:00 AM" Then
            Try

                Timer1.Stop()
                playersperday_lbl.Text = onlinePlayers_lbl.Text
            Catch ex As Exception

            End Try
            Timer1.Start()
        End If


    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CheckForUpdatesToolStripMenuItem1.Click
        Call UpdateChecker()
    End Sub

    Public Sub AppCheck()
        Dim web As New WebClient
        Dim ans As String
        Dim LatestAppVersion As String = web.DownloadString("http://24.210.18.72:64080/gmodversion.txt")
        Dim curVersion As String = Application.ProductVersion & vbCrLf
        Try

            If curVersion <> LatestAppVersion Then
                ans = MessageBox.Show("A newer version of the GMOD Dedicated Server is available for download." & vbCrLf & vbCrLf & "Current Version: " & curVersion & vbCrLf & "Latest Version: " & LatestAppVersion, "New update available.", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If ans = vbYes Then
                    Call DownloadUpdater()
                Else
                    '
                End If
            Else
                'Null for first time run.
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & "Could not check for available updates.", ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try


    End Sub

    Public Sub UpdateChecker() 'Check the server while hosting to prevent interuptions.
        Dim web As New WebClient
        Dim ans As String
        Dim LatestAppVersion As String = web.DownloadString("http://24.210.18.72:64080/gmodversion.txt")
        Dim curVersion As String = Application.ProductVersion & vbCrLf
        Try

            If curVersion <> LatestAppVersion Then
                ans = MessageBox.Show("A newer version of the GMOD Dedicated Server is available for download." & vbCrLf & vbCrLf & "Current Version: " & curVersion & vbCrLf & "Latest Version: " & LatestAppVersion, "New update available.", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If ans = vbYes Then
                    Call DownloadUpdater()
                Else
                    '
                End If
            Else
                MessageBox.Show("No new updates.  Please check back later.", "No Updates.", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & vbCrLf & "Could not check for available updates.", ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub
    Sub DownloadUpdater()


        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted

        'MsgBox("There is a new update available and will begin to download.")

        Try

            client.DownloadFileAsync(New Uri("http://24.210.18.72:64080/gmodupdates/GMODDedicatedServerUpdate.exe"), filePath & "/GMODDedicatedServerUpdate.gmdsu")
        Catch ex As Exception
            MsgBox(ex.ToString, ex.Source)

        End Try




    End Sub
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)

        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100

        ' ProgressBar1.Value = e.ProgressPercentage  'Int32.Parse(Math.Truncate(percentage).ToString())

    End Sub
    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        Dim ans As String
        Try

            If File.Exists(filePath + "\GMODDedicatedServerUpdate.gmdsu") = True Then
                'Label1.Text = "Download complete. Running Update."
                My.Computer.FileSystem.RenameFile(filePath & "/GMODDedicatedServerUpdate.gmdsu", "GMODDedicatedServerUpdate.exe")
                Call Finish()
            Else
                ans = MessageBox.Show("An unknown error has occoured. Would you like to try again?", "Unknown error has occoured.", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                If ans = vbYes Then
                    Call DownloadUpdater()
                Else
                    MessageBox.Show("Update canceled", "Update canceled.", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, ex.Source)
        End Try

    End Sub
    Sub Finish()
        Dim Res
        Dim tool

        tool = filePath & "/GMODDedicatedServerUpdate.exe"
        MsgBox("Download complete. Apply update.")
        System.Threading.Thread.Sleep(200)
        Res = Shell(tool)
        Application.Exit()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles gmodloadingURL_txt.TextChanged
        If gmodloadingURL_txt.Text IsNot "" Then

            svLoadingURL = ("sv_loadingurl " & quote & gmodloadingURL_txt.Text & quote)
        Else
            svLoadingURL = ""

        End If


    End Sub

    Private Sub TextBox1_TextChanged_1(sender As Object, e As EventArgs) Handles downloadMaps_txt.TextChanged
        If gmodloadingURL_txt.Text IsNot "" Then
            sv_downloadURL = ("sv_downloadurl " & quote & downloadMaps_txt.Text & quote)
            sv_allowdownload = "sv_allowdownload 1"
            sv_allowupload = "sv_allowupload 1"
        Else
            sv_downloadURL = ""
            sv_allowdownload = "sv_allowdownload 0"
            sv_allowupload = "sv_allowupload 0"

        End If
    End Sub

    Private Sub addMaxValues_btn_Click(sender As Object, e As EventArgs) Handles addMaxValues_btn.Click
        mProps_txt.Text += 10
        mRagDolls_txt.Text += 10
        mNPCs_txt.Text += 10
        mBallons_txt.Text += 10
        mEffects_txt.Text += 10
        mDynamics_txt.Text += 10
        mLamps_txt.Text += 10
        mThrusters_txt.Text += 10
        mWheels_txt.Text += 10
        mHoverballs_txt.Text += 10
        mVehicles_txt.Text += 10
        mButtons_txt.Text += 10
        mSents_txt.Text += 10
        mEmitters_txt.Text += 10
        mSpawners_txt.Text += 10
        mTurrets_txt.Text += 10

    End Sub

    Private Sub subAllMaxValues_btn_Click(sender As Object, e As EventArgs) Handles subAllMaxValues_btn.Click
        mProps_txt.Text -= 10
        mRagDolls_txt.Text -= 10
        mNPCs_txt.Text -= 10
        mBallons_txt.Text -= 10
        mEffects_txt.Text -= 10
        mDynamics_txt.Text -= 10
        mLamps_txt.Text -= 10
        mThrusters_txt.Text -= 10
        mWheels_txt.Text -= 10
        mHoverballs_txt.Text -= 10
        mVehicles_txt.Text -= 10
        mButtons_txt.Text -= 10
        mSents_txt.Text -= 10
        mEmitters_txt.Text -= 10
        mSpawners_txt.Text -= 10
        mTurrets_txt.Text -= 10
    End Sub

    Private Sub resetAllMaxValues_btn_Click(sender As Object, e As EventArgs) Handles resetAllMaxValues_btn.Click

        mProps_txt.Text = 10
        mRagDolls_txt.Text = 10
        mNPCs_txt.Text = 10
        mBallons_txt.Text = 10
        mEffects_txt.Text = 10
        mDynamics_txt.Text = 10
        mLamps_txt.Text = 10
        mThrusters_txt.Text = 10
        mWheels_txt.Text = 10
        mHoverballs_txt.Text = 10
        mVehicles_txt.Text = 10
        mButtons_txt.Text = 10
        mSents_txt.Text = 10
        mEmitters_txt.Text = 10
        mSpawners_txt.Text = 10
        mTurrets_txt.Text = 10
    End Sub

    '-----------
    Private Sub LatestNewsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LatestNewsToolStripMenuItem.Click
        MessageBox.Show("News portal coming soon.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ExportConfigurationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportConfigurationToolStripMenuItem.Click
        Try

            Using addInfo = File.CreateText(filePath + "\garrysmod\cfg\server.cfg")

                addInfo.WriteLine("Application version: " & Me.ProductVersion)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Server info generated by BLOODED CYBORG.")
                addInfo.WriteLine("log on")
                addInfo.WriteLine("")
                addInfo.WriteLine("hostname " & serverName)
                addInfo.WriteLine("rcon_password " & rconPass)
                addInfo.WriteLine("maxplayers " & maxPLYRS)
                addInfo.WriteLine("gamemode " & gameMode)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Internet, region, lobby setup")
                addInfo.WriteLine("hostport " & usePort)
                addInfo.WriteLine("sv_lan " & useLAN)
                addInfo.WriteLine("sv_region " & regionMode)
                addInfo.WriteLine(svLoadingURL)
                addInfo.WriteLine(sv_downloadURL)
                addInfo.WriteLine(sv_allowdownload)
                addInfo.WriteLine(sv_allowupload)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Game Settings")
                addInfo.WriteLine("")
                addInfo.WriteLine("sbox_godmode		0")
                addInfo.WriteLine("sbox_noclip		1")
                addInfo.WriteLine("")
                addInfo.WriteLine("sbox_maxprops " & maxProps)
                addInfo.WriteLine("sbox_maxragdolls	" & maxRagDolls)
                addInfo.WriteLine("sbox_maxnpcs " & maxNPCs)
                addInfo.WriteLine("sbox_maxballoons	" & maxBallons)
                addInfo.WriteLine("sbox_maxeffects " & maxEffects)
                addInfo.WriteLine("sbox_maxdynamite	" & maxDynamics)
                addInfo.WriteLine("sbox_maxlamps " & maxLamps)
                addInfo.WriteLine("sbox_maxthrusters " & maxThrusters)
                addInfo.WriteLine("sbox_maxwheels " & maxWheels)
                addInfo.WriteLine("sbox_maxhoverballs " & maxHoverBalls)
                addInfo.WriteLine("sbox_maxvehicles " & maxVehicles)
                addInfo.WriteLine("sbox_maxbuttons " & maxButtons)
                addInfo.WriteLine("sbox_maxsents " & maxSents)
                addInfo.WriteLine("sbox_maxemitters	" & maxEmitters)
                addInfo.WriteLine("sbox_maxspawners	" & maxSpawners)
                addInfo.WriteLine("sbox_maxturrets " & maxTurrets)
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Network Tweaks - Increase network performance")
                addInfo.WriteLine("")
                addInfo.WriteLine("rate 10000")       '   //default 10000; Max bytes/sec the host can recieve data")
                addInfo.WriteLine("sv_minrate 15000") '   //default "5000"; Min bandwidth rate allowed on server, 0 = unlimited")
                addInfo.WriteLine("sv_maxrate 30000") '   //default "0";  Max bandwidth rate allowed On server, 0 = unlimited")
                addInfo.WriteLine("sv_mincmdrate 20") '   //Default 0; This sets the minimum value For cl_cmdrate. 0 = unlimited [cevo=67]")
                addInfo.WriteLine("sv_maxcmdrate 33") '   //Default 40; (If sv_mincmdrate Is > 0), this sets the maximum value For cl_cmdrate. [cevo= 101]")
                addInfo.WriteLine("decalfrequency 10")
                addInfo.WriteLine("net_maxfilesize 100")
                addInfo.WriteLine("")
                addInfo.WriteLine("/////////////////////////")
                addInfo.WriteLine("//Bans")
                addInfo.WriteLine("//  execute banned.cfgs at server start. Optimally at launch commandline.")
                addInfo.WriteLine("//  Put writeip/wrtieid commands in the bottom of server.cfg AFTER banned.cfgs have loaded.")
                addInfo.WriteLine("")
                addInfo.WriteLine("exec banned_user.cfg ")      '//loads banned users' ids
                addInfo.WriteLine("exec banned_ip.cfg   ")      '//loads banned users' ips
                addInfo.WriteLine("writeip              ")      '//Save the ban list to banned_ip.cfg.
                addInfo.WriteLine("writeid              ")      '//Writes a list of permanently-banned user IDs to banned_user.cfg.
                addInfo.WriteLine("")
                addInfo.WriteLine("")
                addInfo.WriteLine("")

                ' addInfo.WriteLine()  'new addline template
            End Using
            MessageBox.Show("Server.cfg has been succesfully exported.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox1.ShowDialog()
    End Sub

    Private Sub FAQToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FAQToolStripMenuItem.Click
        MessageBox.Show("Support page coming soon.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ExtractGMAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractGMAToolStripMenuItem.Click

        ' Dim dir As String = My.Computer.FileSystem.SpecialDirectories.Temp
        ' Dim fileName As String = dir + "GmadExtractor.exe"
        ' Dim extraDLL As String = dir + "GmadExtractor.Extras.dll"
        ' Dim MortenDLL As String = dir + "Morten.Gmod.dll"

        Try

            ' MessageBox.Show("")

            ' IO.File.WriteAllBytes(fileName, My.Resources.GmadExtractor)
            ' IO.File.WriteAllBytes(fileName, My.Resources.GmadExtractor_Extras)
            ' IO.File.WriteAllBytes(fileName, My.Resources.Morten_Gmod)
            '
            ' Process.Start(My.Computer.FileSystem.SpecialDirectories.Temp & "GmadExtractor.exe")
        Catch ex As Exception
            MessageBox.Show(ex.Message, ex.Source)
        End Try
    End Sub
End Class
