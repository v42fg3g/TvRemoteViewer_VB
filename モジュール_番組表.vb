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
    'ptTimer
    Public ptTimer_path As String = "" 'pttimerのパス　末尾\
    Public pttimer_pt2count As Integer = 0
    Public TvProgramptTimer_NGword() As String

    Public LIVE_STREAM_STR As String = ""
    Public TvProgram_SelectUptoNum As Integer = 0 '番組表上の配信ナンバーを制限する

    Public TvProgramEDCB_premium As Integer = 0 'EDCB番組表がプレミアム用ならば1

    '次番組を表示する分数デフォルト(7分に設定）
    Public nextmin_default As Integer = 7

    'EDCBの/addprogres.htmlでEDCB管理チャンネルを抽出しない場合=1
    Public EDCB_thru_addprogres As Integer = 0

    'EDCBがVelmy,niisaka版の場合は1
    Public EDCB_Velmy_niisaka As Integer = 0

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
        Public nextFlag As Integer '次の番組なら1
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.stationDispName = pF Then '放送局名と一致するか
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    'EDCB番組表で表示する放送局
    Public EDCB_TSID() As Integer
    Public EDCB_SID() As Integer

    Public Structure TVprogram_html_structure
        Public stationDispName As String
        Public hosokyoku As String
        Public html As String
        Public done As Integer
    End Structure

    '地デジ番組表作成
    Public Function make_TVprogram_html_now(ByVal a As Integer, ByVal NHKMODE As Integer) As String
        'a=0 通常のインターネットから取得　 a=997 ptTimerから取得　 a=998 EDCBから取得　 a=999 tvrockから取得
        Dim chkstr As String = ":" '重複防止用
        Dim html_all As String = ""
        Dim cnt As Integer = 0
        Dim TvProgram_html() As TVprogram_html_structure = Nothing

        Dim use_num_bon() As String = get_use_BonDriver() 'd(0)=地デジnum d(1)=地デジBonDriver d(2)=BS・CS num d(3)=BS・CS BonDriver d(4)=プレミアム num d(5)=プレミアム BonDriver

        Dim TvProgram_ch2() As Integer = Nothing
        If a = 0 Then
            '通常のインターネットから取得
            TvProgram_ch2 = TvProgram_ch
        ElseIf a = 997 Then
            'ptTimerから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 997
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
                    'Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch2(i))
                    Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch2(i), nextmin_default)
                    If program IsNot Nothing Then
                        For Each p As TVprogramstructure In program
                            Dim html As String = ""
                            Dim hosokyoku As String = ""
                            Dim s4 As String = StrConv(p.stationDispName, VbStrConv.Wide) & p.sid 'p.stationDispName
                            '最初の4文字で重複チェック　→廃止
                            'If s4.Length > 4 Then
                            's4 = s4.Substring(0, 4)
                            'End If
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then '重複していないかチェック
                                    If p.nextFlag = 0 Then
                                        'NG処理
                                        Dim chk As Integer = 0
                                        If a = 0 Then
                                            'ネット番組表
                                            chk = isMATCHhosokyoku(TvProgram_NGword, p.stationDispName)
                                        ElseIf a = 997 Then
                                            'ptTimer
                                            chk = isMATCHhosokyoku(TvProgramptTimer_NGword, p.stationDispName, p.sid)
                                        ElseIf a = 998 Then
                                            'EDCBは番組表取得時にNG済
                                        ElseIf a = 999 Then
                                            'TvRock
                                            chk = isMATCHhosokyoku(TvProgramTvRock_NGword, p.stationDispName, p.sid)
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
                                                html &= "<span class=""p_title"">" & escape_program_str(p.programTitle) & "</span><br>" & vbCrLf
                                                html &= "<span class=""p_content"">" & escape_program_str(p.programContent) & "</span><br>" & vbCrLf
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
    Public Function get_TVprogram_now(ByVal regionID As Integer, Optional ByVal getnext As Integer = 0) As Object
        Dim r() As TVprogramstructure = Nothing
        If regionID = 997 Then
            'ptTimer
            '次番組表には未対応
            r = get_ptTimer_program(getnext)
        ElseIf regionID = 998 Then
            'EDCB
            r = get_EDCB_program(getnext)
            'r = get_EDCB_program_old(getnext) '旧方式
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

                        '次の番組を取得
                        If getnext > 0 Then
                            sp = html.IndexOf("><small><i>", sp2 + 1)
                            Dim se As Integer = html.IndexOf(" <small><i>", sp2 + 1)
                            If sp > 0 And sp < se Then
                                '次の番組があれば
                                j = r.Length
                                ReDim Preserve r(j)
                                r(j).stationDispName = r(j - 1).stationDispName
                                r(j).startDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "<small><i>", "～", sp)))
                                r(j).endDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "～", "</i></small>", sp)))
                                r(j).programTitle = Trim(delete_tag(Instr_pickup(html, "<small><small><small>", "</small></small></small>", sp)))
                                r(j).programContent = ""
                                r(j).sid = r(j - 1).sid

                                '次の番組であることを記録
                                r(j).nextFlag = 1
                            End If
                        End If

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

        '終了までgetnext分以内なら詳細に次の番組情報を転写
        If getnext >= 4 Then
            Dim uptonext As Integer = getnext
            Dim t As DateTime = Now() '現在時刻
            Dim ts As Integer = Hour(t) * 100 + Minute(t) '現在時刻4桁の分秒
            Dim tend As DateTime = DateAdd(DateInterval.Minute, uptonext, t) 'この時刻を越えていたら次の番組を詳細に転写
            Dim te As Integer = Hour(tend) * 100 + Minute(tend) 'この時刻を越えていたら次の番組を詳細に転写　4桁分秒
            If te < ts Then
                '日付またぎしていれば
                te += 2400
            End If
            If r IsNot Nothing Then
                For i = 0 To r.Length - 2
                    If r(i).nextFlag = 0 Then
                        Try
                            Dim tr As DateTime = CDate(r(i).endDateTime)
                            Dim re As Integer = Hour(tr) * 100 + Minute(tr) '番組終了時間　4桁分秒
                            If re < ts Then
                                '日付またぎしていれば
                                re += 2400
                            End If
                            If te >= re Then
                                '次番組があれば
                                If r(i + 1).nextFlag = 1 And r(i).sid = r(i + 1).sid Then
                                    r(i).programContent = "[Next] " & Trim(r(i + 1).startDateTime.Substring(r(i + 1).startDateTime.IndexOf(" "))) & " ～ " & r(i + 1).programTitle
                                End If
                            End If
                        Catch ex As Exception
                            'r(i).endDateTimeが空白だったり不正の可能性有り
                        End Try
                    End If
                Next
                '余計な要素を削除
                'For i = r.Length - 1 To 0 Step -1
                'If r(i).nextFlag = 1 Then
                ''要素を削除
                'title_ArrayRemove(r, i)
                'End If
                'Next
                '↓このほうが速そう
                Dim k As Integer = 0
                Dim r2() As TVprogramstructure = Nothing
                For i = 0 To r.Length - 1
                    If r(i).nextFlag = 0 Then
                        ReDim Preserve r2(k)
                        r2(k) = r(i)
                        k += 1
                    End If
                Next
                Return r2
            End If
        End If

        Return r
    End Function

    '配列から要素を削除　■未使用
    Public Sub title_ArrayRemove(ByRef TargetArray As TVprogramstructure(), ByVal deleteIndex As Integer)
        '削除する要素＋１～の内容 → 削除する要素～にコピー
        Array.Copy(TargetArray, deleteIndex + 1, TargetArray, deleteIndex, TargetArray.Length - deleteIndex - 1)
        '最終行を削除する
        ReDim Preserve TargetArray(TargetArray.Length - 2)
    End Sub

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
    Public Function program_translate4WI(ByVal a As Integer, Optional ByVal getnext As Integer = 0) As String
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
        ElseIf a = 997 Then
            'ptTimerから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 997
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
                    Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch2(i), getnext)
                    If program IsNot Nothing Then
                        For Each p As TVprogramstructure In program
                            Dim html As String = ""
                            Dim hosokyoku As String = ""
                            Dim s4 As String = StrConv(p.stationDispName, VbStrConv.Wide) & p.sid & "." & p.nextFlag 'p.stationDispName
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then '重複していないかチェック
                                    If p.nextFlag = 0 Or (p.nextFlag = 1 And getnext > 0) Then
                                        Dim chk As Integer = 0
                                        If ngword.IndexOf(":" & StrConv(p.stationDispName, VbStrConv.Wide) & ":") >= 0 Then
                                            chk = 1
                                        End If

                                        'NG処理
                                        If chk = 0 Then
                                            If a = 0 Then
                                                'ネット番組表
                                                chk = isMATCHhosokyoku(TvProgram_NGword, p.stationDispName)
                                            ElseIf a = 997 Then
                                                'ptTimer
                                                chk = isMATCHhosokyoku(TvProgramptTimer_NGword, p.stationDispName, p.sid)
                                            ElseIf a = 998 Then
                                                'EDCBは番組表取得時にNG済
                                            ElseIf a = 999 Then
                                                'TvRock
                                                chk = isMATCHhosokyoku(TvProgramTvRock_NGword, p.stationDispName, p.sid)
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
                                            If getnext = 2 And p.nextFlag = 1 Then
                                                p.programTitle = "[Next]" & p.programTitle
                                            End If
                                            html &= d(0) & "," & p.stationDispName & "," & d(2) & "," & d(3) & "," & Trim(startt) & "," & Trim(endt) & "," & escape_program_str(p.programTitle) & "," & escape_program_str(p.programContent)
                                            If getnext = 3 Then
                                                html &= "," & p.nextFlag
                                            End If
                                            html &= vbCrLf

                                            chkstr &= s4 & ":"
                                        End If
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
                    Dim chk As Integer = 0
                    For j As Integer = 0 To TvProgram_html.Length - 1
                        If TvProgram_html(j).done = 0 Then
                            If TvProgram_html(j).stationDispName = TvProgramD_sort(i) Or TvProgram_html(j).hosokyoku = TvProgramD_sort(i) Then
                                html_all &= TvProgram_html(j).html
                                TvProgram_html(j).done = 1
                                If getnext = 0 Or chk > 0 Then
                                    '重複可能性が無ければExit For
                                    '次の番組が取得されている場合は放送局が2回出てくる可能性がある
                                    Exit For
                                End If
                                chk += 1
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
        '念のため
        s = s.Replace("&lt;", "＜")
        s = s.Replace("&gt;", "＞")
        s = s.Replace("&amp;", "＆")
        'エスケープするべき文字
        s = s.Replace(",", "，")
        s = s.Replace("<", "＜")
        s = s.Replace(">", "＞")
        s = s.Replace("&", "＆")
        '改行をエスケープ
        s = s.Replace(vbCrLf, " ")
        s = s.Replace(vbLf, " ")
        s = s.Replace(vbCr, " ")
        'trim
        s = Trim(s)
        Return s
    End Function

    'ngword()に局がマッチしているかチェック
    Public Function isMATCHhosokyoku(ByVal ngwords As Object, ByVal name As String, Optional ByVal sid As Integer = 0) As Integer
        'ngword()に指定されていれば1を返す
        Dim r As Integer = -1
        name = StrConv(name, VbStrConv.Wide) '全角に
        Dim sidstr As String = StrConv(sid.ToString, VbStrConv.Wide) '全角に
        If ngwords IsNot Nothing Then
            If name.Length > 0 Then
                r = Array.IndexOf(ngwords, name) '局名で指定
            End If
            If r < 0 And sidstr <> "０" Then
                r = Array.IndexOf(ngwords, sidstr) 'サービスIDで指定
            End If
        End If
        If r >= 0 Then
            r = 1
        Else
            r = 0
        End If

        'Dim r As Integer = -1
        'Dim i As Integer = 0
        'If ngwords IsNot Nothing Then
        'For j As Integer = 0 To ngwords.Length - 1
        'If IsNumeric(ngwords(j)) = True And sid > 0 Then
        ''サービスIDで指定
        'If sid = Val(ngwords(j)) Then
        'r = 1
        'Exit For
        'End If
        'Else
        ''局名で指定
        'If StrConv(name, VbStrConv.Wide) = StrConv(ngwords(j), VbStrConv.Wide) Then
        'r = 1
        'Exit For
        'End If
        'End If
        'Next
        'End If

        Return r
    End Function

    'EDCBが管理し番組表で表示するTSIDを取得する
    Public Sub EDCB_GET_TSID()
        If TvProgram_EDCB_url.Length > 0 Then
            Dim url As String = TvProgram_EDCB_url
            Dim sp As Integer = TvProgram_EDCB_url.IndexOf("://")
            If sp > 0 Then
                sp = TvProgram_EDCB_url.IndexOf("/", sp + 3)
            End If
            If sp > 0 Then
                '番組予約URL
                url = TvProgram_EDCB_url.Substring(0, sp) & "/addprogres.html"
                'log1write(url & " からEDCB番組表に表示する局を取得します")

                Try
                    Dim wc As WebClient = New WebClient()
                    Dim st As Stream = wc.OpenRead(url)
                    Dim enc As Encoding = Encoding.GetEncoding("Shift_JIS")
                    Dim sr As StreamReader = New StreamReader(st, enc)
                    Dim html As String = sr.ReadToEnd()

                    Dim i As Integer = 0
                    sp = html.IndexOf("serviceID")
                    Dim ep As Integer = html.IndexOf("</select>")
                    If sp > 0 Then
                        Dim d_tsid As Integer = 0
                        Dim d_sid As Integer = 8
                        Dim s_tsid As Integer = 1
                        Dim s_sid As Integer = 5

                        'Velmy版かどうかチェック
                        '通常                   Velmy
                        'x[TSID][SID]           x[SID][TSID]
                        '[TSID][SID][TSID]      [TSID][TSID][SID]
                        Dim Velmy_chk As Integer = 0
                        If EDCB_Velmy_niisaka = 1 Then
                            'Velmy版が指定されている
                            Velmy_chk = 1
                            log1write("【EDCB】Velmy,niisaka版の指定がありました")
                        ElseIf html.IndexOf("17186504945") > 0 Then
                            'Velmy版のBS1文字列が存在した
                            Velmy_chk = 1
                            log1write("【EDCB】BS文字列からVelmy,niisaka版であると判断しました")
                        Else
                            ''冒頭のsidで判断（これは確実ではなさそうなので却下）
                            'Dim test_hex As String = Hex(Val(Instr_pickup(html, "option value=""", """", sp, ep)))
                            'Dim test_sid As Integer = 0
                            'Dim test_tsid As Integer = 0
                            'If test_hex.Length = 12 Then
                            'test_sid = h16_10("0x" & Val(test_hex.Substring(4, 4)))
                            'ElseIf test_hex.Length = 9 Then
                            'test_sid = h16_10("0x" & Val(test_hex.Substring(1, 4)))
                            'End If
                            'If test_sid > 0 Then
                            'If Array.IndexOf(ch_list, test_sid) >= 0 Then
                            ''ch_list()にVelmy版sidが見つかった場合はVelmy,niisaka版と判断
                            'Velmy_chk = 1
                            'log1write("【EDCB】サービスIDからVelmy,niisaka版であると判断しました")
                            'End If
                            'End If
                        End If
                        If Velmy_chk = 1 Then
                            'Velmy版
                            d_tsid = 0
                            d_sid = 4
                            s_tsid = 5
                            s_sid = 1
                        End If

                        While sp > 0
                            Dim dex1 As String = Instr_pickup(html, "option value=""", """", sp, ep)
                            Dim tsid_long As String = Hex(dex1) '16進数に変換
                            Dim tsid_hex As String = ""
                            Dim tsid As Integer = 0
                            Dim sid_hex As String = ""
                            Dim sid As Integer = 0

                            If tsid_long.Length = 12 Then
                                '地デジ
                                tsid_hex = tsid_long.Substring(d_tsid, 4) '初めの4文字
                                tsid = h16_10("0x" & tsid_hex)
                                sid_hex = tsid_long.Substring(d_sid, 4) '最後の4文字
                                sid = h16_10("0x" & sid_hex)
                            ElseIf tsid_long.Length = 9 Then
                                'BS/CS
                                sid_hex = tsid_long.Substring(s_sid, 4) '最後の4文字
                                sid = h16_10("0x" & sid_hex)
                                If sid < SPHD_sid_start Or sid > SPHD_sid_end Then
                                    'SPHDでなければTSIDを記録
                                    tsid_hex = tsid_long.Substring(s_tsid, 4) '初めの4文字
                                    tsid = h16_10("0x" & tsid_hex)
                                Else
                                    'SPHDの場合、TSIDはあてにならないみたいなのでTSIDは記録しない
                                End If
                            Else
                                '未知の形式
                                log1write("【エラー】EDCB番組表 " & tsid_long & " は未知のTSID/SID形式です")
                            End If

                            If sid > 0 Then
                                'ch_listに存在するTSIDならばリストに追加
                                If i = 0 Then
                                    ReDim Preserve EDCB_TSID(i)
                                    EDCB_TSID(i) = tsid
                                    ReDim Preserve EDCB_SID(i)
                                    EDCB_SID(i) = sid
                                    i += 1
                                Else
                                    Dim si As Integer = Array.IndexOf(EDCB_SID, sid)
                                    If si < 0 Then
                                        ReDim Preserve EDCB_TSID(i)
                                        EDCB_TSID(i) = tsid
                                        ReDim Preserve EDCB_SID(i)
                                        EDCB_SID(i) = sid
                                        i += 1
                                    Else
                                        '重複有り
                                        If EDCB_TSID(si) <> tsid Then
                                            'TSIDが違えば登録
                                            ReDim Preserve EDCB_TSID(i)
                                            EDCB_TSID(i) = tsid
                                            ReDim Preserve EDCB_SID(i)
                                            EDCB_SID(i) = sid
                                            i += 1
                                        End If
                                    End If
                                End If
                            End If

                            sp = html.IndexOf("option", sp + 1)
                            If sp > ep Then
                                sp = -1
                            End If
                        End While
                    End If
                Catch ex As Exception
                    log1write("【エラー】" & url & " の取得に失敗しました。" & ex.Message)
                End Try
                '最後にチェック
                If EDCB_TSID IsNot Nothing Then
                    log1write(url & " からEDCB番組表に表示する局を取得しました")
                Else
                    log1write("【エラー】EDCBからEDCB番組表に表示する局の取得に失敗しました")
                    EDCB_thru_addprogres = 1
                    log1write("EDCB番組表に全チャンネルを表示するようにセットしました")
                End If
            Else
                log1write("【エラー】" & TvProgram_EDCB_url & " の指定が不正です")
            End If
        End If
    End Sub

    ''指定したTSIDがEDCB_SID()内に存在すればindexを返す 存在しなければ-1
    Public Function check_TSID_in_EDCBprogram(ByVal tsid As Integer, ByVal sid As Integer) As Integer
        Dim r = -1

        If sid > 0 Then
            If EDCB_SID IsNot Nothing Then
                For i As Integer = 0 To EDCB_SID.Length - 1
                    If tsid = EDCB_TSID(i) And sid = EDCB_SID(i) Then
                        r = i
                        Exit For
                    End If
                Next
                If r < 0 Then
                    '完全一致がなければSIDのみ一致を探す
                    For i As Integer = 0 To EDCB_SID.Length - 1
                        If sid = EDCB_SID(i) Then
                            r = i
                            Exit For
                        End If
                    Next
                End If
            End If
        End If

        Return r
    End Function

    '指定したTSIDがch_list()内に存在すればindexを返す 存在しなければ-1 '■未使用
    Public Function check_TSID_in_chlist(ByVal tsid As Integer, ByVal sid As Integer) As Integer
        Dim r = -1
        If sid > 0 Then
            If ch_list IsNot Nothing Then
                For i As Integer = 0 To ch_list.Length - 1
                    If tsid = ch_list(i).tsid And sid = ch_list(i).sid Then
                        r = i
                        Exit For
                    End If
                Next
                If r < 0 Then
                    '完全一致がなければSIDのみ一致を探す
                    For i As Integer = 0 To ch_list.Length - 1
                        If sid = ch_list(i).sid Then
                            r = i
                            Exit For
                        End If
                    Next
                End If
            End If
        End If
        Return r
    End Function

    'ptTimer番組表
    Public Function get_ptTimer_program(ByVal getnext As Integer) As Object
        Dim r() As TVprogramstructure = Nothing

        Dim nextsec As Integer = 0
        If getnext >= 4 Then
            nextsec = getnext * 60
        ElseIf getnext > 0 Then
            '1～3のときは3時間にしておく
            nextsec = 180 * 60
        End If

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

            Dim pt2number As Integer
            For pt2number = 1 To pttimer_pt2count
                Dim msql As String = ""
                Dim db_name As String
                If pt2number <= 1 Then
                    db_name = ptTimer_path & "ptTimer.db"
                Else
                    db_name = ptTimer_path & "ptTimer-" & pt2number & ".db"
                End If

                Dim nowtime As DateTime = Now()
                Dim ut As Integer = time2unix(nowtime) '現在のunixtime

                If nextsec = 0 Then
                    msql = """SELECT sid, eid, stime, length, title, texts FROM t_event WHERE stime <= " & ut & " AND (stime + length) > " & ut & " ORDER BY sid"""
                Else
                    msql = """SELECT sid, eid, stime, length, title, texts FROM t_event WHERE (stime <= " & ut & " AND (stime + length) > " & ut & ") OR (stime <= " & ut + nextsec & " AND stime > " & ut & ") ORDER BY sid,stime"""
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

                '行ごとの配列として、テキストファイルの中身をすべて読み込む
                Dim line As String() = Split(results, vbCrLf) 'results.Split(vbCrLf)

                If line IsNot Nothing Then
                    If line.Length > 0 Then
                        Dim i As Integer = 0
                        Dim j As Integer
                        Dim last_sid As Integer = 0
                        Dim skip_sid As Integer = 0
                        For j = 0 To line.Length - 1
                            '番組表の数だけ繰り返す
                            Try
                                Dim youso() As String = Split(line(j), "//_//")
                                If youso.Length >= 6 Then
                                    '番組内容
                                    Dim sid As Integer = Val(youso(0))
                                    Dim chk161 As Integer = 0
                                    If sid = 65536 + 161 Then
                                        'QSV
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
                                        Dim ystart_time As String = ystartDate.ToString("HH:mm")
                                        '終了時間
                                        Dim yend As Integer = Val(youso(2)) + Val(youso(3))
                                        Dim yendDate As DateTime = DateAdd(DateInterval.Second, Val(youso(3)), ystartDate)
                                        Dim yend_time As String = yendDate.ToString("HH:mm")

                                        '放送局名
                                        Dim station As String
                                        station = sid2jigyousha(sid) 'BS-TBSとQVCが区別できない
                                        If chk161 = 1 Then
                                            station = "ＱＶＣ"
                                        End If
                                        If chk161 = 2 Then
                                            station = ""
                                        End If

                                        If station.Length > 0 And sid <> skip_sid Then
                                            '放送局名が見つかっていれば
                                            Dim chk As Integer = -1
                                            If r IsNot Nothing Then
                                                chk = Array.IndexOf(r, sid)
                                                If chk >= 0 Then
                                                    If r(chk).startDateTime = "1970/01/01 " & ystart_time And r(chk).programTitle = title Then
                                                        '重複（同じ時刻、同じタイトル）
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
                                                r(i).sid = sid
                                                r(i).tsid = 0
                                                r(i).stationDispName = station
                                                Dim t1s As String = "1970/01/01 " & ystart_time
                                                Dim t2s As String = "1970/01/01 " & yend_time
                                                r(i).startDateTime = t1s
                                                r(i).endDateTime = t2s
                                                r(i).programTitle = title
                                                r(i).programContent = texts
                                                '次番組かどうかチェック
                                                If sid = last_sid Then
                                                    '2回目の場合は次番組であろう
                                                    r(i).nextFlag = 1
                                                    skip_sid = sid '3回目以降はスキップするように
                                                End If
                                                last_sid = sid
                                            End If
                                        End If
                                    End If
                                End If
                            Catch ex As Exception
                                log1write("ptTimer番組表取得中にエラーが発生しました。" & ex.Message)
                            End Try
                        Next
                    End If
                End If
            Next
        End If

        Return r
    End Function

    Public Function F_get_pt2count() As Integer
        '何枚PT2が存在するかチェック
        Dim r As Integer = 0

        'よく考えたら番組表は複数枚チェックする必要は無さそう
        '不具合があるときは↓6行を削除
        If file_exist(ptTimer_path & "ptTimer.db") > 0 Then
            r = 1
            log1write("ptTimerのデータベースを認識しました")
        Else
            log1write("【エラー】ptTimerのデータベースを認識できませんでした")
            ptTimer_path = ""
        End If
        Return r
        Exit Function

        '以下、複数のデータベースを調べる場合　■未使用

        If file_exist(ptTimer_path & "ptTimer.db") > 0 Then
            r = 1
            Dim i As Integer = 2
            While file_exist(ptTimer_path & "ptTimer-" & i & ".db") > 0
                r += 1
                i += 1
            End While
        End If
        If r = 1 Then
            log1write("ptTimerのデータベースを認識しました")
        ElseIf r > 1 Then
            log1write("ptTimerのデータベースを" & r & "つ認識しました")
        Else
            log1write("【エラー】ptTimerのデータベースを認識できませんでした")
            ptTimer_path = ""
        End If

        Return r
    End Function

    'EDCB番組表
    Public EDCB_cmd As New CtrlCmdCLI.CtrlCmdUtil
    Public Function get_EDCB_program(ByVal getnext As Integer) As Object
        Dim r() As TVprogramstructure = Nothing

        If TvProgram_EDCB_url.Length > 0 Then
            If ch_list IsNot Nothing Then
                Dim i As Integer = 0
                Dim k As Integer = 0

                Dim ip As String = Instr_pickup(TvProgram_EDCB_url, "://", ":", 0)
                ip = host2ip(ip) 'ホストネームからIPに変換
                Dim sp As Integer = TvProgram_EDCB_url.IndexOf("://")
                Dim port As String = ""
                'If sp > 0 Then
                'port = Instr_pickup(TvProgram_EDCB_url, ":", "/", sp + 3)
                'End If
                port = 4510 'CtrlCmdCLIのポート　決め打ち？

                If ip.Length > 0 And Val(port) > 0 Then
                    EDCB_cmd.SetSendMode(True)
                    EDCB_cmd.SetNWSetting(ip, port)
                    Dim epgList As New System.Collections.Generic.List(Of CtrlCmdCLI.Def.EpgServiceEventInfo)()
                    Dim ret As Integer = EDCB_cmd.SendEnumPgAll(epgList) 'IPやportがおかしいとここで止まる可能性有り
                    If ret = 1 Then
                        For i = 0 To ch_list.Length - 1
                            Dim kc As Integer = -1
                            For k = 0 To epgList.Count - 1
                                If epgList(k).serviceInfo.SID = ch_list(i).sid And epgList(k).serviceInfo.TSID = ch_list(i).tsid Then
                                    'TSID&SID一致
                                    kc = k
                                    Exit For
                                ElseIf epgList(k).serviceInfo.SID = ch_list(i).sid And StrConv(epgList(k).serviceInfo.service_name, VbStrConv.Wide) = StrConv(ch_list(i).jigyousha, VbStrConv.Wide) Then
                                    'SIDと放送局名が一致
                                    kc = k
                                    Exit For
                                ElseIf epgList(k).serviceInfo.SID = ch_list(i).sid Then
                                    'SIDが一致
                                    kc = k
                                    Exit For
                                End If
                            Next

                            If kc >= 0 Then
                                Dim info As CtrlCmdCLI.Def.EpgServiceEventInfo = epgList(kc)

                                Dim chk_j As Integer = 0

                                Dim tsid As Integer = info.serviceInfo.TSID
                                Dim sid As Integer = info.serviceInfo.SID
                                Dim jigyousha As String = info.serviceInfo.service_name

                                'EDCB番組表に存在しているかチェック
                                If EDCB_thru_addprogres = 0 Then
                                    Dim ct As Integer = check_TSID_in_EDCBprogram(tsid, sid)
                                    If ct < 0 Then
                                        '存在していない
                                        chk_j = 1
                                    End If
                                End If

                                'プレミアム指定（1.16からは指定しなくてもOK）
                                If chk_j = 0 Then
                                    If TvProgramEDCB_premium = 1 Then
                                        'プレミアム指定されている場合、プレミアム以外は無視
                                        If sid < SPHD_sid_start Or sid > SPHD_sid_end Then
                                            chk_j = 1
                                        End If
                                    ElseIf TvProgramEDCB_premium = 2 Then
                                        'プレミアムを表示しないよう指定されている場合はプレミアムは無視
                                        If sid >= SPHD_sid_start And sid <= SPHD_sid_end Then
                                            chk_j = 1
                                        End If
                                    End If
                                End If
                                'NGワードに指定されているものは無視
                                If chk_j = 0 Then
                                    chk_j = isMATCHhosokyoku(TvProgramEDCB_NGword, jigyousha, sid)
                                End If
                                '番組情報を取得しないものは無視
                                If chk_j = 0 Then
                                    chk_j = isMATCHhosokyoku(TvProgramEDCB_ignore, jigyousha, sid)
                                End If

                                If chk_j = 0 Then
                                    Dim chk As Integer = 0
                                    Dim t2next As DateTime = CDate("1970/01/01 9:00:00")
                                    For k = 0 To info.eventList.Count - 1
                                        Try
                                            'まず現在時刻にあてはまるかチェック
                                            Dim t As DateTime = Now()
                                            Dim t1long As Integer = Int(info.eventList.Item(k).durationSec / 60) '分
                                            Dim t1 As DateTime = info.eventList.Item(k).start_time
                                            Dim t2 As DateTime = DateAdd(DateInterval.Minute, t1long, t1)
                                            Dim t1s As String = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                            Dim t2s As String = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))
                                            '次番組を探すための開始時間
                                            t2next = t2

                                            If t >= t1 And t < t2 Then
                                                Dim ei As CtrlCmdCLI.Def.EpgShortEventInfo
                                                ei = info.eventList.Item(k).ShortInfo
                                                If ei IsNot Nothing Then
                                                    Dim j As Integer = 0
                                                    If r Is Nothing Then
                                                        j = 0
                                                    Else
                                                        j = r.Length
                                                    End If
                                                    ReDim Preserve r(j)

                                                    r(j).stationDispName = sid2jigyousha(sid, tsid)
                                                    r(j).startDateTime = t1s
                                                    r(j).endDateTime = t2s
                                                    If ei.event_name IsNot Nothing Then r(j).programTitle = escape_program_str(ei.event_name)
                                                    If ei.text_char IsNot Nothing Then r(j).programContent = escape_program_str(ei.text_char)
                                                    r(j).sid = ch_list(i).sid
                                                    r(j).tsid = ch_list(i).tsid '一致しない可能性がある

                                                    chk = 1
                                                End If
                                                Exit For
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    Next

                                    If chk = 1 And getnext > 0 Then
                                        '次の番組を探す
                                        For k = 0 To info.eventList.Count - 1
                                            Try
                                                'まず現在時刻にあてはまるかチェック
                                                Dim t As DateTime = Now()
                                                Dim t1long As Integer = Int(info.eventList.Item(k).durationSec / 60) '分
                                                Dim t1 As DateTime = info.eventList.Item(k).start_time
                                                Dim t2 As DateTime = DateAdd(DateInterval.Minute, t1long, t1)
                                                Dim t1s As String = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                                Dim t2s As String = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))

                                                If t1 = t2next Then
                                                    Dim ei As CtrlCmdCLI.Def.EpgShortEventInfo
                                                    ei = info.eventList.Item(k).ShortInfo
                                                    If ei IsNot Nothing Then
                                                        Dim j As Integer = r.Length
                                                        ReDim Preserve r(j)

                                                        r(j).stationDispName = ch_list(i).jigyousha 'sid2jigyousha(sid, tsid)
                                                        r(j).startDateTime = t1s
                                                        r(j).endDateTime = t2s
                                                        If ei.event_name IsNot Nothing Then r(j).programTitle = escape_program_str(ei.event_name)
                                                        If ei.text_char IsNot Nothing Then r(j).programContent = escape_program_str(ei.text_char)
                                                        r(j).sid = ch_list(i).sid
                                                        r(j).tsid = ch_list(i).tsid '一致しない可能性がある

                                                        '次の番組であることを記録
                                                        r(j).nextFlag = 1
                                                    End If
                                                    Exit For
                                                End If
                                            Catch ex As Exception
                                            End Try
                                        Next
                                    End If

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
                            End If
                        Next
                    End If
                End If
            End If
        End If

        Return r
    End Function

    'ホストネームからIP
    Public Function host2ip(ByVal url As String) As String
        'ホスト名からIPアドレス、IPアドレスからホスト名を取得する
        'http://dobon.net/vb/dotnet/internet/dnslookup.html

        Dim r As String = ""

        Try
            '解決したいホスト名
            Dim hostName As String = url

            'IPHostEntryオブジェクトを取得
            Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(hostName)

            'IPアドレスのリストを取得
            Dim adList As System.Net.IPAddress() = iphe.AddressList

            If adList IsNot Nothing Then
                r = adList(0).ToString
            End If
        Catch ex As Exception
        End Try

        If r.Length = 0 Then
            r = url
        End If

        Return r
    End Function

    'EDCB番組表　旧方式
    Public Function get_EDCB_program_old(ByVal getnext As Integer) As Object
        Dim r() As TVprogramstructure = Nothing
        Try
            'If TvProgram_EDCB_url.Length > 0 Then
            If 1 = 2 And TvProgram_EDCB_url.Length > 0 Then
                If ch_list IsNot Nothing Then
                    For i As Integer = 0 To ch_list.Length - 1
                        Dim chk_j As Integer = 0

                        'EDCB番組表に存在しているかチェック
                        If EDCB_thru_addprogres = 0 Then
                            Dim ct As Integer = check_TSID_in_EDCBprogram(ch_list(i).tsid, ch_list(i).sid)
                            If ct < 0 Then
                                '存在していない
                                chk_j = 1
                            End If
                        End If

                        'プレミアム指定（1.16からは指定しなくてもOK）
                        If chk_j = 0 Then
                            If TvProgramEDCB_premium = 1 Then
                                'プレミアム指定されている場合、プレミアム以外は無視
                                If ch_list(i).sid < SPHD_sid_start Or ch_list(i).sid > SPHD_sid_end Then
                                    chk_j = 1
                                End If
                            ElseIf TvProgramEDCB_premium = 2 Then
                                'プレミアムを表示しないよう指定されている場合はプレミアムは無視
                                If ch_list(i).sid >= SPHD_sid_start And ch_list(i).sid <= SPHD_sid_end Then
                                    chk_j = 1
                                End If
                            End If
                        End If
                        'NGワードに指定されているものは無視
                        If chk_j = 0 Then
                            chk_j = isMATCHhosokyoku(TvProgramEDCB_NGword, ch_list(i).jigyousha, ch_list(i).sid)
                        End If
                        '番組情報を取得しないものは無視
                        If chk_j = 0 Then
                            chk_j = isMATCHhosokyoku(TvProgramEDCB_ignore, ch_list(i).jigyousha, ch_list(i).sid)
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
                            If ch_list(i).sid < SPHD_sid_start Or ch_list(i).sid > SPHD_sid_end Then
                                'SPHD以外ならTSIDも指定
                                If ch_list(i).tsid > 0 Then
                                    st_str = st_str & "&TSID=" & ch_list(i).tsid.ToString
                                End If
                            End If
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
                                    '次番組を探すための文字列
                                    Dim t2str As String = "<startDate>" & Year(t2) & "/" & Month(t2) & "/" & t2.Day & "</startDate><startTime>" & Hour(t2) & ":" & Minute(t2) & ":" & Second(t2) & "</startTime>"

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
                                        'sid,tsidは番組表から取ってきたものだがch_list().tsidが異なる可能性があるのでch_list()のほうを記録することにした
                                        r(j).sid = ch_list(i).sid
                                        r(j).tsid = ch_list(i).tsid '一致しない可能性がある

                                        chk = 1
                                        If getnext > 0 Then
                                            '次の番組を探す
                                            sp = html.IndexOf(t2str)
                                            If sp > 0 Then
                                                ep = html.IndexOf("</eventinfo>", sp)
                                                j = r.Length
                                                ReDim Preserve r(j)
                                                r(j).stationDispName = r(j - 1).stationDispName
                                                r(j).startDateTime = t2s
                                                t1long = Int(Val(Instr_pickup(html, "<duration>", "</duration>", sp, ep)) / 60) '分
                                                t1 = t2
                                                t2 = DateAdd(DateInterval.Minute, t1long, t1)
                                                t1s = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                                t2s = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))
                                                r(j).endDateTime = t2s
                                                r(j).programTitle = Instr_pickup(html, "<event_name>", "</event_name>", sp, ep)
                                                r(j).programContent = Instr_pickup(html, "<event_text>", "</event_text>", sp, ep)
                                                'sidを追加 
                                                'sid,tsidは番組表から取ってきたものだがch_list().tsidが異なる可能性があるのでch_list()のほうを記録することにした
                                                r(j).sid = ch_list(i).sid
                                                r(j).tsid = ch_list(i).tsid '一致しない可能性がある

                                                '次の番組であることを記録
                                                r(j).nextFlag = 1
                                            End If
                                        End If
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

        Return r
    End Function

End Module
