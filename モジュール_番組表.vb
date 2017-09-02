﻿Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO
Imports System.Runtime.Serialization
'参照で.net System.Runtime.Serializationを追加
Imports System.Text.RegularExpressions

Module モジュール_番組表
    Public TvProgram_Force_NoRec As Integer = 0 'TvRock番組表の代わりにダミー番組表を表示する=1

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
    Public TvProgram_tvrock_tuner As Integer = -1 '事前に合わせるTvrockチューナー番号
    Public TvProgram_EDCB_url As String = ""
    'ptTimer
    Public ptTimer_path As String = "" 'pttimerのパス　末尾\
    Public pttimer_pt2count As Integer = 0
    Public TvProgramptTimer_NGword() As String
    Public pt3Timer_str As String = "" 'ptTimer3ならば"3"が入る
    'Tvmaid
    Public Tvmaid_url As String = "" 'Tvmaidのサーバーurl
    Public TvmaidIsEX As Integer = 0 'TvmaidEXなら1
    Public TvProgramTvmaid_NGword() As String
    'TvRock ジャンル判定
    Public TvRock_html_src As String = "" 'TvRock PC用番組表HTMLデータ
    Public TvRock_html_src_last As String = "" 'TvRock PC用番組表HTMLデータ(前回分）
    Public tvrock_genre_str() As String = {"ニュース／報道", "スポーツ", "情報／ワイドショー", "ドラマ", "音楽", "バラエティー", "映画", "アニメ／特撮", "ドキュメンタリー／教養", "劇場／公演", "趣味／教育", "福祉", "その他１", "その他２", "その他３", "その他４"}
    Public TvRock_html_getutime As Long = 0 '最終取得日時
    Public TvRock_web_ch_str As String = "" '&z=～&z=～
    Public TvRock_genre_ON As Integer = 0 '1=ジャンル判定する 0=従来通り

    Public LIVE_STREAM_STR As String = ""
    Public TvProgram_SelectUptoNum As Integer = 0 '番組表上の配信ナンバーを制限する

    Public TvProgramEDCB_premium As Integer = 0 'EDCB番組表がプレミアム用ならば1

    '次番組を表示する分数デフォルト(7分に設定）
    Public nextmin_default As Integer = 7

    '次の次の番組
    Public next2_minutes As Integer = 8 '次の番組が指定分以内の番組の場合は次の次の番組情報を追加表示 0ならば次の次の番組は収集しない
    'Public next2_conv() As String = {"ニュース:News", "天気:天気", "気象:天気", "インフォ:Info", "報:Info", "紹:Info", "プレ:Info", "買:shop", "ショッ:shop"}

    'EDCBの/addprogres.htmlでEDCB管理チャンネルを抽出しない場合=1
    Public EDCB_thru_addprogres As Integer = 0

    'EDCBがVelmy,niisaka版の場合は1
    Public EDCB_Velmy_niisaka As Integer = 0

    'EDCBの管理するチャンネルの取得方法
    Public EDCB_GetCh_method As Integer = 0 '1=旧方式

    '外部の番組情報を取得するURL
    Public Outside_StationName As String = "AbemaTV" '外部放送局名
    Public Outside_CustomURL As String = ""
    Public Outside_CustomURL_method As Integer = 1 '都合の良いデータ=1 今のところ1限定
    Public Outside_data_get_method As Integer = 2 '0=ini設定 1=自動選択（有志の方サイト優先） 2=カッパ手作業限定 3=有志の方サイト限定
    'チャンネルID,チャンネル名,開始unixtime,終了unixtime,タイトル,内容
    'を1行としたUTF-8テキストファイルが都合良く用意されているURL
    Public Outside_CustomURL_getutime As Long = 0 '最後に取得したunixtime
    Public Outside_CustomURL_html As String = "" 'キャッシュ
    Public Outside_CH_NAME() As String 'チャンネル名一覧　「チャンネル」は削除
    Public Outside_sid_temp_str As String = "99999801" '本来は""だが番組表に表示させるため暫定。sid=99999801～がOutsideを示す目印

    Public NoUseProgramCache As Integer = 0 'キャッシュを使用しない場合は1
    Public pcache() As TVprogram_cache_structure
    Public Structure TVprogram_cache_structure
        Public get_utime As Integer
        Public min_utime As Integer
        Public max_utime As Integer
        Public str As String
        Public value_str As String
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得
            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.str = pF Then '放送局名と一致するか
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    Public TvProgram_list() As TVprogramstructure
    Public Structure TVprogramstructure
        Implements IComparable
        Public stationDispName As String
        Public ProgramInformation As String
        Public startDateTime As String
        Public endDateTime As String
        Public programTitle As String
        Public programContent As String
        Public sid As Integer
        Public tsid As Integer
        Public genre As String '番組ジャンル　nibble1 * 256 + nibble2 次番組があれば:区切りで続けて記入
        Public nextFlag As Integer '次の番組なら1
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            '並べ替え用 sid,nextFlag
            Dim compare As Integer = Me.sid.CompareTo(DirectCast(obj, TVprogramstructure).sid)
            If compare = 0 Then
                Return Me.nextFlag.CompareTo(DirectCast(obj, TVprogramstructure).nextFlag)
            Else
                Return compare
            End If
        End Function
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
        Public sid As Integer
        Public title As String
    End Structure

    '地デジ番組表作成
    Public Function make_TVprogram_html_now(ByVal a As Integer, ByVal NHKMODE As Integer) As String
        'a=0 通常のインターネットから取得　 a=996 Tvmaidから取得　 a=997 ptTimerから取得　 a=998 EDCBから取得　 a=999 tvrockから取得
        Dim chkstr As String = ":" '重複防止用
        Dim html_all As String = ""
        Dim cnt As Integer = 0
        Dim TvProgram_html() As TVprogram_html_structure = Nothing

        Dim use_num_bon() As String = get_use_BonDriver() 'd(0)=地デジnum d(1)=地デジBonDriver d(2)=BS・CS num d(3)=BS・CS BonDriver d(4)=プレミアム num d(5)=プレミアム BonDriver

        Dim TvProgram_ch2() As Integer = Nothing
        If a = 0 Then
            '通常のインターネットから取得
            If TvProgram_ch IsNot Nothing Then
                TvProgram_ch2 = TvProgram_ch
            End If
        ElseIf a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
            'ネット番組表＋ダミー番組表
            If TvProgram_ch IsNot Nothing Then
                ReDim Preserve TvProgram_ch2(TvProgram_ch.Length)
                For k As Integer = 0 To TvProgram_ch.Length - 1
                    TvProgram_ch2(k) = TvProgram_ch(k)
                Next
                TvProgram_ch2(TvProgram_ch2.Length - 1) = 991
            Else
                ReDim Preserve TvProgram_ch2(0)
                TvProgram_ch2(0) = 991
            End If
        ElseIf a = 996 Then
            'Tvmaidから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 996
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
                            Dim sid As Integer = 0 'sid
                            Dim title As String = "" '番組名
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
                                        ElseIf a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
                                            'ダミー番組表　ネット番組表と同じ（手抜き）
                                            chk = isMATCHhosokyoku(TvProgram_NGword, p.stationDispName)
                                        ElseIf a = 996 Then
                                            'Tvmaid
                                            chk = isMATCHhosokyoku(TvProgramTvmaid_NGword, p.stationDispName, p.sid)
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
                                                title = escape_program_str(p.programTitle)
                                                html &= "<span class=""p_title"">" & title & "</span><br>" & vbCrLf
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
                                                sid = Val(Trim(d(2)))
                                                html &= "<input type=""hidden"" name=""ChSpace"" value=""" & d(3) & """>" & vbCrLf
                                                html &= "<span class=""p_hosokyoku""> " & d(0) & " </span>" & vbCrLf
                                                'NHK音声選択
                                                'If hosokyoku.IndexOf("ＮＨＫ") >= 0 Then
                                                'NHKでなくとも表示するようにした
                                                If NHKMODE = 3 Then
                                                    html &= WEB_make_NHKMODE_html_B()
                                                ElseIf NHKMODE >= 0 Then
                                                    html &= "<input type=""hidden"" name=""NHKMODE"" value=""" & NHKMODE & """>" & vbCrLf
                                                End If
                                                'End If
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
                            TvProgram_html(cnt).sid = sid
                            TvProgram_html(cnt).title = title
                            cnt += 1
                        Next
                    End If
                End If
            Next
        End If

        'ダミーの場合はネット放送局（番組情報有り）を残しダミー放送局を削除
        If a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
            If TvProgram_html IsNot Nothing Then
                For j As Integer = TvProgram_html.Length - 1 To 1 Step -1
                    If TvProgram_html(j).title = "_" Then
                        Dim d1 As Integer = TvProgram_html(j).sid
                        If d1 > 0 Then
                            For k As Integer = 0 To j - 1
                                Dim d2 As Integer = TvProgram_html(k).sid
                                If d2 > 0 Then
                                    If d1 = d2 Then
                                        'sidが一致
                                        TvProgram_html(j).html = ""
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
            End If
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
        html &= "<option value=""11"">主</option>" & vbCrLf
        html &= "<option value=""12"">副</option>" & vbCrLf
        html &= "<option value=""4"">第二音声</option>" & vbCrLf
        html &= "<option value=""5"">動画主音声</option>" & vbCrLf
        html &= "<option value=""6"">動画副音声</option>" & vbCrLf
        If exepath_VLC.Length > 0 Then
            html &= "<option value=""9"">VLCで再生</option>" & vbCrLf
        End If
        html &= "</select>" & vbCrLf

        Return html
    End Function

    '地域番号から番組表を取得
    Public Function get_TVprogram_now(ByVal regionID As Integer, ByVal getnext As Integer) As Object
        Dim r() As TVprogramstructure = Nothing

        '同一分のものはキャッシュから返す
        Dim msg As String = ""
        r = pcache_search(regionID, msg) 'msgはByRefで返ってくる
        If r IsNot Nothing Then
            log1write("【番組表】番組情報をキャッシュから取得しました。" & region2softname(regionID) & " " & msg)
        Else
            If regionID = 991 Or (TvProgram_Force_NoRec And regionID = 999) = 1 Then
                'ダミー
                r = get_NoRec_program()
            ElseIf regionID = 996 Then
                'Tvmaid
                If TvmaidIsEX = 1 Then
                    r = get_TvmaidEX_program()
                Else
                    r = get_Tvmaid_program()
                End If
            ElseIf regionID = 997 Then
                'ptTimer
                r = get_ptTimer_program()
            ElseIf regionID = 998 Then
                'EDCB
                r = get_EDCB_program()
                'r = get_EDCB_program_old(getnext) '旧方式
            ElseIf regionID = 999 Then
                'TvRock
                r = get_TvRock_program()
            ElseIf regionID = 801 And Outside_CustomURL.Length > 0 Then
                'Outside
                r = get_Outside_program()
            ElseIf regionID > 0 Then
                'ネットから地域の番組表を取得
                r = get_D_program(regionID)
            End If

            'キャッシュに保存
            pcache_set(regionID, r)
            log1write("【番組表】番組情報取得作業を行いました。" & region2softname(regionID))
        End If

        'Next処理
        r = after_raad_cache4next(r, getnext)

        Return r
    End Function

    'TVprogramstructureをstringへ変換
    Public Function convert_tvprogram_str(ByVal s() As TVprogramstructure) As String
        Dim r As String = ""
        If s IsNot Nothing Then
            For i As Integer = 0 To s.Length - 1
                Dim str As String = "" _
                & s(i).stationDispName & "," _
                & s(i).startDateTime & "," _
                & s(i).endDateTime & "," _
                & s(i).sid & "," _
                & s(i).tsid & "," _
                & s(i).genre & "," _
                & s(i).nextFlag & "," _
                & s(i).ProgramInformation & "," _
                & escape_program_str(s(i).programTitle) & "," _
                & escape_program_str(s(i).programContent)
                r &= str & vbCrLf
            Next
        End If
        Return r
    End Function

    'stringをTVprogramstructureへ変換
    Public Function convert_str_tvprogram(ByVal s As String) As TVprogramstructure()
        Dim r() As TVprogramstructure = Nothing
        Dim line() As String = Split(s, vbCrLf)
        For i As Integer = 0 To line.Length - 1
            Dim d() As String = line(i).Split(",")
            If d.Length >= 10 Then
                ReDim Preserve r(i)
                r(i).stationDispName = d(0)
                r(i).startDateTime = d(1)
                r(i).endDateTime = d(2)
                r(i).sid = Val(d(3))
                r(i).tsid = Val(d(4))
                r(i).genre = d(5)
                r(i).nextFlag = Val(d(6))
                r(i).ProgramInformation = d(7)
                r(i).programTitle = d(8)
                r(i).programContent = d(9)
            End If
        Next
        Return r
    End Function

    Private Function after_raad_cache4next(ByVal r As TVprogramstructure(), ByVal getnext As Integer) As TVprogramstructure()
        If r IsNot Nothing Then
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

                For i = 0 To r.Length - 2
                    If r(i).nextFlag = 0 Then
                        Dim n2_str As String = "" '次の次の番組情報
                        Dim n2_genre As String = "-1"
                        If next2_minutes > 0 Then
                            Try
                                If r(i + 2).nextFlag = 2 And ((r(i).sid > 0 And r(i).sid = r(i + 2).sid) Or r(i).stationDispName = r(i + 2).stationDispName) Then
                                    '次の次の番組があれば
                                    Dim tr As DateTime = CDate(r(i + 2).startDateTime)
                                    Dim tt As String = Hour(tr) & ":" & Minute(tr).ToString("d2")
                                    n2_str = "　　≫" & tt & " " & r(i + 2).programTitle
                                    n2_genre = r(i + 2).genre
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                        Try
                            If r(i + 1).nextFlag = 1 And ((r(i).sid > 0 And r(i).sid = r(i + 1).sid) Or r(i).stationDispName = r(i + 1).stationDispName) Then
                                Dim tr As DateTime = CDate(r(i).endDateTime)
                                Dim re As Integer = Hour(tr) * 100 + Minute(tr) '番組終了時間　4桁分秒
                                If re < ts Then
                                    '日付またぎしていれば()
                                    re += 2400
                                End If
                                If te >= re Then
                                    '次番組があれば
                                    Dim n1_str As String = r(i + 1).programTitle
                                    Dim n1_genre As String = r(i + 1).genre
                                    '次の次の番組があれば
                                    If n2_str.Length > 0 Then
                                        Dim du As Integer = time2unix(r(i + 1).endDateTime) - time2unix(r(i + 1).startDateTime)
                                        If du < 0 Then
                                            du += 3600 * 24
                                        End If
                                        If du <= (next2_minutes * 60) Then 'next2_minutes分以下の番組
                                            If next2_minutes < 120 Then
                                                '全ての番組において次の次を調べるわけでないなら、次の次の番組情報が表示されていることを示す印を付ける
                                                n1_str = "≫ " & n1_str
                                            End If
                                            'If next2_conv IsNot Nothing And Val(n1_genre) <> 7 And r(i).stationDispName.IndexOf("教育") < 0 And StrConv(r(i).stationDispName, VbStrConv.Wide).IndexOf("Ｅテレ") < 0 Then
                                            ''短いアニメの場合はタイトル短縮無し
                                            'If n1_str.Length > 10 Then '10文字以上のタイトルの場合は短縮を試みる
                                            ''タイトル短縮
                                            'Dim c_title As String = "&"
                                            'Dim c_cat As String = ""
                                            'For Each a As String In next2_conv
                                            'Dim d() As String = a.Split(":")
                                            'If d.Length = 2 Then
                                            'If n1_str.IndexOf(d(0)) >= 0 Then
                                            'If c_title.IndexOf("&" & d(0) & "&") < 0 Then
                                            'c_title &= c_cat & d(1)
                                            'c_cat = "&"
                                            'End If
                                            'End If
                                            'End If
                                            'Next
                                            'If c_title.Length > 1 Then
                                            'n1_str = c_title.Substring(1)
                                            'End If
                                            'End If
                                            ''ジャンルを上書き(次の次の方が重要なので）
                                            ''n1_genre = n2_genre
                                            ''と思ったが混乱するので中止
                                            'End If
                                            'If n1_str.Length > 10 Then
                                            'n1_str = n1_str.Substring(0, 10) & ".."
                                            'End If
                                        Else
                                            n2_str = ""
                                        End If
                                    End If
                                    r(i).programContent = "[Next] " & Trim(r(i + 1).startDateTime.Substring(r(i + 1).startDateTime.IndexOf(" "))) & "-" & Trim(r(i + 1).endDateTime.Substring(r(i + 1).endDateTime.IndexOf(" "))) & " " & n1_str & n2_str
                                    r(i).genre &= ":" & n1_genre
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
            Else
                '互換性のためnextFlag = 0 or 1 だけを抽出
                If next2_minutes > 0 Then
                    Dim k As Integer = 0
                    Dim r2() As TVprogramstructure = Nothing
                    For i = 0 To r.Length - 1
                        If r(i).nextFlag < 2 Then
                            ReDim Preserve r2(k)
                            r2(k) = r(i)
                            k += 1
                        End If
                    Next
                End If
            End If
        End If

        Return r
    End Function

    Private Function get_D_program(ByVal regionID As Integer) As TVprogramstructure()
        Dim r() As TVprogramstructure = Nothing
        Try
            Dim url As String = "http://tv.so-net.ne.jp/rss/schedulesByCurrentTime.action?group=10&stationAreaId=" & regionID.ToString

            'ネットから地域の番組表を取得
            Dim html As String = get_html_by_webclient(url, "UTF-8")

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
                    r(j).genre = "-1"
                End If

                sp = html.IndexOf("<title>", sp + 1)
            End While
        Catch ex As Exception
            log1write("インターネットからの番組表取得に失敗しました。" & ex.Message)
        End Try
        Return r
    End Function

    Public Sub set_Outside_CustomURL()
        Dim ut As Integer = time2unix(Now())
        Dim temp_name As String = ""
        Dim temp_url As String = ""
        Dim temp_method As Integer = 0
        If TvProgram_ch IsNot Nothing Then
            If Array.IndexOf(TvProgram_ch, 801) >= 0 Then
                If Outside_data_get_method = 0 Then
                    'iniで指定
                ElseIf Outside_data_get_method > 0 Then
                    'Outside_CustomURL
                    Dim ptexts As String = get_html_by_webclient(TvRemoteViewer_VB_version_URL.Replace("/version.txt", "/abm_src.txt") & "?tvrvtm=" & ut.ToString, "UTF-8", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36")
                    Dim line() As String = Split(Trim(ptexts), vbCrLf)
                    Dim rcount As Integer = 0
                    For i = 0 To line.Length - 1
                        If count_str(line(i), ",") = 2 Then
                            rcount += 1
                        Else
                            '想定外の行が出現したらそこで終了
                            Exit For
                        End If
                    Next
                    If rcount > 0 Then
                        Dim rnd As New System.Random(time2unix(Now()))
                        Dim ri As Integer = -1
                        Dim rp As Integer = 0
                        Dim d() As String = Nothing
                        'ランダム選択　最初の1回目は2行目以降にかかれた有志サーバーで取得することとする
                        If Outside_CustomURL.Length = 0 And rcount >= 2 And Outside_data_get_method = 1 Then
                            log1write(Outside_StationName & "番組情報取得先決定方法がランダムの場合、初回は有志提供サイトが優先されます")
                            rp = 1
                            rcount -= 1
                        ElseIf rcount = 1 Or Outside_data_get_method = 2 Then
                            '選択肢無し
                            ri = 0
                        ElseIf rcount >= 2 And Outside_data_get_method = 3 Then
                            '有志サイト
                            rp = 1
                            rcount -= 1
                        End If
                        If ri < 0 Then
                            'Outside_data_get_method=1 or 3
                            Dim cnt As Integer = 50
                            While cnt > 0
                                Dim rtemp As Integer = rnd.Next(rcount - rp) + rp
                                d = line(rtemp).Split(",")
                                If d.Length = 3 Then
                                    If d(2) <> Outside_CustomURL Or rcount = 1 Then
                                        '前回と違うURL、もしくは選択肢が無いなら決定
                                        ri = rtemp
                                        Exit While
                                    End If
                                Else
                                    'ありえない
                                End If
                                cnt -= 1
                            End While
                        End If
                        If ri >= 0 Then
                            d = line(ri).Split(",")
                            If d.Length = 3 Then
                                If Val(d(1)) > 0 And Trim(d(2)).Length > 0 Then
                                    Outside_StationName = Trim(d(0))
                                    Outside_CustomURL_method = Val(d(1))
                                    Outside_CustomURL = Trim(d(2))
                                    log1write(Outside_StationName & "の情報取得先の設定に成功しました。")
                                Else
                                    log1write("【エラー】不適切な" & Outside_StationName & "情報取得先です。" & line(ri))
                                End If
                            Else
                                'ありえない
                            End If
                        Else
                            log1write("【エラー】" & Outside_StationName & "の情報取得先の設定に失敗しました")
                        End If
                    Else
                        log1write("【エラー】" & Outside_StationName & "の情報取得先を取得できませんでした")
                    End If
                Else
                    log1write("【エラー】" & Outside_StationName & "の情報取得先決定方法Outside_data_get_methodが設定されていません")
                End If
                If Outside_CustomURL.Length > 0 Then
                    log1write(Outside_StationName & "情報取得先が決定されました。" & Outside_CustomURL)
                Else
                    If Outside_data_get_method > 0 Then
                        log1write("【エラー】" & Outside_StationName & "情報取得先が取得できませんでした")
                    Else
                        log1write("【エラー】" & Outside_StationName & "情報取得先が未入力です")
                    End If
                End If
            End If
        End If
    End Sub

    Public Function get_Outside_html(ByVal force As Integer) As String
        Dim html As String = Outside_CustomURL_html
        Dim ut As Integer = time2unix(Now())
        Dim timer_str As String = ""
        If force = 1 Then
            timer_str = "(タイマー)"
        End If
        If Outside_CustomURL.IndexOf("://") > 0 Then
            If isThisAbemaProgram(html) = 0 Or force = 1 Then
                'html未取得
                log1write("AbemaTV番組情報をネット上から取得します。" & Now())
                Outside_CustomURL_getutime = ut
                'force=1 タイマーからの指令ならば必ず取得
                Dim nocache_str As String = ""
                Dim ext As String = Path.GetExtension(Outside_CustomURL)
                If ext = ".txt" Then
                    nocache_str = "?tvrvtm=" & ut.ToString 'txtの場合内部キャッシュさせないよう
                End If
                html = get_html_by_webclient(Outside_CustomURL & nocache_str, "UTF-8", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36")
                log1write(Outside_StationName & "番組情報元：" & Outside_CustomURL)
                'うまく取得できなかった場合、次回のために取得先更新だけはしておく。取得は急がない
                If isThisAbemaProgram(html) = 0 Then
                    log1write("【エラー】" & Outside_StationName & "番組情報データ取得に失敗しました" & timer_str & "。情報取得先を再取得します")
                    set_Outside_CustomURL()
                End If
            Else
                log1write(Outside_StationName & "番組情報元：取得済み番組情報データ")
            End If
        ElseIf Outside_CustomURL.IndexOf(":\") > 0 Or Outside_CustomURL.IndexOf("\\") = 0 Then
            'ローカルファイルから
            html = file2str(Outside_CustomURL, "UTF-8")
            log1write(Outside_StationName & "番組情報元：" & Outside_CustomURL)
        Else
            log1write(Outside_StationName & "番組情報元が不明です")
        End If
        If isThisAbemaProgram(html) = 1 Then
            log1write("【番組表】" & Outside_StationName & "番組情報データを取得しました" & timer_str)
            Outside_CustomURL_html = html 'キャッシュに格納
        Else
            log1write("【エラー】" & Outside_StationName & "番組情報データ取得に失敗しました" & timer_str & "。既存のキャッシュを使用します")
            html = Outside_CustomURL_html '既存のキャッシュを返す
        End If
        Return html
    End Function

    'AbemaTVの番組情報かどうかチェック
    Private Function isThisAbemaProgram(ByRef html As String) As Integer
        Dim r As Integer = 0

        Dim cnt As Integer = count_str(html, ",")
        If cnt > 100 And html.IndexOf("bema") > 0 Then
            '区切り記号が100個以上あればまぁ良しとするか

            'と思ったがなんと途中で切れた状態で送られてくることがある（通信エラー？）
            '前回の3分の2以上のデータがあればOKとすることにする
            'チャンネル削減もありえるので余裕を持って判断
            Dim last_cnt As Integer = count_str(Outside_CustomURL_html, ",")
            If cnt >= Int(last_cnt / 3 * 2) Then
                r = 1
            Else
                log1write("【エラー】AbemaTV番組情報が前回取得時のものより極端に短いためデータ取得に失敗したと判断しました")
            End If
        End If

        Return r
    End Function

    Private Function get_Outside_program() As TVprogramstructure()
        Dim r() As TVprogramstructure = Nothing
        Try
            'Outside番組表を取得
            Dim ut As Integer = time2unix(Now())
            Dim html As String = get_Outside_html(0)
            If Outside_CustomURL_method = 1 Then
                '都合の良いデータ形式の場合
                If html.Length >= 300 Then
                    Outside_CH_NAME = Nothing
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
                            If ut >= Val(d(2)) And ut < Val(d(3)) Then
                                chk_inTime = 1 '現在の番組表が見つかった
                                Dim j As Integer = 0
                                If r Is Nothing Then
                                    j = 0
                                Else
                                    j = r.Length
                                End If
                                ReDim Preserve r(j)
                                r(j).stationDispName = d(1).Replace("チャンネル", "")
                                If Outside_CH_NAME Is Nothing Then 'チャンネル名を記録
                                    ReDim Preserve Outside_CH_NAME(0)
                                    Outside_CH_NAME(0) = r(j).stationDispName
                                Else
                                    If Array.IndexOf(Outside_CH_NAME, r(j).stationDispName) < 0 Then
                                        ReDim Preserve Outside_CH_NAME(Outside_CH_NAME.Length)
                                        Outside_CH_NAME(Outside_CH_NAME.Length - 1) = r(j).stationDispName
                                    End If
                                End If
                                r(j).ProgramInformation = d(0) '使ってないのでChanelIdを記録
                                r(j).startDateTime = unix2time(d(2)).ToString("yyyy/MM/dd H:mm")
                                r(j).endDateTime = unix2time(d(3)).ToString("yyyy/MM/dd H:mm")
                                r(j).programTitle = escape_program_str(d(4))
                                r(j).programContent = escape_program_str(d(5))
                                r(j).genre = -1
                                '次の番組
                                Try
                                    Dim chk As Integer = 0
                                    While i < line.Length - 1
                                        d = line(i + 1).Split(",")
                                        If d(1).Replace("チャンネル", "") = r(j).stationDispName Then
                                            If Val(d(2)) > ut Then
                                                j = r.Length
                                                ReDim Preserve r(j)
                                                r(j).stationDispName = d(1).Replace("チャンネル", "")
                                                r(j).ProgramInformation = d(0) '使ってないのでChanelIdを記録
                                                r(j).startDateTime = unix2time(d(2)).ToString("yyyy/MM/dd H:mm")
                                                r(j).endDateTime = unix2time(d(3)).ToString("yyyy/MM/dd H:mm")
                                                r(j).programTitle = escape_program_str(d(4))
                                                r(j).programContent = escape_program_str(d(5))
                                                r(j).genre = -1
                                                '次の番組であることを記録
                                                r(j).nextFlag = 1
                                                If next2_minutes > 0 Then
                                                    '次の次の番組
                                                    Dim last_d2 As Long = Val(d(2))
                                                    d = line(i + 2).Split(",")
                                                    If d(1).Replace("チャンネル", "") = r(j).stationDispName Then
                                                        If Val(d(2)) > last_d2 Then
                                                            j = r.Length
                                                            ReDim Preserve r(j)
                                                            r(j).stationDispName = d(1).Replace("チャンネル", "")
                                                            r(j).ProgramInformation = d(0) '使ってないのでChanelIdを記録
                                                            r(j).startDateTime = unix2time(d(2)).ToString("yyyy/MM/dd H:mm")
                                                            r(j).endDateTime = unix2time(d(3)).ToString("yyyy/MM/dd H:mm")
                                                            r(j).programTitle = escape_program_str(d(4))
                                                            r(j).programContent = escape_program_str(d(5))
                                                            r(j).genre = -1
                                                            '次の次の番組であることを記録
                                                            r(j).nextFlag = 2
                                                        End If
                                                    End If
                                                End If
                                                Exit While
                                            End If
                                        Else
                                            Exit While
                                        End If
                                        i += 1
                                    End While
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                    Next
                    If chk_inTime = 0 Then
                        log1write("【エラー】" & Outside_StationName & "番組情報内に現在の情報が含まれていません")
                    End If
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

    Private Function get_TvRock_program() As TVprogramstructure()
        Dim r() As TVprogramstructure = Nothing
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
                Dim html As String = get_html_by_webclient(TvProgram_tvrock_url, "Shift_JIS")

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
                    r(j).startDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "<i>", "～", sp)))
                    r(j).endDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "～", "</i></small>", sp)))
                    r(j).programTitle = escape_program_str(delete_tag(Instr_pickup(html, "<small><b>", "</b></small>", sp)))
                    Dim sp3 As Integer = html.IndexOf("</b></small>", sp)
                    sp3 = html.IndexOf("<font color=", sp3)
                    r(j).programContent = escape_program_str(delete_tag(Instr_pickup(html, ">", "</font>", sp3)))
                    '予約番号がわからないのでタイトルから推測
                    r(j).genre = get_tvrock_genre(0, 0, r(j).programTitle).ToString '"-1"

                    'サービスIDを取得
                    sp3 = html.IndexOf("javascript:reserv(", sp3)
                    If sp3 > 0 Then
                        r(j).sid = Val(Instr_pickup(html, ",", ",", sp3))
                    Else
                        log1write("【エラー】TvRock番組情報取得に失敗しました。TvRock携帯用番組表の「予約表示」を３番組以上にしてください")
                        r = Nothing
                        Exit While
                    End If

                    '次の番組を取得
                    sp = html.IndexOf("><small><i>", sp2 + 1)
                    Dim se As Integer = html.IndexOf(" <small><i>", sp2 + 1)
                    If se < 0 Then
                        se = html.Length - 1
                    End If
                    If sp > 0 And sp < se Then
                        '次の番組があれば
                        j = r.Length
                        ReDim Preserve r(j)
                        r(j).stationDispName = r(j - 1).stationDispName
                        r(j).startDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "<i>", "～", sp)))
                        r(j).endDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "～", "</i></small>", sp)))
                        r(j).programTitle = escape_program_str(delete_tag(Instr_pickup(html, "<small><small><small>", "</small></small></small>", sp)))
                        r(j).programContent = ""
                        r(j).sid = r(j - 1).sid

                        Dim sidtrid_str As String = Instr_pickup(html, "reserv(", ")", sp3, se)
                        Dim d() As String = sidtrid_str.Split(",")
                        r(j).genre = get_tvrock_genre(Val(d(1)), Val(d(2)), r(j).programTitle).ToString

                        '次の番組であることを記録
                        r(j).nextFlag = 1
                    End If

                    If next2_minutes > 0 Then
                        '次の次の番組を取得
                        sp = html.IndexOf("><small><i>", sp + 1)
                        If sp > 0 And sp < se Then
                            '次の番組があれば
                            j = r.Length
                            ReDim Preserve r(j)
                            r(j).stationDispName = r(j - 1).stationDispName
                            r(j).startDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "<i>", "～", sp)))
                            r(j).endDateTime = Trim(delete_tag("1970/01/01 " & Instr_pickup(html, "～", "</i></small>", sp)))
                            r(j).programTitle = escape_program_str(delete_tag(Instr_pickup(html, "<small><small><small>", "</small></small></small>", sp)))
                            r(j).programContent = ""
                            r(j).sid = r(j - 1).sid
                            r(j).genre = "-1" '次の次は必要無し

                            '次の次の番組であることを記録
                            r(j).nextFlag = 2
                        End If
                    End If

                    sp2 = html.IndexOf(" <small><i>", sp2 + 1)
                    sp = html.LastIndexOf("><small>", sp2 + 1)
                End While
            End If
        Catch ex As Exception
            log1write("TvRockからの番組表取得に失敗しました。" & ex.Message)
        End Try
        Return r
    End Function

    'TvRock PC用番組表からジャンルを推測する
    Private Function get_tvrock_genre(ByVal sid As Integer, ByVal trid As Integer, ByVal title As String) As Integer
        Dim r As Integer = -1

        If TvRock_html_src.Length > 0 Then
            Dim gstr As String = ""
            Dim sp As Integer = -1
            If sid > 0 And trid > 0 Then
                sp = TvRock_html_src.IndexOf("c=" & sid.ToString & "&e=" & trid.ToString)
            ElseIf title.Length > 0 Then
                'TvRock特有の変換に対応
                title = title.Replace("，", ",")
                title = title.Replace("＆", "&")

                sp = TvRock_html_src.IndexOf(title)
                If sp < 0 Then
                    'タイトルそのままでは見つからなかった
                    '携帯版番組表では<～>が消されているので番組名が無茶苦茶になっている場合がある

                    If title.IndexOf("<") < 0 And title.IndexOf(">") < 0 Then
                        While title.LastIndexOf("[") > Int(title.Length * 2 / 3)
                            '後半に[がある場合
                            title = title.Substring(0, title.LastIndexOf("["))
                            sp = TvRock_html_src.IndexOf(title)
                            If sp >= 0 Then
                                Exit While
                            End If
                        End While

                        If sp < 0 Then
                            While title.IndexOf("]") >= 0 And title.IndexOf("]") < Int(title.Length / 3)
                                '前半に]がある場合
                                Try
                                    title = title.Substring(title.IndexOf("]") + 1)
                                Catch ex As Exception
                                    title = ""
                                    sp = -1
                                    Exit While
                                End Try
                                sp = TvRock_html_src.IndexOf(title)
                                If sp >= 0 Then
                                    Exit While
                                End If
                            End While
                        End If
                    End If

                    If sp < 0 And title.Length > 0 Then
                        '【】[]<>を取り除いて一番長い文字列
                        title = Regex.Replace(title, "【+.*?】", "")
                        title = Regex.Replace(title, "\[+.*?\]", "")
                        title = Regex.Replace(title, "&lt;+.*?&gt;", "")
                        title = Regex.Replace(title, "<+.*?>", "")
                        title = Regex.Replace(title, "＜+.*?＞", "")
                        Dim tz As String = zenkakudake_max(title)
                        If tz.Length > 1 Then
                            sp = TvRock_html_src.IndexOf(Trim(tz))
                        Else
                            '1文字以下ならばたぶん英文タイトル 一番長い単語
                            title = title.Replace("　", " ")
                            Dim str As String = ""
                            Dim d() As String = title.Split(" ")
                            For k As Integer = 0 To d.Length - 1
                                If d(k).Length > str Then
                                    str = d(k)
                                End If
                            Next
                            If str.Length > 2 Then
                                sp = TvRock_html_src.IndexOf(str)
                            End If
                        End If
                    End If
                End If
            End If

            'ジャンル
            If sp > 0 Then
                sp = TvRock_html_src.LastIndexOf("width=11%", sp)
                If sp > 0 Then
                    gstr = Trim(Instr_pickup(TvRock_html_src, ">", "<", sp))
                    If gstr.Length > 0 Then
                        r = Array.IndexOf(tvrock_genre_str, gstr)
                        If r >= 0 Then
                            r = r * 256
                        Else
                            r = -1
                        End If
                    End If
                End If
            End If
        End If

        Return r
    End Function

    Private Function region2softname(ByVal regionID As Integer) As String
        Dim r As String = ""

        Select Case regionID
            Case 801
                r = Outside_StationName
            Case 991
                r = "ダミー"
            Case 996
                r = "TVMaid"
            Case 997
                r = "ptTimer"
            Case 998
                r = "EDCB"
            Case 999
                r = "TvRock"
            Case Else
                r = "地デジ(" & regionID & ")"
        End Select

        Return r
    End Function

    'キャッシュから
    Public Function pcache_search(ByVal RegionID As Integer, ByRef msg As String) As TVprogramstructure()
        If NoUseProgramCache = 0 Then
            Dim str As String = RegionID.ToString '識別
            Dim i2 As Integer = -1
            If pcache IsNot Nothing Then
                i2 = Array.IndexOf(pcache, str)
            End If
            If i2 >= 0 Then
                Dim t As DateTime = Now()
                Dim ut As Integer = time2unix(t)

                'log1write("【番組表】キャッシュが見つかりました。" & p_r_s(RegionID, route, getnext))
                Dim chk As Integer = 0
                If Minute(t) = Minute(unix2time(pcache(i2).get_utime)) Then
                    '同じ分ならキャッシュを利用
                    msg = "[same minutes]"
                    Return convert_str_tvprogram(pcache(i2).value_str)
                    Exit Function
                ElseIf pcache(i2).get_utime < ut - (3600 * 2) Then
                    '2時間以上前のキャッシュなら
                    log1write("【番組表】新しいキャッシュが存在しませんでした。" & region2softname(RegionID))
                    Return Nothing
                    Exit Function
                End If

                '波風の無い時間帯かどうか
                If pcache(i2).min_utime > time2unix(DateAdd(DateInterval.Day, -1, t)) And pcache(i2).max_utime < time2unix(DateAdd(DateInterval.Day, 1, t)) Then
                    'おかしな値でなければ続行
                    'Next調査期間4分はキャッシュを使用しない
                    If pcache(i2).min_utime <= ut And ut < (pcache(i2).max_utime) Then
                        '最小時間帯をはみ出してなければキャッシュを利用
                        log1write("【番組表】" & unix2time(pcache(i2).min_utime) & " <= " & t.ToString & " < " & unix2time(pcache(i2).max_utime))
                        msg = "[In a range 1]"
                        Return convert_str_tvprogram(pcache(i2).value_str)
                        Exit Function
                    ElseIf (pcache(i2).max_utime - (3 * 60)) < ut And ut < pcache(i2).max_utime Then
                        'Nextを調べる時間帯～次の番組までの短い間（NEXT期間4分間は実際に調べる）
                        log1write("【番組表】" & unix2time(pcache(i2).max_utime - (3 * 60)) & " < " & t.ToString & " < " & unix2time(pcache(i2).max_utime))
                        msg = "[In a range 2]"
                        Return convert_str_tvprogram(pcache(i2).value_str)
                        Exit Function
                        log1write("【番組表】有効範囲内のキャッシュが存在しませんでした。" & region2softname(RegionID))
                    End If
                Else
                    log1write("【エラー】番組表キャッシュが不正です。" & region2softname(RegionID) & " " & "：" & unix2time(pcache(i2).min_utime) & " > " & unix2time(pcache(i2).max_utime))
                End If
            Else
                log1write("【番組表】キャッシュが見つかりませんでした。" & region2softname(RegionID))
            End If
        End If
        Return Nothing
    End Function

    'キャッシュに格納
    Public Sub pcache_set(ByVal RegionID As Integer, ByVal value() As TVprogramstructure)
        If NoUseProgramCache = 0 Then
            Dim str As String = RegionID.ToString '識別
            Dim pindex As Integer = -1
            Dim k As Integer = 0
            If pcache IsNot Nothing Then
                pindex = Array.IndexOf(pcache, str)
                k = pcache.Length
            End If
            If pindex < 0 Then
                '同条件が見つからない場合
                ReDim Preserve pcache(k)
                pindex = k
            End If
            '一番遅い開始時間と一番早い終了時間を求める
            Dim p_start As Integer = 0
            Dim p_end As Integer = C_INTMAX - (3600 * 9)
            Dim t As DateTime = Now()
            If value IsNot Nothing Then
                For j As Integer = 0 To value.Length - 1
                    If value(j).nextFlag = 0 Then
                        Try
                            Dim s1 As DateTime = CDate(t.ToString("yyyy/MM/dd ") & Trim(value(j).startDateTime.Substring(11)))
                            Dim s2 As DateTime = CDate(t.ToString("yyyy/MM/dd ") & Trim(value(j).endDateTime.Substring(11)))
                            If s1 > s2 Then
                                If Hour(t) * 60 + Minute(t) < Hour(s2) * 60 + Minute(s2) Then
                                    s1 = DateAdd(DateInterval.Day, -1, s1)
                                Else
                                    s2 = DateAdd(DateInterval.Day, 1, s2)
                                End If
                            End If
                            If s1 < s2 Then
                                Dim u1 As Integer = time2unix(s1)
                                Dim u2 As Integer = time2unix(s2)
                                If u1 > p_start Then
                                    p_start = u1
                                End If
                                If u2 < p_end Then
                                    p_end = u2
                                End If
                            Else
                                log1write("【エラー】番組時間解析に失敗しました。" & region2softname(RegionID) & "：" & s1.ToString & " - " & s2.ToString)
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                Next

                'おかしな値でないかチェック
                If p_start > time2unix(DateAdd(DateInterval.Day, -1, t)) And p_end < time2unix(DateAdd(DateInterval.Day, 1, t)) Then
                    '記録
                    pcache(pindex).get_utime = time2unix(t)
                    pcache(pindex).min_utime = p_start
                    pcache(pindex).max_utime = p_end
                    pcache(pindex).str = str
                    pcache(pindex).value_str = convert_tvprogram_str(value)
                    'log1write("【番組表】番組情報をキャッシュに記録しました。[" & region2softname(RegionID) & "]")
                Else
                    log1write("【エラー】番組表の内容が不正です。" & region2softname(RegionID) & "：" & unix2time(p_start) & " > " & unix2time(p_end))
                End If
            End If
        End If
    End Sub

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
                            If ch_list(i).sid >= 1024 And ch_list(i).sid <= 63599 Then
                                '地デジ限定
                                cindex = i
                            End If
                            Exit For
                        End If
                    End If
                Next
            End If

            If cindex >= 0 Then
                r(0) = ch_list(cindex).jigyousha
                r(1) = ch_list(cindex).bondriver
                r(2) = ch_list(cindex).sid.ToString
                r(3) = ch_list(cindex).chspace.ToString
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
        ElseIf a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
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
    Public Function program_translate4WI(ByVal a As Integer, ByVal getnext As Integer, ByVal template As Integer) As String
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
            If TvProgram_ch IsNot Nothing Then
                TvProgram_ch2 = TvProgram_ch
            End If
        ElseIf a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
            'ネット番組表＋ダミー番組表
            If TvProgram_ch IsNot Nothing Then
                ReDim Preserve TvProgram_ch2(TvProgram_ch.Length)
                For k As Integer = 0 To TvProgram_ch.Length - 1
                    TvProgram_ch2(k) = TvProgram_ch(k)
                Next
                TvProgram_ch2(TvProgram_ch2.Length - 1) = 991
            Else
                ReDim Preserve TvProgram_ch2(0)
                TvProgram_ch2(0) = 991
            End If
        ElseIf a = 996 Then
            'Tvmaidから取得
            ReDim Preserve TvProgram_ch2(0)
            TvProgram_ch2(0) = 996
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
                            Dim sid As Integer = 0 'sid
                            Dim title As String = "" '番組名
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
                                            ElseIf a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
                                                'NoRecのNG処理はネット番組表と共有でいいかな（手抜き）
                                                chk = isMATCHhosokyoku(TvProgram_NGword, p.stationDispName)
                                            ElseIf a = 996 Then
                                                'Tvmaid
                                                chk = isMATCHhosokyoku(TvProgramTvmaid_NGword, p.stationDispName, p.sid)
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

                                            Dim c2 As Integer = -1
                                            If Outside_CH_NAME IsNot Nothing Then
                                                c2 = Array.IndexOf(Outside_CH_NAME, p.stationDispName)
                                            End If

                                            If c2 >= 0 Then
                                                'Outside
                                                hosokyoku = p.stationDispName
                                                If getnext = 2 And p.nextFlag = 1 Then
                                                    p.programTitle = "[Next]" & p.programTitle
                                                End If
                                                sid = 0
                                                title = escape_program_str(p.programTitle)
                                                Dim s_str As String = ""
                                                If Val(Outside_sid_temp_str) > 0 Then
                                                    s_str = (Val(Outside_sid_temp_str) + c2).ToString
                                                End If
                                                html &= p.stationDispName & "," & p.ProgramInformation & "," & Outside_sid_temp_str & "," & Outside_sid_temp_str & "," & Trim(startt) & "," & Trim(endt) & "," & title & "," & escape_program_str(p.programContent)
                                                If template = 1 Then
                                                    html &= "," & p.genre
                                                End If
                                                If getnext = 3 Then
                                                    html &= "," & p.nextFlag
                                                End If
                                                html &= vbCrLf
                                            Else
                                                'BonDriver, sid, 事業者を取得
                                                Dim d() As String = bangumihyou2bondriver(p.stationDispName, a, p.sid, p.tsid)
                                                'd(0) = jigyousha d(1) = bondriver d(2) = sid d(3) = chspace

                                                If d(0).Length > 0 Then
                                                    hosokyoku = StrConv(d(0), VbStrConv.Wide)
                                                End If

                                                If getnext = 2 And p.nextFlag = 1 Then
                                                    p.programTitle = "[Next]" & p.programTitle
                                                End If
                                                sid = Val(Trim(d(2)))
                                                title = escape_program_str(p.programTitle)
                                                html &= d(0) & "," & p.stationDispName & "," & d(2) & "," & d(3) & "," & Trim(startt) & "," & Trim(endt) & "," & title & "," & escape_program_str(p.programContent)
                                                If template = 1 Then
                                                    html &= "," & p.genre
                                                End If
                                                If getnext = 3 Then
                                                    html &= "," & p.nextFlag
                                                End If
                                                html &= vbCrLf
                                            End If

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
                            TvProgram_html(cnt).sid = sid
                            TvProgram_html(cnt).title = title
                            cnt += 1
                        Next
                    End If
                End If
            Next
        End If

        'ダミーの場合はネット放送局（番組情報有り）を残しダミー放送局を削除
        If a = 991 Or (TvProgram_Force_NoRec And a = 999) Then
            If TvProgram_html IsNot Nothing Then
                For j As Integer = TvProgram_html.Length - 1 To 1 Step -1
                    If TvProgram_html(j).title = "_" Then
                        Dim d1 As Integer = TvProgram_html(j).sid
                        If d1 > 0 Then
                            For k As Integer = 0 To j - 1
                                Dim d2 As Integer = TvProgram_html(k).sid
                                If d2 > 0 And d1 = d2 Then
                                    'sidが一致
                                    TvProgram_html(j).html = ""
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next
            End If
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
    Public Function escape_program_str(ByVal s As String, Optional ByVal m As Integer = 0) As String
        Dim r As String = ""
        'Outside
        s = s.Replace("&#34;", """")
        s = s.Replace("&#39;", "'")
        's = s.Replace("　", " ")
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
        '改行をエスケープ
        Select Case m
            Case 0
                '従来通り
                s = s.Replace(vbCrLf, " ")
                s = s.Replace(vbLf, " ")
                s = s.Replace(vbCr, " ")
                s = s.Replace("\n", " ")
            Case 1
                s = s.Replace(vbCrLf, "\n")
                s = s.Replace(vbLf, "\n")
                s = s.Replace(vbCr, "\n")
        End Select
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

    'EDCBが管理し番組表で表示するTSIDを取得する(CtrlCmdCLI使用）
    Public Sub EDCB_GET_TSID_CtrlCmdCLI()
        If TvProgram_EDCB_url.Length > 0 Then
            Dim i As Integer = 0
            Dim k As Integer = 0

            Dim ip As String = Instr_pickup(TvProgram_EDCB_url, "://", ":", 0)
            ip = host2ip(ip) 'ホストネームからIPに変換
            Dim sp As Integer = TvProgram_EDCB_url.IndexOf("://")
            Dim port As String = ""
            port = 4510 'CtrlCmdCLIのポート　決め打ち？

            If ip.Length > 0 And Val(port) > 0 Then
                EDCB_cmd.SetSendMode(True)
                EDCB_cmd.SetNWSetting(ip, port)
                Dim serviceList As New System.Collections.Generic.List(Of CtrlCmdCLI.Def.EpgServiceInfo)()
                Dim ret As Integer = EDCB_cmd.SendEnumService(serviceList) 'IPやportがおかしいとここで止まる可能性有り

                If ret = 1 Then
                    Dim last_name As String = ""
                    For k = 0 To serviceList.Count - 1
                        If serviceList(k).SID > 0 And serviceList(k).service_type = 1 Then
                            'ch_listに存在するTSIDならばリストに追加
                            If i = 0 Then
                                ReDim Preserve EDCB_TSID(i)
                                EDCB_TSID(i) = serviceList(k).TSID
                                ReDim Preserve EDCB_SID(i)
                                EDCB_SID(i) = serviceList(k).SID
                                i += 1
                            Else
                                Dim si As Integer = Array.IndexOf(EDCB_SID, serviceList(k).SID)
                                If si < 0 Then
                                    ReDim Preserve EDCB_TSID(i)
                                    EDCB_TSID(i) = serviceList(k).TSID
                                    ReDim Preserve EDCB_SID(i)
                                    EDCB_SID(i) = serviceList(k).SID
                                    i += 1
                                Else
                                    '重複有り
                                    If EDCB_TSID(si) <> serviceList(k).TSID Then
                                        'TSIDが違えば登録
                                        ReDim Preserve EDCB_TSID(i)
                                        EDCB_TSID(i) = serviceList(k).TSID
                                        ReDim Preserve EDCB_SID(i)
                                        EDCB_SID(i) = serviceList(k).SID
                                        i += 1
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            Else

            End If

            '最後にチェック
            If EDCB_TSID IsNot Nothing Then
                log1write("EDCB(CtrlCmdCLI)からEDCB番組表に表示する局を取得しました")
            Else
                log1write("【エラー】EDCB(CtrlCmdCLI)からEDCB番組表に表示する局の取得に失敗しました")
                EDCB_thru_addprogres = 1
                log1write("EDCB番組表に全チャンネルを表示するようにセットしました")
            End If
        End If
    End Sub

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
                        '[TSID][TSID][SID]      [TSID][SID][TSID]      
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
                            ''[TSID]と[TSID]の並びで判断
                            Dim sp2 As Integer = sp
                            Dim chk As Integer = 0
                            While sp2 > 0
                                Dim dex1 As String = Instr_pickup(html, "option value=""", """", sp2, ep)
                                Dim tsid_long As String = Hex(dex1) '16進数に変換
                                Dim tsid_hex As String = ""
                                Dim tsid As Integer = 0
                                Dim sid_hex As String = ""
                                Dim sid As Integer = 0

                                If tsid_long.Length = 12 Then
                                    '地デジ　velmy版なら[TSID][SID][TSID]
                                    tsid_hex = tsid_long.Substring(0, 4) '初めの4文字
                                    tsid = h16_10("0x" & tsid_hex)
                                    sid_hex = tsid_long.Substring(8, 4) '3番目の4文字
                                    sid = h16_10("0x" & sid_hex)
                                    If sid > 0 And tsid = sid Then
                                        chk += 1
                                    End If
                                End If
                                sp2 = html.IndexOf("option", sp2 + 1)
                                If sp2 > ep Then
                                    sp2 = -1
                                End If
                            End While
                            If chk >= 2 Then
                                Velmy_chk = 1
                                log1write("【EDCB】TSIDとSIDの並びからVelmy,niisaka版であると判断しました")
                            End If
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

                    sr.Close()
                    st.Close()
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
    Public Function get_ptTimer_program() As Object
        Dim r() As TVprogramstructure = Nothing

        Dim nextsec As Integer = 180 * 60
        '次の番組を取得するためデータベースから3時間分のデータを取得

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

                If nextsec = 0 Then
                    msql = """SELECT sid, eid, stime, length, title, texts, gen1 FROM t_event WHERE stime <= " & ut & " AND (stime + length) > " & ut & " ORDER BY sid"""
                Else
                    msql = """SELECT sid, eid, stime, length, title, texts, gen1 FROM t_event WHERE (stime <= " & ut & " AND (stime + length) > " & ut & ") OR (stime <= " & ut + nextsec & " AND stime > " & ut & ") ORDER BY sid,stime"""
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
                        Dim last_sid As String = "::"
                        Dim skip_sid As String = ":"
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

                                        If station.Length > 0 And skip_sid.IndexOf(":" & sid.ToString & ":") < 0 Then
                                            '放送局名が見つかっていれば
                                            Dim chk As Integer = -1
                                            If r IsNot Nothing Then
                                                chk = Array.IndexOf(r, station)
                                                If chk >= 0 Then
                                                    If r(chk).startDateTime = "1970/01/01 " & ystart_time Then
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
                                                r(i).sid = sid
                                                r(i).tsid = 0
                                                r(i).stationDispName = station
                                                Dim t1s As String = "1970/01/01 " & ystart_time
                                                Dim t2s As String = "1970/01/01 " & yend_time
                                                r(i).startDateTime = t1s
                                                r(i).endDateTime = t2s
                                                r(i).programTitle = title
                                                r(i).programContent = texts
                                                r(i).genre = Int(Val(youso(6)) / 16) * 256
                                                '次番組かどうかチェック
                                                Dim cnt As Integer = count_str(last_sid, ":" & sid.ToString & ":")
                                                If cnt = 1 Then
                                                    '2回目の場合は次番組であろう
                                                    r(i).nextFlag = 1
                                                ElseIf cnt = 2 Then
                                                    '3回目
                                                    r(i).nextFlag = 2
                                                    skip_sid &= sid.ToString & ":" '4回目以降はスキップするように
                                                End If
                                                last_sid &= sid.ToString & "::"
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
        If file_exist(ptTimer_path & "ptTimer.db") > 0 Then
            r = 1
            log1write("ptTimerのデータベースを認識しました")
        ElseIf file_exist(ptTimer_path & "pt3Timer.db") > 0 Then
            r = 1
            log1write("pt3Timerのデータベースを認識しました")
            pt3Timer_str = "3"
        Else
            log1write("【エラー】ptTimerのデータベースを認識できませんでした")
            ptTimer_path = ""
        End If
        Return r
        'Exit Function

        '以下、複数のデータベースを調べる場合　■未使用
        'If file_exist(ptTimer_path & "ptTimer.db") > 0 Then
        'r = 1
        'Dim i As Integer = 2
        'While file_exist(ptTimer_path & "ptTimer-" & i & ".db") > 0
        'r += 1
        'i += 1
        'End While
        'End If
        'If r = 1 Then
        'log1write("ptTimerのデータベースを認識しました")
        'ElseIf r > 1 Then
        'log1write("ptTimerのデータベースを" & r & "つ認識しました")
        'Else
        'log1write("【エラー】ptTimerのデータベースを認識できませんでした")
        'ptTimer_path = ""
        'End If
        'Return r
    End Function

    'NoRec番組表(TTRec等番組表データが無い時用）
    Public Function get_NoRec_program() As Object
        '全てダミー
        Dim r() As TVprogramstructure = Nothing
        If ch_list IsNot Nothing Then
            Dim i As Integer = 0
            Dim k As Integer = 0

            For i = 0 To ch_list.Length - 1
                Dim j As Integer = 0
                If r Is Nothing Then
                    j = 0
                Else
                    j = r.Length
                End If
                ReDim Preserve r(j)

                'ダミー時刻　開始は現在時　終了は7時間後
                Dim t1s As String = "1970/01/01 " & Hour(Now()).ToString & ":00"
                Dim t2s As String = "1970/01/01 " & ((Hour(Now()) + 6) Mod 24).ToString & ":59"

                r(j).stationDispName = ch_list(i).jigyousha
                r(j).startDateTime = t1s
                r(j).endDateTime = t2s
                r(j).programTitle = "_"
                r(j).programContent = ""
                r(j).sid = ch_list(i).sid
                r(j).tsid = ch_list(i).tsid
                r(j).genre = "-1"
            Next
        End If
        Return r
    End Function

    'EDCB番組表
    Public EDCB_cmd As New CtrlCmdCLI.CtrlCmdUtil
    Public Function get_EDCB_program() As Object
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
                                End If
                            Next

                            '該当がなければSIDだけ一致とか・・（念のため。たぶん役に立たない）
                            If kc < 0 Then
                                For k = 0 To epgList.Count - 1
                                    If epgList(k).serviceInfo.SID = ch_list(i).sid Then
                                        'SIDが一致
                                        kc = k
                                        Exit For
                                    End If
                                Next
                            End If

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
                                    '次番組を調べる時にすでに調べたイベントの開始時間をキャッシュする配列
                                    Dim scache(info.eventList.Count - 1) As DateTime
                                    For k = 0 To info.eventList.Count - 1
                                        Try
                                            'まず現在時刻にあてはまるかチェック
                                            Dim t As DateTime = Now()
                                            Dim t1long As Integer = Int(info.eventList.Item(k).durationSec / 60) '分
                                            Dim t1 As DateTime = info.eventList.Item(k).start_time
                                            Dim t2 As DateTime = DateAdd(DateInterval.Minute, t1long, t1)
                                            Dim t1s As String = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                            Dim t2s As String = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))
                                            '次番組を調べる時用に開始時間をキャッシュ
                                            scache(k) = t1

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
                                                    If ei.event_name IsNot Nothing Then
                                                        r(j).programTitle = escape_program_str(ei.event_name)
                                                    Else
                                                        r(j).programTitle = ""
                                                    End If
                                                    If ei.text_char IsNot Nothing Then
                                                        r(j).programContent = escape_program_str(ei.text_char)
                                                    Else
                                                        r(j).programContent = ""
                                                    End If
                                                    r(j).sid = ch_list(i).sid
                                                    r(j).tsid = ch_list(i).tsid '一致しない可能性がある
                                                    '番組ジャンル
                                                    Dim jnr As CtrlCmdCLI.Def.EpgContentInfo = info.eventList.Item(k).ContentInfo
                                                    r(j).genre = (jnr.nibbleList(0).content_nibble_level_1 * 256 + jnr.nibbleList(0).content_nibble_level_2).ToString

                                                    '次番組を探すための開始時間
                                                    t2next = t2

                                                    chk = 1
                                                End If
                                                Exit For
                                            End If
                                        Catch ex As Exception
                                        End Try
                                    Next

                                    If chk = 1 Then
                                        '次の番組を探す
                                        '今までに調べたなかにあったかキャッシュから探す
                                        Dim k2 As Integer = Array.IndexOf(scache, t2next)
                                        If k2 < 0 And (k + 1) < info.eventList.Count Then
                                            '見つからなければ途中から探す
                                            For k0 As Integer = k + 1 To info.eventList.Count - 1
                                                scache(k0) = info.eventList.Item(k0).start_time
                                                If t2next = info.eventList.Item(k0).start_time Then
                                                    k2 = k0
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                        If k2 >= 0 Then
                                            '次番組が存在すれば
                                            Try
                                                Dim t As DateTime = Now()
                                                Dim t1long As Integer = Int(info.eventList.Item(k2).durationSec / 60) '分
                                                Dim t1 As DateTime = info.eventList.Item(k2).start_time
                                                Dim t2 As DateTime = DateAdd(DateInterval.Minute, t1long, t1)
                                                Dim t1s As String = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                                Dim t2s As String = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))
                                                If t1 = t2next Then
                                                    Dim ei As CtrlCmdCLI.Def.EpgShortEventInfo
                                                    ei = info.eventList.Item(k2).ShortInfo
                                                    If ei IsNot Nothing Then
                                                        Dim j As Integer = r.Length
                                                        ReDim Preserve r(j)

                                                        r(j).stationDispName = ch_list(i).jigyousha 'sid2jigyousha(sid, tsid)
                                                        r(j).startDateTime = t1s
                                                        r(j).endDateTime = t2s
                                                        If ei.event_name IsNot Nothing Then
                                                            r(j).programTitle = escape_program_str(ei.event_name)
                                                        Else
                                                            r(j).programTitle = ""
                                                        End If
                                                        If ei.text_char IsNot Nothing Then
                                                            r(j).programContent = escape_program_str(ei.text_char)
                                                        Else
                                                            r(j).programContent = ""
                                                        End If
                                                        r(j).sid = ch_list(i).sid
                                                        r(j).tsid = ch_list(i).tsid '一致しない可能性がある
                                                        '番組ジャンル
                                                        Dim gnr As CtrlCmdCLI.Def.EpgContentInfo = info.eventList.Item(k2).ContentInfo
                                                        r(j).genre = (gnr.nibbleList(0).content_nibble_level_1 * 256 + gnr.nibbleList(0).content_nibble_level_2).ToString

                                                        '次の番組であることを記録
                                                        r(j).nextFlag = 1

                                                        If next2_minutes > 0 And t1long <= next2_minutes Then
                                                            '次の次の番組
                                                            t2next = t2
                                                            '今までに調べたなかにあったかキャッシュから探す
                                                            k2 = Array.IndexOf(scache, t2next)
                                                            If k2 < 0 And (k + 1) < info.eventList.Count Then
                                                                '見つからなければ途中から探す
                                                                For k0 As Integer = k + 1 To info.eventList.Count - 1
                                                                    scache(k0) = info.eventList.Item(k0).start_time
                                                                    If t2next = info.eventList.Item(k0).start_time Then
                                                                        k2 = k0
                                                                        Exit For
                                                                    End If
                                                                Next
                                                            End If
                                                            If k2 >= 0 Then
                                                                '次の次の番組が存在すれば
                                                                Try
                                                                    t = Now()
                                                                    t1long = Int(info.eventList.Item(k2).durationSec / 60) '分
                                                                    t1 = info.eventList.Item(k2).start_time
                                                                    t2 = DateAdd(DateInterval.Minute, t1long, t1)
                                                                    t1s = "1970/01/01 " & Hour(t1).ToString & ":" & (Minute(t1).ToString("D2"))
                                                                    t2s = "1970/01/01 " & Hour(t2).ToString & ":" & (Minute(t2).ToString("D2"))
                                                                    If t1 = t2next Then
                                                                        ei = info.eventList.Item(k2).ShortInfo
                                                                        If ei IsNot Nothing Then
                                                                            j = r.Length
                                                                            ReDim Preserve r(j)

                                                                            r(j).stationDispName = ch_list(i).jigyousha 'sid2jigyousha(sid, tsid)
                                                                            r(j).startDateTime = t1s
                                                                            r(j).endDateTime = t2s
                                                                            If ei.event_name IsNot Nothing Then
                                                                                r(j).programTitle = escape_program_str(ei.event_name)
                                                                            Else
                                                                                r(j).programTitle = ""
                                                                            End If
                                                                            If ei.text_char IsNot Nothing Then
                                                                                r(j).programContent = escape_program_str(ei.text_char)
                                                                            Else
                                                                                r(j).programContent = ""
                                                                            End If
                                                                            r(j).sid = ch_list(i).sid
                                                                            r(j).tsid = ch_list(i).tsid '一致しない可能性がある
                                                                            '番組ジャンル
                                                                            gnr = info.eventList.Item(k2).ContentInfo
                                                                            r(j).genre = (gnr.nibbleList(0).content_nibble_level_1 * 256 + gnr.nibbleList(0).content_nibble_level_2).ToString

                                                                            '次の次の番組であることを記録
                                                                            r(j).nextFlag = 2
                                                                        End If
                                                                    End If
                                                                Catch ex As Exception
                                                                End Try
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Catch ex As Exception
                                            End Try
                                        End If
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
                                        r(j).genre = "-1"
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
                                    r(j).genre = "-1"
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

        Dim r As String = url

        'まずipかどうかチェック
        Dim isIP As Integer = 0
        Dim d() As String = url.Split(".")
        If d.Length = 4 Then
            If IsNumeric(d(0)) And IsNumeric(d(1)) And IsNumeric(d(2)) And IsNumeric(d(3)) Then
                isIP = 1
            End If
        End If

        If isIP = 0 Then
            If url = "localhost" Then
                r = "127.0.0.1"
            Else
                Try
                    '解決したいホスト名
                    Dim hostName As String = url

                    'IPHostEntryオブジェクトを取得
                    Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(hostName)

                    'IPアドレスのリストを取得
                    Dim adList As System.Net.IPAddress() = iphe.AddressList

                    If adList IsNot Nothing Then
                        For i As Integer = 0 To adList.Length - 1
                            If adList(i).AddressFamily = Sockets.AddressFamily.InterNetwork Then
                                r = adList(i).ToString
                                Exit For
                            End If
                        Next
                    End If
                Catch ex As Exception
                    log1write("【エラー】iniのTvProgram_EDCB_urlで指定された" & url & "のIPアドレス変換に失敗しました")
                End Try
            End If
        End If

        Return r
    End Function

    'EDCB番組表　旧方式 '■未使用
    Public Function get_EDCB_program_old() As Object
        Dim r() As TVprogramstructure = Nothing
        Try
            'If TvProgram_EDCB_url.Length > 0 Then
            If TvProgram_EDCB_url.Length > 0 Then
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
                                        r(j).genre = "-1"

                                        chk = 1
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
                                            r(j).genre = "-1"

                                            '次の番組であることを記録
                                            r(j).nextFlag = 1
                                        End If
                                        Exit While
                                    End If
                                Catch ex As Exception
                                End Try

                                sp = html.IndexOf("<eventinfo>", sp + 1)
                                ep = html.IndexOf("</eventinfo>", sp + 1)
                            End While

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
                                r(j).genre = "-1"
                            End If

                            sr.Close()
                            st.Close()

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
                            r(j).genre = "-1"
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            log1write("EDCBからの番組表取得に失敗しました。" & ex.Message)
        End Try

        Return r
    End Function

    'Tvmaid番組表
    Public Function get_Tvmaid_program() As Object
        Dim r() As TVprogramstructure = Nothing

        Dim nextsec As Integer = 150 * 60
        '次の番組を取得するため期間を2時間半にしてデータベースから取得

        If Tvmaid_url.Length > 0 Then
            'データベースから番組一覧を取得する
            Try
                Dim nowtime As DateTime = Now()
                Dim nowtime_n As DateTime = DateAdd(DateInterval.Second, nextsec, nowtime)
                'Dim ut As Integer = time2unix(nowtime) '現在のunixtime
                'Dim utn As Integer = ut + nextsec
                Dim ut_b As Long = DateTime.Parse(nowtime).ToBinary
                Dim utn_b As Long = DateTime.Parse(nowtime_n).ToBinary
                Dim url As String = Tvmaid_url & "/webapi?api=GetTable&sql="
                If nextsec = 0 Then
                    url &= "SELECT fsid,start,end,duration,title,desc,genre,subgenre from event WHERE start <= " & ut_b & " AND end > " & ut_b & " ORDER BY fsid"
                Else
                    url &= "SELECT fsid,start,end,duration,title,desce,genre,subgenre from event WHERE (start <= " & ut_b & " AND end > " & ut_b & ") OR (start <= " & utn_b & " AND start > " & ut_b & ") ORDER BY fsid,start"
                End If
                url = url.Replace("//webapi?api", "/webapi?api")

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

                Dim jsonRead As New Json.DataContractJsonSerializer(GetType(tvmaidData.TvmaidReserve))
                Dim tr As tvmaidData.TvmaidReserve = jsonRead.ReadObject(ms)

                If tr.Code = 0 Then
                    Dim last_sid As String = "::"
                    Dim skip_sid As String = ":"
                    For i = 0 To tr.Data1.Count - 1
                        'タイトル
                        Dim title As String = escape_program_str(tr.Data1(i)(4))
                        'TSID,SIDを算出
                        Dim tsid_long As String = Hex(tr.Data1(i)(0))
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
                        Dim ystartDate As DateTime = DateTime.FromBinary(tr.Data1(i)(1)) '開始時刻(調整いらず）
                        Dim ystart As Integer = time2unix(ystartDate)
                        '録画時間（秒）
                        Dim duration As Integer = Val(tr.Data1(i)(3))
                        '重複チェック用文字列
                        Dim indexofstr As String = sid & "_" & ystart & "_" & duration

                        If Array.IndexOf(ch_list, sid) >= 0 And ystart > 0 And duration > 0 And sid > 0 Then
                            'ch_list()にsidが登録されていれば
                            '録画開始時間
                            Dim ystart_day As String = ystartDate.ToShortDateString
                            Dim ystart_time As String = ystartDate.ToString("HH:mm")
                            '録画終了時間
                            Dim yend As Integer = ystart + duration
                            Dim yendDate As DateTime = unix2time(yend)
                            Dim yend_time As String = yendDate.ToString("HH:mm")
                            '録画時間
                            Dim delta As Integer = duration
                            Dim deltastr_time As String = Int(delta / (60 * 60)).ToString.PadLeft(2, "0")
                            Dim deltastr_minute As String = (Int(delta / 60) - (Val(deltastr_time) * 60)).ToString.PadLeft(2, "0")
                            Dim deltastr_sec As String = Int(delta Mod 60).ToString.PadLeft(2, "0")
                            Dim deltastr As String = deltastr_time & ":" & deltastr_minute & ":" & deltastr_sec
                            '内容
                            Dim texts As String = escape_program_str(tr.Data1(i)(5))
                            'ジャンル
                            Dim genre As Integer = Val(tr.Data1(i)(6)) * 256 + Val(tr.Data1(i)(7))

                            '放送局名
                            Dim station As String
                            station = sid2jigyousha(sid, tsid)

                            If station.Length > 0 And skip_sid.IndexOf(":" & sid.ToString & ":") < 0 Then
                                '放送局名が見つかっていれば
                                Dim chk As Integer = -1
                                If r IsNot Nothing Then
                                    chk = Array.IndexOf(r, station)
                                    If chk >= 0 Then
                                        If r(chk).startDateTime = "1970/01/01 " & ystart_time Then
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
                                    r(j).sid = sid
                                    r(j).tsid = 0
                                    r(j).stationDispName = station
                                    Dim t1s As String = "1970/01/01 " & ystart_time
                                    Dim t2s As String = "1970/01/01 " & yend_time
                                    r(j).startDateTime = t1s
                                    r(j).endDateTime = t2s
                                    r(j).programTitle = title
                                    r(j).programContent = texts
                                    r(j).genre = genre.ToString
                                    '次番組かどうかチェック
                                    Dim cnt As Integer = count_str(last_sid, ":" & sid.ToString & ":")
                                    If cnt = 1 Then
                                        '2回目の場合は次番組であろう
                                        r(j).nextFlag = 1
                                    ElseIf cnt = 2 Then
                                        '3回目
                                        r(j).nextFlag = 2
                                        skip_sid &= sid.ToString & ":" '4回目以降はスキップするように
                                    End If
                                    last_sid &= sid.ToString & "::"
                                End If
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
                    'sidで並び替え
                    Array.Sort(r)
                End If
            Catch ex As Exception
                log1write("Tvmaid番組情報取得中にエラーが発生しました。" & ex.Message)
            End Try
        End If

        Return r
    End Function

    'TvmaidEX番組表
    Public Function get_TvmaidEX_program() As Object
        Dim r() As TVprogramstructure = Nothing

        Dim nextsec As Integer = 150 * 60
        '次の番組を取得するため期間を2時間半にしてデータベースから取得

        If Tvmaid_url.Length > 0 Then
            'データベースから番組一覧を取得する
            Try
                Dim nowtime As DateTime = Now()
                Dim nowtime_n As DateTime = DateAdd(DateInterval.Second, nextsec, nowtime)
                'Dim ut As Integer = time2unix(nowtime) '現在のunixtime
                'Dim utn As Integer = ut + nextsec
                Dim ut_b As Long = DateTime.Parse(nowtime).ToBinary
                Dim utn_b As Long = DateTime.Parse(nowtime_n).ToBinary
                Dim url As String = Tvmaid_url & "/webapi/GetTable?sql="
                If nextsec = 0 Then
                    url &= "SELECT fsid,start,end,duration,title,desc,genre from event WHERE start <= " & ut_b & " AND end > " & ut_b & " ORDER BY fsid"
                Else
                    url &= "SELECT fsid,start,end,duration,title,desc,genre from event WHERE (start <= " & ut_b & " AND end > " & ut_b & ") OR (start <= " & utn_b & " AND start > " & ut_b & ") ORDER BY fsid,start"
                End If
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

                Dim jsonRead As New Json.DataContractJsonSerializer(GetType(tvmaidExData.TvmaidExReserve))
                Dim tr As tvmaidExData.TvmaidExReserve = jsonRead.ReadObject(ms)

                If tr.code = 0 Then
                    Dim last_sid As String = "::"
                    Dim skip_sid As String = ":"
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

                        If Array.IndexOf(ch_list, sid) >= 0 And ystart > 0 And duration > 0 And sid > 0 Then
                            'ch_list()にsidが登録されていれば
                            '録画開始時間
                            Dim ystart_day As String = ystartDate.ToShortDateString
                            Dim ystart_time As String = ystartDate.ToString("HH:mm")
                            '録画終了時間
                            Dim yend As Integer = ystart + duration
                            Dim yendDate As DateTime = unix2time(yend)
                            Dim yend_time As String = yendDate.ToString("HH:mm")
                            '録画時間
                            Dim delta As Integer = duration
                            Dim deltastr_time As String = Int(delta / (60 * 60)).ToString.PadLeft(2, "0")
                            Dim deltastr_minute As String = (Int(delta / 60) - (Val(deltastr_time) * 60)).ToString.PadLeft(2, "0")
                            Dim deltastr_sec As String = Int(delta Mod 60).ToString.PadLeft(2, "0")
                            Dim deltastr As String = deltastr_time & ":" & deltastr_minute & ":" & deltastr_sec
                            '内容
                            Dim texts As String = escape_program_str(tr.data1(i)(5))
                            '改行（\u000d\u000a）が入ることがあるのかな・・
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
                                    genre = Int((g1 Mod 256) / 16) * 256
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

                            If station.Length > 0 And skip_sid.IndexOf(":" & sid.ToString & ":") < 0 Then
                                '放送局名が見つかっていれば
                                Dim chk As Integer = -1
                                If r IsNot Nothing Then
                                    chk = Array.IndexOf(r, station)
                                    If chk >= 0 Then
                                        If r(chk).startDateTime = "1970/01/01 " & ystart_time Then
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
                                    r(j).sid = sid
                                    r(j).tsid = 0
                                    r(j).stationDispName = station
                                    Dim t1s As String = "1970/01/01 " & ystart_time
                                    Dim t2s As String = "1970/01/01 " & yend_time
                                    r(j).startDateTime = t1s
                                    r(j).endDateTime = t2s
                                    r(j).programTitle = title
                                    r(j).programContent = texts
                                    r(j).genre = genre.ToString
                                    '次番組かどうかチェック
                                    Dim cnt As Integer = count_str(last_sid, ":" & sid.ToString & ":")
                                    If cnt = 1 Then
                                        '2回目の場合は次番組であろう
                                        r(j).nextFlag = 1
                                    ElseIf cnt = 2 Then
                                        '3回目
                                        r(j).nextFlag = 2
                                        skip_sid &= sid.ToString & ":" '4回目以降はスキップするように
                                    End If
                                    last_sid &= sid.ToString & "::"
                                End If
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
                    'sidで並び替え
                    Array.Sort(r)
                End If
            Catch ex As Exception
                log1write("【エラー】TvmaidYUI番組情報取得中にエラーが発生しました。" & ex.Message)
            End Try
        End If

        Return r
    End Function

End Module

Namespace tvmaidData
    Public Class TvmaidReserve
        Public Code As Integer
        Public Message As String
        Public Data1 As List(Of String())
    End Class
End Namespace

Namespace tvmaidExData
    Public Class TvmaidExReserve
        Public code As Integer
        Public message As String
        Public data1 As List(Of String())
    End Class
End Namespace
