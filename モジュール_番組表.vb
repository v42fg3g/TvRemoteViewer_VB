Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO

Module モジュール_番組表
    Public TvProgram_ch() As Integer '東京13等が複数配列で入る
    Public TvProgram_NGword() As String
    Public TvProgramEDCB_NGword() As String
    Public TvProgramTvRock_NGword() As String
    Public TvProgramEDCB_ignore() As String
    Public TvProgramD_channels() As String
    Public TvProgramEDCB_channels() As String
    Public TvProgramTvRock_channels() As String
    Public TvProgramD_sort() As String
    Public TvProgramD_BonDriver1st() As String
    Public TvProgramS_BonDriver1st() As String
    Public TvProgramP_BonDriver1st() As String
    Public TvProgram_tvrock_url As String = ""
    Public TvProgram_EDCB_url As String = ""

    Public LIVE_STREAM_STR As String = ""
    Public TvProgram_SelectUptoNum As Integer = 0 '番組表上の配信ナンバーを制限する

    Public TvProgramEDCB_premium As Integer = 0 'EDCB番組表がプレミアム用ならば1

    Public TvProgram_list() As TVprogramstructure
    Public Structure TVprogramstructure
        Public stationDispName As String
        Public ProgramInformation As String
        Public startDateTime As String
        Public endDateTime As String
        Public programTitle As String
        Public programContent As String
        Public sid As Integer
        Public tsid As Integer
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

        Dim use_num_bon() As String = get_use_BonDriver() 'd(0)=地デジnum d(1)=地デジBonDriver d(2)=BS・CS num d(3)=BS・CS BonDriver d(4)=プレミアム num d(5)=プレミアム BonDriver

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
                                    'NG処理
                                    Dim chk As Integer = 0
                                    If a = 0 Then
                                        'ネット番組表
                                        If TvProgram_NGword IsNot Nothing Then
                                            For j As Integer = 0 To TvProgram_NGword.Length - 1
                                                If StrConv(p.stationDispName, VbStrConv.Wide) = StrConv(TvProgram_NGword(j), VbStrConv.Wide) Then
                                                    chk = 1
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    ElseIf a = 998 Then
                                        'EDCBは番組表取得時にNG済
                                    ElseIf a = 999 Then
                                        'TvRock
                                        If TvProgramTvRock_NGword IsNot Nothing Then
                                            For j As Integer = 0 To TvProgramTvRock_NGword.Length - 1
                                                If StrConv(p.stationDispName, VbStrConv.Wide) = StrConv(TvProgramTvRock_NGword(j), VbStrConv.Wide) Then
                                                    chk = 1
                                                    Exit For
                                                End If
                                            Next
                                        End If
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
                                        Dim d() As String = bangumihyou2bondriver(p.stationDispName, a, p.sid, p.tsid)
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
                                            Dim selected_num As Integer = 1
                                            If Val(d(2)) >= SPHD_sid_start And Val(d(2)) <= SPHD_sid_end And use_num_bon(5).Length > 0 Then
                                                'プレミアム
                                                bhtml = bhtml.Replace(use_num_bon(5).ToLower & """>", use_num_bon(5) & """ selected>")
                                                selected_num = Val(use_num_bon(4))
                                            ElseIf Val(d(3)) = 1 And use_num_bon(3).Length > 0 Then
                                                'CSでBonDriver指定があれば
                                                bhtml = bhtml.Replace(use_num_bon(3).ToLower & """>", use_num_bon(3) & """ selected>")
                                                selected_num = Val(use_num_bon(2))
                                            ElseIf Val(d(2)) >= 1024 And use_num_bon(1).Length > 0 Then
                                                '地デジ
                                                bhtml = bhtml.Replace(use_num_bon(1).ToLower & """>", use_num_bon(1) & """ selected>")
                                                selected_num = Val(use_num_bon(0))
                                            ElseIf Val(d(2)) < 1024 And use_num_bon(3).Length > 0 Then
                                                'BS
                                                bhtml = bhtml.Replace(use_num_bon(3).ToLower & """>", use_num_bon(3) & """ selected>")
                                                selected_num = Val(use_num_bon(2))
                                            Else
                                                '指定無し
                                                bhtml = bhtml.Replace(d(1) & """>", d(1) & """ selected>")
                                                selected_num = 1
                                            End If

                                            html &= "<form action=""StartTV.html"">" & vbCrLf
                                            html &= "<select name=""num"">" & vbCrLf
                                            For ix = 1 To MAX_STREAM_NUMBER
                                                If ix = selected_num Then
                                                    html &= "<option selected>" & ix.ToString & "</option>" & vbCrLf
                                                Else
                                                    html &= "<option>" & ix.ToString & "</option>" & vbCrLf
                                                End If
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

    '使用されていない優先BonDriver名を返す（地デジ）
    Public Function get_use_BonDriver() As Object
        Dim r(5) As String
        r(0) = "" '地デジnum
        r(1) = "" '地デジBonDriver
        r(2) = "" 'BS・CSnum
        r(3) = "" 'BS・CSBonDriver
        r(4) = "" 'プレミアムnum
        r(5) = "" 'プレミアムBonDriver

        '地デジ
        If TvProgramD_BonDriver1st IsNot Nothing Then
            For i As Integer = 0 To TvProgramD_BonDriver1st.Length - 1
                If LIVE_STREAM_STR.IndexOf(TvProgramD_BonDriver1st(i)) < 0 Then
                    r(1) = TvProgramD_BonDriver1st(i)
                    Exit For
                End If
            Next
            If r(1) = "" Then
                r(1) = TvProgramD_BonDriver1st(0)
            End If
        End If

        'BS・CS
        If TvProgramS_BonDriver1st IsNot Nothing Then
            For i As Integer = 0 To TvProgramS_BonDriver1st.Length - 1
                If LIVE_STREAM_STR.IndexOf(TvProgramS_BonDriver1st(i)) < 0 Then
                    r(3) = TvProgramS_BonDriver1st(i)
                    Exit For
                End If
            Next
            If r(3) = "" Then
                r(3) = TvProgramS_BonDriver1st(0)
            End If
        End If

        'プレミアム
        If TvProgramP_BonDriver1st IsNot Nothing Then
            For i As Integer = 0 To TvProgramP_BonDriver1st.Length - 1
                If LIVE_STREAM_STR.IndexOf(TvProgramP_BonDriver1st(i)) < 0 Then
                    r(5) = TvProgramP_BonDriver1st(i)
                    Exit For
                End If
            Next
            If r(5) = "" Then
                r(5) = TvProgramP_BonDriver1st(0)
            End If
        End If

        'numを決める 'すでに使用中ならそのナンバー
        Dim line() As String = Split(LIVE_STREAM_STR, vbCrLf)
        Dim nums As String = ":"
        If line IsNot Nothing Then
            For i As Integer = 0 To line.Length - 1
                If line(i).IndexOf(r(1)) >= 0 Then
                    r(0) = i.ToString
                End If
                If line(i).IndexOf(r(3)) >= 0 Then
                    r(2) = i.ToString
                End If
                If line(i).IndexOf(r(5)) >= 0 Then
                    r(4) = i.ToString
                End If
                '配信中のナンバーを記録
                Dim d() As String = line(i).Split(",")
                If d.Length >= 12 Then
                    nums &= Trim(d(1)) & ":"
                End If
            Next
        End If

        '空いているナンバーを返す
        For i As Integer = 1 To MAX_STREAM_NUMBER
            If nums.IndexOf(":" & i.ToString & ":") < 0 Then
                If r(0) = "" Then
                    r(0) = i.ToString
                End If
                If r(2) = "" Then
                    r(2) = i.ToString
                End If
                If r(4) = "" Then
                    r(4) = i.ToString
                End If
            End If
        Next

        '空いているナンバーが無ければ1
        If r(0) = "" Then
            r(0) = "1"
        End If
        If r(2) = "" Then
            r(2) = "1"
        End If
        If r(4) = "" Then
            r(4) = "1"
        End If

        '番組表での配信ナンバー選択制限が指定されていれば
        If TvProgram_SelectUptoNum > 0 Then
            If Val(r(0)) > TvProgram_SelectUptoNum Then
                r(0) = TvProgram_SelectUptoNum.ToString
            End If
            If Val(r(2)) > TvProgram_SelectUptoNum Then
                r(2) = TvProgram_SelectUptoNum.ToString
            End If
            If Val(r(4)) > TvProgram_SelectUptoNum Then
                r(4) = TvProgram_SelectUptoNum.ToString
            End If
        End If

        Return r
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
                            Dim chk_j As Integer = 0
                            'プレミアム指定
                            If TvProgramEDCB_premium = 0 Then
                                'プレミアム指定されて無い場合はプレミアムのものは無視
                                If ch_list(i).sid >= SPHD_sid_start And ch_list(i).sid <= SPHD_sid_end Then
                                    chk_j = 1
                                End If
                            ElseIf TvProgramEDCB_premium = 1 Then
                                'プレミアム指定されている場合、sidがプレミアム範囲に無いものは無視
                                If ch_list(i).sid < SPHD_sid_start Or ch_list(i).sid > SPHD_sid_end Then
                                    chk_j = 1
                                End If
                            End If
                            'NGワードに指定されているものは無視
                            If chk_j = 0 Then
                                If TvProgramEDCB_NGword IsNot Nothing Then
                                    For j As Integer = 0 To TvProgramEDCB_NGword.Length - 1
                                        If StrConv(ch_list(i).jigyousha, VbStrConv.Wide) = StrConv(TvProgramEDCB_NGword(j), VbStrConv.Wide) Then
                                            chk_j = 1
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                            '番組情報を取得しないものは無視
                            If chk_j = 0 Then
                                If TvProgramEDCB_ignore IsNot Nothing Then
                                    For j As Integer = 0 To TvProgramEDCB_ignore.Length - 1
                                        If StrConv(ch_list(i).jigyousha, VbStrConv.Wide) = StrConv(TvProgramEDCB_ignore(j), VbStrConv.Wide) Then
                                            chk_j = 2
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If

                            If chk_j = 0 Then
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
                                            'sidを追加
                                            r(j).sid = sid
                                            r(j).tsid = tsid
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
                                    r(j).sid = ch_list(i).sid
                                    r(j).tsid = ch_list(i).tsid
                                End If
                            ElseIf chk_j = 2 Then
                                'ダミー
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
                                r(j).sid = ch_list(i).sid
                                r(j).tsid = ch_list(i).tsid
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
                    Dim sp2 As Integer = html.IndexOf(" <small><i>")
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

                        'サービスIDを取得
                        sp3 = html.IndexOf("javascript:reserv(", sp3)
                        r(j).sid = Val(Instr_pickup(html, ",", ",", sp3))

                        sp2 = html.IndexOf(" <small><i>", sp2 + 1)
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
                Dim url As String = "http://tv.so-net.ne.jp/rss/schedulesByCurrentTime.action?group=10&stationAreaId=" & regionID.ToString

                'ネットから地域の番組表を取得
                Dim wc As WebClient = New WebClient()
                Dim st As Stream = wc.OpenRead(url)
                Dim enc As Encoding = Encoding.GetEncoding("UTF-8")
                Dim sr As StreamReader = New StreamReader(st, enc)
                Dim html As String = sr.ReadToEnd()

                Dim hosokyoku As String = ""
                Dim programtitle As String = ""
                Dim jikoku1 As String = ""
                Dim jikoku2 As String = ""
                Dim sp As Integer = html.IndexOf("</channel>", 0)
                sp = html.IndexOf("<title>", sp + 1)
                While sp >= 0
                    programtitle = Instr_pickup(html, "<title>", "</title>", sp)
                    Dim dn As Integer = html.IndexOf("<description>", sp)
                    hosokyoku = Instr_pickup(html, "[", "(", dn)
                    jikoku1 = "1970/01/01 " & Instr_pickup(html, " ", "～", dn)
                    jikoku2 = "1970/01/01 " & Instr_pickup(html, "～", " [", dn)

                    If hosokyoku.Length > 0 Then
                        Dim j As Integer = 0
                        If r Is Nothing Then
                            j = 0
                        Else
                            j = r.Length
                        End If
                        ReDim Preserve r(j)
                        r(j).stationDispName = hosokyoku
                        r(j).startDateTime = jikoku1
                        r(j).endDateTime = jikoku2
                        r(j).programTitle = programtitle
                        'r(j).programSubTitle = CType(program("programSubTitle"), String) '空白
                        r(j).programContent = "" '空白
                        'r(j).programxplanation = CType(program("programExplanation"), String) '空白
                    End If

                    sp = html.IndexOf("<title>", sp + 1)
                End While
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
    Public Function bangumihyou2bondriver(ByVal hosokyoku As String, ByVal a As Integer, ByVal sid As Integer, ByVal tsid As Integer) As Object
        Dim r(3) As String
        r(0) = ""
        r(1) = ""
        r(2) = ""
        r(3) = ""
        hosokyoku = StrConv(hosokyoku, VbStrConv.Wide) '全角に変換
        Dim chk As Integer = 0
        Dim i As Integer = 0
        Dim cindex As Integer = -1

        If ch_list IsNot Nothing Then
            If sid > 0 And tsid > 0 Then
                'sidとtsidが指定されている場合（EDCB)
                For i = 0 To ch_list.Length - 1
                    If ch_list(i).sid = sid And ch_list(i).tsid = tsid Then
                        cindex = i
                        Exit For
                    End If
                Next
            ElseIf sid = 161 Then
                'sid=161は重なっている　BS-TBSかQVCか局名でもチェック
                For i = 0 To ch_list.Length - 1
                    Dim h2 As String = StrConv(ch_list(i).jigyousha, VbStrConv.Wide)
                    If ch_list(i).sid = 161 And hosokyoku = h2 Then
                        cindex = i
                        Exit For
                    End If
                Next
            ElseIf sid > 0 Then
                'sidが指定されていれば（主にTvRock)
                For i = 0 To ch_list.Length - 1
                    If sid = ch_list(i).sid Then
                        '一致した
                        cindex = i
                        Exit For
                    End If
                Next
            Else
                'sidからではなく従来通りチャンネル名で判断する場合
                Dim h2 As String = rename_hosokyoku2jigyousha(hosokyoku, a) 'iniでチャンネル名変換が指定されていれば
                If h2.Length > 0 Then
                    'きっちり変換が指定されていた場合
                    hosokyoku = h2
                    chk = 1
                Else
                    '推測
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

                For i = 0 To ch_list.Length - 1
                    Dim h As String
                    h = StrConv(ch_list(i).jigyousha, VbStrConv.Wide) '全角に変換
                    If chk = 1 Then
                        '変換した場合は完全一致でないとＮＧ
                        If h = hosokyoku Then
                            '一致した
                            cindex = i
                            Exit For
                        End If
                    Else
                        '推測の場合は部分一致おｋ
                        If h.IndexOf(hosokyoku) >= 0 Then
                            cindex = i
                            Exit For
                        End If
                    End If
                Next
            End If

            If cindex >= 0 Then
                r(0) = ch_list(i).jigyousha
                r(1) = ch_list(i).bondriver
                r(2) = ch_list(i).sid.ToString
                r(3) = ch_list(i).chspace.ToString
            End If
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

    'WEBインターフェース用　データを整形して返す
    Public Function program_translate4WI(ByVal a As Integer) As String
        'a=0 通常のインターネットから取得　 a=998 EDCBから取得　 a=999 tvrockから取得
        Dim chkstr As String = ":" '重複防止用
        Dim html_all As String = ""
        Dim cnt As Integer = 0
        Dim TvProgram_html() As TVprogram_html_structure = Nothing

        'ＮＧワード文字列
        Dim ngword As String = ":"
        If TvProgram_NGword IsNot Nothing Then
            For i = 0 To TvProgram_NGword.Length - 1
                ngword &= StrConv(TvProgram_NGword(i), VbStrConv.Wide) & ":"
            Next
        End If

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
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then '重複していないかチェック
                                    Dim chk As Integer = 0
                                    If ngword.IndexOf(":" & StrConv(p.stationDispName, VbStrConv.Wide) & ":") >= 0 Then
                                        chk = 1
                                    End If

                                    'NG処理
                                    If chk = 0 Then
                                        If a = 0 Then
                                            'ネット番組表
                                            If TvProgram_NGword IsNot Nothing Then
                                                For j As Integer = 0 To TvProgram_NGword.Length - 1
                                                    If StrConv(p.stationDispName, VbStrConv.Wide) = StrConv(TvProgram_NGword(j), VbStrConv.Wide) Then
                                                        chk = 1
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                        ElseIf a = 998 Then
                                            'EDCBは番組表取得時にNG済
                                        ElseIf a = 999 Then
                                            'TvRock
                                            If TvProgramTvRock_NGword IsNot Nothing Then
                                                For j As Integer = 0 To TvProgramTvRock_NGword.Length - 1
                                                    If StrConv(p.stationDispName, VbStrConv.Wide) = StrConv(TvProgramTvRock_NGword(j), VbStrConv.Wide) Then
                                                        chk = 1
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                        End If
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
                                        Dim d() As String = bangumihyou2bondriver(p.stationDispName, a, p.sid, p.tsid)
                                        'd(0) = jigyousha d(1) = bondriver d(2) = sid d(3) = chspace

                                        If d(0).Length > 0 Then
                                            hosokyoku = StrConv(d(0), VbStrConv.Wide)
                                        End If

                                        'html &= d(0) & "," & p.stationDispName & "," & d(2) & "," & d(3) & "," & Trim(startt) & "," & Trim(endt) & "," & p.programTitle & "," & p.programContent & vbCrLf
                                        html &= d(0) & "," & p.stationDispName & "," & d(2) & "," & d(3) & "," & Trim(startt) & "," & Trim(endt) & "," & escape_program_str(p.programTitle) & "," & escape_program_str(p.programContent) & vbCrLf

                                        chkstr &= s4 & ":"
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

    '番組表で使えない文字をエスケープ
    Public Function escape_program_str(ByVal s As String) As String
        Dim r As String = ""
        s = s.Replace(",", "，")
        s = s.Replace("<", "＜")
        s = s.Replace(">", "＞")
        '念のため
        s = s.Replace("&lt;", "＜")
        s = s.Replace("&gt;", "＞")
        Return s
    End Function

End Module
