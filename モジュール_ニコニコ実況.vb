Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO

Module モジュール_ニコニコ実況
    'NicoJKフォルダ
    Public NicoJK_path As String = ""
    'NicoJKフォルダのtxtを優先する(=1)
    Public NicoJK_first As Integer = 0
    'NicoConvAss_path
    Public NicoConvAss_path As String = ""

    'fonts.confの存在を確認
    Public fonts_conf_ok As Integer = 0

    Public jk_list() As sidstructure '放送局
    Public Structure sidstructure
        Public jkid As Integer
        Public nid As Integer
        Public sid As Integer
        Public chiiki As String
        Public jigyousha As String
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.sid = pF Then
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    Public Function sid2jk(ByVal sid As Integer, ByVal chspace As Integer) As String
        '放送局sidからjkフォルダに変換
        Dim r As String = ""

        '難視聴かCSか　CSなら排除
        Select Case sid
            Case 291, 292, 294, 295, 296, 297, 298
                If chspace = 1 Then sid = 999999999
        End Select

        Try
            Dim i As Integer = Array.IndexOf(jk_list, sid)

            If i >= 0 Then
                Dim b As Integer = jk_list(i).jkid
                If b > 0 Then
                    r = "jk" & b.ToString
                End If
            End If
        Catch ex As Exception
        End Try

        Return r
    End Function

    Public Sub ch_sid_load()
        Dim i As Integer = 0

        If file_exist("ch_sid.txt") = 0 Then
            log1write("【警告】ch_sid.txtが見つかりません。")
        Else
            Try
                Dim line() As String = file2line("ch_sid.txt")
                If line Is Nothing Then
                Else
                    For i = 0 To line.Length - 1
                        If line(i).IndexOf(";") >= 0 Then
                            line(i) = line(i).Substring(0, line(i).IndexOf(";"))
                        End If
                        If line(i).IndexOf("#") >= 0 Then
                            line(i) = line(i).Substring(0, line(i).IndexOf("#"))
                        End If
                        Dim youso() As String = line(i).Split(vbTab)
                        If youso Is Nothing Then
                        Else
                            If youso.Length >= 5 Then
                                ReDim Preserve jk_list(i)
                                jk_list(i).jkid = Val(trim8(youso(0)))
                                jk_list(i).nid = Val(trim8(youso(1)))
                                jk_list(i).sid = h16_10(trim8(youso(2)))
                                jk_list(i).chiiki = trim8(youso(3))
                                jk_list(i).jigyousha = StrConv(trim8(youso(4)), VbStrConv.Wide)
                            End If
                        End If
                    Next
                    log1write("ch_sid.txtからニコニコ実況変換用データを読み込みました")
                End If
            Catch ex As Exception
                log1write("【警告】ch_sid.txtの読み込みに失敗しました。")
            End Try
        End If
    End Sub

    '16進数から10進数へ
    Public Function h16_10(ByVal s As String) As Integer
        Dim r As Integer = 0
        s = trim8(s)

        If s.IndexOf("0x") = 0 Then
            Try
                's = s.Substring(2)
                If s.Length > 0 Then
                    r = Convert.ToInt32(s, 16)
                Else
                    r = -1
                End If
            Catch ex As Exception
                r = -1
            End Try
        Else
            r = Val(trim8(s))
        End If

        Return r
    End Function

    'ニコニコ実況用接続文字列を取得
    Public Function get_nico_jkvalue(ByVal jknum As String) As String
        Dim r As String = ""
        Try
            Dim wc As WebClient = New WebClient()
            Dim st As Stream = wc.OpenRead("http://jk.nicovideo.jp/api/v2/getflv?v=" & jknum)
            Dim enc As Encoding = Encoding.GetEncoding("UTF-8")
            Dim sr As StreamReader = New StreamReader(st, enc)
            r = sr.ReadToEnd()
            wc.Dispose()
        Catch ex As Exception
        End Try

        Return r
    End Function

    '字幕assファイルをシーク秒数に合わせて修正する
    Public Function ass_adjust_seektime(ByVal ass_file As String, ByVal rename_file As String, ByVal SeekSeconds As Integer, ByVal baisoku As String) As Integer
        Dim r As Integer = 0

        Dim speed As Double = 0
        Try
            speed = Double.Parse(baisoku)
        Catch ex As Exception
            speed = 1
        End Try
        If speed <= 0 Then
            speed = 1
        End If

        Dim txtass As String = file2str(ass_file, "UTF-8")
        Dim sp As Integer = 0
        Dim err As Integer = 0

        Try
            Dim part1 As String = ""
            Dim part2 As String = ""
            Dim resu() As String = Nothing
            Dim cnt As Integer = 0
            sp = txtass.IndexOf("Dialogue:")
            If sp > 0 Then
                'ヘッダーをresu(0)として入れておく
                part1 = txtass.Substring(0, sp)

                '字幕部分
                part2 = txtass.Substring(sp)

                Dim asss() As String = Split(part2, vbCrLf)

                Dim d2() As String = asss(0).Split(",")
                Dim dy1 As DateTime = CDate("2000/01/01 " & d2(1))
                Dim dy2 As DateTime = CDate("2000/01/01 " & d2(2))
                Dim howlong As Integer = DateDiff(DateInterval.Second, dy1, dy2)

                '可変対応(表示時間を変えるべきかどうか）
                'If speed <> 1 Then
                'howlong = Int(Math.Ceiling(howlong / speed))
                'If howlong <= 0 Then
                'howlong = 1
                'End If
                'End If

                log1write("コメントをシークに合わせてシフトしています")
                For i = 0 To asss.Length - 1
                    Dim d() As String = asss(i).Split(",")
                    If d.Length >= 10 Then
                        'd(1)を指定秒シフトしてマイナスなら切り捨てる
                        Dim ms As String = d(1).Substring(d(1).IndexOf(".")) '.01　マイクロセカンド
                        dy1 = CDate("2000/01/01 " & d(1))
                        dy1 = DateAdd(DateInterval.Second, -SeekSeconds, dy1)
                        If Year(dy1) >= 2000 Then
                            '表示しなくていいコメントは1999年になる
                            '可変対応
                            If speed <> 1 Then
                                Dim ta As Integer = (Hour(dy1) * 60 * 60) + (Minute(dy1) * 60) + Second(dy1)
                                ta = Int(ta / speed)
                                Dim h1 As Integer = Int(ta / 3600)
                                Dim h2 As Integer = Int((ta Mod 3600) / 60)
                                Dim h3 As Integer = ta Mod 60
                                Dim lt As String = h1 & ":" & h2 & ":" & h3
                                dy1 = CDate("2000/01/01 " & lt)
                            End If
                            dy2 = DateAdd(DateInterval.Second, howlong, dy1) '表示終了時間　指定秒後
                            Dim dy1_str As String = dy1.ToLongTimeString & ms 'マイクロセカンドも足す
                            Dim dy2_str As String = dy2.ToLongTimeString & ms 'マイクロセカンドも足す
                            Dim replace_str As String = dy1_str & "," & dy2_str
                            Dim asss2 As String = asss(i).Replace(d(1) & "," & d(2), replace_str)
                            ReDim Preserve resu(cnt)
                            resu(cnt) = asss2 '行全体
                            cnt += 1
                        End If
                    End If
                Next
                log1write("コメントシフトが完了しました")

                '可変の場合はここでコメントが重ならないよう再度調整
                If speed <> 1 And resu IsNot Nothing Then
                    resu = rebuild_ass(part1, resu)
                End If

                '最初のコメントにヘッダーを追加
                If resu Is Nothing Then
                    ReDim Preserve resu(0)
                    resu(0) = part1
                Else
                    resu(0) = part1 & resu(0)
                End If

                'ファイルに保存
                line2file(rename_file, resu, "UTF-8")

                r = 1
            Else
                'コメントが無い
                err = 1
                log1write("コメントがありません")
            End If
        Catch ex As Exception
            err = 2
            log1write("エラーが発生しました。" & ex.Message)
        End Try

        Return r
    End Function

    '可変時のコメント表示調整
    Public Function rebuild_ass(ByVal header As String, ByRef line As String()) As String()
        Dim resu() As String = Nothing

        log1write("再生スピードに合わせて字幕を調整します")
        Try
            Dim allow_05 As Integer = 1 '0.5行を許可する
            Dim row_limit As Integer = 530000 '上から何行まで表示するか

            Dim i As Integer = 0
            Dim sp As Integer = 0

            Dim v_width As Integer = Val(Trim(Instr_pickup(header, "PlayResX:", vbCrLf, 0)))
            Dim v_height As Integer = Val(Trim(Instr_pickup(header, "PlayResY:", vbCrLf, 0)))

            Dim howlong As Integer = 0 '何秒表示するか
            Dim font_height_all As Integer = 0 '改行ピクセル高

            '最後に処理したコメントのutime
            Dim movie_start_utime As Integer = time2unix(CDate("2010/01/01 00:00:00"))
            Dim last_utime As Integer = movie_start_utime

            If line IsNot Nothing Then
                '何秒間表示するか
                sp = line(0).IndexOf(",", sp)
                Dim t1u As Integer = time2unix(CDate("2010/01/01 " & Instr_pickup(line(0), ",", ",", sp)))
                sp = line(0).IndexOf(",", sp + 1)
                Dim t2u As Integer = time2unix(CDate("2010/01/01 " & Instr_pickup(line(0), ",", ",", sp)))
                howlong = t2u - t1u
                '改行ピクセル高
                sp = line(0).IndexOf(")}", sp)
                sp = line(0).LastIndexOf(",", sp)
                Dim f1 As Integer = Val(line(0).Substring(sp + 1)) 'これがy上限になる
                Dim y_start As Integer = f1
                Dim f2 As Integer = 0
                i = 1
                While f2 <= f1 And i < line.Length
                    sp = line(i).IndexOf(")}", sp + 1)
                    sp = line(i).LastIndexOf(",", sp + 1)
                    f2 = Val(line(i).Substring(sp + 1))
                    i += 1
                End While
                font_height_all = f2 - f1

                'howlongとkaigyou_heightが判別できていれば
                If howlong > 0 And font_height_all > 0 And v_width > 0 And v_height > 0 Then
                    Dim howmanyrow As Integer = Int((v_height - y_start) / font_height_all)
                    '空きチェック初期化
                    ReDim Preserve row_left(howmanyrow * 2)
                    ReDim Preserve row_right(howmanyrow * 2)
                    ReDim Preserve row_ue(howmanyrow)
                    ReDim Preserve row_shita(howmanyrow)

                    Dim cnt As Integer = 0

                    For i = 0 To line.Length - 1
                        Dim d() As String = line(i).Split(",")
                        If d.Length = 13 Then
                            Dim utime As Integer = time2unix(CDate("2010/01/01 " & Trim(d(1)))) 'コメントタイム
                            Dim x As Integer = Val(Instr_pickup(line(i), "move(", ",", 0))
                            Dim y As Integer = Val(d(10))
                            Dim comment As String = line(i).Substring(line(i).IndexOf("}") + 1)
                            Dim c_width As Integer = -Val(d(11)) 'コメントwidth
                            Dim hus As Integer = 0 '1=ue 2=shita
                            If x < (v_width / 2) Then
                                If y < (v_height / 2) Then
                                    hus = 1
                                Else
                                    hus = 2
                                End If
                            End If
                            Dim dtime_ss As Integer = Val(Instr_pickup(line(i), ".", ",", 0))

                            If utime > 0 Then
                                '前回処理したコメントutimeとの差の分時間が経ったのでrow_left()等をその分進める
                                If utime - last_utime > 0 Then
                                    row_time_progress(utime - last_utime)
                                    last_utime = utime
                                ElseIf utime - last_utime < 0 Then
                                    'もし時間が前後して送られてきた場合は
                                    utime = last_utime
                                End If

                                'コメントの相対時間
                                Dim utime_s As String = utime - movie_start_utime '相対秒数
                                If utime_s < 0 Then
                                    'マイナス値は0にする
                                    utime_s = 0
                                End If
                                Dim utime_start_str As String = utime.ToString
                                Dim utime_end_str As String = (utime + howlong).ToString

                                If hus = 1 Then
                                    '上に表示
                                    '何列目に表示するか
                                    Dim rowindex As Double = -1
                                    Dim rowindex_mitame As Double = -1
                                    Dim j As Integer = 0
                                    While j <= howmanyrow
                                        If row_ue(j) <= 0 Then
                                            rowindex = j
                                            Exit While
                                        End If
                                        j += 1
                                    End While

                                    Dim row_x As Integer = x
                                    Dim row_y As Integer = font_height_all * rowindex + y_start

                                    If rowindex >= 0 Then
                                        '成功
                                        Dim move_str As String = "{\move(" & row_x & "," & row_y & "," & row_x & "," & row_y & ")}"
                                        Dim txtass1 As String = d(0) _
                                             & "," & d(1) _
                                             & "," & d(2) _
                                             & "," & d(3) _
                                             & "," & d(4) _
                                             & "," & d(5) _
                                             & "," & d(6) _
                                             & "," & d(7) _
                                             & "," & d(8) _
                                             & "," & move_str & comment _
                                             & vbCrLf

                                        '空く時間を記録
                                        row_ue(rowindex) = howlong

                                        '記録
                                        ReDim Preserve resu(cnt)
                                        resu(cnt) = txtass1
                                        cnt += 1
                                    Else
                                        '表示しない
                                    End If
                                ElseIf hus = 2 Then
                                    '下に表示
                                    '何列目に表示するか
                                    Dim rowindex As Double = -1
                                    Dim rowindex_mitame As Double = -1
                                    Dim j As Integer = 0
                                    While j <= howmanyrow
                                        If row_shita(j) <= 0 Then
                                            rowindex = j
                                            Exit While
                                        End If
                                        j += 1
                                    End While

                                    Dim row_x As Integer = x
                                    Dim row_y As Integer = v_height - (font_height_all * (rowindex + 1)) - y_start

                                    If rowindex >= 0 Then
                                        '成功
                                        Dim move_str As String = "{\move(" & row_x & "," & row_y & "," & row_x & "," & row_y & ")}"
                                        Dim txtass1 As String = d(0) _
                                             & "," & d(1) _
                                             & "," & d(2) _
                                             & "," & d(3) _
                                             & "," & d(4) _
                                             & "," & d(5) _
                                             & "," & d(6) _
                                             & "," & d(7) _
                                             & "," & d(8) _
                                             & "," & move_str & comment _
                                             & vbCrLf

                                        '空く時間を記録
                                        row_shita(rowindex) = howlong

                                        '記録
                                        ReDim Preserve resu(cnt)
                                        resu(cnt) = txtass1
                                        cnt += 1
                                    Else
                                        '表示しない
                                    End If
                                Else
                                    '移動
                                    Dim all_width As Integer = v_width + c_width
                                    Dim move_x_start As Integer = v_width
                                    Dim move_x_end As Integer = 0 - c_width
                                    Dim right_ok As Double = (c_width * (howlong)) / all_width + (dtime_ss / 100)
                                    Dim left_arrive As Double = (v_width * (howlong)) / all_width + (dtime_ss / 100)
                                    '文字列全体が左端を通り越して見えなくなるまでの時間　=　計算する必要無し　指定されている
                                    Dim left_ok As Double = howlong + (dtime_ss / 100)

                                    '何列目に表示するか
                                    Dim rowindex As Double = -1
                                    Dim rowindex_mitame As Double = -1
                                    Dim j As Integer = 0
                                    While j <= howmanyrow * 2
                                        If row_right(j) <= 0 And left_arrive > row_left(j) Then
                                            '右端が空いており、なおかつ左端到達時間が左端空き時間より長い場合ＯＫ
                                            rowindex = j
                                            rowindex_mitame = rowindex
                                            Exit While
                                        End If
                                        j += 1
                                    End While
                                    'はみ出しているか判断
                                    Dim row_y As Integer = rowindex * font_height_all + y_start '画面上の表示ｙ座標
                                    If rowindex >= howmanyrow Then 'ここを>にして下２つrowindex-howmanyrow-1にすれば最下部に1行部分的に表示される
                                        If allow_05 = 1 Then
                                            'はみ出している場合 0.5行の位置に
                                            row_y = (rowindex - howmanyrow) * font_height_all + Int(font_height_all / 2)
                                            rowindex_mitame = rowindex - howmanyrow + 0.5
                                            If row_y > v_height Then
                                                'それでもはみ出している
                                                row_y = -1 '表示しない
                                            End If
                                        Else
                                            '0.5行表示が許可されていない場合
                                            row_y = -1 '表示しない
                                        End If
                                    End If

                                    If row_y >= 0 And comment.Length > 0 And (row_limit = 0 Or rowindex_mitame < (row_limit - 0.5)) Then
                                        'moveならば
                                        Dim move_str As String = "{\move(" & x & "," & row_y & "," & "-" & c_width & "," & row_y & ")}"

                                        '成功
                                        Dim txtass1 As String = d(0) _
                                             & "," & d(1) _
                                             & "," & d(2) _
                                             & "," & d(3) _
                                             & "," & d(4) _
                                             & "," & d(5) _
                                             & "," & d(6) _
                                             & "," & d(7) _
                                             & "," & d(8) _
                                             & "," & move_str & comment _
                                             & vbCrLf

                                        '両端が空く時間を記録
                                        row_right(rowindex) = right_ok
                                        row_left(rowindex) = left_ok

                                        '記録
                                        ReDim Preserve resu(cnt)
                                        resu(cnt) = txtass1
                                        cnt += 1
                                    Else
                                        '表示しない
                                    End If
                                End If
                            Else
                                '不正な行
                            End If
                        Else
                            '不正な行
                        End If
                    Next
                Else
                    log1write("ヘッダーから必要な情報を取得出来ませんでした")
                    resu = line
                End If
            Else
                log1write("変換する字幕がありません")
                resu = line
            End If
            log1write("再生スピードに合わせての字幕調整が完了しました")
        Catch ex As Exception
            log1write("【エラー】再生スピードに合わせての字幕調整に失敗しました。" & ex.Message)
            resu = line
        End Try

        Return resu
    End Function

    '列が何秒後に空くか
    Public howmanyrow As Integer '= Int(v_height / font_height) '- 1 '最大何列まで許可するか きっちり納めるなら-1
    Public row_left() As Double '上から何行目の左端が何秒後に空くか 表示可能rowの2倍を確保し、1倍以降は0.5行目に表示
    Public row_right() As Double '上から何行目の右端が何秒後に空くか 表示可能rowの2倍を確保し、1倍以降は0.5行目に表示
    Public row_ue() As Double '中央　上から何番目が何秒後に消えるか
    Public row_shita() As Double '中央　下から何番目が何秒後に消えるか

    '空き関係の時間を進める
    Private Sub row_time_progress(ByVal sec As Integer)
        Dim i As Integer
        For i = 0 To row_left.Length - 1
            row_left(i) -= sec
            If row_left(i) < 0 Then
                row_left(i) = 0
            End If
        Next
        For i = 0 To row_right.Length - 1
            row_right(i) -= sec
            If row_right(i) < 0 Then
                row_right(i) = 0
            End If
        Next
        For i = 0 To row_ue.Length - 1
            row_ue(i) -= sec
            If row_ue(i) < 0 Then
                row_ue(i) = 0
            End If
        Next
        For i = 0 To row_shita.Length - 1
            row_shita(i) -= sec
            If row_shita(i) < 0 Then
                row_shita(i) = 0
            End If
        Next
    End Sub

    Public Function F_xml2txt(ByVal s As String, Optional ByVal jkcg As Integer = 0) As String
        'xmlをテキストに変換
        '行末の改行を修正
        s = s.Replace("</chat>" & vbCrLf, "</chat>[]-]")
        '文中の改行を変換
        If jkcg = 1 Then
            'jkcommentgetterと同じ
            s = s.Replace(vbCrLf, "&#10;")
            s = s.Replace(vbLf, "&#10;")
        Else
            'jikkyorecと同じ
            s = s.Replace(vbCrLf, "&#13;&#10;")
            s = s.Replace(vbLf, "&#10;")
        End If
        '行末の改行を戻す
        s = s.Replace("</chat>[]-]", "</chat>" & vbCrLf)
        '行の始まりを修正
        While s.IndexOf(" <chat") >= 0
            s = s.Replace(" <chat", "<chat")
        End While

        'ヘッダを除去
        Dim sp As Integer
        sp = s.IndexOf("<chat")
        If sp >= 0 Then
            s = s.Substring(sp)
        End If
        'フッターを除去
        sp = s.LastIndexOf("</packet>")
        s = s.Substring(0, sp)

        Return s
    End Function

    '文字列の中で一番長い全角部分を抜き出して返す（ファイル名の全角部分を取得）
    Public Function zenkakudake_max(ByVal s As String, Optional ByVal a As Integer = 0) As String
        Dim r As String = ""
        Dim h() As String
        Dim hi As Integer = -1
        Dim z() As String
        Dim zi As Integer = -1

        'Dim hstr As String = "0123456789-+_()[]{}#^@!$.,;'=&~`"

        Dim i As Integer

        Dim f As Integer = 0
        Dim si As String = ""
        Dim zh As Integer
        If s.Length > 0 Then
            For i = 0 To s.Length - 1
                si = s.Substring(i, 1)
                zh = System.Text.Encoding.GetEncoding(932).GetByteCount(si)
                If zh = 2 Then
                    '全角
                    If f = 2 Then
                        z(zi) &= si
                    Else
                        zi += 1
                        ReDim Preserve z(zi)
                        z(zi) = si
                        f = 2
                    End If
                    '半角はいらないので取得しないことにした
                ElseIf zh = 1 Then
                    'If f = 1 Then
                    'h(hi) &= si
                    'Else
                    'hi += 1
                    'ReDim Preserve h(hi)
                    'h(hi) = si
                    f = 1
                    'End If
                End If
            Next
        End If

        Dim b As String = ""

        If z Is Nothing Then
        ElseIf z.Length > 0 Then
            r = z(0)
            For i = 0 To z.Length - 1
                If z(i).Length > r.Length Then
                    b = r '1つ前を記録
                    r = z(i)
                End If
            Next
        End If

        If a = 1 And b <> r Then
            '指定があれば2番目を返す
            r = b
        End If

        If r.Length = 0 Then
            '全角が無いファイル名の場合
            Dim fsp As Integer = s.LastIndexOf("\")
            Dim fed As Integer = s.LastIndexOf(".")
            If fsp > 0 And fed > 0 Then
                r = Instr_pickup(s, "\", ".", fsp)
            ElseIf fed > 0 Then
                r = s.Substring(0, fed)
            Else
                r = s
            End If
        End If

        Return r
    End Function

    '.tsファイル名からニコニココメントファイル.txtか.xmlを取得
    Public Function search_NicoJKtxt_file(ByVal fullpathfilename As String) As String
        Dim filepath As String = ""
        Dim filename As String = ""
        Dim filestamp As Integer = 0

        Dim targetfile As String = ""

        Dim i As Integer = 0

        If file_exist(fullpathfilename) = 1 Then
            filename = Path.GetFileName(fullpathfilename)
            filepath = IO.Path.GetDirectoryName(fullpathfilename)
            filestamp = time2unix(System.IO.File.GetLastWriteTime(fullpathfilename))
            If filename.Length > 0 Then
                'ファイル名と同名でtxtまたはxmlがあるか
                Dim filename_xml As String = fullpathfilename.Replace(".ts", ".xml")
                Dim filename_txt As String = fullpathfilename.Replace(".ts", ".txt")
                If file_exist(filename_xml) = 1 Then
                    targetfile = filepath & "\" & filename_xml
                ElseIf file_exist(filename_txt) = 1 Then
                    targetfile = filepath & "\" & filename_txt
                Else
                    'NicoJKフォルダを探す
                    If folder_exist(NicoJK_path) = 1 Then
                        Dim jklfilename As String = ""
                        '一番長い全角文字列を抜き出す
                        Dim z As String = zenkakudake_max(filename)
                        If z.Length = 0 Then
                            z = filename
                        End If
                        'NicoJK_pathにあるjklファイルをチェックする
                        Dim files As String() = System.IO.Directory.GetFiles(NicoJK_path, "*")
                        Dim chk_nojkl As Integer = 0
                        If files IsNot Nothing Then
                            For i = 0 To files.Length - 1
                                If files(i).IndexOf(z) >= 0 Then
                                    '文字列が含まれている場合、更新時間を照らし合わせる
                                    If files(i).IndexOf(".jkl") > 0 Or files(i).IndexOf(".xml") > 0 Then
                                        Dim stamp As Integer = time2unix(System.IO.File.GetLastWriteTime(files(i)))
                                        If System.Math.Abs(stamp - filestamp) < (60 * 20) Then
                                            '更新時間が前後20分未満ならほぼビンゴ
                                            jklfilename = Path.GetFileName(files(i))
                                            Exit For
                                        End If
                                    End If
                                End If
                            Next
                            If jklfilename.Length > 0 Then
                                If jklfilename.IndexOf(".xml") > 0 Then
                                    targetfile = NicoJK_path & "\" & jklfilename
                                Else
                                    Dim sp As Integer = jklfilename.IndexOf("[jk")
                                    If sp >= 0 Then
                                        'NicojCatch形式
                                        targetfile = NicoJK_path & "\" & "jk" & Instr_pickup(jklfilename, "[jk", "]", sp) & "\" & Instr_pickup(jklfilename, ")", ".", sp) & ".txt"
                                        If file_exist(targetfile) <= 0 Then
                                            targetfile = "" 'ファイルが存在しない　失敗
                                        End If
                                    Else
                                        'jikkyorec形式と思われる
                                        '<JikkyoRec startTime="1367049600000" channel="jk6" />
                                        Dim str As String = file2str(jklfilename, "UTF-8")
                                        Dim jkstr As String = "jk" & Instr_pickup(str, "channel=""jk", """", 0)
                                        Dim starttimestr As String = Instr_pickup(str, "startTime=""", """", 0)
                                        Dim starttime As Integer = 0
                                        If starttimestr.Length > 10 Then
                                            starttimestr = starttimestr.Substring(0, 10)
                                        End If
                                        Try
                                            starttime = Val(starttimestr)
                                        Catch ex As Exception
                                            starttime = 0
                                        End Try
                                        If jkstr.Length > 2 And starttime > 0 Then
                                            Dim commentfolder As String = NicoJK_path & "\" & jkstr
                                            If folder_exist(commentfolder) = 1 Then
                                                starttime += 600 '開始時間から10分の余裕を持たせる
                                                Dim chk_over As Integer = 0
                                                Dim files2 As String() = System.IO.Directory.GetFiles(commentfolder, "*.txt")
                                                If files2 IsNot Nothing Then
                                                    'unixtimeだけを抜き出して並び替え
                                                    For i = 0 To files2.Length - 1
                                                        files2(i) = Val(Path.GetFileName(files2(i))).ToString
                                                    Next
                                                    Dim k As Integer = files2.Length
                                                    ReDim Preserve files2(k)
                                                    files2(k) = time2unix(C_DAY2038).ToString 'ダミー
                                                    Array.Sort(files2)
                                                    '該当時間のファイルがあれば
                                                    For i = 0 To files2.Length - 2
                                                        If starttime >= Val(files2(i)) And starttime < Val(files2(i + 1)) Then
                                                            If starttime - Val(files2(i)) < (60 * 60 * 24) Then
                                                                '24時間以内のものならば
                                                                targetfile = commentfolder & "\" & files2(i) & ".txt"
                                                            End If
                                                            Exit For
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                chk_nojkl = 1
                            End If
                        Else
                            chk_nojkl = 1
                        End If
                        If chk_nojkl = 1 Then
                            '純粋NicoJK（jklファイル無し）
                            '動画ファイルからjkナンバーを取得しなければならないが時間がかかるので止める
                        End If
                    End If
                End If
            End If
        End If

        Return targetfile
    End Function

    'txtからassに変換してfileroot & "\" & "sub" & num.ToString & "_nico.ass"として保存　
    Public Function convert_NicoJK2ass(ByVal num As Integer, ByVal txt_file As String, ByVal fileroot As String, ByVal margin1 As Integer) As String
        'NicoConvAssを呼び出して処理
        Dim r As Integer = 0
        Dim targetfile As String = ""

        Try
            Dim filename As String = Path.GetFileName(txt_file)
            Dim tempfilename As String = filename
            Dim sp As Integer = filename.LastIndexOf(".")
            If sp > 0 Then
                tempfilename = filename.Substring(0, sp) & ".ass"
            End If
            Dim sourcefile As String = fileroot & "\" & tempfilename
            targetfile = fileroot & "\" & "sub" & num.ToString & "_nico.ass"

            Dim results As String = ""
            Dim psi As New System.Diagnostics.ProcessStartInfo()

            psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec") 'ComSpecのパスを取得する
            psi.RedirectStandardInput = False '出力を読み取れるようにする
            psi.RedirectStandardOutput = True
            psi.UseShellExecute = False
            psi.CreateNoWindow = True 'ウィンドウを表示しないようにする
            ''プログラムが存在するディレクトリ
            Dim ppath As String = IO.Path.GetDirectoryName(NicoConvAss_path)
            If ppath.Length > 0 Then
                ''カレントディレクトリ変更
                System.IO.Directory.SetCurrentDirectory(ppath)
                ''作業ディレクトリ変更
                psi.WorkingDirectory = ppath
            End If
            '出力エンコード
            psi.StandardOutputEncoding = Encoding.UTF8

            'psi.Arguments = "/c /d " & NicoConvAss_path & " """ & txt_file & """ -tx_margin " & VideoSeekDefault & " -tx_programname 0 -tx_writefolder """ & fileroot & """"
            psi.Arguments = "/c NicoConvAss.exe """ & txt_file & """ -tx_margin " & margin1 & " -tx_programname 0 -ext .ass -tx_writefolder """ & fileroot & """"
            log1write("NicoConvAss実行：" & txt_file & " 録画前マージン：" & margin1 & "秒")

            Dim p As System.Diagnostics.Process
            Try
                p = System.Diagnostics.Process.Start(psi)
                '出力を読み取る
                results = p.StandardOutput.ReadToEnd
                'WaitForExitはReadToEndの後である必要がある
                '(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit()
            Catch ex As Exception
            End Try

            'カレントディレクトリを戻す必要も無いかもだが
            F_set_ppath4program()

            'ファイルが作られたことを確認してリネームして終了
            If file_exist(sourcefile) = 1 Then
                'リネームして完了
                My.Computer.FileSystem.MoveFile(sourcefile, targetfile, True)
                'ファイルが出来るまで待機
                Dim j As Integer = 100 '5秒
                While j > 0 And file_exist(targetfile) < 1
                    System.Threading.Thread.Sleep(50)
                    j += 1
                End While
                If j > 0 Then
                    '成功
                    r = 1
                End If
            End If
        Catch ex As Exception
            log1write("【エラー】NicoJKログからASSへの変換中にエラーが発生しました。" & ex.Message)
        End Try

        If r = 0 Then
            '失敗
            targetfile = ""
        End If

        Return targetfile
    End Function

End Module
