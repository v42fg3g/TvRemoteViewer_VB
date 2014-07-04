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

    '変更されたら再起動が必要なパラメーター
    Private _udpPort As Integer = Nothing
    Private _wwwroot As String = Nothing
    Private _fileroot As String = Nothing
    Private _wwwport As Integer = Nothing
    Private _BonDriver_NGword As String() = Nothing

    '空きUDPポートが取得できないので暫定
    Private _updCount As Integer

    'VLCのオプション用
    Public vlc_option() As VLCoptionstructure
    Public Structure VLCoptionstructure
        Public resolution As String '解像度　"640x360"
        Public opt As String 'VLCオプション文字列
    End Structure

    Public Sub New(udpApp As String, udpPort As Integer, udpOpt3 As String, chSpace As Integer, hlsApp As String, hlsOpt1 As String, hlsOpt2 As String, wwwroot As String, fileroot As String, wwwport As Integer, BonDriverPath As String, ShowConsole As Boolean, BonDriver_NGword As String())
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

        'VLCオプションを読み込む
        Me.read_vlc_option()

        'ストリーム用インスタンス作成
        Me._procMan = New ProcessManager(udpPort, wwwroot, fileroot)
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
        ' プレフィックスの登録
        Me._listener.Start()

        While Me._isWebStart
            Try
                Dim context As HttpListenerContext = Me._listener.GetContext()
                Dim req As HttpListenerRequest = context.Request
                Dim res As HttpListenerResponse = context.Response

                ' リクエストされたURLからファイルのパスを求める
                Dim path As String = root & req.Url.LocalPath.Replace("/", "\")

                'ルートにアクセスされた場合、index.htmlを表示する
                If path = Me._wwwroot & "\" Then
                    path = path & "index.html"
                End If

                log1write(req.Url.LocalPath & "へのリクエストがありました")

                'ToLower小文字で比較
                Dim StartTv_param As Integer = 0 'StartTvパラメーターが正常かどうか
                Dim request_page As Integer = 0 '特別なリクエストかどうか

                '★リクエストパラメーターを取得
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

                Dim chk_viewtv_ok As Integer = 0

                'WEBページ表示前処理
                '特別なページ　配信スタート停止などサーバー動作を実行
                If req.Url.LocalPath.ToLower.IndexOf("/ViewTV".ToLower) >= 0 Then
                    '通常視聴
                    request_page = 2
                    'numが<form>から渡されていなければURLから取得するViewTV2.htmlなら2
                    If num = 0 Then
                        Dim num_url As String = Val(req.Url.LocalPath.ToLower.Substring(req.Url.LocalPath.ToLower.IndexOf("ViewTV".ToLower) + "ViewTV".Length))
                        If num_url > 0 Then
                            num = num_url
                        End If
                    End If

                    Dim gln As String = Me._procMan.get_live_numbers()
                    If gln.IndexOf(" " & num.ToString & " ") >= 0 Then
                        check_m3u8_ts = check_m3u8_ts_status(num)
                        If check_m3u8_ts <= 2 Then
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
                ElseIf req.Url.LocalPath.ToLower = ("/StartTv.html").ToLower Then
                    '配信スタート
                    request_page = 3
                    'パラメーターが正しいかチェック
                    If num > 0 And bondriver.Length > 0 And Val(sid) > 0 And Val(chspace) >= 0 Then
                        '正しければ配信スタート
                        Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpopt3, resolution)
                    Else
                        StartTv_param = -1
                    End If
                ElseIf req.Url.LocalPath.ToLower = "/CloseTv.html".ToLower Then
                    '配信停止
                    request_page = 4
                    Me.stop_movie(num)
                ElseIf req.Url.LocalPath.ToLower = "/StopAll.html".ToLower Then
                    'すべてのプロセスと関連アプリを停止する
                    request_page = 5
                    stop_movie(-2)
                End If

                'WEBページ表示
                If StartTv_param = -1 Then
                    '/StartTvにリクエストがあったがパラメーターが不正な場合
                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                    sw.WriteLine("<!doctype html>")
                    sw.WriteLine("<html>")
                    sw.WriteLine("<head>")
                    sw.WriteLine("<title>WEBパラメーターが不正です</title>")
                    sw.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />")
                    sw.WriteLine("</head>")
                    sw.WriteLine("<body>")
                    sw.WriteLine("WEBパラメーターが不正です")
                    sw.WriteLine("<br><br>")
                    'sw.WriteLine("<FORM><INPUT type=""button"" Value=""戻る"" onClick=""history.go(-1);""></CENTER></P></FORM>")
                    sw.WriteLine("<input type=""button"" value=""トップメニューへ"" onClick=""location.href='index.html'"">")
                    sw.WriteLine("</body>")
                    sw.WriteLine("</html>")
                    sw.Flush()
                    log1write("WEBパラメーターが不正です")
                ElseIf request_page = 1 Then
                    'waitingページを表示する
                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                    sw.WriteLine("<!doctype html>")
                    sw.WriteLine("<html>")
                    sw.WriteLine("<head>")
                    sw.WriteLine("<title>Waiting " & num.ToString & "</title>")
                    sw.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />")
                    sw.WriteLine("<meta http-equiv=""refresh"" content=""1 ; URL=ViewTV" & num.ToString & ".html"">")
                    sw.WriteLine("</head>")
                    sw.WriteLine("<body>")
                    sw.WriteLine("配信準備中です..(" & check_m3u8_ts.ToString & ")")
                    sw.WriteLine("<br><br>")
                    'sw.WriteLine("<FORM><INPUT type=""button"" Value=""戻る"" onClick=""history.go(-1);""></CENTER></P></FORM>")
                    sw.WriteLine("<input type=""button"" value=""トップメニューへ"" onClick=""location.href='index.html'"">")
                    sw.WriteLine("</body>")
                    sw.WriteLine("</html>")
                    sw.Flush()
                    log1write(num.ToString & ":配信準備できていません")
                ElseIf request_page = 11 Then
                    '配信されていない
                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                    sw.WriteLine("<!doctype html>")
                    sw.WriteLine("<html>")
                    sw.WriteLine("<head>")
                    sw.WriteLine("<title>配信されていません：" & num.ToString & "</title>")
                    sw.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />")
                    sw.WriteLine("</head>")
                    sw.WriteLine("<body>")
                    sw.WriteLine("配信されていません")
                    sw.WriteLine("<br><br>")
                    'sw.WriteLine("<FORM><INPUT type=""button"" Value=""戻る"" onClick=""history.go(-1);""></CENTER></P></FORM>")
                    sw.WriteLine("<input type=""button"" value=""トップメニューへ"" onClick=""location.href='index.html'"">")
                    sw.WriteLine("</body>")
                    sw.WriteLine("</html>")
                    sw.Flush()
                    log1write(num.ToString & ":配信されていません")
                ElseIf path.IndexOf(".htm") > 0 And File.Exists(path) Then
                    'ElseIf request_page >= 2 Then
                    'パラメーターを置換する必要があるページ
                    Dim s As String = ReadAllTexts(path)

                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))

                    'BonDriverと番組名連携選択
                    If s.IndexOf("%SELECTBONSIDCH") >= 0 Then
                        Dim sb1 As Integer = s.IndexOf("%SELECTBONSIDCH:")
                        Dim sb2 As Integer = s.IndexOf("%", sb1 + 1)
                        Dim sbs As String = ""
                        If sb1 >= 0 And sb2 > sb1 Then
                            '間に入れるhtmlを取得
                            sbs = s.Substring(sb1 + "%SELECTBONSIDCH:".Length, sb2 - sb1 - "%SELECTBONSIDCH:".Length)
                        End If
                        Dim selectbon As String = WEB_make_select_Bondriver_html(sbs)
                        s = s.Replace("%SELECTBONSIDCH%", selectbon)
                        s = s.Replace("%SELECTBONSIDCH:" & sbs & "%", selectbon)
                    End If

                    'Viewボタン作成
                    If s.IndexOf("%VIEWBUTTONS") >= 0 Then
                        Dim vb1 As Integer = s.IndexOf("%VIEWBUTTONS:")
                        Dim vb2 As Integer = s.IndexOf("%", vb1 + 1)
                        Dim vbs As String = ""
                        If vb1 >= 0 And vb2 > vb1 Then
                            '間に入れるhtmlを取得
                            vbs = s.Substring(vb1 + "%VIEWBUTTONS:".Length, vb2 - vb1 - "%VIEWBUTTONS:".Length)
                        End If
                        Dim viewbutton_html As String = WEB_make_ViewLink_html(vbs)
                        s = s.Replace("%VIEWBUTTONS%", viewbutton_html)
                        s = s.Replace("%VIEWBUTTONS:" & vbs & "%", viewbutton_html)
                    End If

                    'ストリーム番号
                    If s.IndexOf("%NUM%") >= 0 Then
                        s = s.Replace("%NUM%", num.ToString)
                    End If

                    'ViewTV.html用
                    If chk_viewtv_ok = 1 And num > 0 Then
                        '配信中ならば

                        '_listから取得resolutionを取得
                        Dim rez As String = Me._procMan.get_resolution(num)
                        Dim rezx As Integer = 0
                        Dim rezy As Integer = 0
                        If rez.Length = 0 Then
                            '_listから取得できなければ_hlsOpt2から取得する
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
                            Dim bon As String = Me._procMan.get_bondriver_name(num)

                            Dim bonp As String = Me._BonDriverPath
                            If bonp.Length = 0 Then
                                '指定が無い場合はUDPAPPと同じフォルダにあると見なす
                                bonp = filepath2path(Me._udpApp.ToString)
                            End If
                            Dim vhtml As String = WEB_search_ServiceID(bonp, bon, 1)
                            If vhtml.Length > 0 Then
                                vhtml = "<option value="""">---</option>" & vbCrLf & vhtml
                                vhtml = "<select name=""Bon_Sid_Ch"">" & vbCrLf & vhtml
                                vhtml = "<form action=""StartTV.html"">" & vbCrLf & vhtml
                                vhtml &= "</select>" & vbCrLf
                                vhtml &= "<input type=""submit"" value=""視聴"" />" & vbCrLf
                                vhtml &= "<input type=""hidden"" name=""num"" value=""" & num & """>" & vbCrLf
                                vhtml &= "<input type=""hidden"" name=""redirect"" value=""ViewTV" & num & ".html"">" & vbCrLf
                                If rez.Length > 0 Then
                                    vhtml &= "<input type=""hidden"" name=""resolution"" value=""" & rez & """>" & vbCrLf
                                End If
                                vhtml &= "</form>" & vbCrLf
                                s = s.Replace("%SELECTCH%", vhtml)
                            End If
                        End If
                    End If

                    '配信中簡易リスト
                    If s.IndexOf("%PROCBONLIST%") >= 0 Then
                        Dim js As String = Me._procMan.get_live_numbers_bon().Replace(vbCrLf, "<br>")
                        s = s.Replace("%PROCBONLIST%", js)
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

                    sw.WriteLine(s)
                    sw.Flush()

                    log1write(path & "へのアクセスを受け付けました")

                ElseIf File.Exists(path) Then
                    ' ローカルファイルが存在すればレスポンス・ストリームに書き出す
                    'm3u8、tsへの要求はこちらへ来る
                    Dim content As Byte() = ReadAllBytes(path)
                    res.OutputStream.Write(content, 0, content.Length)
                    log1write(path & "へのアクセスを受け付けました")
                Else
                    'ローカルファイルが存在していない
                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                    sw.WriteLine("<!doctype html>")
                    sw.WriteLine("<html>")
                    sw.WriteLine("<head>")
                    sw.WriteLine("<title>bad request</title>")
                    sw.WriteLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"" />")
                    sw.WriteLine("</head>")
                    sw.WriteLine("<body>")
                    sw.WriteLine("ページが見つかりません")
                    sw.WriteLine("<br><br>")
                    'sw.WriteLine("<FORM><INPUT type=""button"" Value=""戻る"" onClick=""history.go(-1);""></CENTER></P></FORM>")
                    sw.WriteLine("<input type=""button"" value=""トップメニューへ"" onClick=""location.href='index.html'"">")
                    sw.WriteLine("</body>")
                    sw.WriteLine("</html>")
                    sw.Flush()
                    log1write(path & "が見つかりませんでした")
                End If
                res.Close()
            Catch httpEx As HttpListenerException
                log1write(httpEx.Message)
                log1write(httpEx.StackTrace)
            End Try
        End While

    End Sub

    'HTML内置換用　配信しているストリームのボタンを作成
    Public Function WEB_make_ViewLink_html(Optional ByVal bs As String = "") As String
        Dim html As String = ""
        Dim bst As String = ""
        Dim bst2 As String = ""

        Dim bs2() As String = bs.Split(":")
        If bs2.Length >= 2 Then
            bs = bs2(0)
            bst2 = bs2(1) '2番目のhtmlを最後に表示
        End If

        Dim gln As String = Trim(Me._procMan.get_live_numbers())
        If gln.Length > 0 Then
            Dim d() As String = gln.Split(" ")
            For i As Integer = 0 To d.Length - 1
                If d(i) > 0 Then
                    html &= bst
                    html &= "<input type=""button"" value=""ストリーム" & d(i).ToString & "を視聴"" onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                    'html &= "<form action=""ViewTV" & d(i).ToString & ".html"">" & vbCrLf
                    'html &= "    <input type=""submit"" value=""ストリーム" & d(i).ToString & "を視聴"" />" & vbCrLf
                    'html &= "</form>" & vbCrLf
                    bst = bs
                End If
            Next
        End If

        If html.Length > 0 Then
            html &= bst2 & vbCrLf
        End If

        Return html
    End Function

    'HTML内置換用　BonDriverセレクトボックスを作成
    Public Function WEB_make_select_Bondriver_html(Optional ByVal bs As String = "") As String
        's=BonDriverセレクトと番組セレクトの間に入れるhtmlタグ <br>とか
        Dim html As String = ""
        Dim bons() As String = Nothing
        Dim bons_n As Integer = 0

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
                html &= "<script type=""text/javascript"" src=""ConnectedSelect.js""></script>" & vbCrLf
                html &= "<select id=""SEL1"" name=""BonDriver"">" & vbCrLf
                html &= "<option value="""">---</option>" & vbCrLf
                For i = 0 To bons.Length - 1
                    html &= "<option value=""" & bons(i) & """>" & bons(i) & "</option>" & vbCrLf
                Next
                html &= "</select>" & vbCrLf
                html &= bs & vbCrLf
                '各BonDriverに対応したチャンネルを書き込む
                html &= "<select id=""SEL2"" name=""Bon_Sid_Ch"">" & vbCrLf
                html &= "<option value="""">---</option>" & vbCrLf
                For i = 0 To bons.Length - 1
                    html &= "<optgroup label=""" & bons(i) & """>" & vbCrLf
                    '局名を書き込む
                    html &= WEB_search_ServiceID(bondriver_path, bons(i), 0)
                    html &= "</optgroup>" & vbCrLf
                Next
                html &= "</select>" & vbCrLf
                html &= "<script type=""text/javascript"">" & vbCrLf
                html &= "ConnectedSelect(['SEL1','SEL2']);" & vbCrLf
                html &= "</script>" & vbCrLf
            End If
        End If

        Return html
    End Function

    'HTML内置換用　番組選局セレクトボックスを作成（WEB_make_select_Bondriver_html補助）
    Private Function WEB_search_ServiceID(ByVal bondriver_path As String, ByVal bondriver As String, ByVal BonDriverWrite As Integer) As String
        Dim html As String = ""
        If bondriver.Length > 0 Then

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
                            If BonDriverWrite = 1 Then
                                html &= "<option value=""" & bondriver & " ," & s(5) & "," & s(1) & """>" & s(0) & "</option>" & vbCrLf
                            Else
                                'Bondriverはすでに設定済みなので字数節約のため空白
                                html &= "<option value="" ," & s(5) & "," & s(1) & """>" & s(0) & "</option>" & vbCrLf
                            End If
                        End If
                    End If
                Next
            End If
        End If
        Return html
    End Function

    Private Function check_m3u8_ts_status(ByVal num As Integer) As Integer
        Dim r As Integer = 0 '1=m3u8無,ts無、2=m3u8有、123ts無、3=m3u8有、～ts有
        'm3u8が存在していればViewTV1_waiting.htmlのrefresh先を書き換える
        Dim wwwroot As String = Me._wwwroot & "\"

        ' 必要な変数を宣言する
        Dim stPrompt As String = String.Empty
        Dim s As String

        If file_exist(wwwroot & "mystream" & num.ToString & ".m3u8") <= 0 Then
            'm3u8無し
            r = 0
        Else
            'm3u8有り
            'tsチェック
            Dim ts_count As Integer = 0
            For Each stFilePath As String In System.IO.Directory.GetFiles(wwwroot, "mystream" & num.ToString & "-*.ts")
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

    '映像配信開始
    Public Sub start_movie(ByVal num As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal ChSpace As Integer, ByVal udpApp As String, ByVal hlsApp As String, hlsOpt1 As String, ByVal hlsOpt2 As String, ByVal wwwroot As String, ByVal fileroot As String, ByVal hlsroot As String, ByVal ShowConsole As Boolean, ByVal udpOpt3 As String, Optional ByVal resolution As String = "")
        'resolutionの指定が無ければフォーム上のHLSオプションを使用する

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

        log1write("UDP option=" & udpOpt)

        '★HLSオプションの生成
        Dim hlsOpt As String = ""
        If resolution.Length > 0 Then
            '解像度指定があれば
            Dim chk As Integer = 0
            If vlc_option IsNot Nothing Then
                For i As Integer = 0 To vlc_option.Length - 1
                    If vlc_option(i).resolution = resolution Then
                        hlsOpt = vlc_option(i).opt
                        chk = 1
                        log1write("解像度指定がありました。" & resolution)
                    End If
                Next
            End If
            If chk = 0 Then
                '該当がなければフォーム上のVLCオプション文字列を使用する
                hlsOpt = hlsOpt2
                resolution = ""
            End If
        Else
            '解像度指定がなければフォーム上のVLCオプション文字列を使用する
            hlsOpt = hlsOpt2
        End If

        '"%HLSROOT/../%"用
        Dim hlsroot2 As String = hlsroot
        Dim sp As Integer = hlsroot2.LastIndexOf("\")
        If sp > 0 Then
            hlsroot2 = hlsroot2.Substring(0, sp)
        End If
        '文字列内変数を実際の値に変換
        hlsOpt = hlsOpt.Replace("%UDPPORT%", udpPortNumber.ToString)
        hlsOpt = hlsOpt.Replace("mystream", "mystream" & num)
        hlsOpt = hlsOpt.Replace("%WWWROOT%", wwwroot)
        hlsOpt = hlsOpt.Replace("%FILEROOT%", fileroot)
        hlsOpt = hlsOpt.Replace("%HLSROOT%", hlsroot)
        hlsOpt = hlsOpt.Replace("%HLSROOT/../%", hlsroot2)
        hlsOpt = hlsOpt.Replace("%rc-host%", "127.0.0.1:" & udpPortNumber.ToString)

        log1write("HLS option=" & hlsOpt)

        Directory.SetCurrentDirectory(fileroot) 'カレントディレクトリ変更
        '★プロセスを起動
        Me._procMan.startProc(udpApp, udpOpt, hlsApp, hlsOpt, num, udpPortNumber, ShowConsole, resolution)
    End Sub

    'HLS_option.txtから解像度とHLSオプションを読み込む
    Public Sub read_vlc_option()
        Dim i As Integer = 0

        Try
            Dim line() As String = file2line("HLS_option.txt")
            If line Is Nothing Then
            Else
                For i = 0 To line.Length - 1
                    Dim youso() As String = line(i).Split("]")
                    If youso Is Nothing Then
                    Else
                        If youso.Length = 2 Then
                            ReDim Preserve vlc_option(i)
                            vlc_option(i).resolution = Trim(youso(0)).Replace("[", "")
                            vlc_option(i).opt = youso(1)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            MsgBox("HLS_option.txtの読み込みに失敗しました。")
            '終了
            Form1.Close()
        End Try
    End Sub

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
        Me._procMan.stopProc(-1)
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
End Class


