﻿Imports System.IO

Module モジュール_ts解析
    Public Structure tot_structure
        Public fullpathfilename As String '録画ファイル名
        Public start_time As DateTime '開始日時
        Public start_utime As Integer '開始日時 unixtime
        Public err As Integer 'エラー番号
        Public errstr As String 'エラーメッセージ
        Public searchstr As String 'indexof用　fullpathfilename & ":" & start_utime
        Public end_time As DateTime '終了日時
        Public end_utime As Integer '終了日時 unixtime
        Public duration As Integer '動画の長さ
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

    Public Function get_TOT(ByVal fullpathfilename As String) As DateTime
        Dim r As DateTime = C_DAY2038
        Dim f As tot_structure = TOT_read(fullpathfilename)

        r = f.start_time

        Return r
    End Function

    Public Function TOT_read(ByVal fullpathfilename As String, Optional ByVal cache_only As Integer = 0) As tot_structure
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
            r.end_time = TOT_cache(i).end_time
            r.end_utime = TOT_cache(i).end_utime
            r.duration = TOT_cache(i).duration
            log1write(fullpathfilename & "の開始時間をキャッシュから取得しました")
        ElseIf cache_only = 0 Then
            '新規登録
            r = F_ts2tot(fullpathfilename, t2, t2_end)
            If r.err = 0 Then
                TOT_cache_index += 1
                If TOT_cache_index > TOT_cache_max Then
                    TOT_cache_index = 0
                End If
                TOT_cache(TOT_cache_index).fullpathfilename = r.fullpathfilename
                TOT_cache(TOT_cache_index).start_time = r.start_time
                TOT_cache(TOT_cache_index).start_utime = r.start_utime
                TOT_cache(TOT_cache_index).err = r.err
                TOT_cache(TOT_cache_index).errstr = r.errstr
                TOT_cache(TOT_cache_index).searchstr = searchstr
                TOT_cache(TOT_cache_index).end_time = r.end_time
                TOT_cache(TOT_cache_index).end_utime = r.end_utime
                TOT_cache(TOT_cache_index).duration = r.duration
                log1write(fullpathfilename & "の開始時間をTOTと作成日時から取得しました")
            End If
        End If

        Return r
    End Function

    'tsファイルから開始時刻を取得する t2は予備としてファイル作成日を指定
    Public Function F_ts2tot(ByVal fullpathfilename As String, ByVal t2 As DateTime, ByVal t2_end As DateTime) As tot_structure
        Dim r As tot_structure = Nothing
        Try
            Dim print_debug As Integer = -1 '結果を　0=ダンプしない 1=ダンプする -1=最終結果も表示しない

            Dim t0 As DateTime = CDate("1858/11/17")
            'ファイルを開く
            Dim fs As New System.IO.FileStream(fullpathfilename, System.IO.FileMode.Open, System.IO.FileAccess.Read)
            'ファイルを一時的に読み込むバイト型配列を作成する 
            Dim bs As Byte() = New Byte(187) {}

            Dim tstart As DateTime = C_DAY2038

            Dim t_chk As Integer = 0

            Dim err As Integer = 0
            Dim errstr As String = ""

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

            Dim tend As DateTime = CDate("1970/02/01")
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
                '秒数を調整（端数切り捨て）
                '始点
                Dim s_ajust As Integer = Second(tstart) Mod 10
                tstart = DateAdd(DateInterval.Second, -s_ajust, tstart)
                'ファイル作成日時と比べ併せて間違いなければファイル作成日時のほうを優先
                Try
                    If System.Math.Abs(time2unix(t2) - time2unix(tstart)) <= 10 Then
                        '10秒以内のずれならばファイル作成日時に合わせる
                        tstart = t2
                    End If
                Catch ex As Exception
                End Try
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
                    If TOT_get_duration > 0 Then
                        tend = t2_end
                    End If
                    log1write(fullpathfilename & "のTOTが取得できませんでした。作成日時を使用します")
                Catch ex As Exception
                    tstart = C_DAY2038
                End Try
            End If

            '結果を返す
            r.fullpathfilename = fullpathfilename
            r.start_time = tstart '一番はじめの記録時間
            r.start_utime = time2unix(tstart)
            r.end_time = tend '一番はじめの記録時間
            r.end_utime = time2unix(tend)
            r.duration = r.end_utime - r.start_utime
            Dim ext As String = Path.GetExtension(fullpathfilename)
            If ext.ToLower.IndexOf(".ts") > 0 Then
                If r.duration < 150 Or r.duration > (60 * 60 * 10) Then
                    '2分半未満、または10時間以上ならおかしな値なので
                    r.duration = 0
                End If
            Else
                'ts以外の場合、ファイルの作成日時＆更新日時から推測することはできないため
                'end_time,end_utimeも同様に不正確だが使用しないのでそのままにしておくか
                '他の調べ方があるかどうか
                r.duration = 0
            End If
            r.err = err
            r.errstr = errstr

            Return r
        Catch ex As Exception
            log1write("TOT解析中にエラーが発生しました。" & ex.Message)
            r.start_time = unix2time(0)
            r.err = 9
            Return r
        End Try
    End Function

End Module
