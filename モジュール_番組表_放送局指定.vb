Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO
Imports System.Runtime.Serialization
'参照で.net System.Runtime.Serializationを追加
Imports System.Text.RegularExpressions

Module モジュール_番組表_放送局指定
    Public Structure StationTVprogramstructure
        Implements IComparable
        Public name As String
        Public nameid As String
        Public startt As Integer
        Public endt As Integer
        Public title As String
        Public content As String
        Public thumbnail As String
        Public reserve As Integer
        Public rsv_change As String
        Public genre As String '番組ジャンル　nibble1 * 256 + nibble2
        Public fsid_startt As String '録画予約検索用
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.fsid_startt = pF Then
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Return Me.startt.CompareTo(DirectCast(obj, StationTVprogramstructure).startt)
        End Function
    End Structure

    Public Structure spReservestructure
        Public fsid As String
        Public sid As Integer
        Public startt As Integer
        Public endt As Integer
        Public title As String
        Public eid As String
        Public yid As String
        Public station As String
        Public recmode As String 'EDCB 5=無効
    End Structure

    Public Function WI_GET_STATION_PROGRAM(ByVal temp_str As String) As String
        Dim i As Integer = 0
        'temp_str                取得元録画ソフト名,sid又はチャンネルid,開始時間,終了時間
        Dim src() As String = Nothing
        Dim url As String = ""
        Dim sid_str As String = ""
        Dim sid As Integer = 0
        Dim station_name As String = ""
        Dim startt As Integer = 0
        Dim endt As Integer = 0
        Dim force As Integer = 0
        Dim d() As String = temp_str.Split(",")
        If d.Length >= 2 Then
            For i = 0 To d.Length - 1
                d(i) = filename_escape_recall(Trim(d(i)))
            Next
            src = Trim(d(0)).Split("_")
            sid_str = Trim(d(1))
            If IsNumeric(sid_str) = True Then
                sid = Val(sid_str)
            End If
            If d.Length >= 4 Then
                startt = Val(Trim(d(2)))
                If startt = 0 Then
                    startt = time2unix(Now())
                End If
                endt = Val(Trim(d(3)))
                If endt = 0 Then
                    endt = startt + (60 * 60 * 6) '6時間
                End If
            Else
                startt = time2unix(Now())
                endt = startt + (60 * 60 * 6) '6時間
            End If
            If d.Length >= 5 Then
                force = Val(d(4))
            End If
            If d.Length >= 6 Then
                station_name = Trim(d(5))
            End If
        End If
        '1行目に放送局データ 　　放送局名,sid_str,情報取得元録画ソフト,URL
        Dim firstline As String = ""
        '以後番組データ　　　　　start,end,title,content,genre,thumbnail,reserve,reserve_change_data
        Dim r As String = ""
        If sid_str.Length > 0 Then
            For Each src1 As String In src
                '放送局
                Dim p() As StationTVprogramstructure = Nothing
                Select Case src1.ToLower
                    Case "tvrock"
                        'http://127.0.0.1:40003/WI_GET_STATION_PROGRAM.html?temp=TvRock,101
                        url = TvProgram_tvrock_url
                        p = get_station_program_TvRock(sid, endt, force, station_name) 'tvrockにはstarttは不要
                    Case "edcb"
                        url = TvProgram_EDCB_url
                        p = get_station_program_EDCB(sid, startt, endt, station_name)
                    Case "tvmaid"
                        url = Tvmaid_url
                        p = get_station_program_TvmaidEX(sid, startt, endt, station_name)
                    Case "pttimer"
                        url = ptTimer_path
                        p = get_station_program_ptTimer(sid, startt, endt, station_name)
                    Case "abematv"
                        sid = 99999801
                        url = sid_str
                        p = get_station_program_AbemaTV(sid_str, startt, endt)
                End Select
                '文字列に整形
                If p IsNot Nothing Then
                    r &= filename_escape_set(p(0).name) & "," _
                        & filename_escape_set(p(0).nameid) & "," _
                        & sid.ToString & "," _
                        & filename_escape_set(src1) & "," _
                        & filename_escape_set(url) & vbCrLf
                    For i = 0 To p.Length - 1
                        If p(i).startt > 0 Then
                            r &= p(i).startt & "," _
                                & p(i).endt & "," _
                                & filename_escape_set(p(i).title) & "," _
                                & filename_escape_set(p(i).content) & "," _
                                & p(i).genre & "," _
                                & filename_escape_set(p(i).thumbnail) & "," _
                                & p(i).reserve & "," _
                                & filename_escape_set(p(i).rsv_change) & vbCrLf
                        End If
                    Next

                    Exit For '1つ見つかれば十分
                End If
            Next
        End If
        Return r
    End Function

    Private Function get_station_program_AbemaTV(ByVal sid_str As String, ByVal startt As Integer, ByVal endt As Integer) As StationTVprogramstructure()
        Dim r() As StationTVprogramstructure = Nothing
        Try
            'Outside番組表を取得
            Dim html As String = get_Outside_html(0)
            If Outside_CustomURL_method = 1 Then
                '都合の良いデータ形式の場合
                If html.Length >= 300 Then
                    Dim log_temp As String = "　　>>AbemaTV放送局別番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

                    Try
                        Dim line() As String = Split(html, vbCrLf)
                        Dim temp() As String = line(0).Split(",")
                        If temp.Length > 100 Then
                            'うまく分割できていない可能性
                            line = Split(html, vbLf) 'unix
                        End If
                        Dim chk_inTime As Integer = 0
                        Dim chk_past As String = ""
                        For i = 0 To line.Length - 1
                            Dim d() As String = line(i).Split(",")
                            If d.Length >= 6 Then
                                For i2 As Integer = 0 To d.Length - 1
                                    d(i2) = Trim(d(i2))
                                Next
                                If d(0).ToLower = sid_str.ToLower And ((Val(d(2)) < startt And Val(d(3)) >= startt) Or (Val(d(2)) < endt And Val(d(2)) >= startt)) Then
                                    chk_past = sid_str
                                    chk_inTime = 1 '番組表が見つかった
                                    Dim j As Integer = 0
                                    If r Is Nothing Then
                                        j = 0
                                    Else
                                        j = r.Length
                                    End If
                                    ReDim Preserve r(j)
                                    r(j).name = d(1).Replace("チャンネル", "")
                                    r(j).nameid = d(0)
                                    r(j).startt = Val(d(2))
                                    r(j).endt = Val(d(3))
                                    r(j).title = escape_program_str(d(4))
                                    r(j).content = escape_program_str(d(5))
                                    r(j).thumbnail = ""
                                    If d.Length >= 7 Then
                                        r(j).thumbnail = filename_escape_recall(d(6))
                                    End If
                                    r(j).reserve = -1
                                    r(j).rsv_change = ""
                                    If d.Length >= 8 Then
                                        r(j).genre = d(7)
                                    Else
                                        r(j).genre = -1
                                    End If
                                ElseIf chk_past.Length > 0 And sid_str <> chk_past Then
                                    Exit For
                                End If
                            End If
                        Next
                        log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                        log1write(log_temp)
                        If chk_inTime = 0 Then
                            log1write("【エラー】" & Outside_StationName & "番組情報内に現在の情報が含まれていません")
                        End If
                    Catch ex As Exception
                        log1write("【エラー】AbemaTV番組情報取得中にエラーが発生しました。" & ex.Message)
                    End Try
                Else
                    log1write("取得した" & Outside_StationName & "番組表が不正です。" & Outside_CustomURL)
                End If
            Else
                log1write("【エラー】未対応の" & Outside_StationName & "解析形式です。Outside_CustomURL_method=" & Outside_CustomURL_method)
            End If
        Catch ex As Exception
            log1write(Outside_StationName & "番組表取得に失敗しました。" & ex.Message)
        End Try

        Return r
    End Function

    Public Function searchJigyoushaBySIDandNAME(ByVal p_sid As Integer, ByVal p_station As String)
        Dim r As Integer = -1

        If p_sid = 161 Then
            'とりあえずQVCとBS-TBSだけ詳しくチェック
            If ch_list IsNot Nothing Then
                For i As Integer = 0 To ch_list.Length - 1
                    If ch_list(i).sid = p_sid And ch_list(i).jigyousha = p_station Then
                        r = i
                        Exit For
                    End If
                Next
            End If
        Else
            r = Array.IndexOf(ch_list, p_sid)
        End If

        Return r
    End Function

    Private Function get_station_program_EDCB(ByVal p_sid As Integer, ByVal startt As Integer, ByVal endt As Integer, ByVal p_station As String) As StationTVprogramstructure()
        Dim r() As StationTVprogramstructure = Nothing

        If TvProgram_EDCB_url.Length > 0 Then
            If ch_list IsNot Nothing Then
                Try
                    Dim log_temp As String = "　　>>EDCB放送局別番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                    Dim i As Integer = 0
                    Dim k As Integer = 0

                    Dim RecSrc As String = "[EDCB]"

                    Dim fsid_long As Long = 0
                    Dim z As Integer = searchJigyoushaBySIDandNAME(p_sid, p_station)
                    If z >= 0 Then
                        fsid_long = CType(ch_list(z).nid, Long) * 256 * 256 * 256 * 256 + CType(ch_list(z).tsid, Long) * 256 * 256 + CType(ch_list(z).sid, Long)
                        '↑ONIDがよくわからない・・これでも一応動く
                    End If

                    Dim ip As String = Instr_pickup(TvProgram_EDCB_url, "://", ":", 0)
                    ip = host2ip(ip) 'ホストネームからIPに変換
                    Dim port As String = ""
                    port = 4510 'CtrlCmdCLIのポート　決め打ち？

                    If ip.Length > 0 And Val(port) > 0 Then
                        EDCB_cmd.SetSendMode(True)
                        EDCB_cmd.SetNWSetting(ip, port)
                        Dim epgList As New System.Collections.Generic.List(Of CtrlCmdCLI.Def.EpgEventInfo)()
                        Dim ret As Integer = EDCB_cmd.SendEnumPgInfo(fsid_long, epgList) 'IPやportがおかしいとここで止まる可能性有り
                        log_temp &= " > 取得完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                        If ret = 1 Then
                            For Each a As CtrlCmdCLI.Def.EpgEventInfo In epgList
                                'まず現在時刻にあてはまるかチェック
                                Dim t1 As Integer = time2unix(a.start_time)
                                Dim t2 As Integer = t1 + a.durationSec
                                If (t1 < startt And t2 >= startt) Or (t1 < endt And t1 >= startt) Then
                                    Dim j As Integer = 0
                                    If r Is Nothing Then
                                        j = 0
                                    Else
                                        j = r.Length
                                    End If
                                    ReDim Preserve r(j)
                                    r(j).name = ch_list(z).jigyousha
                                    r(j).nameid = ""
                                    r(j).startt = time2unix(a.start_time)
                                    r(j).endt = r(j).startt + a.durationSec
                                    r(j).title = escape_program_str(a.ShortInfo.event_name)
                                    r(j).content = escape_program_str(a.ShortInfo.text_char)
                                    r(j).thumbnail = ""
                                    r(j).reserve = -1
                                    r(j).rsv_change = RecSrc & p_sid & "," & r(j).startt & "," & r(j).endt & "," & a.event_id & "," & "-1" & "," & "-1"
                                    Dim jnr As CtrlCmdCLI.Def.EpgContentInfo = a.ContentInfo
                                    If jnr IsNot Nothing Then
                                        r(j).genre = (jnr.nibbleList(0).content_nibble_level_1 * 256 + jnr.nibbleList(0).content_nibble_level_2).ToString
                                    Else
                                        r(j).genre = -1
                                    End If
                                    r(j).fsid_startt = a.service_id & "_" & a.event_id '予約の検索用
                                End If
                            Next
                            Array.Sort(r)

                            '予約状況を照らし合わせr()を修正
                            If r IsNot Nothing Then
                                log_temp &= " > 予約状況解析開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                                '予約状況を照らし合わせr()を修正
                                Dim spReserve() As spReservestructure = get_station_program_EDCB_reserve(p_sid)
                                If spReserve IsNot Nothing Then
                                    For i = 0 To spReserve.Length - 1
                                        Dim ri As Integer = Array.IndexOf(r, spReserve(i).sid & "_" & spReserve(i).eid)
                                        If ri >= 0 Then
                                            If spReserve(i).recmode <> 5 Then '5=無効
                                                r(ri).reserve = 1
                                            ElseIf r(ri).reserve < 0 Then
                                                r(ri).reserve = 0
                                            End If
                                            r(ri).rsv_change = RecSrc & spReserve(i).sid & "," & r(ri).startt & "," & r(ri).endt & "," & spReserve(i).eid & "," & spReserve(i).yid & "," & spReserve(i).recmode
                                        End If
                                    Next
                                End If
                                '一巡してr()に対して判定がなされてなければ予約されていないとわかる
                                For kk As Integer = 0 To r.Length - 1
                                    If r(kk).reserve < 0 Then
                                        r(kk).reserve = 0
                                    End If
                                Next
                            End If

                            log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                            log1write(log_temp)
                        End If
                    End If
                Catch ex As Exception
                    log1write("【エラー】EDCB放送局別番組情報取得中にエラーが発生しました。" & ex.Message)
                End Try
            End If
        End If

        Return r
    End Function

    Private Function get_station_program_EDCB_reserve(ByVal p_sid As Integer) As spReservestructure()
        Dim r() As spReservestructure = Nothing

        If TvProgram_EDCB_url.Length > 0 Then
            If ch_list IsNot Nothing Then
                Try
                    'Dim log_temp As String = ">>EDCB予約状況 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                    Dim i As Integer = 0
                    Dim k As Integer = 0

                    Dim ip As String = Instr_pickup(TvProgram_EDCB_url, "://", ":", 0)
                    ip = host2ip(ip) 'ホストネームからIPに変換
                    Dim port As String = ""
                    port = 4510 'CtrlCmdCLIのポート　決め打ち？

                    If ip.Length > 0 And Val(port) > 0 Then
                        EDCB_cmd.SetSendMode(True)
                        EDCB_cmd.SetNWSetting(ip, port)
                        Dim epgList As New System.Collections.Generic.List(Of CtrlCmdCLI.Def.ReserveData)()
                        Dim ret As Integer = EDCB_cmd.SendEnumReserve(epgList) 'IPやportがおかしいとここで止まる可能性有り
                        If ret = 1 Then
                            For Each a As CtrlCmdCLI.Def.ReserveData In epgList
                                If p_sid = a.ServiceID Then
                                    Dim j As Integer = 0
                                    If r Is Nothing Then
                                        j = 0
                                    Else
                                        j = r.Length
                                    End If
                                    ReDim Preserve r(j)
                                    r(j).title = a.Title
                                    r(j).sid = p_sid.ToString
                                    r(j).startt = time2unix(a.StartTime)
                                    r(j).endt = r(j).startt + a.DurationSecond
                                    r(j).eid = a.EventID.ToString
                                    r(j).yid = a.ReserveID.ToString
                                    r(j).recmode = a.RecSetting.RecMode.ToString '5=無効
                                End If
                            Next

                            'log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                            'log1write(log_temp)
                        End If
                    End If
                Catch ex As Exception
                    log1write("【エラー】EDCB予約状況取得中にエラーが発生しました。" & ex.Message)
                End Try
            End If
        End If

        Return r
    End Function

    Private Function get_station_program_TvmaidEX(ByVal p_sid As Integer, ByVal startt As Integer, ByVal endt As Integer, ByVal p_station As String) As StationTVprogramstructure()
        Dim r() As StationTVprogramstructure = Nothing

        Dim maya As Integer = 0

        Dim nextsec As Integer = endt - startt
        If nextsec <= 0 Then
            nextsec = 60 * 60 * 6 '6時間
        End If

        Dim RecSrc As String = "[TVMAID YUI]"

        If Tvmaid_url.Length > 0 Then
            'データベースから番組一覧を取得する
            Try
                Dim log_temp As String = "　　>>TvmaidEX放送局別番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

                'fsid算出
                Dim fsid As String = ""
                Dim fsid_long_str As String = ""
                Dim fi As Integer = searchJigyoushaBySIDandNAME(p_sid, p_station)
                If fi >= 0 Then
                    fsid = Hex(ch_list(fi).nid).PadLeft(4, "0") & Hex(ch_list(fi).tsid).PadLeft(4, "0") & Hex(ch_list(fi).sid).PadLeft(4, "0")
                    fsid_long_str = (CType(ch_list(fi).nid, Long) * 65536 * 65536 + CType(ch_list(fi).tsid, Long) * 65536 + CType(ch_list(fi).sid, Long)).ToString
                End If

                If fsid.Length > 0 Then
                    Dim nowtime As DateTime = unix2time(startt)
                    Dim nowtime_n As DateTime = unix2time(endt)
                    Dim ut_b As Long = DateTime.Parse(nowtime).ToBinary
                    Dim utn_b As Long = DateTime.Parse(nowtime_n).ToBinary
                    Dim url As String = Tvmaid_url & "/webapi/GetTable?sql="
                    Dim msql As String = ""
                    url &= "SELECT fsid,start,end,duration,title,desc,genre,eid from event WHERE fsid = " & fsid_long_str & " AND ((start < " & ut_b & " AND end >= " & ut_b & ") OR (start < " & utn_b & " AND start >= " & ut_b & ")) ORDER BY start"
                    url = url.Replace("//webapi/", "/webapi/")

                    Dim wc As New WebClient
                    Dim ms As New MemoryStream
                    Dim sw As New StreamWriter(ms)
                    Dim st As Stream = wc.OpenRead(url)
                    Dim sr As New StreamReader(st, Encoding.UTF8)
                    Dim s As String = sr.ReadToEnd

                    Dim i, j As Integer

                    sw.Write(s)
                    sw.Flush()
                    ms.Position = 0

                    log_temp &= " > 取得完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

                    Dim jsonRead As New Json.DataContractJsonSerializer(GetType(tvmaidExData.TvmaidExReserve))
                    Dim tr As tvmaidExData.TvmaidExReserve = jsonRead.ReadObject(ms)
                    If tr.code = 0 Then
                        For i = 0 To tr.data1.Count - 1
                            'タイトル
                            Dim title As String = escape_program_str(tr.data1(i)(4))
                            'TSID,SIDを算出
                            Dim tsid_long As String = Hex(tr.data1(i)(0))
                            Dim tsid_hex As String = ""
                            Dim tsid As Integer = 0
                            Dim sid_hex As String = ""
                            Dim sid As Integer = 0
                            If tsid_long.Length = 12 Then
                                '地デジ
                                tsid_hex = tsid_long.Substring(0, 4) '初めの4文字
                                tsid = h16_10("0x" & tsid_hex)
                                sid_hex = tsid_long.Substring(8, 4) '最後の4文字
                                sid = h16_10("0x" & sid_hex)
                            ElseIf tsid_long.Length = 9 Then
                                'BS/CS
                                tsid_hex = tsid_long.Substring(1, 4) '初めの4文字
                                tsid = h16_10("0x" & tsid_hex)
                                sid_hex = tsid_long.Substring(5, 4) '最後の4文字
                                sid = h16_10("0x" & sid_hex)
                            Else
                                '未知の形式
                            End If

                            '開始時間
                            Dim ystartDate As DateTime = DateTime.FromBinary(tr.data1(i)(1)) '開始時刻(調整いらず）
                            Dim ystart As Integer = time2unix(ystartDate)
                            '録画時間（秒）
                            Dim duration As Integer = Val(tr.data1(i)(3))
                            '重複チェック用文字列
                            Dim indexofstr As String = sid & "_" & ystart & "_" & duration

                            If sid = p_sid And ystart > 0 And duration > 0 Then
                                'ch_list()にsidが登録されていれば
                                '録画開始時間
                                Dim ystart_day As String = ystartDate.ToShortDateString
                                '録画終了時間
                                Dim yend As Integer = ystart + duration
                                Dim yendDate As DateTime = unix2time(yend)
                                '録画時間
                                Dim delta As Integer = duration
                                Dim deltastr_time As String = Int(delta / (60 * 60)).ToString.PadLeft(2, "0")
                                Dim deltastr_minute As String = (Int(delta / 60) - (Val(deltastr_time) * 60)).ToString.PadLeft(2, "0")
                                Dim deltastr_sec As String = Int(delta Mod 60).ToString.PadLeft(2, "0")
                                Dim deltastr As String = deltastr_time & ":" & deltastr_minute & ":" & deltastr_sec
                                '内容
                                Dim texts As String = escape_program_str(tr.data1(i)(5))
                                '改行（\u000d\u000a）が入ることがあるのかな・・
                                'eid
                                Dim eid As String = Trim(tr.data1(i)(7).ToString)
                                'ジャンル
                                Dim genre As Integer = -1
                                Try
                                    Dim g1_str As String = Trim(tr.data1(i)(6).ToString)
                                    Dim g1 As Long = -1
                                    Try
                                        g1 = CType(g1_str, Long)
                                    Catch ex2 As Exception
                                    End Try
                                    If g1 > 1000000000 Then
                                        'MAYA
                                        RecSrc = "[TVMAID MAYA]"
                                        genre = Int((g1 Mod 256) / 16) * 256
                                        maya = 1
                                    Else
                                        '旧式
                                        If g1 > 15 Then
                                            g1 = 15
                                        End If
                                        If g1 >= 0 And g1 <= 15 Then
                                            genre = g1 * 256
                                        End If
                                    End If
                                    If genre < 0 Then
                                        genre = -1
                                    End If
                                Catch ex2 As Exception
                                End Try

                                '放送局名
                                Dim station As String
                                station = sid2jigyousha(sid, tsid)

                                Dim chk As Integer = -1
                                If r IsNot Nothing Then
                                    chk = Array.IndexOf(r, station)
                                    If chk >= 0 Then
                                        If r(chk).startt = ystart Then
                                            '重複（同じsid,時刻）
                                            chk = 1
                                        Else
                                            chk = -1
                                        End If
                                    End If
                                End If
                                If chk < 0 Then
                                    '重複がなければ
                                    If r Is Nothing Then
                                        j = 0
                                    Else
                                        j = r.Length
                                    End If
                                    ReDim Preserve r(j)
                                    r(j).name = station
                                    r(j).nameid = ""
                                    r(j).startt = ystart
                                    r(j).endt = yend
                                    r(j).title = title
                                    r(j).content = texts
                                    r(j).thumbnail = ""
                                    r(j).reserve = -1
                                    r(j).rsv_change = RecSrc & fsid_long_str & "," & eid & "," & "-1"
                                    r(j).genre = genre
                                    r(j).fsid_startt = fsid_long_str & "_" & eid 'ystart
                                End If
                            End If
                        Next
                    End If

                    '後処理
                    sr.Close()
                    st.Close()
                    sw.Close()
                    st.Close()

                    If r IsNot Nothing Then
                        log_temp &= " > 予約状況解析開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                        '予約状況を照らし合わせr()を修正
                        Dim spReserve() As spReservestructure = get_station_program_TvmaidEX_reserve(fsid_long_str, maya)
                        If spReserve IsNot Nothing Then
                            For i = 0 To spReserve.Length - 1
                                Dim ri As Integer = Array.IndexOf(r, spReserve(i).fsid & "_" & spReserve(i).eid)
                                If ri >= 0 Then
                                    r(ri).reserve = 1
                                    r(ri).rsv_change = RecSrc & spReserve(i).fsid & "," & spReserve(i).eid & "," & spReserve(i).yid
                                End If
                            Next
                        End If
                        '一巡してr()に対して判定がなされてなければ予約されていないとわかる
                        For kk As Integer = 0 To r.Length - 1
                            If r(kk).reserve < 0 Then
                                r(kk).reserve = 0
                            End If
                        Next
                    End If
                End If

                log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                log1write(log_temp)
            Catch ex As Exception
                log1write("【エラー】Tvmaid放送局別番組情報取得中にエラーが発生しました。" & ex.Message)
            End Try
        End If

        Return r
    End Function

    Private Function get_station_program_TvmaidEX_reserve(ByVal fsid As String, ByVal maya As Integer) As spReservestructure()
        'fsid_startunixtime_endunixtimeの形式
        Dim r() As spReservestructure = Nothing

        Dim TableName As String = "record"
        If maya = 1 Then
            TableName = "reserve"
        End If

        If Tvmaid_url.Length > 0 Then
            'データベースから番組一覧を取得する
            Try
                'Dim log_temp As String = ">>TvmaidEX録画予約 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

                If fsid.Length > 0 Then
                    Dim url As String = Tvmaid_url & "/webapi/GetTable?sql="
                    Dim msql As String = ""
                    url &= "SELECT fsid,start,end,duration,title,status,id,eid from " & TableName & " WHERE fsid = " & fsid & " ORDER BY start"
                    url = url.Replace("//webapi/", "/webapi/")

                    Dim wc As New WebClient
                    Dim ms As New MemoryStream
                    Dim sw As New StreamWriter(ms)
                    Dim st As Stream = wc.OpenRead(url)
                    Dim sr As New StreamReader(st, Encoding.UTF8)
                    Dim s As String = sr.ReadToEnd

                    Dim j As Integer = 0

                    sw.Write(s)
                    sw.Flush()
                    ms.Position = 0

                    Dim jsonRead As New Json.DataContractJsonSerializer(GetType(tvmaidExData.TvmaidExReserve))
                    Dim tr As tvmaidExData.TvmaidExReserve = jsonRead.ReadObject(ms)

                    If tr.code = 0 Then
                        For i = 0 To tr.data1.Count - 1
                            'タイトル
                            Dim title As String = escape_program_str(tr.data1(i)(4))
                            '開始時間
                            Dim ystartDate As DateTime = DateTime.FromBinary(tr.data1(i)(1)) '開始時刻(調整いらず）
                            Dim ystart As Integer = time2unix(ystartDate)
                            '終了時間
                            Dim yendDate As DateTime = DateTime.FromBinary(tr.data1(i)(2)) '開始時刻(調整いらず）
                            Dim yend As Integer = time2unix(ystartDate)
                            '録画時間（秒）
                            Dim duration As Integer = Val(tr.data1(i)(3))
                            'status
                            Dim status As Integer = 0
                            Dim status1 As Integer = Val(tr.data1(i)(5))
                            Dim active As Integer = status1 And 1
                            Dim working As Integer = status1 And 64
                            Dim done As Integer = status1 And 128
                            If (active = 1 Or working = 1) And done = 0 Then
                                status = 1
                            End If
                            '予約id
                            Dim yid As String = tr.data1(i)(6)
                            'eid
                            Dim eid As String = tr.data1(i)(7)

                            If status = 1 Then
                                ReDim Preserve r(j)
                                r(j).fsid = fsid
                                r(j).startt = ystart
                                r(j).endt = yend
                                r(j).title = title
                                r(j).yid = yid
                                r(j).eid = eid
                                j += 1
                            End If
                        Next
                    End If

                    '後処理
                    sr.Close()
                    st.Close()
                    sw.Close()
                    st.Close()
                End If

                'log_temp &= " > 取得完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                'log1write(log_temp)
            Catch ex As Exception
                log1write("【エラー】TvmaidEX録画予約情報取得中にエラーが発生しました。" & ex.Message)
            End Try
        End If

        Return r
    End Function

    Private TvRock_station_program_backup As String = ""
    Private TvRock_station_program_date As DateTime = CDate("1980/01/01")
    Private Function get_station_program_TvRock(ByVal p_sid As Integer, ByVal p_endt As Integer, ByVal force As Integer, ByVal p_station As String) As StationTVprogramstructure()
        Dim r() As StationTVprogramstructure = Nothing
        'TvRockの場合はstarttに意味無し
        Dim err_count As Integer = 0
        Dim first_trid As Integer = 0
        Dim RecSrc As String = "[TVROCK]"
        Try
            If TvProgram_tvrock_tuner >= 0 Then
                'チャンネル取得前にチューナーを指定する
                log1write("TVROCKのチューナーを" & TvProgram_tvrock_tuner.ToString & "番にセットしています")
                Dim html As String = get_html_by_webclient(TvProgram_tvrock_url & "?md=2&d=" & TvProgram_tvrock_tuner.ToString, "Shift_JIS")
                If html.IndexOf(")"">チューナー") > 0 And html.IndexOf(">録画<") > 0 Then
                    log1write("TVROCKのチューナーを" & TvProgram_tvrock_tuner.ToString & "番にセットしました")
                ElseIf TvProgram_tvrock_tuner = 0 And html.IndexOf(")"">チューナー") > 0 Then
                    log1write("TVROCKのチューナー指定をリセットしました")
                Else
                    log1write("【エラー】TVROCKのチューナーをセット出来ませんでした。チューナー番号を確認してください")
                End If
            End If

            If TvProgram_tvrock_url.Length > 0 Then
                Dim log_temp As String = "　　>>TvRock放送局別番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                Dim html As String = ""
                Dim t As DateTime = Now
                If force = 1 Or Minute(t) <> Minute(TvRock_station_program_date) Then
                    TvRock_station_program_date = t
                    html = get_html_by_webclient(TvProgram_tvrock_url, "Shift_JIS")
                    TvRock_station_program_backup = html
                    log_temp &= " > 取得完了(" & html.Length.ToString & ")：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                Else
                    html = TvRock_station_program_backup
                    log_temp &= " > 取得完了(キャッシュ" & html.Length.ToString & ")：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                End If

                '<small>ＮＨＫＢＳ１ <small><i> のようになっている
                Dim sp2 As Integer = html.IndexOf(" <small><i>")
                Dim sp As Integer = html.LastIndexOf("><small>", sp2 + 1) 'sp=チャンネルの頭
                While sp > 0
                    Try
                        Dim j As Integer = 0
                        Dim temp_stationDispName As String = ""
                        Dim temp_startt As Integer = 0
                        Dim temp_endt As Integer = 0
                        Dim temp_programTitle As String = ""
                        Dim temp_programContent As String = ""
                        Dim temp_genre As String = ""
                        Dim temp_sid As Integer = 0

                        Dim plus_days As Integer = 0

                        temp_stationDispName = Instr_pickup(html, "<small>", " <small>", sp)
                        If temp_stationDispName.LastIndexOf(")""><small>") > 0 Then
                            '番組表データが無いチャンネルが混じっていることがある
                            temp_stationDispName = temp_stationDispName.Substring(temp_stationDispName.IndexOf(")""><small>") + ")""><small>".Length)
                        End If
                        temp_stationDispName = Trim(temp_stationDispName)
                        temp_stationDispName = Trim(delete_tag(temp_stationDispName))
                        Dim s11 As String = Trim(delete_tag(Instr_pickup(html, "<i>", "～", sp)))
                        Dim s12 As String = Trim(delete_tag(Instr_pickup(html, "～", "</i></small>", sp2)))
                        Dim dt() As Integer = fix_time_d2u(s11, s12, 0, plus_days) 'plus_daysはByRef
                        temp_startt = dt(0)
                        temp_endt = dt(1)
                        temp_programTitle = escape_program_str(delete_tag(Instr_pickup(html, "<small><b>", "</b></small>", sp)))
                        Dim sp3 As Integer = html.IndexOf("<font color=", sp)
                        If sp3 >= 0 Then
                            temp_programContent = escape_program_str(delete_tag(Instr_pickup(html, ">", "</font>", sp3)))
                        Else
                            temp_programContent = ""
                            sp3 = html.IndexOf("<small><b>", sp)
                        End If
                        '予約番号がわからないのでタイトルから推測
                        temp_genre = get_tvrock_genre_from_program(0, 0, temp_programTitle).ToString '"-1"
                        If temp_genre < 0 Then
                            '予約のためわからなかった可能性
                            temp_genre = get_tvrock_genre_from_search(0, 0, temp_programTitle).ToString
                        End If

                        '次のチャンネルが始まる地点
                        Dim se As Integer = html.IndexOf(" <small><i>", sp2 + 1)
                        If se < 0 Then
                            se = html.Length - 1
                        End If

                        'サービスIDを取得
                        Dim sp4 As Integer = html.IndexOf("javascript:reserv(", sp)
                        If sp4 < 0 Or sp4 > se Then
                            sp4 = html.IndexOf("javascript:delsc(", sp)
                        End If
                        If sp4 > 0 And sp4 < se Then
                            temp_sid = Val(Instr_pickup(html, ",", ",", sp4))
                            If temp_sid = 161 Then
                                'とりあえずQVCとBS-TBSだけチェック
                                If StrConv(p_station, VbStrConv.Wide) <> StrConv(temp_stationDispName, VbStrConv.Wide) Then
                                    temp_sid = -1
                                End If
                            End If
                            If temp_sid = p_sid Then
                                '指定局ならば
                                If temp_startt < p_endt Then
                                    j = 0
                                    ReDim Preserve r(j)
                                    r(j).name = temp_stationDispName
                                    r(j).nameid = ""
                                    r(j).startt = temp_startt
                                    r(j).endt = temp_endt
                                    r(j).title = temp_programTitle
                                    r(j).content = temp_programContent
                                    r(j).thumbnail = ""
                                    r(j).reserve = -1
                                    r(j).rsv_change = ""
                                    r(j).genre = temp_genre
                                    r(j).fsid_startt = temp_stationDispName & "_" & temp_startt.ToString
                                End If

                                '次の番組を取得
                                sp3 = html.IndexOf("</i>", sp3 + 1)
                                sp3 = html.LastIndexOf("<i>", sp3 + 1)
                                While sp3 > 0 And sp3 < se
                                    '次の番組があれば
                                    '予約されているかどうか
                                    Dim rsv As Integer = 0
                                    If html.Substring(sp3 - 1, 1) = "]" Then
                                        '予約されている（とは限らない！　無効と有効の区別がつかない）
                                        rsv = -1
                                    End If
                                    Dim spn As Integer = html.IndexOf("href=""javascript:", sp3)
                                    Dim sid As Integer = 0
                                    Dim trid As Integer = 0
                                    Dim sidtrid_str As String = ""
                                    If spn > 0 Then
                                        sidtrid_str = Instr_pickup(html, "(", ")", spn + 1, se)
                                        Dim d() As String = sidtrid_str.Split(",")
                                        If d.Length = 4 Then
                                            sid = Val(d(1))
                                            trid = Val(d(2))
                                        End If
                                    End If

                                    If trid > 0 And first_trid = 0 Then
                                        first_trid = trid
                                    End If

                                    s11 = Trim(delete_tag(Instr_pickup(html, "<i>", "～", sp3)))
                                    s12 = Trim(delete_tag(Instr_pickup(html, "～", "</i></small>", sp3)))
                                    dt = fix_time_d2u(s11, s12, 1, plus_days) 'plus_daysはByRef
                                    temp_startt = dt(0)
                                    temp_endt = dt(1)
                                    If temp_startt < p_endt Then
                                        If r Is Nothing Then
                                            j = 0
                                        Else
                                            j = r.Length
                                        End If
                                        ReDim Preserve r(j)
                                        r(j).name = temp_stationDispName
                                        r(j).nameid = ""
                                        r(j).startt = temp_startt
                                        r(j).endt = temp_endt
                                        r(j).title = escape_program_str(delete_tag(Instr_pickup(html, "<small><small><small>", "</small></small></small>", sp3)))
                                        r(j).content = ""
                                        r(j).thumbnail = ""
                                        r(j).reserve = -1 'iphone画面では確定できない 'rsv
                                        Dim s5 As Integer = html.IndexOf("<a href=""javascript", sp3)
                                        If s5 > 0 And s5 < se Then
                                            r(j).rsv_change = RecSrc & "javascript" & Instr_pickup(html, "<a href=""javascript", ")", sp3) & ")"
                                        Else
                                            r(j).rsv_change = ""
                                        End If
                                        r(j).fsid_startt = temp_stationDispName & "_" & temp_startt.ToString
                                        If TvRock_genre_ON = 1 Then
                                            Dim r1chk As Integer = 0
                                            If rsv = 0 Then
                                                '予約されていない場合は番組表から
                                                r(j).genre = get_tvrock_genre_from_program(sid, trid, r(j).title).ToString
                                                r1chk = 1
                                            Else
                                                '予約されている場合は検索から
                                                r(j).genre = get_tvrock_genre_from_search(sid, trid, r(j).title).ToString
                                                r1chk = 2
                                            End If
                                            If r(j).genre < 0 And temp_startt - time2unix(t) < 120 Then
                                                '120分以内なのに見つからない
                                                log1write("【ジャンルが見つかりませんでした】sid=" & sid & " trid=" & trid & " title=" & r(j).title)
                                            End If
                                        End If
                                    Else
                                        Exit While
                                    End If

                                    sp3 = html.IndexOf("</i>", sp3 + 1)
                                    sp3 = html.IndexOf("<i>", sp3 + 1)
                                    If sp3 < 0 Then
                                        Exit While
                                    End If
                                End While
                                Exit While
                            End If
                        Else
                            '番組情報取得失敗
                            '数局なら番組情報の無い放送局という可能性有り
                            err_count += 1
                        End If
                    Catch ex2 As Exception
                        log1write("【エラー】TvRockからの放送局別番組表取得中にエラーが発生しました。" & ex2.Message)
                    End Try
                    sp2 = html.IndexOf(" <small><i>", sp2 + 1)
                    sp = html.LastIndexOf("><small>", sp2 + 1)
                End While

                '先頭の番組IDを推定する
                Dim r0_chk As Integer = 0
                If TvRock_html_program_src.Length > 0 Then
                    '番組IDがわからない・・1番始めに見つかったtridから遡った直近のものだろう
                    Dim s5 As Integer = TvRock_html_program_src.IndexOf("&c=" & p_sid & "&e=" & first_trid)
                    If s5 > 0 Then
                        Dim s6 As Integer = TvRock_html_program_src.LastIndexOf("&c=" & p_sid, s5 - 1)
                        If s6 > 0 Then
                            Dim s7 As String = Instr_pickup(TvRock_html_program_src, "&e=", "&", s6)
                            r(0).reserve = -1 'まだ確定できない
                            r(0).rsv_change = RecSrc & "javascript:reserv(0," & p_sid & "," & s7 & ",0)"
                            r0_chk = 1 '最初の項目のチェックが終わった
                        End If
                    End If
                End If

                If r IsNot Nothing Then
                    log_temp &= " > 予約状況解析開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                    '予約状況を照らし合わせr()を修正
                    Dim spReserve() As spReservestructure = get_station_program_TvRock_reserve(p_sid)
                    If spReserve IsNot Nothing Then
                        For i = 0 To spReserve.Length - 1
                            Dim ri As Integer = Array.IndexOf(r, spReserve(i).station & "_" & spReserve(i).startt)
                            If ri >= 0 Then
                                '放送局名と開始時間が一致
                                If spReserve(i).recmode = 1 Then
                                    r(ri).reserve = 1
                                    r(ri).rsv_change = RecSrc & "list?i=" & spReserve(i).yid & "&amp;val=" & (Val(spReserve(i).recmode) + 1)
                                ElseIf r(ri).reserve < 0 Then
                                    r(ri).reserve = 0
                                    r(ri).rsv_change = RecSrc & "list?i=" & spReserve(i).yid & "&amp;val=" & (Val(spReserve(i).recmode) + 1)
                                End If
                            Else
                                'webから返ってくるデータはsidが不明なので、該当局以外ののデータも含まれている
                                'log1write("【エラー】TvRock予約状況照合中に当てはまらないデータがありました。" & spReserve(i).station & "_" & spReserve(i).startt)
                            End If
                        Next
                    End If
                    '一巡してr()に対して判定がなされてなければ予約されていないとわかる
                    If r0_chk = 1 And r(0).reserve < 0 Then
                        r(0).reserve = 0
                    End If
                    For kk As Integer = 1 To r.Length - 1
                        If r(kk).reserve < 0 Then
                            r(kk).reserve = 0
                        End If
                    Next
                End If

                log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                log1write(log_temp)
            End If
        Catch ex As Exception
            Write("【エラー】TvRockからの放送局別番組表取得に失敗しました。" & ex.Message)
        End Try

        Return r
    End Function

    Private Function get_station_program_TvRock_reserve(ByVal p_sid As Integer) As spReservestructure()
        'fsid_startunixtime_endunixtimeの形式
        Dim r() As spReservestructure = Nothing

        Dim i, j As Integer
        Dim ut As Integer = time2unix(Now())

        If TvProgram_tvrock_sch.Length > 0 Then
            'tvrock.schから取得
            Dim html As String = file2str(TvProgram_tvrock_sch, "Shift_JIS")
            If html.Length > 100 Then
                Dim line() As String = Split(html, vbLf)
                Dim k As Integer = Int(line.Length / 43)
                If k > 0 Then
                    For i = 0 To k - 1
                        Dim startt As Integer = Val(Trim(line(i * 43 + 0).Replace(i.ToString & " START ", "")))
                        Dim endt As Integer = Val(Trim(line(i * 43 + 1).Replace(i.ToString & " END ", "")))
                        Dim sid As Integer = Val(Trim(line(i * 43 + 12).Replace(i.ToString & " SERVICEID ", "")))
                        Dim uid As String = Trim(line(i * 43 + 4).Replace(i.ToString & " UNIQID ", ""))
                        Dim validate As String = Trim(line(i * 43 + 16).Replace(i.ToString & " VALIDATE ", ""))
                        Dim station As String = Trim(line(i * 43 + 38).Replace(i.ToString & " STATION ", ""))
                        Dim title As String = Trim(line(i * 43 + 39).Replace(i.ToString & " TITLE ", ""))
                        If sid = p_sid Then
                            ReDim Preserve r(j)
                            r(j).startt = startt
                            r(j).endt = endt
                            r(j).sid = sid
                            r(j).yid = uid
                            r(j).recmode = validate
                            r(j).station = station
                            r(j).title = title
                            j += 1
                        End If
                    Next
                Else
                    log1write("tvrock.schからデータが読み込めませんでした。予約が0またはファイルが存在していない可能性があります")
                End If
            End If
        Else
            'WEBから取得
            '番組表読み込み
            Dim url As String = TvProgram_tvrock_url.Replace("/iphone", "/list")
            log1write("TvRock予約一覧を読み込みます。" & url)

            If url.Length > 0 Then
                'Dim html As String = get_html_by_WebBrowser(url, "UTF-8", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648)", 1)
                Dim html As String = get_html_by_webclient(url, "shift_jis", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648)")
                If html.Length > 100 Then
                    log1write("TvRock予約一覧を読み込みました")

                    If html.IndexOf("自動検索予約リスト") > 0 And html.IndexOf("予約日時") > 0 Then
                        Dim sp As Integer = html.IndexOf("<small>チャンネル</small>", 0)
                        sp = html.IndexOf("<a href=""list?", sp + 1)
                        Dim ep As Integer = html.IndexOf("<a href=""list?", sp + 1)
                        If ep < 0 Then
                            ep = html.Length - 1
                        End If

                        While sp >= 0 And ep > sp
                            Dim c_check As String = Instr_pickup(html, """>", "</a>", sp, ep) '○×
                            If c_check = "○" Then
                                c_check = "1"
                            ElseIf c_check = "×" Then
                                c_check = "0"
                            Else
                                c_check = "-1"
                            End If

                            Dim p_title As String = Instr_pickup(html, "<b>", "</b>", sp, ep)
                            Dim p_uid As String = Instr_pickup(html, "<a href=""list?i=", "&", sp, ep) '1498075405457
                            Dim p_station As String = Instr_pickup(html, "q=", """", sp, ep)
                            Dim cp1 As Integer = html.IndexOf(") ", sp)
                            cp1 = html.LastIndexOf(">", cp1 + 1)
                            If cp1 > sp And cp1 < ep Then
                                Dim date_str As String = ">" & Instr_pickup(html, ">", "<", cp1, ep) & "<"
                                '>1月26日(金) 23:00～23:30<
                                Dim mm As Integer = Val(Instr_pickup(date_str, ">", "月", 0))
                                Dim dd As Integer = Val(Instr_pickup(date_str, "月", "日", 0))
                                Dim hm1 As String = Instr_pickup(date_str, ") ", "～", 0)
                                Dim hm2 As String = Instr_pickup(date_str, "～", "<", 0)
                                Dim yyyy As String = Year(Now()).ToString
                                Dim d1_str As String = yyyy & "/" & mm & "/" & dd & " " & hm1
                                Dim d2_str As String = yyyy & "/" & mm & "/" & dd & " " & hm2
                                Dim d1t As DateTime
                                Dim d2t As DateTime
                                Dim d1 As Integer = 0
                                Dim d2 As Integer = 0
                                Try
                                    d1t = CDate(d1_str)
                                    d2t = CDate(d2_str)
                                    d1 = time2unix(d1t)
                                    d2 = time2unix(d2t)
                                    '年またぎを考慮
                                    If d1 < ut - (3600 * 24 * 30 * 6) Then
                                        '半年より前というおかしな日付なら1年足す
                                        d1t = DateAdd(DateInterval.Year, 1, d1t)
                                        d1 = time2unix(d1t)
                                    End If
                                    If d2 < ut - (3600 * 24 * 30 * 6) Then
                                        '半年より前というおかしな日付なら1年足す
                                        d2t = DateAdd(DateInterval.Year, 1, d2t)
                                        d2 = time2unix(d2t)
                                    End If
                                    While d2 < d1
                                        '日をまたいでいれば
                                        d2 += 3600 * 24
                                    End While
                                Catch ex As Exception
                                    log1write("【エラー】不正な日付です。" & p_title & " d1_str=" & d1_str & " d2_str=" & d2_str)
                                End Try
                                If Val(c_check) >= 0 And d1 > 0 And d2 > 0 And p_station.Length > 0 And p_uid.Length > 0 Then
                                    ReDim Preserve r(j)
                                    r(j).startt = d1
                                    r(j).endt = d2
                                    r(j).sid = 0 '不明
                                    r(j).yid = p_uid
                                    r(j).recmode = c_check
                                    r(j).station = p_station
                                    r(j).title = p_title
                                    j += 1
                                End If
                            End If

                            sp = html.IndexOf("<a href=""list?", sp + 1)
                            ep = html.IndexOf("<a href=""list?", sp + 1)
                            If ep < 0 Then
                                ep = html.Length - 1
                            End If
                        End While
                    Else
                        log1write("【エラー】TvRock予約一覧ページではありません")
                    End If
                Else
                    log1write("【エラー】TvRock予約一覧の読み込みに失敗しました")
                End If
            Else
                log1write("【エラー】TvRock番組表のURLが指定されていません")
            End If
        End If

        Return r
    End Function

    Public Function fix_time_d2u(ByVal str1 As String, ByVal str2 As String, ByVal future As Integer, ByRef plus_days As Integer) As Integer()
        '"H:dd"→現在の日付を付加 furure=1の場合は過去の日付は返さない
        'plus_daysはByRefで返す
        Dim r() As Integer = {0, 0}

        Dim s1 As String = ""
        Dim s2 As String = ""
        Dim t1 As DateTime
        Dim t2 As DateTime
        Dim ut1 As Integer = 0
        Dim ut2 As Integer = 0
        Dim nowt As DateTime = Now()
        Dim nowut As Integer = time2unix(nowt)
        s1 = Now().ToString("yyyy/MM/dd ") & str1 '本日の日付になっている
        s2 = Now().ToString("yyyy/MM/dd ") & str2 '本日の日付になっている
        Try
            t1 = CDate(s1)
            t2 = CDate(s2)
            If plus_days > 0 Then
                t1 = DateAdd(DateInterval.Day, plus_days, t1)
                t2 = DateAdd(DateInterval.Day, plus_days, t2)
            End If
            ut1 = time2unix(t1)
            ut2 = time2unix(t2)

            If future = 0 Then
                If ut1 > nowut + (12 * 60 * 60) Then
                    '0時過ぎで23:30の変換（昨日の23:30とみなす）
                    ut1 = ut1 - (24 * 60 * 60)
                End If
            End If

            While ut2 < ut1
                ut2 += (24 * 60 * 60)
                t2 = unix2time(ut2)
            End While

            plus_days = 0
            While nowt.Day <> t2.Day
                nowt = DateAdd(DateInterval.Day, 1, nowt)
                plus_days += 1
            End While

            r(0) = ut1
            r(1) = ut2
        Catch ex As Exception
            r(0) = 0
            r(1) = 0
        End Try

        Return r
    End Function

    Public Function get_station_program_ptTimer(ByVal p_sid As Integer, ByVal p_startt As Integer, ByVal p_endtt As Integer, ByVal p_station As String) As StationTVprogramstructure()
        'ptTimer番組表
        Dim r() As StationTVprogramstructure = Nothing

        Dim nextsec As Integer = p_endtt - p_startt

        If pttimer_pt2count > 0 Then
            Dim log_temp As String = "　　>>ptTimer放送局別番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

            'データベースから番組一覧を取得する
            Dim results As String = ""
            Dim psi As New System.Diagnostics.ProcessStartInfo()

            psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec") 'ComSpecのパスを取得する
            psi.RedirectStandardInput = False '出力を読み取れるようにする
            psi.RedirectStandardOutput = True
            psi.UseShellExecute = False
            psi.CreateNoWindow = True 'ウィンドウを表示しないようにする
            ''プログラムが存在するディレクトリ
            'Dim ppath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
            ''カレントディレクトリ変更
            'System.IO.Directory.SetCurrentDirectory(ppath)
            ''作業ディレクトリ変更
            'psi.WorkingDirectory = F_cut_lastyen(ppath)
            '出力エンコード
            psi.StandardOutputEncoding = Encoding.UTF8

            'カレントディレクトリ変更
            F_set_ppath4program()

            Dim pt2number As Integer
            For pt2number = 1 To pttimer_pt2count
                Dim msql As String = ""
                Dim db_name As String
                If pt2number <= 1 Then
                    db_name = ptTimer_path & "pt" & pt3Timer_str & "Timer.db"
                Else
                    db_name = ptTimer_path & "pt" & pt3Timer_str & "Timer-" & pt2number & ".db"
                End If

                Dim nowtime As DateTime = Now()
                Dim ut As Integer = time2unix(nowtime) '現在のunixtime

                If p_sid = 161 Then
                    Dim n_sid As Integer = 161
                    If p_station = "ＱＶＣ" Then
                        n_sid += 65536
                    End If
                    If nextsec = 0 Then
                        msql = """SELECT sid, eid, stime, length, title ,texts, gen1, '_BR_' as cr FROM t_event WHERE sid = " & n_sid & " AND ((stime <= " & ut & " AND (stime + length) > " & ut & ") OR stime >= " & ut & ") ORDER BY stime"""
                    Else
                        msql = """SELECT sid, eid, stime, length, title ,texts, gen1, '_BR_' as cr FROM t_event WHERE sid = " & n_sid & " AND ((stime <= " & ut & " AND (stime + length) > " & ut & ") OR (stime <= " & ut + nextsec & " AND stime > " & ut & ")) ORDER BY stime"""
                    End If
                Else
                    If nextsec = 0 Then
                        msql = """SELECT sid, eid, stime, length, title ,texts, gen1, '_BR_' as cr FROM t_event WHERE (sid = " & p_sid & " OR sid = " & (p_sid + 65536) & ") AND ((stime <= " & ut & " AND (stime + length) > " & ut & ") OR stime >= " & ut & ") ORDER BY stime"""
                    Else
                        msql = """SELECT sid, eid, stime, length, title ,texts, gen1, '_BR_' as cr FROM t_event WHERE (sid = " & p_sid & " OR sid = " & (p_sid + 65536) & ") AND ((stime <= " & ut & " AND (stime + length) > " & ut & ") OR (stime <= " & ut + nextsec & " AND stime > " & ut & ")) ORDER BY stime"""
                    End If
                End If
                psi.Arguments = "/c sqlite3.exe """ & db_name & """ -separator " & "//_//" & " " & msql

                Dim p As System.Diagnostics.Process
                Try
                    p = System.Diagnostics.Process.Start(psi)
                    '出力を読み取る
                    results = p.StandardOutput.ReadToEnd
                    'WaitForExitはReadToEndの後である必要がある
                    '(親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit()
                Catch ex As Exception
                    log1write("sqlite3.exe実行エラー[" & pt2number & "]")
                End Try

                If results Is Nothing Then results = ""
                log_temp &= " > 取得完了(" & results.Length.ToString & ")：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")

                '結果に数種類の改行が入っておりREPLACEが動作しないことへの対策 「, '_BR_' as cr」
                results = results.Replace(vbCrLf, " ").Replace(vbCr, " ")
                results = results.Replace("//_//_BR_ ", vbCrLf)

                '行ごとの配列として、テキストファイルの中身をすべて読み込む
                Dim line As String() = Split(results, vbCrLf) 'results.Split(vbCrLf)

                If line IsNot Nothing Then
                    If line.Length > 0 Then
                        Dim i As Integer = 0
                        Dim j As Integer
                        Dim last_sid As String = "::"
                        Dim skip_sid As String = ":"
                        For j = 0 To line.Length - 1
                            '番組表の数だけ繰り返す
                            Try
                                Dim youso() As String = Split(line(j), "//_//")
                                If youso.Length >= 7 Then
                                    '番組内容
                                    Dim sid As Integer = Val(youso(0))
                                    Dim chk161 As Integer = 0
                                    If sid = 65536 + 161 Then
                                        'QVC
                                        chk161 = 1
                                    End If
                                    If sid = 800 Then
                                        'なんだろうこのチャンネルは。800がスカチャン０と重なっている
                                        chk161 = 2
                                    End If
                                    If sid > 65536 Then
                                        'ptTimerはCSのサービスIDが+65536されている
                                        sid = sid - 65536
                                    End If
                                    If Array.IndexOf(ch_list, sid) >= 0 Then
                                        'ch_list()にsidが登録されていれば
                                        Dim eid As Integer = Val(youso(1))
                                        Dim title As String = escape_program_str(youso(4)) 'タイトル
                                        Dim texts As String = escape_program_str(youso(5)) '内容
                                        '開始時間
                                        Dim ystart As Integer = Val(youso(2))
                                        Dim ystartDate As DateTime = unix2time(Val(youso(2)))
                                        '終了時間
                                        Dim yend As Integer = Val(youso(2)) + Val(youso(3))
                                        Dim yendDate As DateTime = DateAdd(DateInterval.Second, Val(youso(3)), ystartDate)
                                        '放送局名
                                        Dim station As String
                                        station = sid2jigyousha(sid) 'BS-TBSとQVCが区別できない
                                        If chk161 = 1 Then
                                            station = "ＱＶＣ"
                                        End If
                                        If chk161 = 2 Then
                                            station = ""
                                        End If

                                        If station.Length > 0 And skip_sid.IndexOf(":" & sid.ToString & ":") < 0 Then
                                            '放送局名が見つかっていれば
                                            Dim chk As Integer = -1
                                            If r IsNot Nothing Then
                                                chk = Array.IndexOf(r, eid.ToString & "_" & ystart.ToString)
                                                If chk >= 0 Then
                                                    If r(chk).startt = ystart Then
                                                        '重複（同じsid,時刻）
                                                        chk = 1
                                                    Else
                                                        chk = -1
                                                    End If
                                                End If
                                            End If
                                            If chk < 0 Then
                                                '重複がなければ
                                                If r Is Nothing Then
                                                    ReDim Preserve r(0)
                                                Else
                                                    i = r.Length
                                                    ReDim Preserve r(i)
                                                End If
                                                ReDim Preserve r(j)
                                                r(i).name = station
                                                r(i).nameid = ""
                                                r(i).startt = ystart
                                                r(i).endt = yend
                                                r(i).title = title
                                                r(i).content = texts
                                                r(i).thumbnail = ""
                                                r(i).reserve = -1
                                                r(i).rsv_change = "" '現在対応不可能
                                                r(i).genre = Int(Val(youso(6)) / 16) * 256
                                                r(i).fsid_startt = eid.ToString & "_" & ystart.ToString

                                                last_sid &= sid.ToString & "::"
                                            End If
                                        End If
                                    End If
                                End If
                            Catch ex As Exception
                                log1write("ptTimer放送局別番組表取得中にエラーが発生しました。" & ex.Message)
                            End Try
                        Next
                    End If
                End If
            Next

            If r IsNot Nothing Then
                log_temp &= " > 予約状況解析開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                '予約状況を照らし合わせr()を修正
                Dim spReserve() As spReservestructure = get_station_program_ptTimer_reserve(p_sid)
                If spReserve IsNot Nothing Then
                    For i = 0 To spReserve.Length - 1
                        Dim ri As Integer = Array.IndexOf(r, spReserve(i).eid & "_" & spReserve(i).startt.ToString)
                        If ri >= 0 Then
                            r(ri).reserve = 1
                            r(ri).rsv_change = "" '現在対応不可能
                        End If
                    Next
                End If
                '一巡してr()に対して判定がなされてなければ予約されていないとわかる
                For kk As Integer = 0 To r.Length - 1
                    If r(kk).reserve < 0 Then
                        r(kk).reserve = 0
                        r(kk).rsv_change = "" '現在対応不可能
                    End If
                Next
            End If
            log_temp &= " > 解析完了：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
            log1write(log_temp)
        End If

        Return r
    End Function

    Public Function get_station_program_ptTimer_reserve(ByVal p_sid As Integer) As spReservestructure()
        'ptTimer番組表
        Dim r() As spReservestructure = Nothing

        If pttimer_pt2count > 0 Then
            'データベースから番組一覧を取得する
            Dim results As String = ""
            Dim psi As New System.Diagnostics.ProcessStartInfo()

            psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec") 'ComSpecのパスを取得する
            psi.RedirectStandardInput = False '出力を読み取れるようにする
            psi.RedirectStandardOutput = True
            psi.UseShellExecute = False
            psi.CreateNoWindow = True 'ウィンドウを表示しないようにする
            ''プログラムが存在するディレクトリ
            'Dim ppath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
            ''カレントディレクトリ変更
            'System.IO.Directory.SetCurrentDirectory(ppath)
            ''作業ディレクトリ変更
            'psi.WorkingDirectory = F_cut_lastyen(ppath)
            '出力エンコード
            psi.StandardOutputEncoding = Encoding.UTF8

            'カレントディレクトリ変更
            F_set_ppath4program()

            Dim pt2number As Integer
            For pt2number = 1 To pttimer_pt2count
                Dim msql As String = ""
                Dim db_name As String
                If pt2number <= 1 Then
                    db_name = ptTimer_path & "pt" & pt3Timer_str & "Timer.db"
                Else
                    db_name = ptTimer_path & "pt" & pt3Timer_str & "Timer-" & pt2number & ".db"
                End If

                Dim nowtime As DateTime = Now()
                Dim ut As Integer = time2unix(nowtime) '現在のunixtime

                msql = """SELECT sid, eid, stime, length FROM t_program WHERE (sid = " & p_sid & " OR sid = " & (p_sid + 65536) & ") AND ((stime <= " & ut & " AND (stime + length) > " & ut & ") OR stime >= " & ut & ") ORDER BY stime"""
                psi.Arguments = "/c sqlite3.exe """ & db_name & """ -separator " & "//_//" & " " & msql

                Dim p As System.Diagnostics.Process
                Try
                    p = System.Diagnostics.Process.Start(psi)
                    '出力を読み取る
                    results = p.StandardOutput.ReadToEnd
                    'WaitForExitはReadToEndの後である必要がある
                    '(親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit()
                Catch ex As Exception
                    log1write("sqlite3.exe実行エラー[" & pt2number & "]")
                End Try

                If results Is Nothing Then results = ""

                '行ごとの配列として、テキストファイルの中身をすべて読み込む
                Dim line As String() = Split(results, vbCrLf) 'results.Split(vbCrLf)

                If line IsNot Nothing Then
                    If line.Length > 0 Then
                        Dim i As Integer = 0
                        Dim j As Integer
                        For j = 0 To line.Length - 1
                            '番組表の数だけ繰り返す
                            Try
                                Dim youso() As String = Split(line(j), "//_//")
                                If youso.Length >= 4 Then
                                    '番組内容
                                    Dim sid As Integer = Val(youso(0))
                                    If sid > 65536 Then
                                        'ptTimerはCSのサービスIDが+65536されている
                                        sid = sid - 65536
                                    End If
                                    Dim eid As Integer = Val(youso(1))
                                    '開始時間
                                    Dim ystart As Integer = Val(youso(2))
                                    Dim ystartDate As DateTime = unix2time(Val(youso(2)))
                                    '終了時間
                                    Dim yend As Integer = Val(youso(2)) + Val(youso(3))
                                    Dim yendDate As DateTime = DateAdd(DateInterval.Second, Val(youso(3)), ystartDate)

                                    If r Is Nothing Then
                                        ReDim Preserve r(0)
                                    Else
                                        i = r.Length
                                        ReDim Preserve r(i)
                                    End If
                                    r(i).startt = ystart
                                    r(i).endt = yend
                                    r(i).sid = p_sid
                                    r(j).eid = eid
                                End If
                            Catch ex As Exception
                        log1write("ptTimer予約取得中にエラーが発生しました。" & ex.Message)
                    End Try
                        Next
                    End If
                End If
            Next

        End If

        Return r
    End Function

End Module
