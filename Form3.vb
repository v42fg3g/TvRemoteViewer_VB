Public Class Form3

    Private Sub Form3_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        F_window_set()
    End Sub

    Private Sub Form3_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        show_AccessLogList()
        Timer1.Enabled = True
    End Sub

    Private Sub show_AccessLogList()
        'アクセス一覧表示
        If AccessLogList IsNot Nothing Then
            Array.Sort(AccessLogList) '日時降順
            Try
                DataGridViewAccessLog.SuspendLayout() 'レイアウト表示停止

                '列調整
                Dim row_count As Integer = DataGridViewAccessLog.Rows.Count
                If row_count < AccessLogList.Length Then
                    '増加
                    DataGridViewAccessLog.Rows.Add(AccessLogList.Length - row_count)
                ElseIf row_count > AccessLogList.Length Then
                    '削除
                    While row_count > AccessLogList.Length
                        DataGridViewAccessLog.Rows.RemoveAt(row_count - 1)
                        row_count = DataGridViewAccessLog.Rows.Count
                    End While
                End If

                Dim fc As Color = Color.White
                Dim bc As Color = Color.Black

                Dim t As DateTime = Now()
                Dim ut As Integer = time2unix(t)
                Dim limit_day As DateTime = CDate(Now().ToString("yyyy/MM/dd 5:00"))
                Dim limit_utime As Integer = time2unix(limit_day)
                If Hour(t) >= 5 Then
                    limit_day = DateAdd(DateInterval.Day, 1, limit_day)
                    limit_utime = time2unix(limit_day)
                End If
                For i As Integer = 0 To AccessLogList.Length - 1
                    DataGridViewAccessLog_write_cell_by_name("utime", i, reserve_Hmm(AccessLogList(i).utime), fc, bc)
                    DataGridViewAccessLog_write_cell_by_name("IP", i, AccessLogList(i).IP, fc, bc)
                    DataGridViewAccessLog_write_cell_by_name("domain", i, AccessLogList(i).domain, fc, bc)
                    DataGridViewAccessLog_write_cell_by_name("URL", i, AccessLogList(i).URL, fc, bc)
                Next

                DataGridViewAccessLog.ResumeLayout(False) 'レイアウト表示再開

                DataGridViewAccessLog.ClearSelection()
                DataGridViewAccessLog.Refresh()

                'ドメイン表示
                Dim outchk As Integer = 0
                For i As Integer = 0 To AccessLogList.Length - 1
                    If AccessLogList(i).domain.Length = 0 Then
                        If isLocalIP(AccessLogList(i).IP) = 1 Then
                            AccessLogList(i).domain = "LAN"
                            DataGridViewAccessLog_write_cell_by_name("domain", i, "LAN", fc, bc)
                        Else
                            'IPHostEntryオブジェクトを取得
                            Try
                                Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(AccessLogList(i).IP)
                                If iphe.HostName.Length > 0 Then
                                    AccessLogList(i).domain = iphe.HostName
                                    DataGridViewAccessLog_write_cell_by_name("domain", i, AccessLogList(i).domain, fc, bc)
                                End If
                            Catch ex As Exception
                            End Try
                            outchk = 1
                        End If
                    End If
                Next
                DataGridViewAccessLog.Refresh()

                If outchk = 1 And (form1_ID.Length = 0 Or form1_PASS.Length = 0) Then
                    LabelAccessLogWarning.Text = "※警告　外部からアクセスする場合はIDとPASSを設定してください"
                    If NotifyIcon_status = 0 Then
                        Try
                            Form1.NotifyIcon1.Icon = My.Resources.TvRemoteViewer_Warning
                            NotifyIcon_status = 1
                        Catch ex2 As Exception
                        End Try
                    End If
                End If
            Catch ex As Exception
                log1write("【エラー】DataGridView表示中にエラーが発生しました。" & ex.Message)
                Exit Sub
            End Try
        Else
            Try
                Dim row_count As Integer = DataGridViewAccessLog.Rows.Count
                If row_count > 0 Then
                    For i As Integer = 0 To row_count - 1
                        DataGridViewAccessLog.Rows.RemoveAt(0)
                    Next
                End If
            Catch ex As Exception
                log1write("【エラー】DataGridViewクリア中にエラーが発生しました。" & ex.Message)
            End Try
        End If
    End Sub

    Private Function reserve_Hmm(ByVal startt As Integer) As String
        Dim r As String = ""

        Dim t1 As DateTime = unix2time(startt)

        Dim md As String = t1.ToString("yyyy年MM月dd日(ddd)")
        Dim hm1 As String = t1.ToString("H:mm:ss")
        If Hour(t1) < 10 Then
            hm1 = " " & hm1
        End If
        r = md & "　" & hm1
        Return r
    End Function

    Public Sub DataGridViewAccessLog_write_cell(ByVal x As Object, ByVal y As Integer, ByVal text As String, Optional ByVal fc As Object = Nothing, Optional ByVal bc As Object = Nothing)
        'グリッドに文字列を書き込む
        Try
            If IsNumeric(x) Then
                'xが数値の場合
                If fc Is Nothing And bc Is Nothing Then
                    DataGridViewAccessLog.Rows(y).Cells(x).Value = text
                ElseIf bc Is Nothing Then
                    DataGridViewAccessLog.Rows(y).Cells(x).Value = text
                    DataGridViewAccessLog(x, y).Style.ForeColor = fc
                ElseIf fc Is Nothing Then
                    DataGridViewAccessLog.Rows(y).Cells(x).Value = text
                    DataGridViewAccessLog(x, y).Style.BackColor = bc
                Else
                    DataGridViewAccessLog.Rows(y).Cells(x).Value = text
                    DataGridViewAccessLog(x, y).Style.ForeColor = fc
                    DataGridViewAccessLog(x, y).Style.BackColor = bc
                End If
            Else
                'xが文字列の場合はNameから
                DataGridViewAccessLog_write_cell_by_name(x, y, text, fc, bc)
            End If
        Catch ex As Exception
            log1write("セル書き込み時にエラーが発生しました。" & ex.Message)
        End Try
    End Sub

    Public Sub DataGridViewAccessLog_write_cell_by_name(ByVal name As String, ByVal y As Integer, ByVal text As String, Optional ByVal fc As Object = Nothing, Optional ByVal bc As Object = Nothing)
        'xをヘッダー名から判別して文字列に書き込む
        Dim x As Integer = -1
        If DataGridViewAccessLog.ColumnCount > 0 Then
            For i = 0 To DataGridViewAccessLog.ColumnCount - 1
                If name = DataGridViewAccessLog.Columns(i).HeaderText Or name = DataGridViewAccessLog.Columns(i).Name Then
                    DataGridViewAccessLog_write_cell(i, y, text, fc, bc)
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub Form3_Resize(sender As System.Object, e As System.EventArgs) Handles MyBase.Resize
        'サイズ等調整
        Dim tSize As System.Drawing.Size = Me.ClientSize
        DataGridViewAccessLog.Left = 0
        DataGridViewAccessLog.Top = 24
        DataGridViewAccessLog.Width = tSize.Width
        DataGridViewAccessLog.Height = tSize.Height - 24
        DataGridViewAccessLog.Columns(3).Width = DataGridViewAccessLog.Width - DataGridViewAccessLog.Columns(0).Width - DataGridViewAccessLog.Columns(1).Width - DataGridViewAccessLog.Columns(2).Width - SystemInformation.VerticalScrollBarWidth

        ButtonRefresh.Top = 0
        ButtonRefresh.Left = tSize.Width - ButtonRefresh.Width - 2
        ButtonClear.Top = 0
        ButtonClear.Left = ButtonRefresh.Left - ButtonClear.Width - 8
    End Sub

    Private Sub ButtonRefresh_Click(sender As System.Object, e As System.EventArgs) Handles ButtonRefresh.Click
        show_AccessLogList()
    End Sub

    Private Sub ButtonClear_Click(sender As System.Object, e As System.EventArgs) Handles ButtonClear.Click
        Dim result As DialogResult = MessageBox.Show("アクセスログをクリアしますか？", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If result = DialogResult.Yes Then
            AccessLogList = Nothing
            show_AccessLogList()
            If NotifyIcon_status = 1 Then
                Try
                    Form1.NotifyIcon1.Icon = My.Resources.TvRemoteViewer_VB3
                    NotifyIcon_status = 0
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    'ウィンドウの位置を復元
    Public Sub F_window_set()
        'カレントディレクトリ変更
        F_set_ppath4program()

        Dim line() As String = file2line("form_status3.txt")
        If line IsNot Nothing Then
            Dim i As Integer
            For i = 0 To line.Length - 1
                Dim lr() As String = line(i).Split("=")
                If lr.Length = 2 Then
                    Select Case trim8(lr(0))
                        Case "columns_width"
                            Dim d() As String = lr(1).Split(",")
                            If d.Length >= 4 Then
                                DataGridViewAccessLog.Columns(0).Width = d(0)
                                DataGridViewAccessLog.Columns(1).Width = d(1)
                                DataGridViewAccessLog.Columns(2).Width = d(2)
                                DataGridViewAccessLog.Columns(3).Width = d(3)
                            End If
                        Case "WindowStatus"
                            Dim d() As String = lr(1).Split(",")
                            If d.Length = 4 Then
                                If Val(d(2)) >= 50 And Val(d(3)) >= 50 Then
                                    me_window_backup = Trim(lr(1))
                                    Me.Left = Val(d(0))
                                    Me.Top = Val(d(1))
                                    Me.Width = Val(d(2))
                                    Me.Height = Val(d(3))
                                End If
                            End If
                    End Select
                End If
            Next
        End If
    End Sub

    Private Sub save_form_status()
        Dim s As String = ""

        If Me.Top > -10 Then
            s &= "columns_width = "
            s &= DataGridViewAccessLog.Columns(0).Width
            s &= "," & DataGridViewAccessLog.Columns(1).Width
            s &= "," & DataGridViewAccessLog.Columns(2).Width
            s &= "," & DataGridViewAccessLog.Columns(3).Width
            s &= vbCrLf

            s &= "WindowStatus=" & Me.Left & "," & Me.Top & "," & Me.Width & "," & Me.Height & vbCrLf

            'ステータスファイル書き込み
            str2file("form_status3.txt", s)
        End If

    End Sub

    Private Sub Form3_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        save_form_status()
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If WindowState <> FormWindowState.Minimized Then
            show_AccessLogList()
        End If
    End Sub

    Private Sub DataGridViewAccessLog_ColumnWidthChanged(sender As System.Object, e As System.Windows.Forms.DataGridViewColumnEventArgs) Handles DataGridViewAccessLog.ColumnWidthChanged
        DataGridViewAccessLog.Columns(3).Width = DataGridViewAccessLog.Width - DataGridViewAccessLog.Columns(0).Width - DataGridViewAccessLog.Columns(1).Width - DataGridViewAccessLog.Columns(2).Width - SystemInformation.VerticalScrollBarWidth
    End Sub
End Class