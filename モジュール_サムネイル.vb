Module モジュール_サムネイル
    Public making_thumbnail(MAX_STREAM_NUMBER) As Integer '作成中なら1

    'サムネイル作成
    Public Function F_make_thumbnail(ByVal num As Integer, ByVal ffmpeg_path As String, ByVal stream_folder As String, ByVal url_path As String, ByVal video_path As String, ByVal ss As String, ByVal w As Integer, ByVal h As Integer) As String
        Dim r As String = ""

        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim ss2() As Integer = Nothing

        If ffmpeg_path.IndexOf("ffmpeg") >= 0 Then
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
                thumb_filename_noex = thumb_filename_noex.Replace("#", "♯")
            Else
                'ファイル名 thumb(.秒数).jpg
                thumb_filename_noex = "mystream" & num & "_thumb"
            End If

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

                    '時間がかかるので終了まで待機しない

                    '成功失敗にかかわらず結果を返す
                    r = "/" & url_path & filename
                Else
                    '秒数指定
                    Dim t As DateTime = Now()
                    log1write("サムネイル作成 開始 " & t & " " & t.Millisecond & " " & video_path)

                    Dim sep1 As String = ""
                    For i = 0 To ss2.Length - 1
                        If making_thumbnail(num) = 0 Then
                            Try
                                If num > 0 Then
                                    making_thumbnail(num) = 1 '作成中
                                End If

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

                                j = 15 * 100 '最大15秒待つ
                                Try
                                    While (hlsProc IsNot Nothing AndAlso Not hlsProc.HasExited) And j > 0
                                        System.Threading.Thread.Sleep(10)
                                        j -= 1
                                    End While
                                    If j > 0 Then
                                        '待機時間内に終了した
                                        If file_exist(stream_folder & filename) = 1 Then
                                            r &= sep1 & "/" & url_path & filename
                                            sep1 = ","
                                        Else
                                            'ファイルが作成されていない
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
                                    log1write("【エラー】サムネイル作成が時間内に終了しませんでした")
                                End Try

                            Catch ex As Exception
                                log1write("【エラー】サムネイル作成中にエラーが発生しました。" & ex.Message)
                            Finally
                                making_thumbnail(num) = 0 '作業終了
                            End Try
                        Else
                            '現在作成中
                            log1write("【警告】同ストリームでサムネイル作成中です。作成を中止しました。num=" & num.ToString)
                            r = ""
                        End If
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
