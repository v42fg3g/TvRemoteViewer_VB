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
