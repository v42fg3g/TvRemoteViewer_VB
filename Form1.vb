Imports System.Threading

Public Class Form1
    Private version As String = "TvRemoteViewer_VB version 0.05"

    '指定語句が含まれるBonDriverは無視する
    Private BonDriver_NGword As String() = {"_file", "_udp", "_pipe"}

    Private chk_timer1 As Integer = 0 'timer1重複回避用temp
    Private chk_timer1_deleteTS As Integer = 0 '古いtsファイルを削除する間隔調整用
    Private ServiceID_temp As String '起動時、前回終了時のサービスIDを復活するための一時待避用temp

    '================================================================
    'メイン
    '================================================================

    '視聴スタートボタン
    Private Sub ButtonMovieStart_Click(sender As System.Object, e As System.EventArgs) Handles ButtonMovieStart.Click
        If Me._worker._isWebStart = True Then
            Dim num As Integer = Val(ComboBoxNum.Text.ToString)
            Dim bondriver As String = ComboBoxBonDriver.Text.ToString
            Dim sid As Integer = 0 '下で取得
            Dim chspace As Integer = Val(TextBoxChSpace.Text.ToString)
            Dim udpApp As String = textBoxUdpApp.Text.ToString
            Dim hlsApp As String = textBoxHlsApp.Text.ToString
            Dim hlsroot As String = ""
            Dim ss As String = "\"
            Dim sp As Integer = hlsApp.LastIndexOf(ss)
            If sp > 0 Then
                hlsroot = hlsApp.Substring(0, sp)
            End If
            Dim hlsOpt1 As String = "" 'textBoxHlsOpt1.Text.ToString
            Dim hlsOpt2 As String = textBoxHlsOpt2.Text.ToString
            Dim wwwroot As String = TextBoxWWWroot.Text.ToString
            Dim fileroot As String = TextBoxFILEROOT.Text.ToString
            Dim s() As String = ComboBoxServiceID.Text.Split(",")
            Dim ShowConsole As Boolean = CheckBoxShowConsole.Checked
            If bondriver.IndexOf(".dll") > 0 Then
                If s.Length = 3 Then
                    sid = Val(s(1))
                    Me._worker.start_movie(num, bondriver, sid, chspace, udpApp, hlsApp, hlsOpt1, hlsOpt2, wwwroot, fileroot, hlsroot, ShowConsole)
                Else
                    MsgBox("サービスIDを指定してください")
                End If
            Else
                MsgBox("BonDriverを指定してください")
            End If
        Else
            MsgBox("httpサーバーが起動していません")
        End If
    End Sub

    '視聴ストップボタン
    Private Sub ButtonMovieStop_Click(sender As System.Object, e As System.EventArgs) Handles ButtonMovieStop.Click
        Dim num As Integer = Val(ComboBoxNum.Text.ToString)
        Me._worker.stop_movie(num)
    End Sub

    'タイマー　1秒ごとに実行
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If chk_timer1 = 0 Then '重複実行防止
            chk_timer1 = 1

            'VLCのcrashダイアログが出ていたら消す
            If textBoxHlsApp.Text.IndexOf("vlc.exe") >= 0 Then
                Me._worker.check_crash_dialog()
            End If

            'プロセスがうまく動いているかチェック
            Me._worker.checkAllProc()

            'ffmpeg.exeを使用している場合は、1分間に1回古いTSを削除する
            If chk_timer1_deleteTS >= 60 And textBoxHlsApp.Text.IndexOf("ffmpeg.exe") >= 0 Then
                delete_old_TS()
                chk_timer1_deleteTS = 0
            End If
            chk_timer1_deleteTS += 1

            '現在稼働中のストリームをタスクトレイアイコンのマウスオーバー時に表示する
            Dim s As String = Me._worker.get_live_numbers()
            If s.Length > 1 Then
                LabelStream.Text = "配信中：" & s 'ついでにフォーム上にも表示
                s = "TvRemoteViewer_VB" & vbCrLf & "配信中：" & Trim(s)
            Else
                LabelStream.Text = " " 'ついでにフォーム上にも表示
                s = "TvRemoteViewer_VB"
            End If
            If s.Length > 60 Then
                '64文字を超えるとエラーになる
                s = s.Substring(0, 60) & ".."
            End If
            Try
                NotifyIcon1.Text = s
            Catch ex As Exception
                NotifyIcon1.Text = "TvRemoteViewer_VB"
            End Try

            chk_timer1 = 0
        End If

        'ログ処理
        If log1.Length > 10000 Then
            '10000文字以上になったらカット
            log1 = log1.Substring(0, 10000)
        End If
        If log1 <> TextBoxLog.Text Then
            TextBoxLog.Text = log1
        End If
        Refresh()
        TextBoxLog.Refresh()

    End Sub

    '古いTSファイルを削除する　ffmpeg用
    Private Sub delete_old_TS()
        Dim fileroot As String = TextBoxFILEROOT.Text.ToString
        If fileroot.Length = 0 Then
            'm3u8やtsの格納場所が指定されていなければwwwrootと同じ場所とする
            fileroot = TextBoxWWWroot.Text.ToString
        End If
        'Dim files As String() = System.IO.Directory.GetFiles(tsroot, "*.ts", System.IO.SearchOption.AllDirectories)
        Dim files As String() = System.IO.Directory.GetFiles(fileroot, "*.m3u8")
        For Each tempFile As String In files
            Dim n As Integer = 0
            If tempFile.IndexOf("mystream") >= 0 Then
                Try
                    n = Val(tempFile.Substring(tempFile.IndexOf("mystream") + "mystream".Length)) 'スレッドナンバー
                Catch ex As Exception
                    n = 0
                End Try
                If n > 0 Then
                    Dim s As String = ReadAllTexts(tempFile)
                    Dim sp As Integer = s.IndexOf("mystream" & n.ToString & "-")
                    Dim m As Integer = -1
                    Try
                        m = Val(s.Substring(sp + ("mystream" & n.ToString & "-").Length))
                    Catch ex As Exception
                        m = -1
                    End Try
                    If m > 0 Then
                        Dim files2 As String() = System.IO.Directory.GetFiles(fileroot, "mystream" & n.ToString & "-*.ts")
                        For Each tsFile As String In files2
                            Dim sp2 As Integer = tsFile.IndexOf("mystream" & n.ToString & "-")
                            Dim m2 As Integer = -1
                            Try
                                m2 = Val(tsFile.Substring(sp2 + ("mystream" & n.ToString & "-").Length))
                            Catch ex As Exception
                                m2 = -1
                            End Try
                            If (m2 >= 0 And m2 < m) Or (m2 > (m + 30)) Then
                                '古いものと、なぜか残っている未来のものを消す
                                deletefile(tsFile)
                            End If
                        Next
                    End If
                End If
            End If
        Next
    End Sub

    '================================================================
    'Webサーバー
    '================================================================

    Private _webThread As Thread = Nothing
    Private _worker As WebRemocon = Nothing

    'httpサーバースタートボタン　スタート時に自動実行することにした
    Private Sub ButtonWebStart_Click(sender As System.Object, e As System.EventArgs) Handles ButtonWebStart.Click
        StartHttpServer()
    End Sub

    'httpサーバー　スタート
    Private Sub StartHttpServer()
        'フォームからパラメーターを取得
        Dim udpApp As String = Me.textBoxUdpApp.Text.ToString
        Dim udpPort As Integer = Val(Me.textBoxUdpPort.Text.ToString)
        Dim chSpace As Integer = Val(Me.TextBoxChSpace.Text.ToString)
        Dim hlsApp As String = Me.textBoxHlsApp.Text.ToString
        Dim hlsOpt1 As String = "" 'Me.textBoxHlsOpt1.Text.ToString
        Dim hlsOpt2 As String = Me.textBoxHlsOpt2.Text.ToString
        Dim wwwroot As String = Me.TextBoxWWWroot.Text.ToString
        Dim fileroot As String = Me.TextBoxFILEROOT.Text.ToString
        Dim wwwport As Integer = Val(Me.textHttpPortNumber.Text.ToString)
        Dim num As Integer = Val(Me.ComboBoxNum.Text.ToString)
        Dim BonDriverPath As String = Me.TextBoxBonDriverPath.Text.ToString
        Dim ShowConsole As Boolean = Me.CheckBoxShowConsole.Checked

        Me.ButtonWebStart.Enabled = False
        Me._worker = New WebRemocon(udpApp, udpPort, chSpace, hlsApp, hlsOpt1, hlsOpt2, wwwroot, fileroot, wwwport, BonDriverPath, ShowConsole, BonDriver_NGword)
        Me._webThread = New Thread(New ThreadStart(AddressOf Me._worker.Web_Start))
        Me._webThread.Start()
        Me.ButtonWebStop.Enabled = True
    End Sub

    'httpサーバー　ストップ
    Private Sub ButtonWebStop_Click(sender As System.Object, e As System.EventArgs) Handles ButtonWebStop.Click
        Me.ButtonWebStop.Enabled = False
        'this._webThread.Abort();
        Me._worker.requestStop()
        'Me._webThread.Join()
        Me._webThread.Abort()
        Me.ButtonWebStart.Enabled = True
    End Sub

    'IEを開く
    Public Sub IE_open(ByVal url As String)
        Dim objIE As Object
        objIE = CreateObject("internetexplorer.application")
        objIE.Visible = True
        objIE.navigate(url)
        objIE = Nothing
    End Sub

    'IEでhttp://127.0.0.1:ポート/を開く
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim port As String = textHttpPortNumber.Text
        If Val(port) > 0 Then
            IE_open("http://127.0.0.1:" & port)
        End If
    End Sub

    'index.html編集
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Try
            'カレントディレクトリ変更
            System.IO.Directory.SetCurrentDirectory(TextBoxWWWroot.Text)
            Try
                System.Diagnostics.Process.Start("notepad.exe", """index.html""")
            Catch ex As Exception
                '開けないファイルだった場合
            End Try
        Catch ex As Exception

        End Try
    End Sub

    '================================================================
    '起動・終了時の動作
    '================================================================

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '二重起動をチェックする
        If Diagnostics.Process.GetProcessesByName(Diagnostics.Process.GetCurrentProcess.ProcessName).Length > 1 Then
            'すでに起動していると判断する
            Close()
        End If

        If file_exist("HLS_option.txt") = 0 Then
            MsgBox("HLS_option.txtが見つかりません。")
            '終了
            Close()
        End If

        log1write(version)
    End Sub

    Private Sub Form1_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        'フォームの項目を復元
        F_window_set()
        'コンボボックスの項目をセット
        search_BonDriver()
        search_ServiceID()
        ComboBoxServiceID.Text = ServiceID_temp '前回終了時に選択していたものをセット

        'httpサーバースタート
        log1write("httpサーバーを起動しています")
        StartHttpServer()
        log1write("httpサーバーを起動しました")

        '関連アプリのプロセスが残っていれば停止する
        '全プロセスを名前指定で停止
        log1write("関連アプリのプロセスを停止しています")
        Me._worker.stopProcName("vlc")
        Me._worker.stopProcName("RecTask")
        Me._worker.stopProcName("ffmpeg")
        log1write("関連アプリのプロセスを停止しました")

        '解像度コンボボックスをセット httpサーバースタート後にvlc_option()がセットされている
        search_ComboBoxResolution()

        'プロセスクラッシュ監視等開始
        Timer1.Enabled = True
    End Sub

    'ウィンドウの位置を復元
    Public Sub F_window_set()
        Dim line() As String = file2line("form_status.txt")
        If line IsNot Nothing Then
            Dim i As Integer
            For i = 0 To line.Length - 1
                Dim lr() As String = line(i).Split("=")
                If lr.Length = 2 Then
                    Select Case trim8(lr(0))
                        Case "TextBoxWWWroot"
                            TextBoxWWWroot.Text = lr(1)
                        Case "TextBoxFILEroot"
                            TextBoxFILEROOT.Text = lr(1)
                        Case "textHttpPortNumber"
                            textHttpPortNumber.Text = lr(1)
                        Case "textBoxUdpApp"
                            textBoxUdpApp.Text = lr(1)
                        Case "TextBoxBonDriverPath"
                            TextBoxBonDriverPath.Text = lr(1)
                        Case "textBoxUdpPort"
                            textBoxUdpPort.Text = lr(1)
                        Case "textBoxHlsApp"
                            textBoxHlsApp.Text = lr(1)
                        Case "textBoxHlsOpt1"
                            'textBoxHlsOpt1.Text = lr(1)
                        Case "ComboBoxNum"
                            ComboBoxNum.Text = lr(1)
                        Case "ComboBoxBonDriver"
                            ComboBoxBonDriver.Text = lr(1)
                        Case "TextBoxChSpace"
                            TextBoxChSpace.Text = lr(1)
                        Case "ComboBoxServiceID"
                            ComboBoxServiceID.Text = lr(1)
                            ServiceID_temp = lr(1)
                        Case "ComboBoxResolution"
                            ComboBoxResolution.Text = lr(1)
                        Case "CheckBoxShowConsole"
                            CheckBoxShowConsole.Checked = lr(1)
                    End Select
                ElseIf lr.Length > 2 And trim8(lr(0)) = "textBoxHlsOpt" Then
                    'VLC OPTION
                    Dim sp As Integer = line(i).IndexOf("=")
                    Try
                        textBoxHlsOpt2.Text = line(i).Substring(sp + 1)
                    Catch ex As Exception
                        textBoxHlsOpt2.Text = ""
                    End Try
                End If
            Next
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Timer1.Enabled = False

        '全プロセスを停止
        Try
            Me._worker.stopProc(-1)
            '全プロセスを名前指定で停止
            'stopProcName("vlc")
            'stopProcName("RecTask")
        Catch ex As Exception
        End Try

        'Webスレッド停止
        Try
            Me._worker.requestStop()
            Me._webThread.Abort()
        Catch ex As Exception
        End Try
    End Sub

    'ウィンドウの位置を保存
    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Dim s As String = ""

        s &= "TextBoxWWWroot=" & TextBoxWWWroot.Text & vbCrLf
        s &= "TextBoxFILEroot=" & TextBoxFILEROOT.Text & vbCrLf
        s &= "textHttpPortNumber=" & textHttpPortNumber.Text & vbCrLf
        s &= "textBoxUdpApp=" & textBoxUdpApp.Text & vbCrLf
        s &= "TextBoxBonDriverPath=" & TextBoxBonDriverPath.Text & vbCrLf
        s &= "textBoxUdpPort=" & textBoxUdpPort.Text & vbCrLf
        s &= "textBoxHlsApp=" & textBoxHlsApp.Text & vbCrLf
        's &= "textBoxHlsOpt1=" & textBoxHlsOpt1.Text & vbCrLf
        s &= "ComboBoxNum=" & ComboBoxNum.Text & vbCrLf
        s &= "ComboBoxBonDriver=" & ComboBoxBonDriver.Text & vbCrLf
        s &= "TextBoxChSpace=" & TextBoxChSpace.Text & vbCrLf
        s &= "ComboBoxServiceID=" & ComboBoxServiceID.Text & vbCrLf
        s &= "ComboBoxResolution=" & ComboBoxResolution.Text & vbCrLf
        s &= "textBoxHlsOpt=" & textBoxHlsOpt2.Text & vbCrLf
        s &= "CheckBoxShowConsole=" & CheckBoxShowConsole.Checked & vbCrLf

        'カレントディレクトリ変更
        F_set_ppath4program()
        'ステータスファイル書き込み
        str2file("form_status.txt", s)
    End Sub

    '================================================================
    'ボタン等のフォーム内アイテムの動作
    '================================================================

    '解像度コンボボックスをセット
    Private Sub search_ComboBoxResolution()
        Try
            ComboBoxResolution.Items.Clear()
            For i As Integer = 0 To Me._worker.vlc_option.Length - 1
                ComboBoxResolution.Items.Add(Me._worker.vlc_option(i).resolution)
            Next
        Catch ex As Exception
        End Try
    End Sub

    '解像度コンボボックスの値が変更されたとき
    Private Sub ComboBoxResolution_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxResolution.SelectedIndexChanged
        Dim s As String = ComboBoxResolution.Text
        For i As Integer = 0 To Me._worker.vlc_option.Length - 1
            If Me._worker.vlc_option(i).resolution = s Then
                textBoxHlsOpt2.Text = Me._worker.vlc_option(i).opt
            End If
        Next
    End Sub

    'BonDriverを探してコンボボックスに追加
    Private Sub search_BonDriver()
        ComboBoxBonDriver.Items.Clear()
        Dim bondriver_path As String = TextBoxBonDriverPath.Text.ToString
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(textBoxUdpApp.Text.ToString)
        End If
        Try
            For Each stFilePath As String In System.IO.Directory.GetFiles(bondriver_path, "*.dll")
                Dim s As String = stFilePath & System.Environment.NewLine
                'フルパスファイル名がsに入る
                Dim fpf As String = trim8(s)
                If s.IndexOf("\") >= 0 Then
                    'ファイル名だけを取り出す
                    Dim k As Integer = s.LastIndexOf("\")
                    s = trim8(s.Substring(k + 1))
                End If
                Dim sl As String = s.ToLower() '小文字に変換
                '表示しないBonDriverかをチェック
                If BonDriver_NGword IsNot Nothing Then
                    For i As Integer = 0 To BonDriver_NGword.Length - 1
                        If sl.IndexOf(BonDriver_NGword(i)) >= 0 Then
                            sl = ""
                        End If
                    Next
                End If
                'コンボボックスに追加
                If sl.IndexOf("bondriver") = 0 Then
                    ComboBoxBonDriver.Items.Add(s)
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    'BonDriverが変更されたときはコンボボックス（サービスＩＤ）を変更
    Private Sub ComboBoxBonDriver_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxBonDriver.SelectedIndexChanged
        search_ServiceID()
    End Sub

    'ComboBoxBonDriverの値にしたがってComboBoxServiceIDを変更
    Private Sub search_ServiceID()
        ComboBoxServiceID.Items.Clear()
        Dim bondriver_path As String = TextBoxBonDriverPath.Text.ToString
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(textBoxUdpApp.Text.ToString)
        End If
        Dim filename As String
        If bondriver_path.Length > 0 Then
            filename = bondriver_path & "\" & ComboBoxBonDriver.Text.ToString.Replace(".dll", ".ch2")
        Else
            filename = ComboBoxBonDriver.Text.ToString.Replace(".dll", ".ch2")
        End If
        Dim line() As String = file2line(filename)
        If line IsNot Nothing Then
            For i As Integer = 0 To line.Length - 1
                If line(i).IndexOf(";") < 0 Then
                    Dim s() As String = line(i).Split(",")
                    If s.Length = 9 Then
                        ComboBoxServiceID.Items.Add(s(0) & " ," & s(5) & "," & s(1))
                    End If
                End If
            Next
            Try
                ComboBoxServiceID.SelectedIndex = 0
            Catch ex As Exception
                ComboBoxServiceID.Text = ""
            End Try
        Else
            ComboBoxServiceID.Text = ""
        End If
    End Sub

    'コンボボックス（サービスID）によってTextBoxChSpaceを変更
    Private Sub ComboBoxServiceID_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxServiceID.SelectedIndexChanged
        Dim s() As String = ComboBoxServiceID.Text.ToString.Split(",")
        If s.Length = 3 Then
            TextBoxChSpace.Text = s(2)
        End If
    End Sub

    'フォルダ選択
    Private Function F_get_folder(ByVal s As String) As Object
        Dim r As Object = Nothing
        '上部に表示する説明テキストを指定する
        FolderBrowserDialog1.Description = "フォルダを指定してください。"
        'ルートフォルダを指定する
        'デフォルトでDesktop
        FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop
        '最初に選択するフォルダを指定する
        'RootFolder以下にあるフォルダである必要がある
        If s.Length = 0 Then
            s = "C:\"
        End If
        FolderBrowserDialog1.SelectedPath = s '"C:\Windows"
        'ユーザーが新しいフォルダを作成できるようにする
        'デフォルトでTrue
        FolderBrowserDialog1.ShowNewFolderButton = True

        'ダイアログを表示する
        If FolderBrowserDialog1.ShowDialog(Me) = DialogResult.OK Then
            '選択されたフォルダを表示する
            r = FolderBrowserDialog1.SelectedPath
            'Debug.Print(FolderBrowserDialog1.SelectedPath)
        End If

        Return r
    End Function

    'ファイル選択
    Private Function F_get_filename(ByVal s As String) As Object
        Dim r As Object = Nothing

        If s.LastIndexOf("\") > 0 Then
            s = s.Substring(0, s.LastIndexOf("\"))
        End If

        ' ダイアログボックスのタイトル
        OpenFileDialog1.Title = "コメントファイルを開く"
        ' 初期表示するディレクトリ
        OpenFileDialog1.InitialDirectory = s
        ' デフォルトの選択ファイル名
        OpenFileDialog1.FileName = ""
        ' [ファイルの種類]に表示するフィルタ
        OpenFileDialog1.Filter = _
            "実行ファイル (*.exe)|*.exe" '|すべてのファイル(*.*)|*.*"
        ' [ファイルの種類]の初期表示インデックス
        OpenFileDialog1.FilterIndex = 1
        ' ファイル名に拡張子を自動設定するかどうか
        OpenFileDialog1.AddExtension = True
        ' 複数のファイルを選択するかどうか
        OpenFileDialog1.Multiselect = False
        ' ファイルが存在しない時に警告メッセージを表示するかどうか
        OpenFileDialog1.CheckFileExists = True
        ' PATHが存在しない時に警告メッセージを表示するかどうか
        OpenFileDialog1.CheckPathExists = True
        ' [読み取り専用ファイルとして開く]チェックボックスを表示するかどうか
        OpenFileDialog1.ShowReadOnly = False
        ' [読み取り専用ファイルとして開く]チェックボックスのデフォルト値
        OpenFileDialog1.ReadOnlyChecked = False
        ' ダイアログを閉じる時に、カレントディレクトリを元に戻すかどうか
        OpenFileDialog1.RestoreDirectory = True
        ' [ヘルプ]ボタンを表示するかどうか
        OpenFileDialog1.ShowHelp = False
        ' ダイアログボックスを表示する
        Dim btn As DialogResult = OpenFileDialog1.ShowDialog()
        If btn = Windows.Forms.DialogResult.OK Then
            ' 選択した一つのファイルPATHを取得する
            'MessageBox.Show(OpenFileDialog1.FileName & "が選択されました")
            ' 選択した複数のファイルPATHを取得する
            'MessageBox.Show(OpenFileDialog1.FileNames(0) & "が選択されました")

            r = OpenFileDialog1.FileName

        ElseIf btn = Windows.Forms.DialogResult.Cancel Then
            'MessageBox.Show("キャンセルされました")
        End If

        Return r
    End Function

    'WWWROOT選択
    Private Sub ButtonWWWROOT_Click(sender As System.Object, e As System.EventArgs) Handles ButtonWWWROOT.Click
        Dim s As String = F_get_folder(TextBoxWWWroot.Text.ToString)
        Try
            If s.Length > 0 Then
                TextBoxWWWroot.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'FILEROOT選択
    Private Sub ButtonFILEROOT_Click(sender As System.Object, e As System.EventArgs) Handles ButtonFILEROOT.Click
        Dim s As String = F_get_folder(TextBoxFILEROOT.Text.ToString)
        Try
            If s.Length > 0 Then
                TextBoxFILEROOT.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'BonDriverフォルダ選択
    Private Sub ButtonBonDriverPath_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonDriverPath.Click
        Dim s As String = F_get_folder(TextBoxBonDriverPath.Text.ToString)
        Try
            If s.Length > 0 Then
                TextBoxBonDriverPath.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'UDPアプリ選択
    Private Sub buttonUdpAppPath_Click(sender As System.Object, e As System.EventArgs) Handles buttonUdpAppPath.Click
        Dim s As String = F_get_filename(textBoxUdpApp.Text.ToString)
        Try
            If s.Length > 0 Then
                textBoxUdpApp.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'HLSアプリ選択
    Private Sub buttonHlsAppPath_Click(sender As System.Object, e As System.EventArgs) Handles buttonHlsAppPath.Click
        Dim s As String = F_get_filename(textBoxHlsApp.Text.ToString)
        Try
            If s.Length > 0 Then
                textBoxHlsApp.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'HLS_option.txt表示
    Private Sub ButtonHLSoption_Click(sender As System.Object, e As System.EventArgs) Handles ButtonHLSoption.Click
        Try
            Dim ppath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
            'パスだけを取り出す
            If ppath.Length > 0 Then
                Dim psp As Integer = ppath.LastIndexOf("\")
                If psp >= 0 Then
                    ppath = ppath.Substring(0, psp)
                Else
                    ppath = ""
                End If
            End If

            'カレントディレクトリ変更
            System.IO.Directory.SetCurrentDirectory(ppath)
            Try
                'System.Diagnostics.Process.Start("notepad.exe", """HLS_option.txt""")
                System.Diagnostics.Process.Start("HLS_option.txt")
            Catch ex As Exception
                '開けないファイルだった場合
            End Try
        Catch ex As Exception

        End Try
    End Sub

    'BonDriverPathが変更されたとき
    Private Sub TextBoxBonDriverPath_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxBonDriverPath.TextChanged
        'BonDriverとサービスIDを再セット
        search_BonDriver()
        'search_ServiceID()
        ComboBoxBonDriver.Text = ""
        ComboBoxServiceID.Text = ""
        '項目が変更されたことをインスタンスに知らせる
        Try
            Me._worker._BonDriverPath = TextBoxBonDriverPath.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub textBoxUdpApp_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxUdpApp.TextChanged
        Try
            Me._worker._udpApp = textBoxUdpApp.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub textBoxHlsApp_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxHlsApp.TextChanged
        Try
            Me._worker._hlsApp = textBoxHlsApp.Text.ToString
            Dim ss As String = "\"
            Dim sp As Integer = Me._worker._hlsApp.LastIndexOf(ss)
            If sp > 0 Then
                Me._worker._hlsroot = Me._worker._hlsApp.Substring(0, sp)
            Else
                Me._worker._hlsroot = ""
            End If
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub textBoxHlsOpt2_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxHlsOpt2.TextChanged
        Try
            Me._worker._hlsOpt2 = textBoxHlsOpt2.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub TextBoxChSpace_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxChSpace.TextChanged
        Try
            Me._worker._chSpace = TextBoxChSpace.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub CheckBoxShowConsole_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxShowConsole.CheckedChanged
        Try
            Me._worker._ShowConsole = CheckBoxShowConsole.Checked
        Catch ex As Exception
        End Try
    End Sub

    '初期値ボタン
    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        textHttpPortNumber.Text = "40003"
    End Sub

    '初期値ボタン
    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        textBoxUdpPort.Text = "42424"
    End Sub

    '最小化アイコン右クリック→終了
    Private Sub ContextMenuStrip1_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles ContextMenuStrip1.MouseClick
        Close()
    End Sub

    'タスクトレイアイコンがダブルクリックされたとき
    Private Sub NotifyIcon1_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Visible = True
        Me.WindowState = FormWindowState.Normal
        Me.TopMost = True
        Me.TopMost = False
    End Sub

    '最小化されたときはタスクトレイへ
    Private Sub Form1_Resize(sender As System.Object, e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Visible = False
        End If
    End Sub

    '================================================================
    'etc.
    '================================================================

    '余計な改行等を削除
    Public Function trim8(ByVal s As String) As String
        s = Trim(s)
        s = s.Replace(vbTab, "").Replace(vbCrLf, "").Replace("""", "")
        s = Trim(s)
        Return s
    End Function

    'プログラムが存在するディレクトリにカレントディレクトリ変更
    Public Sub F_set_ppath4program()
        Dim ppath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim psp As Integer
        If ppath.Length > 0 Then
            psp = ppath.LastIndexOf("\")
            If psp >= 0 Then
                ppath = ppath.Substring(0, psp + 1)
            Else
                ppath = ""
            End If
        End If
        System.IO.Directory.SetCurrentDirectory(ppath)
    End Sub

End Class
