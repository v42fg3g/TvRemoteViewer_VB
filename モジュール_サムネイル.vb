Imports System.IO

Module モジュール_サムネイル
    'Public thumbnail_ffmpeg As String = "" 'サムネイル作成用ffmpegパス →exepath_ffmpegに統合

    Public stop_per_thumbnail_minutes As Integer = 60 * 30 '一定間隔サムネイル作成を最大何秒待つか 30分
    Public making_per_thumbnail() As making_thumbnail_structure '一定間隔サムネイル作成中かどうか
    Public Structure making_thumbnail_structure
        Public fullpathfilename As String '録画ファイルフルパス
        Public indexofstr As String '重複認識用
        Public unixtime As Integer '処理開始日時
        Public process As Process
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.indexofstr = pF Then '録画ファイル名と一致するか
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    'サムネイル作成
    Public Function F_make_thumbnail(ByVal num As Integer, ByVal ffmpeg_path As String, ByVal stream_folder As String, ByVal url_path As String, ByVal video_path As String, ByVal ss As String, ByVal w As Integer, ByVal h As Integer, Optional track As Integer = -1) As String
        Dim r As String = ""

        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim ss2() As Integer = Nothing

        Dim thru_wait As Integer = 0

        video_path = filename_escape_recall(video_path) ',エスケープを戻す

        'サムネイル用ffmpegが指定されていればそちらを使用（QSVEnc使用時）
        If exepath_ffmpeg.Length > 0 Then
            ffmpeg_path = exepath_ffmpeg
        End If

        'サムネイル作成終了を待たない場合
        If ss.IndexOf("thru") = 0 Then
            Try
                ss = ss.Substring(4)
                thru_wait = 1
            Catch ex As Exception
                'エラー
                log1write("サムネイルを作成する秒数指定が不正です")
                Return r
                Exit Function
            End Try
        End If
        '指定秒数
        If ss.IndexOf("per") >= 0 Then
            '等間隔
            ReDim Preserve ss2(0)
            ss2(0) = -Val(ss.Replace("per", ""))
        ElseIf ss.IndexOf(":") >= 0 Then
            '複数指定
            Dim d() As String = ss.Split(":")
            j = 0
            For i = 0 To d.Length - 1
                If Trim(d(i)).Length > 0 Then
                    ReDim Preserve ss2(j)
                    ss2(j) = Val(d(i))
                    j += 1
                End If
            Next
        Else
            '単独指定
            ReDim Preserve ss2(0)
            ss2(0) = Val(ss)
        End If

        If Path.GetExtension(video_path).ToLower = ".iso" And exepath_ISO_VLC.Length > 0 And ss2(0) >= 0 Then
            'ISO再生 単独作成のみ対応
            If track >= 0 Then
                Dim t As DateTime = Now()
                log1write("サムネイル作成 開始 " & t & " " & t.Millisecond & " " & video_path)

                Dim filename_noext As String = "mystream" & num & "_thumb"

                'Dim parastr As String = """" & ffmpeg_path & """ dvdsimple:///""" & video_path & """/#" & track.ToString & " --start-time " & ss.ToString & " --stop-time " & (ss + 1).ToString & " --no-repeat  vlc://quit --rate=1 --video-filter=scene -I dummy --dummy-quiet --vout=dummy --aout=dummy --scene-width=" & w.ToString & " --scene-height=" & h.ToString & " --scene-format=jpg --scene-ratio=14 --scene-prefix=" & "mystream" & num & "_thumb" & " --scene-replace --scene-path=""" & stream_folder & """"
                Dim parastr As String = "dvdsimple:///""" & video_path & """/#" & track.ToString & " --start-time " & ss2(0).ToString & " --stop-time " & (ss2(0) + 1).ToString & " --no-repeat  vlc://quit --rate=1 --video-filter=scene -I dummy --dummy-quiet --vout=dummy --aout=dummy --scene-width=" & w.ToString & " --scene-height=" & h.ToString & " --scene-format=jpg --scene-ratio=14 --scene-prefix=" & filename_noext & " --scene-replace --scene-path=""" & stream_folder & """"
                '実行
                Dim psInfo As New ProcessStartInfo()
                psInfo.FileName = exepath_ISO_VLC
                '実行コマンド
                psInfo.Arguments = parastr
                ' コンソール・ウィンドウを開かない
                psInfo.CreateNoWindow = True
                ' シェル機能を使用しない
                psInfo.UseShellExecute = False
                '起動
                Dim p As Process = Process.Start(psInfo)

                r &= "/" & url_path & filename_noext & ".jpg"

                If thru_wait = 0 Then
                    j = 15 * 100 '最大15秒待つ
                    Try
                        While (p IsNot Nothing AndAlso Not p.HasExited) And j > 0
                            System.Threading.Thread.Sleep(10)
                            j -= 1
                        End While
                        If j > 0 Then
                            '待機時間内に終了した
                            'ファイルができるまで待つ
                            Dim k As Integer = 10 * 20 '最大10秒待つ
                            System.Threading.Thread.Sleep(10)
                            While file_exist(stream_folder & "\" & filename_noext & ".jpg") = 0 And k > 0
                                System.Threading.Thread.Sleep(50)
                                k -= 1
                            End While
                            If k = 0 Then
                                'ファイルが作成されていない
                                log1write("【エラー】サムネイル作成に失敗しました")
                            End If
                        Else
                            '時間内に終了しなかった・・これを回数分繰り返すとやばい
                            Try
                                p.Kill()
                            Catch ex As Exception
                            End Try
                            log1write("【エラー】サムネイル作成が時間内に終了しませんでした")
                        End If
                    Catch ex As Exception
                        '存在しないからエラーが出たOK
                        log1write("【エラー】サムネイル作成中にエラーが発生しました[A]。" & ex.Message)
                    End Try
                End If

                Try
                    p.Close()
                Catch ex As Exception
                End Try

                t = Now()
                log1write("サムネイル作成 終了 " & t & " " & t.Millisecond)
            End If
        ElseIf isMatch_HLS(ffmpeg_path, "ffmpeg") = 1 And Path.GetExtension(video_path).ToLower <> ".iso" Then
            'ffmpegの場合
            Dim thumb_filename_noex As String = ""
            If num = 0 Then
                'ビデオファイル直接指定ならstreamフォルダ内のthumbsフォルダに作成することにした
                Dim stream_folder2 As String = stream_folder & "\file_thumbs"
                If folder_exist(stream_folder2) <= 0 Then
                    Try
                        System.IO.Directory.CreateDirectory(stream_folder2)
                        stream_folder = stream_folder2
                        url_path &= "file_thumbs/"
                    Catch ex As Exception
                        log1write("【エラー】" & stream_folder2 & "のフォルダ作成に失敗しました")
                    End Try
                Else
                    stream_folder = stream_folder2
                    url_path &= "file_thumbs/"
                End If
                'ファイル名は、動画ファイル名(.秒数).jpg
                thumb_filename_noex = System.IO.Path.GetFileNameWithoutExtension(video_path)
                'ここで、#が含まれるとどうしても作成したサムネイルにアクセスできないので全角に変換
                thumb_filename_noex = thumb_filename_noex.Replace("#", "＃")
            Else
                'ファイル名 thumb(.秒数).jpg
                thumb_filename_noex = "mystream" & num & "_thumb"
            End If

            thumb_filename_noex = filename_escape_set(thumb_filename_noex) ',をエスケープ

            '縦横指定があれば
            Dim wh As String = ""
            If w > 0 And h > 0 Then
                wh = " -s " & w.ToString & "x" & h.ToString
            End If

            If stream_folder.Length > 0 Then
                stream_folder &= "\" '末尾に\を付加
            End If

            If file_exist(video_path) = 1 Then
                If ss2(0) < 0 Then
                    '等間隔
                    Dim chk As Integer = -1
                    If making_per_thumbnail IsNot Nothing Then
                        chk = Array.IndexOf(making_per_thumbnail, video_path & "." & num.ToString)
                    End If
                    If chk < 0 Then
                        '作成中でなければ
                        Dim idx As Integer = F_find_making_per_thumbnail_number()
                        If idx >= 0 Then
                            '【プロセス監視用】作成中であることを記録
                            making_per_thumbnail(idx).fullpathfilename = video_path
                            making_per_thumbnail(idx).indexofstr = video_path & "." & num.ToString
                            making_per_thumbnail(idx).unixtime = time2unix(Now())

                            Dim t As DateTime = Now()
                            log1write(video_path & " 等間隔サムネイルの作成を開始しました")

                            ss2(0) = -ss2(0)

                            Dim filename As String = thumb_filename_noex & "-%04d.jpg"

                            'ProcessStartInfoオブジェクトを作成する
                            Dim hlsPsi As New System.Diagnostics.ProcessStartInfo()
                            '起動するファイルのパスを指定する
                            hlsPsi.FileName = ffmpeg_path
                            'コマンドライン引数を指定する
                            'http://injury-time.hatenablog.com/entry/2015/01/07/040033
                            Dim fps2 As Double = ToRoundDown(1 / ss2(0), 10) '小数点10桁にしておくか
                            hlsPsi.Arguments = "-y -i """ & video_path & """ -filter:v fps=fps=" & fps2 & ":round=down" & wh & " """ & stream_folder & filename & """"
                            ' コンソール・ウィンドウを開かない
                            hlsPsi.CreateNoWindow = True
                            ' シェル機能を使用しない
                            hlsPsi.UseShellExecute = False

                            'アプリケーションを起動する
                            Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)

                            '【プロセス監視用】プロセスを記録
                            making_per_thumbnail(idx).process = hlsProc

                            '時間がかかるので終了まで待機しない

                            '成功失敗にかかわらず結果を返す
                            r = "/" & url_path & filename
                        Else
                            '全件使用中
                            log1write("【警告】一定間隔サムネイル作成中配列が全て使用中です")
                            r = ""
                        End If
                    Else
                        '現在作成中
                        log1write("【警告】同ファイルのサムネイル作成中です。作成を中止しました。num=" & num.ToString)
                        r = ""
                    End If
                Else
                    '秒数指定
                    Dim t As DateTime = Now()
                    log1write("サムネイル作成 開始 " & t & " " & t.Millisecond & " " & video_path)

                    Dim sep1 As String = ""
                    For i = 0 To ss2.Length - 1
                        Try
                            Dim filename As String = ""
                            If ss.IndexOf(":") >= 0 Then
                                filename = thumb_filename_noex & "." & ss2(i) & ".jpg"
                            Else
                                filename = thumb_filename_noex & ".jpg"
                            End If

                            'ProcessStartInfoオブジェクトを作成する
                            Dim hlsPsi As New System.Diagnostics.ProcessStartInfo()
                            '起動するファイルのパスを指定する
                            hlsPsi.FileName = ffmpeg_path
                            'コマンドライン引数を指定する
                            hlsPsi.Arguments = "-y -ss " & ss2(i) & " -i """ & video_path & """ -vframes 1 -f image2" & wh & " """ & stream_folder & filename & """"
                            ' コンソール・ウィンドウを開かない
                            hlsPsi.CreateNoWindow = True
                            ' シェル機能を使用しない
                            hlsPsi.UseShellExecute = False

                            'アプリケーションを起動する
                            Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)

                            If thru_wait = 0 Then
                                j = 15 * 100 '最大15秒待つ
                                Try
                                    While (hlsProc IsNot Nothing AndAlso Not hlsProc.HasExited) And j > 0
                                        System.Threading.Thread.Sleep(10)
                                        j -= 1
                                    End While
                                    If j > 0 Then
                                        '待機時間内に終了した
                                        r &= sep1 & "/" & url_path & filename
                                        sep1 = ","
                                        'ファイルができるまで待つ
                                        Dim k As Integer = 10 * 20 '最大10秒待つ
                                        System.Threading.Thread.Sleep(10)
                                        While file_exist(stream_folder & "\" & filename) = 0 And k > 0
                                            System.Threading.Thread.Sleep(50)
                                            k -= 1
                                        End While
                                        If k = 0 Then
                                            'ファイルが作成されていない
                                            log1write("【エラー】サムネイル作成に失敗しました")
                                        End If
                                    Else
                                        '時間内に終了しなかった・・これを回数分繰り返すとやばい
                                        Try
                                            hlsProc.Kill()
                                        Catch ex As Exception
                                        End Try
                                        log1write("【エラー】サムネイル作成が時間内に終了しませんでした")
                                        Exit For
                                    End If
                                Catch ex As Exception
                                    '存在しないからエラーが出たOK
                                    log1write("【エラー】サムネイル作成中にエラーが発生しました[B]。" & ex.Message)
                                End Try
                            Else
                                '終了を待たない
                                r &= sep1 & "/" & url_path & filename
                                sep1 = ","
                            End If

                        Catch ex As Exception
                            log1write("【エラー】サムネイル作成中にエラーが発生しました。" & ex.Message)
                        End Try
                    Next

                    t = Now()
                    log1write("サムネイル作成 終了 " & t & " " & t.Millisecond)
                End If
            Else
                log1write("【エラー】サムネイル作成対象ファイルが見つかりませんでした。" & video_path)
            End If
        End If

        Return r
    End Function

    Public Function ncs(ByVal str As String, ByVal cnt As Integer) As String
        Dim r As String = str
        Dim c As Integer = 0
        Dim s As String = Chr(124)
        Dim sp As Integer = str.IndexOf(s)
        While sp > 0
            c += 1
            Try
                sp = str.IndexOf(s, sp + 1)
            Catch ex As Exception
                Exit While
            End Try
        End While
        If c > cnt Then
            r = ""
        End If
        Return str
    End Function

    '使用可能なmaking_per_thumbnail配列indexを返す
    Public Function F_find_making_per_thumbnail_number() As Integer
        Dim r As Integer = -1
        Dim ut As Integer = time2unix(Now())
        If making_per_thumbnail Is Nothing Then
            ReDim Preserve making_per_thumbnail(0)
            r = 0
        Else
            For i As Integer = 0 To making_per_thumbnail.Length - 1
                If making_per_thumbnail(i).fullpathfilename Is Nothing Then
                    r = 0
                    Exit For
                End If
                If making_per_thumbnail(i).fullpathfilename.Length = 0 Then
                    r = i
                    Exit For
                End If
            Next
            If r < 0 Then
                Dim j As Integer = making_per_thumbnail.Length
                ReDim Preserve making_per_thumbnail(j)
                r = j
            End If
        End If

        Return r
    End Function

    'http://jeanne.wankuma.com/tips/vb.net/math/rounddown.html
    ' ------------------------------------------------------------------------
    ' <summary>
    '     指定した精度の数値に切り捨てします。</summary>
    ' <param name="dValue">
    '     丸め対象の倍精度浮動小数点数。</param>
    ' <param name="iDigits">
    '     戻り値の有効桁数の精度。</param>
    ' <returns>
    '     iDigits に等しい精度の数値に切り捨てられた数値。</returns>
    ' ------------------------------------------------------------------------
    Public Function ToRoundDown(ByVal dValue As Double, ByVal iDigits As Integer) As Double
        Dim dCoef As Double = System.Math.Pow(10, iDigits)

        If dValue > 0 Then
            Return System.Math.Floor(dValue * dCoef) / dCoef
        Else
            Return System.Math.Ceiling(dValue * dCoef) / dCoef
        End If
    End Function
End Module
