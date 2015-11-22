Imports System.Text
Imports System.IO

'■未使用
Module モジュール_WhiteBrowser
    Public WhiteBrowserWB_path As String = ""

    'mp4等の長さ（秒）
    Public Function F_get_mp4_length_from_WhiteBrowserDB(ByVal fullpathfilename As String) As Integer
        Dim r As Integer = 0

        If WhiteBrowserWB_path.Length > 0 Then
            Dim filename_noext As String = Path.GetFileNameWithoutExtension(fullpathfilename).ToLower
            If filename_noext.Length > 0 Then
                If file_exist(WhiteBrowserWB_path) = 1 Then

                    'カレントディレクトリ変更
                    F_set_ppath4program()
                    If file_exist("sqlite3.exe") = 1 Then
                        '直接コマンドラインにUTF-8の文字列は送れないのでファイルにしてから実行
                        Dim msql As String = "SELECT movie_length FROM movie WHERE movie_name LIKE '" & filename_noext & "' LIMIT 1;"
                        If str2file("sqlite3_sql.txt", msql, "UTF-8", 0) = 1 Then
                            Dim results As String = ""
                            Dim psi As New System.Diagnostics.ProcessStartInfo()

                            psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec") 'ComSpecのパスを取得する
                            psi.RedirectStandardInput = False '出力を読み取れるようにする
                            psi.RedirectStandardOutput = True
                            psi.UseShellExecute = False
                            psi.CreateNoWindow = True 'ウィンドウを表示しないようにする
                            psi.StandardOutputEncoding = Encoding.UTF8

                            Dim nowtime As DateTime = Now()
                            Dim ut As Integer = time2unix(nowtime) '現在のunixtime

                            psi.Arguments = "/c sqlite3.exe -separator " & "//_// " & """" & WhiteBrowserWB_path & """ < sqlite3_sql.txt"

                            Dim p As System.Diagnostics.Process
                            Try
                                p = System.Diagnostics.Process.Start(psi)
                                '出力を読み取る
                                results = p.StandardOutput.ReadToEnd
                                'WaitForExitはReadToEndの後である必要がある
                                '(親プロセス、子プロセスでブロック防止のため)
                                p.WaitForExit(10000)
                            Catch ex As Exception
                                log1write("【エラー】WhiteBrowserからのデータベース読込に失敗しました")
                            End Try

                            '行ごとの配列として、テキストファイルの中身をすべて読み込む
                            Dim line As String() = Split(results, vbCrLf) 'results.Split(vbCrLf)

                            If line IsNot Nothing Then
                                If line.Length > 0 Then
                                    Dim j As Integer
                                    Dim last_sid As Integer = 0
                                    Dim skip_sid As Integer = 0

                                    '最初の1行だけ
                                    Dim youso() As String = Split(line(j), "//_//")
                                    If youso IsNot Nothing Then
                                        If youso.Length >= 1 Then
                                            '秒数で返ってくる
                                            If Val(youso(0)) > 0 Then
                                                r = Val(youso(0))
                                                log1write("WhiteBrowserデータベースから " & filename_noext & " の長さ(秒)を取得しました")
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            If r = 0 Then
                                log1write("WhiteBrowserデータベースに " & filename_noext & " の長さ情報は見つかりませんでした")
                            End If
                        Else
                            log1write("【エラー】sql.txtの書き込みに失敗しました")
                        End If
                    Else
                        log1write("【エラー】WhiteBrowserデータベースにアクセスするために必要なsqlite3.exeが見つかりません")
                    End If
                Else
                    log1write("【エラー】WhiteBrowserデータベース " & WhiteBrowserWB_path & " が見つかりません")
                End If
            End If
        End If

        Return r
    End Function

End Module
