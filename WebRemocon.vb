Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Net

'★参幸にしたサイト★
'★簡易Webサーバを実装するには？
'☆http://www.atmarkit.co.jp/fdotnet/dotnettips/695httplistener/httplistener.html
'
'★HttpListener BeginGetContext を使わないと並列にリクエストを処理できない気がする。。。
'http://d.hatena.ne.jp/m_yamamo0417/20091220/1261324204
'http://msdn.microsoft.com/ja-jp/library/system.net.httplistener.begingetcontext%28v=vs.110%29.aspx
'
'★.NETを使った簡易HTTPサーバーの実装
'http://ivis-mynikki.blogspot.jp/2011/02/nethttp.html
'
'★DOSコマンドを実行し出力データを取得する
'http://dobon.net/vb/dotnet/process/standardoutput.html
'
'★外部アプリケーションを起動して終了まで待機する
'http://dobon.net/vb/dotnet/process/openfile.html
'
'★PT2/Friioの映像をiPhone/Kindle Fire HDで見る
'http://frmmpgit.blog.fc2.com/blog-entry-127.html
'
Class WebRemocon
    Public _isWebStart As [Boolean] = False
    Private _listener As HttpListener = Nothing
    Private _procMan As ProcessManager = Nothing

    'form1から変更されるパラメーター
    Public _udpApp As String = Nothing
    Public _udpOpt3 As String = Nothing
    Public _chSpace As Integer = Nothing
    Public _hlsApp As String = Nothing
    Public _hlsroot As String = Nothing
    Public _hlsOpt1 As String = Nothing
    Public _hlsOpt2 As String = Nothing
    Public _BonDriverPath As String = Nothing
    Public _ShowConsole As Boolean = Nothing
    Public _id As String
    Public _pass As String

    '変更されたら再起動が必要なパラメーター
    Private _udpPort As Integer = Nothing
    Private _wwwroot As String = Nothing
    Private _fileroot As String = Nothing
    Private _wwwport As Integer = Nothing
    Private _BonDriver_NGword As String() = Nothing
    Public _videopath() As String
    Public _AddSubFolder As Integer
    'NHK音声モード
    '0=主副ステレオ 1=主モノラル固定 2=副モノラル固定 3=選択式 9=VLC使用
    Public _NHK_dual_mono_mode As Integer = 0

    '空きUDPポートが取得できないので暫定
    Private _updCount As Integer

    'HLSのオプション用
    Public hls_option() As HLSoptionstructure
    Public vlc_option() As HLSoptionstructure
    Public vlc_http_option() As HLSoptionstructure
    Public ffmpeg_option() As HLSoptionstructure
    Public ffmpeg_http_option() As HLSoptionstructure
    Public Structure HLSoptionstructure
        Public resolution As String '解像度　"640x360"
        Public opt As String 'VLCオプション文字列
    End Structure

    '配信準備中 指定tsファイル数まで配信準備中にする
    Public _tsfile_wait As Integer = 3

    'MIME TYPE
    Public _MIME_TYPE_DEFAULT As String = ""
    Public _MIME_TYPE() As String

    Public Sub New(udpApp As String, udpPort As Integer, udpOpt3 As String, chSpace As Integer, hlsApp As String, hlsOpt1 As String, hlsOpt2 As String, wwwroot As String, fileroot As String, wwwport As Integer, BonDriverPath As String, ShowConsole As Boolean, BonDriver_NGword As String(), ByVal id As String, ByVal pass As String)
        'Public Sub New(udpPort As Integer, wwwroot As String, wwwport As Integer) ', num As Integer)
        '初期化 

        '一度WEBサーバーが起動したら変わらないパラメーターを読み込む（変更は要再起動）
        Me._udpPort = udpPort
        Me._wwwroot = wwwroot
        Me._wwwport = wwwport
        Me._fileroot = fileroot
        Me._BonDriver_NGword = BonDriver_NGword

        '現在ファームにセットされている値をセット
        Me._udpApp = udpApp
        Me._udpOpt3 = udpOpt3
        Me._hlsApp = hlsApp
        Dim ss As String = "\"
        Dim sp As Integer = hlsApp.LastIndexOf(ss)
        If sp > 0 Then
            Me._hlsroot = hlsApp.Substring(0, sp)
        Else
            Me._hlsroot = ""
        End If
        Me._hlsOpt1 = hlsOpt1
        Me._hlsOpt2 = hlsOpt2
        Me._chSpace = chSpace
        Me._BonDriverPath = BonDriverPath
        Me._ShowConsole = ShowConsole

        Me._id = id
        Me._pass = pass

        'VLCオプションを読み込む
        Me.read_hls_option()

        'ストリーム用インスタンス作成
        Me._procMan = New ProcessManager(udpPort, wwwroot, fileroot)
    End Sub

    'urlからMIMETYPEを取得
    Private Function get_mimetype(ByVal url As String) As String
        Dim r As String = ""

        Dim sp As Integer = url.LastIndexOf(".")
        If sp >= 0 Then
            Dim k As String = url.Substring(sp)
            If (url.Length - sp) = k.Length And k.Length > 1 Then
                k = k.Substring(1)
                'kに拡張子が入っている
                If Me._MIME_TYPE IsNot Nothing Then
                    For i As Integer = 0 To Me._MIME_TYPE.Length - 1
                        Dim d() As String = Me._MIME_TYPE(i).Split(":")
                        If d IsNot Nothing Then
                            If d.Length = 2 Then
                                If k = d(0) Then
                                    r = d(1)
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        End If

        Return r
    End Function

    '%変数に埋め込まれた前中後に挿入すべきhtmlタグを抽出
    Private Function get_atags(ByVal tag As String, ByVal s As String) As Object
        'tag="%～:" s=html
        '返値　d(0)=抽出タグ文字列 d(1)=前 d(2)=中 d(3)=後 d(4)=要素がnothingだった場合に替わりに表示するタグ
        Dim d() As String
        ReDim Preserve d(4)
        Dim vb1 As Integer = s.IndexOf(tag)
        Dim vb2 As Integer = s.IndexOf("%", vb1 + 1)
        Dim vbs As String = ""
        If vb1 >= 0 And vb2 > vb1 Then
            vbs = s.Substring(vb1 + tag.Length, vb2 - vb1 - tag.Length)
        End If
        Dim vbt() As String = vbs.Split(":")

        d(0) = vbs
        d(1) = ""
        d(2) = ""
        d(3) = ""
        d(4) = ""
        If vbt.Length = 2 Then
            d(2) = vbt(1)
        ElseIf vbt.Length = 3 Then
            d(2) = vbt(1)
            d(3) = vbt(2)
        ElseIf vbt.Length = 4 Then
            d(1) = vbt(1)
            d(2) = vbt(2)
            d(3) = vbt(3)
        ElseIf vbt.Length = 5 Then
            d(1) = vbt(1)
            d(2) = vbt(2)
            d(3) = vbt(3)
            d(4) = vbt(4)
        End If

        Return d
    End Function

    '%SELECTCHをhtmlに置換して返す
    Private Function replace_html_selectch(ByVal num As Integer, ByVal rez As String, ByVal atag() As String, ByVal NHKMODE As Integer) As String
        Dim bon As String = Me._procMan.get_bondriver_name(num)

        Dim bonp As String = Me._BonDriverPath
        If bonp.Length = 0 Then
            '指定が無い場合はUDPAPPと同じフォルダにあると見なす
            bonp = filepath2path(Me._udpApp.ToString)
        End If
        Dim vhtml As String = WEB_search_ServiceID(bonp, bon, 1, num)
        If vhtml.Length > 0 Then
            vhtml = "<option value="""">---</option>" & vbCrLf & vhtml
            vhtml = "<select name=""Bon_Sid_Ch"" id=""SEL2"" onChange=""changeSelect()"">" & vbCrLf & vhtml
            vhtml = "<form action=""StartTV.html"">" & vbCrLf & vhtml
            vhtml = atag(1) & vhtml
            vhtml &= "</select>" & vbCrLf
            vhtml &= atag(2)

            'NHKかどうか調べる
            'If Me._procMan.check_isNHK(num) = 1 Then
            'NHKなら
            If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                If NHKMODE = 3 Then
                    Dim atag2(3) As String
                    vhtml &= "<span id=""NHKVIEW"">" & WEB_make_NHKMODE_html(atag2, num) & "</span>"
                Else
                    vhtml &= "<input type=""hidden"" name=""NHKMODE"" value=""" & NHKMODE & """>" & vbCrLf
                End If
            End If
            'End If

            vhtml &= "<input type=""submit"" value=""視聴"" />" & vbCrLf
            vhtml &= "<input type=""hidden"" name=""num"" value=""" & num & """>" & vbCrLf
            vhtml &= "<input type=""hidden"" name=""redirect"" value=""ViewTV" & num & ".html"">" & vbCrLf
            If rez.Length > 0 Then
                vhtml &= "<input type=""hidden"" name=""resolution"" value=""" & rez & """>" & vbCrLf
            End If
            vhtml &= "</form>" & vbCrLf
            vhtml &= atag(3)
        End If

        Return vhtml
    End Function

    'ファイル再生ページ用　ビデオ選択htmlを作成する
    Private Function make_file_select_html() As String
        Dim shtml As String = ""

        Dim i, k As Integer
        If Me._videopath IsNot Nothing Then
            If Me._videopath.Length > 0 Then
                For i = 0 To Me._videopath.Length - 1
                    If Me._videopath(i).Length > 0 Then
                        Try
                            For Each stFilePath As String In System.IO.Directory.GetFiles(Me._videopath(i), "*.*") ', "*.ts")
                                Dim s As String = stFilePath & System.Environment.NewLine
                                s = trim8(s)
                                'フルパスファイル名がsに入る
                                Dim fullpath As String = s
                                Dim filename As String = ""
                                If fullpath.IndexOf("\") >= 0 Then
                                    'ファイル名だけを取り出す
                                    k = fullpath.LastIndexOf("\")
                                    filename = fullpath.Substring(k + 1)
                                End If
                                filename = trim8(filename)
                                If filename.IndexOf(".db") < 0 Then
                                    'なぜかそのまま渡すと返ってきたときに文字化けするのでURLエンコードしておく
                                    fullpath = System.Web.HttpUtility.UrlEncode(fullpath)
                                    shtml &= "<option value=""" & fullpath & """>" & filename & "</option>" & vbCrLf
                                End If
                            Next stFilePath
                        Catch ex As Exception
                        End Try
                    End If
                Next
            End If
        End If

        'If shtml.Length > 0 Then
        shtml = "<option value="""">---</option>" & vbCrLf & shtml
        shtml = "<select name=""VideoName"">" & vbCrLf & shtml

        shtml &= "</select>" & vbCrLf
        'End If

        Return shtml
    End Function

    'エラーページ用ひな形
    Public Function ERROR_PAGE(ByVal title As String, ByVal body As String, Optional ByVal a As Integer = 0) As String
        Dim r As String = ""
        r &= "<!doctype html>" & vbCrLf
        r &= "<html>" & vbCrLf
        r &= "<head>" & vbCrLf
        r &= "<title>" & title & "</title>" & vbCrLf
        r &= "<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />" & vbCrLf
        r &= "</head>" & vbCrLf
        r &= "<body>" & vbCrLf
        r &= body & vbCrLf
        r &= "<br><br>" & vbCrLf
        r &= "<input type=""button"" value=""トップメニュー"" onClick=""location.href='/index.html'"">" & vbCrLf
        If a = 1 Then
            r &= "<input type=""button"" Value=""再読み込み"" onClick=""location.reload();"">" & vbCrLf
        End If
        r &= "<br><br>" & vbCrLf
        r &= "<input type=""button"" value=""直前のページへ戻る"" onClick=""history.go(-1);"">" & vbCrLf
        r &= "</body>" & vbCrLf
        r &= "</html>" & vbCrLf
        Return r
    End Function

    'ＮＨＫ音声選択用セレクト作成
    Public Function WEB_make_NHKMODE_html(ByVal atag() As String, ByVal num As Integer) As String
        Dim html As String = ""
        Dim bst As String = ""
        html &= atag(1)
        html &= "<select name=""NHKMODE"">"
        html &= vbCrLf & "<option value=""0"">主・副</option>" & vbCrLf
        html &= "<option value=""1"">主</option>" & vbCrLf
        html &= "<option value=""2"">副</option>" & vbCrLf
        If BS1_hlsApp.Length > 0 Then
            html &= "<option value=""9"">VLCで再生</option>" & vbCrLf
        End If
        html &= "</select>" & vbCrLf
        html &= atag(3)
        '現在放映中なら選択する
        Dim NHKmode As Integer = Me._procMan.get_NHKmode(num)
        If NHKmode >= 0 Then
            html = html.Replace("value=""" & NHKmode.ToString & """", "value=""" & NHKmode.ToString & """" & " selected")
        End If

        Return html
    End Function

    'ファイルのWEB用相対位置を返す
    Public Function get_soutaiaddress_from_fileroot() As String
        Dim fileroot As String = Me._fileroot
        If Me._fileroot.Length = 0 Then
            fileroot = Me._wwwroot
        End If
        '相対アドレスに変換
        fileroot = fileroot.Replace(Me._wwwroot, "")
        If fileroot.Length > 0 Then
            '先頭が\なら削除
            If fileroot.Substring(0, 1) = "\" Then
                Try
                    fileroot = fileroot.Substring(1)
                Catch ex As Exception
                End Try
            End If
            fileroot = fileroot.Replace("\", "/")
            fileroot &= "/"
        End If

        Return fileroot
    End Function

    'HTML内置換用　配信しているストリームのボタンを作成
    Public Function WEB_make_ViewLink_html(ByVal atag() As String) As String
        Dim html As String = ""
        Dim bst As String = ""

        Dim gln As String = Trim(Me._procMan.get_live_numbers())
        If gln.Length > 0 Then
            Dim d() As String = gln.Split(" ")
            Dim e() As Integer = Me._procMan.get_live_stream_mode() 'stream_modeを取得　並び順はglnと同じはず
            Dim f() As Integer = Me._procMan.get_live_hlsApp() 'hlsAppの種類を取得　並び順はglnと同じはず vlc= 1 ffmpeg=2
            For i As Integer = 0 To d.Length - 1
                Dim chkstr As String = ""
                If d(i).IndexOf("x") >= 0 Then
                    chkstr = "[X]" '配信停止中
                End If
                d(i) = d(i).Replace("x", "")
                If Val(d(i)) > 0 Then
                    html &= bst
                    Dim ChannelName As String = Me._procMan.get_channelname(Val(d(i)))
                    If ChannelName.Length > 20 Then
                        '長すぎるときはカット
                        ChannelName = ChannelName.Substring(0, 18) & ".."
                    End If

                    'stream_mode = 2 VLCによるhttpストリームならば
                    Dim sm_str As String = ""
                    Dim sm_str2 As String = ""
                    Dim sm_str3 As String = ""
                    Try
                        If e(i) = 2 Or e(i) = 3 Then
                            sm_str = HTTPSTREAM_mode2_str '目印を付ける
                            sm_str2 = " " & sm_str
                            sm_str3 = sm_str & "_"
                        End If
                    Catch ex As Exception
                    End Try

                    If (e(i) = 2 Or e(i) = 3) And f(i) = 2 And HTTPSTREAM_WEB_view_ts = 1 Then
                        'HTTPストリーム ffmpeg
                        Dim fileroot As String = get_soutaiaddress_from_fileroot()
                        If ChannelName.Length > 0 Then
                            html &= "<input type=""button"" value=""" & chkstr & d(i) & "　" & ChannelName & sm_str2 & """ onClick=""location.href='" & fileroot & "mystream" & d(i).ToString & ".ts'"">" & vbCrLf
                        Else
                            html &= "<input type=""button"" value=""ストリーム" & chkstr & d(i) & "を視聴" & sm_str2 & """ onClick=""location.href='" & fileroot & "mystream" & d(i).ToString & ".ts'"">" & vbCrLf
                        End If
                    Else
                        'HLS再生ボタン or HTTPストリームは再生できないボタン
                        If ChannelName.Length > 0 Then
                            'html &= "<input type=""button"" value=""" & chkstr & d(i) & "　" & ChannelName & """ onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                            html &= "<input type=""button"" value=""" & chkstr & d(i) & "　" & ChannelName & sm_str2 & """ onClick=""location.href='" & sm_str3 & "ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                        Else
                            'html &= "<input type=""button"" value=""ストリーム" & chkstr & d(i) & "を視聴"" onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                            html &= "<input type=""button"" value=""ストリーム" & chkstr & d(i) & "を視聴" & sm_str2 & """ onClick=""location.href='" & sm_str3 & "ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                        End If
                    End If

                    'html &= "<form action=""ViewTV" & d(i).ToString & ".html"">" & vbCrLf
                    'html &= "    <input type=""submit"" value=""ストリーム" & d(i).ToString & "を視聴"" />" & vbCrLf
                    'html &= "</form>" & vbCrLf
                    bst = atag(2)
                End If
            Next
        End If

        If html.Length > 0 Then
            html = atag(1) & html & atag(3) & vbCrLf
        End If

        Return html
    End Function

    'HTML内置換用　BonDriverセレクトボックスを作成
    Public Function WEB_make_select_Bondriver_html(ByVal atag() As String) As String
        's=BonDriverセレクトと番組セレクトの間に入れるhtmlタグ <br>とか
        Dim html As String = ""
        Dim bons() As String = Nothing
        Dim bons_n As Integer = 0

        If html_selectbonsidch_a.Length = 0 And html_selectbonsidch_b.Length = 0 Then
            '初めの1回　まだhtmlができていない
            Dim bondriver_path As String = Me._BonDriverPath.ToString
            If bondriver_path.Length = 0 Then
                '指定が無い場合はUDPAPPと同じフォルダにあると見なす
                bondriver_path = filepath2path(Me._udpApp.ToString)
            End If
            Try
                For Each stFilePath As String In System.IO.Directory.GetFiles(bondriver_path, "*.dll")
                    Dim s As String = stFilePath & System.Environment.NewLine
                    'フルパスファイル名がsに入る
                    Dim fpf As String = trim8(s)
                    If s.IndexOf("\") >= 0 Then
                        'ファイル名だけを取り出す
                        Dim k As Integer = s.LastIndexOf("\")
                        s = trim8(s.Substring(k + 1))
                    End If
                    Dim sl As String = s.ToLower() '小文字に変換
                    '表示しないBonDriverかをチェック
                    If Me._BonDriver_NGword IsNot Nothing Then
                        For j As Integer = 0 To Me._BonDriver_NGword.Length - 1
                            If sl.IndexOf(Me._BonDriver_NGword(j)) >= 0 Then
                                sl = ""
                            End If
                        Next
                    End If
                    If sl.IndexOf("bondriver") = 0 Then
                        'セレクトボックス用にBonDriverを記録しておく
                        ReDim Preserve bons(bons_n)
                        bons(bons_n) = sl
                        bons_n += 1
                    End If
                Next
            Catch ex As Exception
            End Try

            Dim i As Integer = 0
            If bons IsNot Nothing Then
                If bons.Length > 0 Then
                    'BonDriver一覧

                    html_selectbonsidch_a &= "<script type=""text/javascript"" src=""ConnectedSelect.js""></script>" & vbCrLf
                    html_selectbonsidch_a &= "<select id=""SEL1"" name=""BonDriver"">" & vbCrLf
                    html_selectbonsidch_a &= "<option value="""">---</option>" & vbCrLf
                    For i = 0 To bons.Length - 1
                        html_selectbonsidch_a &= "<option value=""" & bons(i) & """>" & bons(i) & "</option>" & vbCrLf
                    Next
                    html_selectbonsidch_a &= "</select>" & vbCrLf

                    '各BonDriverに対応したチャンネルを書き込む
                    html_selectbonsidch_b &= "<select id=""SEL2"" name=""Bon_Sid_Ch"" onChange=""changeSelect()"">" & vbCrLf
                    html_selectbonsidch_b &= "<option value="""">---</option>" & vbCrLf
                    For i = 0 To bons.Length - 1
                        html_selectbonsidch_b &= "<optgroup label=""" & bons(i) & """>" & vbCrLf
                        '局名を書き込む
                        html_selectbonsidch_b &= WEB_search_ServiceID(bondriver_path, bons(i), 0)
                        html_selectbonsidch_b &= "</optgroup>" & vbCrLf
                    Next
                    html_selectbonsidch_b &= "</select>" & vbCrLf
                    html_selectbonsidch_b &= "<script type=""text/javascript"">" & vbCrLf
                    html_selectbonsidch_b &= "ConnectedSelect(['SEL1','SEL2']);" & vbCrLf
                    html_selectbonsidch_b &= "</script>" & vbCrLf
                End If
            End If
        End If

        html &= atag(1)
        html &= html_selectbonsidch_a
        html &= atag(2)
        html &= html_selectbonsidch_b
        html &= atag(3)

        Return html
    End Function

    'HTML内置換用　番組選局セレクトボックスを作成（WEB_make_select_Bondriver_html補助）
    Private Function WEB_search_ServiceID(ByVal bondriver_path As String, ByVal bondriver As String, ByVal BonDriverWrite As Integer, Optional ByVal num As Integer = 0) As String
        Dim html As String = ""
        Dim ichiran As String = ""
        If bondriver.Length > 0 Then
            Dim k As Integer = -1
            If BonDriver_select_html IsNot Nothing Then
                If BonDriver_select_html.Length > 0 Then
                    k = Array.IndexOf(BonDriver_select_html, bondriver)
                End If
            End If
            If k >= 0 Then
                html = BonDriver_select_html(k).html
            Else
                '追加
                Dim j As Integer = 0
                If BonDriver_select_html IsNot Nothing Then
                    j = BonDriver_select_html.Length
                End If
                ReDim Preserve BonDriver_select_html(j)

                'サービスIDと放送局名用
                Dim si As Integer = 0
                If ch_list IsNot Nothing Then
                    si = ch_list.Length
                End If

                Dim filename As String
                If bondriver_path.Length > 0 Then
                    filename = bondriver_path & "\" & bondriver.Replace(".dll", ".ch2")
                Else
                    filename = bondriver.Replace(".dll", ".ch2")
                End If
                Dim line() As String = file2line(filename)
                If line IsNot Nothing Then
                    For i As Integer = 0 To line.Length - 1
                        If line(i).IndexOf(";") < 0 Then
                            Dim s() As String = line(i).Split(",")
                            If s.Length = 9 Then
                                'If BonDriverWrite = 1 Then
                                html &= "<option value=""" & bondriver & "," & s(5) & "," & s(1) & """>" & s(0) & "</option>" & vbCrLf
                                'Else
                                ''Bondriverはすでに設定済みなので字数節約のため空白
                                'html &= "<option value="" ," & s(5) & "," & s(1) & """>" & s(0) & "</option>" & vbCrLf
                                'End If

                                'クライアント用一覧
                                ichiran &= bondriver & "," & s(5) & "," & s(1) & "," & s(0) & vbCrLf

                                'serviceIDと放送局名を記録しておく
                                If ch_list IsNot Nothing Then
                                    Dim chk As Integer = 0
                                    For i2 As Integer = 0 To ch_list.Length - 1
                                        If ch_list(i2).sid = Val(s(5)) And ch_list(i2).tsid = s(7) Then
                                            'サービスIDとTSIDが一致した
                                            'すでに登録済み
                                            chk = 1
                                            Exit For
                                        End If
                                    Next
                                    If chk = 0 Then
                                        'まだ登録されていなければ
                                        ReDim Preserve ch_list(si)
                                        ch_list(si).sid = Val(s(5))
                                        ch_list(si).jigyousha = s(0)
                                        ch_list(si).bondriver = bondriver
                                        ch_list(si).chspace = Val(s(1))
                                        ch_list(si).tsid = Val(s(7))
                                        si += 1
                                    End If
                                Else
                                    '最初の１つめ
                                    ReDim Preserve ch_list(0)
                                    ch_list(0).sid = Val(s(5))
                                    ch_list(0).jigyousha = s(0)
                                    ch_list(0).bondriver = bondriver
                                    ch_list(0).chspace = Val(s(1))
                                    ch_list(0).tsid = Val(s(7))
                                    si += 1
                                End If
                            End If
                        End If
                    Next
                End If

                'キャッシュに記録
                BonDriver_select_html(j).BonDriver = bondriver
                BonDriver_select_html(j).html = html
                BonDriver_select_html(j).ichiran = ichiran
            End If
        End If

        If num > 0 Then
            '配信ストリームの指定があった場合は放送局名を取得してselectedする
            Dim chname As String = Me._procMan.get_channelname(num)
            Dim sp As Integer = html.IndexOf(">" & chname.ToString & "<")
            If sp > 0 Then
                html = html.Substring(0, sp) & " selected" & html.Substring(sp)
            End If
        End If

        Return html
    End Function

    '配信準備中の進歩度を返す
    Private Function check_m3u8_ts_status(ByVal num As Integer) As Integer
        Dim r As Integer = 0 '1=m3u8無,ts無、2=m3u8有、123ts無、3=m3u8有、～ts有
        'm3u8が存在していればViewTV1_waiting.htmlのrefresh先を書き換える
        Dim fileroot As String = Me._fileroot
        If fileroot.Length = 0 Then
            fileroot = Me._wwwroot
        End If
        fileroot &= "\"

        ' 必要な変数を宣言する
        Dim stPrompt As String = String.Empty
        Dim s As String

        If file_exist(fileroot & "mystream" & num.ToString & ".m3u8") <= 0 Then
            'm3u8無し
            r = 0
        Else
            'm3u8有り
            'tsチェック
            Dim ts_count As Integer = 0
            For Each stFilePath As String In System.IO.Directory.GetFiles(fileroot, "mystream" & num.ToString & "-*.ts")
                s = stFilePath & System.Environment.NewLine
                ts_count += 1
            Next

            r = ts_count
        End If

        Return r
    End Function

    '余計な改行等を削除
    Public Function trim8(ByVal s As String) As String
        s = Trim(s)
        s = s.Replace(vbTab, "").Replace(vbCrLf, "").Replace("""", "")
        s = Trim(s)
        Return s
    End Function

    'HTTPサーバー停止
    Public Sub requestStop()
        Me._isWebStart = False
        Me._listener.Abort()
    End Sub

    'HLS_option.txtから解像度とHLSオプションを読み込む
    Public Sub read_hls_option()
        'カレントディレクトリ変更
        F_set_ppath4program()

        hls_option = set_hls_option("HLS_option.txt")
        vlc_option = set_hls_option("HLS_option_VLC.txt")
        vlc_http_option = set_hls_option("HLS_option_VLC_http.txt")
        ffmpeg_option = set_hls_option("HLS_option_ffmpeg.txt")
        ffmpeg_http_option = set_hls_option("HLS_option_ffmpeg_http.txt")
    End Sub

    Public Function set_hls_option(ByVal filename As String) As Object
        Dim r() As HLSoptionstructure = Nothing
        Dim i As Integer = 0

        'カレントディレクトリ変更
        F_set_ppath4program()

        Try
            Dim line() As String = file2line(filename)
            If line Is Nothing Then
            Else
                For i = 0 To line.Length - 1
                    Dim youso() As String = line(i).Split("]")
                    If youso Is Nothing Then
                    Else
                        If youso.Length = 2 Then
                            ReDim Preserve r(i)
                            r(i).resolution = Trim(youso(0)).Replace("[", "")
                            r(i).opt = youso(1)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
        End Try

        Return r
    End Function

    Public Sub stop_movie(ByVal num As Integer)
        '★起動しているアプリを止める
        Me._procMan.stopProc(num)
    End Sub

    Public Sub check_crash_dialog()
        'crashダイアログが出ていたら消す
        Me._procMan.check_crash_dialog()
    End Sub

    Public Sub checkAllProc()
        'プロセスがうまく動いているかチェック
        Me._procMan.checkAllProc()
    End Sub

    Public Sub stopProc(ByVal num As Integer)
        'プロセスを停止する
        Me._procMan.stopProc(num)
    End Sub

    Public Sub convert_m3u8_xspf()
        'm3u8をxspfに変換する
        Me._procMan.convert_m3u8_xspf()
    End Sub

    'プロセスを名前指定で停止
    Public Sub stopProcName(ByVal name As String)
        Me._procMan.stopProcName(name)
    End Sub

    '現在稼働中のストリームナンバーを取得
    Public Function get_live_numbers() As String
        Dim r As String
        r = Me._procMan.get_live_numbers()
        Return r
    End Function

    '古いTSファイルを削除する　ffmpeg用
    Public Sub delete_old_TS()
        Dim fileroot As String = Me._fileroot.ToString
        If fileroot.Length = 0 Then
            'm3u8やtsの格納場所が指定されていなければwwwrootと同じ場所とする
            fileroot = Me._wwwroot.ToString
        End If

        Me._procMan.delete_old_TS(fileroot)
    End Sub

    'ファイル再生用パスを読み込む
    Public Sub read_videopath()
        Dim errstr As String = ""
        Dim line() As String = Nothing

        'カレントディレクトリ変更
        F_set_ppath4program()

        If file_exist("VideoPath.txt") Then
            line = file2line("VideoPath.txt")
            log1write("設定ファイルとして VideoPath.txt を読み込みました")
        Else
            line = file2line("TvRemoteViewer_VB.ini")
            log1write("設定ファイルとして TvRemoteViewer_VB.ini を読み込みました")
        End If

        Dim i, j As Integer

        Try
            If line Is Nothing Then
            ElseIf line.Length > 0 Then
                '読み込み完了
                For i = 0 To line.Length - 1
                    line(i) = trim8(line(i))
                    'コメント削除
                    If line(i).IndexOf(";") >= 0 Then
                        line(i) = line(i).Substring(0, line(i).IndexOf(";"))
                    End If
                    If line(i).IndexOf("#") >= 0 Then
                        line(i) = line(i).Substring(0, line(i).IndexOf("#"))
                    End If
                    Dim youso() As String = line(i).Split("=")
                    If youso Is Nothing Then
                    ElseIf youso.Length > 1 Then
                        For j = 0 To youso.Length - 1
                            youso(j) = trim8(youso(j))
                        Next
                        Select Case youso(0)
                            Case "VideoPath"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve Me._videopath(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        Me._videopath(j) = trim8(clset(j))
                                    Next
                                End If
                            Case "AddSubFolder"
                                Me._AddSubFolder = Val(youso(1).ToString)
                            Case "BS1_hlsApp"
                                BS1_hlsApp = trim8(youso(1).ToString)
                            Case "TvProgramD"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgram_ch(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgram_ch(j) = Val(trim8(clset(j)))
                                    Next
                                End If
                            Case "TvProgramD_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgram_NGword(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgram_NGword(j) = trim8(clset(j))
                                    Next
                                End If
                            Case "TvProgramD_BonDriver1st"
                                TvProgramD_BonDriver1st = trim8(youso(1).ToString)
                            Case "TvProgramS_BonDriver1st"
                                TvProgramS_BonDriver1st = trim8(youso(1).ToString)
                            Case "TvProgram_tvrock_url"
                                TvProgram_tvrock_url = trim8(youso(1).ToString)
                            Case "TvProgram_EDCB_url"
                                TvProgram_EDCB_url = trim8(youso(1).ToString)
                            Case "TvProgramD_channels"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramD_channels(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        '全角に変換
                                        TvProgramD_channels(j) = StrConv(trim8(clset(j)), VbStrConv.Wide)
                                    Next
                                End If
                            Case "TvProgramEDCB_channels"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramEDCB_channels(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        '全角に変換
                                        TvProgramEDCB_channels(j) = StrConv(trim8(clset(j)), VbStrConv.Wide)
                                    Next
                                End If
                            Case "TvProgramTvRock_channels"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramTvRock_channels(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        '全角に変換
                                        TvProgramTvRock_channels(j) = StrConv(trim8(clset(j)), VbStrConv.Wide)
                                    Next
                                End If
                            Case "TvProgramD_sort"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramD_sort(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        '全角に変換
                                        TvProgramD_sort(j) = StrConv(trim8(clset(j)), VbStrConv.Wide)
                                    Next
                                End If
                            Case "Stop_RecTask_at_StartQuit", "Stop_RecTask_at_StartEnd"
                                Stop_RecTask_at_StartEnd = Val(youso(1).ToString)
                            Case "NHK_dual_mono_mode"
                                Me._NHK_dual_mono_mode = Val(youso(1).ToString)
                            Case "tsfile_wait"
                                Me._tsfile_wait = Val(youso(1).ToString)
                                If Me._tsfile_wait <= 0 Then
                                    Me._tsfile_wait = 3
                                End If
                            Case "MIME_TYPE_DEFAULT"
                                Me._MIME_TYPE_DEFAULT = trim8(youso(1))
                            Case "MIME_TYPE"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve Me._MIME_TYPE(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        Me._MIME_TYPE(j) = trim8(clset(j))
                                    Next
                                End If
                            Case "PipeListGetter"
                                PipeListGetter = trim8(youso(1).ToString)
                            Case "HTTPSTREAM_App"
                                HTTPSTREAM_App = Val(youso(1).ToString)
                            Case "HTTPSTREAM_VLC_port"
                                HTTPSTREAM_VLC_port = Val(youso(1).ToString)
                            Case "MAX_STREAM_NUMBER"
                                MAX_STREAM_NUMBER = Val(youso(1).ToString)
                        End Select
                    End If
                Next
            End If
        Catch ex As Exception
        End Try

    End Sub

    'ファイル再生
    '現在のhlsOptをファイル再生用に書き換える
    Private Function hlsopt_udp2file_ffmpeg(ByVal hlsOpt As String, ByVal filename As String) As String
        Dim sp As Integer = hlsOpt.IndexOf("-i ")
        Dim se As Integer = hlsOpt.IndexOf(" ", sp + 3)
        If sp >= 0 And se > sp Then
            hlsOpt = hlsOpt.Substring(0, sp) & "-i """ & filename & """" & hlsOpt.Substring(se)
            sp = hlsOpt.IndexOf("-segment_list_size ")
            se = hlsOpt.IndexOf(" ", sp + "-segment_list_size ".Length)
            If sp >= 0 And se > sp Then
                hlsOpt = hlsOpt.Substring(0, sp) & hlsOpt.Substring(se)
            End If
        End If
        Return hlsOpt
    End Function

    'ファイル再生
    '現在のhlsOptをファイル再生用に書き換える
    Private Function hlsopt_udp2file_vlc(ByVal hlsOpt As String, ByVal filename As String) As String
        Dim sp As Integer = hlsOpt.IndexOf("udp://@:")
        Dim se As Integer = hlsOpt.IndexOf(" ", sp + 7)
        If sp >= 0 And se > sp Then
            hlsOpt = hlsOpt.Substring(0, sp) & """" & filename & """" & hlsOpt.Substring(se)
        End If
        Return hlsOpt
    End Function

    'BS1用にffmpeg用オプションからvlcオプションに書き換え
    Public Function translate_ffmpeg2vlc(hlsOpt As String, ByVal stream_mode As Integer) As String
        Dim r As String = ""

        'HLS_option_VLC.txtを読み込む
        Dim ho_vlc() As HLSoptionstructure = Nothing
        If stream_mode = 2 Or stream_mode = 3 Then
            ho_vlc = vlc_http_option
        Else
            ho_vlc = vlc_option
        End If

        If ho_vlc IsNot Nothing Then
            If ho_vlc.Length > 0 Then
                '解像度を抜き出す
                '_listから取得resolutionを取得
                Dim rez As String = ""
                Dim sp1, sp2 As Integer
                Try
                    sp1 = hlsOpt.IndexOf("-s ")
                    sp2 = hlsOpt.IndexOf(" ", sp1 + 3)
                    rez = hlsOpt.Substring(sp1 + "-s ".Length, sp2 - sp1 - "-s ".Length)
                Catch ex As Exception
                    rez = ""
                End Try
                'チェック
                Dim d() As String = rez.Split("x")
                If d.Length = 2 Then
                    If Val(d(0)) > 0 And Val(d(1)) > 0 Then
                        'ok
                    Else
                        'エラーの場合はデフォルト
                        rez = "640x360"
                    End If
                Else
                    'エラーの場合はデフォルト
                    rez = "640x360"
                End If

                '解像度に合ったオプションを取り出す
                For i As Integer = 0 To ho_vlc.Length - 1
                    If ho_vlc(i).resolution = rez Then
                        Try
                            r = ho_vlc(i).opt
                            Exit For
                        Catch ex As Exception
                            'エラーが起こったときは
                            r = ""
                        End Try
                    End If
                Next
            End If

            If r.Length = 0 Then
                'どうしてもエラーの場合は無変換
                If r.Length = 0 Then
                    'すでに読み込み済みのhls_optionから無変換を取り出す
                    For i = 0 To ho_vlc.Length - 1
                        If ho_vlc(i).opt = "無変換" Then
                            r = ho_vlc(i).opt
                            Exit For
                        End If
                    Next
                End If
            End If
        End If

        Return r
    End Function

    '映像配信開始
    Public Sub start_movie(ByVal num As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal ChSpace As Integer, ByVal udpApp As String, ByVal hlsApp As String, hlsOpt1 As String, ByVal hlsOpt2 As String, ByVal wwwroot As String, ByVal fileroot As String, ByVal hlsroot As String, ByVal ShowConsole As Boolean, ByVal udpOpt3 As String, ByVal filename As String, ByVal NHK_dual_mono_mode_select As Integer, ByVal Stream_mode As Integer, Optional ByVal resolution As String = "")
        'resolutionの指定が無ければフォーム上のHLSオプションを使用する

        If num > MAX_STREAM_NUMBER Or num < 0 Then
            log1write("最大配信ナンバーを超えています")
            Exit Sub
        End If

        If fileroot.Length = 0 Then
            'm3u8やtsの格納フォルダが指定されていなければwwwrootと同じ場所
            fileroot = wwwroot
        End If

        '★リクエストパラメーターを取得
        Dim opt_serviceID As String = "/sid " & sid.ToString
        Dim opt_bondriver As String
        If Me._BonDriverPath.Length > 0 Then
            opt_bondriver = "/d """ & Me._BonDriverPath & "\" & bondriver & """"
        Else
            opt_bondriver = "/d " & bondriver
        End If

        Dim udpPortNumber As Integer = 0
        '★UDPポート自動取得か確認
        'If Me.checkBoxAutoUdpPort.Checked Then
        udpPortNumber = Me._procMan.getEmptyUdpPort(num)
        'End If

        '★UDPオプションの生成
        Dim udpOpt As String
        udpOpt = "/udp /port " & udpPortNumber & " /chspace " & ChSpace.ToString & " " & opt_serviceID & " " & opt_bondriver
        If udpOpt3.Length > 0 Then
            udpOpt &= " " & Trim(udpOpt3)
        End If

        'log1write("UDP option=" & udpOpt)

        '★HLSオプションの生成
        Dim hlsOpt As String = ""
        If resolution.Length > 0 Then
            '解像度指定があれば
            Dim chk As Integer = 0
            If hls_option IsNot Nothing Then
                For i As Integer = 0 To hls_option.Length - 1
                    If hls_option(i).resolution = resolution Then
                        hlsOpt = hls_option(i).opt
                        chk = 1
                        log1write("解像度指定がありました。" & resolution)
                    End If
                Next
            End If
            If chk = 0 And (Stream_mode = 0 Or Stream_mode = 1) Then
                '該当がなければフォーム上のVLCオプション文字列を使用する
                hlsOpt = hlsOpt2
                resolution = ""
            End If
        Else
            '解像度指定がなければフォーム上のVLCオプション文字列を使用する
            hlsOpt = hlsOpt2
        End If

        'NHK BS1、BSプレミアム対策
        If hlsApp.IndexOf("ffmpeg") >= 0 Then
            'まずこの放送がＮＨＫかどうか
            Dim isNHK As Integer = Me._procMan.check_isNHK(0, udpOpt)
            If isNHK = 1 Then
                If NHK_dual_mono_mode_select = 1 And hlsOpt.IndexOf("-dual_mono_mode") < 0 Then
                    '主モノラル固定
                    hlsOpt = hlsOpt.Replace("-i ", "-dual_mono_mode main -i ")
                ElseIf NHK_dual_mono_mode_select = 2 And hlsOpt.IndexOf("-dual_mono_mode") < 0 Then
                    '副モノラル固定
                    hlsOpt = hlsOpt.Replace("-i ", "-dual_mono_mode sub -i ")
                ElseIf NHK_dual_mono_mode_select = 9 Then
                    If BS1_hlsApp.Length > 0 Then
                        'hlsAppとhlsOptをVLCに置き換える
                        Dim hlsOpt_temp As String = translate_ffmpeg2vlc(hlsOpt, Stream_mode)
                        If hlsOpt_temp.Length > 0 Then
                            hlsOpt = hlsOpt_temp
                            hlsApp = BS1_hlsApp
                        End If
                    Else
                        NHK_dual_mono_mode_select = 0
                        log1write("VLCが指定されていないのでNHK_dual_mono_mode=0に変更します。")
                    End If
                End If
            End If
        End If

        'VLC http ストリーム用にhlsAppとhlsOptを入れ替える
        If Stream_mode = 2 Or Stream_mode = 3 Then
            'httpストリームアプリが指定されていれば
            If HTTPSTREAM_App = 1 Then
                'vlc指定
                If BS1_hlsApp.Length > 0 Then
                    If hlsApp.IndexOf("ffmpeg") >= 0 Then
                        hlsApp = BS1_hlsApp
                    End If
                Else
                    log1write("エラー：BS1_hlsAppが指定されていません")
                    Exit Sub
                End If
            ElseIf HTTPSTREAM_App = 2 Then
                'ffmpeg指定
                If hlsApp.IndexOf("vlc") >= 0 Then
                    'vlcからffmpegへの変換は未対応
                    log1write("VLCからffmpegへの変更は対応していません。VLCのまま続行します")
                End If
            End If

            'hlsOptをHTTPストリーム用のものに入れ替える
            If hlsApp.IndexOf("ffmpeg") >= 0 Then
                'hlsOptを置き換える
                hlsOpt = translate_hls2http(0, hlsOpt, resolution)

                'ファイル再生
                If Stream_mode = 3 Then
                    'VLC httpストリームのとき
                    hlsOpt = hlsopt_udp2file_ffmpeg(hlsOpt, filename)
                End If
            ElseIf hlsApp.IndexOf("vlc") >= 0 Then
                'hlsOptを置き換える
                hlsOpt = translate_hls2http(1, hlsOpt, resolution)
                'パスワードが設定されている場合
                'サーバー　Option := Format('%s --sout-http-user=%s --sout-http-pwd=%s', [Option, Username, Password]);
                'クライアント　Option := Format('http://%s:%s@%s --extraintf="rc" --rc-quiet --rc-host=127.0.0.1:%d', [Username, Password, Copy(AURL, 8, Length(AURL)-7), VLCPort]);
                If Me._id.Length > 0 And Me._pass.Length > 0 Then
                    Dim s1 As Integer = hlsOpt.IndexOf(" --")
                    If s1 > 0 Then
                        hlsOpt = hlsOpt.Substring(0, s1) & " --sout-http-user=" & Me._id & " --sout-http-pwd=" & Me._pass & " " & hlsOpt.Substring(s1)
                    End If
                End If
                'ファイル再生
                If filename.Length > 0 And Stream_mode = 3 Then
                    'VLC httpストリームのとき
                    hlsOpt = hlsopt_udp2file_vlc(hlsOpt, filename)
                End If
            End If
        ElseIf Stream_mode = 0 Or Stream_mode = 1 Then
            'ファイル再生か？
            If filename.Length > 0 Then
                Stream_mode = 1
                'hlsオプションを書き換える
                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                    'ffmpegのとき
                    hlsOpt = hlsopt_udp2file_ffmpeg(hlsOpt, filename)
                Else
                    'その他vlc
                    '今のところ未対応
                    Exit Sub
                End If
            End If
        End If

        '"%HLSROOT/../%"用
        Dim hlsroot2 As String = hlsroot
        Dim sp As Integer = hlsroot2.LastIndexOf("\")
        If sp > 0 Then
            hlsroot2 = hlsroot2.Substring(0, sp)
        End If
        '文字列内変数を実際の値に変換
        hlsOpt = hlsOpt.Replace("%UDPPORT%", udpPortNumber.ToString)
        hlsOpt = hlsOpt.Replace("mystream.", "mystream" & num.ToString & ".") 'ffmpeg,m3u8 無くしたいが互換性のため
        hlsOpt = hlsOpt.Replace("mystream-", "mystream" & num.ToString & "-") 'vlc 無くしたいが互換性のため
        hlsOpt = hlsOpt.Replace("%NUM%", num.ToString)
        hlsOpt = hlsOpt.Replace("%WWWROOT%", wwwroot)
        hlsOpt = hlsOpt.Replace("%FILEROOT%", fileroot)
        hlsOpt = hlsOpt.Replace("%HLSROOT%", hlsroot)
        hlsOpt = hlsOpt.Replace("%HLSROOT/../%", hlsroot2)
        hlsOpt = hlsOpt.Replace("%rc-host%", "127.0.0.1:" & udpPortNumber.ToString)
        'VLC HTTPストリーム用　UDPポート
        If HTTPSTREAM_VLC_port > 0 Then
            '指定があれば
            hlsOpt = hlsOpt.Replace("%VLCPORT%", (HTTPSTREAM_VLC_port + num - 1).ToString) '-1
        Else
            'VLC port udpPortに定数を足して作成することにした
            hlsOpt = hlsOpt.Replace("%VLCPORT%", (udpPortNumber + HTTPSTREAM_VLC_port_plus).ToString)
        End If

        'log1write("HLS option=" & hlsOpt)

        Try
            Directory.SetCurrentDirectory(fileroot) 'カレントディレクトリ変更
        Catch ex As Exception
            '設定しないうちにスタートしようとすると例外が起こる
            Exit Sub
        End Try
        '★プロセスを起動
        Me._procMan.startProc(udpApp, udpOpt, hlsApp, hlsOpt, num, udpPortNumber, ShowConsole, Stream_mode, NHK_dual_mono_mode_select, resolution)
    End Sub

    'hlsオプションをhttpストリームオプションに変換
    Public Function translate_hls2http(ByVal isVLC As Integer, ByVal hlsOpt As String, ByVal resolution As String) As String
        Dim r As String = ""

        Dim rez As String = ""
        Dim sp1 As Integer = -1
        Dim sp2 As Integer = -1

        '解像度が指定されていなければhlsOptから判別する
        If resolution.Length = 0 Then
            'vlcか？
            Dim x As Integer = Val(Trim(Instr_pickup(hlsOpt, "width=", ",", 0)))
            Dim y As Integer = Val(Trim(Instr_pickup(hlsOpt, "height=", ",", 0)))
            If x > 0 And y > 0 Then
                rez = x.ToString & "x" & y.ToString
            End If

            If rez.Length = 0 Then
                'ffmpegか？
                Try
                    sp1 = hlsOpt.IndexOf("-s ")
                    sp2 = hlsOpt.IndexOf(" ", sp1 + 3)
                    rez = hlsOpt.Substring(sp1 + "-s ".Length, sp2 - sp1 - "-s ".Length)
                Catch ex As Exception
                    Try
                        rez = hlsOpt.Substring(sp1 + "-s ".Length)
                    Catch ex2 As Exception
                        rez = ""
                    End Try
                End Try
                'チェック
                Dim d() As String = rez.Split("x")
                If d.Length = 2 Then
                    If Val(d(0)) > 0 And Val(d(1)) > 0 Then
                        'ok
                        rez = Val(d(0)).ToString & "x" & Val(d(1))
                    Else
                        'エラーの場合は無変換
                        rez = "無変換"
                    End If
                Else
                    'エラーの場合は無変換
                    rez = "無変換"
                End If
            End If
        Else
            rez = resolution
        End If

        'HLS_option.txtを読み込む
        Dim ho_vlc() As HLSoptionstructure = Nothing
        If isVLC = 0 Then
            ho_vlc = ffmpeg_http_option
        Else
            ho_vlc = vlc_http_option
        End If

        If ho_vlc IsNot Nothing Then
            '解像度に合ったオプションを取り出す
            For i As Integer = 0 To ho_vlc.Length - 1
                If ho_vlc(i).resolution = rez Then
                    r = ho_vlc(i).opt
                    Exit For
                End If
            Next
        End If

        'どうしてもエラーの場合は無変換
        If r.Length = 0 Then
            'すでに読み込み済みのhls_optionから無変換を取り出す
            For i = 0 To ho_vlc.Length - 1
                If ho_vlc(i).opt = "無変換" Then
                    r = ho_vlc(i).opt
                    Exit For
                End If
            Next
        End If

        ''カレントディレクトリを戻す
        'Try
        'Directory.SetCurrentDirectory(Me._fileroot) 'カレントディレクトリ変更
        'Catch ex As Exception
        'End Try

        Return r
    End Function

    '手動　ストリーム開始　■未使用
    Public Sub Sub_stream_Start(ByVal num As Integer, ByVal bondriver As String, ByVal sid As String, ByVal chspace As String, ByVal bon_sid_ch_str As String, ByVal resolution As String, ByVal stream_mode As Integer, ByVal videoname As String)
        'num        'bondriver        'sid        'chspace'        'bon_sid_ch_str        'resolution
        'stream_mode 'ストリームモード 0=UDP 1=ファイル再生 2=VLChttp 3=VLChttpファイル再生
        'videoname

        '★リクエストパラメーターを取得
        'Bon_Sid_Ch一括指定があった場合（JavaScript等でBon_Sid_Ch="BonDriver_t0.dll,12345,0"というように指定された場合）
        Dim bon_sid_ch() As String = bon_sid_ch_str.Split(",")
        If bon_sid_ch.Length = 3 Then
            '個別に値が決まっていなければセット
            If bondriver.Length = 0 Then bondriver = Trim(bon_sid_ch(0))
            If sid.Length = 0 Then sid = Trim(bon_sid_ch(1))
            If chspace.Length = 0 Then chspace = Trim(bon_sid_ch(2))
        End If

        '配信スタート
        'パラメーターが正しいかチェック
        If num > 0 And bondriver.Length > 0 And Val(sid) > 0 And Val(chspace) >= 0 Then
            '正しければ配信スタート
            Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, videoname, 0, stream_mode, resolution)
        ElseIf num > 0 And videoname.Length > 0 Then
            'ファイル再生
            Me.start_movie(num, "", 0, 0, "", Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, "", videoname, 0, stream_mode, resolution)
        End If

    End Sub

    'HTTPサーバー開始
    Public Sub Web_Start()
        If Me._wwwport = 0 Then
            MsgBox("httpサーバーの起動に失敗しました。" & vbCrLf & "httpポートを指定してこのアプリを再起動してください")
            Exit Sub
        End If
        If Me._udpPort = 0 Then
            MsgBox("httpサーバーの起動に失敗しました。" & vbCrLf & "UDPポートを指定してこのアプリを再起動してください")
            Exit Sub
        End If

        Me._isWebStart = True

        'string root = @"D:\html\"; // ドキュメント・ルート
        'Dim root As String = ".\html\"
        Dim root As String = Me._wwwroot
        ' ドキュメント・ルート
        '★ localhost以外の場合、UACが有効だとHttpListenerExceptionが発生する
        '★ 回避するには管理者権限で実行するか、コマンドプロンプトで
        '★ 「netsh http add urlacl url=http://+:40003/ user=Everyone」(ENTER)と実行する。

        'string prefix = "http://localhost:" + this._portNumber + "/"; // 受け付けるURL
        Dim prefix As String = "http://+:" & Me._wwwport & "/"
        ' 受け付けるURL
        Me._listener = New HttpListener()
        Me._listener.Prefixes.Add(prefix)
        'BASIC認証
        If Me._id.Length > 0 And Me._pass.Length > 0 Then
            'IDとパスが設定されていれば
            Me._listener.AuthenticationSchemes = AuthenticationSchemes.Basic
            Me._listener.Realm = "SECRET AREA"
        End If
        ' プレフィックスの登録
        Me._listener.Start()

        While Me._isWebStart
            Try
                Dim context As HttpListenerContext = Me._listener.GetContext()
                Dim req As HttpListenerRequest = context.Request
                Dim res As HttpListenerResponse = context.Response

                'MIME TYPE
                Dim mimetype As String = get_mimetype(req.Url.LocalPath)
                If mimetype.Length > 0 Then
                    res.ContentType = mimetype
                ElseIf Me._MIME_TYPE_DEFAULT.Length > 0 Then
                    res.ContentType = Me._MIME_TYPE_DEFAULT
                End If

                Dim auth_ok As Integer = 0
                If Me._id.Length = 0 Or Me._pass.Length = 0 Then
                    'パスワード未設定は素通り
                    auth_ok = 1
                ElseIf req.IsAuthenticated Then
                    Dim identity As HttpListenerBasicIdentity = DirectCast(context.User.Identity, HttpListenerBasicIdentity)
                    '判定
                    If Me._id = identity.Name And Me._pass = identity.Password Then
                        '受付
                        auth_ok = 2
                    Else
                        auth_ok = -1
                    End If
                Else
                    auth_ok = -2
                End If

                If auth_ok > 0 Then
                    ' リクエストされたURLからファイルのパスを求める
                    Dim path As String = root & req.Url.LocalPath.Replace("/", "\")

                    'ルートにアクセスされた場合、index.htmlを表示する
                    Dim se1 As Integer = path.LastIndexOf("\")
                    If se1 >= 0 And (se1 + 1) = path.Length Then
                        path = path & "index.html"
                    End If

                    log1write(req.Url.LocalPath & "へのリクエストがありました。")
                    If res.ContentType IsNot Nothing Then
                        If res.ContentType.Length > 0 Then
                            log1write("MIME TYPE : " & res.ContentType)
                        End If
                    End If

                    If path.IndexOf(".htm") > 0 Then
                        'HTMLなら

                        '反応が速くなるかなとこの1行を前に出してみたが何も変わらなかった・・
                        Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))

                        'ToLower小文字で比較
                        Dim StartTv_param As Integer = 0 'StartTvパラメーターが正常かどうか
                        Dim request_page As Integer = 0 '特別なリクエストかどうか
                        Dim chk_viewtv_ok As Integer = 0 'ViewTV.htmlへのリクエストなら1になる

                        '===========================================
                        '★リクエストパラメーターを取得
                        '===========================================
                        'スレッドナンバー
                        Dim num As Integer = 0
                        num = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("num") & "")
                        'Int32.TryParse(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("num"), num)
                        'BonDriver指定
                        Dim bondriver As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("BonDriver") & ""
                        'サービスＩＤ指定
                        Dim sid As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("ServiceID") & ""
                        'chspace指定
                        Dim chspace As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("ChSpace") & ""
                        'Bon_Sid_Ch一括指定があった場合（JavaScript等でBon_Sid_Ch="BonDriver_t0.dll,12345,0"というように指定された場合）
                        Dim bon_sid_ch_str As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("Bon_Sid_Ch") & ""
                        Dim bon_sid_ch() As String = bon_sid_ch_str.Split(",")
                        If bon_sid_ch.Length = 3 Then
                            '個別に値が決まっていなければセット
                            If bondriver.Length = 0 Then bondriver = Trim(bon_sid_ch(0))
                            If sid.Length = 0 Then sid = Trim(bon_sid_ch(1))
                            If chspace.Length = 0 Then chspace = Trim(bon_sid_ch(2))
                        End If
                        '解像度指定 "640x360"等
                        Dim resolution As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("resolution") & ""
                        'redirect指定
                        Dim redirect As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("redirect") & ""
                        'm3u8,tsの準備状況
                        Dim check_m3u8_ts As Integer = 0
                        'ストリームモード 0=UDP 1=ファイル再生
                        Dim stream_mode As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("StreamMode") & "")
                        'ファイル名
                        Dim videoname As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoName") & ""
                        'URLエンコードしておいたフルパスを文字列に変換
                        videoname = System.Web.HttpUtility.UrlDecode(videoname)
                        'NHKの音声モード
                        Dim NHK_dual_mono_mode_select As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("NHKMODE") & "")
                        If Me._NHK_dual_mono_mode <> 3 Then
                            NHK_dual_mono_mode_select = Me._NHK_dual_mono_mode
                        End If

                        'リクエストされたURL
                        Dim req_Url As String = req.Url.LocalPath
                        '===========================================
                        'WEBインターフェース
                        '===========================================
                        Dim WI_cmd As String = ""
                        Dim WI_cmd_reply As String = "" '返事
                        Dim WI_cmd_reply_force As Integer = 0 '1ならwebページ処理へ向かわない
                        Dim WI_skip_html As Integer = 0
                        If req_Url.IndexOf("/WI_") >= 0 Then
                            WI_cmd = "WI_" & instr_pickup_para(req_Url, "/WI_", ".html", 0)
                            Select Case WI_cmd
                                Case "WI_GET_CHANNELS"
                                    'BonDriver, ServiceID, ch_space, チャンネル名
                                    WI_cmd_reply = Me._procMan.WI_GET_CHANNELS(Me._BonDriverPath, Me._udpApp, Me._BonDriver_NGword)
                                    WI_cmd_reply_force = 1
                                Case "WI_START_STREAM"
                                    '配信スタート
                                    'Sub_stream_Start(num, bondriver, sid, chspace, bon_sid_ch_str, resolution, stream_mode, videoname)
                                    req_Url = "/StartTv.html"
                                    path = path.Replace("WI_START_STREAM.html", "StartTv.html")
                                Case "WI_STOP_STREAM"
                                    '配信ストップ
                                    If num > 0 Then
                                        req_Url = "/CloseTv.html"
                                        path = path.Replace("WI_STOP_STREAM.html", "CloseTv.html")
                                    Else
                                        req_Url = "/StopAll.html"
                                        path = path.Replace("WI_STOP_STREAM.html", "StopAll.html")
                                    End If
                                Case "WI_GET_PROGRAM_D"
                                    '地デジ番組表取得
                                    WI_cmd_reply = Me.WI_GET_PROGRAM_D()
                                    WI_cmd_reply_force = 1
                                Case "WI_GET_PROGRAM_EDCB"
                                    'EDCB番組表取得
                                    WI_cmd_reply = Me.WI_GET_PROGRAM_EDCB()
                                    WI_cmd_reply_force = 1
                                Case "WI_GET_PROGRAM_TVROCK"
                                    'TVROCK番組表取得
                                    WI_cmd_reply = Me.WI_GET_PROGRAM_TVROCK()
                                    WI_cmd_reply_force = 1
                                Case "WI_GET_LIVE_STREAM"
                                    '現在配信中のストリーム
                                    '_listNo.,num, udpPort, BonDriver, ServiceID, ch_space, stream_mode, NHKMODE
                                    'stopping, チャンネル名, hlsApp
                                    WI_cmd_reply = Me._procMan.WI_GET_LIVE_STREAM()
                                    WI_cmd_reply_force = 1
                                Case "WI_GET_TVRV_STATUS"
                                    'サーバー設定
                                    WI_cmd_reply = Me.WI_GET_TVRV_STATUS()
                                    WI_cmd_reply_force = 1
                                Case "WI_GET_TSFILE_COUNT"
                                    '作られている.tsの数
                                    WI_cmd_reply = Me.WI_GET_TSFILE_COUNT(num)
                                    WI_cmd_reply_force = 1
                            End Select
                        End If

                        '===========================================
                        'WEBページ表示前処理
                        '===========================================
                        '特別なページ　配信スタート停止などサーバー動作を実行
                        If req_Url.ToLower.IndexOf("/" & HTTPSTREAM_mode2_str.ToLower & "_ViewTV".ToLower) >= 0 Then
                            'VLC HTTPストリーミング
                            'URLからVLChttpストリーム識別文字列を取り除く
                            Dim s1 As Integer = req_Url.IndexOf(HTTPSTREAM_mode2_str & "_")
                            req_Url = req_Url.Substring(0, s1) & req_Url.Substring(s1 + HTTPSTREAM_mode2_str.Length + 1)
                            request_page = 19
                        ElseIf req_Url.ToLower.IndexOf("/ViewTV".ToLower) >= 0 Then
                            '通常視聴
                            'numが<form>から渡されていなければURLから取得するViewTV2.htmlなら2
                            If num = 0 Then
                                Dim num_url As String = Val(req_Url.ToLower.Substring(req_Url.ToLower.IndexOf("ViewTV".ToLower) + "ViewTV".Length))
                                If num_url > 0 Then
                                    num = num_url
                                End If
                            End If

                            Dim gln As String = Me._procMan.get_live_numbers()
                            gln = gln.Replace("x", "")
                            If gln.IndexOf(" " & num.ToString & " ") >= 0 Then
                                check_m3u8_ts = check_m3u8_ts_status(num)
                                If check_m3u8_ts < Me._tsfile_wait Then
                                    '準備ができていない
                                    request_page = 1 'waiting表示
                                Else
                                    'ViewTV.html用
                                    chk_viewtv_ok = 1
                                End If
                            Else
                                '配信されていない
                                request_page = 11
                            End If
                        ElseIf req_Url.ToLower = ("/StartTv.html").ToLower Then
                            '配信スタート
                            'パラメーターが正しいかチェック
                            If num > 0 And bondriver.Length > 0 And Val(sid) > 0 And Val(chspace) >= 0 Then
                                '正しければ配信スタート
                                Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, videoname, NHK_dual_mono_mode_select, stream_mode, resolution)
                                'すぐさま視聴ページへリダイレクトする
                                redirect = "ViewTV" & num & ".html"
                            ElseIf num > 0 And videoname.Length > 0 Then
                                'ファイル再生
                                If Me._hlsApp.IndexOf("ffmpeg") > 0 Then
                                    'ffmpegなら
                                    Me.start_movie(num, "", 0, 0, "", Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, "", videoname, NHK_dual_mono_mode_select, stream_mode, resolution)
                                Else
                                    '今のところVLCには未対応
                                    request_page = 12
                                End If
                                'すぐさま視聴ページへリダイレクトする
                                redirect = "ViewTV" & num & ".html"
                            Else
                                StartTv_param = -1
                            End If
                        ElseIf req_Url.ToLower = "/CloseTv.html".ToLower Then
                            '配信停止
                            Me.stop_movie(num)
                        ElseIf req_Url.ToLower = "/StopAll.html".ToLower Then
                            'すべてのプロセスと関連アプリを停止する
                            stop_movie(-2)
                        End If

                        '===========================================
                        'WEBページ表示
                        '===========================================
                        If StartTv_param = -1 Then
                            '/StartTvにリクエストがあったがパラメーターが不正な場合
                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                            sw.WriteLine(ERROR_PAGE("パラメーターが不正です", "パラメーターが不正です"))
                            'sw.Flush()
                            log1write("パラメーターが不正です")
                        ElseIf WI_cmd_reply.Length > 0 Or WI_cmd_reply_force = 1 Then
                            'ＷＥＢインターフェース　コマンドの返事を返す
                            sw.WriteLine(WI_cmd_reply)
                        ElseIf request_page = 1 Or request_page = 11 Then
                            'waitingページを表示する
                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                            sw.WriteLine("<!doctype html>")
                            sw.WriteLine("<html>")
                            sw.WriteLine("<head>")
                            sw.WriteLine("<title>Waiting " & num.ToString & "</title>")
                            sw.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />")
                            sw.WriteLine("<meta http-equiv=""refresh"" content=""1 ; URL=ViewTV" & num.ToString & ".html"">")
                            sw.WriteLine("</head>")
                            sw.WriteLine("<body>")
                            If request_page = 1 Then
                                sw.WriteLine("配信準備中です..(" & check_m3u8_ts.ToString & ")")
                                log1write(num.ToString & ":配信準備中です")
                            ElseIf request_page = 11 Then
                                sw.WriteLine("配信されていません")
                                log1write(num.ToString & ":配信されていません")
                            End If
                            sw.WriteLine("<br><br>")
                            sw.WriteLine("<input type=""button"" value=""トップメニュー"" onClick=""location.href='/index.html'"">")
                            sw.WriteLine("<br><br>")
                            sw.WriteLine("<input type=""button"" value=""直前のページへ戻る"" onClick=""history.go(-1);"">")
                            'sw.WriteLine("<input type=""button"" value=""地デジ番組表"" onClick=""location.href='TvProgram.html'"">")
                            sw.WriteLine("</body>")
                            sw.WriteLine("</html>")
                            'sw.Flush()
                        ElseIf request_page = 19 Then
                            Dim html19 As String = ""
                            html19 &= "VLC httpストリーミングで配信中です<br>"
                            html19 &= "ブラウザでの再生はできません"
                            'ここで.tsへのリンクを貼るべきか・・
                            sw.WriteLine(ERROR_PAGE("VLC httpストリーミング", html19))
                        ElseIf request_page = 12 Then
                            'VLCはファイル再生未対応
                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                            sw.WriteLine(ERROR_PAGE("ファイル再生失敗", "VLCでのファイル再生には対応していません"))
                            'sw.Flush()
                            log1write(num.ToString & ":配信されていません")
                        ElseIf path.IndexOf(".htm") > 0 And File.Exists(path) Then
                            'ElseIf request_page >= 2 Then
                            'パラメーターを置換する必要があるページ
                            Dim s As String = ReadAllTexts(path)

                            Debug.Print("[path=" & path & "]")
                            Debug.Print("[s=" & s & "]")

                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))

                            'BonDriverと番組名連携選択
                            If s.IndexOf("%SELECTBONSIDCH") >= 0 Then
                                Dim gt() As String = get_atags("%SELECTBONSIDCH", s)
                                Dim selectbon As String = WEB_make_select_Bondriver_html(gt)
                                If selectbon.Length > 0 Then
                                    s = s.Replace("%SELECTBONSIDCH" & gt(0) & "%", selectbon)
                                Else
                                    s = s.Replace("%SELECTBONSIDCH" & gt(0) & "%", gt(4))
                                End If
                            End If

                            'Viewボタン作成
                            If s.IndexOf("%VIEWBUTTONS") >= 0 Then
                                Dim gt() As String = get_atags("%VIEWBUTTONS", s)
                                Dim viewbutton_html As String = WEB_make_ViewLink_html(gt)
                                If viewbutton_html.Length > 0 Then
                                    s = s.Replace("%VIEWBUTTONS" & gt(0) & "%", viewbutton_html)
                                Else
                                    s = s.Replace("%VIEWBUTTONS" & gt(0) & "%", gt(4))
                                End If
                            End If

                            'ストリーム番号
                            If s.IndexOf("%NUM%") >= 0 Then
                                s = s.Replace("%NUM%", num.ToString)
                            End If

                            'NHK音声モード
                            If s.IndexOf("%SELECTNHKMODE") >= 0 Then
                                Dim gt() As String = get_atags("%SELECTNHKMODE", s)
                                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                    If Me._NHK_dual_mono_mode = 3 Then
                                        Dim viewbutton_html As String = "<span id=""NHKVIEW"">" & WEB_make_NHKMODE_html(gt, num) & "</span>"
                                        s = s.Replace("%SELECTNHKMODE" & gt(0) & "%", viewbutton_html)
                                    Else
                                        s = s.Replace("%SELECTNHKMODE" & gt(0) & "%", "<input type=""hidden"" name=""NHKMODE"" value=""" & Me._NHK_dual_mono_mode & """>")
                                    End If
                                Else
                                    s = s.Replace("%SELECTNHKMODE" & gt(0) & "%", gt(4))
                                End If
                            End If

                            'ViewTV.html用
                            If chk_viewtv_ok = 1 And num > 0 Then
                                '配信中ならば

                                '_listから取得resolutionを取得
                                Dim rez As String = Me._procMan.get_resolution(num)
                                Dim rezx As Integer = 0
                                Dim rezy As Integer = 0
                                Dim rez_hlsopt As Integer = 0
                                If rez.Length = 0 Then
                                    '_listから取得できなければ_hlsOpt2から取得する
                                    rez_hlsopt = 1
                                    Dim ho As String = Me._hlsOpt2
                                    Dim sp1, sp2 As Integer
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        Try
                                            sp1 = ho.IndexOf("-s ")
                                            sp2 = ho.IndexOf(" ", sp1 + 3)
                                            rez = ho.Substring(sp1 + "-s ".Length, sp2 - sp1 - "-s ".Length)
                                        Catch ex As Exception
                                            rez = ""
                                        End Try
                                    ElseIf Me._hlsApp.IndexOf("vlc") >= 0 Then
                                        Try
                                            sp1 = ho.IndexOf("width=")
                                            sp2 = ho.IndexOf(",", sp1)
                                            rezx = ho.Substring(sp1 + "width=".Length, sp2 - sp1 - "width=".Length)
                                            sp1 = ho.IndexOf("height=")
                                            sp2 = ho.IndexOf(",", sp1)
                                            rezy = ho.Substring(sp1 + "height=".Length, sp2 - sp1 - "height=".Length)
                                            If rezx > 0 And rezy > 0 Then
                                                rez = Trim(rezx) & "x" & Trim(rezy)
                                            Else
                                                rez = ""
                                            End If
                                        Catch ex As Exception
                                            rez = ""
                                        End Try
                                    End If
                                End If
                                Dim d() As String = rez.Split("x")
                                If d.Length = 2 Then
                                    If Val(d(0)) > 0 And Val(d(1)) > 0 Then
                                        rezx = Val(d(0))
                                        rezy = Val(d(1))
                                    Else
                                        rez = ""
                                    End If
                                Else
                                    rez = ""
                                End If
                                If rezx > 0 And rezy > 0 Then
                                    s = s.Replace("%WIDTH%", rezx.ToString)
                                    s = s.Replace("%HEIGHT%", rezy.ToString)
                                Else
                                    '値がなければデフォルトをセット
                                    s = s.Replace("%WIDTH%", "640")
                                    s = s.Replace("%HEIGHT%", "360")
                                End If

                                '配信中ならばnumからBonDriver_pathとBonDriverを取得
                                If s.IndexOf("%SELECTCH") >= 0 Then
                                    '%SELECTCHをhtmlに置換
                                    Dim gt() As String = get_atags("%SELECTCH", s)
                                    Dim vhtml As String
                                    If rez_hlsopt = 0 Then
                                        vhtml = replace_html_selectch(num, rez, gt, Me._NHK_dual_mono_mode)
                                    Else
                                        'hlsOptから解像度を取得した場合は値を渡さない（HLS_option.txtのオプションが使われてしまうため）
                                        vhtml = replace_html_selectch(num, "", gt, Me._NHK_dual_mono_mode)
                                    End If
                                    If vhtml.Length > 0 Then
                                        s = s.Replace("%SELECTCH" & gt(0) & "%", vhtml)
                                    Else
                                        s = s.Replace("%SELECTCH" & gt(0) & "%", gt(4))
                                    End If
                                End If

                            End If

                            '%FILEROOT%変換
                            If s.IndexOf("%FILEROOT%") >= 0 Then
                                Dim fileroot As String = get_soutaiaddress_from_fileroot()
                                s = s.Replace("%FILEROOT%", fileroot)
                            End If

                            'ファイル選択ページ用
                            If s.IndexOf("%SELECTVIDEO") >= 0 Then
                                Dim shtml As String = make_file_select_html()
                                s = s.Replace("%SELECTVIDEO%", shtml)
                            End If

                            '配信中簡易リスト
                            If s.IndexOf("%PROCBONLIST") >= 0 Then
                                Dim gt() As String = get_atags("%PROCBONLIST", s)
                                Dim js As String = Me._procMan.get_live_numbers_bon().Replace(vbCrLf, "<br>")
                                If js.Length > 0 Then
                                    js = gt(1) & js & gt(3)
                                End If
                                If js.Length > 0 Then
                                    s = s.Replace("%PROCBONLIST" & gt(0) & "%", js)
                                Else
                                    s = s.Replace("%PROCBONLIST" & gt(0) & "%", gt(4))
                                End If
                            End If

                            'リダイレクト
                            If s.IndexOf("%REDIRECT%") >= 0 Then
                                If redirect.Length > 3 Then
                                    'リダイレクト指定があれば
                                    s = s.Replace("%REDIRECT%", "<meta http-equiv=""refresh"" content=""0 ; URL=" & redirect & """>")
                                Else
                                    '無ければリダイレクト変数を消す
                                    s = s.Replace("%REDIRECT%", "")
                                End If
                            End If

                            '地デジ番組表（通常のネットから取得）
                            If s.IndexOf("%TVPROGRAM-D%") >= 0 Then
                                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                    s = s.Replace("%TVPROGRAM-D%", make_TVprogram_html_now(0, Me._NHK_dual_mono_mode))
                                Else
                                    s = s.Replace("%TVPROGRAM-D%", make_TVprogram_html_now(0, -1))
                                End If
                            End If

                            'TvRock番組表
                            If s.IndexOf("%TVPROGRAM-TVROCK%") >= 0 Then
                                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                    s = s.Replace("%TVPROGRAM-TVROCK%", make_TVprogram_html_now(999, Me._NHK_dual_mono_mode))
                                Else
                                    s = s.Replace("%TVPROGRAM-TVROCK%", make_TVprogram_html_now(999, -1))
                                End If
                            End If
                            'TvRock番組表ボタン
                            If s.IndexOf("%TVPROGRAM-TVROCK-BUTTON") >= 0 Then
                                Dim gt() As String = get_atags("%TVPROGRAM-TVROCK-BUTTON", s)
                                If TvProgram_tvrock_url.Length > 0 Then
                                    s = s.Replace("%TVPROGRAM-TVROCK-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" value=""TvRock番組表"" onClick=""location.href='TvProgram_TvRock.html'"">") & gt(3)
                                Else
                                    s = s.Replace("%TVPROGRAM-TVROCK-BUTTON" & gt(0) & "%", gt(4))
                                End If
                            End If

                            'EDCB番組表
                            If s.IndexOf("%TVPROGRAM-EDCB%") >= 0 Then
                                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                    s = s.Replace("%TVPROGRAM-EDCB%", make_TVprogram_html_now(998, Me._NHK_dual_mono_mode))
                                Else
                                    s = s.Replace("%TVPROGRAM-EDCB%", make_TVprogram_html_now(998, -1))
                                End If
                            End If
                            'EDCB番組表ボタン
                            If s.IndexOf("%TVPROGRAM-EDCB-BUTTON") >= 0 Then
                                Dim gt() As String = get_atags("%TVPROGRAM-EDCB-BUTTON", s)
                                If TvProgram_EDCB_url.Length > 0 Then
                                    s = s.Replace("%TVPROGRAM-EDCB-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" value=""EDCB番組表"" onClick=""location.href='TvProgram_EDCB.html'"">") & gt(3)
                                Else
                                    s = s.Replace("%TVPROGRAM-EDCB-BUTTON" & gt(0) & "%", gt(4))
                                End If
                            End If

                            sw.WriteLine(s)
                            'sw.Flush()

                            log1write(path & "へのアクセスを受け付けました")

                        Else
                            'ローカルファイルが存在していない
                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                            sw.WriteLine(ERROR_PAGE("bad request", "ページが見つかりません"))
                            'sw.Flush()
                            log1write(path & "が見つかりませんでした")
                        End If

                        sw.Flush()
                    Else
                        'HTML以外なら
                        If File.Exists(path) Then
                            ' ローカルファイルが存在すればレスポンス・ストリームに書き出す
                            'm3u8、tsへの要求はこちらへ来る
                            Dim content As Byte() = ReadAllBytes(path)
                            res.OutputStream.Write(content, 0, content.Length)
                            log1write(path & "へのアクセスを受け付けました")
                        Else
                            'ローカルファイルが存在していない
                            Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                            sw.WriteLine(ERROR_PAGE("bad request", "ページが見つかりません"))
                            sw.Flush()
                            log1write(path & "が見つかりませんでした")
                        End If
                    End If
                Else
                    '認証エラー
                    context.Response.StatusCode = 401
                End If

                res.Close()

                Try
                    context.Response.Close()
                Catch ex As Exception
                    ' client closed connection before the content was sent
                End Try

            Catch httpEx As HttpListenerException
                log1write(httpEx.Message)
                log1write(httpEx.StackTrace)
            End Try
        End While
    End Sub

    '===================================
    'WEBインターフェース
    '===================================
    'TvRemoteViewer_VB status
    Public Function WI_GET_TVRV_STATUS() As String
        'NHKSELECTMODE,フォーム上の解像度、等
        Dim r As String = ""
        Dim i As Integer = 0

        r &= "【全般】" & vbCrLf
        r &= "_tsfile_wait=" & Me._tsfile_wait & vbCrLf
        r &= "Stop_RecTask_at_StartEnd=" & Stop_RecTask_at_StartEnd & vbCrLf
        r &= vbCrLf
        r &= "【UDPアプリ】" & vbCrLf
        r &= "_udpApp=" & Me._udpApp & vbCrLf
        r &= "_udpPort=" & Me._udpPort & vbCrLf
        r &= "_udpOpt=" & Me._udpOpt3 & vbCrLf
        r &= vbCrLf
        r &= "【BonDriver】" & vbCrLf
        r &= "_BonDriverPath=" & Me._udpApp & vbCrLf
        r &= vbCrLf
        r &= "【HLSアプリ】" & vbCrLf
        r &= "_hlsApp=" & Me._hlsApp & vbCrLf
        r &= "_hlsroot=" & Me._hlsroot & vbCrLf
        r &= "_hlsOpt=" & Me._hlsOpt2 & vbCrLf
        r &= "_NHK_dual_mono_mode=" & Me._NHK_dual_mono_mode & vbCrLf
        r &= "BS1_hlsApp=" & BS1_hlsApp & vbCrLf
        r &= vbCrLf
        r &= "【HTTPサーバー】" & vbCrLf
        r &= "_wwwroot=" & Me._wwwroot & vbCrLf
        r &= "_wwwport=" & Me._wwwport & vbCrLf
        r &= "_fileroot=" & Me._fileroot & vbCrLf
        r &= "_id=" & Me._id & vbCrLf
        If Me._pass.Length > 0 Then
            r &= "_pass=" & "********" & vbCrLf
        Else
            r &= "_pass=" & "" & vbCrLf
        End If
        r &= "_MIME_TYPE_DEFAULT=" & Me._MIME_TYPE_DEFAULT & vbCrLf
        If Me._MIME_TYPE IsNot Nothing Then
            For i = 0 To Me._MIME_TYPE.Length - 1
                r &= "_MIME_TYPE=" & Me._MIME_TYPE(i) & vbCrLf
            Next
        End If
        r &= vbCrLf
        r &= "【ファイル再生】" & vbCrLf
        If Me._videopath IsNot Nothing Then
            For i = 0 To Me._videopath.Length - 1
                r &= "_videopath=" & Me._videopath(i) & vbCrLf
            Next
        End If
        r &= "_AddSubFolder=" & Me._AddSubFolder & vbCrLf
        r &= vbCrLf
        r &= "【HTTPストリーム再生】" & vbCrLf
        'HTTPストリーム再生にどのhlsアプリを使用するか 0=フォーム 1=vlc 2=ffmpeg
        r &= "HTTPSTREAM_App=" & HTTPSTREAM_App & vbCrLf
        'VLCポート　'とりあえずは個別設定無し
        r &= "HTTPSTREAM_VLC_port=" & HTTPSTREAM_VLC_port & vbCrLf
        'VLCポート＝UDPポートナンバーにいくつプラスするか
        'r &= "HTTPSTREAM_VLC_port_plus=" & HTTPSTREAM_VLC_port_plus & vbCrLf

        Return r
    End Function

    'filerootに作成された.tsの数
    Public Function WI_GET_TSFILE_COUNT(ByVal num As Integer) As Integer
        Dim r As Integer = 0

        Dim fileroot As String = Me._fileroot
        If fileroot.Length = 0 Then
            fileroot = Me._wwwroot
        End If
        fileroot &= "\"

        ' 必要な変数を宣言する
        Dim stPrompt As String = String.Empty
        Dim s As String

        'tsチェック
        Dim ts_count As Integer = 0
        For Each stFilePath As String In System.IO.Directory.GetFiles(fileroot, "mystream" & num.ToString & "*.ts")
            s = stFilePath & System.Environment.NewLine
            ts_count += 1
        Next

        r = ts_count

        Return r
    End Function

    '地デジ番組表取得
    Public Function WI_GET_PROGRAM_D() As String
        Dim r As String = ""

        Return r
    End Function

    'EDCB番組表取得
    Public Function WI_GET_PROGRAM_EDCB(Optional ByVal t1 As Integer = 0, Optional ByVal t2 As Integer = 2147400000) As String
        Dim r As String = ""

        Return r
    End Function

    'TVROCK番組表取得
    Public Function WI_GET_PROGRAM_TVROCK() As String
        Dim r As String = ""

        Return r
    End Function

    '■テスト
    Public Function WI_GET_LIVE_STREAM() As String
        Return Me._procMan.WI_GET_LIVE_STREAM()
    End Function
    Public Function WI_GET_CHANNELS() As String
        Return Me._procMan.WI_GET_CHANNELS(Me._BonDriverPath, Me._udpApp, Me._BonDriver_NGword)
    End Function

End Class


