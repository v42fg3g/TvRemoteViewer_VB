Imports System.Threading

Public Class Form1
    Private version As String = "TvRemoteViewer_VB version 1.10"

    '指定語句が含まれるBonDriverは無視する
    Private BonDriver_NGword As String() = {"_file", "_udp", "_pipe"}

    Private chk_timer1 As Integer = 0 'timer1重複回避用temp
    Private chk_timer1_deleteTS As Integer = 0 '古いtsファイルを削除する間隔調整用
    Private ServiceID_temp As String '起動時、前回終了時のサービスIDを復活するための一時待避用temp

    'TvRemoteViewer_VBが無事スタートしたら1
    Public TvRemoteViewer_VB_Start As Integer = 0

    '================================================================
    'メイン
    '================================================================

    '視聴スタートボタン
    Private Sub ButtonMovieStart_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles ButtonMovieStart.MouseClick
        If Me._worker._isWebStart = True Then
            Dim num As Integer = Val(ComboBoxNum.Text.ToString)
            Dim bondriver As String = ComboBoxBonDriver.Text.ToString
            Dim sid As Integer = 0 '下で取得
            Dim chspace As Integer = Val(TextBoxChSpace.Text.ToString)
            Dim udpApp As String = textBoxUdpApp.Text.ToString
            Dim udpOpt3 As String = TextBoxUdpOpt3.Text.ToString
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
            Dim NHK_dual_mono_mode_select As Integer = 0 'フォームからは指定しない常に０
            Dim HLSorHTTP As Integer = ComboBoxHLSorHTTP.SelectedIndex * 2 '0or2
            If num > 0 Then
                'If fileroot.IndexOf(wwwroot) = 0 Or fileroot.Length = 0 Then
                If bondriver.IndexOf(".dll") > 0 Then
                    If hlsOpt2.Length > 0 Then
                        If s.Length = 3 Then
                            sid = Val(s(1))
                            Dim filename As String = "" 'UDP配信モード　フォームからはUDP配信モード限定
                            Me._worker.start_movie(num, bondriver, sid, chspace, udpApp, hlsApp, hlsOpt1, hlsOpt2, wwwroot, fileroot, hlsroot, ShowConsole, udpOpt3, filename, NHK_dual_mono_mode_select, HLSorHTTP, "", 0, 0)
                        Else
                            MsgBox("サービスIDを指定してください")
                        End If
                    Else
                        MsgBox("HLSオプションを記入してください")
                    End If
                Else
                    MsgBox("BonDriverを指定してください")
                End If
                'Else
                'MsgBox("%FILEROOT%は%WWWROOT%と同じか子フォルダ以下にしてください")
                'End If
            Else
                MsgBox("ストリーム Numberは1以上に設定してください")
            End If
        Else
            MsgBox("httpサーバーが起動していません")
        End If
    End Sub

    '視聴ストップボタン
    Private Sub ButtonMovieStop_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles ButtonMovieStop.MouseClick
        Dim num As Integer = Val(ComboBoxNum.Text.ToString)
        Me._worker.stop_movie(num)
    End Sub

    'タイマー　1秒ごとに実行
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If chk_timer1 = 0 Then '重複実行防止
            chk_timer1 = 1

            'VLCのcrashダイアログが出ていたら消す
            If textBoxHlsApp.Text.IndexOf("vlc.exe") >= 0 Or BS1_hlsApp.IndexOf("vlc.exe") >= 0 Then
                Me._worker.check_crash_dialog()
            End If

            'プロセスがうまく動いているかチェック
            Me._worker.checkAllProc()

            'ffmpeg.exeを使用している場合は、1分間に1回古いTSを削除する
            If OLDTS_NODELETE = 0 Then
                If chk_timer1_deleteTS >= 60 And textBoxHlsApp.Text.IndexOf("ffmpeg.exe") >= 0 Then
                    delete_old_TS()
                    chk_timer1_deleteTS = 0
                End If
                chk_timer1_deleteTS += 1
            End If

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

            'アイドル時間が指定分に達した場合は全て切断する
            If STOP_IDLEMINUTES > 0 Then
                If (Now() - STOP_IDLEMINUTES_LAST).Minutes >= STOP_IDLEMINUTES Then
                    STOP_IDLEMINUTES_LAST = CDate("2199/12/31 23:59:59")
                    log1write("アイドル時間が" & STOP_IDLEMINUTES.ToString & "分に達しましたので全切断します")
                    Me._worker.stop_movie(-2)
                End If
            End If

            chk_timer1 = 0
        End If

        'ログ処理
        If log1 <> log1_dummy Then
            If log1.Length > 30000 Then
                '30000文字以上になったらカット
                log1 = log1.Substring(0, 30000)
            End If
            TextBoxLog.Text = log1
            log1_dummy = log1
            TextBoxLog.Refresh()
        End If

        'ファイル一覧　最後の更新から10秒経ったらファイル一覧更新
        Dim duration1 As TimeSpan = Now.Subtract(watcher_lasttime)
        If duration1.TotalSeconds >= 10 Then
            watcher_lasttime = C_DAY2038
            Me._worker.make_file_select_html("", 1, C_DAY2038, 0)
        End If

    End Sub

    '古いTSファイルを削除する　ffmpeg用
    Private Sub delete_old_TS()
        Me._worker.delete_old_TS()
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
        Dim udpOpt3 As String = Me.TextBoxUdpOpt3.Text.ToString
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
        Dim id As String = Me.TextBoxID.Text.ToString
        Dim pass As String = Me.TextBoxPASS.Text.ToString

        Me.ButtonWebStart.Enabled = False
        Me._worker = New WebRemocon(udpApp, udpPort, udpOpt3, chSpace, hlsApp, hlsOpt1, hlsOpt2, wwwroot, fileroot, wwwport, BonDriverPath, ShowConsole, BonDriver_NGword, id, pass)
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

    'VideoPath.txt編集
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Try
            'カレントディレクトリ変更
            F_set_ppath4program()
            Try
                'System.Diagnostics.Process.Start("notepad.exe", """index.html""")
                If file_exist("VideoPath.txt") Then
                    System.Diagnostics.Process.Start("VideoPath.txt")
                Else
                    System.Diagnostics.Process.Start("TvRemoteViewer_VB.ini")
                End If
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
        '二重起動をチェックする（コマンドラインで-dokが指定されていればチェックしない）
        If F_check_cmd() = 0 Then
            If Diagnostics.Process.GetProcessesByName(Diagnostics.Process.GetCurrentProcess.ProcessName).Length > 1 Then
                'すでに起動していると判断する
                Close()
            End If
        End If

        'カレントディレクトリ変更
        F_set_ppath4program()

        If file_exist("HLS_option.txt") = 0 Then
            MsgBox("HLS_option.txtが見つかりません。")
            '終了
            Close()
        End If

        log1write(version)
    End Sub

    Private Function F_check_cmd() As Integer
        'とりあえずコマンドラインに「-dok」(二重起動OK）があるかどうかだけチェック
        Dim r As Integer = 0

        Dim cmd_name() As String = Nothing
        Dim cmd_value() As String = Nothing
        Dim i, j As Integer

        Dim chk As Integer = 0
        For Each cmd As String In My.Application.CommandLineArgs
            If (cmd.Substring(0, 1) = "-") Then
                chk = 1
                ReDim Preserve cmd_name(j)
                ReDim Preserve cmd_value(j)
                cmd_name(j) = trim8(cmd)
                cmd_value(j) = ""
                j += 1
            Else
                If chk = 0 Then
                    'パラメーター指定無し
                Else
                    'value
                    Try
                        If j > 0 Then
                            cmd_value(j - 1) &= " " & trim8(cmd)
                        End If
                    Catch ex As Exception
                    End Try
                End If
                chk = 0
            End If
        Next

        If j > 0 Then
            For i = 0 To j - 1
                cmd_name(i) = trim8(cmd_name(i)).ToLower '小文字
                cmd_value(i) = trim8(cmd_value(i))

                Select Case cmd_name(i)
                    Case "-dok"
                        '二重起動を許可
                        r = 1
                End Select
            Next
        End If

        Return r
    End Function

    Private Sub Form1_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        'ニコニコ実況用サービスIDとjkチャンネルとの対応表を読み込む
        ch_sid_load()

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

        'チャンネル情報を取得　今までは表示要求があった時点で１つ１つ取得していた
        Me._worker.WI_GET_CHANNELS()

        'iniからパラ－メータを読み込む
        Me._worker.read_videopath()

        '関連アプリのプロセスが残っていれば停止する
        '全プロセスを名前指定で停止
        Me._worker.stopProc(-2)

        'HTMLキャラクターコード
        log1write("HTML入力キャラクターコード： " & HTML_IN_CHARACTER_CODE)
        log1write("HTML出力キャラクターコード： " & HTML_OUT_CHARACTER_CODE)

        'ffmpegバッファ
        log1write("ffmepg HTTPストリームバッファ：　" & HTTPSTREAM_FFMPEG_BUFFER & "MB")

        'アイドル切断分数
        If STOP_IDLEMINUTES > 0 Then
            log1write("アイドル時間が" & STOP_IDLEMINUTES & "分に達すると全切断するようセットしました")
        End If
        STOP_IDLEMINUTES_LAST = Now()

        If Me._worker._AddSubFolder = 1 Then
            If Me._worker._videopath IsNot Nothing Then
                'サブフォルダを含める
                Dim sf As New ArrayList
                Dim errf As New ArrayList
                For j = 0 To Me._worker._videopath.Length - 1
                    GetSubfolders(Me._worker._videopath(j), sf, errf)
                    errf.Add("RECYCLER") '"RECYCLER"を除外する
                    errf.Add("chapters") 'chaptersを除外する
                Next
                'ここまでで、sf.arrayにフォルダ、errf.arrayにエラーフォルダ
                Dim folder As String
                For Each folder In sf
                    Dim chk As Integer = 0
                    Dim errfolder As String
                    For Each errfolder In errf
                        If folder.IndexOf(errfolder) >= 0 Then
                            chk = 1
                            Exit For
                        End If
                    Next
                    If chk = 0 Then
                        'video_path()に追加
                        Dim b As Integer = Me._worker._videopath.Length
                        ReDim Preserve Me._worker._videopath(b)
                        Me._worker._videopath(b) = folder
                    End If
                Next folder
            End If
        End If
        If Me._worker._videopath IsNot Nothing Then
            For j = 0 To Me._worker._videopath.Length - 1
                log1write("ファイルフォルダ " & Me._worker._videopath(j))
            Next
        End If

        '起動時にビデオファイル一覧を作成
        Me._worker.WI_GET_VIDEOFILES2("", 1, C_DAY2038, 0)

        '解像度コンボボックスをセット httpサーバースタート後にhls_option()がセットされている
        search_ComboBoxResolution()

        'フォーム上の項目が正常かどうかチェック
        check_form_youso()

        'プロセスクラッシュ監視等開始
        Timer1.Enabled = True

        'ビデオフォルダ　更新監視スタート
        start_watch_folders()

        '無事起動
        TvRemoteViewer_VB_Start = 1
    End Sub

    'フォーム上の項目が正常かどうかチェック
    Private Sub check_form_youso()
        'UDPアプリチェック
        Dim f_udp_exe As String = Me.textBoxUdpApp.Text.ToString
        If file_exist(f_udp_exe) = 1 Then
            log1write("起動チェック　UDPアプリ：OK")
        Else
            log1write("【エラー】UDPアプリ " & f_udp_exe & " が見つかりません")
        End If
        'HLSアプリチェック
        Dim f_hls_exe As String = Me.textBoxHlsApp.Text.ToString
        If file_exist(f_hls_exe) = 1 Then
            log1write("起動チェック　HLSアプリ：OK")
        Else
            log1write("【エラー】HLSアプリ " & f_hls_exe & " が見つかりません")
        End If
        'ffmpegプリセットチェック
        If f_hls_exe.IndexOf("ffmpeg.exe") >= 0 Then
            Dim f_hlsopt As String = Me.textBoxHlsOpt2.Text.ToString
            If f_hlsopt.Length > 0 Then
                Dim f_fpre_str As String = Instr_pickup(f_hlsopt, "-fpre """, """", 0)
                If f_fpre_str.Length > 0 Then
                    Dim f_hls_path As String = filepath2path(f_hls_exe) './
                    Dim f_hls_path2 As String = filepath2path(f_hls_path) ' ../
                    f_fpre_str = f_fpre_str.Replace("%HLSROOT%", f_hls_path)
                    f_fpre_str = f_fpre_str.Replace("%HLSROOT/../%", f_hls_path2)
                    If file_exist(f_fpre_str) = 1 Then
                        log1write("起動チェック　ffmpeg presetsファイル：OK")
                    Else
                        log1write("【エラー】ffmpegプリセット " & f_fpre_str & " が見つかりません")
                    End If
                End If
            Else
                log1write("【警告】HLSオプションが設定されていません")
            End If
        End If
        'wwwroot
        Dim f_wwwroot As String = Me.TextBoxWWWroot.Text.ToString
        If f_wwwroot.Length > 0 Then
            If folder_exist(f_wwwroot) = 1 Then
                log1write("起動チェック　WWWROOT：OK")
            Else
                log1write("【エラー】WWWROOT " & f_wwwroot & " が見つかりません")
            End If
        Else
            log1write("【エラー】WWWROOTが設定されていません")
        End If
        'fileroot
        Dim f_fileroot As String = Me.TextBoxFILEROOT.Text.ToString
        If f_fileroot.Length > 0 Then
            If folder_exist(f_fileroot) = 1 Then
                log1write("起動チェック　FILEROOT：OK")
            Else
                log1write("【エラー】FILEROOT " & f_fileroot & " が見つかりません")
            End If
        End If
        'bondriver
        Dim f_bondriver As String = Me.TextBoxBonDriverPath.Text.ToString
        If f_bondriver.Length = 0 Then
            Try
                f_bondriver = IO.Path.GetDirectoryName(Me.textBoxUdpApp.Text.ToString)
            Catch ex As Exception
                f_bondriver = ""
            End Try
        End If
        If f_bondriver.Length > 0 Then
            If folder_exist(f_bondriver) = 1 Then
                log1write("起動チェック　BonDriverパス：OK")
                'Bondriverが存在するかまたch2が存在するかチェック
                Dim bchk As Integer = 0
                Try
                    For Each stFilePath As String In System.IO.Directory.GetFiles(f_bondriver, "*.dll")
                        If System.IO.Path.GetExtension(stFilePath) = ".dll" Then
                            Dim s As String = trim8(stFilePath & System.Environment.NewLine)
                            Dim bonfile As String = IO.Path.GetFileName(s).ToLower 'ファイル名
                            '表示しないBonDriverかをチェック
                            If BonDriver_NGword IsNot Nothing Then
                                For i As Integer = 0 To BonDriver_NGword.Length - 1
                                    If bonfile.IndexOf(BonDriver_NGword(i)) >= 0 Then
                                        bonfile = ""
                                    End If
                                Next
                            End If
                            If bonfile.IndexOf("_file") >= 0 Or bonfile.IndexOf("_udp") >= 0 Or bonfile.IndexOf("_pipe") >= 0 Then
                                bonfile = ""
                            End If
                            If bonfile.IndexOf("bondriver") = 0 Then
                                bchk += 1
                                Dim ch2file As String = f_bondriver & "\" & IO.Path.GetFileNameWithoutExtension(s) & ".ch2"
                                If file_exist(ch2file) <= 0 Then
                                    log1write("【警告】" & bonfile & " に対応するch2ファイルが見つかりませんでした")
                                End If
                            End If
                        End If
                    Next
                Catch ex As Exception
                    log1write("【エラー】Bondriver一覧取得中にエラーが発生しました。" & ex.Message)
                End Try
                If bchk > 0 Then
                    log1write("起動チェック　BonDriver：OK")
                Else
                    log1write("【エラー】" & f_bondriver & " にBondriverが見つかりませんでした")
                End If
            Else
                log1write("【エラー】BonDriverパス " & f_bondriver & " が見つかりません")
            End If
        Else
            log1write("【警告】BonDriverパスが指定されていません")
        End If
        'ASS字幕用font_confがあるかどうか確認
        If Me._worker._hlsApp.IndexOf("ffmpeg") >= 0 Then
            If file_exist(Me._worker._hlsApp.Replace("ffmpeg.exe", "\fonts\fonts.conf")) = 1 Then
                fonts_conf_ok = 1
                log1write("起動チェック　ASS字幕に必要なfonts.conf：OK")
            Else
                log1write("ffmepgファイル再生時：ASS字幕を表示させるのに必要なfonts.confは見つかりませんでした")
            End If
        End If

        'RAMドライブに作成されることを考慮して存在しない場合は作成
        Dim fileroot As String = TextBoxFILEROOT.Text.ToString
        If fileroot.Length > 0 Then
            Dim sp As Integer = fileroot.LastIndexOf("\")
            If sp > 0 Then
                Dim s As String = fileroot.Substring(0, sp)
                'フォルダが存在するか確認し、無ければ作成
                Try
                    If System.IO.Directory.Exists(fileroot) Then
                    Else
                        log1write("【フォルダ作成】%FILEROOT%が存在しません。" & fileroot & " を作成しました")
                        System.IO.Directory.CreateDirectory(fileroot)
                    End If
                Catch ex As Exception
                End Try
            ElseIf fileroot.IndexOf(":") >= 0 Then
                log1write("【エラー】%FILEROOT%にドライブそのものを指定することはできません。ドライブ内フォルダを指定してください")
            ElseIf fileroot = "\\" Then
                log1write("【エラー】%FILEROOT%にネットワークドライブそのものを指定することはできません。ネットワークドライブ内フォルダを指定してください")
            Else
                log1write("【エラー】%FILEROOT%が不正です")
            End If
        End If

    End Sub

    'サブフォルダを取得
    Public Sub GetSubfolders(ByVal folderName As String, ByRef subFolders As ArrayList, ByRef errFolders As ArrayList)
        Dim folder As String
        Try
            For Each folder In System.IO.Directory.GetDirectories(folderName)
                'リストに追加
                subFolders.Add(folder)
                '再帰的にサブフォルダを取得する
                GetSubfolders(folder, subFolders, errFolders)
            Next folder
        Catch ex As Exception
            errFolders.Add(folderName)
            Exit Sub
        End Try
    End Sub

    'ウィンドウの位置を復元
    Public Sub F_window_set()
        'カレントディレクトリ変更
        F_set_ppath4program()

        Dim line() As String = file2line("form_status.txt")
        If line IsNot Nothing Then
            Dim i As Integer
            For i = 0 To line.Length - 1
                Dim lr() As String = line(i).Split("=")
                If lr.Length = 2 Or (lr.Length > 2 And Trim(lr(0)) = "PASS") Then
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
                        Case "textBoxUdpOpt3"
                            TextBoxUdpOpt3.Text = lr(1)
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
                        Case "ID"
                            TextBoxID.Text = lr(1)
                        Case "PASS"
                            If lr.Length > 2 Then
                                For j As Integer = 2 To lr.Length - 1
                                    lr(1) &= "=" & lr(j)
                                Next
                            End If
                            TextBoxPASS.Text = DecryptString(lr(1), TextBoxID.Text.ToString & "TRVVB")
                        Case "ComboBoxHLSorHTTP"
                            ComboBoxHLSorHTTP.Text = lr(1)
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

        'ビデオフォルダ　監視終了
        stop_watch_videofolders()

        '全プロセスを停止
        Try
            Me._worker.stopProc(-2)
        Catch ex As Exception
        End Try

        'ウィンドウの位置を保存
        If TvRemoteViewer_VB_Start = 1 Then
            save_form_status()
        End If

        'Webスレッド停止
        Try
            Me._worker.requestStop()
            Me._webThread.Abort()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
    End Sub

    Private Sub save_form_status()
        Dim s As String = ""

        s &= "TextBoxWWWroot=" & TextBoxWWWroot.Text & vbCrLf
        s &= "TextBoxFILEroot=" & TextBoxFILEROOT.Text & vbCrLf
        s &= "textHttpPortNumber=" & textHttpPortNumber.Text & vbCrLf
        s &= "textBoxUdpApp=" & textBoxUdpApp.Text & vbCrLf
        s &= "TextBoxBonDriverPath=" & TextBoxBonDriverPath.Text & vbCrLf
        s &= "textBoxUdpPort=" & textBoxUdpPort.Text & vbCrLf
        s &= "textBoxUdpOpt3=" & TextBoxUdpOpt3.Text & vbCrLf
        s &= "textBoxHlsApp=" & textBoxHlsApp.Text & vbCrLf
        's &= "textBoxHlsOpt1=" & textBoxHlsOpt1.Text & vbCrLf
        s &= "ComboBoxNum=" & ComboBoxNum.Text & vbCrLf
        s &= "ComboBoxBonDriver=" & ComboBoxBonDriver.Text & vbCrLf
        s &= "TextBoxChSpace=" & TextBoxChSpace.Text & vbCrLf
        s &= "ComboBoxServiceID=" & ComboBoxServiceID.Text & vbCrLf
        s &= "ComboBoxResolution=" & ComboBoxResolution.Text & vbCrLf
        s &= "CheckBoxShowConsole=" & CheckBoxShowConsole.Checked & vbCrLf
        s &= "ID=" & TextBoxID.Text & vbCrLf
        s &= "PASS=" & EncryptString(TextBoxPASS.Text.ToString, TextBoxID.Text.ToString & "TRVVB") & vbCrLf
        s &= "textBoxHlsOpt=" & textBoxHlsOpt2.Text & vbCrLf
        s &= "ComboBoxHLSorHTTP=" & ComboBoxHLSorHTTP.Text

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
            For i As Integer = 0 To Me._worker.hls_option.Length - 1
                ComboBoxResolution.Items.Add(Me._worker.hls_option(i).resolution)
            Next
        Catch ex As Exception
        End Try
    End Sub

    '解像度コンボボックスの値が変更されたとき
    Private Sub ComboBoxResolution_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxResolution.SelectedIndexChanged
        Dim s As String = ComboBoxResolution.Text
        For i As Integer = 0 To Me._worker.hls_option.Length - 1
            If Me._worker.hls_option(i).resolution = s Then
                textBoxHlsOpt2.Text = Me._worker.hls_option(i).opt
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
                If System.IO.Path.GetExtension(stFilePath) = ".dll" Then
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
                End If
            Next
        Catch ex As Exception
            log1write("Bondriver一覧取得中にエラーが発生しました。" & ex.Message)
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
        Dim filename As String = ""
        If ComboBoxBonDriver.Text.ToString.Length > 0 Then
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
            'カレントディレクトリ変更
            F_set_ppath4program()
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
        search_BonDriver()
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

    '項目が変更されたことをインスタンスに知らせる
    Private Sub TextBoxUdpOpt3_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxUdpOpt3.TextChanged
        Try
            Me._worker._udpOpt3 = TextBoxUdpOpt3.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub TextBoxID_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxID.TextChanged
        Try
            Me._worker._id = TextBoxID.Text.ToString
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub TextBoxPASS_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxPASS.TextChanged
        Try
            Me._worker._pass = TextBoxPASS.Text.ToString
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

    '初期値ボタン
    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        TextBoxUdpOpt3.Text = "/sendservice 1"
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

    'ビデオフォルダ　監視開始
    Public Sub start_watch_folders()
        If Me._worker._videopath Is Nothing Then
            Exit Sub
        End If

        'watcher = New System.IO.FileSystemWatcher
        watcher = New System.IO.FileSystemWatcher(UBound(Me._worker._videopath)) {}
        Dim j As Integer
        For j = 0 To UBound(Me._worker._videopath)
            watcher(j) = New System.IO.FileSystemWatcher
        Next

        For i = 0 To Me._worker._videopath.Length - 1
            Try
                '監視するディレクトリを指定
                watcher(i).Path = trim8(Me._worker._videopath(i))
                If watcher(i).Path.Length > 0 Then
                    '最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                    watcher(i).NotifyFilter = System.IO.NotifyFilters.LastAccess Or _
                    System.IO.NotifyFilters.LastWrite Or _
                    System.IO.NotifyFilters.FileName Or _
                    System.IO.NotifyFilters.DirectoryName
                    'すべてのファイルを監視
                    watcher(i).Filter = ""
                    'サブディレクトリは監視しない
                    watcher(i).IncludeSubdirectories = False
                    'UIのスレッドにマーシャリングする
                    'コンソールアプリケーションでの使用では必要ない
                    'watcher.SynchronizingObject = Me

                    'イベントハンドラの追加
                    AddHandler watcher(i).Changed, AddressOf watcher_Changed
                    AddHandler watcher(i).Created, AddressOf watcher_Changed
                    AddHandler watcher(i).Deleted, AddressOf watcher_Changed
                    AddHandler watcher(i).Renamed, AddressOf watcher_Changed

                    '監視を開始する
                    watcher(i).EnableRaisingEvents = True
                End If
            Catch ex As Exception
                log1write("ビデオフォルダ " & Me._worker._videopath(i) & " の監視開始においてエラーが発生しました。" & ex.Message)
            End Try
        Next
        log1write("ビデオフォルダの監視を開始しました。")
    End Sub

    'ビデオフォルダ　監視終了
    Public Sub stop_watch_videofolders()
        '監視を終了
        If watcher IsNot Nothing Then
            For i = 0 To watcher.Length - 1
                Try
                    watcher(i).EnableRaisingEvents = False
                    watcher(i).Dispose()
                    watcher(i) = Nothing
                Catch ex As Exception
                    log1write("ビデオフォルダ監視終了においてエラーが発生しました。" & ex.Message)
                End Try
            Next
            watcher = Nothing
            log1write("ビデオフォルダの監視を終了しました。")
        End If
    End Sub

    'ビデオフォルダ　イベントハンドラ
    Private Sub watcher_Changed(ByVal source As System.Object, ByVal e As System.IO.FileSystemEventArgs)
        'もしかして並列ではなく直列に実行されているみたい・・
        If e.FullPath.ToString.IndexOf("Thumbs.db") < 0 Then

            Select Case e.ChangeType
                Case System.IO.WatcherChangeTypes.Changed
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が変更されました。"))
                Case System.IO.WatcherChangeTypes.Created
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が作成されました。"))
                Case System.IO.WatcherChangeTypes.Deleted
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が削除されました。"))
                Case System.IO.WatcherChangeTypes.Renamed
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が名前変更されました。"))
            End Select

            watcher_lasttime = Now()
            '最後の変更からタイマーで10秒経ったら更新
        End If
    End Sub

    Private Sub ButtonCopy2Clipboard_Click(sender As System.Object, e As System.EventArgs) Handles ButtonCopy2Clipboard.Click
        Try
            'クリップボードに文字列をコピーする
            Clipboard.SetText(TextBoxLog.Text.ToString)
        Catch ex As Exception
            log1write("ログのクリップボードへのコピーに失敗しました。" & ex.Message)
        End Try
    End Sub
End Class
