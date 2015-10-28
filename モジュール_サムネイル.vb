Module モジュール_サムネイル
    Public making_thumbnail(MAX_STREAM_NUMBER) As Integer '作成中なら1

    'サムネイル作成
    Public Function F_make_thumbnail(ByVal num As Integer, ByVal ffmpeg_path As String, ByVal stream_folder As String, ByVal url_path As String, ByVal video_path As String, ByVal ss As Integer, ByVal w As Integer, ByVal h As Integer) As String
        Dim r As String = ""

        If making_thumbnail(num) = 0 Then
            Try
                making_thumbnail(num) = 1 '作成中

                '縦横指定があれば
                Dim wh As String = ""
                If w > 0 And h > 0 Then
                    wh = " -s " & w.ToString & "x" & h.ToString
                End If

                Dim t As DateTime = Now()
                log1write("サムネイル作成 開始 " & t & " " & t.Millisecond)

                If stream_folder.Length > 0 Then
                    stream_folder &= "\" '末尾に\を付加
                End If

                If ffmpeg_path.IndexOf("ffmpeg") >= 0 Then
                    If file_exist(video_path) = 1 Then

                        Dim filename As String = "thumb" & num & ".jpg"

                        'ProcessStartInfoオブジェクトを作成する
                        Dim hlsPsi As New System.Diagnostics.ProcessStartInfo()
                        '起動するファイルのパスを指定する
                        hlsPsi.FileName = ffmpeg_path
                        'コマンドライン引数を指定する
                        hlsPsi.Arguments = "-y -ss " & ss & " -i """ & video_path & """ -vframes 1 -f image2" & wh & " """ & stream_folder & filename & """"
                        ' コンソール・ウィンドウを開かない
                        hlsPsi.CreateNoWindow = True
                        ' シェル機能を使用しない
                        hlsPsi.UseShellExecute = False

                        'アプリケーションを起動する
                        Dim hlsProc As System.Diagnostics.Process = System.Diagnostics.Process.Start(hlsPsi)

                        Dim i As Integer = 500 '5秒待つ
                        Try
                            While (hlsProc IsNot Nothing AndAlso Not hlsProc.HasExited) And i > 0
                                System.Threading.Thread.Sleep(10)
                                i -= 1
                            End While
                            If i > 0 Then
                                '待機時間内に終了した
                                If file_exist(stream_folder & filename) = 1 Then
                                    r = "/" & url_path & "thumb" & num & ".jpg"
                                Else
                                    r = ""
                                End If
                            End If
                        Catch ex As Exception
                            '存在しないからエラーが出たOK
                            r = ""
                        End Try
                    End If
                End If

                log1write("サムネイル作成 終了 " & Now() & " " & Now().Millisecond)

            Catch ex As Exception
                log1write("サムネイル作成中にエラーが発生しました。" & ex.Message)
            Finally
                making_thumbnail(num) = 0 '作業終了
            End Try
        Else
            '現在作成中
            r = ""
        End If

        Return r
    End Function
End Module
