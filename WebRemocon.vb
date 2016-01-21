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
'★【ffmpeg】倍速再生できる動画にエンコードする
'http://looooooooop.blog35.fc2.com/blog-entry-1014.html

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
    Public ffmpeg_file_option() As HLSoptionstructure
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

    'ビデオ一覧で表示する拡張子
    Public VideoExtensions() As String

    '同一BonDriverを複数ストリームで使用可能とする
    Public Allow_BonDriver4Streams As Integer = 0

    'ファイル再生において前回使用したファイル名file_last_filename(num)
    'ファイル再生　nohsub=3用
    Public file_last_filename(MAX_STREAM_NUMBER + 1) As String

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
            vhtml = "<select class=""c_sel_bonsidch"" name=""Bon_Sid_Ch"" id=""SEL2"" onChange=""changeSelect()"">" & vbCrLf & vhtml
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

            'If rez.Length > 0 Then
            'vhtml &= "<input type=""hidden"" name=""resolution"" value=""" & rez & """>" & vbCrLf
            'End If
            vhtml &= WEB_make_select_resolution(rez) & vbCrLf '解像度選択
            vhtml &= "<input type=""submit"" class=""c_smt_view"" value=""視聴"" />" & vbCrLf
            vhtml &= "<input type=""hidden"" name=""num"" value=""" & num & """>" & vbCrLf
            vhtml &= "<input type=""hidden"" name=""redirect"" value=""ViewTV" & num & ".html"">" & vbCrLf
            vhtml &= "</form>" & vbCrLf
            vhtml &= atag(3)
        End If

        Return vhtml
    End Function

    'ファイル再生ページ用　ビデオ選択htmlを作成する
    Public Function make_file_select_html(ByVal videoexword As String, ByVal RefreshList As Integer, ByVal start_date As DateTime, ByVal vl_volume As Integer) As String
        Dim shtml As String = ""

        Dim datestr As String = start_date.ToString("yyyyMMddHH")
        If vl_volume = 0 Then
            vl_volume = C_INTMAX
        End If

        If RefreshList = 1 Then
            'リフレッシュするように指定されていればファイルリストを更新
            video = RefreshVideoList("")
        End If
        Dim video2() As videostructure = Nothing
        If videoexword.Length > 0 Then
            'フィルタ
            video2 = RefreshVideoListExword(videoexword)
        Else
            video2 = video
        End If

        Dim i As Integer = 0
        Dim cnt As Integer = 0
        Dim last_date As DateTime
        If video2 IsNot Nothing Then
            For i = 0 To video2.Length - 1
                Dim f As videostructure = video2(i)
                Dim fdatestr As String = ""
                Try
                    fdatestr = f.datestr.Substring(0, 8) 'yyyyMMddHHからyyyyMMddにする
                    If f.datestr < datestr And (cnt < vl_volume Or (cnt >= vl_volume And fdatestr = last_date.ToString("yyyyMMdd"))) Then
                        shtml &= "<option value=""" & f.datestr & "," & f.encstr & """>" & f.filename & "</option>" & vbCrLf
                        last_date = video2(i).modifytime
                        VIDEOFROMDATE = last_date
                        cnt += 1
                    ElseIf cnt > 0 Then
                        Exit For
                    End If
                Catch ex As Exception
                    '不正なデータ
                    log1write("ビデオファイル一覧に不正なデータがありました" & f.fullpathfilename & " " & ex.Message)
                End Try
            Next
        End If

        'If shtml.Length > 0 Then
        shtml = "<option value="""">---</option>" & vbCrLf & shtml
        shtml = "<select class=""c_sel_videoname"" id=""VideoName"" name=""VideoName"" size=""15"">" & vbCrLf & shtml

        shtml &= "</select>" & vbCrLf
        'End If

        '抽出テキストに抽出ワードを戻すスクリプトを追加
        If videoexword.Length > 0 Then
            shtml &= "<SCRIPT>" & vbCrLf
            shtml &= "objSelect = document.getElementById('VideoExWord');" & vbCrLf
            shtml &= "objSelect.value = '" & videoexword & "';" & vbCrLf
            shtml &= "</SCRIPT>" & vbCrLf
        End If

        Return shtml
    End Function

    'ビデオファイルネーム取得
    Public Function WI_GET_VIDEOFILES(ByVal videoexword As String, ByVal RefreshList As Integer, ByVal start_date As DateTime, ByVal vl_volume As Integer) As String
        Return make_file_select_html(videoexword, RefreshList, start_date, vl_volume)
    End Function

    'ファイル一覧リスト　WEBインターフェース
    Public Function WI_GET_VIDEOFILES2(ByVal videoexword As String, ByVal RefreshList As Integer, ByVal start_date As DateTime, ByVal vl_volume As Integer) As String
        Dim r As String = ""

        Dim datestr As String = start_date.ToString("yyyyMMddHH")
        If vl_volume = 0 Then
            vl_volume = C_INTMAX
        End If

        If RefreshList = 1 Then
            'リフレッシュするように指定されていればファイルリストを更新
            video = RefreshVideoList("")
        End If
        Dim video2() As videostructure = Nothing
        If videoexword.Length > 0 Then
            'フィルタ
            video2 = RefreshVideoListExword(videoexword)
        Else
            video2 = video
        End If

        Dim i As Integer = 0
        Dim cnt As Integer = 0
        Dim last_date As DateTime
        If video2 IsNot Nothing Then
            For i = 0 To video2.Length - 1
                Dim f As videostructure = video2(i)
                Dim fdatestr As String = ""
                Try
                    fdatestr = f.datestr.Substring(0, 8) 'yyyyMMddHHからyyyyMMddにする
                    If f.datestr < datestr And (cnt < vl_volume Or (cnt >= vl_volume And fdatestr = last_date.ToString("yyyyMMdd"))) Then
                        'r &= video2(i).modifytime & "," & video2(i).fullpathfilename
                        r &= video2(i).datestr & "," & video2(i).encstr & vbCrLf 'value値と同じもの
                        last_date = video2(i).modifytime
                        VIDEOFROMDATE = last_date
                        cnt += 1
                    ElseIf cnt > 0 Then
                        Exit For
                    End If
                Catch ex As Exception
                    '不正なデータ
                    log1write("ビデオファイル一覧に不正なデータがありました" & f.fullpathfilename & " " & ex.Message)
                End Try
            Next
        End If

        'ヘッダを追加
        Dim rcode As Integer = 0
        If cnt = 0 Then
            rcode = 2 'リクエストを満たすデータがありません
            last_date = CDate("1970/01/01 00:00:00")
        ElseIf i >= video2.Length Then
            rcode = 1 'これが最終です　yyyは 0と同じ xxxx/xx/xxは空白
        End If
        'Dim header As String = rcode & "," & cnt & "," & start_date.ToString("yyyy/MM/dd") & vbCrLf
        'Dim header As String = rcode & "," & cnt & "," & last_date.ToString("yyyy/MM/dd HH:mm:ss") & vbCrLf
        Dim header As String = rcode & "," & cnt & "," & last_date.ToString("yyyy/MM/dd") & vbCrLf

        r = header & r

        Return r
    End Function

    'video()に抽出ワードでフィルタをかけた結果を返す（リフレッシュ無し）
    Public Function RefreshVideoListExword(ByVal videoexword As String) As Object
        Dim video2() As videostructure = Nothing
        If videoexword.Length > 0 Then
            videoexword = videoexword.Replace("　", " ")
            Dim v() As String = videoexword.Split(" ")
            If video IsNot Nothing Then
                If video.Length > 0 Then
                    Dim cnt As Integer = 0
                    For i As Integer = 0 To video.Length - 1
                        'Dim filename As String = video(i).filename
                        Dim filename As String = video(i).fullpathfilename
                        '抽出
                        '全てのワードに当てはまるかチェック
                        For j As Integer = 0 To v.Length - 1
                            If filename.IndexOf(v(j)) < 0 Then
                                filename = ""
                            End If
                        Next

                        If filename.Length > 0 Then
                            ReDim Preserve video2(cnt)
                            video2(cnt).fullpathfilename = video(i).fullpathfilename
                            video2(cnt).filename = video(i).filename
                            video2(cnt).encstr = video(i).encstr
                            video2(cnt).modifytime = video(i).modifytime
                            video2(cnt).datestr = video(i).datestr
                            cnt += 1
                        End If
                    Next
                End If
            End If
        Else
            video2 = video
        End If
        Return video2
    End Function

    'video()にビデオファイルリストを作成
    Public Function RefreshVideoList(ByVal videoexword As String) As Object
        Dim video2() As videostructure = Nothing
        Dim cnt As Integer = 0
        Dim i As Integer
        If Me._videopath IsNot Nothing Then
            If Me._videopath.Length > 0 Then

                For i = 0 To Me._videopath.Length - 1
                    If Me._videopath(i).Length > 0 Then
                        Try
                            Dim files As String() = System.IO.Directory.GetFiles(Me._videopath(i), "*")
                            'For Each stFilePath As String In System.IO.Directory.GetFiles(Me._videopath(i), "*.*") ', "*.ts")
                            For Each stFilePath As String In files
                                Try
                                    Dim fullpath As String = stFilePath
                                    '拡張子を取得
                                    Dim ext As String = System.IO.Path.GetExtension(fullpath)
                                    '表示拡張子が指定されていれば該当するかチェックする
                                    Dim chk As Integer = -2
                                    If VideoExtensions IsNot Nothing And ext.Length > 0 Then
                                        chk = Array.IndexOf(VideoExtensions, ext)
                                    End If
                                    'chk=-2 拡張子指定は無い
                                    'chk>=0 拡張子指定が有り、一致した
                                    'chk=-1 拡張子指定が有り、一致しなかった
                                    If chk >= 0 Or (chk = -2 And ext <> ".db" < 0 And ext <> ".chapter" And ext <> ".srt" And ext <> ".ass") Then
                                        '更新日時 作成日時に変更と思ったがコピー等するとおかしくなるので更新日時にした
                                        Dim modifytime As DateTime = System.IO.File.GetLastWriteTime(fullpath)
                                        Dim datestr As String = modifytime.ToString("yyyyMMddHH")
                                        Dim filename As String = System.IO.Path.GetFileName(fullpath)
                                        'なぜかそのまま渡すと返ってきたときに文字化けするのでURLエンコードしておく
                                        'UTF-8化で解決
                                        'Dim encstr As String = System.Web.HttpUtility.UrlEncode(fullpath)
                                        Dim encstr As String = fullpath

                                        '抽出
                                        If videoexword.Length > 0 Then
                                            videoexword = videoexword.Replace("　", " ")
                                            Dim v() As String = videoexword.Split(" ")
                                            '全てのワードに当てはまるかチェック
                                            For j As Integer = 0 To v.Length - 1
                                                'If filename.IndexOf(v(j)) < 0 Then
                                                If encstr.IndexOf(v(j)) < 0 Then 'フォルダもフィルタに含めるようにした
                                                    filename = ""
                                                End If
                                            Next
                                        End If

                                        If filename.Length > 0 Then
                                            'ファイルのサイズを取得 fi.length
                                            Dim flength As Long = 100
                                            If VideoSizeCheck = 1 Then
                                                Dim fi As New System.IO.FileInfo(fullpath)
                                                flength = fi.Length
                                            End If
                                            If flength >= 100 Then
                                                '登録
                                                ReDim Preserve video2(cnt)
                                                fullpath = filename_escape_set(fullpath) ',をエスケープ
                                                video2(cnt).fullpathfilename = fullpath
                                                filename = filename_escape_set(filename) ',をエスケープ
                                                video2(cnt).filename = filename
                                                encstr = filename_escape_set(encstr) ',をエスケープ
                                                video2(cnt).encstr = encstr
                                                video2(cnt).modifytime = modifytime
                                                video2(cnt).datestr = datestr
                                                cnt += 1
                                            End If
                                        End If
                                    End If
                                Catch ex As Exception
                                    log1write("ビデオフォルダ一覧作成中に" & stFilePath & "の処理に失敗しました。" & ex.Message)
                                End Try
                            Next stFilePath
                        Catch ex As Exception
                            log1write("ビデオフォルダ " & Me._videopath(i) & " の読み込みに失敗しました。" & ex.Message)
                        End Try
                    End If
                Next

                '並べ替え（日付の新しい順）
                If video2 IsNot Nothing Then
                    Array.Sort(video2)
                End If
            End If
        End If
        log1write("ビデオファイル一覧を取得しました")
        Return video2
    End Function

    'エラーページ用ひな形
    Public Function ERROR_PAGE(ByVal title As String, ByVal body As String, Optional ByVal a As Integer = 0) As String
        Dim r As String = ""
        If file_exist(Me._wwwroot & "\ERROR.html") = 1 Then
            r = ReadAllTexts(Me._wwwroot & "\ERROR.html")
            r = r.Replace("%ERRORTITLE%", title)
            r = r.Replace("%ERRORMESSAGE%", body)
            Dim reload As String = ""
            If a = 1 Then
                reload = "<input type=""button"" class=""c_btn_reload"" Value=""再読み込み"" onClick=""location.reload();"">"
            End If
            r = r.Replace("%ERRORRELOAD%", reload)
        Else
            r &= "<!doctype html>" & vbCrLf
            r &= "<html>" & vbCrLf
            r &= "<head>" & vbCrLf
            r &= "<title>" & title & "</title>" & vbCrLf
            r &= "<meta http-equiv=""Content-Type"" content=""text/html; charset=" & HTML_OUT_CHARACTER_CODE & """ />" & vbCrLf
            'r &= "<meta name=""viewport"" content=""width=device-width"">"
            r &= "</head>" & vbCrLf
            r &= "<body>" & vbCrLf
            r &= body & vbCrLf
            r &= "<br><br>" & vbCrLf
            r &= "<input type=""button"" class=""c_btn_topmenu"" value=""トップメニュー"" onClick=""location.href='/index.html'"">" & vbCrLf
            If a = 1 Then
                r &= "<input type=""button"" class=""c_btn_reload"" Value=""再読み込み"" onClick=""location.reload();"">" & vbCrLf
            End If
            r &= "<br><br>" & vbCrLf
            r &= "<input type=""button"" class=""c_btn_back"" value=""直前のページへ戻る"" onClick=""history.go(-1);"">" & vbCrLf
            r &= "</body>" & vbCrLf
            r &= "</html>" & vbCrLf
        End If

        Return r
    End Function

    'ＮＨＫ音声選択用セレクト作成
    Public Function WEB_make_NHKMODE_html(ByVal atag() As String, ByVal num As Integer) As String
        Dim html As String = ""
        Dim bst As String = ""
        html &= atag(1)
        html &= "<select class=""c_sel_nhkmode"" name=""NHKMODE"">"
        html &= vbCrLf & "<option value=""0"">主・副</option>" & vbCrLf
        html &= "<option value=""11"">主</option>" & vbCrLf
        html &= "<option value=""12"">副</option>" & vbCrLf
        html &= "<option value=""4"">第二音声</option>" & vbCrLf
        html &= "<option value=""5"">動画主音声</option>" & vbCrLf
        html &= "<option value=""6"">動画副音声</option>" & vbCrLf
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
        If fileroot.IndexOf(Me._wwwroot) >= 0 Then
            'パスがwwwrootの子ディレクトリ
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
        Else
            '全く違うディレクトリ
            'G:\streamだとしたら
            Dim sp As Integer = fileroot.LastIndexOf("\")
            If sp >= 0 Then
                Try
                    fileroot = fileroot.Substring(sp + 1) & "/"
                Catch ex As Exception
                    fileroot = ""
                    log1write("【エラー】%FILEROOT%の末尾が\になっています")
                End Try
            Else
                'ドライブそのものを指定など D:
                fileroot = ""
                log1write("【エラー】%FILEROOT%にドライブを直接指定はできません。ドライブ内フォルダを指定してください")
            End If
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

                    If (e(i) = 2 Or e(i) = 3) And f(i) = 2 And HTTPSTREAM_WEB_VIEW_TS = 1 Then
                        'HTTPストリーム ffmpeg
                        Dim fileroot As String = get_soutaiaddress_from_fileroot()
                        If ChannelName.Length > 0 Then
                            html &= "<input type=""button"" class=""c_btn_stream"" value=""" & chkstr & d(i) & "　" & ChannelName & sm_str2 & """ onClick=""location.href='" & fileroot & "mystream" & d(i).ToString & ".ts'"">" & vbCrLf
                        Else
                            html &= "<input type=""button"" class=""c_btn_stream"" value=""ストリーム" & chkstr & d(i) & "を視聴" & sm_str2 & """ onClick=""location.href='" & fileroot & "mystream" & d(i).ToString & ".ts'"">" & vbCrLf
                        End If
                    Else
                        'HLS再生ボタン or HTTPストリームは再生できないボタン
                        If ChannelName.Length > 0 Then
                            'html &= "<input type=""button"" value=""" & chkstr & d(i) & "　" & ChannelName & """ onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                            html &= "<input type=""button"" class=""c_btn_stream"" value=""" & chkstr & d(i) & "　" & ChannelName & sm_str2 & """ onClick=""location.href='" & sm_str3 & "ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                        Else
                            'html &= "<input type=""button"" value=""ストリーム" & chkstr & d(i) & "を視聴"" onClick=""location.href='ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
                            html &= "<input type=""button"" class=""c_btn_stream"" value=""ストリーム" & chkstr & d(i) & "を視聴" & sm_str2 & """ onClick=""location.href='" & sm_str3 & "ViewTV" & d(i).ToString & ".html'"">" & vbCrLf
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
                    If System.IO.Path.GetExtension(stFilePath) = ".dll" Then
                        Dim s As String = stFilePath
                        'フルパスファイル名がsに入る
                        'ファイル名だけを取り出す
                        s = Path.GetFileName(s)
                        Dim sl As String = s.ToLower() '小文字に変換
                        '表示しないBonDriverかをチェック
                        If Me._BonDriver_NGword IsNot Nothing Then
                            For j As Integer = 0 To Me._BonDriver_NGword.Length - 1
                                If sl.IndexOf(Me._BonDriver_NGword(j).ToLower) >= 0 Then
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
                    End If
                Next
            Catch ex As Exception
            End Try

            Dim i As Integer = 0
            If bons IsNot Nothing Then
                If bons.Length > 0 Then
                    'BonDriver一覧

                    html_selectbonsidch_a &= "<script type=""text/javascript"" src=""ConnectedSelect.js""></script>" & vbCrLf
                    html_selectbonsidch_a &= "<select class=""c_sel_bondriver"" id=""SEL1"" name=""BonDriver"">" & vbCrLf
                    html_selectbonsidch_a &= "<option value="""">---</option>" & vbCrLf
                    For i = 0 To bons.Length - 1
                        html_selectbonsidch_a &= "<option value=""" & bons(i) & """>" & bons(i) & "</option>" & vbCrLf
                    Next
                    html_selectbonsidch_a &= "</select>" & vbCrLf

                    '各BonDriverに対応したチャンネルを書き込む
                    html_selectbonsidch_b &= "<select class=""c_sel_bonsidch"" id=""SEL2"" name=""Bon_Sid_Ch"" onChange=""changeSelect()"">" & vbCrLf
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

    'HTML内置換用　解像度選択セレクトボックスを作成
    Private Function WEB_make_select_resolution(Optional ByVal rez As String = "", Optional ByVal hls_file As Integer = 0) As String
        Dim html As String = ""
        Dim i As Integer

        Dim ho() As HLSoptionstructure = Nothing
        If hls_file = 1 And ffmpeg_file_option IsNot Nothing Then
            'HLS_option_ffmpeg_file.txtを使用するよう指定があれば
            ho = ffmpeg_file_option
        Else
            ho = hls_option
        End If

        If ho IsNot Nothing Then
            html &= "<select class=""c_sel_resolution"" name=""resolution"">" & vbCrLf
            html &= "<option>---</option>" & vbCrLf
            For i = 0 To ho.Length - 1
                html &= "<option>" & ho(i).resolution & "</option>" & vbCrLf
            Next
            html &= "</select>" & vbCrLf

            If rez.Length > 0 Then
                '指定があれば選択
                html = html.Replace("<option>" & rez & "</option>", "<option selected>" & rez & "</option>")
            End If
        End If

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
                                If IsNumeric(s(1)) And IsNumeric(s(5)) And IsNumeric(s(7)) Then 'サービスID,TSIDが数値なら
                                    If Val(s(8)) > 0 Then
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
                                                If ch_list(i2).sid = Val(s(5)) And ch_list(i2).tsid = Val(s(7)) And ch_list(i2).chspace = Val(s(1)) Then
                                                    'サービスIDとTSIDとchspaceが一致した
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
        'tsの数を返す。m3u8が存在すれば正の値、存在していなければ負の値
        Dim r As Integer = 0
        Dim fileroot As String = Me._fileroot
        If fileroot.Length = 0 Then
            fileroot = Me._wwwroot
        End If
        fileroot &= "\"

        Dim exist_m3u8 As Integer = 1
        If file_exist(fileroot & "mystream" & num.ToString & ".m3u8") <= 0 Then
            'm3u8無し
            exist_m3u8 = -1
        End If

        'tsチェック
        Dim ts_count As Integer = 0
        For Each stFilePath As String In System.IO.Directory.GetFiles(fileroot, "mystream" & num.ToString & "-*.ts")
            ts_count += 1
        Next

        r = exist_m3u8 * ts_count

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
        Try
            Me._listener.Abort() 'Close()
        Catch ex As Exception
            log1write(ex.Message)
        End Try
    End Sub

    'HLS_option.txtから解像度とHLSオプションを読み込む
    Public Sub read_hls_option()
        'カレントディレクトリ変更
        F_set_ppath4program()

        hls_option = set_hls_option("HLS_option.txt")
        vlc_option = set_hls_option("HLS_option_VLC.txt")
        vlc_http_option = set_hls_option("HLS_option_VLC_http.txt")
        ffmpeg_option = set_hls_option("HLS_option_ffmpeg.txt")
        ffmpeg_file_option = set_hls_option("HLS_option_ffmpeg_file.txt")
        ffmpeg_http_option = set_hls_option("HLS_option_ffmpeg_http.txt")
    End Sub

    Public Function set_hls_option(ByVal filename As String) As Object
        Dim r() As HLSoptionstructure = Nothing
        Dim i As Integer = 0

        'カレントディレクトリ変更
        F_set_ppath4program()

        If file_exist(filename) = 1 Then
            Try
                Dim line() As String = file2line(filename)
                If line Is Nothing Then
                    log1write("[HlsOpt] " & filename & "内にオプション記述がありませんでした[A]")
                Else
                    Dim chk As Integer = 0
                    For i = 0 To line.Length - 1
                        Dim youso() As String = line(i).Split("]")
                        If youso Is Nothing Then
                        Else
                            If youso.Length = 2 Then
                                ReDim Preserve r(i)
                                r(i).resolution = Trim(youso(0)).Replace("[", "")
                                r(i).opt = youso(1)
                                chk = 1
                            End If
                        End If
                    Next
                    If chk = 1 Then
                        log1write("[HlsOpt] " & filename & "からHLSオプションを取得しました")
                    Else
                        log1write("[HlsOpt] " & filename & "内にオプション記述がありませんでした")
                    End If
                End If
            Catch ex As Exception
                log1write("[HlsOpt] " & filename & "からHLSオプションを取得できませんでした。" & ex.Message)
            End Try
        Else
            log1write("[HlsOpt] " & filename & "が存在しませんでした")
        End If

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
                Try
                    If youso Is Nothing Then
                    ElseIf youso.Length > 1 Then
                        For j = 0 To youso.Length - 1
                            youso(j) = trim8(youso(j))
                        Next
                        Select Case youso(0)
                            Case "VideoPath"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "")
                                If trim8(youso(1)).Length > 0 Then
                                    Dim clset() As String = youso(1).Split(",")
                                    If clset Is Nothing Then
                                    ElseIf clset.Length > 0 Then
                                        ReDim Preserve Me._videopath(clset.Length - 1)
                                        For j = 0 To clset.Length - 1
                                            Me._videopath(j) = trim8(clset(j))
                                        Next
                                    End If
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
                                        TvProgram_NGword(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramptTimer_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramptTimer_NGword(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgramptTimer_NGword(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramTvmaid_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramTvmaid_NGword(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgramTvmaid_NGword(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramEDCB_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramEDCB_NGword(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgramEDCB_NGword(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramTvRock_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramTvRock_NGword(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgramTvRock_NGword(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramEDCB_ignore"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    ReDim Preserve TvProgramEDCB_ignore(clset.Length - 1)
                                    For j = 0 To clset.Length - 1
                                        TvProgramEDCB_ignore(j) = StrConv(trim8(clset(j)), VbStrConv.Wide) '全角で保存
                                    Next
                                End If
                            Case "TvProgramD_BonDriver1st"
                                TvProgramD_BonDriver1st = para_split_str(youso(1).ToString)
                            Case "TvProgramS_BonDriver1st"
                                TvProgramS_BonDriver1st = para_split_str(youso(1).ToString)
                            Case "TvProgramP_BonDriver1st"
                                TvProgramP_BonDriver1st = para_split_str(youso(1).ToString)
                            Case "TvProgram_tvrock_url"
                                TvProgram_tvrock_url = trim8(youso(1).ToString)
                                Dim sp As Integer = TvProgram_tvrock_url.IndexOf("?d")
                                If sp > 0 Then
                                    Try
                                        TvProgram_tvrock_tuner = Val(youso(2))
                                    Catch ex As Exception
                                        TvProgram_tvrock_tuner = -1
                                    End Try
                                    TvProgram_tvrock_url = TvProgram_tvrock_url.Substring(0, sp)
                                    log1write("TVROCK番組表チューナーに" & TvProgram_tvrock_tuner.ToString & "番を指定しました")
                                End If
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
                            Case "TvProgramEDCB_premium"
                                'プレミアム指定
                                TvProgramEDCB_premium = Val(youso(1).ToString)
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
                            Case "VideoExtensions"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    For j = 0 To clset.Length - 1
                                        clset(j) = trim8(clset(j))
                                        If clset(j).Length > 0 Then
                                            '.が先頭になければ追加
                                            If clset(j).Substring(0, 1) <> "." Then
                                                clset(j) = "." & clset(j)
                                            End If
                                            If VideoExtensions Is Nothing Then
                                                ReDim Preserve VideoExtensions(0)
                                            Else
                                                ReDim Preserve VideoExtensions(VideoExtensions.Length)
                                            End If
                                            VideoExtensions(VideoExtensions.Length - 1) = clset(j)
                                        End If
                                    Next
                                End If
                            Case "BonDriver_NGword"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    '既存のBonDriver_NGwordに追加
                                    Dim k As Integer = Me._BonDriver_NGword.Length
                                    For j = 0 To clset.Length - 1
                                        If clset(j).Length > 0 Then
                                            If Me._BonDriver_NGword Is Nothing Then
                                                ReDim Preserve Me._BonDriver_NGword(0)
                                            Else
                                                ReDim Preserve Me._BonDriver_NGword(Me._BonDriver_NGword.Length)
                                            End If
                                            Me._BonDriver_NGword(Me._BonDriver_NGword.Length - 1) = trim8(clset(j))
                                        End If
                                    Next
                                End If
                            Case "Stop_RecTask_at_StartQuit", "Stop_RecTask_at_StartEnd"
                                Stop_RecTask_at_StartEnd = Val(youso(1).ToString)
                            Case "Stop_ffmpeg_at_StartEnd"
                                Stop_ffmpeg_at_StartEnd = Val(youso(1).ToString)
                            Case "Stop_vlc_at_StartEnd"
                                Stop_vlc_at_StartEnd = Val(youso(1).ToString)
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
                            Case "HTTPSTREAM_FFMPEG_BUFFER"
                                HTTPSTREAM_FFMPEG_BUFFER = Val(youso(1).ToString)
                            Case "MAX_STREAM_NUMBER"
                                If Val(youso(1).ToString) > 0 Then
                                    MAX_STREAM_NUMBER = Val(youso(1).ToString)
                                    ReDim Preserve file_last_filename(MAX_STREAM_NUMBER + 1)
                                End If
                            Case "UDP_PRIORITY"
                                UDP_PRIORITY = trim8(youso(1).ToString)
                            Case "HLS_PRIORITY"
                                HLS_PRIORITY = trim8(youso(1).ToString)
                            Case "UDP2HLS_WAIT"
                                UDP2HLS_WAIT = Val(youso(1).ToString)
                            Case "OPENFIX_WAIT"
                                OPENFIX_WAIT = Val(youso(1).ToString)
                            Case "ALLOW_IDPASS2HTML"
                                ALLOW_IDPASS2HTML = Val(youso(1).ToString)
                            Case "FFMPEG_HTTP_CUT_SECONDS"
                                FFMPEG_HTTP_CUT_SECONDS = Val(youso(1).ToString)
                                '入出力をUTF-8以外のものは扱わないようにした
                                'Case "HTML_IN_CHARACTER_CODE"
                                'HTML_IN_CHARACTER_CODE = trim8(youso(1).ToString)
                                'Case "HTML_OUT_CHARACTER_CODE"
                                'HTML_OUT_CHARACTER_CODE = trim8(youso(1).ToString)
                            Case "STOP_IDLEMINUTES"
                                STOP_IDLEMINUTES = Val(youso(1).ToString)
                            Case "VideoSeekDefault"
                                VideoSeekDefault = Val(youso(1).ToString)
                            Case "VideoSizeCheck"
                                VideoSizeCheck = Val(youso(1).ToString)
                            Case "TvProgram_SelectUptoNum"
                                TvProgram_SelectUptoNum = Val(youso(1).ToString)
                            Case "OLDTS_NODELETE"
                                OLDTS_NODELETE = Val(youso(1).ToString)
                            Case "RecTask_SPHD"
                                RecTask_SPHD = trim8(youso(1).ToString)
                                If RecTask_SPHD.Length > 0 Then
                                    If file_exist(RecTask_SPHD) <= 0 Then
                                        log1write("【エラー】" & RecTask_SPHD & " が見つかりません")
                                        RecTask_SPHD = ""
                                    Else
                                        log1write("スカパープレミアムSPHD用RecTaskとして " & RecTask_SPHD & " が指定されました")
                                    End If
                                End If
                            Case "RecTask_force_restart"
                                RecTask_force_restart = Val(youso(1).ToString)
                            Case "ptTimer_path"
                                ptTimer_path = youso(1).ToString
                                If ptTimer_path.Length > 0 Then
                                    If ptTimer_path.Substring(ptTimer_path.Length - 1, 1) <> "\" Then
                                        '末尾に\を付ける
                                        ptTimer_path += "\"
                                    End If
                                End If
                            Case "Allow_BonDriver4Streams"
                                Allow_BonDriver4Streams = Val(youso(1).ToString)
                            Case "EDCB_thru_addprogres"
                                EDCB_thru_addprogres = Val(youso(1).ToString)
                            Case "EDCB_Velmy_niisaka"
                                EDCB_Velmy_niisaka = Val(youso(1).ToString)
                            Case "EDCB_GetCh_method"
                                EDCB_GetCh_method = Val(youso(1).ToString)
                            Case "Tvmaid_url"
                                If youso(1).Length > 0 Then
                                    Tvmaid_url = youso(1).ToString
                                    TvmaidIsEX = 0
                                    log1write("TvmaidのサーバーURLが指定されました。" & Tvmaid_url)
                                End If
                            Case "TvmaidEX_url", "TvmaidYUI_url"
                                If youso(1).Length > 0 Then
                                    Tvmaid_url = youso(1).ToString
                                    TvmaidIsEX = 1
                                    log1write("TvmaidYUIのサーバーURLが指定されました。" & Tvmaid_url)
                                End If
                            Case "NicoJK_path"
                                NicoJK_path = youso(1).ToString
                                If NicoJK_path.Length > 0 Then
                                    If folder_exist(NicoJK_path) <= 0 Then
                                        log1write("【エラー】" & NicoJK_path & " が見つかりません")
                                        NicoJK_path = ""
                                    Else
                                        log1write("NicoJKフォルダ：" & NicoJK_path & " が指定されました")
                                    End If
                                End If
                            Case "NicoJK_first"
                                NicoJK_first = Val(youso(1).ToString)
                            Case "NicoConvAss_path"
                                NicoConvAss_path = youso(1).ToString
                                If NicoConvAss_path.Length > 0 Then
                                    If file_exist(NicoConvAss_path) <= 0 Then
                                        log1write("【エラー】" & NicoConvAss_path & " が見つかりません")
                                        NicoConvAss_path = ""
                                    Else
                                        log1write("NicoConvAss：" & NicoConvAss_path & " が指定されました")
                                    End If
                                End If
                            Case "Nico_delay"
                                Nico_delay = Val(youso(1).ToString)
                            Case "RecTask_CH_MaxWait"
                                RecTask_CH_MaxWait = Val(youso(1).ToString)
                                If RecTask_CH_MaxWait < 1 Then
                                    RecTask_CH_MaxWait = 1 '最小値1秒
                                End If
                                log1write("RecTaskがチャンネル変更する際に待機する最大秒数を" & RecTask_CH_MaxWait & "秒に設定しました")
                            Case "make_chapter"
                                make_chapter = Val(youso(1).ToString)
                            Case "chapter_bufsec"
                                chapter_bufsec = Val(youso(1).ToString)
                            Case "openfix_BonSid"
                                youso(1) = youso(1).Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", "")
                                Dim clset() As String = youso(1).Split(",")
                                If clset Is Nothing Then
                                ElseIf clset.Length > 0 Then
                                    If trim8(clset(0)).Length > 0 Then
                                        ReDim Preserve openfix_BonSid(clset.Length - 1)
                                        For j = 0 To clset.Length - 1
                                            openfix_BonSid(j) = trim8(clset(j))
                                        Next
                                    End If
                                End If
                            Case "log_size"
                                log_size = Val(youso(1).ToString)
                            Case "html_publish_method"
                                If IsNumeric(youso(1)) Then
                                    html_publish_method = Val(youso(1).ToString)
                                End If
                            Case "TOT_get_duration"
                                TOT_get_duration = Val(youso(1).ToString)
                                If TOT_get_duration > 0 Then
                                    log1write("ファイル再生時に動画の長さを調べるようセットしました")
                                End If
                            Case "meta_refresh_fix"
                                meta_refresh_fix = Val(youso(1).ToString)



                                'Case "WhiteBrowserWB_path"
                                'WhiteBrowserWB_path = trim8(youso(1).ToString)
                                'If file_exist(WhiteBrowserWB_path) = 1 Then
                                'log1write("WhiteBrowserのデータベースとして " & WhiteBrowserWB_path & " をセットしました")
                                'Else
                                'log1write("【エラー】WhiteBrowserのデータベース " & WhiteBrowserWB_path & " が見つかりません")
                                'WhiteBrowserWB_path = ""
                                'End If
                        End Select
                    End If
                Catch ex As Exception
                    log1write("パラメーター " & youso(0) & " の読み込みに失敗しました。" & ex.Message)
                End Try
            Next
        End If

    End Sub

    'パラメーターを,で区切って数値を配列で返す
    Private Function para_split_int(ByVal s As String) As Object
        Dim r() As String = para_split_str(s)
        If r IsNot Nothing Then
            For i As Integer = 0 To r.Length - 1
                r(i) = Val(r(i))
            Next
        End If
        Return r
    End Function

    'パラメーターを,で区切って文字列を配列で返す
    Private Function para_split_str(ByVal s As String) As Object
        Dim r() As String = Nothing
        Dim cnt As Integer = 0
        s = trim8(s)
        If s.Length > 0 Then
            Dim d() As String = s.Split(",")
            For i As Integer = 0 To d.Length - 1
                d(i) = trim8(d(i))
                If d(i).Length > 0 Then
                    ReDim Preserve r(cnt)
                    r(cnt) = d(i)
                    cnt += 1
                End If
            Next
        End If
        Return r
    End Function

    'ファイル再生
    '現在のhlsOptをファイル再生用に書き換える
    Private Function hlsopt_udp2file_ffmpeg(ByVal hlsOpt As String, ByVal filename As String, ByVal num As Integer, ByVal fileroot As String, ByVal VideoSeekSeconds As Integer, ByVal nohsub As Integer, ByVal baisoku As String, ByVal margin1 As Integer) As String
        'ffmpeg時のみ字幕ファイルがあれば挿入
        Dim chk_duration As Integer = 0 'TOTで動画の長さを調べた場合は1

        filename = filename_escape_recall(filename) ',エスケープを元に戻す

        'エラー防止
        If file_last_filename(num) Is Nothing Then
            file_last_filename(num) = ""
        End If

        '前回のファイル名と違えば字幕ファイルを削除
        Dim exist_nico_ass As Integer = 0 'タイムシフト前大元のASSファイルが存在するかどうか
        If file_exist(fileroot & "\" & "sub" & num.ToString & "_nico.ass") = 1 Then
            exist_nico_ass = 1
        End If
        If file_last_filename(num) <> filename Or filename.Length = 0 Then
            '古いsub%num%.assがあれば削除
            If file_exist(fileroot & "\" & "sub" & num.ToString & ".ass") = 1 Then
                deletefile(fileroot & "\" & "sub" & num.ToString & ".ass")
            End If
            If exist_nico_ass = 1 Then
                exist_nico_ass = 0
                deletefile(fileroot & "\" & "sub" & num.ToString & "_nico.ass")
            End If
            'If file_exist(fileroot & "\" & "chapter" & num.ToString & ".chapter") = 1 Then
            'deletefile(fileroot & "\" & "chapter" & num.ToString & ".chapter")
            'End If
        End If

        'タイムシフトを古い方式でやるなら1
        Dim timeshift_old_flg As Integer = 0
        'シーク方式が旧式指定かどうか
        If ffmpeg_seek_method_files.Length > 1 Then
            Dim line() As String = Split(ffmpeg_seek_method_files, vbCrLf)
            For j2 As Integer = 0 To line.Length - 1
                If Trim(line(j2)).Length > 0 Then
                    If filename.IndexOf(Trim(line(j2))) >= 0 Then
                        'マッチ 古い方式でシークする
                        timeshift_old_flg = 1
                        log1write(filename & "のシーク方式を旧式にセットしました")
                        Exit For
                    End If
                End If
            Next
        End If

        Dim new_file As String = ""
        Dim rename_file As String = "" 'assフルパス
        If (fonts_conf_ok = 1 And hlsOpt.IndexOf("-vcodec copy") < 0 And nohsub = 0) Or nohsub = 2 Then
            'fonts.confが存在し、無変換でなく、ハードサブ禁止でなければ （又はnohsub=2）
            Dim dt As Integer = filename.LastIndexOf(".")
            If dt > 0 Then
                Dim ass_file As String = filename.Substring(0, dt) & ".ass"
                If file_exist(ass_file) <= 0 Then
                    ass_file = ""
                End If
                If exist_nico_ass = 0 Then
                    'NicoJKログをassに変換
                    If NicoJK_path.Length > 0 And NicoConvAss_path.Length > 0 Then
                        If (NicoJK_first = 0 And ass_file.Length = 0) Or NicoJK_first = 1 Then
                            '動画の開始日時（微妙な誤差はあるかも）
                            Dim VideoStartTime As DateTime = get_TOT(filename, Me._hlsApp)
                            chk_duration = 1
                            'txtを探してassに変換してファイル(ass_file)として保存
                            Dim txt_file As String = search_NicoJKtxt_file(filename, Me._hlsApp)
                            If txt_file.Length > 0 Then
                                'txtからassに変換してfileroot & "\" & "sub" & num.ToString & "_nico.ass"として保存
                                log1write("字幕ファイルとしてNicoJKコメント " & txt_file & " を使用します")
                                ass_file = convert_NicoJK2ass(num, txt_file, fileroot, margin1, filename, VideoStartTime)
                                If ass_file.Length > 0 Then
                                    log1write("字幕ASSファイルへの変換が終了しました")
                                End If
                            End If
                        End If
                    End If
                Else
                    'すでに_nico.assファイルが存在する
                    log1write("既存の字幕ASSファイル_nico.assを使用します")
                    ass_file = fileroot & "\" & "sub" & num.ToString & "_nico.ass"
                End If
                If ass_file.Length > 0 Then
                    If file_exist(ass_file) = 1 Then
                        'Try
                        log1write("字幕ASSファイルとして" & ass_file & "を読み込みます")
                        '存在していればstreamフォルダに名前を変えてコピー
                        new_file = "sub" & num.ToString & ".ass"
                        '現在のカレントフォルダを取得（ffmpegの場合そこがstreamフォルダ）
                        rename_file = fileroot & "\" & new_file

                        If (VideoSeekSeconds <= 0 And baisoku = "1") Or timeshift_old_flg = 1 Then
                            'シークが指定されていなければそのままコピー
                            'リネーム
                            My.Computer.FileSystem.CopyFile(ass_file, rename_file, True)
                            'ファイルが出来るまで待機
                            Dim i As Integer = 0
                            While i < 100 And file_exist(rename_file) < 1
                                System.Threading.Thread.Sleep(50)
                                i += 1
                            End While
                            log1write("字幕ASSファイルとして" & rename_file & "をセットしました")
                            'オプションに挿入する文字列を作成
                            If nohsub = 0 Then
                                new_file = " -vf ass=""" & new_file & """"
                            Else
                                new_file = ""
                            End If
                        Else
                            'シークが指定されていれば一旦読み込んで指定秒を開始時間とするようassをシフト
                            log1write("字幕ASSファイルを修正しています")
                            Dim VideoSeekSeconds_temp As Integer = VideoSeekSeconds
                            If timeshift_old_flg = 1 Then
                                '古い方式ならタイムシフトはしない
                                VideoSeekSeconds_temp = 0
                            End If
                            If ass_adjust_seektime(ass_file, rename_file, VideoSeekSeconds_temp, baisoku) = 1 Then
                                '修正完了
                                log1write("字幕ASSファイルの修正が完了しました")
                                log1write("字幕ASSファイルとして" & rename_file & "をセットしました")
                                'オプションに挿入する文字列を作成
                                If nohsub = 0 Then
                                    new_file = " -vf ass=""" & new_file & """"
                                Else
                                    new_file = ""
                                End If
                            Else
                                'エラー
                                log1write("字幕ASSファイルの修正に失敗しました")
                                'オプションに-vfは挿入しない
                                new_file = ""
                            End If
                        End If
                        'Catch ex As Exception
                        'log1write("字幕ASSファイル処理でエラーが発生しました。" & ex.Message)
                        'new_file = ""
                        'End Try
                    End If
                End If
            End If
        ElseIf nohsub = 3 Then
            'nohsub=3の場合は、.assファイルをstreamフォルダに別名でコピーするだけとする
            Dim dt As Integer = filename.LastIndexOf(".")
            If dt > 0 Then
                Dim ass_file As String = filename.Substring(0, dt) & ".ass"
                If file_exist(ass_file) <= 0 Then
                    ass_file = ""
                End If

                '前回と違うファイル、または字幕ファイルが存在しなければ作成
                '前回と同じファイルならすでに存在しているので無駄なことはしない
                If file_last_filename(num) <> filename Or (file_last_filename(num) = filename And file_exist(fileroot & "\" & "sub" & num.ToString & ".ass") <= 0) Then
                    If exist_nico_ass = 0 Then
                        'NicoJKログをassに変換
                        If NicoJK_path.Length > 0 And NicoConvAss_path.Length > 0 Then
                            If (NicoJK_first = 0 And ass_file.Length = 0) Or NicoJK_first = 1 Then
                                '動画の開始日時（微妙な誤差はあるかも）
                                Dim VideoStartTime As DateTime = get_TOT(filename, Me._hlsApp)
                                chk_duration = 1
                                'txtを探してassに変換してファイル(ass_file)として保存
                                Dim txt_file As String = search_NicoJKtxt_file(filename, Me._hlsApp)
                                If txt_file.Length > 0 Then
                                    'txtからassに変換してfileroot & "\" & "sub" & num.ToString & "_nico.ass"として保存
                                    ass_file = convert_NicoJK2ass(num, txt_file, fileroot, margin1, filename, VideoStartTime)
                                    If ass_file.Length > 0 Then
                                        log1write("字幕ファイルとしてNicoJKコメント " & txt_file & " を使用します")
                                    End If
                                End If
                            End If
                        End If
                    Else
                        'すでに_nico.assファイルが存在する
                        log1write("既存の字幕ASSファイル_nico.assを使用します[CopyOnly]")
                        ass_file = fileroot & "\" & "sub" & num.ToString & "_nico.ass"
                    End If

                    If ass_file.Length > 0 Then
                        log1write("字幕ASSファイルとして" & ass_file & "を読み込みます[CopyOnly]")
                        '存在していればstreamフォルダに名前を変えてコピー
                        '現在のカレントフォルダを取得（ffmpegの場合そこがstreamフォルダ）
                        rename_file = fileroot & "\" & "sub" & num.ToString & ".ass"
                        'シークが指定されていなければそのままコピー
                        'リネーム
                        My.Computer.FileSystem.CopyFile(ass_file, rename_file, True)
                        'ファイルが出来るまで待機
                        Dim i As Integer = 0
                        While i < 100 And file_exist(rename_file) < 1
                            System.Threading.Thread.Sleep(50)
                            i += 1
                        End While
                        log1write("字幕ASSファイルとして" & rename_file & "をセットしました[CopyOnly]")
                    End If
                End If
            End If
        End If

        '字幕があればチャプターを打てるかどうかチェック
        If rename_file.Length > 0 And make_chapter = 1 Then
            log1write("コメントファイルからchapterファイル作成を試みます")
            'rename_fileが実際のassファイル（フルパス）
            F_make_chapter(filename, rename_file)
        End If

        '動画の長さを調べていなければ調べる（かつTOT_get_durationが指定されていれば）
        If chk_duration = 0 And TOT_get_duration > 0 Then
            Dim vt As DateTime = get_TOT(filename, Me._hlsApp)
        End If

        '使用したファイル名を記録
        file_last_filename(num) = filename

        filename = """" & filename & """"

        Dim sp As Integer = hlsOpt.IndexOf("-i ")
        If sp >= 0 Then
            Dim se As Integer = hlsOpt.IndexOf(" ", sp + 3)
            If sp >= 0 And se > sp Then
                If hlsOpt.IndexOf(" -vf ") < 0 Then
                    'HLSオプション内に-vfが存在しない場合は
                    ''hlsOpt = hlsOpt.Substring(0, sp) & "-i " & filename & hlsOpt.Substring(se)
                    hlsOpt = hlsOpt.Substring(0, sp) & "-i " & filename & new_file & hlsOpt.Substring(se)
                Else
                    'HLSオプション内に-vfが存在する場合は -vf部分を入れ替える
                    hlsOpt = hlsOpt.Substring(0, sp) & "-i " & filename & hlsOpt.Substring(se)
                    If new_file.Length > 0 Then
                        new_file &= "," '" -vf ass=""" & new_file & """" & " "
                        hlsOpt = hlsOpt.Replace(" -vf ", new_file)
                    End If
                End If
                'セグメント分割部分を削除
                sp = hlsOpt.IndexOf("-segment_list_size ")
                se = hlsOpt.IndexOf(" ", sp + "-segment_list_size ".Length)
                If sp >= 0 And se > sp Then
                    hlsOpt = hlsOpt.Substring(0, sp) & hlsOpt.Substring(se)
                End If
                'セグメントリスト数部分を修正(-hls)
                Dim hsize As Integer = Val(instr_pickup_para(hlsOpt, "-hls_list_size ", " ", 0))
                If hsize <> 0 Then
                    sp = hlsOpt.IndexOf("-hls_list_size ")
                    se = hlsOpt.IndexOf(" ", sp + "-hls_list_size ".Length)
                    If sp >= 0 And se > sp Then
                        hlsOpt = hlsOpt.Substring(0, sp) & "-hls_list_size 0" & hlsOpt.Substring(se)
                    End If
                End If
            End If

            'シーク秒数が指定されていれば「-ss 秒」を挿入
            If VideoSeekSeconds > 0 And timeshift_old_flg = 0 Then
                sp = hlsOpt.IndexOf("-i ")
                hlsOpt = hlsOpt.Substring(0, sp) & "-ss " & VideoSeekSeconds & " " & hlsOpt.Substring(sp)
            ElseIf VideoSeekSeconds > 0 And timeshift_old_flg = 1 Then
                '古い方式でシフトするならば-iの後に挿入
                hlsOpt = insert_str_in_hlsOpt(hlsOpt, "-ss " & VideoSeekSeconds, 2, 4)
            End If

            '倍速指定があれば
            If baisoku <> "1" And baisoku.Length > 0 Then
                Dim bunsuu As String = get_bunsuu_R(baisoku) 'おかしな数値の場合は1で返ってくる
                If bunsuu <> "1/1" Then
                    '2倍速より上の場合はatempoを分割しないといけない
                    Dim atempo As String = ""
                    Dim dbl As Double
                    Try
                        dbl = Decimal.Parse(baisoku)
                        If dbl > 2 Then
                            Dim sep As String = ""
                            While dbl > 2
                                atempo &= sep & "atempo=2"
                                dbl = dbl / 2
                                sep = ","
                            End While
                            If dbl <> 1 Then
                                '小数点3桁までにしておく　→ 廃止。よく考えたら2で割って無理数にはならないわな
                                'dbl = Math.Round(dbl, 3, MidpointRounding.AwayFromZero)
                                atempo &= sep & "atempo=" & dbl
                            End If
                        Else
                            atempo = "atempo=" & baisoku
                        End If
                    Catch ex As Exception
                        dbl = 0
                        log1write("【エラー】倍速指定が不正です")
                    End Try

                    If dbl > 0 Then
                        Dim bchk As Integer = 0
                        If hlsOpt.IndexOf(" -vf ") < 0 Then
                            sp = hlsOpt.IndexOf(" -f ")
                            If sp >= 0 Then
                                '-fの前に付ける
                                If sp > 0 Then
                                    hlsOpt = hlsOpt.Substring(0, sp) & " -vf setpts=" & bunsuu & "*PTS" & hlsOpt.Substring(sp)
                                    bchk = 1
                                End If
                            End If
                        Else
                            '-vfが存在している場合は前部に追加
                            hlsOpt = hlsOpt.Replace(" -vf ", " -vf setpts=" & bunsuu & "*PTS,")
                            bchk = 1
                        End If
                        If bchk = 1 Then
                            If hlsOpt.IndexOf(" -af ") < 0 Then
                                sp = hlsOpt.IndexOf(" -f ")
                                If sp >= 0 Then
                                    '-fの前に付ける
                                    If sp > 0 Then
                                        hlsOpt = hlsOpt.Substring(0, sp) & " -af " & atempo & hlsOpt.Substring(sp)
                                        bchk = 2
                                    End If
                                End If
                            Else
                                '-afが存在している場合は前部に追加
                                hlsOpt = hlsOpt.Replace(" -af ", " -af " & atempo & ",")
                                bchk = 2
                            End If
                        End If
                        If bchk <> 2 Then
                            log1write("HLSオプションへの倍速指定に失敗しました")
                        End If
                    End If
                End If
            End If
        Else
            log1write("【エラー】HlsOptが指定されていません")
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

    'テスト
    Public Function change_exe_name(ByVal s As String, ByVal num As Integer) As String
        'Dim sp As Integer = s.LastIndexOf(".exe")
        Dim sp As Integer = s.LastIndexOf("\")
        If sp > 0 Then
            s = s.Substring(0, sp) & num.ToString & s.Substring(sp)
        End If
        Return s
    End Function

    '映像配信開始
    Public Sub start_movie(ByVal num As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal ChSpace As Integer, ByVal udpApp As String, ByVal hlsApp As String, hlsOpt1 As String, ByVal hlsOpt2 As String, ByVal wwwroot As String, ByVal fileroot As String, ByVal hlsroot As String, ByVal ShowConsole As Boolean, ByVal udpOpt3 As String, ByVal filename As String, ByVal NHK_dual_mono_mode_select As Integer, ByVal Stream_mode As Integer, ByVal resolution As String, ByVal VideoSeekSeconds As Integer, ByVal nohsub As Integer, ByVal baisoku As String, ByVal hlsOptAdd As String, ByVal margin1 As Integer)
        'resolutionの指定が無ければフォーム上のHLSオプションを使用する

        filename = filename_escape_recall(filename) ',を戻す

        'テスト　多重テストを違うexeファイルで行う
        Dim udpapp2 As String = change_exe_name(udpApp, num)
        If file_exist(udpapp2) = 1 Then
            udpApp = udpapp2
        End If
        Dim hlsapp2 As String = change_exe_name(hlsApp, num)
        If file_exist(hlsapp2) = 1 Then
            hlsApp = hlsapp2
        End If

        'プレミアムSPHDならばRecTask入れ替え
        If RecTask_SPHD.Length > 0 And sid >= SPHD_sid_start And sid <= SPHD_sid_end Then
            udpApp = RecTask_SPHD
            log1write("SPHD用UDPアプリとして " & RecTask_SPHD & " を使用します")
        End If

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
                hlsOpt = translate_hls2http(0, hlsOpt2, resolution)

                'ファイル再生
                If filename.Length > 0 And Stream_mode = 3 Then
                    'VLC httpストリームのとき
                    hlsOpt = hlsopt_udp2file_ffmpeg(hlsOpt, filename, num, fileroot, VideoSeekSeconds, nohsub, baisoku, margin1)
                    'chapterをコピー
                    'copy_chapter_to_fileroot(num, filename, fileroot)
                End If
            ElseIf hlsApp.IndexOf("vlc") >= 0 Then
                'hlsOptを置き換える
                hlsOpt = translate_hls2http(1, hlsOpt2, resolution)
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
            If resolution.Length = 0 And hlsApp.IndexOf("ffmpeg") >= 0 And hlsOpt2.Length > 0 And filename.Length > 0 And ffmpeg_file_option IsNot Nothing And (Stream_mode = 0 Or Stream_mode = 1) Then
                '解像度が無指定でHLS配信でHLS_option_ffmpeg_file.txt指定のファイル再生ならば
                'フォーム上のオプションから解像度を算出して解像度をセット
                resolution = trim8(instr_pickup_para(hlsOpt2, "-s ", " ", 0))
            End If
            If resolution.Length > 0 Then
                '解像度指定があれば
                Dim chk As Integer = 0
                If filename.Length > 0 And ffmpeg_file_option IsNot Nothing And (Stream_mode = 0 Or Stream_mode = 1) Then
                    'HLS配信でHLS_option_ffmpeg_file.txt指定のファイル再生ならば
                    If hls_option IsNot Nothing Then
                        For i As Integer = 0 To ffmpeg_file_option.Length - 1
                            If ffmpeg_file_option(i).resolution = resolution Then
                                hlsOpt = ffmpeg_file_option(i).opt
                                chk = 1
                                log1write("HLS_option_ffmpeg_file.txt内の解像度指定がありました。" & resolution)
                            End If
                        Next
                    End If
                End If

                If chk = 0 And hls_option IsNot Nothing Then
                    '標準
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

            'ファイル再生か？
            If filename.Length > 0 Then
                Stream_mode = 1
                'hlsオプションを書き換える
                If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                    'ffmpegのとき
                    If ffmpeg_file_option IsNot Nothing Then
                        'HLS_option_ffmpeg_file.txtでオプションが指定されていれば
                        'resolution=空白、640x380等
                        If resolution.Length = 0 Then
                            'フォーム上のhlsoptから解像度を取得
                            resolution = instr_pickup_para(hlsOpt, " -s ", " ", 0)
                            log1write("フォーム上のHLSオプションから解像度を取得しました。resolution=" & resolution)
                        End If
                        If resolution.Length > 0 Then
                            Dim chk As Integer = 0
                            For i = 0 To ffmpeg_file_option.Length - 1
                                If ffmpeg_file_option(i).resolution = resolution Then
                                    'hlsOpt入れ替え
                                    hlsOpt = ffmpeg_file_option(i).opt
                                    '%VIDEOFILE%変換
                                    'hlsOpt = hlsOpt.Replace("%VIDEOFILE%", "DUMMYFILE")
                                    chk = 1
                                    log1write("HLS_option_ffmpeg_file.txtに記述されているHLSオプションを使用します")
                                    Exit For
                                End If
                            Next
                            If chk = 0 Then
                                log1write("HLS_option_ffmpeg_file.txtに該当する解像度のオプションがありませんでした。resolution=" & resolution)
                            End If
                        End If
                    End If
                    hlsOpt = hlsopt_udp2file_ffmpeg(hlsOpt, filename, num, fileroot, VideoSeekSeconds, nohsub, baisoku, margin1)
                    'chapterをコピー
                    'copy_chapter_to_fileroot(num, filename, fileroot)
                Else
                    'その他vlc
                    '今のところ未対応
                    Exit Sub
                End If
            End If
        End If

        If hlsOpt.Length > 0 Then
            'NHK BS1、BSプレミアム対策
            If hlsApp.IndexOf("ffmpeg") >= 0 Then
                Dim isNHK As Integer = Me._procMan.check_isNHK(0, udpOpt)
                '1=NHKなら主　それ以外はステレオ
                '2=NHKなら主　それ以外はステレオ
                '11=全部主
                '12=N全部副
                '4=メイン
                '5=サブ
                If ((NHK_dual_mono_mode_select = 1 And isNHK = 1) Or NHK_dual_mono_mode_select = 11) And hlsOpt.IndexOf("-dual_mono_mode") < 0 Then
                    '主モノラル固定 1or11
                    hlsOpt = hlsOpt.Replace("-i ", "-dual_mono_mode main -i ")
                ElseIf ((NHK_dual_mono_mode_select = 2 And isNHK = 1) Or NHK_dual_mono_mode_select = 12) And hlsOpt.IndexOf("-dual_mono_mode") < 0 Then
                    '副モノラル固定 2or12
                    hlsOpt = hlsOpt.Replace("-i ", "-dual_mono_mode sub -i ")
                ElseIf NHK_dual_mono_mode_select = 4 Then
                    '第二音声
                    hlsOpt = insert_str_after_i_in_hlsOpt(hlsOpt, "-map 0:v:0 -map 0:a -map -0:a:0")
                ElseIf NHK_dual_mono_mode_select = 5 Then
                    '動画主音声
                    hlsOpt = insert_str_after_i_in_hlsOpt(hlsOpt, "-af pan=stereo|c0=c0|c1=c0")
                ElseIf NHK_dual_mono_mode_select = 6 Then
                    '動画副音声
                    hlsOpt = insert_str_after_i_in_hlsOpt(hlsOpt, "-af pan=stereo|c0=c1|c1=c1")
                ElseIf isNHK = 1 And NHK_dual_mono_mode_select = 9 Then
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
                Else
                    'それ以外は書き換えない　→主・副
                End If

                'hlsOptに追加で文字列
                If hlsOptAdd.Length > 0 Then
                    '例 hlsOptAdd="2,2,-map 0,0 -map 0,1"
                    '例 hlsOptAdd="2,9,-map_-_2,2,-map 0,0 -map 0,1"
                    Dim hOAs() As String = Nothing
                    If hlsOptAdd.IndexOf("_-_") >= 0 Then
                        '複数指定
                        hOAs = hlsOptAdd.Split("_-_")
                    Else
                        ReDim Preserve hOAs(0)
                        hOAs(0) = hlsOptAdd
                    End If
                    For i As Integer = 0 To hOAs.Length - 1
                        Dim d() As String = hOAs(i).Split(",")
                        If d.Length >= 3 Then
                            Dim ba As Integer = Val(d(0)) '1=-iの前　2=-iの後
                            Dim force As Integer = Val(d(1)) '重複の場合、0,1=入れ替えしない 2=交換 3=追加(要素追加優先) 4=追加
                            Dim str As String = ""
                            Dim sep As String = ""
                            For j As Integer = 2 To d.Length - 1
                                str &= sep & d(j)
                                sep = ","
                            Next
                            '文字列を追加
                            hlsOpt = insert_str_in_hlsOpt(hlsOpt, str, ba, force)
                        End If
                    Next
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
            Me._procMan.startProc(udpApp, udpOpt, hlsApp, hlsOpt, num, udpPortNumber, ShowConsole, Stream_mode, NHK_dual_mono_mode_select, resolution, VideoSeekSeconds)
        Else
            log1write("【エラー】HLSオプションが指定されていません。解像度を指定するかフォーム上のHLSオプションを記入してください")
        End If
    End Sub

    '.chapterがあればストリームフォルダにコピー ■未使用
    Public Sub copy_chapter_to_fileroot(ByVal num As Integer, ByVal fullpathfilename As String, ByVal fileroot1 As String)
        Dim targetfilename As String = fileroot1 & "\chapter" & num & ".chapter"
        Dim chapterfullpathfilename As String = ""
        Dim chapterpath As String = Path.GetDirectoryName(fullpathfilename)
        Dim chapterfilename As String = Path.GetFileNameWithoutExtension(fullpathfilename) & ".chapter"
        If chapterfilename.Length > 0 Then
            If file_exist(chapterpath & "\" & chapterfilename) = 1 Then
                chapterfullpathfilename = chapterpath & "\" & chapterfilename
            ElseIf file_exist(chapterpath & "\chapters\" & chapterfilename) = 1 Then
                chapterfullpathfilename = chapterpath & "\chapters\" & chapterfilename
            End If
        End If
        If chapterfullpathfilename.Length > 0 Then
            '見つかればコピー
            Try
                System.IO.File.Copy(chapterfullpathfilename, targetfilename)
                Dim i As Integer = 100 '最大5秒間待つ
                While file_exist(targetfilename) <= 0 And i > 0
                    System.Threading.Thread.Sleep(50)
                    i -= 1
                End While
                If i > 0 Then
                    '成功
                    log1write(chapterfullpathfilename & "を" & targetfilename & "としてコピーしました")
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    'hlsOptの-iの直後にパラメーターを挿入する
    Public Function insert_str_after_i_in_hlsOpt(ByVal hlsstr As String, ByVal str As String) As String
        '-iの直後を探す

        '"がある場合は一旦エスケープ　" -"がパラメータの始まりとは限らない＆日本語は何があるかわからないので
        Dim zenkakufilename() As String = Nothing
        Dim j As Integer = 0
        While hlsstr.IndexOf("""") >= 0
            Dim kakomi As String = Instr_pickup(hlsstr, """", """", 0)
            If kakomi.Length = 0 Then
                '抽出が失敗した　"が１つしかない・・
                Exit While
            Else
                ReDim Preserve zenkakufilename(j)
                zenkakufilename(j) = """" & kakomi & """"
                '一旦エスケープ
                hlsstr = hlsstr.Replace(zenkakufilename(j), "%VIDEOFILENAME" & j & "%")
                j += 1
            End If
        End While

        '-iの後に挿入
        Dim sp As Integer = hlsstr.IndexOf("-i ")
        If sp >= 0 Then
            '日本語が入っていない状態で次のパラメーターの始まりを探すことにした
            sp = hlsstr.IndexOf(" -", sp)
            If sp > 0 Then
                '正常
                hlsstr = hlsstr.Substring(0, sp) & " " & str & hlsstr.Substring(sp)
            Else
                'ありえないだろうが-iの次のパラメーターが無い場合はそのまま追加
                hlsstr = hlsstr & " " & str
            End If
        Else
            'ありえないが-iが無い場合はそのまま追加
            hlsstr = hlsstr & " " & str
        End If

        'エスケープしたファイル名を戻す
        If zenkakufilename IsNot Nothing Then
            For j = 0 To zenkakufilename.Length - 1
                If zenkakufilename(j).Length > 0 Then
                    hlsstr = hlsstr.Replace("%VIDEOFILENAME" & j & "%", zenkakufilename(j))
                End If
            Next
        End If

        Return hlsstr
    End Function

    'hlsOptに任意のパラメーターを挿入する
    Public Function insert_str_in_hlsOpt(ByVal hlsstr As String, ByVal str As String, ByVal ba As Integer, ByVal force As Integer) As String
        'ba=1 -iの前　ba=2 -iの後
        'force=0,1 既にパラメーターが存在すれば書き換えない、force=2 入れ替える force=3追加（値に追加を優先）force=4 そのまま追加 force=9 指定されたパラメータを削除
        Dim r As String = ""

        '"がある場合は一旦エスケープ　" -"がパラメータの始まりとは限らない＆日本語は何があるかわからないので
        Dim zenkakufilename() As String = Nothing
        Dim j As Integer = 0
        While hlsstr.IndexOf("""") >= 0
            Dim kakomi As String = Instr_pickup(hlsstr, """", """", 0)
            If kakomi.Length = 0 Then
                '抽出が失敗した　"が１つしかない・・
                Exit While
            Else
                ReDim Preserve zenkakufilename(j)
                zenkakufilename(j) = """" & kakomi & """"
                '一旦エスケープ
                hlsstr = hlsstr.Replace(zenkakufilename(j), "%VIDEOFILENAME" & j & "%")
                j += 1
            End If
        End While

        '2重空白を除去
        While hlsstr.IndexOf("  ") >= 0
            hlsstr = hlsstr.Replace("  ", " ")
        End While

        'まず挿入しようとするパラメーターの種類を取得
        hlsstr = " " & hlsstr
        Dim s0 As String = "" 'パラメーターより前
        Dim s1 As String = "" '-i前半
        Dim s2 As String = "" '-i部分
        Dim s3 As String = "" '-i後半
        Dim sp As Integer = hlsstr.IndexOf(" -")
        If sp > 0 Then
            'パラメーター前に文字列があればs0に保存
            s0 = Trim(hlsstr.Substring(0, sp))
            hlsstr = hlsstr.Substring(sp)
        ElseIf sp < 0 Then
            s0 = hlsstr
            hlsstr = ""
        End If
        hlsstr = Trim(hlsstr)
        If hlsstr.Length > 0 Then
            'hlsOptを前半、後半に分解
            sp = hlsstr.IndexOf("-i ")
            If sp >= 0 Then
                If sp > 0 Then
                    s1 = Trim(hlsstr.Substring(0, sp))
                End If
                Dim sp2 As Integer = hlsstr.IndexOf("""", sp)
                Dim sp3 As Integer = hlsstr.IndexOf(""" ", sp + 4)
                If sp2 = sp + 3 And sp3 > sp2 Then
                    '-iの値が""で囲まれている
                    s2 = "-i " & instr_pickup_para(hlsstr, "-i ", """ ", sp) & """"
                    s3 = Trim(hlsstr.Substring(sp3 + 1))
                Else
                    '-iの値は空白まで
                    sp3 = hlsstr.IndexOf(" ", sp + 3)
                    s2 = "-i " & instr_pickup_para(hlsstr, "-i ", " ", sp)
                    s3 = Trim(hlsstr.Substring(sp3))
                End If
            Else
                '-iが無い
                s2 = hlsstr
            End If

            'ここまでで以下のような値になっているはず
            'hlsOpt構成要素
            's0 = ""
            's1 = "-vf a"
            's2 = "-i a.mp4"
            's3 = "-af b"

            '挿入予定パラメーター
            Dim p1() As String = Nothing 'パラメータ「-map」
            Dim p2() As String = Nothing 'パラメータのかたまり 「-map 0,1」
            Dim p3() As Integer = Nothing '重複無し1　重複有り0
            Dim d() As String = str.Split(" ")
            Dim i As Integer = 0
            j = 0
            Dim mapchk As Integer = 0
            For i = 0 To d.Length - 1
                If d(i).Length > 0 Then
                    If d(i).Substring(0, 1) = "-" Then
                        Dim chk As Integer = 0
                        If j > 0 And mapchk = 0 Then
                            If p1(j - 1) = "-map" Then
                                '-mapの直後ならば
                                'パラメーターに付随する値
                                p2(j - 1) &= " " & d(i)
                                chk = 1
                                mapchk = 1 '1度-mapの後に付けたら次は付けない
                            End If
                        End If
                        If chk = 0 Then
                            ReDim Preserve p1(j)
                            ReDim Preserve p2(j)
                            ReDim Preserve p3(j)
                            p1(j) = d(i)
                            p2(j) = d(i)
                            p3(j) = 1
                            j += 1
                            mapchk = 0
                        End If
                    Else
                        If p2 IsNot Nothing Then
                            'パラメーターに付随する値
                            p2(j - 1) &= " " & d(i)
                        End If
                    End If
                End If
            Next

            Dim targetstr As String = "" 'パラメーターを追加するターゲットstring
            Dim anotherstr As String = ""
            If ba = 1 Then
                targetstr = s1
                anotherstr = s3
            Else
                targetstr = s3
                anotherstr = s1
            End If

            If p1 IsNot Nothing Then
                If force = 9 Then
                    '削除指定
                    For i = 0 To p1.Length - 1
                        p3(i) = -1
                    Next
                Else
                    'パラメーターがすでに指定されているかチェック
                    For i = 0 To p1.Length - 1
                        If targetstr.IndexOf(p1(i) & " ") >= 0 Then
                            'すでにtargetstr内に存在する
                            p3(i) = 0
                        End If
                    Next
                End If
            End If

            targetstr = " " & targetstr
            Dim ps0 As String = ""
            sp = targetstr.IndexOf(" -")
            If sp > 0 Then
                'パラメーター前に文字列があればp0に保存
                ps0 = Trim(targetstr.Substring(0, sp))
                targetstr = str.Substring(sp)
            End If
            targetstr = Trim(targetstr)
            Dim ps1() As String = Nothing 'パラメータ「-map」
            Dim ps2() As String = Nothing 'パラメータのかたまり 「-map 0,1」
            Dim ps3() As Integer = Nothing '更新済み=1
            d = targetstr.Split(" ")
            j = 0
            mapchk = 0
            For i = 0 To d.Length - 1
                If d(i).Length > 0 Then
                    If d(i).Substring(0, 1) = "-" Then
                        Dim chk As Integer = 0
                        If j > 0 And mapchk = 0 Then
                            If ps1(j - 1) = "-map" Then
                                '-mapの直後ならば
                                'パラメーターに付随する値
                                ps2(j - 1) &= " " & d(i)
                                chk = 1
                                mapchk = 1
                            End If
                        End If
                        If chk = 0 Then
                            ReDim Preserve ps1(j)
                            ReDim Preserve ps2(j)
                            ReDim Preserve ps3(j)
                            ps1(j) = d(i)
                            ps2(j) = d(i)
                            ps3(j) = 0
                            j += 1
                            mapchk = 0
                        End If
                    Else
                        If ps2 IsNot Nothing Then
                            'パラメーターに付随する値
                            ps2(j - 1) &= " " & d(i)
                        End If
                    End If
                End If
            Next

            'target文字列構成要素
            'ps0 = ""
            'ps1(y) = "-vf"
            'ps2(y) = "-vf a"
            'ps3(y) = 0
            '追加するパラメーター
            'p1(x) = "-map"
            'p2(x) = "-map 0,1"
            'p3(x) = 1

            If p1 IsNot Nothing Then
                'targetにパラメーターを追加
                For i = 0 To p2.Length - 1
                    If p3(i) = 1 Then
                        '重複していないのでパラメーターを追加
                        ps0 = ps0 & " " & p2(i)
                    ElseIf p3(i) = -1 Then
                        '削除指定
                        If ps1 IsNot Nothing Then
                            For j = ps1.Length - 1 To 0 Step -1
                                If ps1(j) = p1(i) Then
                                    ps1(j) = ""
                                    ps2(j) = ""
                                End If
                            Next
                        End If
                    Else
                        'すでにパラメーターが存在する
                        Select Case force
                            Case 0, 1 '入れ替えない　追加すべきパラメーターは破棄
                            Case 2
                                '入れ替え
                                'まず該当パラメータを削除
                                If ps1 IsNot Nothing Then
                                    For j = ps1.Length - 1 To 0 Step -1
                                        If ps1(j) = p1(i) Then
                                            ps1(j) = ""
                                            ps2(j) = ""
                                        End If
                                    Next
                                End If
                                'そのうえで追加
                                ps0 = ps0 & " " & p2(i)
                            Case 3
                                '追加（まず要素に追加を試みる）
                                If p2(i) > p1(i) Then
                                    Dim chk As Integer = 0
                                    For j = ps1.Length - 1 To 0 Step -1
                                        If ps1(j) = p1(i) And ps3(j) = 0 Then
                                            ps2(j) = ps2(j) & "," & p2(i).Replace(p1(i) & " ", "")
                                            ps3(j) = 1
                                            chk = 1
                                            Exit For
                                        End If
                                    Next
                                    If chk = 0 Then
                                        '追加するパラメータが無かった場合
                                        ps0 = ps0 & " " & p2(i)
                                    End If
                                Else
                                    '値が無いので追加する必要無し
                                End If
                            Case 4
                                'そのまま追加
                                ps0 = ps0 & " " & p2(i)
                            Case Else
                        End Select
                    End If
                Next

                '変化したtargetstrを結合
                Dim fstr As String = ""
                If ps2 IsNot Nothing Then
                    For i = 0 To ps2.Length - 1
                        fstr &= ps2(i) & " "
                    Next
                End If
                'targetstrのヘッダを戻す
                fstr = ps0 & " " & fstr

                '最終結合
                If ba = 1 Then
                    r = fstr & " " & s2 & " " & anotherstr
                Else
                    r = anotherstr & " " & s2 & " " & fstr
                End If
                'ヘッダを戻す
                r = s0 & r
            Else
                '追加するパラ－メーターが無い
                r = s0 & " " & hlsstr
            End If
        Else
            'ターゲット文字列が無い
            r = s0 & " " & str
        End If

        '2重空白を除去
        While r.IndexOf("  ") >= 0
            r = r.Replace("  ", " ")
        End While

        r = Trim(r)

        'エスケープしたファイル名を戻す
        If zenkakufilename IsNot Nothing Then
            For j = 0 To zenkakufilename.Length - 1
                If zenkakufilename(j).Length > 0 Then
                    r = r.Replace("%VIDEOFILENAME" & j & "%", zenkakufilename(j))
                End If
            Next
        End If

        If r.Length = 0 Then
            '失敗した場合は元のhlsOptを返す
            r = hlsstr
        End If

        Return r
    End Function

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
    Public Sub Sub_stream_Start(ByVal num As Integer, ByVal bondriver As String, ByVal sid As String, ByVal chspace As String, ByVal bon_sid_ch_str As String, ByVal resolution As String, ByVal stream_mode As Integer, ByVal videoname As String, ByVal VideoSeekSeconds As Integer, ByVal nohsub As Integer)
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
            Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, videoname, 0, stream_mode, resolution, 0, 0, "1", "", 0)
        ElseIf num > 0 And videoname.Length > 0 Then
            'ファイル再生
            Me.start_movie(num, "", 0, 0, "", Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, "", videoname, 0, stream_mode, resolution, VideoSeekSeconds, nohsub, "1", "", 0)
        End If

    End Sub

    'HTTPサーバー開始
    Public Sub Web_Start()
        If Me._wwwport = 0 Then
            log1write("【エラー】httpサーバーの起動に失敗しました。" & vbCrLf & "httpポートを指定してこのアプリを再起動してください")
            MsgBox("【エラー】httpサーバーの起動に失敗しました。" & vbCrLf & "httpポートを指定してこのアプリを再起動してください")
            Exit Sub
        End If
        If Me._udpPort = 0 Then
            log1write("【エラー】httpサーバーの起動に失敗しました。" & vbCrLf & "UDPポートを指定してこのアプリを再起動してください")
            MsgBox("【エラー】httpサーバーの起動に失敗しました。" & vbCrLf & "UDPポートを指定してこのアプリを再起動してください")
            Exit Sub
        End If

        Try
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
            '開始
            Me._listener.BeginGetContext(AddressOf Me.OnRequested, Me._listener)
        Catch ex As Exception
            log1write("【エラー】HTTPサーバの開始に失敗しました。再起動してください" & vbCrLf & ex.Message)
            MsgBox("【エラー】HTTPサーバの開始に失敗しました。再起動してください" & vbCrLf & ex.Message)
        End Try
    End Sub

    '応答
    Public Sub OnRequested(result As IAsyncResult)
        Try
            Dim listener As HttpListener = DirectCast(result.AsyncState, HttpListener)
            listener.BeginGetContext(AddressOf Me.OnRequested, listener)

            If Not listener.IsListening Then
                log1write("listening finished.")
                Return
            End If

            Dim WI_GET_HTML_output_encstr As String = "" 'WI_GET_HTMLでshift_jisが文字化けする対策

            Dim context As HttpListenerContext = listener.EndGetContext(result)
            'D:\TvRemoteViewer\html\WatchTV1.tsが見つかりませんでした
            If context.Request.RawUrl.IndexOf("/WatchTV") >= 0 And HTTPSTREAM_App = 2 Then
                Dim auth_ok As Integer = 0
                If Me._id.Length = 0 Or Me._pass.Length = 0 Then
                    'パスワード未設定は素通り
                    auth_ok = 1
                Else
                    Dim identity As HttpListenerBasicIdentity = DirectCast(context.User.Identity, HttpListenerBasicIdentity)
                    '判定
                    If Me._id = identity.Name And Me._pass = identity.Password Then
                        '受付
                        auth_ok = 2
                    Else
                        auth_ok = -1
                    End If
                End If

                If auth_ok > 0 Then
                    '★ffmpeg HTTPストリームモード
                    'numが<form>から渡されていなければURLから取得するwatch2.tsなら2
                    Dim ffmpeg_num As Integer = Val(context.Request.RawUrl.Substring(context.Request.RawUrl.IndexOf("WatchTV") + "WatchTV".Length))

                    Dim http_err As Integer = 0
                    Dim rUrl As String = context.Request.RawUrl
                    If rUrl.IndexOf("?") > 0 And rUrl.IndexOf("=") > 0 Then
                        'パラメーターがGETで渡されていればここで配信準備し直接配信する
                        '必須
                        Dim h_num As String = instr_pickup_para(rUrl, "num=", "&", 0)
                        If IsNumeric(h_num) Then
                            ffmpeg_num = h_num
                        End If
                        Dim h_resolution As String = instr_pickup_para(rUrl, "resolution=", "&", 0)
                        Dim h_bondriver As String = instr_pickup_para(rUrl, "BonDriver=", "&", 0)
                        Dim h_sid As String = Val(instr_pickup_para(rUrl, "ServiceID=", "&", 0))
                        Dim h_chspace As String = Val(instr_pickup_para(rUrl, "ChSpace=", "&", 0))
                        'ファイル名はurlエンコードされて送られてくる
                        Dim h_videoname As String = instr_pickup_para(rUrl, "VideoName=", "&", 0)
                        h_videoname = System.Web.HttpUtility.UrlDecode(h_videoname) 'urlデコード
                        h_videoname = filename_escape_set(h_videoname) '念のため
                        Dim h_VideoSeekSeconds As Integer = 0
                        Dim h_VideoSeekSeconds_str As String = instr_pickup_para(rUrl, "VideoSeekSeconds=", "&", 0)
                        h_VideoSeekSeconds_str = StrConv(h_VideoSeekSeconds_str, VbStrConv.Narrow) '半角に
                        If h_VideoSeekSeconds_str.IndexOf(":") > 0 Then
                            Dim vd() As String = h_VideoSeekSeconds_str.Split(":")
                            If vd.Length = 3 Then
                                h_VideoSeekSeconds = (vd(0) * 60 * 60) + (vd(1) * 60) + vd(2)
                            ElseIf vd.Length = 2 Then
                                h_VideoSeekSeconds = (vd(0) * 60) + vd(1)
                            Else
                                h_VideoSeekSeconds = Val(h_VideoSeekSeconds_str)
                                log1write(h_VideoSeekSeconds_str & "の秒数への変換に失敗しました" & h_VideoSeekSeconds & "秒にセットしました")
                            End If
                        Else
                            h_VideoSeekSeconds = Val(h_VideoSeekSeconds_str)
                        End If
                        'NHKの音声モード
                        Dim h_NHK_dual_mono_mode_select As Integer = Me._NHK_dual_mono_mode 'iniで指定された形式
                        Dim h_NHK_dual_mono_mode_select_str As String = instr_pickup_para(rUrl, "NHKMODE=", "&", 0)
                        If IsNumeric(h_NHK_dual_mono_mode_select_str) Then
                            'パラメーターとして指定があった場合はパラメーター優先
                            h_NHK_dual_mono_mode_select = Val(h_NHK_dual_mono_mode_select_str)
                        End If
                        Dim h_nohsub As String = Val(instr_pickup_para(rUrl, "nohsub=", "&", 0))
                        'ファイル再生時NicoJKコメント調整　録画前マージンを知らせる
                        Dim h_margin1 As Integer = Nico_delay
                        Dim h_margin1_str As String = instr_pickup_para(rUrl, "nicodelay=", "&", 0)
                        If Trim(h_margin1_str.Length) > 0 Then
                            'パラメーターとして指定があった場合はパラメーター優先
                            h_margin1 = Val(h_margin1_str)
                        End If
                        Dim h_baisoku As String = instr_pickup_para(rUrl, "VideoSpeed=", "&", 0)
                        If h_baisoku.Length = 0 Then
                            h_baisoku = "1" '等速
                        End If
                        Dim h_hlsOptAdd As String = instr_pickup_para(rUrl, "hlsOptAdd=", "&", 0)

                        Dim h_stream_mode As Integer = 2
                        If h_videoname.Length > 0 Then
                            h_stream_mode = 3
                        End If

                        If (h_stream_mode = 2 And h_bondriver.Length > 0 And Val(h_sid) > 0) Or (h_stream_mode = 3 And h_videoname.Length > 0) Then
                            '直前配信履歴簡易チェック＆記録
                            Dim st As Integer = 2
                            If h_videoname.Length >= 3 Then
                                st = 3 'ファイル配信
                            End If
                            If check_last_StBonSidCh(h_num, st, h_bondriver, h_sid, h_chspace) = 0 Then
                                'httpストリーム配信開始
                                'start_movie(ByVal num As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal ChSpace As Integer, ByVal udpApp As String, ByVal hlsApp As String, hlsOpt1 As String, ByVal hlsOpt2 As String, ByVal wwwroot As String, ByVal fileroot As String, ByVal hlsroot As String, ByVal ShowConsole As Boolean, ByVal udpOpt3 As String, ByVal filename As String, ByVal NHK_dual_mono_mode_select As Integer, ByVal Stream_mode As Integer, ByVal resolution As String, ByVal VideoSeekSeconds As Integer, ByVal nohsub As Integer, ByVal baisoku As String, ByVal hlsOptAdd As String, ByVal margin1 As Integer)
                                Me.start_movie(ffmpeg_num, h_bondriver, h_sid, h_chspace, Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, h_videoname, h_NHK_dual_mono_mode_select, h_stream_mode, h_resolution, h_VideoSeekSeconds, h_nohsub, h_baisoku, h_hlsOptAdd, h_margin1)
                            Else
                                '直前に同様の内容を配信
                                log1write("直前に同一配信指令を受け取りました")
                                'Exit Sub '無いほうがうまくいったような・・
                            End If
                        Else
                            'パラメーターが不正
                            context.Response.Headers("Content-Type") = "text/plain"
                            Dim sw = New StreamWriter(context.Response.OutputStream)
                            sw.Write("Bad Request")
                            'Test String
                            sw.Close()
                            context.Response.Close()
                            log1write("【エラー】ffmpeg HTTP ストリーム配信開始のパラメーターが不正です")
                            http_err = 1
                        End If
                    End If

                    If http_err = 0 Then
                        '配信開始
                        'Me._list(ffmpeg_num)._stoppingが100より大きければ接続待機中なので配信スタート
                        Dim ffmpeg_num_stopping As Integer = Me._procMan.get_stopping_status(ffmpeg_num)
                        If ffmpeg_num_stopping > 100 Then
                            '配信準備中
                            If ffmpeg_num > 0 Then
                                'ffmpeg HTTP ストリーム配信開始
                                log1write("ffmpeg HTTP ストリーム配信開始要求がありました")
                                'context.Response.Headers("Content-Type") = "video/mpeg"
                                context.Response.Headers("Content-Type") = "video/MP2T"
                                Me._procMan.ffmpeg_http_stream_Start(ffmpeg_num, context.Response.OutputStream)

                                '現在稼働中のlist(i)._numをログに表示
                                Dim js As String = get_live_numbers()
                                log1write("現在稼働中のNumber：" & js)
                            Else
                                '不正なURL
                                context.Response.Headers("Content-Type") = "text/plain"
                                Dim sw = New StreamWriter(context.Response.OutputStream)
                                sw.Write("Bad Request")
                                'Test String
                                sw.Close()
                                context.Response.Close()
                                log1write(context.Request.RawUrl & "は不正なリクエストです")
                            End If
                        ElseIf ffmpeg_num_stopping > 0 Then
                            '終了処理中
                            context.Response.Headers("Content-Type") = "text/plain"
                            Dim sw = New StreamWriter(context.Response.OutputStream)
                            sw.Write("Stream" & ffmpeg_num & " is stopping.")
                            sw.Close()
                            context.Response.Close()
                            log1write("ストリーム" & ffmpeg_num & "は終了処理中です。配信は中止されました")
                        Else
                            '配信準備がなされていない
                            context.Response.Headers("Content-Type") = "text/plain"
                            Dim sw = New StreamWriter(context.Response.OutputStream)
                            sw.Write("Stream" & ffmpeg_num & " is not ready.")
                            sw.Close()
                            context.Response.Close()
                            log1write("ストリーム" & ffmpeg_num & "は配信準備されていません。配信は中止されました")
                        End If
                    End If
                Else
                    '認証エラー
                    context.Response.StatusCode = 401

                    Try
                        context.Response.Close()
                    Catch ex As Exception
                        ' client closed connection before the content was sent
                    End Try
                End If
            Else

                Dim req As HttpListenerRequest = Nothing
                Dim res As HttpListenerResponse = Nothing
                Dim reader As StreamReader = Nothing
                Dim writer As StreamWriter = Nothing

                Dim ffmpeg_http_stream_on As Integer = 0

                Try
                    req = context.Request
                    res = context.Response

                    'reader = New StreamReader(req.InputStream)
                    'Dim received As String = reader.ReadToEnd()

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
                        Dim path As String = Me._wwwroot & req.Url.LocalPath.Replace("/", "\")

                        '%FILEROOT%へのアクセスならパスを変換
                        If path.IndexOf(Me._fileroot) < 0 Then
                            Dim sp As Integer = Me._fileroot.LastIndexOf("\")
                            Dim folder As String = Me._fileroot.Substring(sp) & "\"
                            sp = path.IndexOf(folder)
                            If sp >= 0 Then
                                path = Me._fileroot & "\" & path.Substring(sp + folder.Length)
                            End If
                        End If

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

                        'リクエストされたURL
                        Dim req_Url As String = req.Url.LocalPath

                        'If path.IndexOf(".htm") > 0 Or path.IndexOf(".js") > 0 Then 'Or path.IndexOf(".css") > 0 Then
                        If path.IndexOf(".htm") > 0 Then
                            'HTMLなら

                            '最後に.htmlにアクセスがあった日時を記録
                            STOP_IDLEMINUTES_LAST = Now()

                            'ページが表示されないことがあるので
                            res.ContentType = "text/html"

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
                            'ストリームモード 0=UDP 1=ファイル再生 2=http配信 3=http配信ファイル再生
                            Dim stream_mode As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("StreamMode") & "")
                            'ファイル名
                            Dim videoname As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoName") & ""
                            Dim vname() As String = videoname.Split(",")
                            If vname.Length = 1 Then
                                If vname(0).Length > 0 Then
                                    '日付が入ってない場合も受け入れる
                                    videoname = vname(0)
                                End If
                            ElseIf vname.Length >= 2 Then
                                '1番目が数値かどうか調べて対応
                                Dim sep2 As String = ""
                                Dim fl2 As Integer = 0
                                videoname = ""
                                If IsNumeric(vname(0)) Then
                                    fl2 = 1
                                End If
                                For ii2 As Integer = fl2 To vname.Length - 1
                                    videoname &= sep2 & vname(ii2)
                                    sep2 = ","
                                Next
                            End If
                            'ファイル再生シーク秒数
                            Dim VideoSeekSeconds As Integer = 0
                            Dim VideoSeekSeconds_str As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoSeekSeconds") & ""
                            VideoSeekSeconds_str = StrConv(VideoSeekSeconds_str, VbStrConv.Narrow) '半角に
                            If VideoSeekSeconds_str.IndexOf(":") > 0 Then
                                Dim vd() As String = VideoSeekSeconds_str.Split(":")
                                If vd.Length = 3 Then
                                    VideoSeekSeconds = (vd(0) * 60 * 60) + (vd(1) * 60) + vd(2)
                                ElseIf vd.Length = 2 Then
                                    VideoSeekSeconds = (vd(0) * 60) + vd(1)
                                Else
                                    VideoSeekSeconds = Val(VideoSeekSeconds_str)
                                    log1write(VideoSeekSeconds_str & "の秒数への変換に失敗しました" & VideoSeekSeconds & "秒にセットしました")
                                End If
                            Else
                                VideoSeekSeconds = Val(VideoSeekSeconds_str)
                            End If
                            '倍速ファイル再生
                            Dim baisoku As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoSpeed") & ""
                            If baisoku.Length = 0 Then
                                baisoku = "1" '等速
                            End If
                            'URLエンコードしておいたフルパスを文字列に変換
                            'UTF-8化で解決
                            'videoname = System.Web.HttpUtility.UrlDecode(videoname)
                            'ビデオファイル名抽出
                            Dim videoexword As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("VideoExWord") & ""
                            'If req.QueryString.Count > 0 Then
                            ''クエリから1つずつチェック
                            'For ii As Integer = 0 To req.QueryString.Count - 1
                            'If req.QueryString.Keys(ii) = "VideoExWord" Then
                            'videoexword = req.QueryString.Item(ii)
                            'Exit For
                            'End If
                            'Next
                            'End If
                            'ビデオリスト　更新する=1
                            Dim vl_refresh As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("vl_refresh") & "")
                            'ビデオリスト　何件返すか
                            Dim vl_volume As Integer = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("vl_volume") & "")
                            If vl_volume = 0 Then
                                vl_volume = C_INTMAX '制限無し
                            End If
                            'ビデオリスト　指定日以前のファイルをリストアップする
                            Dim vl_startdate As DateTime
                            Dim vl_startdate_str As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("vl_startdate") & ""
                            If vl_startdate_str.Length = 0 Then
                                '未指定
                                vl_startdate = C_DAY2038 '制限無し
                            Else
                                Try
                                    vl_startdate = CDate(vl_startdate_str)
                                Catch ex As Exception
                                    '不正な値
                                    vl_volume = -99
                                    vl_startdate = CDate("1980/01/01")
                                End Try
                            End If

                            'ハードサブ不許可
                            Dim nohsub As String = Val(System.Web.HttpUtility.ParseQueryString(req.Url.Query)("nohsub") & "")
                            'ファイル再生時NicoJKコメント調整　録画前マージンを知らせる
                            Dim margin1 As Integer = Nico_delay
                            Dim margin1_str As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("nicodelay") & ""
                            If Trim(margin1_str.Length) > 0 Then
                                'パラメーターとして指定があった場合はパラメーター優先
                                margin1 = Val(margin1_str)
                            End If

                            'ファイル書き込みコマンド
                            Dim fl_cmd As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("fl_cmd") & ""
                            Dim fl_file As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("fl_file") & ""
                            Dim fl_text As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("fl_text") & ""

                            'NHKの音声モード
                            Dim NHK_dual_mono_mode_select As Integer = Me._NHK_dual_mono_mode 'iniで指定された形式
                            Dim NHK_dual_mono_mode_select_str As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("NHKMODE") & ""
                            If IsNumeric(NHK_dual_mono_mode_select_str) Then
                                'パラメーターとして指定があった場合はパラメーター優先
                                NHK_dual_mono_mode_select = Val(NHK_dual_mono_mode_select_str)
                            End If

                            'hlsOptに追加するべき文字列
                            Dim hlsOptAdd As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("hlsOptAdd") & ""

                            '汎用文字列
                            Dim temp As String = System.Web.HttpUtility.ParseQueryString(req.Url.Query)("temp") & ""

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
                                        'Sub_stream_Start(num, bondriver, sid, chspace, bon_sid_ch_str, resolution, stream_mode, videoname, nohsub)
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
                                    Case "WI_GET_PROGRAM_TVMAID"
                                        'Tvmaid番組表取得
                                        WI_cmd_reply = Me.WI_GET_PROGRAM_TVMAID(Val(temp))
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_PROGRAM_PTTIMER"
                                        'EDCB番組表取得
                                        WI_cmd_reply = Me.WI_GET_PROGRAM_PTTIMER(Val(temp))
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_PROGRAM_EDCB"
                                        'EDCB番組表取得
                                        WI_cmd_reply = Me.WI_GET_PROGRAM_EDCB(Val(temp))
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_PROGRAM_TVROCK"
                                        'TVROCK番組表取得
                                        WI_cmd_reply = Me.WI_GET_PROGRAM_TVROCK(Val(temp))
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_PROGRAM_NUM"
                                        '放送中の番組
                                        WI_cmd_reply = Me.WI_GET_PROGRAM_NUM(num)
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_LIVE_STREAM"
                                        '現在配信中のストリーム
                                        '_listNo.,num, udpPort, BonDriver, ServiceID, ch_space, stream_mode, NHKMODE
                                        'stopping, チャンネル名, hlsApp, シーク秒, URL
                                        WI_cmd_reply = Me.WI_GET_LIVE_STREAM()
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_TVRV_STATUS"
                                        'サーバー設定
                                        WI_cmd_reply = Me.WI_GET_TVRV_STATUS()
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_TSFILE_COUNT"
                                        '作られている.tsの数
                                        WI_cmd_reply = Me.WI_GET_TSFILE_COUNT(num)
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_TSFILE_COUNT2"
                                        '作られている.tsの数 （m3u8が存在すれば正の値　m3u8が存在しなければ負の値）
                                        WI_cmd_reply = Me.check_m3u8_ts_status(num)
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_RESOLUTION"
                                        '解像度
                                        WI_cmd_reply = Me.WI_GET_RESOLUTION()
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_VIDEOFILES"
                                        'ビデオファイル
                                        If videolist_firstview = 0 Then
                                            '閲覧最初の1回目は強制リフレッシュ
                                            videolist_firstview = 1
                                            vl_refresh = 1
                                        End If
                                        WI_cmd_reply = Me.WI_GET_VIDEOFILES(videoexword, vl_refresh, vl_startdate, vl_volume)
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_VIDEOFILES2"
                                        'ビデオファイル
                                        If vl_volume = -99 Then
                                            '不正な日付だったときにはvl_volume=-99になっている
                                            WI_cmd_reply = "99,," & vbCrLf
                                        Else
                                            If videolist_firstview = 0 Then
                                                '閲覧最初の1回目は強制リフレッシュ
                                                videolist_firstview = 1
                                                vl_refresh = 1
                                            End If
                                            WI_cmd_reply = Me.WI_GET_VIDEOFILES2(videoexword, vl_refresh, vl_startdate, vl_volume)
                                        End If
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_ERROR_STREAM"
                                        '再起動中のストリームを返す
                                        WI_cmd_reply = Me.WI_GET_ERROR_STREAM()
                                        WI_cmd_reply_force = 1
                                    Case "WI_SET_HTTPSTREAM_App"
                                        'http配信アプリを切り替える　手抜き・・numを一時代用
                                        'tempで指定されてもokにした
                                        Dim n As Integer = num
                                        If Val(temp) > 0 Then
                                            n = Val(temp)
                                        End If
                                        WI_cmd_reply = Me.WI_SET_HTTPSTREAM_App(n)
                                        WI_cmd_reply_force = 1
                                    Case "WI_FILE_OPE"
                                        'ファイル書き込み dirのフィルタをtempで追加
                                        WI_cmd_reply = Me.WI_FILE_OPE(fl_cmd, fl_file, fl_text, temp)
                                        WI_cmd_reply_force = 1
                                    Case "WI_STREAMFILE_EXIST"
                                        'ストリームフォルダにファイルが存在するかどうか
                                        WI_cmd_reply = Me.WI_STREAMFILE_EXIST(fl_file)
                                        WI_cmd_reply_force = 1
                                    Case "WI_SHOW_LOG"
                                        'ログ出力
                                        WI_cmd_reply = Me.WI_SHOW_LOG()
                                        WI_cmd_reply_force = 1
                                    Case "WI_GET_CHAPTER"
                                        '録画ファイルのチャプター取得
                                        If temp.Length > 0 Then
                                            WI_cmd_reply = Me.WI_GET_CHAPTER(temp)
                                            WI_cmd_reply_force = 1
                                        End If
                                    Case "WI_WRITE_CHAPTER"
                                        'チャプターファイルへ書き込み
                                        If temp.Length > 0 Then
                                            WI_cmd_reply = Me.WI_WRITE_CHAPTER(temp)
                                            WI_cmd_reply_force = 1
                                        End If
                                    Case "WI_GET_HTML"
                                        'HTML取得
                                        If temp.Length > 0 Then
                                            WI_cmd_reply = Me.WI_GET_HTML(temp, WI_GET_HTML_output_encstr)
                                            WI_cmd_reply_force = 1
                                        End If
                                    Case "WI_GET_THUMBNAIL"
                                        'サムネイル作成
                                        If temp.Length > 0 Then
                                            Dim d() As String = temp.Split(",")
                                            If d.Length > 4 Then
                                                'ファイル名に,が混じっている可能性有り
                                                Dim fname1 As String = ""
                                                For ii2 = 1 To d.Length - 4
                                                    d(0) &= "," & d(ii2)
                                                Next
                                                For ii2 = d.Length - 4 To d.Length - 2
                                                    d(ii2) = d(ii2 + 1)
                                                Next
                                            End If
                                            If d.Length >= 4 Then
                                                WI_cmd_reply = Me.WI_GET_THUMBNAIL(d(0), d(1), Val(d(2)), Val(d(3)))
                                                WI_cmd_reply_force = 1
                                            End If
                                        End If
                                    Case "WI_SHOW_MAKING_PER_THUMB"
                                        If making_per_thumbnail IsNot Nothing Then
                                            For ii2 = 0 To making_per_thumbnail.Length - 1
                                                If making_per_thumbnail(ii2).indexofstr.Length > 0 Then
                                                    WI_cmd_reply &= making_per_thumbnail(ii2).fullpathfilename & vbCrLf
                                                End If
                                            Next
                                        End If
                                        WI_cmd_reply_force = 1
                                    Case "WI_WRITE_LOG"
                                        'ログに出力
                                        If Trim(temp).Length > 0 Then
                                            log1write(temp)
                                            WI_cmd_reply = "OK"
                                            WI_cmd_reply_force = 1
                                        End If
                                End Select
                            End If

                            '===========================================
                            'WEBページ表示前処理
                            '===========================================
                            '直前配信履歴簡易チェック＆記録
                            If (stream_mode = 0 Or stream_mode = 2) And req_Url.ToLower = ("/StartTv.html").ToLower And bondriver.Length > 0 Then
                                Dim rn As Integer = 0
                                If check_last_StBonSidCh(num, stream_mode, bondriver, Val(sid), Val(chspace)) > 0 Then
                                    '直前に同様の内容を配信
                                    log1write("直前に同一配信命令を受け取りました")
                                    rn = 1
                                Else
                                    'すでに同じnum,BonDriver,サービスID,chspaceならば再度配信スタートはしない
                                    Dim num2 As Integer = CHKECK_num_Bon_sid_ch(bondriver, Val(sid), Val(chspace))
                                    If num2 >= 0 Then
                                        'すでに同じ放送を放送中
                                        log1write("すでにストリーム" & num2.ToString & "で同一放送を配信中です")
                                        num = num2
                                        rn = 1
                                    End If
                                End If

                                If rn = 1 Then
                                    '配信中のストリームへ誘導
                                    req_Url = "/ViewTV" & num & ".html"
                                    path = IO.Path.GetDirectoryName(path) & "\ViewTV" & num & ".html"
                                End If
                            End If

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
                                    'BonDriverが使用中ならばそのnumに変更する
                                    If Allow_BonDriver4Streams = 0 Then
                                        num = GET_num_check_BonDriver(num, bondriver)
                                    End If
                                    '正しければ配信スタート
                                    Me.start_movie(num, bondriver, Val(sid), Val(chspace), Me._udpApp, Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, Me._udpOpt3, videoname, NHK_dual_mono_mode_select, stream_mode, resolution, 0, 0, "1", hlsOptAdd, 0)
                                    'すぐさま視聴ページへリダイレクトする
                                    redirect = "ViewTV" & num & ".html"
                                ElseIf num > 0 And videoname.Length > 0 Then
                                    'ファイル再生
                                    If Me._hlsApp.IndexOf("ffmpeg") > 0 Then
                                        'ffmpegなら
                                        Me.start_movie(num, "", 0, 0, "", Me._hlsApp, Me._hlsOpt1, Me._hlsOpt2, Me._wwwroot, Me._fileroot, Me._hlsroot, Me._ShowConsole, "", videoname, NHK_dual_mono_mode_select, stream_mode, resolution, VideoSeekSeconds, nohsub, baisoku, hlsOptAdd, margin1)
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
                                stop_movie(num)
                            End If

                            '===========================================
                            'WEBページ表示
                            '===========================================
                            'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                            Dim swdata As String = ""

                            If StartTv_param = -1 Then
                                '/StartTvにリクエストがあったがパラメーターが不正な場合
                                'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                                'sw.WriteLine(ERROR_PAGE("パラメーターが不正です", "パラメーターが不正です"))
                                swdata = ERROR_PAGE("パラメーターが不正です", "パラメーターが不正です")
                                'sw.Flush()
                                log1write("パラメーターが不正です")
                            ElseIf WI_cmd_reply.Length > 0 Or WI_cmd_reply_force = 1 Then
                                'ＷＥＢインターフェース　コマンドの返事を返す
                                'sw.WriteLine(WI_cmd_reply)
                                swdata = WI_cmd_reply
                            ElseIf request_page = 1 Or request_page = 11 Then
                                'waitingページを表示する
                                Dim s As String = ""
                                Dim path_waiting As String = path.Replace("ViewTV" & num.ToString & ".html", "Waiting.html")
                                If file_exist(path_waiting) = 1 Then
                                    s = ReadAllTexts(path_waiting)
                                    Dim waitmessage As String = ""
                                    If request_page = 1 Then
                                        waitmessage = "配信準備中です..(" & System.Math.Abs(check_m3u8_ts).ToString & ")"
                                    ElseIf request_page = 11 Then
                                        waitmessage = "配信されていません"
                                    End If
                                    s = s.Replace("%WAITING%", waitmessage)
                                    s = s.Replace("%NUM%", num.ToString)
                                    If meta_refresh_fix = 1 Then
                                        '<head>にrefreshスクリプトを追加
                                        Dim rsec As Integer = Val(Trim(Instr_pickup(s, "refresh"" content=""", ";", 0)))
                                        If rsec = 0 Then
                                            rsec = 1
                                        End If
                                        Dim jstr As String = "<script type=""text/javascript"">" & vbCrLf
                                        jstr &= "function AutoJump(){location.href=""ViewTV" & num.ToString & ".html"";}" & vbCrLf
                                        jstr &= "</script>" & vbCrLf
                                        jstr &= "</head"
                                        s = s.Replace("</head", jstr)
                                        Dim sp As Integer = s.IndexOf("<meta http-equiv=""refresh""")
                                        Dim sp2 As Integer = s.IndexOf(">", sp)
                                        s = s.Substring(0, sp) & "<!--refresh fix--" & s.Substring(sp2)
                                        s = s.Replace("<body", "<body onLoad=""javascript:setTimeout('AutoJump()'," & (rsec * 1000).ToString & ");"" ")
                                    End If
                                Else
                                    '従来通り
                                    'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE & vbcrlf)
                                    s &= "<!doctype html>" & vbCrLf
                                    s &= "<html>" & vbCrLf
                                    s &= "<head>" & vbCrLf
                                    s &= "<title>Waiting " & num.ToString & "</title>" & vbCrLf
                                    s &= "<meta http-equiv=""Content-Type"" content=""text/html; charset=" & HTML_OUT_CHARACTER_CODE & """ />" & vbCrLf
                                    's &= "<meta name=""viewport"" content=""width=device-width"">" & vbcrlf
                                    If meta_refresh_fix = 1 Then
                                        s &= "<script type=""text/javascript"">" & vbCrLf
                                        s &= "function AutoJump(){location.href=""ViewTV" & num.ToString & ".html"";}" & vbCrLf
                                        s &= "</script>" & vbCrLf
                                        s &= "</head>" & vbCrLf
                                        s &= "<body onLoad=""javascript:setTimeout('AutoJump()',1000);"">" & vbCrLf
                                    Else
                                        s &= "<meta http-equiv=""refresh"" content=""1 ; URL=ViewTV" & num.ToString & ".html"">" & vbCrLf
                                        s &= "</head>" & vbCrLf
                                        s &= "<body>" & vbCrLf
                                    End If
                                    If request_page = 1 Then
                                        s &= "配信準備中です..(" & System.Math.Abs(check_m3u8_ts).ToString & " & vbcrlf" & vbCrLf
                                        log1write(num.ToString & ":配信準備中です")
                                    ElseIf request_page = 11 Then
                                        s &= "配信されていません" & vbCrLf
                                        log1write(num.ToString & ":配信されていません")
                                    End If
                                    s &= "<br><br>" & vbCrLf
                                    s &= "<input type=""button"" class=""c_btn_topmenu"" value=""トップメニュー"" onClick=""location.href='/index.html'"">" & vbCrLf
                                    s &= "<br><br>" & vbCrLf
                                    s &= "<input type=""button"" class=""c_btn_back"" value=""直前のページへ戻る"" onClick=""history.go(-1);"">" & vbCrLf
                                    's &= "<input type=""button"" value=""地デジ番組表"" onClick=""location.href='TvProgram.html'"">" & vbcrlf
                                    s &= "</body>" & vbCrLf
                                    s &= "</html>" & vbCrLf

                                    'sw.Flush()
                                End If

                                'sw.WriteLine(s)
                                swdata = s

                            ElseIf request_page = 19 Then
                                Dim html19 As String = ""
                                html19 &= "HTTPストリーミングで配信中です<br>"
                                html19 &= "ブラウザでの再生はできません<br>"
                                'ここで.tsへのリンクを貼るべきか・・
                                ''numが<form>から渡されていなければURLから取得するViewTV2.htmlなら2
                                'If num = 0 Then
                                'Dim num_url As String = Val(req_Url.ToLower.Substring(req_Url.ToLower.IndexOf("ViewTV".ToLower) + "ViewTV".Length))
                                'If num_url > 0 Then
                                'num = num_url
                                'End If
                                'End If
                                'html19 &= "<a href=""WatchTV" & num & ".ts"">WatchTV" & num & ".ts</a>" & vbCrLf
                                'sw.WriteLine(ERROR_PAGE("HTTPストリーミング", html19))
                                swdata = ERROR_PAGE("HTTPストリーミング", html19)
                            ElseIf request_page = 12 Then
                                'VLCはファイル再生未対応
                                'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                                'sw.WriteLine(ERROR_PAGE("ファイル再生失敗", "VLCでのファイル再生には対応していません"))
                                swdata = ERROR_PAGE("ファイル再生失敗", "VLCでのファイル再生には対応していません")
                                'sw.Flush()
                                log1write(num.ToString & ":配信されていません")
                            Else
                                Dim s As String = ""
                                If File.Exists(path) Then
                                    s = ReadAllTexts(path)
                                ElseIf path.IndexOf("ViewTV" & num.ToString & ".html") >= 0 Then
                                    s = ReadAllTexts(path.Replace("ViewTV" & num.ToString & ".html", "ViewTV1.html"))
                                Else
                                    '存在しない
                                End If

                                If s.Length > 0 Then
                                    'ElseIf request_page >= 2 Then
                                    'パラメーターを置換する必要があるページ

                                    'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))

                                    '★表示状態により変換内容変更が無いパラメーター
                                    'これらは一括変換できる

                                    'ストリーム番号
                                    s = s.Replace("%NUM%", num.ToString)

                                    'ストリーム番号セレクト
                                    If s.IndexOf("%SELECTNUM%") >= 0 Then
                                        Dim selectnum_html As String = ""
                                        selectnum_html &= "<select class=""c_sel_num"" name=""num"">" & vbCrLf
                                        For ix = 1 To MAX_STREAM_NUMBER
                                            selectnum_html &= "<option>" & ix.ToString & "</option>" & vbCrLf
                                        Next
                                        selectnum_html &= "</select>" & vbCrLf
                                        s = s.Replace("%SELECTNUM%", selectnum_html)
                                    End If

                                    'ユーザー名とパスワード変換
                                    If ALLOW_IDPASS2HTML = 1 And Me._id.Length > 0 And Me._pass.Length > 0 Then
                                        s = s.Replace("%IDPASS%", Me._id & ":" & Me._pass & "@")
                                    Else
                                        s = s.Replace("%IDPASS%", "")
                                    End If

                                    '%FILEROOT%変換
                                    If s.IndexOf("%FILEROOT%") >= 0 Then
                                        Dim fileroot As String = get_soutaiaddress_from_fileroot()
                                        s = s.Replace("%FILEROOT%", fileroot)
                                    End If

                                    'リダイレクト
                                    If redirect.Length > 3 Then
                                        'リダイレクト指定があれば
                                        If meta_refresh_fix = 1 Then
                                            Dim rsec As Integer = Val(Trim(Instr_pickup(s, "refresh"" content=""", ";", 0)))
                                            Dim jstr As String = "<script type=""text/javascript"">" & vbCrLf
                                            jstr &= "function AutoJump(){location.href=""" & redirect & """;}" & vbCrLf
                                            jstr &= "</script>"
                                            s = s.Replace("%REDIRECT%", jstr)
                                            s = s.Replace("<body", "<body onLoad=""javascript:setTimeout('AutoJump()'," & (rsec * 1000).ToString & ");"" ")
                                        Else
                                            s = s.Replace("%REDIRECT%", "<meta http-equiv=""refresh"" content=""0 ; URL=" & redirect & """>")
                                        End If
                                    Else
                                        '無ければリダイレクト変数を消す
                                        s = s.Replace("%REDIRECT%", "")
                                    End If

                                    'ファイル選択ページ用
                                    If s.IndexOf("%SELECTVIDEO%") >= 0 Then
                                        If videolist_firstview = 0 Then
                                            '閲覧最初の1回目は強制リフレッシュ
                                            videolist_firstview = 1
                                            vl_refresh = 1
                                        End If
                                        Dim shtml As String = make_file_select_html(videoexword, vl_refresh, vl_startdate, vl_volume)
                                        s = s.Replace("%SELECTVIDEO%", shtml)
                                    End If
                                    '%VIDEOFROMDATE%
                                    s = s.Replace("%VIDEOFROMDATE%", VIDEOFROMDATE.ToString("yyyy/MM/dd"))

                                    '地デジ番組表（通常のネットから取得）
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        s = s.Replace("%TVPROGRAM-D%", make_TVprogram_html_now(0, Me._NHK_dual_mono_mode))
                                    Else
                                        s = s.Replace("%TVPROGRAM-D%", make_TVprogram_html_now(0, -1))
                                    End If

                                    'TvRock番組表
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        s = s.Replace("%TVPROGRAM-TVROCK%", make_TVprogram_html_now(999, Me._NHK_dual_mono_mode))
                                    Else
                                        s = s.Replace("%TVPROGRAM-TVROCK%", make_TVprogram_html_now(999, -1))
                                    End If

                                    'EDCB番組表
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        s = s.Replace("%TVPROGRAM-EDCB%", make_TVprogram_html_now(998, Me._NHK_dual_mono_mode))
                                    Else
                                        s = s.Replace("%TVPROGRAM-EDCB%", make_TVprogram_html_now(998, -1))
                                    End If

                                    'ptTimer番組表
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        s = s.Replace("%TVPROGRAM-PTTIMER%", make_TVprogram_html_now(997, Me._NHK_dual_mono_mode))
                                    Else
                                        s = s.Replace("%TVPROGRAM-PTTIMER%", make_TVprogram_html_now(997, -1))
                                    End If

                                    'Tvmaid番組表
                                    If Me._hlsApp.IndexOf("ffmpeg") >= 0 Then
                                        s = s.Replace("%TVPROGRAM-TVMAID%", make_TVprogram_html_now(996, Me._NHK_dual_mono_mode))
                                    Else
                                        s = s.Replace("%TVPROGRAM-TVMAID%", make_TVprogram_html_now(996, -1))
                                    End If

                                    'ニコニコ実況用jkチャンネル変換
                                    If s.IndexOf("%JKNUM%") >= 0 Then
                                        'numからsidとchspaceを取得する
                                        Dim jkd() As Integer = Me._procMan.get_jk_para(num)
                                        If jkd(0) > 0 Then
                                            Dim jkstr As String = sid2jk(jkd(0), jkd(1))
                                            If jkstr.Length > 0 Then
                                                s = s.Replace("%JKNUM%", jkstr)
                                            Else
                                                s = s.Replace("%JKNUM%", "")
                                            End If
                                        Else
                                            s = s.Replace("%JKNUM%", "")
                                        End If
                                    End If

                                    'ニコニコ実況用　接続用stringをニコニコ実況から取得
                                    If s.IndexOf("%JKVALUE%") >= 0 Then
                                        'numからsidとchspaceを取得する
                                        Dim jkd() As Integer = Me._procMan.get_jk_para(num)
                                        If jkd(0) > 0 Then
                                            Dim jknum As String = sid2jk(jkd(0), jkd(1))
                                            If jknum.Length > 0 Then
                                                'jk～からニコニコ実況接続用stringを取得
                                                Dim jkvalue As String = get_nico_jkvalue(jknum)
                                                s = s.Replace("%JKVALUE%", jkvalue)
                                            Else
                                                s = s.Replace("%JKVALUE%", "")
                                            End If
                                        Else
                                            s = s.Replace("%JKVALUE%", "")
                                        End If
                                    End If

                                    'ファイル再生開始シーク秒
                                    s = s.Replace("%VIDEOSEEKSECONDS%", VideoSeekDefault)

                                    '★表示状態により変換内容変更されるパラメーター

                                    'NHK音声モード
                                    While s.IndexOf("%SELECTNHKMODE") >= 0
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
                                    End While

                                    'BonDriverと番組名連携選択
                                    While s.IndexOf("%SELECTBONSIDCH") >= 0
                                        Dim gt() As String = get_atags("%SELECTBONSIDCH", s)
                                        Dim selectbon As String = WEB_make_select_Bondriver_html(gt)
                                        If selectbon.Length > 0 Then
                                            s = s.Replace("%SELECTBONSIDCH" & gt(0) & "%", selectbon)
                                        Else
                                            s = s.Replace("%SELECTBONSIDCH" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    '解像度セレクトボックス '%SELECTBONSIDCHより後でないといけない
                                    If s.IndexOf("%SELECTRESOLUTION%") >= 0 Then
                                        Dim selectresolution As String = Nothing
                                        If req_Url.ToLower.IndexOf("/SelectVideo".ToLower) >= 0 Then
                                            If ffmpeg_file_option Is Nothing Then
                                                selectresolution = WEB_make_select_resolution()
                                            Else
                                                selectresolution = WEB_make_select_resolution("", 1)
                                            End If
                                        Else
                                            selectresolution = WEB_make_select_resolution()
                                        End If
                                        s = s.Replace("%SELECTRESOLUTION%", selectresolution)
                                    End If

                                    '動画の長さ
                                    If TOT_get_duration > 0 And num > 0 Then
                                        'ストリームからファイル名を取得
                                        Dim duration As Integer = F_get_file_duration(num)
                                        s = s.Replace("%VIDEODURATION%", duration)
                                    Else
                                        s = s.Replace("%VIDEODURATION%", "0")
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
                                        While s.IndexOf("%SELECTCH") >= 0
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
                                        End While

                                        'Nico2HLSが必要無くなったのでコメントアウト
                                        'Dim vttfilename As String = Me._fileroot & "\mystream_s" & num.ToString & ".m3u8"
                                        'If Me._procMan.get_stream_mode(num) = 0 Then
                                        ''ストリームモードが0ならば
                                        'If file_exist(vttfilename) = 1 Then
                                        's = s.Replace("%SUBSTR%", "_s")
                                        'Else
                                        's = s.Replace("%SUBSTR%", "")
                                        'End If
                                        'Else
                                        's = s.Replace("%SUBSTR%", "")
                                        'End If
                                        s = s.Replace("%SUBSTR%", "")
                                    End If

                                    '配信中簡易リスト
                                    While s.IndexOf("%PROCBONLIST") >= 0
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
                                    End While

                                    'Viewボタン作成
                                    While s.IndexOf("%VIEWBUTTONS") >= 0
                                        Dim gt() As String = get_atags("%VIEWBUTTONS", s)
                                        Dim viewbutton_html As String = WEB_make_ViewLink_html(gt)
                                        If viewbutton_html.Length > 0 Then
                                            s = s.Replace("%VIEWBUTTONS" & gt(0) & "%", viewbutton_html)
                                        Else
                                            s = s.Replace("%VIEWBUTTONS" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    'TvRock番組表ボタン
                                    While s.IndexOf("%TVPROGRAM-TVROCK-BUTTON") >= 0
                                        Dim gt() As String = get_atags("%TVPROGRAM-TVROCK-BUTTON", s)
                                        If TvProgram_tvrock_url.Length > 0 Then
                                            s = s.Replace("%TVPROGRAM-TVROCK-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" class=""c_btn_ptvrock"" value=""TvRock番組表"" onClick=""location.href='TvProgram_TvRock.html'"">") & gt(3)
                                        Else
                                            s = s.Replace("%TVPROGRAM-TVROCK-BUTTON" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    'EDCB番組表ボタン
                                    While s.IndexOf("%TVPROGRAM-EDCB-BUTTON") >= 0
                                        Dim gt() As String = get_atags("%TVPROGRAM-EDCB-BUTTON", s)
                                        If TvProgram_EDCB_url.Length > 0 Then
                                            s = s.Replace("%TVPROGRAM-EDCB-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" class=""c_btn_pedcb"" value=""EDCB番組表"" onClick=""location.href='TvProgram_EDCB.html'"">") & gt(3)
                                        Else
                                            s = s.Replace("%TVPROGRAM-EDCB-BUTTON" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    'ptTimer番組表ボタン
                                    While s.IndexOf("%TVPROGRAM-PTTIMER-BUTTON") >= 0
                                        Dim gt() As String = get_atags("%TVPROGRAM-PTTIMER-BUTTON", s)
                                        If ptTimer_path.Length > 0 Then
                                            s = s.Replace("%TVPROGRAM-PTTIMER-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" class=""c_btn_ppttimer"" value=""ptTimer番組表"" onClick=""location.href='TvProgram_ptTimer.html'"">") & gt(3)
                                        Else
                                            s = s.Replace("%TVPROGRAM-PTTIMER-BUTTON" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    'Tvmaid番組表ボタン
                                    While s.IndexOf("%TVPROGRAM-TVMAID-BUTTON") >= 0
                                        Dim gt() As String = get_atags("%TVPROGRAM-TVMAID-BUTTON", s)
                                        If Tvmaid_url.Length > 0 Then
                                            s = s.Replace("%TVPROGRAM-TVMAID-BUTTON" & gt(0) & "%", gt(1) & "<input type=""button"" class=""c_btn_ptvmaid"" value=""Tvmaid番組表"" onClick=""location.href='TvProgram_Tvmaid.html'"">") & gt(3)
                                        Else
                                            s = s.Replace("%TVPROGRAM-TVMAID-BUTTON" & gt(0) & "%", gt(4))
                                        End If
                                    End While

                                    'sw.WriteLine(s)
                                    swdata = s
                                    'sw.Flush()

                                    'log1write(path & "へのアクセスを受け付けました")
                                Else
                                    'ローカルファイルが存在していない
                                    'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                                    'sw.WriteLine(ERROR_PAGE("bad request", "ページが見つかりません"))
                                    swdata = ERROR_PAGE("bad request", "ページが見つかりません")
                                    'sw.Flush()
                                    log1write(path & "が見つかりませんでした")
                                End If
                            End If

                            If html_publish_method = 1 Then
                                Try
                                    Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                                    sw.WriteLine(swdata) 'ここでエラーになることがある
                                    sw.Flush() 'ここでエラーになることがある
                                    sw.Close()
                                Catch ex As Exception
                                    log1write("【エラー】" & "HTML出力に失敗しました(StreamWriter)" & req_Url & " " & ex.Message)
                                End Try
                            Else
                                Dim content As Byte() = Nothing
                                '出力エンコード
                                Select Case WI_GET_HTML_output_encstr.ToLower
                                    Case "shift_jis", "sjis"
                                        'charset=Shift_JISのままだと文字化けする
                                        swdata = swdata.Replace("charset=Shift_JIS", "charset=UTF-8")
                                End Select
                                'UTF-8
                                content = System.Text.Encoding.UTF8.GetBytes(swdata)

                                Try
                                    res.OutputStream.Write(content, 0, content.Length)
                                Catch ex As Exception
                                    log1write("【エラー】" & "HTML出力に失敗しました。" & req_Url & " " & ex.Message)
                                    'エラーが起こったら1回だけリトライするか・・
                                    System.Threading.Thread.Sleep(50)
                                    Try
                                        res.OutputStream.Write(content, 0, content.Length)
                                        log1write("【リトライ】HTML再出力に成功しました")
                                    Catch ex2 As Exception
                                        log1write("【エラー】" & "HTML再出力に失敗しました。" & ex2.Message)
                                    End Try
                                End Try
                            End If
                        Else
                            'HTML以外なら
                            If File.Exists(path) Then
                                ' ローカルファイルが存在すればレスポンス・ストリームに書き出す
                                'm3u8、tsへの要求はこちらへ来る
                                Dim content As Byte() = ReadAllBytes(path)
                                Try
                                    res.OutputStream.Write(content, 0, content.Length)
                                    'log1write(path & "へのアクセスを受け付けました")
                                Catch ex As Exception
                                    log1write("【エラー】" & path & "のバイトデータ送信に失敗しました。" & ex.Message)
                                    'エラーが起こったら1回だけリトライするか・・
                                    System.Threading.Thread.Sleep(50)
                                    Try
                                        res.OutputStream.Write(content, 0, content.Length)
                                        log1write("【リトライ】" & path & "のバイトデータ再送信に成功しました")
                                    Catch ex2 As Exception
                                        log1write("【エラー】" & path & "のバイトデータ再送信に失敗しました。" & ex2.Message)
                                    End Try
                                End Try
                            Else
                                'ローカルファイルが存在していない
                                'Dim sw As New StreamWriter(res.OutputStream, System.Text.Encoding.GetEncoding(HTML_OUT_CHARACTER_CODE))
                                'sw.WriteLine(ERROR_PAGE("bad request", "ページが見つかりません"))
                                'sw.Flush()
                                Dim content As Byte() = System.Text.Encoding.UTF8.GetBytes(ERROR_PAGE("bad request", "ページが見つかりません"))
                                Try
                                    res.OutputStream.Write(content, 0, content.Length)
                                Catch ex As Exception
                                    log1write("【エラー】" & "HTML出力に失敗しました[2]。" & ex.Message)
                                End Try
                                log1write(path & "が見つかりませんでした")
                            End If
                        End If
                    Else
                        '認証エラー
                        context.Response.StatusCode = 401
                    End If

                    Try
                        res.Close()
                    Catch ex As Exception
                        'たまにエラーになる・・が無害のよう
                    End Try

                    Try
                        context.Response.Close()
                    Catch ex As Exception
                        ' client closed connection before the content was sent
                    End Try

                Catch httpEx As HttpListenerException
                    log1write(httpEx.Message)
                    log1write(httpEx.StackTrace)
                Finally
                    Try
                        If writer IsNot Nothing Then
                            writer.Close()
                        End If
                        If reader IsNot Nothing Then
                            reader.Close()
                        End If
                        If res IsNot Nothing Then
                            res.Close()
                        End If
                        If context IsNot Nothing Then
                            context.Response.Close()
                        End If
                    Catch ex As Exception
                        log1write(ex.ToString())
                    End Try
                End Try
            End If

        Catch ex As Exception
            log1write(ex.ToString())
        End Try
    End Sub

    'ファイル再生中の動画の長さ（秒）を取得
    Public Function F_get_file_duration(ByVal num As Integer) As Integer
        '　本体はProcessManager.vbに
        Return Me._procMan.F_get_file_duration(num)
    End Function

    '直前配信履歴簡易チェック＆記録　直前に同じ配信指令を受け取っていれば1を返す　同時に配信内容を簡易記録
    Public Function check_last_StBonSidCh(ByVal num As Integer, ByVal st As Integer, ByVal bondriver As String, ByVal sid As Integer, ByVal chspace As Integer) As Integer
        Dim r As Integer = 0

        Dim ut As Integer = time2unix(Now())

        Dim cs As String = st.ToString & "," & bondriver & "," & sid.ToString & "," & chspace.ToString
        If st = 0 Or st = 2 Then
            '最後に配信開始してから何秒経っているか
            Dim utb As Integer = ut - stream_last_StBonSidCh(num).utime
            '放送配信
            If (stream_last_StBonSidCh(num).str = cs And utb <= 60) Or utb <= 3 Then
                '同一の配信指令で60秒以内(うまく配信がスタートしていれば早めにクリアされている)、もしくは前回の指令から3秒以内なら重複と見なす
                r = 1
            Else
                '新規
                stream_last_StBonSidCh(num).utime = ut
                stream_last_StBonSidCh(num).str = cs
            End If
        Else
            'ファイル配信
            stream_last_StBonSidCh(num).utime = 0
            stream_last_StBonSidCh(num).str = ""
        End If

        Return r
    End Function

    'すでに同じ放送が放送中かどうか(該当無しならば-1、該当があればnumを返す）
    Public Function CHKECK_num_Bon_sid_ch(ByVal bondriver As String, ByVal sid As Integer, ByVal chspace As Integer) As Integer
        Dim r As Integer = -1

        If sid > 0 And bondriver.Length > 0 Then
            Dim str As String = WI_GET_LIVE_STREAM()
            If str.Length > 10 Then
                Dim line() As String = Split(str, vbCrLf)
                For i As Integer = 0 To line.Length - 1
                    Dim d() As String = line(i).Split(",")
                    If d.Length >= 6 Then
                        If Trim(bondriver.ToLower) = Trim(d(3).ToLower) And sid = Val(Trim(d(4))) And chspace = Val(Trim(d(5))) Then
                            r = Val(Trim(d(1))) '配信中のnum
                            Exit For
                        End If
                    End If
                Next
            End If
        End If

        Return r
    End Function

    'すでに配信中のストリームでBonDriverが使われていれば該当numを返す。使われていなければそのままのnumを返す
    Public Function GET_num_check_BonDriver(ByVal num As Integer, ByVal bondriver As String) As Integer
        Dim r As String = Me._procMan.get_live_numbers_bon()
        Dim num_org As Integer = num
        If r.Length > 0 Then
            Dim list() As String = Split(r, vbCrLf)
            Dim i As Integer
            For i = 0 To list.Length - 1
                Dim d() As String = list(i).Split(":")
                If d.Length = 2 Then
                    If Trim(d(1)).ToLower = bondriver.ToLower Then
                        '既に使用中
                        Dim temp As Integer = Val(d(0).Replace("x", ""))
                        If temp > 0 Then
                            '既存のストリームを停止してnumは指定されたものを新規として使用するなら↓
                            'Me.stop_movie(temp)
                            'log1write("指定された" & bondriver & "はストリーム" & temp & "で使用中です。ストリーム番号" & temp & "を停止しました")

                            'そうでなければnumのほうを変更する。こちらのほうが切り替えが速い
                            If num <> temp Then
                                num = temp
                                log1write("指定された" & bondriver & "はストリーム" & temp & "で使用中です。ストリーム番号を" & num_org & "から" & num & "に変更しました")
                            End If
                        Else
                            log1write("【エラー】" & d(0) & "は無効な配信ナンバーです")
                        End If
                        Exit For
                    End If
                End If
            Next
        End If

        Return num
    End Function

    '===================================
    'WEBインターフェース
    '===================================
    'ログを送る
    Public Function WI_SHOW_LOG() As String
        Return log1
    End Function

    'TvRemoteViewer_VB status
    Public Function WI_GET_TVRV_STATUS() As String
        'NHKSELECTMODE,フォーム上の解像度、等
        Dim r As String = ""
        Dim i As Integer = 0

        r &= "【全般】" & vbCrLf
        r &= "_tsfile_wait=" & Me._tsfile_wait & vbCrLf
        r &= "Stop_RecTask_at_StartEnd=" & Stop_RecTask_at_StartEnd & vbCrLf
        r &= "Stop_ffmpeg_at_StartEnd=" & Stop_ffmpeg_at_StartEnd & vbCrLf
        r &= "Stop_vlc_at_StartEnd=" & Stop_vlc_at_StartEnd & vbCrLf
        r &= "MAX_STREAM_NUMBER=" & MAX_STREAM_NUMBER & vbCrLf
        r &= "STOP_IDLEMINUTES=" & STOP_IDLEMINUTES & vbCrLf
        r &= vbCrLf
        r &= "【UDPアプリ】" & vbCrLf
        r &= "_udpApp=" & Me._udpApp & vbCrLf
        r &= "_udpPort=" & Me._udpPort & vbCrLf
        r &= "_udpOpt=" & Me._udpOpt3 & vbCrLf
        r &= vbCrLf
        r &= "【BonDriver】" & vbCrLf
        r &= "_BonDriverPath=" & Me._udpApp & vbCrLf
        r &= "TvProgramD_BonDriver1st="
        If TvProgramD_BonDriver1st IsNot Nothing Then
            Dim s As String = ""
            For i = 0 To TvProgramD_BonDriver1st.Length - 1
                r &= s & TvProgramD_BonDriver1st(i)
                s = ","
            Next
            r &= vbCrLf
        End If
        r &= "TvProgramS_BonDriver1st="
        If TvProgramS_BonDriver1st IsNot Nothing Then
            Dim s As String = ""
            For i = 0 To TvProgramS_BonDriver1st.Length - 1
                r &= s & TvProgramS_BonDriver1st(i)
                s = ","
            Next
            r &= vbCrLf
        End If
        r &= "TvProgramP_BonDriver1st="
        If TvProgramP_BonDriver1st IsNot Nothing Then
            Dim s As String = ""
            For i = 0 To TvProgramP_BonDriver1st.Length - 1
                r &= s & TvProgramP_BonDriver1st(i)
                s = ","
            Next
            r &= vbCrLf
        End If
        r &= "Allow_BonDriver4Streams=" & Me.Allow_BonDriver4Streams & vbCrLf
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
        r &= "【番組表】" & vbCrLf
        r &= "TvProgram_tvrock_url=" & TvProgram_tvrock_url & vbCrLf
        r &= "TvProgram_EDCB_url=" & TvProgram_EDCB_url & vbCrLf
        r &= "SelectNum1=" & TvProgram_SelectUptoNum & vbCrLf
        r &= "TvProgramEDCB_premium=" & TvProgramEDCB_premium & vbCrLf
        r &= "ptTimer_path=" & ptTimer_path & vbCrLf
        r &= "Tvmaid_url=" & Tvmaid_url & vbCrLf
        r &= vbCrLf
        r &= "【ファイル再生】" & vbCrLf
        If Me._videopath IsNot Nothing Then
            For i = 0 To Me._videopath.Length - 1
                r &= "_videopath=" & Me._videopath(i) & vbCrLf
            Next
        End If
        r &= "_AddSubFolder=" & Me._AddSubFolder & vbCrLf
        r &= "VideoSeekDefault=" & VideoSeekDefault & vbCrLf
        r &= "VideoSizeCheck=" & VideoSizeCheck & vbCrLf
        r &= vbCrLf
        r &= "【HTTPストリーム再生】" & vbCrLf
        'HTTPストリーム再生にどのhlsアプリを使用するか 0=フォーム 1=vlc 2=ffmpeg
        r &= "HTTPSTREAM_App=" & HTTPSTREAM_App & vbCrLf
        'VLCポート　'とりあえずは個別設定無し
        r &= "HTTPSTREAM_VLC_port=" & HTTPSTREAM_VLC_port & vbCrLf
        'VLCポート＝UDPポートナンバーにいくつプラスするか
        'r &= "HTTPSTREAM_VLC_port_plus=" & HTTPSTREAM_VLC_port_plus & vbCrLf
        r &= "HTTPSTREAM_FFMPEG_BUFFER=" & HTTPSTREAM_FFMPEG_BUFFER & vbCrLf

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
        For Each stFilePath As String In System.IO.Directory.GetFiles(fileroot, "mystream" & num.ToString & "-*.ts")
            s = stFilePath
            ts_count += 1
        Next

        r = ts_count

        Return r
    End Function

    '地デジ番組表取得
    Public Function WI_GET_PROGRAM_D() As String
        Dim r As String = ""
        r = program_translate4WI(0)
        Return r
    End Function

    'Tvmaid番組表取得
    Public Function WI_GET_PROGRAM_TVMAID(Optional ByVal getnext As Integer = 0) As String
        Dim r As String = ""
        r = program_translate4WI(996, getnext)
        Return r
    End Function

    'ptTimer番組表取得
    Public Function WI_GET_PROGRAM_PTTIMER(Optional ByVal getnext As Integer = 0) As String
        Dim r As String = ""
        r = program_translate4WI(997, getnext)
        Return r
    End Function

    'EDCB番組表取得
    Public Function WI_GET_PROGRAM_EDCB(Optional ByVal getnext As Integer = 0) As String
        Dim r As String = ""
        r = program_translate4WI(998, getnext)
        Return r
    End Function

    'TVROCK番組表取得
    Public Function WI_GET_PROGRAM_TVROCK(Optional ByVal getnext As Integer = 0) As String
        Dim r As String = ""
        r = program_translate4WI(999, getnext)
        Return r
    End Function

    '解像度取得 stream_mode = 0 and 2
    Public Function WI_GET_RESOLUTION() As String
        Dim r As String = ""
        Dim i As Integer = 0

        r &= "[stream_mode=0]" & vbCrLf
        If hls_option IsNot Nothing Then
            For i = 0 To hls_option.Length - 1
                r &= hls_option(i).resolution & vbCrLf
            Next
        End If

        r &= vbCrLf
        r &= "[stream_mode=2]" & vbCrLf
        If HTTPSTREAM_App = 1 Then
            'VLC
            If vlc_http_option IsNot Nothing Then
                For i = 0 To vlc_http_option.Length - 1
                    r &= vlc_http_option(i).resolution & vbCrLf
                Next
            End If
        ElseIf HTTPSTREAM_App = 2 Then
            'ffmpeg
            If ffmpeg_http_option IsNot Nothing Then
                For i = 0 To ffmpeg_http_option.Length - 1
                    r &= ffmpeg_http_option(i).resolution & vbCrLf
                Next
            End If
        End If

        Return r
    End Function

    'HTTPSTREAM_App遠隔切り替え
    Public Function WI_SET_HTTPSTREAM_App(ByVal a As Integer) As String
        Dim r As String = ""
        If a = 1 Then
            HTTPSTREAM_App = a
            r = "HTTP配信アプリとしてVLCを指定しました"
        ElseIf a = 2 Then
            HTTPSTREAM_App = a
            r = "HTTP配信アプリとしてffmpegを指定しました"
        End If
        Return r
    End Function

    'ストリームフォルダにファイルが存在するかどうか
    Public Function WI_STREAMFILE_EXIST(ByVal fl_file As String) As String
        Dim r As String = ""

        'fl_file = filename_escape_recall(fl_file) ',エスケープを戻す　サムネイルはエスケープ済なので戻す必要がない

        fl_file = trim8(fl_file)
        If fl_file.IndexOf("\..\") < 0 And fl_file.IndexOf(":\") < 0 Then
            If fl_file.Length > 0 Then
                fl_file = Me._fileroot & "\" & fl_file
                While fl_file.IndexOf("\\") >= 0
                    fl_file = fl_file.Replace("\\", "\")
                End While
                If file_exist(fl_file) = 1 Then
                    r = "1"
                End If
            End If
        End If

        Return r
    End Function

    'ファイル書き込み
    Public Function WI_FILE_OPE(ByVal fl_cmd As String, ByVal fl_file As String, ByVal fl_text As String, ByVal dir_filter As String) As String
        Dim r As String = ""
        Dim i As Integer = 0

        If dir_filter.Length = 0 Then
            dir_filter = "*"
        End If

        Dim filepath As String = ""
        Dim filename As String = ""
        fl_file = fl_file.Replace("/", "\") '/で指定されてくるかもしれない
        While fl_file.IndexOf("\\") >= 0
            fl_file = fl_file.Replace("\\", "\")
        End While
        If fl_file.Length > 0 Then
            '末尾が\なら削る
            Try
                While fl_file.Substring(fl_file.Length - 1, 1) = "\"
                    fl_file = fl_file.Substring(0, fl_file.Length - 1)
                End While
            Catch ex As Exception
                fl_file = ""
            End Try
            '先頭が\なら削る
            Try
                While fl_file.Substring(0, 1) = "\"
                    fl_file = fl_file.Substring(1)
                End While
            Catch ex As Exception
                fl_file = ""
            End Try
        End If
        If fl_cmd = "dir" Then
            filepath = fl_file
            filename = ""
        Else
            If fl_file.IndexOf("\") >= 0 Then
                filepath = filepath2path(fl_file)
                filename = Path.GetFileName(fl_file)
            Else
                filepath = ""
                filename = fl_file
            End If
        End If

        Dim fullpath As String = ""
        Dim fullpathfilename As String = ""

        Dim lastf As String = Path.GetFileName(Me._fileroot) '末フォルダ名
        If Me._fileroot.Length > 0 And Me._fileroot.IndexOf(Me._wwwroot) < 0 And filepath = lastf Then
            '動画フォルダへのアクセス
            fullpath = Me._fileroot & "\" '末尾は\
            fullpathfilename = Me._fileroot & "\" & filename
        ElseIf Me._fileroot.Length > 0 And Me._fileroot.IndexOf(Me._wwwroot) < 0 And filepath.IndexOf(lastf & "\") >= 0 Then
            '動画フォルダの子フォルダへのアクセス
            Dim sp9 As Integer = filepath.IndexOf(lastf)
            Dim cf As String = ""
            Try
                cf = filepath.Substring(sp9 + lastf.Length)
            Catch ex As Exception
            End Try
            fullpath = Me._fileroot & cf & "\"
            fullpathfilename = Me._fileroot & cf & "\" & filename
        Else
            '普通のフォルダへのアクセスだった
            fullpath = Me._wwwroot & "\" & filepath '末尾は\
            fullpathfilename = Me._wwwroot & "\" & filepath & "\" & filename
        End If

        'フォルダが存在しなければ作成
        If fl_cmd.IndexOf("write") >= 0 Then
            If folder_exist(fullpath) < 1 Then
                Try
                    System.IO.Directory.CreateDirectory(fullpath)
                Catch ex As Exception
                    r = "2,フォルダ作成に失敗しました。" & ex.Message & vbCrLf
                    Return r
                    Exit Function
                End Try
            End If
        End If

        If fl_file.IndexOf("\..\") >= 0 Then
            r = "2,フォルダ指定が不正です" & vbCrLf '失敗
        ElseIf folder_exist(fullpath) < 1 Then
            r = "2,指定されたフォルダが見つかりません" & vbCrLf
        Else
            Select Case fl_cmd
                Case "dir"
                    'ファイル一覧
                    Try
                        Dim files As String() = System.IO.Directory.GetFiles(fullpath, dir_filter)
                        r &= "0,SUCCESS" & vbCrLf
                        If files IsNot Nothing Then
                            For Each fn As String In files
                                fn = filename_escape_set(fn) ',をエスケープ
                                r &= fn & vbCrLf
                            Next
                        End If
                    Catch ex As Exception
                        'エラー
                        r = "2,ファイル一覧取得中にエラーが発生しました。" & ex.Message & vbCrLf
                    End Try
                Case "read"
                    If file_exist(fullpathfilename) = 1 Then
                        r = file2str(fullpathfilename, "UTF-8")
                        r = "0,SUCCESS" & vbCrLf & r
                    Else
                        r = "2,ファイルが見つかりませんでした" & vbCrLf
                    End If
                Case "write" '新規・上書き
                    If filename.Length > 0 Then
                        If str2file(fullpathfilename, fl_text, "UTF-8") = 1 Then
                            '新規・上書き成功
                            r = "0,SUCCESS" & vbCrLf
                        Else
                            '失敗
                            r = "2,ファイル書き込みに失敗しました" & vbCrLf
                        End If
                    Else
                        'ファイル名が指定されていない
                        r = "2,ファイル名が不正です" & vbCrLf
                    End If
                Case "write_add" '追記
                    If filename.Length > 0 Then
                        Try
                            Dim sw As New System.IO.StreamWriter(fullpathfilename, True, System.Text.Encoding.GetEncoding("UTF-8"))
                            '内容を追加モードで書き込む
                            sw.Write(fl_text)
                            '閉じる
                            sw.Close()
                            r = "0,SUCCESS" & vbCrLf
                        Catch ex As Exception
                            'ファイルオープンエラー
                            r = "2,追記書き込みに失敗しました。" & ex.Message & vbCrLf
                        End Try
                    Else
                        'ファイル名が指定されていない
                        r = "2,ファイル名が不正です" & vbCrLf
                    End If
                Case "delete" '削除
                    If filename.Length > 0 Then
                        If deletefile(fullpathfilename) = 1 Then
                            '削除成功
                            r = "0,SUCCESS" & vbCrLf
                        Else
                            '失敗
                            r = "2,ファイル削除に失敗しました" & vbCrLf
                        End If
                    Else
                        'ファイル名が指定されていない
                        r = "2,ファイル名が不正です" & vbCrLf
                    End If
                Case Else
                    'コマンドエラー
                    r = "2,コマンドが不正です" & vbCrLf
            End Select
        End If

        Return r
    End Function

    '　本体はProcessManager.vbに
    'Public Function WI_GET_LIVE_STREAM() As String
    'Return Me._procMan.WI_GET_LIVE_STREAM()
    'End Function

    'WI_GET_LIVE_STREAMの末尾にURLを追加
    Public Function WI_GET_LIVE_STREAM() As String
        'WI_GET_LIVE_STREAM()で配信されているストリームを取得
        '_listNo.,num, udpPort, BonDriver, ServiceID, ch_space, stream_mode, NHKMODE
        'stopping, チャンネル名, hlsApp, シーク秒
        Dim r As String = Me._procMan.WI_GET_LIVE_STREAM()
        'これにURLを付加することにする
        '_listNo.,num, udpPort, BonDriver, ServiceID, ch_space, stream_mode, NHKMODE
        'stopping, チャンネル名, hlsApp, シーク秒,URL
        If r.Length > 0 Then
            Dim list() As String = Split(r, vbCrLf)
            Dim i As Integer
            For i = 0 To list.Length - 1
                Dim url As String = "http://127.0.0.1"
                Dim port As Integer = Me._wwwport
                Dim path1 As String = "/"
                Dim fname As String = ""

                Dim d() As String = list(i).Split(",")
                If d.Length >= 12 Then
                    Dim smode As Integer = Val(d(6)) 'stream_mode
                    Dim HApp As String = d(10) 'HLS exe
                    Dim n As String = Val(Trim(d(1))) '配信ナンバー
                    If smode <= 1 Then
                        'HLS配信
                        'パスを求める
                        If Me._fileroot.IndexOf(Me._wwwroot) = 0 Then
                            '相対パス
                            path1 = Me._fileroot.Replace(Me._wwwroot, "").Replace("\", "/") '/stream
                        Else
                            '別フォルダ
                            Dim sp As Integer = Me._fileroot.LastIndexOf("\")
                            If sp > 0 Then
                                Try
                                    path1 = "/" & Me._fileroot.Substring(sp + 1)
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                        'ファイル名
                        fname = "/" & "mystream" & n & ".m3u8"
                    Else
                        'HTTP配信
                        If HApp.ToLower.IndexOf("vlc") >= 0 Then
                            'VLC配信
                            fname = "mystream" & n & ".ts"
                            port = HTTPSTREAM_VLC_port + n - 1
                        ElseIf HApp.ToLower.IndexOf("ffmpeg") >= 0 Then
                            'ffmpeg配信
                            fname = "WatchTV" & n & ".ts"
                        Else
                            'その他　いまのところありえない　とりあえずffmpegと同じにしておくか
                            fname = "WatchTV" & n & ".ts"
                        End If
                    End If

                    list(i) &= ", " & url & ":" & port.ToString & path1 & fname
                End If
            Next
            '最後にひとつの文字列にする
            r = ""
            For i = 0 To list.Length - 1
                If list(i).Length > 10 Then
                    r &= list(i) & vbCrLf
                End If
            Next
        End If

        Return r
    End Function

    '　本体はProcessManager.vbに
    Public Function WI_GET_CHANNELS() As String
        Return Me._procMan.WI_GET_CHANNELS(Me._BonDriverPath, Me._udpApp, Me._BonDriver_NGword)
    End Function

    '　本体はProcessManager.vbに
    Public Function WI_GET_ERROR_STREAM() As String
        Return Me._procMan.WI_GET_ERROR_STREAM()
    End Function

    '　本体はProcessManager.vbに
    Public Function WI_GET_PROGRAM_NUM(ByVal num As Integer) As String
        Return Me._procMan.WI_GET_PROGRAM_NUM(num)
    End Function

    '倍速ファイル再生のために逆数の分数を求める
    Public Function get_bunsuu_R(ByVal s As String) As String
        Dim r As String = "1/1" '標準は等速
        If Val(s.Replace(".", "")) <> 0 And s.IndexOf("-") < 0 Then
            Dim bunbo As Integer
            Dim bunshi As Integer
            Dim s2() As String = s.Split(".")
            If s2.Length = 2 Then
                bunshi = Val(s.Replace(".", ""))
                bunbo = 10 ^ s2(1).Length
                Dim k As Double = F_Gcd(bunshi, bunbo)
                If k > 1 Then
                    bunshi = (bunshi / k)
                    bunbo = (bunbo / k)
                End If
            ElseIf s2.Length = 1 Then
                bunshi = Val(s)
                bunbo = 1
            End If
            r = bunbo & "/" & bunshi '逆数
        End If
        Return r
    End Function

    Public Function F_Gcd(ByVal a As Integer, ByVal b As Integer) As Integer
        '最大公約数を求める
        '参考　http://ameblo.jp/sdw7/entry-11022575474.html
        If a < b Then
            Return F_Gcd(b, a)
        End If
        Dim d As Integer = 0
        Do
            d = a Mod b
            a = b
            b = d
        Loop Until d = 0
        Return a
    End Function

    '録画ファイルのチャプターを返す
    Public Function WI_GET_CHAPTER(ByVal fullpathfilename As String) As String
        Dim r As String = ""
        Dim chapterfullpathfilename As String = ""
        Dim chapterpath As String = Path.GetDirectoryName(fullpathfilename)
        Dim chapterfilename As String = Path.GetFileNameWithoutExtension(fullpathfilename) & ".chapter"
        If chapterfilename.Length > 0 Then
            If file_exist(chapterpath & "\" & chapterfilename) = 1 Then
                chapterfullpathfilename = chapterpath & "\" & chapterfilename
            ElseIf file_exist(chapterpath & "\chapters\" & chapterfilename) = 1 Then
                chapterfullpathfilename = chapterpath & "\chapters\" & chapterfilename
            End If
        End If
        If chapterfullpathfilename.Length > 0 Then
            '見つかれば取得
            r = ReadAllTexts(chapterfullpathfilename)
        End If
        Return r
    End Function

    '録画ファイルのチャプター情報を書き出す(↓のいずれか）
    'temp = num,チャプター文字列
    '↓は危険かもしれないので廃止
    'temp = chapterファイルフルパス,チャプター文字列
    'temp = 録画ファイルフルパス,チャプター文字列
    Public Function WI_WRITE_CHAPTER(ByVal temp As String) As String
        Dim r As String = ""
        Dim chapterfullpathfilename As String = ""
        Dim d() As String = temp.Split(",")
        If d.Length = 2 Then
            Dim chapterstr As String = trim8(d(1)) '書き込むchapter情報
            If IsNumeric(d(0)) = True Then
                'numで指定された場合
                d(0) = Me._procMan.get_fullpathfilename(d(0)) 'ProcessManager.vbで取得
                'endif 'ファイル名での指定を廃止したのでend ifは下に移動
                If trim8(d(0)).Length > 0 Then
                    'ファイル名で指定された場合
                    Dim ext As String = Path.GetExtension(d(0))
                    If ext = ".chapter" Then
                        chapterfullpathfilename = d(0)
                    Else
                        '動画ファイルが指定された場合は.chapterを探す
                        Dim p1 As String = Path.GetDirectoryName(d(0))
                        Dim f1 As String = Path.GetFileNameWithoutExtension(d(0))
                        'まずchaptersフォルダの中にあるかどうか
                        If file_exist(p1 & "\chapters\" & f1 & ".chapter") = 1 Then
                            chapterfullpathfilename = p1 & "\chapters\" & f1 & ".chapter"
                        ElseIf file_exist(p1 & "\" & f1 & ".chapter") = 1 Then
                            chapterfullpathfilename = p1 & "\" & f1 & ".chapter"
                        End If
                        If chapterfullpathfilename.Length = 0 Then
                            '既存のファイルがなかった
                            If folder_exist(p1 & "\chapters") = 1 Then
                                chapterfullpathfilename = p1 & "\chapters\" & f1 & ".chapter"
                            Else
                                chapterfullpathfilename = p1 & "\" & f1 & ".chapter"
                            End If
                        End If
                    End If

                    If chapterfullpathfilename.Length > 0 Then
                        '修正するファイルの更新日時
                        Dim modifydate As DateTime = Now()
                        Try
                            modifydate = System.IO.File.GetLastWriteTime(chapterfullpathfilename)
                        Catch ex As Exception
                        End Try
                        If str2file(chapterfullpathfilename, trim8(chapterstr)) = 1 Then
                            log1write(chapterfullpathfilename & "にチャプター情報を書き出しました")
                            '更新日時を元に戻す
                            F_modify_filedate(chapterfullpathfilename, modifydate)
                            r = "SUCCESS"
                        Else
                            log1write(chapterfullpathfilename & "へのチャプター情報書き出しに失敗しました")
                            r = "FAILED"
                        End If
                    End If
                End If
            End If
        End If
        Return r
    End Function

    'HTMLを取得する
    Public Function WI_GET_HTML(ByVal temp As String, ByRef enc_str As String) As String
        'ByRefでエンコードを返す
        Dim r As String = ""
        Dim d() As String = temp.Split(",")
        Dim method As Integer = 0
        'Dim enc_str As String = ""
        Dim UserAgent As String = ""
        Dim url As String = ""
        If d.Length >= 4 Then
            '取得方法
            method = Val(d(0))
            If method > 0 Then
                'エンコード
                If d(1).Length > 0 Then
                    enc_str = Trim(d(1))
                End If
                If enc_str.Length = 0 Then
                    enc_str = "UTF-8"
                End If
                'UserAgent
                UserAgent = Trim(d(2))
                'URL
                Dim sep1 As String = ""
                For i As Integer = 3 To d.Length - 1
                    url = d(i) & sep1
                    sep1 = ","
                Next

                '.2ch.net以外ははじく
                Dim chk As Integer = 0
                Dim sp As Integer = url.IndexOf("://")
                If sp = 4 Or sp = 5 Then
                    If Instr_pickup(url, "://", "/", 0).indexof(".2ch.net") > 0 Then
                        If url.IndexOf("/read.cgi/") > 0 Or url.IndexOf("/subback.html") > 0 Or url.IndexOf("/bbsmenu.html") > 0 Then
                            '正常
                            chk = 1
                        End If
                    End If
                End If
                If chk = 0 Then
                    '2ch以外
                    log1write("【警告】" & url & " へのアクセスを拒否しました")
                End If

                If chk = 1 Then
                    log1write("HTMLを取得します。" & url)
                    Select Case method
                        Case 1
                            r = get_html_by_WebBrowser(url, enc_str, UserAgent)
                        Case 2
                            r = get_html_by_webclient(url, enc_str, UserAgent)
                        Case 3
                            r = get_html_by_HttpWebRequest(url, enc_str, UserAgent)
                        Case Else
                            log1write("【エラー】HTML取得方法指定が不正です。" & temp)
                    End Select
                End If
            End If
        Else
            log1write("【エラー】HTML取得パラメータが不正です。" & temp)
        End If

        Return r
    End Function

    'サムネイルを取得（ss=何秒目 w=幅 h=縦）
    Public Function WI_GET_THUMBNAIL(ByVal num_str As String, ByVal ss As String, ByVal w As Integer, ByVal h As Integer) As String
        Dim r As String = ""

        Dim i As Integer = 0

        Dim num As Integer = Val(num_str)

        '動画ファイル名
        Dim video_path As String = ""
        If num > 0 And num <= MAX_STREAM_NUMBER And num_str.IndexOf(".") < 0 Then
            'numから再生中の動画ファイル名を取得
            Dim linestr As String = Me._procMan.WI_GET_LIVE_STREAM
            Dim line() As String = Split(linestr, vbCrLf)
            For i = 0 To line.Length - 1
                Dim d() As String = line(i).Split(",")
                If d.Length >= 12 Then
                    If Val(d(1)) = num And Val(d(6)) = 1 Then
                        video_path = Trim(d(3))
                        Exit For
                    End If
                End If
            Next
        ElseIf num = 0 And num_str.IndexOf(".") > 0 Then
            'ファイルのフルパスで指定された
            video_path = trim8(num_str)
        Else
            '不正な指定
        End If

        '対象ファイルが
        If video_path.IndexOf(".") > 0 Then
            Dim url_path As String = get_soutaiaddress_from_fileroot()
            Dim ffmpeg_path As String = Me._hlsApp
            Dim stream_folder As String = Me._fileroot
            r = F_make_thumbnail(num, ffmpeg_path, stream_folder, url_path, video_path, ss, w, h)
        End If

        Return r
    End Function

    '本体はProcessManager.vb
    Public Sub resume_file_streams()
        Me._procMan.resume_file_streams()
    End Sub
End Class


