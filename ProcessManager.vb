Imports System.Collections.Generic
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
        Dim r As Integer = 0
        For i As Integer = Me._list.Count - 1 To 0 Step -1
            If Me._list(i)._num = num Then
                If Me._list(i)._stopping = 1 Then
                    r = 1
                End If
            End If
        Next
        Return r
    End Function

    Public Sub startProc(udpApp As String, udpOpt As String, hlsApp As String, hlsOpt As String, num As Integer, udpPort As Integer, ShowConsole As Integer)
        '★起動している場合は既存のプロセスを止める
        stopProc(num)

        If get_stopping_status(num) = 1 Then
            '停止段階で失敗している
            log1write("No.=" & num & "のプロセスは使用できません。前回使用時の停止に失敗したようです")
        Else
            '関連するファイルを削除
            delete_mystreamnum(num)

            'If Me._list.Count < Me._maxSize Then

            Dim pipeListBefore As New List(Of Integer)()
            If Path.GetFileName(udpApp).Equals("RecTask.exe") Then
                '★実行されている名前付きパイプのリストを取得する(プロセス実行前)
                Dim listOfPipes As String() = System.IO.Directory.GetFiles("\\.\pipe\")
                For Each pipeName As String In listOfPipes
                    If pipeName.Contains("RecTask_Server_Pipe_") Then
                        Dim pindex1 As Integer = 0
                        Try
                            pindex1 = Val(pipeName.Substring(pipeName.IndexOf("RecTask_Server_Pipe_") + 20))
                        Catch ex As Exception
                        End Try
                        If pindex1 > 0 Then
                            pipeListBefore.Add(pindex1)
                            'log1write("Before PipeName=" & pipeName & " PipeIndex=" & pindex1)
                        End If
                    End If
                Next
            End If

            '★UDPソフトを実行
            'ProcessStartInfoオブジェクトを作成する
            Dim udpPsi As New System.Diagnostics.ProcessStartInfo()
            '起動するファイルのパスを指定する
            udpPsi.FileName = udpApp
            'コマンドライン引数を指定する
            udpPsi.Arguments = udpOpt
            'アプリケーションを起動する
            Dim udpProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(udpPsi)
            udpProc.PriorityClass = System.Diagnostics.ProcessPriorityClass.High
            log1write("No.=" & num & "のUDPアプリを起動しました。handle=" & udpProc.Handle.ToString)

            'pipeindexを取得
            Dim pipeIndex As Integer = 0
            Dim chk As Integer = 0
            While chk < 200
                'RecTaskのパイプが増加するまで繰り返す
                If Path.GetFileName(udpApp).Equals("RecTask.exe") Then
                    '★実行されている名前付きパイプのリストを取得する(プロセス実行後)
                    Dim listOfPipes As String() = System.IO.Directory.GetFiles("\\.\pipe\")
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

            If pipeIndex > 0 Then
                log1write("No.=" & num & "のパイプインデックスを取得しました。pipeindex=" & pipeIndex.ToString)

                System.Threading.Thread.Sleep(1000)

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
                'アプリケーションを起動する
                Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)

                log1write("No.=" & num & "のHLSアプリを起動しました。handle=" & hlsProc.Handle.ToString)

                'Dim pb As New ProcessBean(udpProc, hlsProc, num, pipeIndex)'↓再起動用にパラメーターを渡しておく
                Dim pb As New ProcessBean(udpProc, hlsProc, num, pipeIndex, udpApp, udpOpt, hlsApp, hlsOpt, udpPort, ShowConsole)
                Me._list.Add(pb)
            Else
                Try
                    'UDPアプリ終了　pipeindexがわからないので強制終了
                    udpProc.Kill()
                    udpProc.Dispose()
                    udpProc.Close()
                Catch ex As Exception
                End Try
                log1write("No.=" & num & "のpipeindexの取得に失敗しました")
            End If
            'End If
        End If

        '現在稼働中のlist(i)._numをログに表示
        Dim js As String = get_live_numbers()
        log1write("現在稼働中のNumber：" & js)

    End Sub

    'プロセスが順調に動いているかチェック
    Public Sub checkAllProc()
        For i As Integer = Me._list.Count - 1 To 0 Step -1
            If Me._list(i)._num > 0 And Me._list(i)._stopping = 0 Then
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
                    'プロセスを停止
                    stopProc(p5)
                    'System.Threading.Thread.Sleep(500)
                    'プロセスを開始
                    startProc(p1, p2, p3, p4, p5, p6, p7)
                    log1write("No.=" & p5 & "のプロセスを再起動しました")
                End If
            End If
        Next
    End Sub

    '指定numberプロセスを停止する
    Public Sub stopProc(num As Integer)
        'index = -1 の場合は全て停止
        Dim proc As System.Diagnostics.Process = Nothing

        Dim hls_stop As Integer = 0 '上手く停止したら1
        Dim udp_stop As Integer = 0 '上手く停止したら1

        For i As Integer = Me._list.Count - 1 To 0 Step -1
            If Me._list(i)._num = num Or num <= -1 Then

                log1write("No.=" & num & "のプロセスを停止しています")

                '停止フラグを書き込んでおいたほうがいいかな
                Me._list(i)._stopping = 1 'checkAllProc()でチェックさせないため・・

                '★ HLS
                proc = Me._list(i).GetHlsProc()
                If proc IsNot Nothing AndAlso Not proc.HasExited Then
                    If Me._list(i)._hlsApp.IndexOf("vlc") >= 0 Then
                        'VLCならば
                        If quit_VLC(i) = 1 Then
                            'プロセスが無くなるまで待機
                            If wait_stop_proc(proc) = 1 Then
                                log1write("No.=" & num & "のVLCアプリを終了しました")
                                hls_stop = 1
                            Else
                                log1write("No.=" & num & "のVLCアプリ終了に失敗しました")
                            End If
                            proc.Close()
                            proc.Dispose()
                        Else
                            '強制的に終了
                            'proc.CloseMainWindow()
                            proc.Kill()
                            If wait_stop_proc(proc) = 1 Then
                                log1write("No.=" & num & "のVLCアプリを強制終了しました")
                                hls_stop = 1
                            Else
                                log1write("No.=" & num & "のVLCアプリ強制終了に失敗しました")
                            End If
                            'proc.WaitForExit()
                            proc.Close()
                            proc.Dispose()
                            'System.Threading.Thread.Sleep(1000)
                        End If
                    ElseIf Me._list(i)._hlsApp.IndexOf("ffmpeg") >= 0 Then
                        'ffmpeg
                        'コンソールが表示されてないときにも有効かどうかわからないのでsendkeyは却下
                        'AppActivate(proc.Id) '最前面にする
                        'SendKeys.Send("q") 'qを送る
                        'F_sendkeycode(&HD) 'エンターキー
                        'proc.CloseMainWindow()
                        'proc.WaitForExit()
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
                        ''System.Threading.Thread.Sleep(1000)
                    End If
                Else
                    'プロセスが無い
                    hls_stop = 1
                End If

                '★ UDP
                proc = Me._list(i).GetUdpProc()
                If proc IsNot Nothing AndAlso Not proc.HasExited Then
                    Dim udpPipeIndex As Integer = Me._list(i).GetProcUdpPipeIndex
                    If udpPipeIndex > 0 Then
                        Dim rr As String = sendRecTaskMsg("EndTask", udpPipeIndex)

                        'UDPソフトが終了するまで待つ
                        '★実行されている名前付きパイプのリストを取得する(プロセス実行前)
                        Dim chk As Integer = 1000
                        While chk > 0
                            Dim listOfPipes As String() = System.IO.Directory.GetFiles("\\.\pipe\")
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
                        If chk >= 0 Then
                            log1write("No.=" & num & "のUDPアプリを終了しました")
                        Else
                            'タイムアウト
                            log1write("No.=" & num & "のUDPアプリの終了に失敗しました")
                        End If
                        '最後にプロセスチェック
                        If wait_stop_proc(proc) = 1 Then
                            'log1write("No.=" & num & "のudpアプリを強制終了しました")
                            udp_stop = 1
                        Else
                            log1write("No.=" & num & "のudpアプリの終了に失敗したようです。")
                        End If
                        proc.Close()
                        proc.Dispose()
                    Else
                        'パイプが見当たらない
                        '強制的に終了
                        'proc.CloseMainWindow()
                        proc.Kill()
                        'proc.WaitForExit()
                        If wait_stop_proc(proc) = 1 Then
                            log1write("No.=" & num & "のudpアプリを強制終了しました")
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

                '★ リストから取り除く
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

                'System.Threading.Thread.Sleep(1000)

            End If
        Next

        If num = -2 Then
            log1write("関連アプリのプロセスを停止しています")
            stopProcName("vlc")
            stopProcName("RecTask")
            stopProcName("ffmpeg")
            log1write("関連アプリのプロセスを停止しました")
        End If

        '現在稼働中のlist(i)._numをログに表示
        Dim js As String = get_live_numbers()
        log1write("現在稼働中のNumber：" & js)
    End Sub

    '現在稼働中のlist(i)._numを取得
    Public Function get_live_numbers() As String
        '現在のlist(i)
        Dim js As String = " "
        For j As Integer = 0 To Me._list.Count - 1
            If Me._list(j)._stopping = 1 Then
                js &= "x" & Me._list(j)._num.ToString & " "
            Else
                js &= Me._list(j)._num.ToString & " "
            End If
        Next
        Return js
    End Function

    '現在稼働中のlist(i)に関するBonDriver情報を併せて取得
    Public Function get_live_numbers_bon() As String
        '現在のlist(i)
        Dim js As String = ""
        For j As Integer = 0 To Me._list.Count - 1
            If Me._list(j)._stopping = 1 Then
                js &= "x"
            End If
            Dim s As String = Me._list(j)._udpOpt
            Dim sp As Integer = s.ToLower.IndexOf("bondriver")
            Dim ep As Integer = s.IndexOf(".dll")
            Dim bon As String = ""
            If sp >= 0 And ep > sp Then
                bon = s.Substring(sp, (ep - sp) + ".dll".Length)
            End If
            js &= Me._list(j)._num.ToString & ": " & bon & vbCrLf
        Next
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
        '                    System.Console.WriteLine(read);
        '
        '                    pipeClient.WaitForPipeDrain();
        '                    string temp = ss.ReadString();
        '                    System.Console.WriteLine(temp);
        '
        '                    pipeClient.Close();
        '                    return temp;
        '                }
        '                else
        '                {
        '                    System.Console.WriteLine("not Connect");
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
        End If
    End Sub

    'TCPコマンドを送ってVLCを終了する
    Public Function quit_VLC(ByVal i As Integer) As Integer
        Dim r As Integer = 0
        Try

            Dim tcpPort As Integer = Me._list(i)._udpPort

            ' ソケット生成
            Dim objSck As New System.Net.Sockets.TcpClient
            Dim objStm As System.Net.Sockets.NetworkStream

            ' TCP/IP接続
            objSck.Connect("127.0.0.1", tcpPort)
            objStm = objSck.GetStream()

            ' TCP/IP接続待ち
            Do While objSck.Connected = False
                System.Threading.Thread.Sleep(500)
            Loop

            ' データ送信(文字列をByte配列に変換して送信)
            Dim sdat As Byte() = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes("quit")
            objStm.Write(sdat, 0, sdat.GetLength(0))

            ' TCP/IP切断
            objStm.Close()
            objSck.Close()

            r = 1 '成功
        Catch ex As Exception
            r = 0
        End Try

        Return r
    End Function

    'wwwrootにあるmystream[num]～というファイルを削除する
    Public Sub delete_mystreamnum(ByVal num As Integer)
        For Each tempFile As String In System.IO.Directory.GetFiles(Me._fileroot)
            If tempFile.IndexOf("mystream" & num.ToString & "-") >= 0 Or tempFile.IndexOf("mystream" & num.ToString & ".") >= 0 Then
                deletefile(tempFile)
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
                Dim sp As Integer = s.ToLower.IndexOf("bondriver")
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
End Class


