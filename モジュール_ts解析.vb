Imports System.IO
Imports System.Text

Module モジュール_ts解析
    Public Structure tot_structure
        Public fullpathfilename As String '録画ファイル名
        Public start_time As DateTime '開始日時
        Public start_utime As Integer '開始日時 unixtime
        Public err As Integer 'エラー番号
        Public errstr As String 'エラーメッセージ
        Public searchstr As String 'indexof用　fullpathfilename & ":" & start_utime
        Public duration As Integer '動画の長さ
        Public video_fps As Double 'ts以外の場合のフレームレート
        Public sid As Integer 'サービスID
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.searchstr = pF Then '録画ファイル名&":"&開始日時unixtimeと一致するか
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    Public TOT_cache_max As Integer = 100 '最大何個キャッシュするか
    Public TOT_cache(TOT_cache_max) As tot_structure
    Public TOT_cache_index As Integer = 0

    Public TOT_get_duration As Integer = 1 '動画の長さを調べるか1=必ず調べる　2=キャッシュからのみ調べる

    Public Function get_TOT(ByVal fullpathfilename As String, ByVal ffmpeg_path As String, Optional ByRef video_fps As Double = 0) As DateTime
        'オプションとしてByRefでフレームレートも返す
        'ffmpeg_pathはmp4等の開始時間と動画の長さを調べるときのみに使用（exepath_ffmpegが指定されていればなんでもいい）
        Dim r As DateTime = C_DAY2038

        Dim f As tot_structure = TOT_read(fullpathfilename, ffmpeg_path)

        r = f.start_time
        video_fps = f.video_fps

        Return r
    End Function

    Public Function TOT_read(ByVal fullpathfilename As String, ByVal ffmpeg_path As String) As tot_structure
        Dim r As tot_structure = Nothing
        Dim t2 As DateTime = C_DAY2038
        Dim t2_end As DateTime = CDate("1970/01/02")
        Try
            t2 = System.IO.File.GetCreationTime(fullpathfilename)
        Catch ex As Exception
        End Try
        Try
            t2_end = System.IO.File.GetLastWriteTime(fullpathfilename)
        Catch ex As Exception
        End Try
        '普通はありえないが、ファイル名変換等によってファイルが存在しないとおかしな値が返ってくる　例：t2 = 1601/01/01 9:00:00
        Dim searchstr As String = fullpathfilename & ":" & time2unix(t2)
        Dim i As Integer = Array.IndexOf(TOT_cache, searchstr)
        If i >= 0 Then
            'キャッシュに該当有り
            r.fullpathfilename = TOT_cache(i).fullpathfilename
            r.start_time = TOT_cache(i).start_time
            r.start_utime = TOT_cache(i).start_utime
            r.err = TOT_cache(i).err
            r.errstr = TOT_cache(i).errstr
            r.searchstr = TOT_cache(i).searchstr
            r.duration = TOT_cache(i).duration
            r.video_fps = TOT_cache(i).video_fps
            r.sid = TOT_cache(i).sid
            log1write(fullpathfilename & "の開始時間をキャッシュから取得しました")
        Else
            '新規登録
            r = F_ts2tot(fullpathfilename, t2, t2_end, ffmpeg_path)
            If r.err = 0 Then
                TOT_cache_index += 1
                If TOT_cache_index > TOT_cache_max Then
                    TOT_cache_index = 0
                End If
                TOT_cache(TOT_cache_index).fullpathfilename = fullpathfilename
                TOT_cache(TOT_cache_index).start_time = r.start_time
                TOT_cache(TOT_cache_index).start_utime = r.start_utime
                TOT_cache(TOT_cache_index).err = r.err
                TOT_cache(TOT_cache_index).errstr = r.errstr
                TOT_cache(TOT_cache_index).searchstr = searchstr
                TOT_cache(TOT_cache_index).duration = r.duration
                TOT_cache(TOT_cache_index).video_fps = r.video_fps
                TOT_cache(TOT_cache_index).sid = r.sid
                log1write(fullpathfilename & "の開始時間をTOTと作成日時から取得しました")
            End If
        End If

        Return r
    End Function

    'tsファイルから開始時刻を取得する t2は予備としてファイル作成日を指定
    Public Function F_ts2tot(ByVal fullpathfilename As String, ByVal t2 As DateTime, ByVal t2_end As DateTime, ByVal ffmpeg_path As String) As tot_structure
        Dim r As tot_structure = Nothing

        Dim ext As String = Path.GetExtension(fullpathfilename)
        If ext = ".ts" Then
            'TSの場合
            Try
                Dim print_debug As Integer = -1 '結果を　0=ダンプしない 1=ダンプする -1=最終結果も表示しない

                Dim t0 As DateTime = CDate("1858/11/17")
                'フFァイルを開く
                Dim fs As New System.IO.FileStream(fullpathfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read)
                'ファイルを一時的に読み込むバイト型配列を作成する 
                Dim bs As Byte() = New Byte(187) {}

                Dim tstart As DateTime = C_DAY2038

                Dim t_chk As Integer = 0

                Dim err As Integer = 0
                Dim errstr As String = ""

                Dim sid As Integer = 0
                Dim sid_count As Integer = 0

                Dim pcr_first As Long = -1 '最初に出てきたPCR
                Dim pcr_last As Long = -1 'TOT直前のPCR

                'ファイルを読み込む 
                Dim chk As Integer = 0
                While True
                    'ファイルの一部を読み込む 
                    Try
                        Dim readSize As Integer = fs.Read(bs, 0, bs.Length)
                    Catch ex As Exception
                        err = 6
                        errstr = fullpathfilename & "を読み込み中にエラーが発生しました。" & ex.Message
                        Exit While
                    End Try

                    If bs(0) <> Convert.ToInt32("47", 16) Then
                        err = 1
                        errstr = "ヘッダーエラー 0x47から始まっていません"
                        Exit While
                    End If
                    Dim t1 As Integer = 0
                    Dim tdate As DateTime = C_DAY2038
                    Dim payload_unit_start_indicator As Integer = (bs(1) And Convert.ToInt32("40", 16)) / Convert.ToInt32("40", 16)
                    Dim pid As Integer = (bs(1) And Convert.ToInt32("1F", 16)) * 256 + bs(2)
                    Dim pidx As String = Convert.ToString(pid, 16).PadLeft(2, "0"c)
                    Dim adaptation_field As Integer = (bs(3) And Convert.ToInt32("20", 16)) / Convert.ToInt32("20", 16)
                    Dim payload_field As Integer = (bs(3) And Convert.ToInt32("10", 16)) / Convert.ToInt32("10", 16)

                    'PCR取得
                    If adaptation_field = 1 And t_chk = 0 Then
                        If bs(5) And Convert.ToInt32("10", 16) Then
                            pcr_last = CType(bs(6), Long) * 33554432 + bs(7) * 131072 + bs(8) * 512 + bs(9) * 2 + ((bs(10) And 128) / 128)
                            If pcr_first < 0 Then
                                pcr_first = pcr_last
                            End If
                        End If
                    End If

                    Select Case pidx
                        Case "14"
                            'TOT
                            t1 = bs(8) * 256 + bs(9)
                            tdate = DateAdd(DateInterval.Day, t1, t0)
                            tdate = DateAdd(DateInterval.Hour, Val(Convert.ToString(bs(10), 16)), tdate)
                            tdate = DateAdd(DateInterval.Minute, Val(Convert.ToString(bs(11), 16)), tdate)
                            tdate = DateAdd(DateInterval.Second, Val(Convert.ToString(bs(12), 16)), tdate)

                            'tstartに記録
                            If tdate < tstart Then
                                tstart = tdate
                                t_chk += 1
                            End If
                        Case "00"
                            'PAT
                            If bs(4) = 0 And payload_unit_start_indicator = 1 Then
                                Dim stemp As String = (bs(17) * 256 + bs(18)).ToString
                                If Val(stemp) > 0 And (Val(stemp) < sid Or sid = 0) Then
                                    '一番小さいsidを記録
                                    sid = Val(stemp)
                                    sid_count += 1
                                End If
                            End If
                    End Select

                    chk += 1
                    If chk >= 300000 Or t_chk >= 1 Then
                        Exit While
                    End If
                End While

                If chk >= 300000 Then
                    err = 3
                    errstr = "必要なTOTパケットが見つかりませんでした"
                End If

                Dim tend As DateTime = CDate("1970/01/02")
                Dim t_chk2 As Integer = 0
                Dim k As Integer = 1 '１からスタート
                If err = 0 And TOT_get_duration > 0 Then
                    'ファイルの最後からTOTを探す
                    fs.Seek(0, IO.SeekOrigin.End) 'このほうがスピード早い
                    While k < 300000
                        Try
                            'ファイルの最後から
                            'fs.Seek(-188 * k, IO.SeekOrigin.End)　'こっちより
                            fs.Seek(-188 * 2, IO.SeekOrigin.Current) 'このほうがスピード早い
                            'ファイルの一部を読み込む 
                            Dim readSize2 As Integer = fs.Read(bs, 0, bs.Length)
                        Catch ex As Exception
                            err = 15
                            errstr = "[末尾から]" & ex.Message
                            Exit While
                        End Try

                        If bs(0) <> Convert.ToInt32("47", 16) Then
                            err = 11
                            errstr = "[末尾から]ヘッダーエラー0x47から始まっていません"
                            Exit While
                        End If
                        Dim tdate As DateTime = CDate("1970/01/01 09:00:00")
                        Dim pid As Integer = (bs(1) And Convert.ToInt32("1F", 16)) * 256 + bs(2)
                        Dim pidx As String = Convert.ToString(pid, 16).PadLeft(2, "0"c)

                        Dim c As Integer = 0
                        Select Case pidx
                            Case "14"
                                'TOT
                                If t_chk2 <= 1 Then
                                    c = 2
                                    Dim t1 As Integer = bs(8) * 256 + bs(9)
                                    tdate = DateAdd(DateInterval.Day, t1, t0)
                                    tdate = DateAdd(DateInterval.Hour, Val(Convert.ToString(bs(10), 16)), tdate)
                                    tdate = DateAdd(DateInterval.Minute, Val(Convert.ToString(bs(11), 16)), tdate)
                                    tdate = DateAdd(DateInterval.Second, Val(Convert.ToString(bs(12), 16)), tdate)

                                    'tstartに記録
                                    If tdate > tend Then
                                        tend = tdate
                                        t_chk2 += 1
                                    End If
                                End If
                        End Select

                        If t_chk2 > 0 Then
                            Exit While
                        End If
                        k += 1
                    End While

                    If k >= 300000 Then
                        err = 14
                        errstr = "[末尾から]必要なパケットが見つかりませんでした。(tot)"
                    End If
                End If

                '閉じる 
                fs.Close()

                If err = 0 Then
                    '秒数を調整

                    '始点
                    'PCR補正
                    If pcr_first = pcr_last Then
                        'PCRが1個以下しか見つかっていない場合はTOTそのままでＯＫ
                    Else
                        Dim pcr_delay As Long = pcr_last - pcr_first
                        If pcr_delay < 0 Then
                            pcr_delay = 8589934592 - pcr_last + pcr_first
                        End If
                        pcr_delay += 9000 '両端で最大0.2秒の誤差を考え平均値0.1秒足しておく
                        pcr_delay = Math.Ceiling(pcr_delay / 90000) '切り上げ
                        log1write("最初のPCR出現より" & pcr_delay.ToString & "秒経過しています")
                        log1write("最初のTOT" & tstart.ToString & "から" & pcr_delay & "秒遡ります")
                        tstart = DateAdd(DateInterval.Second, -CType(pcr_delay, Integer), tstart)
                    End If
                    'ファイル作成日時が1秒速ければそちらを採用する（誤差を考えて。早い分には団子防止で緩和されるので）
                    If time2unix(tstart) - time2unix(t2) = 1 Then
                        log1write("ファイル作成日時の方が1秒早いので誤差を考え作成日時を採用します")
                        tstart = t2
                    End If
                    log1write("動画の開始日時を" & tstart.ToString & "にセットしました")

                    '終点
                    Dim e_ajust As Integer = 60 - Second(tend)
                    If e_ajust < 10 Then
                        tend = DateAdd(DateInterval.Second, e_ajust, tend)
                    End If
                    'ファイル作成日時と比べ併せて間違いなければファイル作成日時のほうを優先
                    Try
                        If System.Math.Abs(time2unix(t2_end) - time2unix(tend)) <= 10 Then
                            '10秒以内のずれならばファイル作成日時に合わせる
                            tend = t2_end
                        End If
                    Catch ex As Exception
                    End Try
                Else
                    'ファイル作成時間を返す
                    Try
                        tstart = t2
                        tend = t2_end
                        log1write(fullpathfilename & "のTOTが取得できませんでした。作成日時を使用します")
                    Catch ex As Exception
                        tstart = C_DAY2038
                    End Try
                End If

                '結果を返す
                r.fullpathfilename = fullpathfilename
                r.start_time = tstart '一番はじめの記録時間
                r.start_utime = time2unix(tstart)
                If TOT_get_duration > 0 Then
                    r.duration = time2unix(tend) - r.start_utime
                    If r.duration < 150 Or r.duration > (60 * 60 * 10) Then
                        '2分半未満、または10時間以上ならおかしな値なので
                        r.duration = 0
                    End If
                Else
                    r.duration = 0
                End If
                r.sid = sid
                r.err = err
                r.errstr = errstr
            Catch ex As Exception
                log1write("TOT解析中にエラーが発生しました。" & ex.Message)
                r.start_time = unix2time(0)
                r.err = 9
            End Try
        Else
            'TS以外
            'ts以外の場合、ファイルの作成日時＆更新日時から推測することはできないため
            'ffprobe.exeから長さを取得する
            If TOT_get_duration > 0 Then
                'Dim duration As Integer = F_get_mp4_length_from_WhiteBrowserDB(fullpathfilename)
                Dim fps As Double = 0 '↓からByRefで取得
                Dim duration As Integer = F_get_mp4_length_from_ffprobe(fullpathfilename, ffmpeg_path, fps)
                If duration >= 150 And duration <= (60 * 60 * 10) Then
                    '正常に取得できていれば
                    'その他データと合わせて結果を返す
                    r.fullpathfilename = fullpathfilename
                    Try
                        r.start_time = System.IO.File.GetCreationTime(fullpathfilename)
                    Catch ex As Exception
                        r.start_time = C_DAY2038 'ダミーデータ
                    End Try
                    r.start_utime = time2unix(r.start_time)
                    r.duration = duration
                    r.video_fps = fps
                    r.err = 0
                    r.errstr = ""
                Else
                    r.start_time = unix2time(0)
                    r.err = 91
                End If
            Else
                r.start_time = unix2time(0)
                r.err = 92
            End If
        End If

        Return r
    End Function

    'mp4等の長さ（秒）ffprobe.exe使用 ついでにフレームレートをByRefで返す
    Public Function F_get_mp4_length_from_ffprobe(ByVal fullpathfilename As String, ByVal ffmpeg_path As String, ByRef fps As Double) As Integer
        Dim r As Integer = 0

        'exepath_ffmpegが指定されていれば（QSVEnc使用時）
        If exepath_ffmpeg.Length > 0 Then
            ffmpeg_path = exepath_ffmpeg
        End If

        If isMatch_HLS(ffmpeg_path, "ffmpeg") = 1 Then
            Try
                Dim f_path As String = Path.GetDirectoryName(ffmpeg_path)
                Dim ffprobe_path As String = f_path & "\ffprobe.exe"

                If file_exist(ffprobe_path) = 1 Then
                    'カレントディレクトリ変更
                    Try
                        Directory.SetCurrentDirectory(f_path) 'カレントディレクトリ変更
                    Catch ex As Exception
                        '設定しないうちにスタートしようとすると例外が起こる
                        Return r
                        Exit Function
                    End Try

                    Dim results As String = ""
                    Dim psi As New System.Diagnostics.ProcessStartInfo()

                    psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec") 'ComSpecのパスを取得する
                    psi.RedirectStandardInput = False '出力を読み取れるようにする
                    psi.RedirectStandardError = True
                    psi.UseShellExecute = False
                    psi.CreateNoWindow = True 'ウィンドウを表示しないようにする
                    'psi.StandardOutputEncoding = Encoding.UTF8

                    psi.Arguments = "/c ffprobe.exe """ & fullpathfilename & """"

                    Dim p As System.Diagnostics.Process
                    Try
                        p = System.Diagnostics.Process.Start(psi)
                        '出力を読み取る
                        results = p.StandardError.ReadToEnd
                        'WaitForExitはReadToEndの後である必要がある
                        '(親プロセス、子プロセスでブロック防止のため)
                        p.WaitForExit(10000)
                    Catch ex As Exception
                        log1write("【エラー】pprobe.exeの実行に失敗しました")
                    End Try

                    If results.IndexOf("Duration:") > 0 Then
                        Dim jifunbyou As String = Trim(Instr_pickup(results, "Duration:", ".", 0))
                        Dim d() As String = jifunbyou.Split(":")
                        If d.Length = 3 Then
                            r = Val(d(0)) * (60 * 60) + Val(d(1)) * 60 + Val(d(2))
                        Else
                            log1write("【エラー】ffprobeによるDuration形式が不正です。" & jifunbyou)
                        End If
                        'フレームレート ByRefで返す
                        Try
                            fps = Double.Parse(Instr_pickup(results, " kb/s, ", " fps,", 0))
                        Catch ex As Exception
                        End Try
                    Else
                        log1write("【エラー】ffprobeによるDuration取得に失敗しました")
                    End If

                    'カレントディレクトリ変更
                    F_set_ppath4program()
                Else
                    log1write("【エラー】ffmpegフォルダにffprobe.exeが見つかりません")
                End If
            Catch ex As Exception
                log1write("【エラー】ffprobe.exe実行中にエラーが発生しました。" & ex.Message)
            End Try
        Else
            log1write("HLSアプリとしてffmpeg以外を使用している場合、動画情報を取得することはできません。iniのexepath_ffmpegを指定してください")
        End If

        Return r
    End Function
End Module
