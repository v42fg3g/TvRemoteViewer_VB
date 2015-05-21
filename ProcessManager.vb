﻿Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Collections
Imports System.IO.Pipes
Imports System.Security.Principal
Imports System.IO

'★C#からマルチストリームを扱う方法
'http://www110.kir.jp/csharp/chip0208.html
'
'★How to access parallel port from C#?
'http://www.daniweb.com/software-development/csharp/threads/226004/how-to-access-parallel-port-from-c
'
'★方法: ネットワークのプロセス間通信で名前付きパイプを使用する
'http://msdn.microsoft.com/ja-jp/library/bb546085%28v=vs.110%29.aspx
'
Public Class ProcessManager
    Private Const PREFIX_RECTASK_SERVER_PIPE As String = "RecTask_Server_Pipe_"
    Private _list As List(Of ProcessBean) = Nothing
    'Private _maxSize As Integer = 0
    Private _updCount As Integer = 0 '空きUDPポートが取得できないので暫定
    Private _udpPort As Integer = 0
    Private _wwwroot As String = Nothing
    Private _fileroot As String = Nothing

    Public Sub New(udpport As Integer, wwwroot As String, fileroot As String) 'maxSize As Integer)
        Me._list = New List(Of ProcessBean)()
        Me._udpPort = udpport
        Me._wwwroot = wwwroot
        If fileroot.Length > 0 Then
            Me._fileroot = fileroot
        Else
            'm3u8やtsの格納場所が指定されていなければwwwrootと同じ場所とする
            Me._fileroot = wwwroot
        End If
    End Sub

    '停止段階で失敗しているかどうか
    Public Function get_stopping_status(ByVal num As Integer) As Integer
        Dim r As Integer = -1
        For i As Integer = Me._list.Count - 1 To 0 Step -1
            If Me._list(i)._num = num Then
                r = Me._list(i)._stopping
            End If
        Next
        Return r
    End Function

    '本体はProcessBeans内
    Public Sub ffmpeg_http_stream_Start(ByVal num As Integer, output As Stream)
        Dim i As Integer = num2i(num)
        If i >= 0 Then
            Me._list(i).ffmpeg_http_stream_Start(output)
        Else
            log1write("No." & num.ToString & "のプロセスが見つかりません[1]")
        End If
    End Sub

    '本体はProcessBeans内
    Public Sub ffmpeg_http_stream_Stop(ByVal num As Integer)
        Dim i As Integer = num2i(num)
        If i >= 0 Then
            Me._list(i).ffmpeg_http_stream_Stop()
        Else
            log1write("No." & num.ToString & "のプロセスが見つかりません[2]")
        End If
    End Sub

    Public Sub startProc(udpApp As String, udpOpt As String, hlsApp As String, hlsOpt As String, num As Integer, udpPort As Integer, ShowConsole As Integer, stream_mode As Integer, NHK_dual_mono_mode_select As Integer, resolution As String, ByVal VideoSeekSeconds As Integer)
        Dim stopping As Integer = get_stopping_status(num)
        Dim i As Integer = num2i(num)
        Dim http_udp_changing As Integer = 0
        If i >= 0 Then
            http_udp_changing = Me._list(i)._http_udp_changing
        End If

        If stopping > 0 And http_udp_changing = 0 Then
            'HTTPストリームチャンネル変更中に再配信要求が来た場合は初めから
            log1write("停止中のプロセスを終了します")
            'プロセス停止
            stopProc(num)

            stopping = get_stopping_status(num)
            i = num2i(num)
            If i >= 0 Then
                http_udp_changing = Me._list(i)._http_udp_changing
            End If
        End If

        If stopping <= 0 Or http_udp_changing = 1 Then
            '配信されていない-1又は正常配信中0
            'numが放映中でかつBonDriverが同一ならばパイプを使用してチャンネル変更だけを行う
            'ffmpegだけを停止しチャンネル変更が完了するまで.stopping>=100にして完了したら.stopping=0にする
            Dim hls_only As Integer = 0
            Dim hls_only_sid As Integer = 0
            Dim hls_only_chspace As Integer = 0
            Dim hls_only_channel As Integer = 0
            Dim hls_only_TSID As Integer = 0
            Dim hls_only_NID As Integer = 0
            If i >= 0 Then
                'すでに同num=iで配信中である
                Dim BonDriver_prev As String = Trim(instr_pickup_para(Me._list(i)._udpOpt, "/d ", " ", 0))
                Dim BonDriver_next As String = Trim(instr_pickup_para(udpOpt, "/d ", " ", 0))
                'If BonDriver_prev.Length > 0 And BonDriver_next.Length > 0 And BonDriver_prev = BonDriver_next Then
                If RecTask_force_restart = 0 And (BonDriver_prev.Length > 0 And BonDriver_next.Length > 0 And BonDriver_prev = BonDriver_next) Then
                    log1write("BonDriverが同一のためUDPアプリを再起動せずにチャンネル変更を試みます")
                    '同一BonDriverだった場合はRectaskを再起動せずパイプでのチャンネル変更をする
                    '/udp /port 42425 /chspace 0 /sid 51208 /d BonDriver_Spinel_PT3_t0.dll /sendservice 1
                    '切り替えるServiceIDとChSpaceを取得
                    hls_only_sid = Val(Trim(instr_pickup_para(udpOpt, "/sid ", " ", 0)))
                    hls_only_chspace = Val(Trim(instr_pickup_para(udpOpt, "/chspace ", " ", 0)))
                    Dim d() As Integer = F_sid2para(Val(hls_only_sid), Val(hls_only_chspace))
                    hls_only_channel = d(0)
                    hls_only_TSID = d(1)
                    hls_only_NID = d(2)
                    If hls_only_sid > 0 And hls_only_chspace >= 0 And hls_only_TSID > 0 And hls_only_NID > 0 Then
                        hls_only = 1 'パイプでチャンネル変更を行う
                    End If
                End If
            End If

            '新ストリームがファイル再生ならば作成したばかりのassファイルを消さないようにする
            Dim NoDeleteAss As Integer = 0
            If i >= 0 And (stream_mode = 1 Or stream_mode = 3) Then
                NoDeleteAss = 1
            End If

            '★起動している場合は既存のプロセスを止める
            stopProc(num, hls_only, NoDeleteAss) 'hls_only=1ならばHLSアプリのみを停止する
            'stopProcでチャンネル変更ならstopping=-2になる

            stopping = get_stopping_status(num)
            If stopping <= 0 Or stopping = 2 Then
                '関連するファイルを削除
                delete_mystreamnum(num)

                'If Me._list.Count < Me._maxSize Then

                If stream_mode = 0 Or stream_mode = 2 Then
                    'UDPストリーム再生
                    Dim pipeIndex As Integer
                    Dim udpProc As System.Diagnostics.Process

                    'まず名前付きパイプでのチャンネル変更を試みる
                    If hls_only = 1 Then
                        'パイプでチャンネルのみ変更
                        '既存のpipeindex
                        pipeIndex = Me._list(i).GetProcUdpPipeIndex
                        'ここでパイプを使ってチャンネル変更
                        Dim ks As String = Pipe_change_channel(pipeIndex, hls_only_sid, hls_only_chspace, hls_only_channel, hls_only_TSID, hls_only_NID)
                        If ks.IndexOf("""OK""") > 0 Then
                            '成功
                            '既存プロセス
                            udpProc = Me._list(i).GetUdpProc
                            udpProc.WaitForInputIdle()
                            log1write("No.=" & num & "名前付きパイプを使用してチャンネルを変更しました")
                            log1write("pipeindex=" & pipeIndex & " sid=" & hls_only_sid & " chspace=" & hls_only_chspace & " channel=" & hls_only_channel)
                        Else
                            log1write(ks)
                            '失敗したら引き続き通常起動を試みる
                            log1write("No.=" & num & "名前付きパイプを使用してのチャンネル変更に失敗しました。通常起動を試みます")
                            log1write("No.=" & num & "UDPアプリの再起動を試みます")
                            hls_only = 0
                            stopProc(num)
                            If get_stopping_status(num) > 0 Then
                                '結局何をやっても失敗
                                log1write("No.=" & num & "のプロセスは使用できません。UDPアプリの停止に失敗したようです")
                                '現在稼働中のlist(i)._numをログに表示
                                log1write("現在稼働中のNumber：" & get_live_numbers())
                                Exit Sub
                            End If
                        End If
                    End If

                    If hls_only = 0 Then
                        '通常起動
                        Dim pipeListBefore As New List(Of Integer)()
                        'If Path.GetFileName(udpApp).Equals("RecTask.exe") Then
                        If Path.GetFileName(udpApp).Contains("RecTask") Then
                            '★実行されている名前付きパイプのリストを取得する(プロセス実行前)
                            Dim listOfPipes As String() = Nothing
                            listOfPipes = GetPipes()
                            If listOfPipes IsNot Nothing Then
                                For Each pipeName As String In listOfPipes
                                    If pipeName.Contains("RecTask_Server_Pipe_") Then
                                        Dim pindex1 As Integer = 0
                                        Try
                                            pindex1 = Val(pipeName.Substring(pipeName.IndexOf("RecTask_Server_Pipe_") + "RecTask_Server_Pipe_".Length))
                                        Catch ex As Exception
                                        End Try
                                        If pindex1 > 0 Then
                                            pipeListBefore.Add(pindex1)
                                        End If
                                    End If
                                Next
                            Else
                                '何をやってもエラー
                                log1write("No.=" & num & "のUDPアプリ実行前パイプ一覧取得に失敗しました")
                                Exit Sub
                            End If
                        End If

                        '★UDPソフトを実行
                        'ProcessStartInfoオブジェクトを作成する
                        Dim udpPsi As New System.Diagnostics.ProcessStartInfo()
                        '起動するファイルのパスを指定する
                        udpPsi.FileName = udpApp
                        'コマンドライン引数を指定する
                        udpPsi.Arguments = udpOpt
                        'ログ表示
                        log1write("UDP アプリ=" & udpApp)
                        log1write("UDP option=" & udpOpt)
                        'アプリケーションを起動する
                        udpProc = System.Diagnostics.Process.Start(udpPsi)
                        Dim UDP_PRIORITY_STR As String = UDP_PRIORITY
                        Select Case UDP_PRIORITY
                            Case "Idle"
                                udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle
                            Case "Normal"
                                udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal
                            Case "High"
                                udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.High
                            Case "RealTime"
                                udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime
                            Case Else
                                UDP_PRIORITY_STR = "無指定"
                                'udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.High
                        End Select
                        log1write("No.=" & num & "のUDPアプリを起動しました。優先度：" & UDP_PRIORITY_STR & "　handle=" & udpProc.Handle.ToString)

                        'pipeindexを取得
                        pipeIndex = 0
                        Dim chk As Integer = 0
                        While chk < 200
                            'RecTaskのパイプが増加するまで繰り返す
                            'If Path.GetFileName(udpApp).Equals("RecTask.exe") Then
                            If Path.GetFileName(udpApp).Contains("RecTask") Then
                                '★実行されている名前付きパイプのリストを取得する(プロセス実行後)
                                Dim listOfPipes As String() = Nothing
                                'listOfPipes = System.IO.Directory.GetFiles("\\.\pipe\")
                                listOfPipes = GetPipes()
                                If listOfPipes Is Nothing Then
                                    log1write("No.=" & num & "のUDPアプリ実行後パイプ一覧取得に失敗しました")
                                    Try
                                        udpProc.Kill()
                                        udpProc.Dispose()
                                        udpProc.Close()
                                    Catch ex As Exception
                                    End Try
                                    log1write("No.=" & num & "のUDPアプリを強制終了しました[F]")
                                    Exit Sub
                                End If

                                For Each pipeName As String In listOfPipes
                                    If pipeName.Contains("RecTask_Server_Pipe_") Then
                                        Dim pindex2 As Integer = 0
                                        Try
                                            pindex2 = pipeName.Substring(pipeName.IndexOf("RecTask_Server_Pipe_") + 20)
                                        Catch ex As Exception
                                        End Try
                                        If pindex2 > 0 Then
                                            'log1write("After PipeName=" & pipeName & " PipeIndex=" & pindex2)
                                            Dim c2 As Integer = 0
                                            '起動前のパイプindexに存在しなければOK
                                            For Each pt As Integer In pipeListBefore
                                                If pindex2 = pt Then
                                                    c2 += 100
                                                End If
                                            Next
                                            '該当するpipeindexが見つからなければ新規
                                            If c2 = 0 Then
                                                pipeIndex = pindex2
                                                Exit While
                                            End If
                                        Else
                                            pipeIndex = pindex2
                                            Exit While
                                        End If
                                    End If
                                Next
                            End If
                            System.Threading.Thread.Sleep(50)
                            chk += 1
                        End While

                        '配信が期待されるサービスID
                        hls_only_sid = Trim(instr_pickup_para(udpOpt, "/sid ", " ", 0))
                    End If

                    If pipeIndex > 0 Then
                        log1write("No.=" & num & "のパイプインデックスを取得しました。pipeindex=" & pipeIndex.ToString)

                        udpProc.WaitForInputIdle()

                        '実際にチャンネルが変わったかどうかパイプで確認してから次へ
                        'GetChannelでサービスIDが変わっているかチェック
                        Dim j As Integer = 10 '最大10回チャレンジ
                        While Pipe_get_channel(pipeIndex, hls_only_sid) = 0 And j >= 0
                            log1write("No.=" & num & "UDPの配信チャンネルが切り替わるまで待機しています")
                            j -= 1
                            System.Threading.Thread.Sleep(50)
                        End While
                        If j >= 0 Then
                            log1write("No.=" & num & "UDPの配信チャンネル切り替えに成功しました")
                        Else
                            log1write("No.=" & num & "UDPの配信チャンネル切り替えに失敗しました")
                        End If

                        If j >= 0 Then
                            'チャンネル切り替えに成功したので引き続きHLSアプリ起動

                            System.Threading.Thread.Sleep(UDP2HLS_WAIT) '1000からちょっと少なくしてみた　0でもほぼおｋだが極希にHLSエラーが起こった

                            If HTTPSTREAM_App = 2 And hlsApp.IndexOf("ffmpeg.exe") >= 0 And (stream_mode = 2 Or stream_mode = 3) Then
                                'ffmpeg HTTPストリーム
                                'この場合、ffmpegはすぐには実行しない 後でwatch.tsにアクセスがあったときに起動
                                'ProcessBeans作成
                                If hls_only = 1 Then
                                    '既存のリストを削除してから改めて追加
                                    Me._list.RemoveAt(i)
                                End If
                                '                                  ↓Processはまだ決まっていない
                                Dim pb As New ProcessBean(udpProc, Nothing, num, pipeIndex, udpApp, udpOpt, hlsApp, hlsOpt, udpPort, ShowConsole, stream_mode, NHK_dual_mono_mode_select, resolution, "", 0)
                                Me._list.Add(pb)

                                '1秒毎のプロセスチェックさせない
                                'Me._list(num2i(num))._stopping >= 100
                                log1write("配信が開始されない場合は" & FFMPEG_HTTP_CUT_SECONDS & "秒後に配信を終了します。")
                                Me._list(num2i(num))._stopping = 100 + FFMPEG_HTTP_CUT_SECONDS 'チャンネル変更ならば数秒以内に処理されるかな。100になるFFMPEG_HTTP_CUT_SECONDS秒後にタイマーにより配信は停止される
                            Else
                                '通常
                                '★HLSソフトを実行
                                'ProcessStartInfoオブジェクトを作成する
                                Dim hlsPsi As New System.Diagnostics.ProcessStartInfo()
                                '起動するファイルのパスを指定する
                                hlsPsi.FileName = hlsApp
                                'VLCは上の非表示コマンドが効かないのでオプションを書き換える
                                If ShowConsole = False And hlsApp.IndexOf("vlc") >= 0 Then
                                    If hlsOpt.IndexOf("--dummy-quiet") < 0 And hlsOpt.IndexOf("-I dummy") >= 0 Then
                                        hlsOpt = hlsOpt.Replace("-I dummy", "-I dummy --dummy-quiet")
                                    End If
                                    If hlsOpt.IndexOf("--rc-quiet") < 0 And hlsOpt.IndexOf("--rc-host=") >= 0 Then
                                        hlsOpt = hlsOpt.Replace("--rc-host=", "--rc-quiet --rc-host=")
                                    End If
                                End If
                                'コマンドライン引数を指定する
                                hlsPsi.Arguments = hlsOpt
                                If ShowConsole = False Then
                                    ' コンソール・ウィンドウを開かない
                                    hlsPsi.CreateNoWindow = True
                                    ' シェル機能を使用しない
                                    hlsPsi.UseShellExecute = False
                                End If
                                'ログ表示
                                log1write("No.=" & num & " HLS アプリ=" & hlsApp)
                                log1write("No.=" & num & " HLS option=" & hlsOpt)
                                'アプリケーションを起動する
                                Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)
                                Dim HLS_PRIORITY_STR As String = HLS_PRIORITY
                                Select Case HLS_PRIORITY
                                    Case "Idle"
                                        hlsProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle
                                    Case "Normal"
                                        hlsProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal
                                    Case "High"
                                        hlsProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.High
                                    Case "RealTime"
                                        hlsProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime
                                    Case Else
                                        HLS_PRIORITY_STR = "無指定"
                                        'hlsProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.High
                                End Select

                                log1write("No.=" & num & "のHLSアプリを起動しました。優先度：" & HLS_PRIORITY_STR & "　handle=" & hlsProc.Handle.ToString)

                                If hls_only = 1 Then
                                    '既存のリストを削除してから改めて追加
                                    Me._list.RemoveAt(i)
                                End If

                                'Dim pb As New ProcessBean(udpProc, hlsProc, num, pipeIndex)'↓再起動用にパラメーターを渡しておく
                                Dim pb As New ProcessBean(udpProc, hlsProc, num, pipeIndex, udpApp, udpOpt, hlsApp, hlsOpt, udpPort, ShowConsole, stream_mode, NHK_dual_mono_mode_select, resolution, "", 0)
                                Me._list.Add(pb)
                            End If
                        Else
                            'チャンネル切り替え失敗したのでUDPアプリを終了させる
                            If num2i(num) >= 0 Then
                                'RecTaskを終了させる
                                stopProc(num)
                                log1write("No.=" & num & "　チャンネル変更に失敗したので配信を中止しました")
                            Else
                                'RecTaskを終了させる（まだlistが作られていない場合）
                                Dim rr As Integer = stroUdpProc_by_pipeindex(num, pipeIndex, udpProc)
                            End If
                        End If
                    Else
                        Try
                            'UDPアプリ終了　pipeindexがわからないので強制終了
                            udpProc.Kill()
                            If wait_stop_proc(udpProc) = 1 Then
                                log1write("No.=" & num & "　名前付きパイプ一覧取得に失敗したのでUDPアプリを終了しました")
                            Else
                                log1write("No.=" & num & "　名前付きパイプ一覧取得に失敗したのでUDPアプリ終了を試みましたが失敗しました")
                            End If
                            udpProc.Dispose()
                            udpProc.Close()
                        Catch ex As Exception
                            log1write("No.=" & num & "　名前付きパイプ一覧取得に失敗したのでUDPアプリを終了しました[B]")
                        End Try
                    End If
                ElseIf stream_mode = 1 Or stream_mode = 3 Then
                    'ファイル再生
                    'フルパスファイル名を取得
                    Dim fullpathfilename As String = trim8(Instr_pickup(hlsOpt, "-i """, """", 0)) 'ffmpeg限定
                    If HTTPSTREAM_App = 2 And hlsApp.IndexOf("ffmpeg.exe") >= 0 And (stream_mode = 2 Or stream_mode = 3) Then
                        'ffmpeg HTTPストリーム ファイル再生
                        'この場合、ffmpegはすぐには実行しない 後でwatch.tsにアクセスがあったときに起動
                        'すでにリストにある場合はリストから取り除いた後に改めて作成
                        If i >= 0 Then
                            Me._list.RemoveAt(i)
                        End If
                        'ProcessBeans作成
                        '                                  ↓Processはまだ決まっていない
                        Dim pb As New ProcessBean(Nothing, Nothing, num, 0, udpApp, udpOpt, hlsApp, hlsOpt, udpPort, ShowConsole, stream_mode, 0, resolution, fullpathfilename, VideoSeekSeconds)
                        Me._list.Add(pb)

                        '1秒毎のプロセスチェックさせない
                        'Me._list(num2i(num))._stopping >= 100
                        log1write("配信が開始されない場合は" & FFMPEG_HTTP_CUT_SECONDS & "秒後に配信を終了します。")
                        Me._list(num2i(num))._stopping = 100 + FFMPEG_HTTP_CUT_SECONDS 'チャンネル変更ならば数秒以内に処理されるかな。100になるFFMPEG_HTTP_CUT_SECONDS秒後にタイマーにより配信は停止される
                    Else
                        '★HLSソフトを実行
                        'ProcessStartInfoオブジェクトを作成する
                        Dim hlsPsi As New System.Diagnostics.ProcessStartInfo()
                        '起動するファイルのパスを指定する
                        hlsPsi.FileName = hlsApp
                        'コマンドライン引数を指定する
                        hlsPsi.Arguments = hlsOpt
                        If ShowConsole = False Then
                            ' コンソール・ウィンドウを開かない
                            hlsPsi.CreateNoWindow = True
                            ' シェル機能を使用しない
                            hlsPsi.UseShellExecute = False
                        End If
                        'ログ表示
                        log1write("No.=" & num & "HLS アプリ=" & hlsApp)
                        log1write("No.=" & num & "HLS option=" & hlsOpt)
                        'アプリケーションを起動する
                        Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)

                        log1write("No.=" & num & "のHLSアプリを起動しました。handle=" & hlsProc.Handle.ToString)

                        'Dim pb As New ProcessBean(udpProc, hlsProc, num, pipeIndex)'↓再起動用にパラメーターを渡しておく
                        Dim pb As New ProcessBean(Nothing, hlsProc, num, 0, udpApp, udpOpt, hlsApp, hlsOpt, udpPort, ShowConsole, stream_mode, 0, resolution, fullpathfilename, VideoSeekSeconds)
                        Me._list.Add(pb)
                    End If
                End If
                'End If
            End If
        ElseIf stopping = 1 Then
            log1write("No." & num.ToString & "は配信停止処理中です")
        ElseIf stopping = 2 Then
            log1write("No." & num.ToString & "はUDPサービスID変更処理宙です")
        ElseIf stopping >= 100 Then
            log1write("No." & num.ToString & "はffmpegHTTPストリーム配信待機中です")
        End If

        '現在稼働中のlist(i)._numをログに表示
        Dim js As String = get_live_numbers()
        log1write("現在稼働中のNumber：" & js)

        '番組表用にライブストリームを記録
        LIVE_STREAM_STR = WI_GET_LIVE_STREAM()
    End Sub

    'このプロセスがＮＨＫ関連かどうか調べる　■未使用
    Public Function check_isNHK(ByVal num As Integer, Optional ByVal udpOpt As String = "") As Integer
        Dim r As Integer = 0
        Dim i As Integer = -1
        If num > 0 Then
            '_list()からudpOptを取得
            i = num2i(num)
            If i >= 0 Then
                udpOpt = Me._list(i)._udpOpt
            End If
        End If

        Dim sp As Integer = udpOpt.IndexOf("/sid ")
        Dim ep As Integer = udpOpt.IndexOf(" ", sp + "/sid ".Length)
        Dim sid As Integer = 0
        If sp > 0 And ep > sp Then
            sid = Val(udpOpt.Substring(sp + "/sid ".Length, ep - sp - "/sid ".Length))
        End If
        Dim chspace As Integer = Val(Instr_pickup(udpOpt, "/chspace ", " ", 0))
        Dim hosokyoku_name As String = ""
        If sid > 0 Then
            hosokyoku_name = StrConv(Trim(F_sid2channelname(sid, chspace)), VbStrConv.Wide)
        End If
        If hosokyoku_name.IndexOf("ＮＨＫ") >= 0 Then
            r = 1
        End If
        Return r
    End Function

    'numから_list(i)のiを取得する
    Public Function num2i(ByVal num As Integer) As Integer
        Dim r As Integer = -1

        If Me._list.Count > 0 Then
            For i As Integer = 0 To Me._list.Count - 1
                If Me._list(i)._num = num Then
                    r = i
                    Exit For
                End If
            Next
        End If

        Return r
    End Function

    'プロセスが順調に動いているかチェック
    Public Sub checkAllProc()
        Try
            For i As Integer = Me._list.Count - 1 To 0 Step -1
                If Me._list(i)._stopping > 100 Then
                    Me._list(i)._stopping -= 1
                    If Me._list(i)._stopping = 100 Then
                        'ffmpeg HTTPストリーム クライアントが終了したのでストリームを終了させる
                        log1write("No." & Me._list(i)._num & " ffmpegHTTPストリームの終了予約を検知しました")
                        stopProc(Me._list(i)._num)
                    End If
                ElseIf Me._list(i)._stopping = 1 Then
                    '終了途中
                ElseIf Me._list(i)._stopping = 2 Then
                    'チャンネル変更中
                ElseIf Me._list(i)._stopping >= 100 Then '=3
                    'ffmpeg HTTPストリーム UDPアプリだけが起動してHLSアプリの起動を待っている
                ElseIf Me._list(i)._num > 0 And Me._list(i)._stopping = 0 And (Me._list(i)._stream_mode = 0 Or Me._list(i)._stream_mode = 2) Then
                    '通常再生　順調かどうかチェック
                    Dim chk As Integer = 0
                    Dim procudp As System.Diagnostics.Process = Nothing
                    Dim prochls As System.Diagnostics.Process = Nothing
                    procudp = Me._list(i).GetUdpProc()
                    prochls = Me._list(i).GetHlsProc()
                    If procudp IsNot Nothing AndAlso Not procudp.HasExited Then
                    Else
                        'エラー
                        Me._list(i)._chk_proc += 100
                        chk += 1
                        log1write("No.=" & Me._list(i)._num.ToString & " UDPアプリが応答しません")
                    End If
                    'If procudp.Responding Then
                    'Else
                    '応答しない
                    'Me._list(i)._chk_proc += 10
                    'chk += 1
                    'log1write("UDPアプリが応答しません[B] No.=" & Me._list(i)._num.ToString)
                    'End If
                    If prochls IsNot Nothing AndAlso Not prochls.HasExited Then
                    Else
                        'エラー
                        Me._list(i)._chk_proc += 100
                        chk += 1
                        log1write("No.=" & Me._list(i)._num.ToString & " HLSアプリが応答しません")
                    End If
                    'If prochls.Responding Then
                    'Else
                    '応答しない
                    'Me._list(i)._chk_proc += 10
                    'chk += 1
                    'log1write("HLSアプリが応答しません[B] No.=" & Me._list(i)._num.ToString)
                    'End If

                    If chk = 0 Then
                        'エラーがなければエラーカウンタをリセット
                        Me._list(i)._chk_proc = 0
                    End If

                    If Me._list(i)._chk_proc >= 300 Then 'プロセスが無いか3秒応答がなければ
                        'エラーカウンタをリセット
                        Me._list(i)._chk_proc = 0
                        '終了して再起動
                        'stopProcする前に起動パラメーターを一時待避
                        Dim p1 As String = Me._list(i)._udpApp
                        Dim p2 As String = Me._list(i)._udpOpt
                        Dim p3 As String = Me._list(i)._hlsApp
                        Dim p4 As String = Me._list(i)._hlsOpt
                        Dim p5 As Integer = Me._list(i)._num
                        Dim p6 As Integer = Me._list(i)._udpPort
                        Dim p7 As Boolean = Me._list(i)._ShowConsole
                        Dim p8 As Integer = Me._list(i)._stream_mode
                        Dim p9 As Integer = Me._list(i)._NHK_dual_mono_mode_select
                        Dim p10 As String = Me._list(i)._resolution
                        Dim p11 As Integer = Me._list(i)._VideoSeekSeconds
                        'プロセスを停止
                        stopProc(p5) 'startprocでも冒頭で停止処理をするので割愛と思ったが再起動時には停止しておいたほうが正常に動いた
                        'プロセスを開始
                        startProc(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11)
                        log1write("No.=" & p5 & "のプロセスを再起動しました")
                    End If
                End If
            Next

        Catch ex As Exception
            '終了時などに希に既に存在していないMe._list(i)へのアクセス
        End Try
    End Sub

    '指定numberプロセスを停止する
    Public Sub stopProc(num As Integer, Optional ByVal hls_only As Integer = 0, Optional ByVal NoDeleteAss As Integer = 0)
        'index = -1 の場合は全て停止
        Dim proc As System.Diagnostics.Process = Nothing

        Dim hls_stop As Integer = 0 '上手く停止したら1
        Dim udp_stop As Integer = 0 '上手く停止したら1

        If Me._list.Count > 0 Then
            For i As Integer = Me._list.Count - 1 To 0 Step -1
                If Me._list(i)._num = num Or num <= -1 Then

                    log1write("No.=" & num & "のプロセスを停止しています")

                    '停止フラグを書き込んでおいたほうがいいかな
                    Me._list(i)._stopping = 1 'checkAllProc()でチェックさせないため・・

                    '★ HLS
                    Try
                        proc = Me._list(i).GetHlsProc()
                        If proc IsNot Nothing AndAlso Not proc.HasExited Then
                            If Me._list(i)._hlsApp.IndexOf("vlc") >= 0 Then
                                'VLCならば
                                If quit_VLC(i) = 1 Then
                                    'プロセスが無くなるまで待機
                                    If wait_stop_proc(proc) = 1 Then
                                        log1write("No.=" & num & "のVLCを終了しました")
                                        hls_stop = 1
                                    Else
                                        log1write("No.=" & num & "のVLC終了に失敗しました")
                                    End If
                                    proc.Close()
                                    proc.Dispose()
                                Else
                                    '強制的に終了
                                    'proc.CloseMainWindow()
                                    proc.Kill()
                                    If wait_stop_proc(proc) = 1 Then
                                        log1write("No.=" & num & "のVLCを強制終了しました")
                                        hls_stop = 1
                                    Else
                                        log1write("No.=" & num & "のVLC強制終了に失敗しました")
                                    End If
                                    'proc.WaitForExit()
                                    proc.Close()
                                    proc.Dispose()
                                End If
                            ElseIf Me._list(i)._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                'ffmpeg
                                'コンソールが表示されてないときにも有効かどうかわからないのでsendkeyは却下
                                'AppActivate(proc.Id) '最前面にする
                                'SendKeys.Send("q") 'qを送る
                                'F_sendkeycode(&HD) 'エンターキー
                                'proc.CloseMainWindow()
                                'proc.WaitForExit()

                                If Me._list(i)._stream_mode = 2 Or Me._list(i)._stream_mode = 3 Then
                                    ffmpeg_http_stream_Stop(num)
                                    log1write("No.=" & num & "のffmpeg HTTPストリームを終了しました")
                                End If

                                proc.Kill()
                                If wait_stop_proc(proc) = 1 Then
                                    log1write("No.=" & num & "のffmpegを強制終了しました")
                                    hls_stop = 1
                                Else
                                    log1write("No.=" & num & "のffmpeg強制終了に失敗しました")
                                End If
                                proc.Close()
                                proc.Dispose()
                            Else
                                '強制的に終了
                                'proc.CloseMainWindow()
                                proc.Kill()
                                If wait_stop_proc(proc) = 1 Then
                                    log1write("No.=" & num & "のhlsアプリを強制終了しました")
                                    hls_stop = 1
                                Else
                                    log1write("No.=" & num & "のhlsアプリ強制終了に失敗しました")
                                End If
                                proc.Close()
                                proc.Dispose()
                            End If

                            'ウェイトを入れないと関連ファイル削除に失敗することがある →　ファイル削除のところで対処
                            'System.Threading.Thread.Sleep(1000)
                        Else
                            'プロセスが無い
                            hls_stop = 1
                        End If
                    Catch ex As Exception
                        'Procが存在しない
                        log1write("No.=" & num & "のHLSプロセスが見つかりません。" & ex.Message)
                        hls_stop = 1
                    End Try

                    If hls_only = 0 Then
                        '★ UDP
                        Try
                            proc = Me._list(i).GetUdpProc()
                            If proc IsNot Nothing AndAlso Not proc.HasExited Then
                                Dim udpPipeIndex As Integer = Me._list(i).GetProcUdpPipeIndex
                                If udpPipeIndex > 0 Then
                                    'RecTaskを終了させる
                                    udp_stop = stroUdpProc_by_pipeindex(num, udpPipeIndex, proc)
                                Else
                                    'パイプが見当たらない
                                    '強制的に終了
                                    'proc.CloseMainWindow()
                                    proc.Kill()
                                    'proc.WaitForExit()
                                    If wait_stop_proc(proc) = 1 Then
                                        log1write("No.=" & num & "のudpアプリを強制終了しました[D]")
                                        udp_stop = 1
                                    Else
                                        log1write("No.=" & num & "のudpアプリ強制終了に失敗しました")
                                    End If
                                    proc.Close()
                                    proc.Dispose()
                                End If
                            Else
                                'プロセスが無い
                                udp_stop = 1
                            End If
                        Catch ex As Exception
                            'Procが存在しない
                            log1write("No.=" & num & "のUDPプロセスが見つかりません。" & ex.Message)
                            udp_stop = 1
                        End Try
                    Else
                        'パイプでのチャンネル変更によって、停止するように指定されていない
                        udp_stop = 1
                    End If

                    '関連するファイルを削除
                    delete_mystreamnum(Me._list(i)._num)
                    If NoDeleteAss = 0 Then
                        '古いsub%num%.assがあれば削除
                        If file_exist(Me._fileroot & "\" & "sub" & Me._list(i)._num.ToString & ".ass") = 1 Then
                            deletefile(Me._fileroot & "\" & "sub" & Me._list(i)._num.ToString & ".ass")
                        End If
                        If file_exist(Me._fileroot & "\" & "sub" & Me._list(i)._num.ToString & "_nico.ass") = 1 Then
                            deletefile(Me._fileroot & "\" & "sub" & Me._list(i)._num.ToString & "_nico.ass")
                        End If
                    End If

                    Try
                        '★ リストから取り除く
                        If hls_only = 0 Then
                            If num = -2 Then
                                '強制全停止　実際に停止されたかどうかかまわず
                                Me._list.RemoveAt(i)
                                'delete_mystreamnum(num) 'm3u8,tsを削除
                                log1write("No.=" & num & "のプロセスを停止しました")
                            ElseIf hls_stop = 1 And udp_stop = 1 Then
                                Me._list.RemoveAt(i)
                                'delete_mystreamnum(num) 'm3u8,tsを削除
                                log1write("No.=" & num & "のプロセスを停止しました")
                            Else
                                log1write("No.=" & num & "のプロセス停止に失敗しました")
                                'me._list(i)._stopping=1のまま残るので後で判別できる
                            End If
                        ElseIf hls_only = 1 Then
                            'Me._listからの削除はしない
                            If hls_stop = 1 Then 'udpは起動し続けている
                                log1write("No.=" & num & "のHLSプロセスを停止しました")
                                Me._list(i)._stopping = 2 '後で無事起動したときに0にする
                            Else
                                log1write("No.=" & num & "のHLSプロセス停止に失敗しました")
                                'me._list(i)._stopping=1のまま残るので後で判別できる
                            End If
                        End If
                    Catch ex As Exception
                        log1write("停止時listから取り除く際のエラー:" & ex.Message)
                    End Try

                    'System.Threading.Thread.Sleep(1000)
                End If
            Next
        Else
            log1write("配信中のストリームlistが存在しません")
        End If

        If hls_only = 0 Then
            If num = -2 Then
                log1write("関連アプリのプロセスを停止しています")
                If Stop_vlc_at_StartEnd = 1 Then
                    stopProcName("vlc")
                End If
                If Stop_ffmpeg_at_StartEnd = 1 Then
                    stopProcName("ffmpeg")
                End If
                If Stop_RecTask_at_StartEnd = 1 Then
                    stopProcName("RecTask")
                End If
                Me._list.Clear() 'リストクリア
                log1write("関連アプリのプロセスを停止しました")
            End If
        End If

        '現在稼働中のlist(i)._numをログに表示
        Dim js As String = get_live_numbers()
        log1write("現在稼働中のNumber：" & js)

        '番組表用にライブストリームを記録
        LIVE_STREAM_STR = WI_GET_LIVE_STREAM()
    End Sub

    '名前付きパイプを使用してRecTaskを終了させる
    Public Function stroUdpProc_by_pipeindex(ByVal num As Integer, ByVal udpPipeIndex As Integer, ByRef proc As System.Diagnostics.Process) As Integer
        Dim r As Integer = 1

        '名前付きパイプを使用して終了を試みる
        proc.WaitForInputIdle()
        Dim rr As String = sendRecTaskMsg("EndTask", udpPipeIndex)

        'UDPソフトが終了するまで待つ
        '★実行されている名前付きパイプのリストを取得する(プロセス実行前)
        Dim chk As Integer = 1000
        While chk > 0
            Dim listOfPipes As String() = Nothing
            'listOfPipes = System.IO.Directory.GetFiles("\\.\pipe\")
            listOfPipes = GetPipes()
            If listOfPipes Is Nothing Then
                '名前付きパイプ一覧取得でエラーが起こった
                '外部プログラムによりパイプ一覧取得を試みる
                listOfPipes = F_get_NamedPipeList_from_program()
                If listOfPipes Is Nothing Then
                    '何をやってもエラー
                    chk = -1
                    Exit While
                End If
            End If

            Dim chk2 As Integer = 0
            For Each pipeName As String In listOfPipes
                If pipeName.Contains("RecTask_Server_Pipe_") Then
                    Dim pindex1 As Integer = 0
                    Try
                        pindex1 = Val(pipeName.Substring(pipeName.IndexOf("RecTask_Server_Pipe_") + 20))
                    Catch ex As Exception
                    End Try
                    If pindex1 = udpPipeIndex Then
                        'まだ起動中
                        chk2 = 1
                        If chk Mod 100 = 1 Then
                            '1秒毎に何度も送る
                            proc.WaitForInputIdle()
                            sendRecTaskMsg("EndTask", udpPipeIndex)
                        End If
                    End If
                End If
            Next
            If chk2 = 0 Then
                'パイプ一覧に該当が無くなった
                Exit While
            End If
            System.Threading.Thread.Sleep(10)
            chk -= 1
        End While
        If chk > 0 Then
            '最後にプロセスチェック
            If wait_stop_proc(proc) = 1 Then
                log1write("No.=" & num & "のUDPアプリを終了しました。")
            Else
                log1write("No.=" & num & "のUDPアプリの名前付きパイプによる終了が行われたはずにもかかわらず[C]")
                log1write("No.=" & num & "のUDPアプリのプロセスが残っています。")
                r = 0
            End If
        Else
            'タイムアウト
            log1write("No.=" & num & "のUDPアプリの名前付きパイプによる終了に失敗しました")
            log1write("No.=" & num & "のUDPアプリの強制終了を試みます")
            '引き続き強制終了を試みる
            proc.Kill()
            '最後にプロセスチェック
            If wait_stop_proc(proc) = 1 Then
                log1write("No.=" & num & "のudpアプリを強制終了しました[E]")
            Else
                log1write("No.=" & num & "のUDPアプリの強制終了に失敗しました")
                log1write("No.=" & num & "のUDPアプリのプロセスが残っています")
                r = 0
            End If
        End If
        proc.Close()
        proc.Dispose()

        Return r
    End Function

    'ストリームnumの放送局名を取得する
    Public Function get_channelname(ByVal num As Integer) As String
        Dim r As String = ""
        Dim i As Integer = num2i(num) 'numから_list(i)のiに変換
        If i >= 0 Then
            If Me._list(i)._stream_mode = 1 Then
                r = "ファイル再生"
            Else
                Dim udpOpt As String = Me._list(i)._udpOpt
                'udpOptからsidを抜き出す
                Dim sp As Integer = udpOpt.IndexOf("/sid ")
                Dim ep As Integer = udpOpt.IndexOf(" ", sp + "/sid ".Length)
                Dim sid As Integer = 0
                If sp >= 0 And ep > sp Then
                    sid = Val(udpOpt.Substring(sp + "/sid ".Length, ep - sp - "/sid ".Length))
                End If
                'udpOptからchspaceを抜き出す
                sp = udpOpt.IndexOf("/chspace ")
                ep = udpOpt.IndexOf(" ", sp + "/chspace ".Length)
                Dim chspace As Integer = -1
                If sp >= 0 And ep > sp Then
                    chspace = Val(udpOpt.Substring(sp + "/chspace ".Length, ep - sp - "/chspace ".Length))
                End If
                If sid > 0 And chspace >= 0 Then
                    Dim j As Integer = -1
                    If ch_list IsNot Nothing Then
                        For k As Integer = 0 To ch_list.Length - 1
                            If ch_list(k).sid = sid And ch_list(k).chspace = chspace Then
                                j = k
                                Exit For
                            End If
                        Next
                    End If
                    If j >= 0 Then
                        r = ch_list(j).jigyousha
                    End If
                End If
            End If
        End If
        Return r
    End Function

    '配信番号numののlist(i)._stream_modeを取得
    Public Function get_stream_mode(ByVal num As Integer) As Integer
        Dim r As Integer = Me._list(num2i(num))._stream_mode
        Return r
    End Function

    '現在稼働中のlistナンバーをnumでソートして返す
    Public Function get_live_index_sort() As Object
        Dim r() As Integer = Nothing
        Dim s() As Integer = Nothing
        Dim i As Integer = 0
        Dim j As Integer = 0
        If Me._list.Count > 0 Then
            For j = 0 To Me._list.Count - 1
                ReDim Preserve r(j)
                r(j) = Me._list(j)._num
                ReDim Preserve s(j)
                s(j) = j
            Next

            '並び替え
            If r.Length > 1 Then
                Dim temp As Integer = -1
                For j = 0 To Me._list.Count - 2
                    For i = 0 To Me._list.Count - 2
                        If r(i + 1) < r(i) Then
                            temp = r(i)
                            r(i) = r(i + 1)
                            r(i + 1) = temp
                            temp = s(i)
                            s(i) = s(i + 1)
                            s(i + 1) = temp
                        End If
                    Next
                Next
            End If
        End If

        Return s
    End Function

    '現在稼働中のlist(i)._numを取得
    Public Function get_live_numbers() As String
        Dim js As String = " "
        Dim d() As Integer = get_live_index_sort() 'listナンバーがnumでソートされて返ってくる
        If d IsNot Nothing Then
            For j As Integer = 0 To d.Length - 1
                If Me._list(d(j))._stopping > 0 Then
                    js &= "x" & Me._list(d(j))._num.ToString & " "
                Else
                    js &= Me._list(d(j))._num.ToString & " "
                End If
            Next
        End If
        Return js
    End Function

    '現在稼働中のlist(i)._stream_modeを取得
    Public Function get_live_stream_mode() As Object
        Dim r() As Integer = Nothing
        Dim d() As Integer = get_live_index_sort() 'listナンバーがnumでソートされて返ってくる
        If d IsNot Nothing Then
            For j As Integer = 0 To d.Length - 1
                ReDim Preserve r(j)
                r(j) = Me._list(j)._stream_mode
            Next
        End If
        Return r
    End Function

    '現在稼働中のlist(i)._hlsAppの種類を取得 vlc=1 ffmpeg=2
    Public Function get_live_hlsApp() As Object
        Dim r() As Integer = Nothing
        Dim d() As Integer = get_live_index_sort() 'listナンバーがnumでソートされて返ってくる
        If d IsNot Nothing Then
            For j As Integer = 0 To d.Length - 1
                ReDim Preserve r(j)
                If Me._list(j)._hlsApp.IndexOf("vlc") >= 0 Then
                    r(j) = 1
                ElseIf Me._list(j)._hlsApp.IndexOf("ffmpeg") >= 0 Then
                    r(j) = 2
                End If
            Next
        End If
        Return r
    End Function

    '現在稼働中のlist(i)に関するBonDriver情報を併せて取得
    Public Function get_live_numbers_bon() As String
        Dim js As String = ""
        Dim d() As Integer = get_live_index_sort() 'listナンバーがnumでソートされて返ってくる
        If d IsNot Nothing Then
            For j As Integer = 0 To d.Length - 1
                If Me._list(d(j))._stopping > 0 Then
                    js &= "x"
                End If
                Dim s As String = Me._list(d(j))._udpOpt
                Dim sp As Integer = s.ToLower.LastIndexOf("bondriver")
                Dim ep As Integer = s.IndexOf(".dll")
                Dim bon As String = ""
                If sp >= 0 And ep > sp Then
                    bon = s.Substring(sp, (ep - sp) + ".dll".Length)
                End If
                If Me._list(d(j))._stream_mode = 1 Then
                    bon = "ファイル再生"
                End If
                js &= Me._list(d(j))._num.ToString & ": " & bon & vbCrLf
            Next
        End If

        Return js
    End Function

    Private Function wait_stop_proc(ByRef proc As System.Diagnostics.Process) As Integer
        'プロセスが停止するまで待機 プロセスはByrefで受け継ぐ
        Dim r As Integer = 0
        Dim i As Integer = 1000 '10秒待つ
        Try
            While (proc IsNot Nothing AndAlso Not proc.HasExited) And i > 0
                System.Threading.Thread.Sleep(10)
                i -= 1
            End While
            If i > 0 Then
                '待機時間内に終了した
                r = 1
            End If
        Catch ex As Exception
            '存在しないからエラーが出たOK
            r = 1
        End Try
        Return r
    End Function

    'プロセスを名前指定で停止
    Public Sub stopProcName(ByVal name As String)
        Dim p As New System.Diagnostics.Process
        Dim inst As Process
        Dim myProcess() As Process
        myProcess = System.Diagnostics.Process.GetProcessesByName(name)
        For Each inst In myProcess
            Try
                p = System.Diagnostics.Process.GetProcessById(inst.Id)
                p.Kill()
            Catch ex As Exception
                log1write(name & "の名前指定によるプロセス終了に失敗しました")
            End Try
        Next
    End Sub

    Private Function sendRecTaskMsg(msg As String, recTaskId As Integer) As String
        '
        '            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(
        '                ".", PREFIX_RECTASK_SERVER_PIPE + recTaskId,
        '                PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation))
        '            {
        '                pipeClient.Connect();
        '
        '                if (pipeClient.IsConnected)
        '                {
        '                    StreamString ss = new StreamString(pipeClient);
        '
        '                    pipeClient.WaitForPipeDrain();
        '                    int read = ss.WriteString(msg);
        '                    debug.print(read);
        '
        '                    pipeClient.WaitForPipeDrain();
        '                    string temp = ss.ReadString();
        '                    debug.print(temp);
        '
        '                    pipeClient.Close();
        '                    return temp;
        '                }
        '                else
        '                {
        '                    debug.print("not Connect");
        '                    return "";
        '                }
        '            }
        '

        Dim k32pm As New Kernel32PipeManager("" & recTaskId)
        k32pm.WriteString(msg)
        Dim temp As String = k32pm.ReadString()

        k32pm.Close()
        Return temp
    End Function

    Public Function getEmptyUdpPort(ByVal num As Integer) As Integer
        Dim udpPortNumber As Integer = 0

        SyncLock Me
            '
            '                for (int i = 1025; i < 65535; i++)
            '                {
            '                    //Processオブジェクトを作成
            '                    System.Diagnostics.Process p = new System.Diagnostics.Process();
            '
            '                    //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            '                    p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            '                    //出力を読み取れるようにする
            '                    p.StartInfo.UseShellExecute = false;
            '                    p.StartInfo.RedirectStandardOutput = true;
            '                    p.StartInfo.RedirectStandardInput = false;
            '                    //ウィンドウを表示しないようにする
            '                    p.StartInfo.CreateNoWindow = true;
            '                    //コマンドラインを指定（"/c"は実行後閉じるために必要）
            '                    p.StartInfo.Arguments = "/c netstat -p udp | Find \"" + i + "\"";
            '
            '                    //起動
            '                    p.Start();
            '
            '                    //出力を読み取る
            '                    string results = p.StandardOutput.ReadToEnd();
            '
            '                    //プロセス終了まで待機する
            '                    //WaitForExitはReadToEndの後である必要がある
            '                    //(親プロセス、子プロセスでブロック防止のため)
            '                    p.WaitForExit();
            '                    p.Close();
            '
            '                    //ポート番号が見つからなかったらそれを使用する
            '                    if (results.Equals(string.Empty))
            '                    {
            '                        udpPortNumber = i;
            '                        break;
            '                    }
            '                }
            '

            'udpPortNumber = Val(Me._udpPort.ToString) + Me._updCount
            'Me._updCount += 1

            '延々と増えていく可能性があるので＋numにした
            udpPortNumber = Val(Me._udpPort.ToString) + num
        End SyncLock

        If udpPortNumber = 0 OrElse udpPortNumber > 65535 Then
            Throw New SystemException("使用できるUDPポートがありませんでした。")
        End If

        log1write("UDPポート=" & udpPortNumber & "を取得しました")

        Return udpPortNumber
    End Function

    'VLCのcrashダイアログが表示されていれば消す
    Public Sub check_crash_dialog()
        Dim h As IntPtr
        Dim w As RECT

        h = FindWindow(vbNullString, "VLC crash reporting")
        If GetWindowRect(h, w) = 1 Then
            '最前面にする
            SetForegroundWindow(h)
            F_sendkeycode(&H9) 'VK_TAB = &H9 'TAB key
            System.Threading.Thread.Sleep(30)
            F_sendkeycode(&HD) 'VK_RETURN = &HD 'ENTER key
            log1write("VLC crash reportingウィンドウを消しました。")
        End If
    End Sub

    'TCPコマンドを送ってVLCを終了する
    Public Function quit_VLC(ByVal i As Integer) As Integer
        Return Me._list(i).vlc_quit_VLC()
    End Function

    'wwwrootにあるmystream[num]～というファイルを削除する
    Public Sub delete_mystreamnum(ByVal num As Integer)
        For Each tempFile As String In System.IO.Directory.GetFiles(Me._fileroot)
            If tempFile.IndexOf("mystream" & num.ToString & "-") >= 0 Or tempFile.IndexOf("mystream" & num.ToString & ".") >= 0 Then
                Dim i As Integer = 40 '2秒間は再チャレンジする
                While deletefile(tempFile) = 0 And i >= 0
                    System.Threading.Thread.Sleep(50)
                    i -= 1
                End While
            End If
        Next
    End Sub

    '.m3u8を.xspfに変換してみる ■未使用
    Public Sub convert_m3u8_xspf()
        For Each tempFile As String In System.IO.Directory.GetFiles(Me._wwwroot)
            If tempFile.IndexOf("mystream") >= 0 And tempFile.IndexOf(".m3u8") > 0 Then
                Dim s1 As String = file2str(tempFile, "UTF-8")
                Dim line() As String = s1.Split(vbCrLf)
                Dim s2 As String = ""
                Dim s3 As String = file2str(tempFile.Replace(".m3u8", ".xspf"), "UTF-8")

                For i As Integer = 0 To line.Length - 1
                    If line(i).IndexOf("mystream") >= 0 And line(i).IndexOf(".ts") >= 0 Then
                        s2 &= "    <track>" & vbLf
                        s2 &= "      <location>http://127.0.0.1:40003/" & line(i).Substring(1) & "</location>" & vbLf
                        s2 &= "    </track>" & vbLf
                        s2 &= "    <title>" & line(i).Substring(1) & "</title>" & vbLf
                    End If
                Next

                If s2.Length > 0 Then
                    s2 = "  <trackList>" & vbLf & s2
                    s2 = "  <title>Playlist</title>" & vbLf & s2
                    s2 = "<location>http://127.0.0.1:40003/mystream1.xspf</location>" & vbLf & s2
                    s2 = "<playlist version=""1"" xmlns=""http://xspf.org/ns/0/"">" & vbLf & s2
                    s2 = "<?xml version=""1.0"" encoding=""UTF-8""?>" & vbLf & s2

                    s2 &= "  </trackList>" & vbLf
                    s2 &= "</playlist>" & vbLf
                End If
                If s2 <> s3 Then
                    str2file(tempFile.Replace(".m3u8", ".xspf"), s2, "UTF-8")
                End If
            End If
        Next
    End Sub

    'numからbon_driverのパスと名前を取得
    Public Function get_bondriver_name(ByVal num As Integer) As String
        Dim r As String = ""
        Dim udpopt As String = ""
        '現在のlist(i)
        Dim js As String = ""
        For j As Integer = 0 To Me._list.Count - 1
            If Me._list(j)._num = num Then
                Dim s As String = Me._list(j)._udpOpt
                Dim sp As Integer = s.ToLower.LastIndexOf("bondriver")
                Dim ep As Integer = s.IndexOf(".dll")
                Dim bon As String = ""
                If sp >= 0 And ep > sp Then
                    bon = s.Substring(sp, (ep - sp) + ".dll".Length)
                    r = bon
                End If
            End If
        Next

        Return r
    End Function

    'numからbon_driverのパスと名前を取得
    Public Function get_resolution(ByVal num As Integer) As String
        Dim r As String = ""
        '現在のlist(i)
        Dim js As String = ""
        For j As Integer = 0 To Me._list.Count - 1
            If Me._list(j)._num = num Then
                r = Me._list(j)._resolution
            End If
        Next

        Return r
    End Function

    Public Sub delete_old_TS(ByVal fileroot As String)
        'Dim files As String() = System.IO.Directory.GetFiles(tsroot, "*.ts", System.IO.SearchOption.AllDirectories)
        Dim files As String() = Nothing
        Try
            files = System.IO.Directory.GetFiles(fileroot, "*.m3u8")
        Catch ex As Exception
            log1write("m3u8一覧の取得に失敗しました。" & fileroot & " " & ex.Message)
            Exit Sub
        End Try
        For Each tempFile As String In files
            Dim n As Integer = 0
            If tempFile.IndexOf("mystream") >= 0 Then
                Try
                    n = Val(tempFile.Substring(tempFile.IndexOf("mystream") + "mystream".Length)) 'スレッドナンバー
                Catch ex As Exception
                    n = 0
                End Try

                'nからlist(i)を求める
                Dim stream_mode As Integer = 0
                If Me._list.Count > 0 Then
                    Dim j As Integer = -1
                    For i As Integer = 0 To Me._list.Count - 1
                        If Me._list(i)._num = n Then
                            j = i
                        End If
                    Next
                    If j >= 0 Then
                        'ファイル再生なら1が入る
                        stream_mode = Me._list(j)._stream_mode
                    End If
                End If

                If n > 0 And stream_mode = 0 Then
                    Dim s As String = ReadAllTexts(tempFile)
                    'm3u8に書かれている最初のファイル
                    Dim m As Integer = -1
                    Dim sp As Integer = s.IndexOf("mystream" & n.ToString & "-")
                    Try
                        m = Val(s.Substring(sp + ("mystream" & n.ToString & "-").Length))
                    Catch ex As Exception
                        m = -1
                    End Try
                    m -= 10 '再生中の可能性もあるのでm3u8に書かれている最初のtsの10個前まで残す
                    'm3u8に書かれている最後のファイル
                    Dim mend As Integer = 999999999
                    sp = s.LastIndexOf("mystream" & n.ToString & "-")
                    Try
                        mend = Val(s.Substring(sp + ("mystream" & n.ToString & "-").Length))
                    Catch ex As Exception
                        mend = -1
                    End Try
                    mend += 5 '処理中に新たなファイルが作成される可能性があるのでm3u8に書かれている最終tsの5つ後まで残す
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
                            If (m2 >= 0 And m2 < m) Or (m2 > mend) Then
                                '古いものと、なぜか残っている未来のものを消す
                                deletefile(tsFile)
                            End If
                        Next
                    End If
                End If
            End If
        Next
    End Sub

    'numから放送中のNHK音声モードを取得する
    Public Function get_NHKmode(ByVal num As Integer) As Integer
        Dim NHKmode As Integer = -1
        Dim i As Integer = num2i(num)
        If i >= 0 Then
            Dim hlsOpt As String = Me._list(i)._hlsOpt
            NHKmode = Me._list(i)._NHK_dual_mono_mode_select
        End If
        Return NHKmode
    End Function

    '===========================================================
    'WEB インターフェース
    '===========================================================

    '配信中の番組を返す
    Public Function WI_GET_PROGRAM_NUM(ByVal num As Integer) As String
        Dim r As String = ""

        Dim i As Integer = 0
        If Me._list.Count > 0 Then
            For i = 0 To Me._list.Count - 1

                Dim fullpathfilename As String = ""
                Dim program_sid As String = ""

                If Me._list(i)._num = num Or num = 0 Then
                    Dim stream_mode As Integer = Me._list(i)._stream_mode
                    If stream_mode = 0 Or stream_mode = 2 Then
                        Dim chk As Integer = 0
                        'テレビ配信
                        program_sid = Val(Instr_pickup(Me._list(i)._udpOpt, "/sid ", " ", 0))
                        Dim str As String = ""
                        'まずはBS・CSで番組を探す
                        If TvProgram_tvrock_url.Length > 0 Then
                            str = program_translate4WI(999)
                        ElseIf TvProgram_EDCB_url.Length > 0 Then
                            str = program_translate4WI(998)
                        ElseIf ptTimer_path.Length > 0 Then
                            str = program_translate4WI(997)
                        ElseIf Tvmaid_url.Length > 0 Then
                            str = program_translate4WI(996)
                        End If
                        If str.Length > 0 Then
                            Dim d() As String = Split(str, vbCrLf)
                            For j As Integer = 0 To d.Length - 1
                                Dim e() As String = d(j).Split(",")
                                If e.Length >= 8 Then
                                    If Val(e(2)) = program_sid Then
                                        r &= Me._list(i)._num.ToString & "," & d(j) & ",0" & vbCrLf
                                        chk = 1
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                        If chk = 0 Then
                            'ネット番組表
                            str = program_translate4WI(0)
                            If str.Length > 0 Then
                                Dim d() As String = Split(str, vbCrLf)
                                For j As Integer = 0 To d.Length - 1
                                    Dim e() As String = d(j).Split(",")
                                    If e.Length >= 8 Then
                                        If Val(e(2)) = program_sid Then
                                            r &= Me._list(i)._num.ToString & "," & d(j) & ",0" & vbCrLf
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    ElseIf stream_mode = 1 Or stream_mode = 3 Then
                        'ファイル再生
                        '_VideoSeekSeconds
                        Dim VideoSeekSeconds As Integer = Me._list(i)._VideoSeekSeconds
                        Dim videoSeekSeconds_str As String = seconds2jifun(VideoSeekSeconds)
                        Dim hms As String = ""
                        fullpathfilename = Me._list(i)._fullpathfilename
                        r &= Me._list(i)._num.ToString & "," & "ファイル再生,ファイル再生,0,0," & videoSeekSeconds_str & ",00:00," & fullpathfilename & ",," & VideoSeekSeconds.ToString & vbCrLf
                    End If
                End If
            Next
        End If
        Return r
    End Function

    '秒を0:00:00形式に変換
    Public Function seconds2jifun(ByVal ss As Integer) As String
        Dim r As String = ""
        Dim h As Integer = Int(ss / (60 * 60))
        Dim m As Integer = Int((ss - (h * (60 * 60))) / 60)
        Dim s As Integer = ss Mod 60
        If h > 0 Then
            r &= h.ToString & ":"
        End If
        r &= m.ToString("D2") & ":" & s.ToString("D2")
        Return r
    End Function

    '配信可能なチャンネル情報
    Public Function WI_GET_CHANNELS(ByVal BonDriverPath As String, ByVal udpApp As String, ByVal BonDriver_NGword() As String) As String
        'BonDriver
        'ServiceID, ch_space, チャンネル名
        Dim r As String = ""

        Dim bons() As String = Nothing
        Dim bons_n As Integer = 0

        '初めの1回　まだhtmlができていない
        Dim bondriver_path As String = BonDriverPath.ToString
        If bondriver_path.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bondriver_path = filepath2path(udpApp.ToString)
        End If
        Try
            For Each stFilePath As String In System.IO.Directory.GetFiles(bondriver_path, "*.dll")
                If System.IO.Path.GetExtension(stFilePath) = ".dll" Then
                    Dim s As String = stFilePath
                    'フルパスファイル名がsに入る
                    If s.IndexOf("\") >= 0 Then
                        'ファイル名だけを取り出す
                        Dim k As Integer = s.LastIndexOf("\")
                        s = trim8(s.Substring(k + 1))
                    End If
                    Dim sl As String = s.ToLower() '小文字に変換
                    '表示しないBonDriverかをチェック
                    If BonDriver_NGword IsNot Nothing Then
                        For j As Integer = 0 To BonDriver_NGword.Length - 1
                            If sl.IndexOf(BonDriver_NGword(j)) >= 0 Then
                                sl = ""
                            End If
                        Next
                    End If
                    If sl.IndexOf("bondriver") = 0 Then
                        'セレクトボックス用にBonDriverを記録しておく
                        ReDim Preserve bons(bons_n)
                        bons(bons_n) = sl
                        bons_n += 1
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

        Dim i As Integer = 0
        If bons IsNot Nothing Then
            If bons.Length > 0 Then
                'BonDriver＆チャンネル一覧
                For i = 0 To bons.Length - 1
                    r &= "[" & bons(i) & "]" & vbCrLf
                    '局名を書き込む
                    r &= list_channel(bondriver_path, bons(i), 0)
                Next
            End If
        End If

        Return r
    End Function

    Public Function list_channel(ByVal bondriver_path As String, ByVal bondriver As String, ByVal a As Integer) As String
        Dim ichiran As String = ""
        If bondriver.Length > 0 Then
            Dim k As Integer = -1
            If BonDriver_select_html IsNot Nothing Then
                If BonDriver_select_html.Length > 0 Then
                    k = Array.IndexOf(BonDriver_select_html, bondriver)
                End If
            End If
            If k >= 0 Then
                ichiran = BonDriver_select_html(k).ichiran
            Else
                'サービスIDと放送局名用
                Dim si As Integer = 0
                If ch_list IsNot Nothing Then
                    si = ch_list.Length
                End If

                Dim filename As String
                If bondriver_path.Length > 0 Then
                    filename = bondriver_path & "\" & bondriver.Replace(".dll", ".ch2")
                Else
                    filename = bondriver.Replace(".dll", ".ch2")
                End If
                Dim line() As String = file2line(filename)
                If line IsNot Nothing Then
                    For i As Integer = 0 To line.Length - 1
                        If line(i).IndexOf(";") < 0 Then
                            Dim s() As String = line(i).Split(",")
                            If s.Length = 9 Then
                                If IsNumeric(s(1)) And IsNumeric(s(5)) And IsNumeric(s(7)) Then 'サービスID,TSIDが数値なら
                                    If Val(s(8)) > 0 Then
                                        ichiran &= bondriver & "," & s(5) & "," & s(1) & "," & s(0) & vbCrLf

                                        'serviceIDと放送局名を記録しておく
                                        If ch_list IsNot Nothing Then
                                            Dim chk As Integer = 0
                                            For i2 As Integer = 0 To ch_list.Length - 1
                                                If ch_list(i2).sid = Val(s(5)) And ch_list(i2).tsid = Val(s(7)) Then
                                                    'サービスIDとTSIDが一致した
                                                    'すでに登録済み
                                                    chk = 1
                                                    Exit For
                                                End If
                                            Next
                                            If chk = 0 Then
                                                'まだ登録されていなければ
                                                ReDim Preserve ch_list(si)
                                                ch_list(si).sid = Val(s(5))
                                                ch_list(si).jigyousha = s(0)
                                                ch_list(si).bondriver = bondriver
                                                ch_list(si).chspace = Val(s(1))
                                                ch_list(si).channel = Val(s(2))
                                                ch_list(si).tsid = Val(s(7))
                                                ch_list(si).nid = Val(s(6))
                                                si += 1
                                            End If
                                        Else
                                            '最初の１つめ
                                            ReDim Preserve ch_list(0)
                                            ch_list(0).sid = Val(s(5))
                                            ch_list(0).jigyousha = s(0)
                                            ch_list(0).bondriver = bondriver
                                            ch_list(0).chspace = Val(s(1))
                                            ch_list(0).channel = Val(s(2))
                                            ch_list(0).tsid = Val(s(7))
                                            ch_list(0).nid = Val(s(6))
                                            si += 1
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        End If

        Return ichiran
    End Function

    '現在配信中のストリーム
    Public Function WI_GET_LIVE_STREAM() As String
        '_listNo.,num, udpPort, BonDriver, ServiceID, ch_space, stream_mode, NHKMODE
        'stopping, チャンネル名, hlsApp, シーク秒
        Dim r As String = ""

        Dim d() As Integer = get_live_index_sort() 'listナンバーがnumでソートされて返ってくる
        If d IsNot Nothing Then
            For i As Integer = 0 To d.Length - 1
                Dim ListNo As Integer = d(i)
                Dim num As Integer = Me._list(d(i))._num
                Dim udpOpt As String = Me._list(d(i))._udpOpt
                Dim BonDriver As String = Trim(instr_pickup_para(udpOpt, "/d ", " ", 0))
                Dim ServiceID As String = Trim(instr_pickup_para(udpOpt, "/sid ", " ", 0))
                Dim ch_space As String = Trim(instr_pickup_para(udpOpt, "/chspace ", " ", 0))
                Dim udpPort As String = Trim(instr_pickup_para(udpOpt, "/port ", " ", 0))
                Dim stream_mode As Integer = Me._list(d(i))._stream_mode
                Dim NHKMODE As Integer = Me._list(d(i))._NHK_dual_mono_mode_select
                Dim stopping As Integer = Me._list(d(i))._stopping
                Dim channel_name As String = F_sid2channelname(Val(ServiceID), Val(ch_space))
                Dim hlsApp As String = Me._list(d(i))._hlsApp
                Dim hlsApp_name As String = ""
                Dim fullpathfilename As String = Me._list(d(i))._fullpathfilename
                Dim VideoSeekSeconds As Integer = Me._list(d(i))._VideoSeekSeconds
                If channel_name.Length = 0 And (stream_mode = 1 Or stream_mode = 3) Then
                    'ファイル再生ならばファイル名をBonDriverとして表示するようにする
                    BonDriver = fullpathfilename
                End If
                Dim sp As Integer = hlsApp.LastIndexOf("\")
                If sp >= 0 Then
                    'ファイル名だけを抜き出す()
                    Try
                        hlsApp_name = hlsApp.Substring(sp + 1)
                    Catch ex As Exception
                    End Try
                Else
                    hlsApp_name = hlsApp
                End If
                '最後に文字列に追加
                Dim sep As String = ", "
                r &= ListNo.ToString _
                    & sep & num.ToString _
                    & sep & udpPort _
                    & sep & BonDriver _
                    & sep & ServiceID _
                    & sep & ch_space _
                    & sep & stream_mode _
                    & sep & NHKMODE _
                    & sep & stopping _
                    & sep & channel_name _
                    & sep & hlsApp_name _
                    & sep & VideoSeekSeconds _
                    & vbCrLf
            Next
        End If

        Return r
    End Function

    '再起動が起こっているnumを返す
    Public Function WI_GET_ERROR_STREAM() As String
        Dim r As String = ":"
        Dim i As Integer = 0
        If Me._list.Count > 0 Then
            For i = 0 To Me._list.Count - 1
                If Me._list(i)._chk_proc > 0 Then
                    'エラーがあればnumを返す
                    r &= Me._list(i)._num.ToString & ":"
                End If
            Next
        End If
        Return r
    End Function

    'ニコニコ実況用　numからサービスIDとchspaceを取得して返す
    Public Function get_jk_para(ByVal num As Integer) As Object
        Dim d(1) As Integer
        d(0) = 0
        d(1) = 0

        Dim udpopt As String = ""
        Dim i As Integer = 0
        If Me._list.Count > 0 Then
            For i = 0 To Me._list.Count - 1
                If Me._list(i)._num = num Then
                    udpopt = Me._list(i)._udpOpt
                    Exit For
                End If
            Next
        End If

        If udpopt.Length > 0 Then
            d(0) = Val(instr_pickup_para(udpopt, "/sid ", " ", 0))
            d(1) = Val(instr_pickup_para(udpopt, "/chspace ", " ", 0))
        End If

        Return d
    End Function
End Class


