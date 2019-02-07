Imports System.IO

Module モジュール_その他
    'バージョン
    Public TvRemoteViewer_VB_version As Double = 2.91
    Public TvRemoteViewer_VB_notrecommend_version As Double = 0
    Public TvRemoteViewer_VB_recommend_version As Double = 0
    Public TvRemoteViewer_VB_version_check_datetime As DateTime = CDate("2000/01/01") '何分何秒にチェックするか　起動時に決定
    Public TvRemoteViewer_VB_version_check_on As Integer = 1 'バージョンチェックする=1
    Public TvRemoteViewer_VB_version_NG As Integer = 0 '強く更新を求めるバージョンならば1
    Public TvRemoteViewer_VB_version_URL As String = "http://vb45wb5b.up.seesaa.net/image/version.txt"
    Public TvRemoteViewer_VB_revision As String = "g"

    Public RestartExe_path As String = 0

    'Write制限
    Public CanFileOpeWrite As Integer = 0
    Public form1_ID As String = ""
    Public form1_PASS As String = ""

    Public NotifyIcon_status As Integer = 0 '1=NG状態

    'debug
    Public log_debug As Integer = 0
    Public Sub write_log_debug(ByVal title As String, ByVal value As String)
        If log_debug = 1 Then
            log1 = "　" & vbCrLf & "　" & vbCrLf & log1
            If title.Length > 0 Then
                If StrConv(title.Substring(title.Length - 1, 1), VbStrConv.Wide) = "：" Then
                    log1 = "【DEBUG】 " & title & value & vbCrLf & log1
                    log1 = "　" & vbCrLf & "　" & vbCrLf & log1
                    Exit Sub
                End If
            End If
            log1 = value & vbCrLf & log1
            log1 = "【DEBUG】 " & title & vbCrLf & log1
            log1 = "　" & vbCrLf & "　" & vbCrLf & log1
        End If
    End Sub

    'StarDigioダミー
    Public StarDigio_dummy_ON As Integer = 0 '1の場合は番組データが無くとも放送局名だけは表示する

    '指定語句が含まれるBonDriverは無視する
    Public BonDriver_NGword As String() = Nothing

    'クライアント設定
    Public client_allowDomains() As String 'アクセス許可するドメイン
    Public client_allowFiles() As String 'アクセス許可するファイル

    'TvRemoteFilesスタイル
    Public TVRemoteFilesNEW As Integer = 1 '1なら新しい画面推移方法

    'リモコンで使用するドメイン
    Public Remocon_Domains() As String = Nothing

    'LOGのPATH
    Public log_path As String = "TvRemoteViewer_VB.log"

    'Windowの状態
    Public me_left As Integer = 0
    Public me_top As Integer = -10
    Public me_width As Integer = 0
    Public me_height As Integer = 0
    Public me_window_backup As String = ""
    Public close2min As Integer = 0

    'プロファイル　hlsApp,resolution,Audioモード
    Public profiletxt As String = ""

    'パイプ経由時にffmpegに渡すパラメータ
    Public PipeRun_ffmpeg_option As String = "-i %VIDEOFILE% -vcodec copy -vsync -1 -async 1000 -f mpegts pipe:1"

    'ファイル再生はffmpegを使用する場合は1
    Public video_force_ffmpeg As Integer = 0

    '明示的にHLSアプリを指定する場合の各アプリ実行ファイルパス
    Public exepath_ffmpeg As String = "" 'thumbnail_ffmpegと置き換え
    Public exepath_VLC As String = ""
    Public exepath_QSVEnc As String = ""
    Public exepath_NVEnc As String = ""
    Public exepath_VCEEnc As String = ""
    Public exepath_ISO_VLC As String = ""

    'フォーム上の解像度コンボボックス.text
    Public form1_resolution As String = ""
    'フォーム上のHLSオプションか解像度どちらを選択するかのコンボボックス
    Public form1_hls_or_rez As String = ""

    '直前配信履歴簡易記録　各ストリームが最後に使用した識別文字列（短時間での重複配信指令防止）
    Public stream_last_utime(8) As Integer

    'Android UCブラウザのようにRefreshの解釈が違うブラウザ対策
    Public meta_refresh_fix As Integer = 0

    'html発行方法 0=OutputStream.Write&BYTE() 1=StreamWriter&flush()←旧方式
    Public html_publish_method As Integer = 1

    'EDCBのopenfix機能（チューナー初起動時に一旦違うチャンネルに合わせる）用
    Public openfix_BonSid() As String = Nothing

    '名前指定で終了させるアプリ
    Public StopUdpAppName As String = ""

    'アイドルが指定分続くときは切断する
    Public STOP_IDLEMINUTES As Integer = 300
    Public STOP_IDLEMINUTES_LAST As DateTime '最後に.htmlにアクセスがあった日時
    Public STOP_IDLEMINUTES_METHOD As Integer = 1 '1=WIアクセスのみ反応　2=WEBアクセス全てに反応

    'HTML入力文字コード（HTMLファイルの文字コード）
    Public HTML_IN_CHARACTER_CODE As String = "UTF-8" 'Shift_JIS
    'HTML出力文字コード（ブラウザに出力する文字コード）
    Public HTML_OUT_CHARACTER_CODE As String = "UTF-8" 'Shift_JIS

    '最大配信ナンバー
    Public MAX_STREAM_NUMBER As Integer = 8

    'アプリCPU優先度
    Public UDP_PRIORITY As String = "" 'High
    Public HLS_PRIORITY As String = ""

    'UDPとHLSの間に挟むウェイト(ms) 0=WaitForInputIdle
    Public UDP2HLS_WAIT As Integer = 500

    'ダミーチャンネルに合わせた後に入れるWAIT 0=WaitForInputIdle
    Public OPENFIX_WAIT As Integer = 0

    'TvRemoteViewer_VBの起動時、終了時、全停止時にRecTaskまたはTSTaskを名前付きで停止するかどうか
    Public Stop_RecTask_at_StartEnd As Integer = 1

    'TvRemoteViewer_VBの起動時、終了時、全停止時にffmpegを名前付きで停止するかどうか
    Public Stop_ffmpeg_at_StartEnd As Integer = 1

    'TvRemoteViewer_VBの起動時、終了時、全停止時にvlcを名前付きで停止するかどうか
    Public Stop_vlc_at_StartEnd As Integer = 1

    'TvRemoteViewer_VBの起動時、終了時、全停止時にQSVEncを名前付きで停止するかどうか
    Public Stop_QSVEnc_at_StartEnd As Integer = 1

    'TvRemoteViewer_VBの起動時、終了時、全停止時にNVEncを名前付きで停止するかどうか
    Public Stop_NVEnc_at_StartEnd As Integer = 1

    'TvRemoteViewer_VBの起動時、終了時、全停止時にVCEEncを名前付きで停止するかどうか
    Public Stop_VCEEnc_at_StartEnd As Integer = 1

    'HTMLにID:PASS@を変換してもよいかどうか
    Public ALLOW_IDPASS2HTML As Integer = 0

    'ffmpegHTTPストリーム　クライアントから切断された場合、何秒後に配信自体を停止するか
    Public FFMPEG_HTTP_CUT_SECONDS As Integer = 5 '元は3

    '配信中に古いtsファイルを削除するかどうか
    Public OLDTS_NODELETE As Integer = 0

    'プレミアムSPHD用RecTask
    Public RecTask_SPHD As String = ""
    'プレミアムSPHD用ServiceIDの範囲
    Public SPHD_sid_start As Integer = 33024
    Public SPHD_sid_end As Integer = 33791

    'チャンネル切り替え時にRecTaskを必ず再起動するかどうか
    Public RecTask_force_restart As Integer = 0

    'RecTaskのチャンネル変更最大待機時間
    Public RecTask_CH_MaxWait As Integer = 5 '標準は5秒

    '何回起動失敗したか
    Public stream_reset_count() As Integer
    Public stream_reset_limit As Integer = 3 '何回ストリーム起動に失敗したら配信停止するか

    '何回waitingメッセージを表示したか
    Public waitingmessage_count() As Integer
    Public waitingmessage_str() As String
    Public waitingmessage_slow_limit As Integer = 10 '秒間waitingが続いたらrefreshを長くする
    Public waitingmessage_slow_sec As Integer = 4 'どれだけのrefresh値にするか

    Public Function time2unix(ByVal t As DateTime) As Integer
        Try
            Dim ut As Integer = DateDiff("s", #1/1/1970#, t)
            ut = ut - (60 * 60 * 9) '日本時間
            Return ut
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function unix2time(ByVal unixTimeStamp As Integer) As DateTime
        unixTimeStamp = unixTimeStamp + (60 * 60 * 9) '日本時間
        Dim unixDate As DateTime = (New DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(unixTimeStamp) '.ToLocalTime()
        'Return CDate(unixDate.ToShortDateString & " " & unixDate.ToLongTimeString)
        Return unixDate
    End Function

    'ファイル名に含まれている,をエスケープ
    Public Function filename_escape_set(ByVal s As String) As String
        Return s.Replace(",", "_，").Replace("'", "_.’") 'エスケープ
    End Function

    'ファイル名に含まれている,を戻す
    Public Function filename_escape_recall(ByVal s As String) As String
        Return s.Replace("_，", ",").Replace("_.’", "'") 'エスケープしていた,を元に戻す
    End Function

    '余計な改行等を削除
    Public Function trim8(ByVal s As String) As String
        s = Trim(s)
        s = s.Replace(vbTab, "").Replace(vbCrLf, "").Replace("""", "")
        s = Trim(s)
        Return s
    End Function

    'パラメーター値を取得 例：instr_pickup_para("/ch 9","/ch "," ", 0) → 9
    Public Function instr_pickup_para(ByVal s As String, ByVal s1 As String, ByVal s2 As String, ByVal sp As Integer) As String
        Dim r As String = ""
        Try
            Dim a1 As Integer = s.IndexOf(s1, sp)
            Dim a2 As Integer = -1
            Try
                a2 = s.IndexOf(s2, a1 + s1.Length)
            Catch ex As Exception
                a2 = -1
            End Try
            If a1 >= 0 And a2 > a1 Then
                r = s.Substring(a1 + s1.Length, a2 - a1 - s1.Length)
            ElseIf a1 >= 0 And a2 < 0 Then
                '2番目の識別子が見つからない場合
                r = s.Substring(a1 + s1.Length)
            End If
        Catch ex As Exception
        End Try
        Return r
    End Function

    '(相対)パスを絶対パスで返す
    Public Function path_s2z(ByVal path As String) As String
        If path.IndexOf(":") > 0 Or path.IndexOf("\\") = 0 Or trim8(path).Length = 0 Then
            '絶対パス
            Return path
            Exit Function
        Else
            '相対パス
            Dim basePath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            basePath &= "\"

            'http://dobon.net/vb/dotnet/file/getabsolutepath.html

            Try
                While path.Substring(0, 2) = ".\"
                    Try
                        path = path.Substring(2)
                    Catch ex As Exception
                        path = ""
                    End Try
                End While
            Catch ex As Exception
                '1文字のフォルダの可能性があるのでそのままスルー
            End Try

            Try
                Dim filePath As String = path

                '"%"を"%25"に変換しておく（デコード対策）
                basePath = basePath.Replace("%", "%25")
                filePath = filePath.Replace("%", "%25")

                '絶対パスを取得する
                Dim u1 As New Uri(basePath)
                Dim u2 As New Uri(u1, filePath)
                Dim absolutePath As String = u2.LocalPath
                '"%25"を"%"に戻す
                absolutePath = absolutePath.Replace("%25", "%")

                Return absolutePath
            Catch ex As Exception
                log1write("【エラー】相対→絶対パス変換エラー。" & ex.Message)
                Return ""
            End Try
        End If
    End Function

    'URLから拡張子を取得
    Public Function GetExtensionFromURL(ByVal url As String) As String
        Dim sp As Integer = url.IndexOf("?")
        If sp > 0 Then
            url = url.Substring(0, sp)
        End If
        Return Path.GetExtension(url).ToLower
    End Function

    '文字列に指定文字が何個含まれているか
    Public Function count_str(ByVal html As String, ByVal s As String) As Integer
        Dim r As Integer = 0
        If s.Length > 0 Then
            r = Int((html.Length - html.Replace(s, "").Length) / s.Length)
        End If
        Return r
    End Function

    'プログラム用
    Public C_DAY2038 As DateTime = CDate("2038/01/01 23:59")
    Public C_INTMAX As Integer = 2147483647
End Module

