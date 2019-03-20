Module モジュール_番組表_ジャンル指定
    Public Function WI_GET_1GENRE_PROGRAM(ByVal temp_str As String) As String
        Dim t As DateTime = Now()
        If Second(t) >= 55 Then
            '時計がずれている場合に対処
            t = DateAdd(DateInterval.Second, 60 - Second(t), t)
        End If
        Dim ut As Integer = time2unix(t)
        Dim i As Integer = 0
        'temp_str                取得元録画ソフト名,ジャンル(0～15),開始時間,終了時間,force
        Dim src() As String = Nothing
        Dim genre As Integer = -2
        Dim station_name As String = ""
        Dim startt As Integer = 0
        Dim endt As Integer = 0
        Dim force As Integer = 0
        Dim r As String = ""
        Dim d() As String = temp_str.Split(",")
        If d.Length >= 2 Then
            For i = 0 To d.Length - 1
                d(i) = filename_escape_recall(Trim(d(i)))
            Next
            src = Trim(d(0)).Split("_")
            If IsNumeric(d(1)) Then
                genre = Val(d(1))
            End If
            If d.Length >= 4 Then
                startt = Val(Trim(d(2)))
                If startt = 0 Then
                    startt = ut
                End If
                endt = Val(Trim(d(3)))
                If endt = 0 Then
                    endt = startt + (60 * 60 * 6) '6時間
                End If
            Else
                startt = ut
                endt = startt + (60 * 60 * 6) '6時間
            End If
            If d.Length >= 5 Then
                force = Val(d(4))
            End If
        End If

        '番組データ　　　　　start,end,title,content,放送局名,nameid,sid,tsid,reserve,reserve_change_data
        If genre >= -1 Then
            Dim ut1 As Integer = 0
            Dim ut2 As Integer = 0
            Dim str As String = ""
            Dim value As String = ""
            For Each src1 As String In src
                '放送局
                Dim p() As StationTVprogramstructure = Nothing
                Select Case src1.ToLower
                    Case "abematv"
                        p = get_1genre_program_AbemaTV(genre, startt, endt)
                End Select
                '文字列に整形
                If p IsNot Nothing Then
                    For i = 0 To p.Length - 1
                        If p(i).startt > 0 Then
                            If ut1 = 0 Then ut1 = p(i).startt
                            If ut2 = 0 Then ut2 = p(i).endt
                            r &= p(i).startt & "," _
                                & p(i).endt & "," _
                                & filename_escape_set(p(i).title) & "," _
                                & filename_escape_set(p(i).content) & "," _
                                & p(i).name & "," _
                                & p(i).nameid & "," _
                                & p(i).sid.ToString & "," _
                                & p(i).tsid.ToString & "," _
                                & p(i).reserve & "," _
                                & filename_escape_set(p(i).rsv_change) & vbCrLf
                        End If
                    Next
                End If
            Next
        End If

        Return r
    End Function

    Private Function get_1genre_program_AbemaTV(ByVal genre As Integer, ByVal startt As Integer, ByVal endt As Integer) As StationTVprogramstructure()
        'genre=0～15
        Dim r() As StationTVprogramstructure = Nothing
        Try
            'Outside番組表を取得
            Dim html As String = get_Outside_html(0)
            If Outside_CustomURL_method = 1 Then
                '都合の良いデータ形式の場合
                If html.Length >= 300 Then
                    Dim log_temp As String = "　　>>AbemaTVジャンル指定番組表 取得開始：" & Now().ToString("ss") & "." & Now().Millisecond.ToString("d3")
                    Try
                        Dim line() As String = Split(html, vbCrLf)
                        Dim temp() As String = line(0).Split(",")
                        If temp.Length > 100 Then
                            'うまく分割できていない可能性
                            line = Split(html, vbLf) 'unix
                        End If
                        Dim chk_inTime As Integer = 0
                        For i = 0 To line.Length - 1
                            Dim d() As String = line(i).Split(",")
                            If d.Length >= 6 Then
                                For i2 As Integer = 0 To d.Length - 1
                                    d(i2) = Trim(d(i2))
                                Next
                                Dim g1 As Integer = Int(Val(d(7)) / 256)
                                If (g1 = genre Or (genre = 15 And (g1 = 2 Or g1 >= 8))) And ((Val(d(2)) < startt And Val(d(3)) >= startt) Or (Val(d(2)) < endt And Val(d(2)) >= startt)) Then
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
                                    r(j).sid = 99999801
                                    r(j).tsid = 0
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
                                        r(j).genre = "-1"
                                    End If
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
            log1write(Outside_StationName & "ジャンル指定番組表取得に失敗しました。" & ex.Message)
        End Try

        Return r
    End Function
End Module
