Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO

Module モジュール_番組表
    Public TvProgram_ch() As Integer '東京13等が複数配列で入る
    Public TvProgram_NGword() As String
    Public TvProgramD_channels() As String
    Public TvProgramEDCB_channels() As String
    Public TvProgramTvRock_channels() As String
    Public TvProgramD_sort() As String
    Public TvProgramD_BonDriver1st As String = ""
    Public TvProgramS_BonDriver1st As String = ""
    Public TvProgram_tvrock_url As String = ""
    Public TvProgram_EDCB_url As String = ""

    Public TvProgram_list() As TVprogramstructure
    Public Structure TVprogramstructure
        Public stationDispName As String
        Public ProgramInformation As String
        Public startDateTime As String
        Public endDateTime As String
        Public programTitle As String
        Public programContent As String
    End Structure

    Public Structure TVprogram_html_structure
        Public stationDispName As String
        Public hosokyoku As String
        Public html As String
        Public done As Integer
    End Structure

    '地デジ番組表作成
    Public Function make_TVprogram_html_now(ByVal a As Integer, ByVal NHKMODE As Integer) As String
        'a=0 通常のインターネットから取得　 a=998 EDCBから取得　 a=999 tvrockから取得
        Dim chkstr As String = ":" '重複防止用
        Dim html_all As String = ""
        Dim cnt As Integer = 0
        Dim TvProgram_html() As TVprogram_html_structure = Nothing

        Dim TvProgram_ch2() As Integer = Nothing
        If a = 0 Then
            '通常のインターネットから取得
            TvProgram_ch2 = TvProgram_ch
        ElseIf a = 998 Then
            'EDCBから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 998
        ElseIf a = 999 Then
            'tvrockから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 999
        End If

        If TvProgram_ch2 IsNot Nothing Then
            For i As Integer = 0 To TvProgram_ch2.Length - 1
                If TvProgram_ch2(i) > 0 Then
                    Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch2(i))
                    If program IsNot Nothing Then
                        For Each p As TVprogramstructure In program
                            Dim html As String = ""
                            Dim hosokyoku As String = ""
                            Dim s4 As String = StrConv(p.stationDispName, VbStrConv.Wide) 'p.stationDispName
                            '最初の4文字で重複チェック　→廃止
                            'If s4.Length > 4 Then
                            's4 = s4.Substring(0, 4)
                            'End If
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then '重複していないかチェック
                                    Dim chk As Integer = 0
                                    If TvProgram_NGword IsNot Nothing Then
                                        For j As Integer = 0 To TvProgram_NGword.Length - 1
                                            If StrConv(p.stationDispName, VbStrConv.Wide) = StrConv(TvProgram_NGword(j), VbStrConv.Wide) Then
                                                chk = 1
                                            End If
                                        Next
                                    End If

                                    If chk = 0 Then
                                        Dim startt As String = ""
                                        Dim endt As String = ""
                                        Try
                                            '時：分だけ取り出す
                                            If p.startDateTime = "2038/01/01 23:59" Then
                                                startt = ""
                                            Else
                                                startt = p.startDateTime.Substring(p.startDateTime.IndexOf(" "))
                                            End If
                                            If p.endDateTime = "2038/01/01 23:59" Then
                                                endt = ""
                                            Else
                                                endt = p.endDateTime.Substring(p.endDateTime.IndexOf(" "))
                                            End If
                                        Catch ex As Exception
                                        End Try

                                        'BonDriver, sid, 事業者を取得
                                        Dim d() As String = bangumihyou2bondriver(p.stationDispName, a)
                                        'd(0) = jigyousha d(1) = bondriver d(2) = sid d(3) = chspace

                                        html &= "<span class=""p_name"">" & d(0) & "　<span class=""p_name2"">(" & p.stationDispName & ")</span></span><br>" & vbCrLf 'p.stationDispName
                                        If startt.Length > 0 Then
                                            html &= "<span class=""p_time"">" & startt & " ～" & endt & "</span><br>"
                                        Else
                                            html &= "<span class=""p_time"">　</span><br>"
                                        End If
                                        If p.programTitle.Length > 0 Or p.programContent.Length > 0 Then
                                            html &= "<span class=""p_title"">" & p.programTitle & "</span><br>" & vbCrLf
                                            html &= "<span class=""p_content"">" & p.programContent & "</span><br>" & vbCrLf
                                        Else
                                            '放送されていない
                                            html &= "<span class=""p_title"">" & "　" & "</span><br>" & vbCrLf
                                        End If
                                        'html &= "<br>" & vbCrLf
                                        chkstr &= s4 & ":"

                                        If d(0).Length > 0 Then
                                            hosokyoku = StrConv(d(0), VbStrConv.Wide)
                                            '該当があれば選択済みにする
                                            Dim bhtml As String = ""
                                            Dim atag(3) As String 'ダミー
                                            'bhtml = WEB_make_select_Bondriver_html(atag)
                                            bhtml = html_selectbonsidch_a 'BonDriver一覧セレクトhtml
                                            bhtml = bhtml.Replace("<script type=""text/javascript"" src=""ConnectedSelect.js""></script>", "") '余計な1行を消す
                                            bhtml = bhtml.Replace(" id=""SEL1""", "") '余計な文字を消す
                                            '優先的に割り当てるBonDriverが指定されていればそれを選択
                                            If Val(d(3)) = 1 And TvProgramS_BonDriver1st.Length > 0 Then
                                                'CSでBonDriver指定があれば
                                                bhtml = bhtml.Replace(TvProgramS_BonDriver1st.ToLower & """>", TvProgramS_BonDriver1st & """ selected>")
                                            ElseIf Val(d(2)) >= 1024 And TvProgramD_BonDriver1st.Length > 0 Then
                                                '地デジ
                                                bhtml = bhtml.Replace(TvProgramD_BonDriver1st.ToLower & """>", TvProgramD_BonDriver1st & """ selected>")
                                            ElseIf Val(d(2)) < 1024 And TvProgramS_BonDriver1st.Length > 0 Then
                                                'BS
                                                bhtml = bhtml.Replace(TvProgramS_BonDriver1st.ToLower & """>", TvProgramS_BonDriver1st & """ selected>")
                                            Else
                                                '指定無し
                                                bhtml = bhtml.Replace(d(1) & """>", d(1) & """ selected>")
                                            End If

                                            html &= "<form action=""StartTV.html"">" & vbCrLf
                                            html &= "<select name=""num"">" & vbCrLf
                                            For ix = 1 To MAX_STREAM_NUMBER
                                                html &= "<option>" & ix.ToString & "</option>" & vbCrLf
                                            Next
                                            html &= "</select>" & vbCrLf
                                            html &= bhtml & vbCrLf
                                            html &= "%SELECTRESOLUTION%" & vbCrLf '解像度選択
                                            html &= "<input type=""hidden"" name=""ServiceID"" value=""" & d(2) & """>" & vbCrLf
                                            html &= "<input type=""hidden"" name=""ChSpace"" value=""" & d(3) & """>" & vbCrLf
                                            html &= "<span class=""p_hosokyoku""> " & d(0) & " </span>" & vbCrLf
                                            'NHK音声選択
                                            If hosokyoku.IndexOf("ＮＨＫ") >= 0 Then
                                                If NHKMODE = 3 Then
                                                    html &= WEB_make_NHKMODE_html_B()
                                                ElseIf NHKMODE >= 0 Then
                                                    html &= "<input type=""hidden"" name=""NHKMODE"" value=""" & NHKMODE & """>" & vbCrLf
                                                End If
                                            End If
                                            html &= "<input type=""submit"" value=""視聴"">" & vbCrLf
                                            html &= "</form>" & vbCrLf
                                        End If
                                        html &= "<br><br>" & vbCrLf
                                    End If
                                End If
                            End If

                            ReDim Preserve TvProgram_html(cnt)
                            TvProgram_html(cnt).stationDispName = StrConv(p.stationDispName, VbStrConv.Wide)
                            TvProgram_html(cnt).hosokyoku = hosokyoku
                            TvProgram_html(cnt).html = html
                            TvProgram_html(cnt).done = 0
                            cnt += 1
                        Next
                    End If
                End If
            Next
        End If

        '並び替え
        If TvProgramD_sort IsNot Nothing Then
            For i As Integer = 0 To TvProgramD_sort.Length - 1
                If TvProgram_html IsNot Nothing Then
                    For j As Integer = 0 To TvProgram_html.Length - 1
                        If TvProgram_html(j).done = 0 Then
                            If TvProgram_html(j).stationDispName = TvProgramD_sort(i) Or TvProgram_html(j).hosokyoku = TvProgramD_sort(i) Then
                                html_all &= TvProgram_html(j).html
                                TvProgram_html(j).done = 1
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        End If
        '並び替え指定が無いものを追加
        If TvProgram_html IsNot Nothing Then
            For j As Integer = 0 To TvProgram_html.Length - 1
                If TvProgram_html(j).done = 0 Then
                    html_all &= TvProgram_html(j).html
                    TvProgram_html(j).done = 1
                End If
            Next
        End If

        Return html_all
    End Function

    'ＮＨＫ音声選択用セレクト作成
    Public Function WEB_make_NHKMODE_html_B() As String
        Dim html As String = ""
        html &= "<select name=""NHKMODE"">"
        html &= vbCrLf & "<option value=""0"">主・副</option>" & vbCrLf
        html &= "<option value=""1"">主</option>" & vbCrLf
        html &= "<option value=""2"">副</option>" & vbCrLf
        If BS1_hlsApp.Length > 0 Then
            html &= "<option value=""9"">VLCで再生</option>" & vbCrLf
        End If
        html &= "</select>" & vbCrLf

        Return html
    End Function

    '地域番号から番組表を取得
    Public Function get_TVprogram_now(ByVal regionID As Integer) As Object
        Dim r() As TVprogramstructure = Nothing
        If regionID = 998 Then
            'EDCB
            Try
                If TvProgram_EDCB_url.Length > 0 Then
                    If ch_list IsNot Nothing Then
                        For i As Integer = 0 To ch_list.Length - 1
                            Dim wc As WebClient = New WebClient()
                            Dim st_add As String = ""
                            If TvProgram_EDCB_url.IndexOf("?") < 0 Then
                                st_add = "?"
                            Else
                                st_add = "&"
                            End If
                            Dim st_str As String = TvProgram_EDCB_url & st_add & "SID=" & ch_list(i).sid.ToString
                            st_str = st_str & "&TSID=" & ch_list(i).tsid.ToString
                            Dim st As Stream = wc.OpenRead(st_str)
                            Dim enc As Encoding = Encoding.GetEncoding("UTF-8")
                            Dim sr As StreamReader = New StreamReader(st, enc)
                            Dim html As String = sr.ReadToEnd()

                            Dim sp As Integer = html.IndexOf("<eventinfo>")
                            Dim ep As Integer = html.IndexOf("</eventinfo>", sp + 1)
                            Dim chk As Integer = 0
                            While sp >= 0 And ep > sp
                                Try
                                    'まず現在時刻にあてはまるかチェック
                                    Dim t As DateTime = Now()
                                    Dim t1date As String = Instr_pickup(html, "<startDate>", "</startDate>", sp, ep)
                                    Dim t1time As String = Instr_pickup(html, "<startTime>", "</startTime>", sp, ep)
                                    Dim t1long As Integer = Int(Val(Instr_pickup(html, "<duration>", "</duration>", sp, ep)) / 60) '分
                                    Dim t1 As DateTime = CDate(t1date & " " & t1time)
                                    Dim t2 As DateTime = DateAdd(DateInterval.Minute, t1long, t1)
                                    Dim t1s As String = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                    Dim t2s As String = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))

                                    If t >= t1 And t < t2 Then
                                        Dim j As Integer = 0
                                        If r Is Nothing Then
                                            j = 0
                                        Else
                                            j = r.Length
                                        End If
                                        ReDim Preserve r(j)
                                        Dim sid As Integer = Val(Instr_pickup(html, "<SID>", "</SID>", sp, ep))
                                        Dim tsid As Integer = Val(Instr_pickup(html, "<TSID>", "</TSID>", sp, ep))
                                        r(j).stationDispName = sid2jigyousha(sid, tsid)
                                        r(j).startDateTime = t1s
                                        r(j).endDateTime = t2s
                                        r(j).programTitle = Instr_pickup(html, "<event_name>", "</event_name>", sp, ep)
                                        r(j).programContent = Instr_pickup(html, "<event_text>", "</event_text>", sp, ep)
                                        '1個みつかればおｋ
                                        chk = 1
                                        Exit While
                                    End If
                                Catch ex As Exception
                                End Try

                                sp = html.IndexOf("<eventinfo>", sp + 1)
                                ep = html.IndexOf("</eventinfo>", sp + 1)
                            End While

                            sr.Close()
                            st.Close()

                            If chk = 0 Then
                                '該当時間帯の番組が無かった場合
                                Dim j As Integer = 0
                                If r Is Nothing Then
                                    j = 0
                                Else
                                    j = r.Length
                                End If
                                ReDim Preserve r(j)
                                r(j).stationDispName = ch_list(i).jigyousha
                                r(j).startDateTime = ""
                                r(j).endDateTime = ""
                                r(j).programTitle = ""
                                r(j).programContent = ""
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                log1write("EDCBからの番組表取得に失敗しました。" & ex.Message)
            End Try

        ElseIf regionID = 999 Then
            'TvRock
            Try
                If TvProgram_tvrock_url.Length > 0 Then
                    Dim wc As WebClient = New WebClient()
                    Dim st As Stream = wc.OpenRead(TvProgram_tvrock_url)
                    Dim enc As Encoding = Encoding.GetEncoding("Shift_JIS")
                    Dim sr As StreamReader = New StreamReader(st, enc)
                    Dim html As String = sr.ReadToEnd()

                    '<small>ＮＨＫＢＳ１ <small><i> のようになっている
                    Dim sp2 As Integer = html.IndexOf("<small><i>")
                    Dim sp As Integer = html.LastIndexOf("><small>", sp2 + 1)
                    While sp > 0
                        Dim j As Integer = 0
                        If r Is Nothing Then
                            j = 0
                        Else
                            j = r.Length
                        End If
                        ReDim Preserve r(j)
                        r(j).stationDispName = Instr_pickup(html, "<small>", " <small>", sp)
                        If r(j).stationDispName.LastIndexOf(")""><small>") > 0 Then
                            '番組表データが無いチャンネルが混じっていることがある
                            r(j).stationDispName = r(j).stationDispName.Substring(r(j).stationDispName.IndexOf(")""><small>") + ")""><small>".Length)
                        End If
                        r(j).stationDispName = Trim(r(j).stationDispName)
                        r(j).stationDispName = Trim(delete_tag(r(j).stationDispName))
                        r(j).startDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "<small><i>", "～", sp)))
                        r(j).endDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "～", "</i></small>", sp)))
                        r(j).programTitle = Trim(delete_tag(Instr_pickup(html, "<small><b>", "</b></small>", sp)))
                        Dim sp3 As Integer = html.IndexOf("</b></small>", sp)
                        sp3 = html.IndexOf("<font color=", sp3)
                        r(j).programContent = Trim(delete_tag(Instr_pickup(html, ">", "</font>", sp3)))

                        sp2 = html.IndexOf("<small><i>", sp2 + 1)
                        sp = html.LastIndexOf("><small>", sp2 + 1)
                    End While

                    sr.Close()
                    st.Close()

                End If
            Catch ex As Exception
                log1write("TvRockからの番組表取得に失敗しました。" & ex.Message)
            End Try
        ElseIf regionID > 0 Then
            'ネットから地域の番組表を取得
            Try
                Dim sTargetUrl As String = "http://miruzow-cloud.nifty.com/tv-program/now/" & regionID.ToString & ".jsonp"
                Dim objWeb As WebClient = New WebClient()
                Dim objSrializer As JavaScriptSerializer = New JavaScriptSerializer()
                Dim objEncode As Encoding = Encoding.UTF8
                Dim bResult As Byte() = objWeb.DownloadData(sTargetUrl)
                Dim sJson As String = objEncode.GetString(bResult)
                Dim objHash As Hashtable = objSrializer.Deserialize(Of Hashtable)(sJson)

                Dim response As Dictionary(Of String, Object) = objHash("response")
                Dim result As Dictionary(Of String, Object) = response("result")
                Dim StationLocation() As Object = result("dtt")

                For Each station As Dictionary(Of String, Object) In StationLocation
                    Dim program As Dictionary(Of String, Object) = station("now")

                    Dim j As Integer = 0
                    If r Is Nothing Then
                        j = 0
                    Else
                        j = r.Length
                    End If
                    ReDim Preserve r(j)
                    r(j).stationDispName = CType(station("station"), String)
                    r(j).startDateTime = get_time_from_at(CType(program("at_start"), String))
                    r(j).endDateTime = get_time_from_at(CType(program("at_end"), String))
                    r(j).programTitle = CType(program("title"), String)
                    r(j).programContent = CType(program("detail"), String)

                    'Dim program_next As Dictionary(Of String, Object) = station("now")
                Next

            Catch ex As Exception
                log1write("インターネットからの番組表取得に失敗しました。" & ex.Message)
            End Try
        End If

        Return r
    End Function

    '時刻を取得する
    Public Function get_time_from_at(ByVal s As String) As String
        '2014-10-04 12:00:00 +0900
        Dim sp As Integer = s.IndexOf(" ")
        Dim ep As Integer = s.LastIndexOf(":")
        s = s.Substring(0, ep).Replace("-", "/")
        Return s
    End Function

    '文字列内のhtmlタグを消去
    Public Function delete_tag(ByVal s As String)
        'タイトル内のタグを削除
        Dim k1 As Integer = s.IndexOf("<")
        Dim k2 As Integer = s.IndexOf(">", k1 + 1)
        While k1 >= 0 And k2 > k1
            Dim s2 As String = Instr_pickup(s, "<", ">", 0)
            s = s.Replace("<" & s2 & ">", "")
            k1 = s.IndexOf("<")
            k2 = s.IndexOf(">", k1 + 1)
        End While

        Return s
    End Function

    '番組表の放送局名からbondriver,sid等の取得を試みる
    Public Function bangumihyou2bondriver(ByVal hosokyoku As String, ByVal a As Integer) As Object
        Dim r(3) As String
        hosokyoku = StrConv(hosokyoku, VbStrConv.Wide) '全角に変換
        Debug.Print("hosokyoku=" & hosokyoku)
        Dim h2 As String = rename_hosokyoku2jigyousha(hosokyoku, a)
        Debug.Print("henkango=" & h2)
        Dim chk As Integer = 0
        If h2.Length > 0 Then
            hosokyoku = h2
            chk = 1
        Else
            Dim sp1 As Integer
            sp1 = hosokyoku.IndexOf("テレビ")
            If sp1 > 0 Then
                If hosokyoku.Substring(sp1) = "テレビ" Then
                    '末尾が"テレビ"なら
                    hosokyoku = hosokyoku.Replace("テレビ", "")
                End If
            End If
            sp1 = hosokyoku.IndexOf("放送")
            If sp1 > 0 Then
                If hosokyoku.Substring(sp1) = "放送" Then
                    '末尾が"放送"なら
                    hosokyoku = hosokyoku.Replace("放送", "")
                End If
            End If
            Dim sp As Integer = hosokyoku.IndexOf("　")
            If sp > 0 Then
                Try
                    hosokyoku = hosokyoku.Substring(sp + 1)
                Catch ex As Exception
                End Try
            End If
        End If

        r(0) = ""
        r(1) = ""
        r(2) = ""
        r(3) = ""
        Dim i As Integer = 0
        If ch_list IsNot Nothing Then
            For i = 0 To ch_list.Length - 1
                Dim h As String
                h = StrConv(ch_list(i).jigyousha, VbStrConv.Wide) '全角に変換
                If chk = 1 Then
                    '変換した場合は完全一致でないとＮＧ
                    If h = hosokyoku Then
                        '一致した
                        r(0) = ch_list(i).jigyousha
                        r(1) = ch_list(i).bondriver
                        r(2) = ch_list(i).sid.ToString
                        r(3) = ch_list(i).chspace.ToString
                        Exit For
                    End If
                Else
                    '推測の場合は部分一致おｋ
                    If h.IndexOf(hosokyoku) >= 0 Then
                        r(0) = ch_list(i).jigyousha
                        r(1) = ch_list(i).bondriver
                        r(2) = ch_list(i).sid.ToString
                        r(3) = ch_list(i).chspace.ToString
                        Exit For
                    End If
                End If
            Next
        End If

        Return r
    End Function

    '番組表の放送局からBonDriver.ch2に登録の放送局名へ変換
    Public Function rename_hosokyoku2jigyousha(ByVal hosokyoku As String, ByVal a As Integer) As String
        Dim r As String = ""
        Dim chs() As String = Nothing
        If a = 0 Then
            chs = TvProgramD_channels
        ElseIf a = 998 Then
            chs = TvProgramEDCB_channels
        ElseIf a = 999 Then
            chs = TvProgramTvRock_channels
        End If
        If chs IsNot Nothing Then
            For i As Integer = 0 To chs.Length - 1
                Dim sp As Integer = chs(i).IndexOf("：")
                If chs(i).IndexOf(hosokyoku & "：") = 0 Then
                    Try
                        r = chs(i).Substring(sp + 1)
                    Catch ex As Exception
                    End Try
                    Exit For
                End If
            Next
        End If

        Return r
    End Function

    '文字列から、文字列と文字列に挟まれた文字列を抽出する。
    Public Function Instr_pickup(ByRef strdat As String, ByVal findstr As String, ByVal endstr As String, ByVal startpos As Integer, Optional ByVal endpos As Integer = 2147483647) As Object
        Dim r As String = ""

        Try
            Dim sp As Integer
            Dim ep As Integer
            sp = strdat.IndexOf(findstr, startpos)
            ep = strdat.IndexOf(endstr, sp + findstr.Length)
            If sp >= 0 And ep > sp And ep <= endpos Then
                r = strdat.Substring(sp + findstr.Length, ep - sp - findstr.Length)
            End If
        Catch ex As Exception
        End Try

        Return r
    End Function

    'sidから放送局名を取得する
    Public Function sid2jigyousha(ByVal sid As Integer, Optional tsid As Integer = 0) As String
        Dim r As String = ""

        If sid > 0 Then
            Dim i As Integer = 0
            If ch_list IsNot Nothing Then
                For i = 0 To ch_list.Length - 1
                    If tsid = 0 Then
                        If sid = ch_list(i).sid Then
                            '一致した
                            r = ch_list(i).jigyousha
                            Exit For
                        End If
                    Else
                        If sid = ch_list(i).sid And tsid = ch_list(i).tsid Then
                            '一致した
                            r = ch_list(i).jigyousha
                            Exit For
                        End If
                    End If
                Next
            End If
        End If

        Return r
    End Function

End Module
