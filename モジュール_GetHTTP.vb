Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO
Imports System.Runtime.InteropServices

Module モジュール_GetHTTP
    <DllImport("urlmon.dll", CharSet:=CharSet.Ansi)> _
    Private Function UrlMkSetSessionOption(ByVal intOption As Integer, ByVal str As String, ByVal intLength As Integer, ByVal intReserved As Integer) As Integer
    End Function
    Private Const URLMON_OPTION_USERAGENT As Integer = &H10000001

    Public Function get_html_by_WebBrowser(ByVal url As String, Optional ByVal enc_str As String = "UTF-8", Optional ByVal user_agent As String = "", Optional ByVal inner_html As Integer = 0) As String
        Dim r As String = ""

        Try
            Dim WebBrowser1 As New WebBrowser '←！！！ここで即座にエラー！！！
            'エラー内容　'System.Threading.ThreadStateException' の初回例外が System.Windows.Forms.dll で発生しました。
            WebBrowser1.ScriptErrorsSuppressed = True 'スクリプトエラー非表示
            If user_agent.Length > 0 Then
                UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, user_agent, user_agent.Length, 0)
            End If
            WebBrowser1.Navigate(url)

            '読み込み完了まで待つ
            Do
                Do
                    Application.DoEvents()
                Loop While WebBrowser1.IsBusy Or WebBrowser1.ReadyState <> WebBrowserReadyState.Complete
                Application.DoEvents()
            Loop While WebBrowser1.IsBusy Or WebBrowser1.ReadyState <> WebBrowserReadyState.Complete

            If inner_html = 1 Then
                r = WebBrowser1.Document.Body.InnerHtml
            Else
                r = WebBrowser1.Document.Body.OuterHtml
            End If

            WebBrowser1.Dispose()
        Catch ex As Exception
            log1write("【エラー】HTML取得に失敗しました[1]。" & ex.Message)
        End Try

        Return r
    End Function

    Public Function get_html_by_webclient(ByVal url As String, ByVal enc_str As String, Optional ByVal UserAgent As String = "") As String
        Dim r As String = ""
        Try
            Dim wc As WebClient = New WebClient()
            If UserAgent.Length > 0 Then
                wc.Headers.Add("User-Agent", UserAgent)
            End If
            Dim st As Stream = wc.OpenRead(url)
            Dim enc As Encoding = Encoding.GetEncoding(enc_str)
            Dim sr As StreamReader = New StreamReader(st, enc)
            r = sr.ReadToEnd()
            sr.Close()
            st.Close()
            wc.Dispose()
        Catch ex As Exception
            log1write("【エラー】HTML取得に失敗しました[2]。" & ex.Message)
        End Try

        Return r
    End Function

    Public Function get_html_by_HttpWebRequest(ByVal url As String, ByVal enc_str As String, Optional ByVal UserAgent As String = "") As String
        Dim ghtml As String = ""
        Try
            Dim reply_url As String
            Dim reply_code As Integer
            Dim reply_message As String

            'WebRequestの作成
            Dim webreq As System.Net.HttpWebRequest = _
                CType(System.Net.WebRequest.Create(url),  _
                System.Net.HttpWebRequest)

            'UserAgent
            If UserAgent.Length > 0 Then
                webreq.UserAgent = UserAgent
            End If

            'タイムアウト関係
            webreq.Timeout = 15000 '15秒
            webreq.KeepAlive = False '試しにこうしてみる

            'HTML内を調べるとき
            Dim enc As System.Text.Encoding
            Dim st As System.IO.Stream
            Dim html As String = ""

            Dim webres As System.Net.HttpWebResponse = Nothing
            Try
                'サーバーからの応答を受信するためのWebResponseを取得
                webres = CType(webreq.GetResponse(), System.Net.HttpWebResponse)
                'エンコード
                enc = System.Text.Encoding.GetEncoding(enc_str)
                '応答データを受信するためのStreamを取得
                st = webres.GetResponseStream()
                Dim sr As New System.IO.StreamReader(st, enc)
                '受信して表示
                html = sr.ReadToEnd()
                sr.Close()
                '応答したURIを表示する
                reply_url = webres.ResponseUri.ToString
                '応答ステータスコードを表示する
                reply_code = webres.StatusCode
                reply_message = webres.StatusDescription
                '閉じる
                webres.Close()
            Catch e123 As System.Net.WebException
                'HTTPプロトコルエラーかどうか調べる
                If e123.Status = System.Net.WebExceptionStatus.ProtocolError Then
                    'HttpWebResponseを取得
                    Dim errres As System.Net.HttpWebResponse = _
                        CType(e123.Response, System.Net.HttpWebResponse)
                    reply_url = errres.ResponseUri.ToString
                    reply_code = errres.StatusCode
                    reply_message = errres.StatusDescription
                Else
                    reply_url = ""
                    reply_code = 0
                    reply_message = e123.Message.ToString
                End If
            End Try

            If reply_code = 200 Then
                'ページが正常に読み込めたら
                'htmlにページソース
                ghtml = html
            Else
                'URLが見つからない
                ghtml = "[ERROR:" & reply_code & "]"
                log1write("【エラー】URL読み込みエラー " & url & " エラーコード：" & reply_code)
            End If
        Catch ex As Exception
            log1write("【エラー】HTML取得に失敗しました[3]。" & ex.Message)
        End Try

        Return ghtml

    End Function

    Public Function get_html_by_HttpWebRequest_Proxy(ByVal url As String, ByVal enc_str As String, Optional ByVal UserAgent As String = "") As String
        Dim reply_URL As String = ""
        Dim reply_code As Integer = 0

        Dim url_org As String = url
        Dim comment_vol As Integer = 0 '末尾から何件返すか
        Dim isDat As Integer = 0
        Dim base_url As String = "" 'ヘッダー用
        Dim url_footer As String = url 'フッター用
        Dim f_livename As String = ""
        Dim f_sure_num As String = ""
        Dim f_servername As String = ""
        If url.IndexOf("/read.cgi/") > 0 Then
            Dim sp As Integer = url.LastIndexOf("/l")
            If sp > 0 Then
                Dim cc As String = url.Substring(sp).Replace("/l", "").Trim
                If IsNumeric(cc) Then
                    comment_vol = Val(cc)
                End If
                url_footer = url.Substring(0, sp).Replace("https:", "").Replace("http:", "") '//himawari.5ch.net/test/read.cgi/livecx/9876543210
            End If
            'URLをdat形式に変更
            'http://nhk2.5ch.net/test/read.cgi/livenhk/1578976941/l50
            'http://nhk2.5ch.net/livenhk/dat/1578976941.dat
            Dim d() As String = url.Split("/")
            If d.Length >= 7 Then
                'httpsだとプロキシがうまく働かなかった
                'url = d(0) & "//" & d(2) & "/" & d(5) & "/dat/" & d(6) & ".dat"
                url = "http://" & d(2) & "/" & d(5) & "/dat/" & d(6) & ".dat"
                base_url = "https://" & d(2) & "/" & d(5) & "/" 'ヘッダー用
                log1write("【dat】URLを変換しました。" & url)
                f_livename = d(5)
                f_sure_num = d(6)
                f_servername = d(2)
                isDat = 1
            Else
                log1write("【エラー】URL変換に失敗しました。" & url)
            End If
        ElseIf url.IndexOf("/dat/") > 0 And url.IndexOf(".dat") > 0 Then
            isDat = 1
        End If

        Dim ghtml As String = ""
        'Dim reply_url As String = ""
        Dim reply_message As String = ""
        Try
            'WebRequestの作成
            Dim webreq As System.Net.HttpWebRequest =
                CType(System.Net.WebRequest.Create(url),
                System.Net.HttpWebRequest)

            'プロキシの設定
            If Ch5_Proxy_Server.Length > 0 Then
                If (Ch5_read_dat = 1 And isDat = 1) Or Ch5_read_dat = 2 Then
                    Debug.Print("[PROXY]")
                    Dim proxy As New System.Net.WebProxy(Ch5_Proxy_Server)
                    webreq.Proxy = proxy
                End If
            End If

            'UserAgent
            If UserAgent.Length > 0 Then
                webreq.UserAgent = UserAgent
            Else
                webreq.UserAgent = Ch5_UserAgent
            End If

            'タイムアウト関係
            webreq.Timeout = 8000 '8秒
            webreq.KeepAlive = False '試しにこうしてみる

            'HTML内を調べるとき
            Dim enc As System.Text.Encoding
            Dim st As System.IO.Stream
            Dim html As String = ""

            Dim webres As System.Net.HttpWebResponse = Nothing
            Try
                'サーバーからの応答を受信するためのWebResponseを取得
                webres = CType(webreq.GetResponse(), System.Net.HttpWebResponse)
                'エンコード
                enc = System.Text.Encoding.GetEncoding(enc_str)
                '応答データを受信するためのStreamを取得
                st = webres.GetResponseStream()
                Dim sr As New System.IO.StreamReader(st, enc)
                '受信して表示
                html = sr.ReadToEnd()
                sr.Close()
                '応答したURIを表示する
                reply_URL = webres.ResponseUri.ToString
                '応答ステータスコードを表示する
                reply_code = webres.StatusCode
                reply_message = webres.StatusDescription
                '閉じる
                webres.Close()
            Catch e123 As System.Net.WebException
                'HTTPプロトコルエラーかどうか調べる
                If e123.Status = System.Net.WebExceptionStatus.ProtocolError Then
                    'HttpWebResponseを取得
                    Dim errres As System.Net.HttpWebResponse =
                        CType(e123.Response, System.Net.HttpWebResponse)
                    reply_URL = errres.ResponseUri.ToString
                    reply_code = errres.StatusCode
                    reply_message = errres.StatusDescription
                Else
                    reply_URL = ""
                    reply_code = 0
                    reply_message = e123.Message.ToString
                End If
            End Try

            If reply_code = 200 Then
                'ページが正常に読み込めたら
                ghtml = html

                If url.IndexOf("/subback.html") > 0 Then
                    Ch5_subback_cache = ghtml
                End If

                If isDat = 1 Then
                    reply_URL = url_org
                    If ghtml.IndexOf("5ちゃんねる ★<>") >= 0 And ghtml.IndexOf("5ch.netに対応しておりません") > 0 Then
                        ghtml = ""
                        log1write("【警告】要専ブラ。" & url & " ReplyCode=" & reply_code)
                    ElseIf ghtml.IndexOf("<title>もうずっと人大杉") > 0 Then
                        ghtml = ""
                        log1write("【警告】人大杉。" & url & " ReplyCode=" & reply_code)
                    ElseIf ghtml.IndexOf("2017/10/01(") > 0 Then
                        '要専ブラと同一だった
                        ghtml = ""
                        log1write("【警告】取得エラー。" & url & " ReplyCode=" & reply_code)
                    ElseIf ghtml.IndexOf("<>") > 0 Then
                        '5chスレhtml風に加工
                        Dim last_date As String = "1980/01/01(火) 00:00:00.00"
                        Dim line() As String = Split(ghtml, vbLf)
                        Dim coms() As String = Nothing
                        If comment_vol = 0 Or comment_vol > line.Length Then
                            comment_vol = line.Length
                        End If
                        ReDim coms(comment_vol + 1) '51=52スロット
                        Dim j As Integer = coms.Length - 1
                        For i As Integer = line.Length - 1 To 0 Step -1
                            Dim num As Integer = i + 1 'スレ番
                            Dim name As String = ""
                            Dim mail As String = ""
                            Dim datestr As String = ""
                            Dim id As String = ""
                            Dim comment As String = ""

                            If j = 0 Then
                                i = 0 '1番最初はレス1を入れる
                            End If

                            Dim chk As Integer = 0
                            Dim d() As String = Split(line(i), "<>")
                            If d.Length >= 4 Then
                                Dim f() As String = Split(d(2), " ID:")
                                If f.Length = 1 Then
                                    'IDが無い場合もある
                                    id = "NoID_" & (i + 1).ToString
                                ElseIf f.Length >= 2 Then
                                    id = f(1)
                                End If
                                If f(0).IndexOf("/") > 0 Then
                                    'Dim id As String = id
                                    num = i + 1 'スレ番
                                    name = d(0)
                                    mail = d(1)
                                    datestr = f(0)
                                    comment = d(3) 'ここでTrimしていたのでAAの先頭がおかしくなっていた
                                    last_date = datestr

                                    If j = coms.Length - 1 And comment.Trim.Length = 0 Then
                                        '末尾の意味の無い行は飛ばす
                                    Else
                                        coms(j) = "<div class=""post"" id=""" & num & """ data-date=""NG"" data-userid=""ID:" & id & """ data-id=""" & num & """>"
                                        coms(j) &= "<div class=""meta""><span class=""number"">" & num & "</span>"
                                        coms(j) &= "<span class=""name""><b>" & name & "</b></span>"
                                        coms(j) &= "<span class=""date"">" & datestr & "</span>"
                                        coms(j) &= "<span class=""uid"">ID:" & id & "</span></div>"
                                        coms(j) &= "<div class=""message""><span class=""escaped"">" & comment & "</span></div></div>"

                                        If j > 0 Or i > 0 Then
                                            coms(j) &= "<br>"
                                        End If

                                        j -= 1
                                        chk = 1
                                    End If
                                End If
                            End If
                            If chk = 0 And j < coms.Length - 1 And j > 0 Then
                                If i < 1000 And line(i).Trim.Length > 0 Then
                                    log1write("【dat】レス変換に失敗しました。" & (i + 1).ToString & ": " & line(i))
                                End If
                                coms(j) = "<div class=""post"" id=""" & num & """ data-date=""NG"" data-userid=""ID:" & "failed" & """ data-id=""" & num & """>"
                                coms(j) &= "<div class=""meta""><span class=""number"">" & num & "</span>"
                                coms(j) &= "<span class=""name""><b>" & "dat取得失敗" & "</b></span>"
                                coms(j) &= "<span class=""date"">" & last_date & "</span>"
                                coms(j) &= "<span class=""uid"">ID:" & "failed" & "</span></div>"
                                coms(j) &= "<div class=""message""><span class=""escaped""> " & "" & " </span></div></div>"

                                j -= 1
                            End If
                            If j < 0 Or i = 0 Then
                                Exit For
                            End If
                        Next
                        ghtml = String.Join("", coms).Trim

                        If ghtml.IndexOf("<div class=""message"">") > 0 Then
                            reply_code = 200
                            'ヘッダーとフッターを付加
                            Dim sure_title As String = get_sure_title(f_servername, f_livename, f_sure_num)
                            Dim header As String = "<!DOCTYPE HTML>" & vbLf & "<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=Shift_JIS""><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><meta name=""viewport"" content=""width=device-width, initial-scale=1""><base href=""" & base_url & """><link href=""//agree.5ch.net/_guchi/css/header.css"" rel=""stylesheet"" type=""text/css""><link href=""//agree.5ch.net/_guchi/css/bootstrap.min.css"" rel=""stylesheet"" type=""text/css""><link href=""//agree.5ch.net/css/_st.css"" rel=""stylesheet"" type=""text/css""><link href=""//himawari.5ch.net/css/ad.css"" rel=""stylesheet"" type=""text/css""><script type=""text/javascript"" src=""//potato.5ch.net/js/jquery-1.11.3.min.js""></script><script type=""text/javascript"" src=""//agree.5ch.net/_guchi/js/bootstrap.min.js""></script><script type=""text/javascript"" src=""//assets.5ch.net/main.js""></script><script type=""text/javascript"" src=""//agree.5ch.net/js/ad.js?""></script><script type=""text/javascript"" src=""//penguin.5ch.net/js/premium.js""></script><title>" & sure_title & " " & vbLf & "</title><link href=""//agree.5ch.net/_guchi/css/style.css"" rel=""stylesheet"" type=""text/css""><!--[if lt IE 9]> <link href='//agree.5ch.net/_guchi/css/ie.css' rel='stylesheet' type='text/css'><script src='//agree.5ch.net/_guchi/js/html5shiv.min.js'></script> <script src='//agree.5ch.net/_guchi/js/respond.min.js'></script> <![endif]--><script type=""text/javascript"" src=""//agree.5ch.net/js/thumbnailer.js""></script></head><body><input type=""hidden"" id=""zxcvtypo"" value=""//himawari.5ch.net/test/read.cgi/livecx/1578993333""><nav class=""navbar-fixed-top search-header""><div class=""container""><div class=""navbar-header""><button type=""button"" class=""navbar-toggle"" data-toggle=""collapse"" data-target=""#myNavbar""><span class=""icon-bar""></span><span class=""icon-bar""></span><span class=""icon-bar""></span></button><div class=""search-logo pull-left hidden-xs""><a href=""https://www.5ch.net/""><img src=""//penguin.5ch.net/images/5ch_logo.png""></a></div><div class=""search-input col-md-5 col-sm-6""><div class=""input-group""><input id=""search-text"" type=""text"" placeholder=""キーワードを入力"" class=""form-control""><span class=""input-group-btn""><button id=""search-button"" class=""btn"" type=""button""><img src=""//penguin.5ch.net/images/magni.png"" class=""magni"">検索</button></span></div></div></div><div class=""navbar-collapse collapse"" id=""myNavbar""><div class=""search-right hidden-xs""><div class=""search-setting dropdown""><img class=""dropBtnSetting settingDrop searchright"" src=""//penguin.5ch.net/images/icon_settings.png"" style=""height: 22px; width: auto;""><span class=""dropBtnSetting settingDrop""><span class=""dropBtnSetting"">設定</span><img class=""dropBtnSetting extend"" src=""//penguin.5ch.net/images/icon_extend.png""></span><div class=""arrow-triangle""></div></div></div><div class=""nav visible-xs-block""><div class=""search-right""></div></div><div id=""mySetting"" class=""dropdownContentSetting""><a href=""https://be.5ch.net"" target=""_blank"" class=""optsetting"">BE ログイン</a><a href=""https://premium.5ch.net"" target=""_blank"" class=""optsetting"">プレミアムRonin を購入</a><hr><a href=""javascript:void(0)"" id=""options"" class=""optsetting"">閲覧設定</a></div></div></div></nav><div class=""container container_body mascot"" style=""background-image: url(&#39;//agree.5ch.net/mona.png&#39;);""><h1 class=""title"">" & sure_title & vbLf & "</h1><div class=""pagestats""><ul class=""menujust""><li class=""metastats meta centered"">220コメント</li><li class=""metastats meta centered"">35KB</li></ul></div><div class=""topnav centered""><ul class=""menujust""><li class=""menutopnav""><a class=""menuitem"" href=""//himawari.5ch.net/test/read.cgi/livecx/1578993333/"">全部</a></li><li class=""menutopnav""><a class=""menuitem"" href=""//himawari.5ch.net/test/read.cgi/livecx/1578993333/-100"">1-100</a></li><li class=""menutopnav""><a class=""menuitem"" href=""//himawari.5ch.net/test/read.cgi/livecx/1578993333/l50"">最新50</a></li></ul></div><div class=""topmenu centered""><ul class=""menujust""><li class=""menutopmenu""><a class=""itest menuitem"" href=""//itest.5ch.net/test/read.cgi/livecx/1578993333/"">★スマホ版★</a></li><li class=""menutopmenu""><a class=""highlight menuitem"" href=""//himawari.5ch.net/livecx/"">■掲示板に戻る■</a></li><li class=""menutopmenu""><a class=""ula menuitem"" href=""//ula.5ch.net/5ch/livecx/himawari.5ch.net/1578993333/l10"">★ULA版★</a></li></ul></div><div class=""socialmedia""><span class=""social sclsusucoin""><a target=""_BLANK"" href=""https://susukino.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/susu_coin.png""></a></span><span class=""social sclozma""><a target=""_BLANK"" href=""http://ozma.beer/""><img class=""iconsize"" src=""//agree.5ch.net/images/ozma.png""></a></span><span class=""social sclfacebook""><a target=""_BLANK"" href=""https://www.facebook.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/facebook.png""></a></span><span class=""social sclline""><a target=""_BLANK"" href=""line://msg/text/%E5%AE%9F%E6%B3%81%20%E2%97%86%20%E3%83%95%E3%82%B8%E3%83%86%E3%83%AC%E3%83%93%2091132%20%0A%20https%3A%2F%2Fhimawari.5ch.net%2Ftest%2Fread.cgi%2Flivecx%2F1578993333%2F""><img class=""iconsize"" src=""//agree.5ch.net/images/line.png""></a></span><span class=""social scltwit""><a target=""_BLANK"" href=""https://twitter.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/twitter.png""></a></span></div><div class=""thread"">"
                            Dim footer As String = "</div><div class=""push""></div><div class=""pagestats""><ul class=""menujust""><li class=""metastats meta centered"">" & (line.Length - 1).ToString & "コメント</li><li class=""metastats meta centered"">35KB</li></ul></div><div class=""newposts""><span class=""newpostbutton""><a class=""newpb"" href=""" & url_footer & "/220-n"">新着レスの表示</a></span></div><div class=""bottomnav centered""><ul class=""menujust""><li class=""menubottomnav""><a class=""menuitem"" href=""" & url_footer & "/"">全部</a></li><li class=""menubottomnav""><a class=""menuitem"" href=""" & url_footer & "/1-1"">前100</a></li><li class=""menubottomnav""><a class=""menuitem"" href=""" & url_footer & "/221-320"">次100</a></li><li class=""menubottomnav""><a class=""menuitem"" href=""" & url_footer & "/l50"">最新50</a></li></ul></div><div class=""bottommenu centered""><ul class=""menujust""><li class=""menubottommenu""><a class=""itest menuitem"" href=""//itest.5ch.net/test/read.cgi/" & f_livename & "/" & f_sure_num & "/"">★スマホ版★</a></li><li class=""menubottommenu""><a class=""menuitem highlight"" href=""//" & f_servername & "/" & f_livename & "/"">■掲示板に戻る■</a></li><li class=""menubottommenu""><a class=""ula menuitem"" href=""//ula.5ch.net/5ch/" & f_livename & "/" & f_servername & "/" & f_sure_num & "/l10"">★ULA版★</a></li></ul></div><div class=""socialmedia""><span class=""social sclsusucoin""><a target=""_BLANK"" href=""https://susukino.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/susu_coin.png""></a></span><span class=""social sclozma""><a target=""_BLANK"" href=""http://ozma.beer/""><img class=""iconsize"" src=""//agree.5ch.net/images/ozma.png""></a></span><span class=""social sclfacebook""><a target=""_BLANK"" href=""https://www.facebook.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/facebook.png""></a></span><span class=""social sclline""><a target=""_BLANK"" href=""line://msg/text/%E5%AE%9F%E6%B3%81%20%E2%97%86%20%E3%83%95%E3%82%B8%E3%83%86%E3%83%AC%E3%83%93%2091132%20%0A%20https%3A%2F%2F" & f_servername & "%2Ftest%2Fread.cgi%2F" & f_livename & "%2F" & f_sure_num & "%2F""><img class=""iconsize"" src=""//agree.5ch.net/images/line.png""></a></span><span class=""social scltwit""><a target=""_BLANK"" href=""https://twitter.com/""><img class=""iconsize"" src=""//agree.5ch.net/images/twitter.png""></a></span></div><div class=""formbox""><div class=""stronger formheader"">レスを投稿する</div><div class=""formbody""><form method=""POST"" accept-charset=""Shift_JIS"" action=""//" & f_servername & "/test/bbs.cgi""><p><input class=""formelem maxwidth"" placeholder=""名前(省略可)"" name=""FROM"" size=""70""><input class=""formelem maxwidth"" placeholder=""メールアドレス(省略可)"" name=""mail"" size=""70""><textarea class=""formelem maxwidth"" placeholder=""コメント内容"" rows=""5"" cols=""70"" wrap=""off"" name=""MESSAGE""></textarea><input type=""hidden"" name=""bbs"" value=""" & f_livename & """><input type=""hidden"" name=""key"" value=""" & f_sure_num & """><input type=""hidden"" name=""time"" value=""1579021351""><input class=""submitbtn btn"" type=""submit"" value=""書き込む"" name=""submit""><input class=""oekakibtn btn"" type=""hidden"" name=""oekaki_thread1""><script type=""text/javascript""><!--" & vbLf & " function standardize(node){" & vbLf & "  if(!node.addEventListener)" & vbLf & "   node.addEventListener=function(t,l,c){this[""on""+t]=l;};" & vbLf & "  if(!node.dispatchEvent) " & vbLf & "   node.dispatchEvent=function(e){this[""on""+e.type](e);};" & vbLf & " }" & vbLf & " var pf=document.getElementsByName('oekaki_thread1');" & vbLf & " pf[0].insertAdjacentHTML('afterend','<input type=""button"" class=""oekaki_load1"" value=""お絵描きLOAD"">')" & vbLf & " var ol=document.getElementsByClassName('oekaki_load1')[0];" & vbLf & " standardize(ol);" & vbLf & " var oekaki_script=document.createElement('SCRIPT');" & vbLf & " oekaki_script.setAttribute('src','//www2.5ch.net/wpaint2/oekaki.js');" & vbLf & " oekaki_script.setAttribute('id','oekaki_script');" & vbLf & " ol.addEventListener(""click""," & vbLf & "  function(){" & vbLf & "   if(typeof window.oekaki!==""undefined"")return false;" & vbLf & "   document.getElementsByTagName('HEAD')[0].appendChild(oekaki_script);" & vbLf & "   document.getElementById('oekaki_script').addEventListener('load',function(){ setTimeout(500,oekaki.init()); });" & vbLf & "  }" & vbLf & " );" & vbLf & "--></script></p></form></div></div><div class=""footer push"">read.cgi ver 07.2.6 2018/12 <a class=""orange"" href=""https://twitter.com/5chan_nel"">Walang Kapalit ★</a><br><a class=""green"" href=""https://www.instagram.com/ciphersimian/"">Cipher Simian ★</a></div></div></body></html>"

                            '結合
                            ghtml = header & ghtml & footer
                        Else
                            ghtml = ""
                            log1write("【警告】スレ内にコメントが1件もありませんでした。" & url & " ReplyCode=" & reply_code)
                        End If
                    Else
                        ghtml = ""
                        log1write("【警告】未知のエラー。" & url & " ReplyCode=" & reply_code)
                    End If
                End If
            ElseIf reply_code = 503 Then
                ghtml = ""
                log1write("【エラー】URL読み込みエラー " & url & " ReplyCode=" & reply_code)
            ElseIf html.Length > 200 Then
                ghtml = ""
                log1write("【警告】想定外の返答です。" & url & " ReplyCode=" & reply_code & vbCrLf & html)
            Else
                'URLが見つからない
                ghtml = ""
                log1write("【エラー】HTML取得エラー " & url & " ReplyCode=" & reply_code & vbCrLf & html)
            End If
        Catch ex As Exception
            log1write("【エラー】HTML取得に失敗しました[3]。" & url & "    " & ex.Message)
        End Try

        Return ghtml
    End Function

    Private Function get_sure_title(ByVal server_name As String, ByVal ita_name As String, ByVal thread_num As String) As String
        Dim r As String = "タイトル不明"
        'http://nhk2.5ch.net/livenhk/subback.html
        Dim subback_url As String = "https://" & server_name & "/" & ita_name & "/subback.html"
        Dim ti As Integer = -1
        If Ch5_subback_cache.Length > 0 Then
            Dim html As String = Ch5_subback_cache
            Dim sp As Integer = html.IndexOf(thread_num)
            If sp > 0 Then
                Dim title As String = Instr_pickup(html, ":", "</", sp)
                sp = title.LastIndexOf("(")
                If sp > 0 Then
                    title = title.Substring(0, sp)
                End If
                title = title.Trim
                If title.Length > 0 Then
                    r = title
                End If
            End If
        End If

        Return r
    End Function

    Public Function isLocalIP(ByVal IP As String) As Integer
        Dim r As Integer = 0

        Dim d() As String = IP.Split(".")
        If d.Length = 4 Then
            If Val(d(0)) = 10 Then
                r = 1
            ElseIf Val(d(0)) = 172 Then
                If Val(d(1)) >= 16 And Val(d(1)) <= 31 Then
                    r = 1
                End If
            ElseIf Val(d(0)) = 192 And Val(d(1)) = 168 Then
                r = 1
            ElseIf IP = "127.0.0.1" Then
                r = 1
            End If
        End If

        Return r
    End Function
End Module
