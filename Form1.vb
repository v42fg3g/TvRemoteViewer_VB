Imports System
Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions

Public Class Form1
    Private chk_timer1 As Integer = 0 'timer1重複回避用temp
    Private chk_timer1_deleteTS As Integer = 0 '古いtsファイルを削除する間隔調整用
    Private ServiceID_temp As String '起動時、前回終了時のサービスIDを復活するための一時待避用temp

    'TvRemoteViewer_VBが無事スタートしたら1
    Public TvRemoteViewer_VB_Start As Integer = 0
    Public TvRemoteViewer_VB_Start_utime As Integer = 0

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
            Dim udpApp As String = path_s2z(textBoxUdpApp.Text.ToString)
            Dim udpOpt3 As String = TextBoxUdpOpt3.Text.ToString
            Dim hlsApp As String = path_s2z(textBoxHlsApp.Text.ToString)
            Dim hlsroot As String = ""
            Dim ss As String = "\"
            Dim sp As Integer = hlsApp.LastIndexOf(ss)
            If sp > 0 Then
                hlsroot = hlsApp.Substring(0, sp)
            End If
            Dim hlsOpt1 As String = "" 'textBoxHlsOpt1.Text.ToString
            Dim hlsOpt2 As String = textBoxHlsOpt2.Text.ToString
            Dim wwwroot As String = path_s2z(TextBoxWWWroot.Text.ToString)
            Dim fileroot As String = path_s2z(TextBoxFILEROOT.Text.ToString)
            Dim s() As String = ComboBoxServiceID.Text.Split(",")
            Dim ShowConsole As Boolean = CheckBoxShowConsole.Checked
            Dim NHK_dual_mono_mode_select As Integer = 0 'フォームからは指定しない常に０
            Dim HLSorHTTP As Integer = ComboBoxHLSorHTTP.SelectedIndex * 2 '0or2
            '解像度
            Dim resolution As String = ""
            'If ComboBoxRezFormOrCombo.Text.IndexOf("解像度") >= 0 Then
            'resolution = ComboBoxResolution.Text.ToString
            'hlsOpt2 = ""
            'End If
            If num > 0 Then
                'If fileroot.IndexOf(wwwroot) = 0 Or fileroot.Length = 0 Then
                If bondriver.IndexOf(".dll") > 0 Then
                    If s.Length = 3 Then
                        sid = Val(s(1))
                        Dim filename As String = "" 'UDP配信モード　フォームからはUDP配信モード限定
                        Me._worker.start_movie(num, bondriver, sid, chspace, udpApp, hlsApp, hlsOpt1, hlsOpt2, wwwroot, fileroot, hlsroot, ShowConsole, udpOpt3, filename, NHK_dual_mono_mode_select, HLSorHTTP, resolution, 0, 0, "1", "", 0, "", "", 0, Nothing)
                    Else
                        MsgBox("サービスIDを指定してください")
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
        Dim live_chk As Integer = -1
        Dim t As DateTime = Now()
        If chk_timer1 = 0 Then '重複実行防止
            chk_timer1 = 1

            'VLCのcrashダイアログが出ていたら消す
            If Diagnostics.Process.GetProcessesByName("vlc").Length > 0 Then
                Me._worker.check_crash_dialog()
            End If

            'プロセスがうまく動いているかチェック
            Me._worker.checkAllProc()

            'ffmpeg.exeを使用している場合は、1分間に1回古いTSを削除する
            If OLDTS_NODELETE = 0 Then
                If chk_timer1_deleteTS >= 60 And Me._worker.check_hlsApp_in_stream("ffmpeg,QSVEnc,NVEnc") = 1 Then
                    delete_old_TS()
                    chk_timer1_deleteTS = 0
                End If
                chk_timer1_deleteTS += 1
            End If

            '現在稼働中のストリームをタスクトレイアイコンのマウスオーバー時に表示する
            Dim s As String = ""
            If log_debug = 1 Then
                s = Me._worker.get_live_numbers(1) 'デバッグ _list数表示
            Else
                s = Me._worker.get_live_numbers()
            End If
            Dim s_temp As String = s
            If s_temp.IndexOf("(") > 0 Then
                s_temp = s_temp.Substring(0, s_temp.IndexOf("("))
            End If
            live_chk = Trim(s_temp).Length
            If live_chk > 0 Then
                LabelStream.Text = "配信中：" & s 'ついでにフォーム上にも表示
                s = "TvRemoteViewer_VB" & vbCrLf & "配信中：" & Trim(s)
            Else
                LabelStream.Text = " " 'ついでにフォーム上にも表示
                s = "TvRemoteViewer_VB " & Format(TvRemoteViewer_VB_version, "0.00") & TvRemoteViewer_VB_revision '"TvRemoteViewer_VB"
            End If
            If s.Length > 60 Then
                '64文字を超えるとエラーになる
                s = s.Substring(0, 60) & ".."
            End If
            'アップデートの必要性の有無
            If TvRemoteViewer_VB_notrecommend_version > 0 And TvRemoteViewer_VB_version <= TvRemoteViewer_VB_notrecommend_version Then
                s = "【警告】アップデートのお願い　非推奨バージョンです"
                TvRemoteViewer_VB_version_NG = 1 'NGフラグON
                LabelVersionWarning.Visible = True 'フォーム上に警告を表示
            End If
            Try
                NotifyIcon1.Text = s
            Catch ex As Exception
                NotifyIcon1.Text = "TvRemoteViewer_VB " & Format(TvRemoteViewer_VB_version, "0.00") & TvRemoteViewer_VB_revision  '"TvRemoteViewer_VB"
            End Try

            'アイドル時間が指定分に達した場合は全て切断する
            If STOP_IDLEMINUTES > 0 Then
                If (Now() - STOP_IDLEMINUTES_LAST).Minutes >= STOP_IDLEMINUTES Then
                    STOP_IDLEMINUTES_LAST = CDate("2199/12/31 23:59:59")
                    log1write("アイドル時間が" & STOP_IDLEMINUTES.ToString & "分に達しましたので全切断します")
                    Me._worker.stop_movie(-3)
                End If
            End If

            'サムネイル作成が終了したかどうかチェック
            If making_per_thumbnail IsNot Nothing Then
                If making_per_thumbnail.Length > 0 Then
                    Dim ut As Integer = time2unix(Now())
                    For i = 0 To making_per_thumbnail.Length - 1
                        If making_per_thumbnail(i).indexofstr.Length > 0 Then
                            Try
                                If making_per_thumbnail(i).process.HasExited = True Then
                                    '終了している
                                    log1write(making_per_thumbnail(i).fullpathfilename & "の一定間隔サムネイル作成が終了しました")
                                    making_per_thumbnail(i).indexofstr = ""
                                ElseIf making_per_thumbnail(i).indexofstr.Length > 0 Then
                                    If (ut - making_per_thumbnail(i).unixtime) > stop_per_thumbnail_minutes Then
                                        '開始して指定秒数以上経過した案件がある場合プロセスを終了させる
                                        Try
                                            'プロセスを終了させる
                                            Try
                                                making_per_thumbnail(i).process.Kill()
                                            Catch ex As Exception
                                            End Try
                                        Catch ex As Exception
                                        End Try
                                        log1write(making_per_thumbnail(i).fullpathfilename & "は等間隔サムネイル作成開始から" & stop_per_thumbnail_minutes.ToString & "秒経過していたので作成プロセスを破棄しました")
                                        making_per_thumbnail(i).indexofstr = ""
                                        Exit For
                                    End If
                                End If
                            Catch ex As Exception
                                '存在していない
                                log1write(making_per_thumbnail(i).fullpathfilename & "の一定間隔サムネイルプロセスが見つかりません")
                                making_per_thumbnail(i).indexofstr = ""
                            End Try
                        End If
                    Next
                End If
            End If

            chk_timer1 = 0
        End If

        '現在時刻unixtime
        Dim ut2 As Integer = time2unix(Now())

        'スリープ抑止解除
        If DisableSleep_ON = 1 Then
            If ut2 - sleep_stopping_utime >= 60 Then
                '最後の.tsアクセスから1分以上経過した場合はスリープ抑止解除
                DisableSleep(0)
            End If
        End If

        '6時間に1回バージョンチェック
        If TvRemoteViewer_VB_version_check_on = 1 Then
            LabelVersionCheckDate.Text = TvRemoteViewer_VB_version_check_datetime
            Dim ct As Integer = time2unix(TvRemoteViewer_VB_version_check_datetime)
            If ut2 - ct > 3600 * 6 + 180 Then
                check_version_multi() '邪魔にならないようマルチスレッドで確認
                log1write("推奨バージョンチェックを行いました")
            End If
        End If

        '指定時間に1回カスタムOutside番組表チェック(タイマー起動直後にも実行される）
        If Outside_CustomURL.Length > 0 Then
            If ut2 - Outside_CustomURL_getutime > (Outside_Program_get_interval_min * 60) Then
                check_Outside_CustomURL_multi()
            End If
        End If

        '1時間に1回TvRockPC用番組表を取得（ランチャー用ジャンル判別のため）
        If TvRock_genre_ON = 1 And TvProgram_tvrock_url.Length > 0 Then
            If TvProgram_ch IsNot Nothing And TvRock_genre_color IsNot Nothing Then
                If ut2 - TvRock_html_getutime > (3600 * 1 - 180) Then
                    TvRock_html_getutime = ut2
                    check_TvRock_Program_PC_multi()
                End If
            End If
        End If

        If live_chk <= 0 And (form1_ID.Length = 0 Or form1_PASS.Length = 0) Then
            '10分に1度アクセス元チェック
            If (ut2 - TvRemoteViewer_VB_Start_utime) Mod 600 = 500 Then
                AccessLogListCheck()
            End If
        End If

        'ログ処理
        If log1 <> log1_dummy Then
            If log1.Length > log_size Then
                'log_size文字以上になったらカット
                log1 = log1.Substring(0, log_size)
            End If
            log1_dummy = log1
            show_log() 'フォーム上へ表示
        End If

        'ファイル一覧　最後の更新から10秒経ったらファイル一覧更新
        Dim duration1 As TimeSpan = Now.Subtract(watcher_lasttime)
        If (live_chk > 0 Or live_chk = -1) And duration1.TotalSeconds < 3700 Then
            'ストリーム再生中、またはノーチェックならファイル一覧は更新しない
            'が、少なくとも1時間に1回は更新する
        Else
            If duration1.TotalSeconds >= 10 Then
                watcher_lasttime = C_DAY2038
                Me._worker.make_file_select_html("", 1, C_DAY2038, 0)
            End If
        End If

        If NoUseProgramCache = 0 Then
            Dim rnd As Integer = TvRemoteViewer_VB_Start_utime Mod 10 'アクセスが集中しないよう配慮
            If Second(t) >= 10 + rnd And Second(t) <= 15 + rnd And now_caching = 0 Then
                now_caching = 1 '取りこぼしのないよう
                'キャッシュ切れ寸前のものを前もって先読み
                If pcache IsNot Nothing Then
                    For i As Integer = 0 To pcache.Length - 1
                        If pcache(i).max_utime - time2unix(Now()) < 55 Then
                            Dim th As New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf update_pcache))
                            th.Start()
                            Exit For
                        End If
                    Next
                End If
            ElseIf Second(t) > 50 Then
                now_caching = 0
            End If
        End If
    End Sub

    'マルチスレッドで番組表キャッシュ先読み
    Private now_caching As Integer = 0
    Private Sub update_pcache()
        If pcache IsNot Nothing Then
            For i As Integer = 0 To pcache.Length - 1
                If pcache(i).max_utime - time2unix(Now()) < 50 Then
                    '現在時刻を1分後にセットして実行
                    Dim t2 As DateTime = DateAdd(DateInterval.Minute, 1, Now())
                    log1write("番組表を先読みします。取得設定日時 " & t2.ToString)
                    'キャッシュ作成が目的なのでgetnextは指定しなくて良い
                    Dim getnext As Integer = 0
                    get_TVprogram_now(pcache(i).str, getnext, t2)
                End If
            Next
        End If
    End Sub

    'ログ表示
    Private Sub show_log()
        If Me.WindowState <> FormWindowState.Minimized And (CheckBoxLogReq.Checked = False Or CheckBoxLogWI.Checked = False Or CheckBoxLogETC.Checked = False) Then
            '手の込んだログ表示
            TextBoxLog.Text = edit_log(log1)
            TextBoxLog.Refresh()
        Else
            TextBoxLog.Text = log1
            TextBoxLog.Refresh()
        End If
    End Sub

    'ログ整形
    Private Function edit_log(ByVal log1 As String) As String
        Dim vlog1 As String = ""
        Dim line() As String = Split(log1, vbCrLf)
        Dim i As Integer = 0
        For Each s As String In line
            If s.IndexOf("リクエストがありました") > 0 Then
                If CheckBoxLogWI.Checked = False And s.IndexOf("/WI_") >= 0 Then
                    s = ""
                End If
                If CheckBoxLogReq.Checked = False And s.IndexOf("/WI_") < 0 Then
                    s = ""
                End If
            ElseIf CheckBoxLogETC.Checked = False Then
                s = ""
            End If
            If s.Length > 0 Then
                line(i) = s
                i += 1
            End If
        Next
        If i > 0 Then
            '余計な行を削除
            If i < line.Length Then
                For j As Integer = i To line.Length - 1
                    line(j) = ""
                Next
            End If
            vlog1 = String.Join(vbCrLf, line) '結合
            vlog1 = System.Text.RegularExpressions.Regex.Replace(vlog1, "[\r\n]+$", "") & vbCrLf '余計な改行を削除
        Else
            vlog1 = ""
        End If

        Return vlog1
    End Function

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
        Dim udpApp As String = path_s2z(Me.textBoxUdpApp.Text.ToString)
        Dim udpPort As Integer = Val(Me.textBoxUdpPort.Text.ToString)
        Dim udpOpt3 As String = Me.TextBoxUdpOpt3.Text.ToString
        Dim chSpace As Integer = Val(Me.TextBoxChSpace.Text.ToString)
        Dim hlsApp As String = path_s2z(Me.textBoxHlsApp.Text.ToString)
        Dim hlsOpt1 As String = "" 'Me.textBoxHlsOpt1.Text.ToString
        Dim hlsOpt2 As String = Me.textBoxHlsOpt2.Text.ToString
        Dim wwwroot As String = path_s2z(Me.TextBoxWWWroot.Text.ToString)
        Dim fileroot As String = path_s2z(Me.TextBoxFILEROOT.Text.ToString)
        Dim wwwport As Integer = Val(Me.textHttpPortNumber.Text.ToString)
        Dim num As Integer = Val(Me.ComboBoxNum.Text.ToString)
        Dim BonDriverPath As String = path_s2z(Me.TextBoxBonDriverPath.Text.ToString)
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

    'IEでhttp://127.0.0.1:ポート/を開く
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim port As String = textHttpPortNumber.Text.ToString
        Try
            System.Diagnostics.Process.Start("http://127.0.0.1:" & port)
        Catch ex As Exception
            '開けないファイルだった場合
        End Try
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

        If file_exist("CtrlCmdCLI.dll") <= 0 Then
            MsgBox("CtrlCmdCLI.dllが見つかりません" & vbCrLf & "TvRemoteViewer_VB.exeと同じフォルダにコピーしてください")
            '終了
            Close()
        End If
        If file_exist("ICSharpCode.SharpZipLib.dll") <= 0 Then
            MsgBox("SharpZipLib.dllが見つかりません" & vbCrLf & "TvRemoteViewer_VB.exeと同じフォルダにコピーしてください")
            '終了
            Close()
        End If

        Dim verstr As String = Format(TvRemoteViewer_VB_version, "0.00")
        log1write("TvRemoteViewer_VB " & verstr & TvRemoteViewer_VB_revision)

        Try
            NotifyIcon1.Text = "TvRemoteViewer_VB " & Format(TvRemoteViewer_VB_version, "0.00") & TvRemoteViewer_VB_revision  '"TvRemoteViewer_VB"
            NotifyIcon1.Icon = My.Resources.TvRemoteViewer_VB3
        Catch ex As Exception
            log1write("【エラー】NotifyIcon1の初期化に失敗しました。" & ex.Message)
        End Try

        '最初はini設定欄を非表示　と思ったが起動時エラー注意喚起を表示するため表示するようにした
        'Me.Width = 546
    End Sub

    Private Sub check_version_multi()
        Dim t As New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf check_version))
        'スレッドを開始する
        t.Start()
    End Sub

    Private Function check_version() As Integer
        Dim r As Integer = 0

        TvRemoteViewer_VB_version_check_datetime = Now()

        Dim s As String = get_html_by_webclient(TvRemoteViewer_VB_version_URL, "shift_jis", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36")
        If s.Length > 8 Then
            Dim line() As String = Split(s, vbCrLf)
            If line IsNot Nothing Then
                For i As Integer = 0 To line.Length - 1
                    Dim d() As String = line(i).Split("=")
                    If d.Length = 2 Then
                        Select Case Trim(d(0))
                            Case "notrecommend"
                                Try
                                    TvRemoteViewer_VB_notrecommend_version = Double.Parse(Trim(d(1)))
                                Catch ex As Exception
                                    log1write("【エラー】非推奨バージョン番号が不正です。[" & Trim(d(1)) & "]")
                                End Try
                            Case "recommend"
                                Try
                                    TvRemoteViewer_VB_recommend_version = Double.Parse(Trim(d(1)))
                                Catch ex As Exception
                                    log1write("【エラー】推奨バージョン番号が不正です。[" & Trim(d(1)) & "]")
                                End Try
                        End Select
                    End If
                Next
                If TvRemoteViewer_VB_notrecommend_version = 0 Then
                    log1write("非推奨バージョンが取得できませんでした")
                End If
                If TvRemoteViewer_VB_recommend_version = 0 Then
                    log1write("推奨バージョンが取得できませんでした")
                End If
            End If
        Else
            log1write("ネット上から推奨バージョン情報が取得出来ませんでした")
        End If

        Return r
    End Function

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

        'バージョンチェック
        If TvRemoteViewer_VB_version_check_on = 1 Then
            log1write("アップデートチェックを開始しました")
            check_version()
            If TvRemoteViewer_VB_notrecommend_version > 0 And TvRemoteViewer_VB_version <= TvRemoteViewer_VB_notrecommend_version Then
                If TvRemoteViewer_VB_recommend_version > 0 Then
                    MsgBox("TvRemoteViewer_VB " & TvRemoteViewer_VB_version.ToString & "は非推奨バージョンに指定されています" & vbCrLf & TvRemoteViewer_VB_recommend_version.ToString & "以上へアップデートしてください")
                Else
                    MsgBox("TvRemoteViewer_VB " & TvRemoteViewer_VB_version.ToString & "は非推奨バージョンに指定されています" & vbCrLf & "最新バージョンへアップデートしてください")
                End If
                '終了
                Close()
            End If
            log1write("アップデートチェックを行いました")
            If TvRemoteViewer_VB_recommend_version > 0 And TvRemoteViewer_VB_version < TvRemoteViewer_VB_recommend_version Then
                log1write("【お願い】TvRemoteViewer_VB " & TvRemoteViewer_VB_version.ToString & "は推奨バージョン未満です。アップデートを推奨します")
            End If
        Else
            log1write("【お願い】アップデートの有無をチェックしてください")
        End If

        'httpサーバースタート
        log1write("httpサーバーを起動しています")
        StartHttpServer()
        log1write("httpサーバーを起動しました")

        'ファイル操作許可リスト（file_ope_allow.txtが存在すれば）
        Me._worker.load_file_ope_allow_filelist()

        'チャンネル情報を取得　今までは表示要求があった時点で１つ１つ取得していた
        Me._worker.WI_GET_CHANNELS()

        If file_exist("TvRemoteViewer_VB_r.exe") = 1 Then
            RestartExe_path = path_s2z("TvRemoteViewer_VB_r.exe")
            log1write("ini更新適用プログラムが見つかりました。" & RestartExe_path)
        End If

        '標準ini読み込み
        If read_ini_default() = 0 Then
            'TvRemoteViewer_VB.ini.dataが存在しなかった
            Me.ButtonIniCancel.Enabled = False
            Me.ButtonIniApply.Enabled = False
        End If
        'iniからパラ－メータを読み込む
        Me._worker.read_videopath()
        '×ボタン動作
        If close2min = 2 Then
            Me.Visible = False
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.SizableToolWindow
        End If
        'iniを元に設定したパラメータの整合性チェック
        Me._worker.check_ini_parameter()
        'クライアントiniを読み込み
        Me._worker.read_client_ini()
        'フォーム上のiniタブを初期化
        ini_init_tab()

        'コンボボックスの項目をセット
        search_BonDriver()
        search_ServiceID()
        ComboBoxServiceID.Text = ServiceID_temp '前回終了時に選択していたものをセット

        '起動時のスリープ状態を取得
        If viewing_NoSleep = 1 Then
            previousExecutionState = DisableSleepMode()
            If previousExecutionState = 0 Then
                log1write("【エラー】スリープ状態取得に失敗しました")
            Else
                log1write("起動時のスリープ状態を取得しました")
            End If
        End If

        'Outside_CustomURL取得
        If TvProgram_ch IsNot Nothing Then
            If Array.IndexOf(TvProgram_ch, 801) >= 0 Then
                set_Outside_CustomURL()
            Else
                Outside_CustomURL = ""
            End If
        Else
            Outside_CustomURL = ""
        End If

        '関連アプリのプロセスが残っていれば停止する
        '全プロセスを名前指定で停止
        StopUdpAppName = path_s2z(Me.textBoxUdpApp.Text.ToString) '名前指定でストップすべきUDPアプリを記録
        Me._worker.stopProc(-2)

        'HTMLキャラクターコード
        log1write("HTML入力キャラクターコード： " & HTML_IN_CHARACTER_CODE)
        log1write("HTML出力キャラクターコード： " & HTML_OUT_CHARACTER_CODE)
        Select Case html_publish_method
            Case 0
                log1write("HTMLテキストをByte()変換のうえOutputStream Write出力するよう設定しました。html_publish_method=" & html_publish_method)
            Case 1
                log1write("HTMLテキストをStreamWrite WriteLine出力するよう設定しました。html_publish_method=" & html_publish_method)
        End Select

        'ffmpegバッファ
        log1write("ffmepg HTTPストリームバッファ：　" & HTTPSTREAM_FFMPEG_BUFFER & "MB")

        'アイドル切断分数
        If STOP_IDLEMINUTES > 0 Then
            log1write("アイドル時間が" & STOP_IDLEMINUTES & "分に達すると全切断するようセットしました")
        End If

        'プロファイル読込
        If file_exist("profile.txt") = 1 Then
            profiletxt = file2str("profile.txt", "UTF-8")
            log1write("プロファイルを読み込みました")
        End If

        'サブフォルダ監視修正
        'サブフォルダをファイル一覧作成のために追加
        Me._worker.add_subfolder()

        '起動時にビデオファイル一覧を作成
        Me._worker.WI_GET_VIDEOFILES2("", 1, C_DAY2038, 0, "", "")

        '解像度コンボボックスをセット httpサーバースタート後にhls_option()がセットされている
        search_ComboBoxResolution()
        form1_resolution = ComboBoxResolution.Text.ToString
        form1_hls_or_rez = ComboBoxRezFormOrCombo.Text.ToString
        NicoConvAss_ConfigSet = F_set_ComboboxNicoSet(NicoConvAss_ConfigSet)
        ComboBoxNicoSet.Text = NicoConvAss_ConfigSet

        'フォーム上の項目が正常かどうかチェック
        check_form_youso()

        'ISO再生用VLCオプション読み込み
        set_VLC_ISO_option()

        'プロセスクラッシュ監視等開始
        Timer1.Enabled = True

        'ビデオフォルダ　更新監視スタート
        start_watch_folders()

        'EDCB番組表に表示する局（TSID）を取得
        If EDCB_thru_addprogres = 0 Then
            If EDCB_GetCh_method = 0 Then
                'CtrlCmdCLIを使用
                EDCB_GET_TSID_CtrlCmdCLI()
            Else
                '旧方式
                EDCB_GET_TSID()
            End If
        Else
            log1write("【EDCB】EDCB_thru_addprogresが指定されています")
        End If

        'ptTimerが管理するPT2の数を取得
        If ptTimer_path.Length > 0 Then
            pttimer_pt2count = F_get_pt2count()
        End If

        'DVD2 ISO再生用
        ReDim Preserve dvdObject(MAX_STREAM_NUMBER + 1)
        '起動時クリーンアップ： 通常のDVDダンプのクリーンアップの他、作成途中のダンプ(.tmp)ファイルも削除する。
        Try
            DVDClass.CleanupDumpCache(ISO_DumpDirPath, ISO_maxDump, True)
        Catch ex As Exception
            log1write("【エラー】DVDダンプフォルダチェックに失敗しました[Shown]。" & ex.Message)
        End Try

        'エンコ済ファイル再生ストリーム復帰作業
        '■■■ISO新方式に対応しないとかも（保存も）　後で考察
        Me._worker.resume_file_streams()

        'stream_last_utimeの再定義
        ReDim Preserve stream_last_utime(MAX_STREAM_NUMBER + 1)

        'ストリーム再起動回数の再定義
        ReDim Preserve stream_reset_count(MAX_STREAM_NUMBER + 1)

        'waitingmessage_countの再定義
        ReDim Preserve waitingmessage_count(MAX_STREAM_NUMBER + 1)
        ReDim Preserve waitingmessage_str(MAX_STREAM_NUMBER + 1)

        'HLS_option.txtの内容との整合性チェック
        check_hls_option_txt()

        'ViewTV～.htmlが更新されていないかチェック
        Me._worker.check_ViewTVhtml()

        'エラーが発生している場合注意喚起
        log1_show_warning()

        '無事起動
        TvRemoteViewer_VB_Start = 1
        TvRemoteViewer_VB_Start_utime = time2unix(Now())
    End Sub

    Private Sub log1_show_warning()
        Dim err_str As String = ""
        Dim line() As String = Split(log1, vbCrLf)
        For Each s As String In line
            If s.IndexOf("エラー") >= 0 Or s.IndexOf("警告") >= 0 Or s.IndexOf("ません") >= 0 Then
                If s.IndexOf("配信中のストリームlistが存在しません") < 0 Then 'これはOK
                    Try
                        '日時削除
                        Dim sp As Integer = s.IndexOf(" ")
                        If sp > 0 Then
                            sp = s.IndexOf(" ", sp + 1)
                        End If
                        If sp > 0 Then
                            s = s.Substring(sp + 1)
                        End If
                    Catch ex As Exception
                    End Try
                    err_str &= s & vbCrLf
                End If
            End If
        Next
        If err_str.Length > 0 Then
            err_str = "■起動時エラー＆警告■" & vbCrLf & err_str
            Me.TextBoxIniDoc.Text = err_str
        End If
    End Sub

    'フォーム上の項目が正常かどうかチェック
    Private Sub check_form_youso()
        'UDPアプリチェック
        Dim f_udp_exe As String = path_s2z(Me.textBoxUdpApp.Text.ToString)
        If file_exist(f_udp_exe) = 1 Then
            log1write("起動チェック　UDPアプリ：OK")
        Else
            log1write("【エラー】UDPアプリ " & f_udp_exe & " が見つかりません")
        End If
        'HLSアプリチェック
        Dim f_hls_exe As String = path_s2z(Me.textBoxHlsApp.Text.ToString)
        If file_exist(f_hls_exe) = 1 Then
            log1write("起動チェック　HLSアプリ：OK")
        Else
            log1write("【エラー】HLSアプリ " & f_hls_exe & " が見つかりません")
        End If
        'ffmpegプリセットチェック
        If isMatch_HLS(f_hls_exe, "ffmpeg") = 1 Then
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
                        log1write("【警告】ffmpegプリセット " & f_fpre_str & " が見つかりません")
                    End If
                End If
            Else
                log1write("【警告】HLSオプションが設定されていません")
            End If
        End If
        'wwwroot
        Dim f_wwwroot As String = path_s2z(Me.TextBoxWWWroot.Text.ToString)
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
        Dim f_fileroot As String = path_s2z(Me.TextBoxFILEROOT.Text.ToString)
        If f_fileroot.Length > 0 Then
            If folder_exist(f_fileroot) = 1 Then
                log1write("起動チェック　FILEROOT：OK")
            Else
                log1write("【エラー】FILEROOT " & f_fileroot & " が見つかりません")
            End If
        End If
        'bondriver
        Dim f_bondriver As String = path_s2z(Me.TextBoxBonDriverPath.Text.ToString)
        If f_bondriver.Length = 0 Then
            Try
                f_bondriver = IO.Path.GetDirectoryName(path_s2z(Me.textBoxUdpApp.Text.ToString))
            Catch ex As Exception
                f_bondriver = ""
            End Try
        End If
        If f_bondriver.Length > 0 Then
            If folder_exist(f_bondriver) = 1 Then
                log1write("起動チェック　BonDriverパス：OK")
                'Bondriverが存在するかまたch2が存在するかチェック
                Dim bchk As Integer = 0
                Dim bons() As String = get_and_sort_BonDrivers(f_bondriver)
                If bons IsNot Nothing Then
                    bchk = bons.Length
                    For Each stFilePath As String In bons
                        Dim s As String = trim8(stFilePath)
                        Dim bonfile As String = IO.Path.GetFileName(s).ToLower 'ファイル名
                        Dim ch2file As String = f_bondriver & "\" & IO.Path.GetFileNameWithoutExtension(s) & ".ch2"
                        If file_exist(ch2file) <= 0 Then
                            log1write("【警告】" & bonfile & " に対応するch2ファイルが見つかりませんでした")
                        Else
                            '文字コード判別
                            'テキストファイルを開く
                            Dim bs As Byte() = System.IO.File.ReadAllBytes(ch2file)
                            '文字コードを判別する
                            Dim ecode As System.Text.Encoding = Nothing
                            If IsThisShiftJIS_GetCode(bs, ecode) <> 1 Then
                                If ecode IsNot Nothing Then
                                    Dim f_ch2 As String = Path.GetFileName(ch2file)
                                    Dim udpAppName As String = Path.GetFileNameWithoutExtension(Me._worker._udpApp).ToLower
                                    If udpAppName.IndexOf("tstask") < 0 Then
                                        log1write("【警告】" & ch2file & " の文字コードがRecTaskで使用できる形式では無い可能性があります。ch2ファイルはshift_jis形式で保存してください")
                                        If udpAppName.IndexOf("rectask") >= 0 Then
                                            Dim result As DialogResult = MessageBox.Show(f_ch2 & vbCrLf & "の文字コードがRecTaskで使用できる形式では無いようです。" & vbCrLf & "Shift_JISへの変換を試みますか？" & vbCrLf & "元のファイルは" & f_ch2 & ".bakとして保存されます。" & vbCrLf & "※完璧ではありませんのでテキストエディタでの変更を推奨致します", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                                            If result = DialogResult.Yes Then
                                                Dim ch2_str As String = file2str(ch2file, "", ecode)
                                                If ch2_str.Length > 0 Then
                                                    'コピー
                                                    Try
                                                        System.IO.File.Move(ch2file, ch2file & ".bak")
                                                    Catch ex As Exception
                                                        Try
                                                            System.IO.File.Delete(ch2file)
                                                        Catch ex2 As Exception
                                                        End Try
                                                    End Try
                                                    str2file(ch2file, ch2_str, "shift_jis")
                                                    ch2_str = file2str(ch2file, "shift_jis")
                                                    Dim ch2_chk As Integer = count_str(ch2_str, "?")
                                                    If ch2_str.IndexOf("?") > 0 Then
                                                        Dim w_str As String = ""
                                                        Dim line() As String = Split(ch2_str, vbCrLf)
                                                        If line IsNot Nothing Then
                                                            For k As Integer = 0 To line.Length - 1
                                                                If line(k).IndexOf("?") > 0 Then
                                                                    w_str &= "変換前：" & line(k) & vbCrLf
                                                                    line(k) = line(k).Replace("メ?テレ", "メ～テレ") '文字化け対策
                                                                    line(k) = line(k).Replace("ＡＴ?Ｘ", "ＡＴ－Ｘ") '文字化け対策
                                                                    line(k) = line(k).Replace("?", "－") '文字化け対策
                                                                    w_str &= "変換後：" & line(k) & vbCrLf
                                                                End If
                                                            Next
                                                        End If
                                                        line2file(ch2file, line, "shift_jis")
                                                        MsgBox(f_ch2 & vbCrLf & "内の変換できなかった文字「?」を" & ch2_chk.ToString & "カ所「－」または「～」に変更しました" & vbCrLf & w_str)
                                                        log1write(ch2file & vbCrLf & "内の変換できなかった文字「?」を" & ch2_chk.ToString & "カ所「－」または「～」に変更しました" & vbCrLf & w_str)
                                                    End If
                                                    log1write(ch2file & "の文字コードをshift_jisに変更しました。")
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
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
        If isMatch_HLS(Me._worker._hlsApp, "ffmpeg") = 1 Or exepath_ffmpeg.Length > 0 Then
            Dim fchkstr As String = Me._worker._hlsApp
            If exepath_ffmpeg.Length > 0 Then
                fchkstr = exepath_ffmpeg
            End If
            If file_exist(fchkstr.Replace("ffmpeg.exe", "\fonts\fonts.conf")) = 1 Then
                fonts_conf_ok = 1
                log1write("起動チェック　ASS字幕に必要なfonts.conf：OK")
            Else
                log1write("【警告】ファイル再生時：ASS字幕を焼込み表示させるために必要なfonts.confが見つかりませんでした")
            End If
        End If
        'ptTimer用sqlite3.exeがあるかどうか確認
        If ptTimer_path.Length > 0 Then
            If file_exist("sqlite3.exe") = 1 Then
                log1write("起動チェック　ptTimer番組表に必要なsqlite3.exe：OK")
            Else
                log1write("【エラー】ptTimer番組表に必要なsqlite3.exeが見つかりませんでした")
                ptTimer_path = ""
            End If
        End If

        'RAMドライブに作成されることを考慮して存在しない場合は作成
        Dim fileroot As String = path_s2z(TextBoxFILEROOT.Text.ToString)
        fileroot = (fileroot & "\").TrimEnd("\") '末尾の\を取り除く
        If fileroot.Length > 0 Then
            Dim sp As Integer = fileroot.LastIndexOf("\")
            If fileroot.IndexOf("\\") = 0 And sp = 1 Then
                'ネットワークドライブ直
                log1write("【エラー】%FILEROOT%にネットワークドライブそのものを指定することはできません。ネットワークドライブ内フォルダを指定してください")
            ElseIf sp > 1 Then
                'フォルダが存在するか確認し、無ければ作成
                If folder_exist(fileroot) <= 0 Then
                    Try
                        System.IO.Directory.CreateDirectory(fileroot)
                        log1write("【フォルダ作成】%FILEROOT%が存在しません。" & fileroot & " を作成しました")
                    Catch ex As Exception
                        log1write("【エラー】フォルダ作成に失敗しました。" & fileroot)
                    End Try
                End If
            ElseIf fileroot.IndexOf(":") >= 0 Then
                log1write("【エラー】%FILEROOT%にドライブそのものを指定することはできません。ドライブ内フォルダを指定してください")
            Else
                log1write("【エラー】%FILEROOT%が不正です")
            End If
        End If

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
                        Case "ComboBoxRezFormOrCombo"
                            ComboBoxRezFormOrCombo.Text = lr(1)
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
                        Case "ffmpeg_seek_method_files"
                            ffmpeg_seek_method_files = lr(1).Replace("\r\n", vbCrLf)
                        Case "ComboBoxVideoForce"
                            ComboBoxVideoForce.Text = lr(1)
                        Case "CheckBoxVersionCheck"
                            CheckBoxVersionCheck.Checked = lr(1)
                        Case "CheckBoxLogReq"
                            CheckBoxLogReq.Checked = lr(1)
                        Case "CheckBoxLogWI"
                            CheckBoxLogWI.Checked = lr(1)
                        Case "CheckBoxLogETC"
                            CheckBoxLogETC.Checked = lr(1)
                        Case "CheckBoxLogDebug"
                            CheckBoxLogDebug.Checked = lr(1)
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
                        Case "BonDriverSort"
                            bondriver_sort = lr(1).Split(",")
                        Case "NicoConvAss_ConfigSet"
                            NicoConvAss_ConfigSet = Trim(lr(1))
                        Case "CheckBoxCanFileOpeWrite"
                            CheckBoxCanFileOpeWrite.Checked = lr(1)
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
        '×で最小化
        If close2min > 0 And e.CloseReason.ToString = "UserClosing" Then
            e.Cancel = True
            Me.WindowState = FormWindowState.Minimized
            Exit Sub
        End If

        Timer1.Enabled = False

        'カレントディレクトリ変更
        F_set_ppath4program()

        'ビデオフォルダ　監視終了
        stop_watch_videofolders()

        '全プロセスを停止
        Try
            '終了時にはエンコ済みファイルは消さずにおくように指定(-3)
            Me._worker.stopProc(-3)
        Catch ex As Exception
        End Try

        'EDCB TCP接続終了
        Try
            EDCB_cmd.Dispose()
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

        'ログをファイル出力
        If log_path.Length > 0 Then
            Dim alog As String = ""
            If AccessLogList IsNot Nothing Then
                For i As Integer = 0 To AccessLogList.Length - 1
                    alog &= "  " & unix2time(AccessLogList(i).utime) & " "
                    alog &= AccessLogList(i).IP & "（"
                    alog &= AccessLogList(i).domain & ")    "
                    alog &= Analyse_UserAgent(AccessLogList(i).UserAgent) & "     "
                    alog &= AccessLogList(i).URL & vbCrLf
                    alog &= "-------------------------------------------------" & vbCrLf
                Next
            End If
            str2file(log_path, alog & log1, "UTF-8")
            '整形済みログをTvRemoteViewer_VB_edited.logに出力
            Dim log_path2 As String = ""
            Dim ext As String = Path.GetExtension(log_path)
            If ext.Length > 0 Then
                Dim sp As Integer = log_path.LastIndexOf(ext)
                If sp > 0 Then
                    log_path2 = log_path.Substring(0, sp) & "_edited" & ext
                    str2file(log_path2, edit_log(log1), "UTF-8")
                End If
            End If
        End If

        'スリープ状態復帰
        If viewing_NoSleep = 1 Then
            If previousExecutionState <> 0 Then
                SetSleepMode(previousExecutionState)
            End If
        End If

        NotifyIcon1.Visible = False
        NotifyIcon1.Dispose()
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
        s &= "ComboBoxRezFormOrCombo=" & ComboBoxRezFormOrCombo.Text & vbCrLf
        s &= "CheckBoxShowConsole=" & CheckBoxShowConsole.Checked & vbCrLf
        s &= "ID=" & TextBoxID.Text & vbCrLf
        s &= "PASS=" & EncryptString(TextBoxPASS.Text.ToString, TextBoxID.Text.ToString & "TRVVB") & vbCrLf
        s &= "textBoxHlsOpt=" & textBoxHlsOpt2.Text & vbCrLf
        s &= "ComboBoxHLSorHTTP=" & ComboBoxHLSorHTTP.Text & vbCrLf
        s &= "ffmpeg_seek_method_files=" & ffmpeg_seek_method_files.Replace(vbCrLf, "\r\n") & vbCrLf
        s &= "ComboBoxVideoForce=" & ComboBoxVideoForce.Text & vbCrLf
        s &= "CheckBoxVersionCheck=" & CheckBoxVersionCheck.Checked & vbCrLf
        s &= "CheckBoxLogReq=" & CheckBoxLogReq.Checked & vbCrLf
        s &= "CheckBoxLogWI=" & CheckBoxLogWI.Checked & vbCrLf
        s &= "CheckBoxLogETC=" & CheckBoxLogETC.Checked & vbCrLf
        s &= "CheckBoxCanFileOpeWrite=" & CheckBoxCanFileOpeWrite.Checked & vbCrLf
        s &= "CheckBoxLogDebug=" & CheckBoxLogDebug.Checked & vbCrLf
        s &= "NicoConvAss_ConfigSet=" & NicoConvAss_ConfigSet & vbCrLf
        Dim bondriver_sort_str As String = ""
        If bondriver_sort IsNot Nothing Then
            bondriver_sort_str = String.Join(",", bondriver_sort)
        End If
        s &= "BonDriverSort=" & bondriver_sort_str & vbCrLf
        If me_top > -10 Then
            s &= "WindowStatus=" & me_left & "," & me_top & "," & me_width & "," & me_height & vbCrLf
        Else
            s &= "WindowStatus=" & me_window_backup & vbCrLf 'タスクトレイのまま閉じられた場合は前回のものをそのまま引き継ぐ
        End If

        'ステータスファイル書き込み
        str2file("form_status.txt", s)
    End Sub

    Private Sub check_Outside_CustomURL_multi()
        Dim t As New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf check_Outside_CustomURL))
        t.Start()
    End Sub

    Private Sub check_Outside_CustomURL()
        Dim Outside_CustomURL_html As String = get_Outside_html(1)
        'バグかと思ったがget_Outside_html内でOutside_CustomURL_htmlに代入済み
    End Sub

    Private Sub check_TvRock_Program_PC_multi()
        Dim t As New System.Threading.Thread(New System.Threading.ThreadStart(AddressOf check_TvRock_Program_PC))
        t.Start()
    End Sub

    Private Sub check_TvRock_Program_PC()
        Dim i3 As Integer = 0
        If TvRock_isVer2 = -1 Or TvRock_isVer2 = 1 Then
            i3 = get_tvrock_html_plc()
        End If
        Dim i2 As Integer = 0
        Dim i1 As Integer = 0
        System.Threading.Thread.Sleep(100)
        If get_tvrock_html_program() = 1 Then
            i1 = 1
            i2 = get_tvrock_html_search()
        End If
        If i1 + i2 < 2 Then
            'どちらか一方でも失敗していれば10分後に再チャレンジ
            TvRock_html_getutime = time2unix(Now()) - 3600 + 180 + 600
            log1write("TvRockジャンル判定用HTML取得を約10分後に再度試みます")
        End If
    End Sub

    Private Function get_tvrock_html_program() As Integer
        '番組表を取得
        Dim r As Integer = 0
        Dim url As String = TvProgram_tvrock_url
        Dim sp As Integer = url.IndexOf("/iphone")
        If sp > 0 Then
            url = url.Substring(0, sp) & "/now?b=4"
            'TvRock_html_src = get_html_by_webclient(url, "UTF-8", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36")
            Dim html As String = get_html_by_webclient(url, "Shift_JIS") '番組表1.0と思われる
            If html.Length > 500 Then
                '容量を減らす努力をする あまりにも巨大でジャンル判別に時間がかかるため
                sp = html.IndexOf(">新番組<")
                If sp > 0 Then
                    html = html.Substring(sp)
                End If
                sp = html.IndexOf(">6時間<")
                If sp > 0 Then
                    html = html.Substring(sp + ">6時間<".Length)
                End If
                html = html.Replace("<small>", "").Replace("</small>", "").Replace("&nbsp;", "").Replace("td width=", "")
                html = html.Replace("<wbr>", "").Replace("<tbody>", "").Replace("align=center", "").Replace("valign=top", "")
                html = html.Replace("<b>", "").Replace("<br>", "")
                html = Regex.Replace(html, "<a.href..http+.*?>.*?</a>", "")
                'html = Regex.Replace(html, "<a.href..now+.*?>.*?</a>", "")
                html = Regex.Replace(html, "<font.color+.*?>.*?</font>", "")
                html = Regex.Replace(html, "<img+.*?>", "")
                html = Regex.Replace(html, "<tr+.*?>", "")
                html = Regex.Replace(html, "noshade>.*?<", "<")
                html = Regex.Replace(html, "</b>.*?<", "<")
                html = Regex.Replace(html, "<table+.*?>", "")
                html = html.Replace("<td  bgcolor=#fdfeff>", "").Replace("<td bgcolor=#fdfeff>", "").Replace("<td bgcolor=#888ca0 border=0 bordercolor=#888ca0>", "")
                html = Regex.Replace(html, "</+.*?>", "")

                TvRock_html_program_src = html
                r = 1
                log1write("ジャンル判別用にTvRockのPC番組表を取得しました")
            Else
                log1write("【エラー】TvRockのPC用番組表取得に失敗しました")
            End If
        Else
            log1write("TvRockの番組取得URL（TvProgram_tvrock_url）が未知の形式です。末尾に/iphoneが記入されていません。TvProgram_tvrock_url=" & TvProgram_tvrock_url)
        End If

        Return r
    End Function

    Private Function get_tvrock_html_plc() As Integer
        '番組リストを取得
        Dim r As Integer = 0
        Dim url As String = TvProgram_tvrock_url
        Dim sp As Integer = url.IndexOf("/iphone")
        If sp > 0 Then
            url = url.Substring(0, sp) & "/plc"
            Dim html As String = get_html_by_webclient(url, "UTF-8", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36")
            If html.Length > 5000 Then
                If html.Substring(0, 5).ToLower = "<span" Then
                    html = Regex.Replace(html, "<h3>.*?</h3>", "")
                    html = html.Replace("<tr><td align=center width=", "<trs>")
                    html = Regex.Replace(html, "</td><td align=center width=.*?=\[", "<t>")
                    html = Regex.Replace(html, "　録画予約　.*?</td></tr></table></div></div></td></tr>", "</trs>")
                    html = Regex.Replace(html, "[0123456789] [123456789]*?\/[0123456789]", "</t>")
                    html = Regex.Replace(html, "target=""_blank""><img src=.*?&c=", "<k>&c=")
                    html = Regex.Replace(html, "</trs></table>.*?</tr><trs>", "</trs><trs>")

                    '番組表2.0が取得できた
                    TvRock_html_plc_src = html
                    r = 1
                    log1write("ジャンル判別用にTvRock番組表ver2から番組リストを取得しました")
                    TvRock_isVer2 = 1
                Else
                    If TvRock_isVer2 = -1 Then
                        log1write("ジャンル判別用TvRockのPC番組リストが番組表ver2形式ではありませんでした")
                        TvRock_isVer2 = 0
                    End If
                End If
            Else
                log1write("TvRockのPC用番組リスト取得に失敗しました")
            End If
        Else
            log1write("TvRockの番組取得URL（TvProgram_tvrock_url）が未知の形式です。末尾に/iphoneが記入されていません。TvProgram_tvrock_url=" & TvProgram_tvrock_url)
        End If

        Return r
    End Function

    Private Sub get_tvrock_ch_str()
        Dim url As String = TvProgram_tvrock_url

        TvRock_web_ch_str = Nothing
        Dim vol As Integer = 10 'ひとまとめ
        Dim vol_org As Integer = vol
        Dim cnt As Integer = 0

        Dim TvRock_web_ch_str_temp As String() = Nothing
        Dim sp As Integer = url.IndexOf("/iphone")
        If sp > 0 Then
            'チャンネル一覧文字列を取得する
            Try
                url = url.Substring(0, sp) & "/kws"
                Dim str1 As String = ""
                Dim chstr As String = get_html_by_webclient(url, "Shift_JIS")
                Dim sp1 As Integer = chstr.IndexOf("name=""z""")
                Dim ep1 As Integer = chstr.IndexOf("name=""g""")
                If sp1 > 0 And ep1 > 0 Then
                    sp1 = chstr.IndexOf("<OPTION", sp1 + 1)
                    Dim j As Integer = 0
                    While sp1 > 0 And sp1 < ep1
                        Dim sid_str As String = Instr_pickup(chstr, "value=""", """", sp1)
                        str1 &= "&z=" & sid_str
                        sp1 = chstr.IndexOf("<OPTION", sp1 + 1)
                        vol -= 1
                        cnt += 1
                        If vol <= 0 Then
                            ReDim Preserve TvRock_web_ch_str_temp(j)
                            TvRock_web_ch_str_temp(j) = str1
                            str1 = ""
                            j += 1
                            vol = vol_org
                        End If
                    End While
                    If str1.Length > 0 Then
                        ReDim Preserve TvRock_web_ch_str_temp(j)
                        TvRock_web_ch_str_temp(j) = str1
                    End If
                Else
                    log1write("【エラー】TvRockジャンル判別用チャンネル一覧取得に失敗しました")
                    TvRock_web_ch_str_temp = Nothing
                End If
            Catch ex As Exception
                log1write("【エラー】TvRockジャンル判別用チャンネル一覧取得中にエラーが発生しました。" & ex.Message)
                TvRock_web_ch_str_temp = Nothing
            End Try
        End If

        '地上波は情報が多いので分散させる
        If TvRock_web_ch_str_temp IsNot Nothing Then
            Dim k As Integer = 0
            ReDim Preserve TvRock_web_ch_str(TvRock_web_ch_str_temp.Length - 1)
            For i As Integer = 0 To TvRock_web_ch_str_temp.Length - 1
                Dim d() As String = Split(TvRock_web_ch_str_temp(i), "&z=")
                For j As Integer = 0 To d.Length - 1
                    If d(j).Length > 0 Then
                        TvRock_web_ch_str(k) &= "&z=" & d(j)
                        k += 1
                        If k > TvRock_web_ch_str_temp.Length - 1 Then
                            k = 0
                        End If
                    End If
                Next
            Next
        End If
    End Sub

    Private Function get_tvrock_html_search() As Integer
        '検索画面を取得
        Dim r As Integer = 0
        Dim url As String = TvProgram_tvrock_url
        Dim sp As Integer = url.IndexOf("/iphone")
        If sp > 0 Then
            If TvRock_web_ch_str Is Nothing Then
                get_tvrock_ch_str()
                System.Threading.Thread.Sleep(100)
            End If

            If TvRock_web_ch_str IsNot Nothing Then
                '検索用URL
                Dim t As DateTime = Now() '現在
                Dim m1 As Integer = Month(t)
                Dim d1 As Integer = Microsoft.VisualBasic.Day(t)
                Dim t2 As DateTime = DateAdd(DateInterval.Day, 1, t)
                Dim m2 As Integer = Month(t2)
                Dim d2 As Integer = Microsoft.VisualBasic.Day(t2)

                TvRock_genre_cache = Nothing
                For i As Integer = 0 To TvRock_web_ch_str.Length - 1
                    Dim html As String = ""
                    If TvRock_isVer2 = 0 Then
                        html = get_TvRock_search_html(m1, d1, m2, d2, 0, 24, 0, TvRock_web_ch_str(i))
                        If html.IndexOf("<g>") >= 0 Then
                            add_tvrock_genre_data(html)
                            If TvRock_genre_cache IsNot Nothing Then
                                log1write("TvRockジャンル判定用データを取得しました。総数：" & TvRock_genre_cache.Length)
                                r = 1
                            End If
                        End If
                    Else
                        html = get_TvRock_search_html_ver2(m1, d1, m2, d2, 0, 24, 0, TvRock_web_ch_str(i))
                        If html.IndexOf("<span") = 0 Then
                            add_tvrock_genre_data_ver2(html)
                            If TvRock_genre_cache IsNot Nothing Then
                                log1write("TvRockジャンル判定用データを番組表ver2から取得しました。総数：" & TvRock_genre_cache.Length)
                                r = 1
                            End If
                        End If
                    End If
                    System.Threading.Thread.Sleep(100)
                Next
            End If
        Else
            log1write("TvRockの番組取得URL（TvProgram_tvrock_url）が未知の形式です。末尾に/iphoneが記入されていません")
        End If

        Return r
    End Function

    Private Sub add_tvrock_genre_data(ByVal html As String)
        Dim sp As Integer = 0
        Dim ep As Integer = 0
        sp = html.IndexOf("<s>")
        Dim j As Integer = 0
        If TvRock_genre_cache IsNot Nothing Then
            j = TvRock_genre_cache.Length
        End If
        While sp > 0
            ep = html.IndexOf("><s>", sp)
            If ep < 0 Then
                ep = html.Length '2147483647
            End If
            Dim station As String = Trim(Instr_pickup(html, "<s>", "<", sp, ep))
            Dim genre_str As String = Trim(Instr_pickup(html, "<g>", "<", sp, ep))

            Dim title As String = Trim(Instr_pickup(html, "<>", "<><a ", sp, ep))
            If title.IndexOf(""">") >= 0 Then
                title = Trim((title & " ").Substring(title.IndexOf(""">") + 2))
            End If
            title = Regex.Replace(title, "<.*?>", "") 'TvRockのルビを消す
            Dim title_key As String = title
            If title.Length > 0 Then
                title_key = get_tvrock_title_key(title) '最大長全角文字列
            End If
            Dim sid_eid As String = Instr_pickup(html, "&c=", "&d", sp, ep) '101&e=16399
            If station.Length > 0 And genre_str.Length > 0 And title.Length > 0 And sid_eid.Length > 0 Then
                ReDim Preserve TvRock_genre_cache(j)
                TvRock_genre_cache(j).station = station
                TvRock_genre_cache(j).genre_str = genre_str
                TvRock_genre_cache(j).color_str = ""
                TvRock_genre_cache(j).title = title
                TvRock_genre_cache(j).title_key = title_key
                TvRock_genre_cache(j).sid_eid = sid_eid
                j += 1
            Else
                log1write("【エラー】ジャンル解析に失敗しました。" & genre_str & " " & sid_eid & " " & title)
            End If
            sp = html.IndexOf("<s>", sp + 1)
        End While
    End Sub

    Private Function get_TvRock_search_html(ByVal mm1 As Integer, ByVal dd1 As Integer, ByVal mm2 As Integer, ByVal dd2 As Integer, ByVal hh As Integer, ByVal du As Integer, ByVal past As Integer, ByVal web_ch_str As String) As String
        Dim url As String = TvProgram_tvrock_url
        Dim sp As Integer = url.IndexOf("/iphone")
        url = url.Substring(0, sp) & "/kws?title=*&submit=%81%40%8C%9F%8D%F5%81%40&content=&mtor=0"
        url &= "&sh=" & hh.ToString & "&dur=" & du.ToString '何時から何時間
        url &= "&w1=true&w2=true&w3=true&w4=true&w5=true&w6=true&w7=true" '曜日
        url &= "&exp=1&exs1=" & mm1.ToString & "&exs2=" & dd1.ToString & "&exe1=" & mm2.ToString & "&exe2=" & dd2.ToString '日付
        url &= web_ch_str '放送局
        url &= "&g=0&g=1&g=2&g=3&g=4&g=5&g=6&g=7&g=8&g=9&g=10&g=11&g=12&g=13&g=14&g=15" 'ジャンル
        url &= "&bc=0&bc=1&bc=2&bc=3&bc=4&bc=5&bc=6&bc=7&bc=8&bc=9&bc=10&bc=11&bc=12&bc=13&bc=14&bc=15&bc=16&bc=17&bc=18&bc=19&bc=20&bc=21&bc=22&bc=23&bc=24&bc=25&bc=26&bc=27&bc=28&bc=29" '種別
        url &= "&dno=-1&idle=60&ready=30&tale=0&extmd=0&cuscom=&trep=&ffex=&rnm=&ffrm=%40TT%40NB%40SB&nmb=0&lei=0&wonly=&ronly=true&oneseg=&asd=true&eflw=true&coop=true&rmt=&ron=&roff=&dchk=&dchk2=&vsbt=&vdsc=&tflw=true&npri=true"
        Dim html As String = get_html_by_webclient(url, "Shift_JIS") '番組表1.0と思われる
        If html.IndexOf("<small>内容</small>") > 0 Then
            '成功
            sp = html.IndexOf("<small>内容</small>")
            If sp > 0 Then
                html = html.Substring(sp + "<small>内容</small>".Length)
            End If
            '削る
            html = html.Replace("<small>", "").Replace("</small>", "")
            html = html.Replace(" align=center", "")
            html = html.Replace("<b>", "").Replace("</b>", "")
            html = Regex.Replace(html, "blank""><font color.*?>", "blank""></a><s>") '放送局に目印
            html = Regex.Replace(html, "<a.href..kws.tsea=+.*?>", "") '二重なので
            html = Regex.Replace(html, "<a.href..http+.*?>.*?</a>", "")
            html = Regex.Replace(html, "<a.href..day+.*?>.*?</a>", "")
            html = Regex.Replace(html, "size=-2>.*?</font>", "size=-2></font>")
            html = Regex.Replace(html, "<font.color+.*?>", "")
            html = html.Replace("</font>", "").Replace("<tr>", "").Replace("</tr>", "")
            html = Regex.Replace(html, "<img+.*?>", "")
            html = Regex.Replace(html, "<td width=11%+.*?>", "<g>") 'ジャンル目印に変換
            html = Regex.Replace(html, "<td+.*?>", "")
            html = Regex.Replace(html, "</+.*?>", "<>")
            html = html.Replace("<><>", "<>").Replace("<><g>", "<g>")
            If past = 1 Then
                '過去分には必要無い部分を更に削る
                html = Regex.Replace(html, "<a.href..kws+.*?>", "")
            End If

            'log1write("ジャンル判別用にTvRockのPC用検索結果を取得しました。" & mm1.ToString & "/" & dd1.ToString & "～" & mm2.ToString & "/" & dd2.ToString & " " & hh.ToString & "から" & du.ToString & "時間")
        Else
            log1write("【エラー】TvRockのジャンル判別用PC用検索結果取得に失敗しました。" & mm1.ToString & "/" & dd1.ToString & "～" & mm2.ToString & "/" & dd2.ToString & " " & hh.ToString & "から" & du.ToString & "時間")
            html = ""
        End If

        Return html
    End Function

    Private Sub add_tvrock_genre_data_ver2(ByVal html As String)
        Dim sp As Integer = 0
        Dim ep As Integer = 0
        sp = html.IndexOf("<trs>")
        Dim j As Integer = 0
        If TvRock_genre_cache IsNot Nothing Then
            j = TvRock_genre_cache.Length
        End If

        While sp > 0
            ep = html.IndexOf("</trs>", sp)
            If ep < 0 Then
                ep = html.Length '2147483647
            End If
            Dim color_str As String = Trim(Instr_pickup(html, "<trs>", " ", sp, ep))
            Dim station As String = Trim(Instr_pickup(html, "<t>", "</t>", sp, ep))
            Dim title As String = Trim(Instr_pickup(html, "] ", """", sp, ep).Replace("<>", " "))
            title = Regex.Replace(title, "<.*?>", "") 'TvRockのルビを消す
            Dim title_key As String = title
            If title.Length > 0 Then
                title_key = get_tvrock_title_key(title) '最大長全角文字列
            End If
            Dim sid_eid As String = Instr_pickup(html, "&c=", "&d", sp, ep) '101&e=16399
            If station.Length > 0 And color_str.Length > 0 And title.Length > 0 And sid_eid.Length > 0 Then
                ReDim Preserve TvRock_genre_cache(j)
                TvRock_genre_cache(j).station = station
                TvRock_genre_cache(j).genre_str = ""
                TvRock_genre_cache(j).color_str = color_str
                TvRock_genre_cache(j).title = title
                TvRock_genre_cache(j).title_key = title_key
                TvRock_genre_cache(j).sid_eid = sid_eid
                'log1write(station & " " & color_str & " " & sid_eid & " " & title & " (" & title_key & ")")
                j += 1
            Else
                log1write("【エラー】ジャンル解析に失敗しました2。" & station & " " & color_str & " " & sid_eid & " " & title)
            End If
            sp = html.IndexOf("<trs>", sp + 1)
        End While
    End Sub

    'TvRock ver2.0 検索
    Private Function get_TvRock_search_html_ver2(ByVal mm1 As Integer, ByVal dd1 As Integer, ByVal mm2 As Integer, ByVal dd2 As Integer, ByVal hh As Integer, ByVal du As Integer, ByVal past As Integer, ByVal web_ch_str As String) As String
        'get_TvRock_search_html_ver2(月, 日, 月, 日, 0, 24, 0)
        Dim url As String = TvProgram_tvrock_url
        Dim sp As Integer = url.IndexOf("/iphone")
        url = url.Substring(0, sp) & "/swc?title=*&submit=%81%40%8C%9F%8D%F5%81%40&content=&mtor=0"
        url &= "&sh=" & hh.ToString & "&dur=" & du.ToString '何時から何時間
        url &= "&w1=true&w2=true&w3=true&w4=true&w5=true&w6=true&w7=true" '曜日
        url &= "&exp=1&exs1=" & mm1.ToString & "&exs2=" & dd1.ToString & "&exe1=" & mm2.ToString & "&exe2=" & dd2.ToString '日付
        url &= web_ch_str '放送局
        url &= "&g=0&g=1&g=2&g=3&g=4&g=5&g=6&g=7&g=8&g=9&g=10&g=11&g=12&g=13&g=14&g=15" 'ジャンル
        url &= "&bc=0&bc=1&bc=2&bc=3&bc=4&bc=5&bc=6&bc=7&bc=8&bc=9&bc=10&bc=11&bc=12&bc=13&bc=14&bc=15&bc=16&bc=17&bc=18&bc=19&bc=20&bc=21&bc=22&bc=23&bc=24&bc=25&bc=26&bc=27&bc=28&bc=29" '種別
        url &= "&dno=-1&idle=60&ready=30&tale=0&extmd=0&cuscom=&trep=&ffex=&rnm=&ffrm=%40TT%40NB%40SB&nmb=0&lei=0&wonly=&ronly=true&oneseg=&asd=true&eflw=true&coop=true&rmt=&ron=&roff=&dchk=&dchk2=&vsbt=&vdsc=&tflw=true&npri=true"
        Dim html As String = get_html_by_webclient(url, "UTF-8", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36") '番組表2.0
        If html.IndexOf("<span") = 0 Then
            '成功
            html = Regex.Replace(html, "<h3>.*?</h3>", "")
            html = html.Replace("<tr><td align=center bgcolor=", "<trs>")
            html = Regex.Replace(html, "</td><td align=center bgcolor=.*?=\[", "<t>")
            html = Regex.Replace(html, "　録画予約　.*?</td></tr></table></div></div></td></tr>", "</trs>")
            html = Regex.Replace(html, " [123456789]*?\/[0123456789]*?\([月火水木金土日]\) [0123456789]", "</t>")
            html = Regex.Replace(html, "target=""_blank""><img src=.*?&c=", "&c=")

            'log1write("ジャンル判別用にTvRockのPC用検索結果ver2を取得しました。" & mm1.ToString & "/" & dd1.ToString & "～" & mm2.ToString & "/" & dd2.ToString & " " & hh.ToString & "から" & du.ToString & "時間")
        Else
            log1write("【エラー】TvRockのジャンル判別用PC用検索結果ver2取得に失敗しました。" & mm1.ToString & "/" & dd1.ToString & "～" & mm2.ToString & "/" & dd2.ToString & " " & hh.ToString & "から" & du.ToString & "時間")
            html = ""
        End If

        Return html
    End Function

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
        form1_resolution = ComboBoxResolution.Text.ToString
    End Sub

    'BonDriverを探してコンボボックスに追加
    Private Sub search_BonDriver()
        ComboBoxBonDriver.Items.Clear()
        Dim bondriver_path As String = path_s2z(TextBoxBonDriverPath.Text.ToString)
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(path_s2z(textBoxUdpApp.Text.ToString))
        End If

        Dim bons() As String = get_and_sort_BonDrivers(bondriver_path)
        If bons IsNot Nothing Then
            For i As Integer = 0 To bons.Length - 1
                'コンボボックスに追加
                Dim s As String = Path.GetFileName(bons(i))
                ComboBoxBonDriver.Items.Add(s)
            Next
        End If
    End Sub

    'BonDriverが変更されたときはコンボボックス（サービスＩＤ）を変更
    Private Sub ComboBoxBonDriver_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxBonDriver.SelectedIndexChanged
        search_ServiceID()
    End Sub

    'ComboBoxBonDriverの値にしたがってComboBoxServiceIDを変更
    Private Sub search_ServiceID()
        ComboBoxServiceID.Items.Clear()
        Dim bondriver_path As String = path_s2z(TextBoxBonDriverPath.Text.ToString)
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(path_s2z(textBoxUdpApp.Text.ToString))
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
                            If IsNumeric(s(1)) And IsNumeric(s(5)) And IsNumeric(s(7)) Then 'サービスID,TSIDが数値なら
                                If Val(s(8)) > 0 Then
                                    ComboBoxServiceID.Items.Add(s(0) & " ," & s(5) & "," & s(1))
                                End If
                            End If
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
        Dim s As String = F_get_folder(path_s2z(TextBoxWWWroot.Text.ToString))
        Try
            If s.Length > 0 Then
                TextBoxWWWroot.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'FILEROOT選択
    Private Sub ButtonFILEROOT_Click(sender As System.Object, e As System.EventArgs) Handles ButtonFILEROOT.Click
        Dim s As String = F_get_folder(path_s2z(TextBoxFILEROOT.Text.ToString))
        Try
            If s.Length > 0 Then
                TextBoxFILEROOT.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'BonDriverフォルダ選択
    Private Sub ButtonBonDriverPath_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonDriverPath.Click
        Dim s As String = F_get_folder(path_s2z(TextBoxBonDriverPath.Text.ToString))
        Try
            If s.Length > 0 Then
                TextBoxBonDriverPath.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'UDPアプリ選択
    Private Sub buttonUdpAppPath_Click(sender As System.Object, e As System.EventArgs) Handles buttonUdpAppPath.Click
        Dim s As String = F_get_filename(path_s2z(textBoxUdpApp.Text.ToString))
        Try
            If s.Length > 0 Then
                textBoxUdpApp.Text = s
            End If
        Catch ex As Exception
        End Try
    End Sub

    'HLSアプリ選択
    Private Sub buttonHlsAppPath_Click(sender As System.Object, e As System.EventArgs) Handles buttonHlsAppPath.Click
        Dim s As String = F_get_filename(path_s2z(textBoxHlsApp.Text.ToString))
        Try
            If s.Length > 0 Then
                textBoxHlsApp.Text = s

                'HLS_option.txtの内容との整合性チェック
                check_hls_option_txt()
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
            Me._worker._BonDriverPath = path_s2z(TextBoxBonDriverPath.Text.ToString)
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub textBoxUdpApp_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxUdpApp.TextChanged
        search_BonDriver()
        Try
            Me._worker._udpApp = path_s2z(textBoxUdpApp.Text.ToString)
        Catch ex As Exception
        End Try
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub textBoxHlsApp_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxHlsApp.TextChanged
        Try
            Me._worker._hlsApp = path_s2z(textBoxHlsApp.Text.ToString)
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
        form1_ID = Trim(TextBoxID.Text.ToString)
    End Sub

    '項目が変更されたことをインスタンスに知らせる
    Private Sub TextBoxPASS_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBoxPASS.TextChanged
        Try
            Me._worker._pass = TextBoxPASS.Text.ToString
        Catch ex As Exception
        End Try
        form1_PASS = Trim(TextBoxID.Text.ToString)
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
    Private Sub ContextMenuStrip1_ItemClicked(sender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ContextMenuStrip1.ItemClicked
        Select Case e.ClickedItem.Name
            Case "SeekMethodList"
                Form2.Show()
            Case "AccessLog"
                Form3.Show()
            Case "WarningReset"
                AccessLogList = Nothing
                NotifyIcon1.Icon = My.Resources.TvRemoteViewer_VB3
                NotifyIcon_status = 0
            Case "quit"
                close2min = 0 'すんなり終了させる
                'close()
                Application.Exit()
        End Select
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
        Else
            me_width = Me.Width
            me_height = Me.Height
            show_log()

            'ini設定ボタン
            If Me.Width > 546 Then
                ButtonIniSetting.Text = "<< ini 設定"
            Else
                ButtonIniSetting.Text = "ini 設定 >>"
            End If
        End If
    End Sub

    'ビデオフォルダ　監視開始
    Public Sub start_watch_folders()
        If Me._worker._videopath_ini Is Nothing Then
            Exit Sub
        End If

        'watcher = New System.IO.FileSystemWatcher
        watcher = New System.IO.FileSystemWatcher(UBound(Me._worker._videopath_ini)) {}
        Dim j As Integer
        For j = 0 To UBound(Me._worker._videopath_ini)
            watcher(j) = New System.IO.FileSystemWatcher
        Next

        For i = 0 To Me._worker._videopath_ini.Length - 1
            Try
                '監視するディレクトリを指定
                watcher(i).Path = trim8(Me._worker._videopath_ini(i))
                If watcher(i).Path.Length > 0 Then
                    '最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                    'watcher(i).NotifyFilter = System.IO.NotifyFilters.LastAccess Or _
                    'System.IO.NotifyFilters.LastWrite Or _
                    'System.IO.NotifyFilters.FileName Or _
                    'System.IO.NotifyFilters.DirectoryName
                    watcher(i).NotifyFilter = System.IO.NotifyFilters.LastWrite Or _
                    System.IO.NotifyFilters.FileName Or _
                    System.IO.NotifyFilters.DirectoryName
                    'すべてのファイルを監視
                    watcher(i).Filter = ""
                    'サブディレクトリを監視する
                    watcher(i).IncludeSubdirectories = True
                    'UIのスレッドにマーシャリングする
                    'コンソールアプリケーションでの使用では必要ない
                    'watcher.SynchronizingObject = Me
                    watcher(i).InternalBufferSize = watcher_BufferSize

                    'イベントハンドラの追加
                    AddHandler watcher(i).Changed, AddressOf watcher_Changed
                    AddHandler watcher(i).Created, AddressOf watcher_Changed
                    AddHandler watcher(i).Deleted, AddressOf watcher_Changed
                    AddHandler watcher(i).Renamed, AddressOf watcher_Changed
                    AddHandler watcher(i).Error, AddressOf watcher_Error

                    '監視を開始する
                    watcher(i).EnableRaisingEvents = True

                    log1write("ファイルフォルダ " & Me._worker._videopath(i))
                End If
            Catch ex As Exception
                log1write("ビデオフォルダ " & Me._worker._videopath_ini(i) & " の監視開始においてエラーが発生しました。" & ex.Message)
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
            Dim cmd As Integer = 0
            Select Case e.ChangeType
                Case System.IO.WatcherChangeTypes.Changed
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が変更されました。"))
                    cmd = 1
                Case System.IO.WatcherChangeTypes.Created
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が作成されました。"))
                    cmd = 2
                Case System.IO.WatcherChangeTypes.Deleted
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が削除されました。"))
                    cmd = 3
                Case System.IO.WatcherChangeTypes.Renamed
                    log1write(("ファイル 「" + e.FullPath + _
                        "」が名前変更されました。"))
                    cmd = 4
                    If tsRenameSyncChapter = 1 Then
                        'tsファイルがリネームされた場合は自動的にchapterファイルをリネームする
                        Dim ext As String = Path.GetExtension(e.FullPath)
                        If ext = ".ts" Then
                            Try
                                Dim renameEventArgs = DirectCast(e, RenamedEventArgs)
                                Dim file_path As String = Path.GetDirectoryName(e.FullPath)
                                Dim file_old As String = Path.GetFileNameWithoutExtension(renameEventArgs.OldFullPath)
                                Dim file_new As String = Path.GetFileNameWithoutExtension(renameEventArgs.FullPath)
                                If file_old <> file_new Then
                                    If file_exist(file_path & "\chapters\" & file_old & ".chapter") = 1 Then
                                        My.Computer.FileSystem.RenameFile(file_path & "\chapters\" & file_old & ".chapter", file_new & ".chapter")
                                        log1write(file_path & "\chapters\" & file_old & ".chapterから" & file_new & ".chapterへリネームしました")
                                    ElseIf file_exist(file_path & "\" & file_old & ".chapter") = 1 Then
                                        My.Computer.FileSystem.RenameFile(file_path & "\" & file_old & ".chapter", file_new & ".chapter")
                                        log1write(file_path & "\" & file_old & ".chapterから" & file_new & ".chapterへリネームしました")
                                    End If
                                End If
                            Catch ex As Exception
                                log1write("【エラー】chapterファイルリネーム中にエラーが発生しました。" & ex.Message)
                            End Try
                        End If
                    End If
            End Select

            Dim dir_changed As Integer = 0
            Try
                Dim chk As Integer = 0
                If cmd = 2 Or cmd = 4 Then
                    If folder_exist(e.FullPath) = 1 Then
                        'フォルダが作成された
                        chk = 1
                    End If
                ElseIf cmd = 3 Then
                    '削除の場合は、フォルダかファイルか判別しフォルダなら再構築
                    Dim isDir As Integer = 0
                    Dim vp() As String = Nothing
                    vp = Me._worker._videopath
                    If vp IsNot Nothing Then
                        If vp.Length > 0 Then
                            For i As Integer = 0 To vp.Length - 1
                                If Not String.IsNullOrEmpty(vp(i)) Then
                                    If vp(i).TrimEnd("\") = e.FullPath Then
                                        isDir = 1
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End If
                    If isDir = 1 Then
                        'フォルダが削除された場合
                        chk = 1
                    End If
                End If
                If chk = 1 Then
                    '作成されたのに存在していない、または削除されたのに存在している場合
                    log1write("フォルダ構造が変更されました。10秒後に更新します")
                    dir_changed = 1
                    VideoChangedFolders = vbCrLf '全フォルダリフレッシュ予約
                End If
            Catch ex As Exception
                log1write("【エラー】wathcer_Changedでエラーが発生しました。" & ex.Message)
            End Try

            If dir_changed = 0 Then
                '更新されたファイルがあるフォルダを記録
                Dim folder As String = Path.GetDirectoryName(e.FullPath)
                If VideoChangedFolders.IndexOf(vbCrLf & folder & vbCrLf) < 0 Then
                    VideoChangedFolders &= folder & vbCrLf
                End If
            End If

            watcher_lasttime = Now()
            '最後の変更からタイマーで10秒経ったら更新
        End If
    End Sub

    Private Sub watcher_Error(ByVal source As System.Object, ByVal e As System.IO.ErrorEventArgs)
        'なんらかのエラーで監視ができなくなった　（フォルダごと削除された場合、反応しないことも）
        Try
            'source.EnableRaisingEvents = False
        Catch ex As Exception
        End Try
        log1write(source.path & "の監視中にエラーが発生しました")
    End Sub

    Private Sub ButtonCopy2Clipboard_Click(sender As System.Object, e As System.EventArgs) Handles ButtonCopy2Clipboard.Click
        Try
            'クリップボードに文字列をコピーする
            Dim s As String = edit_log(TextBoxLog.Text.ToString)
            Clipboard.SetText(s)
            log1write("ログをクリップボードへコピーしました。")
        Catch ex As Exception
            log1write("ログのクリップボードへのコピーに失敗しました。" & ex.Message)
        End Try
    End Sub

    '現在のHLS_option.txtがHLSアプリにマッチしているかチェックして適切なものをコピー
    Private Sub check_hls_option_txt()
        'カレントディレクトリ変更
        F_set_ppath4program()

        Dim hls1 As String = file2str("HLS_option.txt")

        'HLS_option.txtが目次としてのみ使われていればスルー
        If hls1.IndexOf(" --sout ") >= 0 Or hls1.IndexOf("vlc:") >= 0 Or hls1.IndexOf(" -acodec ") >= 0 Or hls1.IndexOf(" -vcodec ") >= 0 Or hls1.IndexOf("hls_segment_filename:") >= 0 Or hls1.IndexOf(" --audio-codec ") >= 0 Then
            Dim hlsAppNameForm As String = ""
            Dim hlsAppNameFile As String = ""
            Dim hlsOptFile As String = ""

            Dim hlsAppFilename As String = Path.GetFileName(path_s2z(textBoxHlsApp.Text.ToString))
            Dim hlsAppNum As Integer = 0
            If isMatch_HLS(hlsAppFilename, "vlc") = 1 Then
                hlsAppNum = 1
                hlsAppNameForm = "vlc"
                hlsOptFile = "HLS_option_VLC.txt"
            ElseIf isMatch_HLS(hlsAppFilename, "ffmpeg") = 1 Then
                hlsAppNum = 2
                hlsAppNameForm = "ffmpeg"
                hlsOptFile = "HLS_option_ffmpeg.txt"
            ElseIf isMatch_HLS(hlsAppFilename, "qsvencc") = 1 Then
                hlsAppNum = 3
                hlsAppNameForm = "QSVEnc"
                hlsOptFile = "HLS_option_QSVEnc.txt"
            End If

            If hlsAppNum > 0 Then
                Dim hls1str As String = ""
                If hls1.IndexOf(" --sout ") >= 0 Or hls1.IndexOf("vlc:") >= 0 Then
                    hls1str &= ":1:"
                    hlsAppNameFile = "vlc"
                End If
                If hls1.IndexOf(" -acodec ") >= 0 Or hls1.IndexOf(" -vcodec ") >= 0 Then
                    hls1str &= ":2:"
                    hlsAppNameFile = "ffmpeg"
                End If
                If hls1.IndexOf("hls_segment_filename:") >= 0 Or hls1.IndexOf(" --audio-codec ") >= 0 Then
                    hls1str &= ":3:"
                    hlsAppNameFile = "QSVEnc"
                End If

                If hls1str.Length > 0 And hls1str.IndexOf(":" & hlsAppNum.ToString & ":") < 0 Then
                    'メッセージボックスを表示する 
                    Dim result As DialogResult = MessageBox.Show("HLS_option.txtの内容が" & hlsAppNameForm & "と一致しません。" & vbCrLf & hlsOptFile & "の内容をHLS_option.txtへコピーしますか？" & vbCrLf & "※フォーム上のHLSオプション欄は保持されますので必要ならば手動で変更してください", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                    If result = DialogResult.Yes Then
                        Try
                            System.IO.File.Copy("HLS_option.txt", "HLS_option.txt.bak", True)
                            System.IO.File.Copy(hlsOptFile, "HLS_option.txt", True)
                            log1write(hlsOptFile & "の内容をHLS_option.txtにコピーしました")
                            'HLSオプション変数更新
                            Me._worker.read_hls_option()
                            Dim hlsopt2temp As String = textBoxHlsOpt2.Text.ToString
                            'コンボボックス更新
                            search_ComboBoxResolution()
                            textBoxHlsOpt2.Text = hlsopt2temp
                        Catch ex As Exception
                            log1write("【エラー】ファイルコピーに失敗しました。" & ex.Message)
                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub textBoxHlsApp_Leave(sender As System.Object, e As System.EventArgs) Handles textBoxHlsApp.Leave
        'HLS_option.txtの内容との整合性チェック
        check_hls_option_txt()
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        'HLSオプション変数更新
        Me._worker.read_hls_option()
        Dim hlsopt2temp As String = textBoxHlsOpt2.Text.ToString
        'コンボボックス更新
        search_ComboBoxResolution()
        textBoxHlsOpt2.Text = hlsopt2temp

        log1write("HLS_option*.txtを再読み込みしました")
        log1write("フォーム上のHLSオプションは保持されていますので必要ならば手動で更新してください")
    End Sub

    Private Sub ComboBoxRezFormOrCombo_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxRezFormOrCombo.SelectedIndexChanged
        form1_hls_or_rez = ComboBoxRezFormOrCombo.Text.ToString
    End Sub

    Private Sub Button8_Click(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        Try
            'カレントディレクトリ変更
            F_set_ppath4program()
            Try
                System.Diagnostics.Process.Start("profile.txt")
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ComboBoxVideoForce_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxVideoForce.SelectedIndexChanged
        video_force_ffmpeg = Val(ComboBoxVideoForce.Text.ToString)
        If TvRemoteViewer_VB_Start = 1 Then
            log1write("video_force_ffmpeg=" & video_force_ffmpeg)
        End If
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        'プロファイル更新
        'カレントディレクトリ変更
        F_set_ppath4program()
        If file_exist("profile.txt") = 1 Then
            profiletxt = file2str("profile.txt", "UTF-8")
            log1write("プロファイルを読み込みました")
        Else
            log1write("【エラー】profile.txtが見つかりません")
        End If
    End Sub

    Private Sub CheckBoxVersionCheck_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxVersionCheck.CheckedChanged
        If CheckBoxVersionCheck.Checked = True Then
            TvRemoteViewer_VB_version_check_on = 1
            '前回より１分以上経過していればバージョンチェックする
            If DateDiff("n", TvRemoteViewer_VB_version_check_datetime, Now) >= 1 And TvRemoteViewer_VB_Start = 1 Then
                check_version()
                log1write("アップデートチェックを行いました")
            End If
        Else
            TvRemoteViewer_VB_version_check_on = 0
        End If
    End Sub

    Private Sub Form1_Move(sender As System.Object, e As System.EventArgs) Handles MyBase.Move
        If Me.WindowState = FormWindowState.Minimized = False Then
            me_left = Me.Left
            me_top = Me.Top
        End If
    End Sub

    Private Sub CheckBoxLogReq_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxLogReq.CheckedChanged
        show_log()
    End Sub

    Private Sub CheckBoxLogWI_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxLogWI.CheckedChanged
        show_log()
    End Sub

    Private Sub CheckBoxLogETC_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxLogETC.CheckedChanged
        show_log()
    End Sub

    Private Sub CheckBoxWriteLog_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxWriteLog.CheckedChanged
        Try
            Me._worker._writeLog = CheckBoxWriteLog.Checked
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CheckBoxLogDebug_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxLogDebug.CheckedChanged
        If CheckBoxLogDebug.Checked = True Then
            log_debug = 1
        Else
            log_debug = 0
        End If
    End Sub

    'タブにini要素を追加
    Private iniLabel1() As System.Windows.Forms.Label
    Private iniTextbox() As System.Windows.Forms.TextBox
    Private iniButton1() As System.Windows.Forms.Button
    Private Sub ini_init_tab()
        Dim i As Integer = 0
        If ini_array IsNot Nothing And ini_genre IsNot Nothing Then
            Dim y(ini_genre.Length) As Integer
            For i = 1 To ini_genre.Length
                y(i) = 10
            Next

            Me.iniLabel1 = New System.Windows.Forms.Label(ini_array.Length - 1) {}
            Me.iniTextbox = New System.Windows.Forms.TextBox(ini_array.Length - 1) {}
            Me.iniButton1 = New System.Windows.Forms.Button(ini_array.Length - 1) {}

            Me.SuspendLayout()
            For i = 0 To ini_array.Length - 1
                Dim iniTextbox_leftmove As Integer = 0
                Dim label_width_plus As Integer = 0
                'button1
                Me.iniButton1(i) = New System.Windows.Forms.Button
                Me.iniButton1(i).Visible = False
                Me.iniButton1(i).Name = "Button1_-" & ini_array(i).name & "_-" & ini_array(i).value_type
                Me.iniButton1(i).Text = ".."
                Me.iniButton1(i).Size = New System.Drawing.Size(20, 20)
                'textbox
                Me.iniTextbox(i) = New System.Windows.Forms.TextBox
                Me.iniTextbox(i).Name = "Textbox_-" & ini_array(i).name
                Me.iniTextbox(i).Text = ini_array(i).value
                Me.iniTextbox(i).Visible = True
                If ini_array(i).value_type IsNot Nothing Then
                    If ini_array(i).value_type = "integer" Then
                        Me.iniTextbox(i).Size = New System.Drawing.Size(60, 20)
                        Me.iniTextbox(i).TextAlign = HorizontalAlignment.Right
                        iniTextbox_leftmove -= 60
                        label_width_plus = 60
                    ElseIf ini_array(i).value_type.IndexOf("string") >= 0 Then
                        Dim w As Integer = Val(ini_array(i).value_type.Replace("string", ""))
                        If w > 0 Then
                            iniTextbox_leftmove = w - 120
                            If iniTextbox_leftmove <= 0 Then iniTextbox_leftmove = 0
                        End If
                        Me.iniTextbox(i).Size = New System.Drawing.Size(120 + iniTextbox_leftmove, 20)
                    ElseIf ini_array(i).value_type.IndexOf("file") = 0 Then
                        Dim vtype As String = ini_array(i).value_type
                        Dim mleft As Integer = 0
                        Dim ext As String = ""
                        Dim d() As String = vtype.Split("_")
                        If d.Length = 2 Then
                            vtype = Trim(d(0))
                            ext = Trim(d(1))
                        End If
                        Dim w As Integer = Val(vtype.Replace("file", ""))
                        If w > 0 Then
                            iniTextbox_leftmove = w - 120
                            If iniTextbox_leftmove <= 0 Then iniTextbox_leftmove = 0
                        End If
                        Me.iniTextbox(i).Size = New System.Drawing.Size(120 + iniTextbox_leftmove - 22, 20)
                        Me.iniButton1(i).Visible = True
                        AddHandler Me.iniButton1(i).Click, AddressOf Me.ini_Dialog_file_folder
                    ElseIf ini_array(i).value_type.IndexOf("folder") = 0 Then
                        If ini_array(i).value_type.IndexOf("folders") = 0 Then
                            Me.iniButton1(i).Text = "+"
                        End If
                        Dim w As Integer = Val(ini_array(i).value_type.Replace("folders", "").Replace("folder", ""))
                        If w > 0 Then
                            iniTextbox_leftmove = w - 120
                            If iniTextbox_leftmove <= 0 Then iniTextbox_leftmove = 0
                        End If
                        Me.iniTextbox(i).Size = New System.Drawing.Size(120 + iniTextbox_leftmove - 22, 20)
                        Me.iniButton1(i).Visible = True
                        AddHandler Me.iniButton1(i).Click, AddressOf Me.ini_Dialog_file_folder
                    Else
                        Me.iniTextbox(i).Size = New System.Drawing.Size(120, 20)
                    End If
                Else
                    Me.iniTextbox(i).Size = New System.Drawing.Size(120, 20)
                End If
                'label1
                Me.iniLabel1(i) = New System.Windows.Forms.Label
                Me.iniLabel1(i).Name = "Label_-" & ini_array(i).name
                Me.iniLabel1(i).Text = ini_array(i).name
                If ini_array(i).title IsNot Nothing Then
                    If ini_array(i).title.Length > 0 Then
                        Me.iniLabel1(i).Text = ini_array(i).title
                    End If
                End If
                If ini_array(i).need_reset = 1 Then
                    Me.iniLabel1(i).Text = "(*)" & Me.iniLabel1(i).Text
                End If
                Me.iniLabel1(i).AutoSize = False
                Me.iniLabel1(i).Size = New System.Drawing.Size(380 + label_width_plus, 16)
                Me.iniLabel1(i).AutoEllipsis = True
                'イベント
                AddHandler Me.iniTextbox(i).TextChanged, AddressOf Me.iniTextbox_changed
                AddHandler Me.iniTextbox(i).Enter, AddressOf Me.iniTextbox_enter
                AddHandler Me.iniLabel1(i).Click, AddressOf Me.iniTextbox_enter
                'AddHandler Me.iniLabel1(i).MouseHover, AddressOf Me.iniTextbox_enter
                Dim x1 As Integer = 5
                Dim x2 As Integer = 390 - iniTextbox_leftmove
                Dim x3 As Integer = 415
                Dim y1 As Integer = 3
                Dim h1 As Integer = 20
                '説明
                If ini_array(i).value_type = "document" Then
                    Me.iniTextbox(i).Visible = False
                    If Trim(ini_array(i).title.Replace("　", "")).Length = 0 Then
                        Me.iniLabel1(i).Visible = False
                        h1 = 10
                    Else
                        Me.iniLabel1(i).Size = New System.Drawing.Size(535, 16)
                    End If
                End If
                Select Case ini_array(i).genre
                    Case "WEBサーバー"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(2))
                            TabPage2.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(2) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(2))
                        y(2) += h1
                        TabPage2.Controls.Add(Me.iniTextbox(i))
                        TabPage2.Controls.Add(Me.iniLabel1(i))
                    Case "番組表全般"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(3))
                            TabPage3.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(3) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(3))
                        y(3) += h1
                        TabPage3.Controls.Add(Me.iniTextbox(i))
                        TabPage3.Controls.Add(Me.iniLabel1(i))
                    Case "番組表データ"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(4))
                            TabPage4.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(4) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(4))
                        y(4) += h1
                        TabPage4.Controls.Add(Me.iniTextbox(i))
                        TabPage4.Controls.Add(Me.iniLabel1(i))
                    Case "HLS配信"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(5))
                            TabPage5.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(5) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(5))
                        y(5) += h1
                        TabPage5.Controls.Add(Me.iniTextbox(i))
                        TabPage5.Controls.Add(Me.iniLabel1(i))
                    Case "HTTP配信"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(6))
                            TabPage6.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(6) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(6))
                        y(6) += h1
                        TabPage6.Controls.Add(Me.iniTextbox(i))
                        TabPage6.Controls.Add(Me.iniLabel1(i))
                    Case "ファイル再生"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(7))
                            TabPage7.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(7) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(7))
                        y(7) += h1
                        TabPage7.Controls.Add(Me.iniTextbox(i))
                        TabPage7.Controls.Add(Me.iniLabel1(i))
                    Case Else
                        'Case "全般"
                        If Me.iniButton1(i).Visible = True Then
                            Me.iniButton1(i).Location = New Point(490, y(1))
                            TabPage1.Controls.Add(Me.iniButton1(i))
                        End If
                        Me.iniLabel1(i).Location = New Point(x1, y(1) + y1)
                        Me.iniTextbox(i).Location = New Point(x2, y(1))
                        y(1) += h1
                        TabPage1.Controls.Add(Me.iniTextbox(i))
                        TabPage1.Controls.Add(Me.iniLabel1(i))
                End Select
            Next
            Me.ResumeLayout(False)
        End If
    End Sub

    'ボタンが押されたら
    Private Sub ini_Dialog_file_folder(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim name As String = (CType(sender, System.Windows.Forms.Button).Name).Replace("Button1_-", "")
            Dim d() As String = Split(name, "_-")
            If d.Length = 2 Then
                '現在の値を取得
                Dim textbox_name As String = "Textbox_-" & Trim(d(0))
                Dim cs As Control() = Me.Controls.Find(textbox_name, True)
                Dim value As String = ""
                If cs.Length > 0 Then
                    value = CType(cs(0), TextBox).Text.ToString
                End If
                'ダイアログ表示
                If Trim(d(1)).IndexOf("file") = 0 Then
                    DisplayOpenFileDialog("Textbox_-" & Trim(d(0)), value)
                ElseIf Trim(d(1)).IndexOf("folders") = 0 Then
                    DisplayFolderBrowserDialog("Textbox_-" & Trim(d(0)), value, 1)
                ElseIf Trim(d(1)).IndexOf("folder") = 0 Then
                    DisplayFolderBrowserDialog("Textbox_-" & Trim(d(0)), value, 0)
                End If
            End If
        Catch ex As Exception
            log1write("【エラー】ini設定内ボタンクリック処理中にエラーが発生しました" & ex.Message)
        End Try
    End Sub

    Private Sub DisplayOpenFileDialog(ByVal name As String, ByVal value As String)
        Try
            Dim openFile As New System.Windows.Forms.OpenFileDialog()
            openFile.DefaultExt = "exe"
            openFile.Filter = "(*.exe)|*.exe"
            If value.Length > 0 Then
                openFile.InitialDirectory = Path.GetDirectoryName(value)
            Else
                openFile.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            End If
            openFile.ShowDialog()
            If openFile.FileNames.Length > 0 Then
                Dim filename As String
                For Each filename In openFile.FileNames
                    Dim cs As Control() = Me.Controls.Find(name, True)
                    If cs.Length > 0 Then
                        CType(cs(0), TextBox).Text = filename
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            log1write("【エラー】ファイル選択ダイアログ表示中にエラーが発生しました。" & ex.Message)
        End Try
    End Sub

    Private Sub DisplayFolderBrowserDialog(ByVal name As String, ByVal value As String, ByVal a As Integer)
        Try
            Dim openFolder As New System.Windows.Forms.FolderBrowserDialog()
            openFolder.RootFolder = Environment.SpecialFolder.Desktop
            openFolder.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            If value.Length > 0 Then
                Dim d() As String = value.Split(",")
                If d.Length > 0 Then
                    Try
                        openFolder.SelectedPath = d(d.Length - 1)
                    Catch ex2 As Exception
                    End Try
                End If
            End If
            'ダイアログを表示する
            If openFolder.ShowDialog(Me) = DialogResult.OK Then
                Dim cs As Control() = Me.Controls.Find(name, True)
                If cs.Length > 0 Then
                    If a = 0 Or Trim(value).Length = 0 Then
                        CType(cs(0), TextBox).Text = openFolder.SelectedPath
                    ElseIf ("," & Trim(value) & ",").IndexOf(openFolder.SelectedPath) < 0 Then
                        CType(cs(0), TextBox).Text &= "," & openFolder.SelectedPath
                    End If
                End If
            End If
        Catch ex As Exception
            log1write("【エラー】フォルダ選択ダイアログ表示中にエラーが発生しました。" & ex.Message)
        End Try
    End Sub

    Private Sub iniTextbox_changed(ByVal sender As Object, ByVal e As EventArgs)
        Dim name As String = (CType(sender, System.Windows.Forms.TextBox).Name).Replace("Textbox_-", "")
        Dim j As Integer = Array.IndexOf(ini_array, name)
        If j >= 0 Then
            ini_array(j).value_temp = (CType(sender, System.Windows.Forms.TextBox).Text).ToString
            If ini_array(j).value <> ini_array(j).value_temp Then
                CType(sender, System.Windows.Forms.TextBox).BackColor = Color.Yellow
            Else
                CType(sender, System.Windows.Forms.TextBox).BackColor = Color.White
            End If
        End If
    End Sub

    Private Sub iniTextbox_enter(ByVal sender As Object, ByVal e As EventArgs)
        Dim name As String = ""
        If sender.GetType.FullName.IndexOf(".Label") > 0 Then
            name = (CType(sender, System.Windows.Forms.Label).Name).Replace("Label_-", "")
        Else
            name = (CType(sender, System.Windows.Forms.TextBox).Name).Replace("Textbox_-", "")
        End If
        Dim j As Integer = Array.IndexOf(ini_array, name)
        If j >= 0 Then
            TextBoxIniDoc.Text = "パラメーター名： " & ini_array(j).name
            If ini_array(j).need_reset = 1 Then
                TextBoxIniDoc.Text = "【要再起動】 " & vbCrLf & TextBoxIniDoc.Text
            End If
            If ini_array(j).value IsNot Nothing Then
                TextBoxIniDoc.Text &= vbCrLf & "現在値： " & ini_array(j).value
            End If
            If ini_array(j).title IsNot Nothing Then
                TextBoxIniDoc.Text &= vbCrLf & ini_array(j).title
            End If
            If ini_array(j).document IsNot Nothing Then
                TextBoxIniDoc.Text &= vbCrLf & ini_array(j).document
            End If
        End If
    End Sub

    Private Sub ButtonIniApply_Click(sender As System.Object, e As System.EventArgs) Handles ButtonIniApply.Click
        Dim restart_str As String = "iniに変更を保存し適用しますか？"
        If RestartExe_path.Length > 0 Then
            restart_str = "iniの変更を適用し再起動しますか？"
        End If
        Dim result As DialogResult = MessageBox.Show(restart_str, "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If result = DialogResult.Yes Then
            rewrite_ini_file()

            If RestartExe_path.Length > 0 Then
                'https://dobon.net/vb/dotnet/programing/applicationrestart.html
                'プロセスのIDを取得する
                Dim processId As Integer = System.Diagnostics.Process.GetCurrentProcess().Id
                'アプリケーションが終了するまで待機する時間
                Dim waitTime As Integer = 30000
                'コマンドライン引数を作成する
                Dim cmd As String = """" & processId.ToString() & """ " & _
                                     """" & waitTime.ToString() & """ " & _
                                     Environment.CommandLine
                '再起動用アプリケーションのパスを取得する
                Dim restartPath As String = RestartExe_path 'System.IO.Path.Combine(Application.StartupPath, "restart.exe")
                '再起動用アプリケーションを起動する
                System.Diagnostics.Process.Start(restartPath, cmd)
                'アプリケーションを終了する
                Application.Exit()
            Else
                Me._worker.read_videopath()
                Me._worker.check_ini_parameter()
                If close2min = 2 Then
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.SizableToolWindow
                Else
                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
                End If
                If iniTextbox IsNot Nothing Then
                    For i As Integer = 0 To iniTextbox.Length - 1
                        iniTextbox(i).BackColor = Color.White
                    Next
                End If
                'フォームのBonDriverコンボボックス更新
                search_BonDriver()
                log1write("iniを変更し適用作業を行いました")
            End If
        End If
    End Sub

    Private Sub ButtonIniCancel_Click(sender As System.Object, e As System.EventArgs) Handles ButtonIniCancel.Click
        'キャンセル
        Dim result As DialogResult = MessageBox.Show("入力した内容を破棄しますか？", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If result = DialogResult.Yes Then
            If iniTextbox IsNot Nothing Then
                For i As Integer = 0 To iniTextbox.Length - 1
                    Dim name As String = iniTextbox(i).Name.Replace("Textbox_-", "")
                    Dim j As Integer = Array.IndexOf(ini_array, name)
                    If j >= 0 Then
                        If ini_array(i).value <> ini_array(i).value_temp Then
                            ini_array(i).value_temp = ini_array(i).value
                            iniTextbox(i).Text = ini_array(j).value
                            iniTextbox(i).BackColor = Color.White
                        End If
                    End If
                Next
                log1write("入力内容を破棄しました")
            End If
        End If
    End Sub

    Private Sub ButtonIniSetting_Click(sender As System.Object, e As System.EventArgs) Handles ButtonIniSetting.Click
        If Me.Width >= 1078 Then
            Me.Width = 546
        ElseIf Me.Width > 546 Then
            Me.Width = 546
        Else
            Me.Width = 1078
        End If
    End Sub

    Private Sub ButtonIniBackup_Click(sender As System.Object, e As System.EventArgs) Handles ButtonIniBackup.Click
        Dim result As DialogResult = MessageBox.Show("iniファイルをバックアップしますか？", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If result = DialogResult.Yes Then
            Dim bak_str As String = file2str("TvRemoteViewer_VB.ini")
            If bak_str.Length > 0 Then
                str2file("TvRemoteViewer_VB.ini.bak2", bak_str)
                log1write("TvRemoteViewer_VB.ini を TvRemoteViewer_VB.ini.bak2 にバックアップしました")
            End If
        End If
    End Sub

    'セットされた値にiniを書き換える
    Public Sub rewrite_ini_file()
        'その後read_videopathを実行すれば環境変更完了

        'カレントディレクトリ変更
        F_set_ppath4program()

        Dim log_str As String = ""
        Dim ini_filename As String = "TvRemoteViewer_VB.ini"

        If file_exist(ini_filename) = 1 Then
            Dim line() As String = file2line(ini_filename)
            If line IsNot Nothing Then
                Dim i, j As Integer
                Dim need_reset As Integer = 0

                If line Is Nothing Then
                ElseIf line.Length > 0 Then
                    '読み込み完了

                    If ini_array IsNot Nothing Then
                        If ini_array.Length > 0 Then
                            For i = 0 To line.Length - 1
                                Dim line_temp As String = line(i)
                                line_temp = Trim(line(i))
                                'コメント削除
                                Dim inline_comment As String = ""
                                If line_temp.IndexOf(";") >= 0 Then
                                    inline_comment = line_temp.Substring(line(i).IndexOf(";"))
                                    line_temp = line_temp.Substring(0, line(i).IndexOf(";"))
                                End If
                                Dim youso() As String = line_temp.Split("=")
                                If youso Is Nothing Then
                                ElseIf youso.Length >= 2 Then
                                    If youso.Length > 2 Then
                                        For j = 2 To youso.Length - 1
                                            youso(1) &= "=" & youso(j)
                                        Next
                                    End If

                                    For j = 0 To youso.Length - 1
                                        youso(j) = Trim(youso(j))
                                    Next

                                    'BugFix 先のTvRemoteViewer_VB.ini.dataの説明文ミスを修正
                                    If i > 0 Then
                                        If youso(0) = "MIME_TYPE_DEFAULT" And trim8(line(i - 1)) = ";[WEBサーバー] MIME TYPE(例：m3u8:application/x-mpegURL, ts:video/MP2T)" Then
                                            line(i - 1) = ";[WEBサーバー] 指定が無い場合の標準MIME TYPE(例：""text/html"")"
                                        End If
                                    End If

                                    If youso(0).Length > 0 Then
                                        j = Array.IndexOf(ini_array, youso(0))
                                        If j >= 0 Then
                                            ini_array(j).write_chk = 1
                                            If ini_array(j).value <> ini_array(j).value_temp Then
                                                ini_array(j).value = ini_array(j).value_temp
                                                line(i) = ini_array(j).name & " = " & ini_array(j).value_temp
                                                If inline_comment.Length > 0 Then
                                                    line(i) &= " " & inline_comment
                                                End If
                                                '要再起動かどうか
                                                If ini_array(j).need_reset > 0 Then
                                                    need_reset = 1
                                                End If
                                                log1write(ini_filename & "の項目を修正しました。[" & ini_array(j).genre & "] " & ini_array(j).name & "=" & ini_array(j).value_temp)
                                                log_str &= ini_filename & "の項目を修正しました。[" & ini_array(j).genre & "] " & ini_array(j).name & "=" & ini_array(j).value_temp & vbCrLf
                                            End If
                                        Else
                                            log1write(ini_filename & "の項目 " & youso(0) & " が見つかりません")
                                            log_str &= ini_filename & "の項目 " & youso(0) & " が見つかりません" & vbCrLf
                                        End If
                                    End If
                                End If
                            Next

                            '書き残しを追加
                            For i = 0 To ini_array.Length - 1
                                If ini_array(i).write_chk = 0 And ini_array(i).name.Length > 0 Then
                                    ''直前項目の後ろに追加
                                    'Dim last_i As Integer = 0
                                    'If i > 0 Then
                                    'last_i = i - 1
                                    'End If
                                    'Dim last_name As String = ini_array(last_i).name
                                    'For j = 0 To line.Length - 1
                                    'If line(j).Replace(" ", "").IndexOf(last_name & "=") = 0 Or j = line.Length - 1 Then
                                    'Dim genre_str As String = ""
                                    'If line(j).Replace(" ", "").IndexOf(last_name & "=") < 0 And j = line.Length - 1 Then
                                    'genre_str = "[" & ini_array(i).genre & "] "
                                    'End If
                                    'line(j) &= vbCrLf & vbCrLf
                                    'line(j) &= ";" & genre_str & ini_array(i).title & " " & ini_array(i).document & vbCrLf
                                    'line(j) &= ini_array(i).name & " = " & ini_array(i).value
                                    'log1write(ini_filename & "に項目" & ini_array(i).name & "=" & ini_array(i).value & "を追加しました")
                                    'Exit For
                                    'End If
                                    'Next

                                    '最後に追加（このほうがわかりやすい）
                                    j = line.Length - 1
                                    ini_array(i).value = ini_array(i).value_temp
                                    Dim genre_str As String = ""
                                    genre_str = "[" & ini_array(i).genre & "] "
                                    line(j) &= vbCrLf & vbCrLf
                                    line(j) &= ";" & genre_str & ini_array(i).title.Replace(vbCrLf, vbCrLf & ";") & " " & ini_array(i).document.Replace(vbCrLf, vbCrLf & ";") & vbCrLf
                                    line(j) &= ini_array(i).name & " = " & ini_array(i).value
                                    log1write(ini_filename & "に [" & ini_array(i).genre & "] " & ini_array(i).name & "=" & ini_array(i).value & " を追加しました")
                                    log_str &= ini_filename & "に [" & ini_array(i).genre & "] " & ini_array(i).name & "=" & ini_array(i).value & " を追加しました" & vbCrLf

                                    '要再起動かどうか
                                    If ini_array(i).need_reset = 1 Then
                                        need_reset = 1
                                    End If
                                End If
                            Next

                            '書き出し
                            line2file(ini_filename, line)
                            log1write("iniファイル " & ini_filename & " を更新しました")
                            log_str &= "iniファイル " & ini_filename & " を更新しました" & vbCrLf

                            If need_reset = 1 Then
                                log_str = "【再起動が必要です】" & vbCrLf & log_str
                                If RestartExe_path.Length = 0 Then
                                    MsgBox("再起動が必要です")
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Else
            log1write("【エラー】iniファイル " & ini_filename & " が見つかりませんでした")
            log_str &= "【エラー】iniファイル " & ini_filename & " が見つかりませんでした" & vbCrLf
        End If
        'ログ出力
        TextBoxIniDoc.Text = log_str
    End Sub

    Private Sub CheckBoxBonSort_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxBonSort.CheckedChanged
        If CheckBoxBonSort.Checked = True Then
            ListBoxBonSort_refresh()
            If Me.Width = 546 Then
                PanelBonSort.Location = New Point(240, 160)
            Else
                PanelBonSort.Location = New Point(374, 242)
            End If
            PanelBonSort.Visible = True
        Else
            PanelBonSort.Visible = False
        End If
    End Sub

    Private Sub ListBoxBonSort_refresh()
        ListBoxBonSort.Items.Clear()
        Dim bondriver_path As String = path_s2z(TextBoxBonDriverPath.Text.ToString)
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(path_s2z(textBoxUdpApp.Text.ToString))
        End If
        Dim bons() As String = get_and_sort_BonDrivers(bondriver_path)
        If bons IsNot Nothing Then
            For i As Integer = 0 To bons.Length - 1
                ListBoxBonSort.Items.Add(bons(i))
            Next
        End If
    End Sub

    Private Sub ButtonBonSortUp_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonSortUp.Click
        Dim k As Integer = ListBoxBonSort.SelectedIndex
        Dim j As Integer = 0
        If k >= 1 Then
            'キャッシュクリア
            html_selectbonsidch_a = ""
            html_selectbonsidch_b = ""

            Dim combotext As String = ComboBoxBonDriver.Text
            Dim temp As String = ListBoxBonSort.Items(k - 1)
            ListBoxBonSort.Items(k - 1) = ListBoxBonSort.Items(k)
            ListBoxBonSort.Items(k) = temp
            ListBoxBonSort.SelectedIndex = k - 1
            bondriver_sort = Nothing
            For i As Integer = 0 To ListBoxBonSort.Items.Count - 1
                ReDim Preserve bondriver_sort(j)
                bondriver_sort(j) = ListBoxBonSort.Items(i)
                j += 1
            Next
            'combobox更新
            search_BonDriver()
            ComboBoxBonDriver.Text = combotext
        End If
    End Sub

    Private Sub ButtonBonSrortDown_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonSrortDown.Click
        Dim k As Integer = ListBoxBonSort.SelectedIndex
        Dim j As Integer = 0
        If k >= 0 And k < ListBoxBonSort.Items.Count - 1 Then
            'キャッシュクリア
            html_selectbonsidch_a = ""
            html_selectbonsidch_b = ""

            Dim combotext As String = ComboBoxBonDriver.Text
            Dim temp As String = ListBoxBonSort.Items(k + 1)
            ListBoxBonSort.Items(k + 1) = ListBoxBonSort.Items(k)
            ListBoxBonSort.Items(k) = temp
            ListBoxBonSort.SelectedIndex = k + 1
            bondriver_sort = Nothing
            For i As Integer = 0 To ListBoxBonSort.Items.Count - 1
                ReDim Preserve bondriver_sort(j)
                bondriver_sort(j) = ListBoxBonSort.Items(i)
                j += 1
            Next
            'combobox更新
            search_BonDriver()
            ComboBoxBonDriver.Text = combotext
        End If
    End Sub


    Private Sub ButtonBonSortClose_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonSortClose.Click
        CheckBoxBonSort.Checked = False
    End Sub

    Private Sub ButtonBonSortInit_Click(sender As System.Object, e As System.EventArgs) Handles ButtonBonSortInit.Click
        Dim result As DialogResult = MessageBox.Show("優先順位を初期化しますか？", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If result = DialogResult.Yes Then
            'キャッシュクリア
            html_selectbonsidch_a = ""
            html_selectbonsidch_b = ""

            Dim combotext As String = ComboBoxBonDriver.Text
            bondriver_sort = Nothing
            ListBoxBonSort_refresh()
            search_BonDriver()
            ComboBoxBonDriver.Text = combotext
        End If
    End Sub

    Private Sub ComboBoxNicoSet_TextChanged(sender As System.Object, e As System.EventArgs) Handles ComboBoxNicoSet.TextChanged
        NicoConvAss_ConfigSet = ComboBoxNicoSet.Text.ToString
    End Sub

    Public Function F_set_ComboboxNicoSet(ByVal s As String) As String
        Dim r As String = ""

        Try
            If NicoConvAss_path.Length > 0 Then
                Dim config_folder As String = Path.GetDirectoryName(NicoConvAss_path) & "\configset"
                If folder_exist(config_folder) = 1 Then
                    Dim files As String() = System.IO.Directory.GetFiles(config_folder, "*.txt", System.IO.SearchOption.AllDirectories)
                    If files IsNot Nothing Then
                        ComboBoxNicoSet.Items.Clear()
                        ComboBoxNicoSet.Items.Add("")
                        For i As Integer = 0 To files.Length - 1
                            Dim config_name As String = Path.GetFileNameWithoutExtension(files(i))
                            If config_name.Length > 0 Then
                                ComboBoxNicoSet.Items.Add(config_name)
                                If config_name = s Then
                                    r = s
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            log1write("【エラー】F_set_ComboboxNicoSet内でエラーが発生しました。" & ex.Message)
        End Try

        Return r
    End Function

    Private Sub ComboBoxNicoSet_Enter(sender As System.Object, e As System.EventArgs) Handles ComboBoxNicoSet.Enter
        NicoConvAss_ConfigSet = F_set_ComboboxNicoSet(ComboBoxNicoSet.Text.ToString)
        ComboBoxNicoSet.Text = NicoConvAss_ConfigSet
    End Sub

    Private Sub CheckBoxCanFileOpeWrite_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxCanFileOpeWrite.CheckedChanged
        If CheckBoxCanFileOpeWrite.Checked = True Then
            CanFileOpeWrite = 1
        Else
            If TvRemoteViewer_VB_Start = 1 Then
                Dim result As DialogResult = MessageBox.Show("このチェックを外すとセキュリティは高まりますが" & vbCrLf & "TvRemoteFilesの動作に一部支障が出ます。" & vbCrLf & "TVRVLauncher中心の運用等ロケフリを重視しない場合にチェックを外してもよいでしょう。" & vbCrLf & "※変更は即反映されます。" & vbCrLf & "よろしいですか？", "TvRemoteViewer_VB 確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                If result = DialogResult.Yes Then
                    CanFileOpeWrite = 0
                Else
                    CanFileOpeWrite = 1
                    CheckBoxCanFileOpeWrite.Checked = True
                End If
            Else
                CanFileOpeWrite = 0
            End If
        End If
    End Sub

    Private Sub ButtonShowAccessLog_Click(sender As System.Object, e As System.EventArgs) Handles ButtonShowAccessLog.Click
        Form3.Show()
    End Sub

    Private Sub AccessLogListCheck()
        If AccessLogList IsNot Nothing Then
            'ドメイン解析
            Dim outchk As Integer = 0
            For i As Integer = 0 To AccessLogList.Length - 1
                If AccessLogList(i).domain.Length = 0 Then
                    If isLocalIP(AccessLogList(i).IP) = 1 Then
                        AccessLogList(i).domain = "LAN"
                    Else
                        'IPHostEntryオブジェクトを取得
                        Try
                            Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(AccessLogList(i).IP)
                            If iphe.HostName.Length > 0 Then
                                AccessLogList(i).domain = iphe.HostName
                            End If
                        Catch ex As Exception
                        End Try
                        outchk = 1
                    End If
                End If
            Next

            If outchk = 1 And (form1_ID.Length = 0 Or form1_PASS.Length = 0) Then
                '"※警告　外部からアクセスする場合はIDとPASSを設定してください"
                If NotifyIcon_status = 0 Then
                    Try
                        NotifyIcon1.Icon = My.Resources.TvRemoteViewer_Warning
                        NotifyIcon_status = 1
                    Catch ex2 As Exception
                    End Try
                End If
            End If
        End If
    End Sub
End Class
