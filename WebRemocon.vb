﻿Imports System.Collections.Generic
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

    '空きUDPポートが取得できないので暫定
    Private _updCount As Integer

    'VLCのオプション用
    Public vlc_option() As VLCoptionstructure
    Public Structure VLCoptionstructure
        Public resolution As String '解像度　"640x360"
        Public opt As String 'VLCオプション文字列
    End Structure

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

                Dim auth_ok As Integer = 0
                If Me._id.Length = 0 Or Me._pass.Length = 0 Then
                    'パスワード未設定は素通り
                    auth_ok = 1
                ElseIf req.IsAuthenticated Then
                    'ヘッダから認証情報を解析し、Base64デコード
                    Dim auth As String = Encoding.GetEncoding("Shift_JIS").GetString(Convert.FromBase64String(req.Headers("Authorization").Substring(6)))
                    'IDとパスワードに分離する
                    Dim p As String() = auth.Split(":")
                    '判定
                    If Me._id = p(0) And Me._pass = p(1) Then
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

                    'ストリームモード 0=UDP 1=ファイル再生
                    Dim stream_mode As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("StreamMode") & "")

                    'ファイル名
                    Dim videoname As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoName") & ""
                    'URLエンコードしておいたフルパスを文字列に変換
                    videoname = System.Web.HttpUtility.UrlDecode(videoname)

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
                            Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, videoname, resolution)
                            'すぐさま視聴ページへリダイレクトする
                            redirect = "ViewTV" & num & ".html"
                        ElseIf num > 0 And videoname.Length > 0 Then
                            'ファイル再生
                            If Me._hlsApp.IndexOf("ffmpeg") > 0 Then
                                'ffmpegなら
                                Me.start_movie(num, "", 0, 0, "", Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, "", videoname, resolution)
                            Else
                                '今のところVLCには未対応
                                request_page = 12
                            End If
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
                    ElseIf req.Url.LocalPath.ToLower = "/SelectVideo.html".ToLower Then
                        'ファイル選択ページ
                        request_page = 6
                    End If

                    'WEBページ表示
                    If StartTv_param = -1 Then
                        '/StartTvにリクエストがあったがパラメーターが不正な場合
                        Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                        sw.WriteLine(ERROR_PAGE("パラメーターが不正です", "パラメーターが不正です"))
                        sw.Flush()
                        log1write("パラメーターが不正です")
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
                        sw.WriteLine(ERROR_PAGE("配信されていません：" & num.ToString, "配信されていません"))
                        sw.Flush()
                        log1write(num.ToString & ":配信されていません")
                    ElseIf request_page = 12 Then
                        'VLCはファイル再生未対応
                        Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))
                        sw.WriteLine(ERROR_PAGE("ファイル再生失敗", "VLCでのファイル再生には対応していません"))
                        sw.Flush()
                        log1write(num.ToString & ":配信されていません")
                    ElseIf path.IndexOf(".htm") > 0 And File.Exists(path) Then
                        'ElseIf request_page >= 2 Then
                        'パラメーターを置換する必要があるページ
                        Dim s As String = ReadAllTexts(path)

                        Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding("shift_jis"))

                        'BonDriverと番組名連携選択
                        If s.IndexOf("%SELECTBONSIDCH") >= 0 Then
                            Dim gt() As String = get_atags("%SELECTBONSIDCH:", s)
                            Dim selectbon As String = WEB_make_select_Bondriver_html(gt)
                            s = s.Replace("%SELECTBONSIDCH%", selectbon)
                            s = s.Replace("%SELECTBONSIDCH:" & gt(0) & "%", selectbon)
                        End If

                        'Viewボタン作成
                        If s.IndexOf("%VIEWBUTTONS") >= 0 Then
                            Dim gt() As String = get_atags("%VIEWBUTTONS:", s)
                            Dim viewbutton_html As String = WEB_make_ViewLink_html(gt)
                            s = s.Replace("%VIEWBUTTONS%", viewbutton_html)
                            s = s.Replace("%VIEWBUTTONS:" & gt(0) & "%", viewbutton_html)
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
                                Dim gt() As String = get_atags("%SELECTCH:", s)
                                Dim vhtml As String
                                If rez_hlsopt = 0 Then
                                    vhtml = replace_html_selectch(num, rez, gt)
                                Else
                                    'hlsOptから解像度を取得した場合は値を渡さない（HLS_option.txtのオプションが使われてしまうため）
                                    vhtml = replace_html_selectch(num, "", gt)
                                End If
                                s = s.Replace("%SELECTCH%", vhtml)
                                s = s.Replace("%SELECTCH:" & gt(0) & "%", vhtml)
                            End If

                        End If

                        '%FILEROOT%変換
                        If s.IndexOf("%FILEROOT%") >= 0 Then
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
                            s = s.Replace("%FILEROOT%", fileroot)
                        End If

                        'ファイル選択ページ用
                        If request_page = 6 Then
                            Dim shtml As String = make_file_select_html()
                            s = s.Replace("%SELECTVIDEO%", shtml)
                        End If

                        '配信中簡易リスト
                        If s.IndexOf("%PROCBONLIST") >= 0 Then
                            Dim gt() As String = get_atags("%PROCBONLIST:", s)
                            Dim js As String = Me._procMan.get_live_numbers_bon().Replace(vbCrLf, "<br>")
                            If js.Length > 0 Then
                                js = gt(1) & js & gt(3)
                            End If
                            s = s.Replace("%PROCBONLIST:" & gt(0) & "%", js)
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
                        sw.WriteLine(ERROR_PAGE("bad request", "ページが見つかりません"))
                        sw.Flush()
                        log1write(path & "が見つかりませんでした")
                    End If
                    res.Close()
                Else
                    '認証エラー
                    context.Response.StatusCode = 401
                End If

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

    '%変数に埋め込まれた前中後に挿入すべきhtmlタグを抽出
    Private Function get_atags(ByVal tag As String, ByVal s As String) As Object
        'tag="%～:" s=html
        '返値　d(0)=抽出タグ文字列 d(1)=前 d(2)=中 d(3)=後
        Dim d() As String
        ReDim Preserve d(3)
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
        If vbt.Length = 1 Then
            d(2) = vbt(0)
        ElseIf vbt.Length = 2 Then
            d(2) = vbt(0)
            d(3) = vbt(1)
        ElseIf vbt.Length = 3 Then
            d(1) = vbt(0)
            d(2) = vbt(1)
            d(3) = vbt(2)
        End If

        Return d
    End Function

    '%SELECTCHをhtmlに置換して返す
    Private Function replace_html_selectch(ByVal num As Integer, ByVal rez As String, ByVal atag() As String) As String
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
            vhtml = atag(1) & vhtml
            vhtml &= "</select>" & vbCrLf
            vhtml &= atag(2)
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
                                If filename.IndexOf(".ts") > 0 Or filename.IndexOf(".mp4") Then
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
    Public Function ERROR_PAGE(ByVal title As String, ByVal body As String) As String
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
        r &= "<input type=""button"" value=""トップメニューへ"" onClick=""location.href='index.html'"">" & vbCrLf
        r &= "</body>" & vbCrLf
        r &= "</html>" & vbCrLf
        Return r
    End Function

    'HTML内置換用　配信しているストリームのボタンを作成
    Public Function WEB_make_ViewLink_html(ByVal atag() As String) As String
        Dim html As String = ""
        Dim bst As String = ""

        Dim gln As String = Trim(Me._procMan.get_live_numbers())
        If gln.Length > 0 Then
            Dim d() As String = gln.Split(" ")
            For i As Integer = 0 To d.Length - 1
                If d(i) > 0 Then
                    html &= bst
                    Dim ChannelName As String = Me._procMan.get_channelname(d(i))
                    If ChannelName.Length > 20 Then
                        '長すぎるときはカット
                        ChannelName = ChannelName.Substring(0, 18) & ".."
                    End If
                    If ChannelName.Length > 0 Then
                        html &= "<input type=""button"" value=""" & d(i).ToString & "　" & ChannelName & """ onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                    Else
                        html &= "<input type=""button"" value=""ストリーム" & d(i).ToString & "を視聴"" onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
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
                html &= atag(1)
                html &= "<script type=""text/javascript"" src=""ConnectedSelect.js""></script>" & vbCrLf
                html &= "<select id=""SEL1"" name=""BonDriver"">" & vbCrLf
                html &= "<option value="""">---</option>" & vbCrLf
                For i = 0 To bons.Length - 1
                    html &= "<option value=""" & bons(i) & """>" & bons(i) & "</option>" & vbCrLf
                Next
                html &= "</select>" & vbCrLf
                html &= atag(2)
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
                html &= atag(3)
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

    '映像配信開始
    Public Sub start_movie(ByVal num As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal ChSpace As Integer, ByVal udpApp As String, ByVal hlsApp As String, hlsOpt1 As String, ByVal hlsOpt2 As String, ByVal wwwroot As String, ByVal fileroot As String, ByVal hlsroot As String, ByVal ShowConsole As Boolean, ByVal udpOpt3 As String, ByVal filename As String, Optional ByVal resolution As String = "")
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

        'log1write("UDP option=" & udpOpt)

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

        'ファイル再生か？
        Dim stream_mode As Integer = 0
        If filename.Length > 0 Then
            stream_mode = 1
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

        'log1write("HLS option=" & hlsOpt)

        Directory.SetCurrentDirectory(fileroot) 'カレントディレクトリ変更
        '★プロセスを起動
        Me._procMan.startProc(udpApp, udpOpt, hlsApp, hlsOpt, num, udpPortNumber, ShowConsole, stream_mode, resolution)
    End Sub

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
        Dim line() As String = file2line("VideoPath.txt")

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
                        End Select
                    End If
                Next
            End If
        Catch ex As Exception
        End Try

    End Sub

End Class


