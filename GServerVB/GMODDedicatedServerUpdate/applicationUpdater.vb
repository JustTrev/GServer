Imports System.IO
Imports System.Net

Public Class applicationUpdater

    Dim web As New WebClient
    Dim filePath As String = CurDir()

    'Dim LatestAppVersion As String = web.DownloadString("http://24.210.18.72:64080/gmodversion.txt")

    Dim client As WebClient = New WebClient


    'Dim FileName As String = "ACO Launcher.temp"
    'Dim NewFileName As String = "GMOD Prop Hunt Server.exe"
    Private Sub ApplicationUpdater_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try


            If File.Exists(filePath & "/GMOD Dedicated Server.exe") = True Then
                File.Delete(filePath & "/GMOD Dedicated Server.exe")
            Else
                '
            End If
        Catch ex As Exception
            MsgBox(ex.Message, ex.Source)
        End Try

        Call UpdateNow()
    End Sub


    Sub UpdateNow()

        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf client_DownloadCompleted

        'MsgBox("There is a new update available and will begin to download.")

        Try
            client.DownloadFileAsync(New Uri("http://24.210.18.72:64080/gmodupdates/GMOD%20Dedicated%20Server.exe"), filePath & "/GMOD Dedicated Server.gmdsu")
        Catch ex As Exception
            MsgBox(ex.Message, "Error")
            Me.Close()
        End Try




    End Sub

    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)

        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100

        ProgressBar1.Value = e.ProgressPercentage  'Int32.Parse(Math.Truncate(percentage).ToString())

    End Sub

    Private Sub client_DownloadCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        Try


            If File.Exists(filePath + "\GMOD Dedicated Server.gmdsu") = True Then
                'Label1.Text = "Download complete. Running Update."
                My.Computer.FileSystem.RenameFile(filePath & "/GMOD Dedicated Server.gmdsu", "GMOD Dedicated Server.exe")
                Call Finish()
            Else
                'Label1.Text = "Gathering Files. Please wait.."
                'Call DownloadUpdater()
            End If
        Catch ex As Exception

        End Try
    End Sub




    Sub Finish()

        Dim Res
        Dim tool

        Try
            tool = filePath & "/GMOD Dedicated Server.exe"
            'MsgBox("Download complete. Applying update.")
            'System.Threading.Thread.Sleep(1000)
            Res = Shell(tool)
        Catch ex As Exception
            MsgBox(ex.Message, ex.Source)
        End Try
        Me.Close()
    End Sub
End Class
