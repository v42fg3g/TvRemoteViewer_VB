Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization

Module モジュール_番組表
    Public TvProgram_ch() As Integer
    Public TvProgram_NGword() As String
    Public TvProgramD_channels() As String
    Public TvProgramD_sort() As String
    Public TvProgramD_BonDriver1st As String = ""

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
    Public Function make_TVprogram_html_now() As String
        Dim chkstr As String = ":" '重複防止用
        Dim html_all As String = ""
        Dim cnt As Integer = 0
        Dim TvProgram_html() As TVprogram_html_structure = Nothing

        If TvProgram_ch IsNot Nothing Then
            For i As Integer = 0 To TvProgram_ch.Length - 1
                If TvProgram_ch(i) > 0 Then
                    Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch(i))
                    If program IsNot Nothing Then
                        For Each p As TVprogramstructure In program
                            Dim html As String = ""
                            Dim hosokyoku As String = ""
                            Dim s4 As String = StrConv(p.stationDispName, VbStrConv.Wide) 'p.stationDispName
                            If s4.Length > 4 Then
                                s4 = s4.Substring(0, 4)
                            End If
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then
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
                                            startt = p.startDateTime.Substring(p.startDateTime.IndexOf(" "))
                                            endt = p.endDateTime.Substring(p.endDateTime.IndexOf(" "))
                                        Catch ex As Exception
                                        End Try

                                        'BonDriver, sid, 事業者を取得
                                        Dim d() As String = bangumihyou2bondriver(p.stationDispName)
                                        'd(0) = jigyousha d(1) = bondriver d(2) = sid d(3) = chspace

                                        html &= "<span class=""p_name"">" & d(0) & "　<span class=""p_name2"">(" & p.stationDispName & ")</span></span><br>" & vbCrLf 'p.stationDispName
                                        html &= "<span class=""p_time"">" & startt & " ～" & endt & "</span><br>"
                                        html &= "<span class=""p_title"">" & p.programTitle & "</span><br>" & vbCrLf
                                        html &= "<span class=""p_content"">" & p.programContent & "</span><br>" & vbCrLf
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
                                            If TvProgramD_BonDriver1st.Length > 0 Then
                                                bhtml = bhtml.Replace(TvProgramD_BonDriver1st.ToLower & """>", TvProgramD_BonDriver1st & """ selected>")
                                            Else
                                                bhtml = bhtml.Replace(d(1) & """>", d(1) & """ selected>")
                                            End If

                                            html &= "<form action=""StartTV.html"">" & vbCrLf
                                            html &= "<select name=""num"">" & vbCrLf
                                            html &= "<option>1</option>" & vbCrLf
                                            html &= "<option>2</option>" & vbCrLf
                                            html &= "<option>3</option>" & vbCrLf
                                            html &= "<option>4</option>" & vbCrLf
                                            html &= "<option>5</option>" & vbCrLf
                                            html &= "<option>6</option>" & vbCrLf
                                            html &= "<option>7</option>" & vbCrLf
                                            html &= "<option>8</option>" & vbCrLf
                                            html &= "</select>" & vbCrLf
                                            html &= bhtml & vbCrLf
                                            html &= "<input type=""hidden"" name=""ServiceID"" value=""" & d(2) & """>" & vbCrLf
                                            html &= "<input type=""hidden"" name=""ChSpace"" value=""" & d(3) & """>" & vbCrLf
                                            html &= "<span class=""p_hosokyoku""> " & d(0) & " </span>" & vbCrLf
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

    Public Function get_TVprogram_now(ByVal regionID As Integer) As Object

        Dim r() As TVprogramstructure = Nothing

        Try
            Dim sTargetUrl As String = "http://www.tvguide.or.jp/TXML301PG.php?type=TVG&regionId=" & regionID.ToString
            Dim objWeb As WebClient = New WebClient()
            Dim objSrializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim objEncode As Encoding = Encoding.UTF8
            Dim bResult As Byte() = objWeb.DownloadData(sTargetUrl)
            Dim sJson As String = objEncode.GetString(bResult)
            Dim objHash As Hashtable = objSrializer.Deserialize(Of Hashtable)(sJson)

            Dim dict As Dictionary(Of String, Object) = objHash("ProgramScheduleInfomartion")
            Dim MediaLocation As Dictionary(Of String, Object) = dict("MediaLocation")
            Dim StationLocation() As Object = MediaLocation("StationLocation")
            For Each station As Dictionary(Of String, Object) In StationLocation
                Dim j As Integer = 0
                If r Is Nothing Then
                    j = 0
                Else
                    j = r.Length
                End If
                ReDim Preserve r(j)
                r(j).stationDispName = CType(station("stationDispName"), String)
                Dim program As Dictionary(Of String, Object) = station("ProgramInformation")
                r(j).startDateTime = CType(program("startDateTime"), String)
                r(j).endDateTime = CType(program("endDateTime"), String)
                r(j).programTitle = CType(program("programTitle"), String)
                'r(j).programSubTitle = CType(program("programSubTitle"), String) '空白
                r(j).programContent = CType(program("programContent"), String)
                'r(j).programExplanation = CType(program("programExplanation"), String) '空白
            Next

        Catch ex As Exception

        End Try

        Return r
    End Function

    '番組表の放送局名からbondriver,sid等の取得を試みる
    Public Function bangumihyou2bondriver(ByVal hosokyoku As String) As Object
        Dim r(3) As String
        hosokyoku = StrConv(hosokyoku, VbStrConv.Wide) '全角に変換
        Dim h2 As String = rename_hosokyoku2jigyousha(hosokyoku)
        If h2 <> hosokyoku Then
            hosokyoku = h2
        Else
            hosokyoku = hosokyoku.Replace("テレビ", "")
            hosokyoku = hosokyoku.Replace("放送", "")
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
                If h.IndexOf(hosokyoku) >= 0 Then
                    '一致した
                    r(0) = ch_list(i).jigyousha
                    r(1) = ch_list(i).bondriver
                    r(2) = ch_list(i).sid.ToString
                    r(3) = ch_list(i).chspace.ToString
                    Exit For
                End If
            Next
        End If

        Return r
    End Function

    '番組表の放送局からBonDriver.ch2に登録の放送局名へ変換
    Public Function rename_hosokyoku2jigyousha(ByVal hosokyoku As String) As String
        If TvProgramD_channels IsNot Nothing Then
            For i As Integer = 0 To TvProgramD_channels.Length - 1
                Dim sp As Integer = TvProgramD_channels(i).IndexOf("：")
                If TvProgramD_channels(i).IndexOf(hosokyoku & "：") = 0 Then
                    Try
                        hosokyoku = TvProgramD_channels(i).Substring(sp + 1)
                    Catch ex As Exception
                    End Try
                    Exit For
                End If
            Next
        End If

        Return hosokyoku
    End Function
End Module
