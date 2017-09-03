Module モジュール_ini設定
    Public ini_genre() As String = {"全般", "WEBサーバー", "番組表全般", "番組表データ", "HLS配信", "HTTP配信", "ファイル再生"}

    Public ini_array() As inistructure
    Public Structure inistructure
        Public name As String
        Public genre As String
        Public value As String
        Public value_temp As String
        Public value_default As String
        Public value_type As String
        Public title As String
        Public document As String
        Public need_reset As Integer
        Public ini_read As Integer
        Public write_chk As Integer
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得
            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.name = pF Then '放送局名と一致するか
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    Public Function read_ini_default() As Integer
        Dim r As Integer = 1

        'カレントディレクトリ変更
        F_set_ppath4program()

        Dim ini_default_filename As String = "TvRemoteViewer_VB.ini.data"

        Dim update_chk As Integer = 0

        If file_exist(ini_default_filename) = 1 Then
            Dim line() As String = file2line(ini_default_filename)
            If line IsNot Nothing Then

                log1write("標準設定ファイルとして " & ini_default_filename & " を読み込みました")

                Dim i, j As Integer

                If line Is Nothing Then
                ElseIf line.Length > 0 Then
                    '読み込み完了
                    For i = 0 To line.Length - 1
                        line(i) = Trim(line(i))
                        'コメント削除
                        If line(i).IndexOf(";") >= 0 Then
                            line(i) = line(i).Substring(0, line(i).IndexOf(";"))
                        End If
                        Dim youso() As String = Split(line(i), vbTab)
                        Try
                            If youso Is Nothing Then
                            ElseIf youso.Length >= 5 Then
                                Dim url_text As String = youso(1) '=以降がURLの場合(=が途中に入っている可能性を考慮）
                                'ジャンル,need_reset,型タイプ,名前,デフォルト値,見出し,説明
                                '0       ,1         ,2       ,3   ,4           ,5     ,6

                                'ジャンルを記録
                                If Array.IndexOf(ini_genre, youso(0)) < 0 Then
                                    Dim k As Integer = 0
                                    If ini_genre IsNot Nothing Then
                                        k = ini_genre.Length
                                    End If
                                    ReDim Preserve ini_genre(k)
                                    ini_genre(k) = youso(0)
                                    log1write("【警告】標準に無いiniジャンルが追加されました。" & youso(0))
                                End If
                                'ジャンルが足りない場合はアップデートをうながすため
                                If youso(0) = "番組表全般" Then
                                    update_chk = 1
                                End If

                                If youso.Length = 5 Then
                                    '説明が省かれている場合
                                    ReDim Preserve youso(6)
                                    youso(5) = ""
                                    youso(6) = ""
                                ElseIf youso.Length = 6 Then
                                    ReDim Preserve youso(6)
                                    youso(6) = ""
                                ElseIf (youso.Length > 1 And youso.Length < 7) Or youso.Length > 20 Then
                                    log1write("【エラー】不正な標準ini項目です。" & line(i))
                                End If

                                For j = 0 To youso.Length - 1
                                    youso(j) = Trim(youso(j))
                                Next

                                set_ini_data_default(youso(3), youso(0), youso(4), youso(2), youso(5), youso(6), youso(1))
                            End If
                        Catch ex As Exception
                            log1write("標準パラメーター " & youso(0) & " の読み込みに失敗しました。" & ex.Message)
                        End Try
                    Next
                End If
            End If
        Else
            log1write("【エラー】標準設定ファイル " & ini_default_filename & " が見つかりませんでした")
            r = 0
        End If

        'ファイルコピーし忘れの際の警告
        If update_chk = 0 Then
            MsgBox("設定ファイルが古くなっています" & vbCrLf & "TvRemoteViewer_VB.ini.dataをexeと同じフォルダにコピーしてください")
            log1write("【警告】設定ファイルが古くなっています。TvRemoteViewer_VB.ini.dataをexeと同じフォルダにコピーしてください")
        End If

        Return r
    End Function

    Public Sub set_ini_data_default(ByVal name As String, ByVal genre As String, ByVal value_default As String, ByVal value_type As String, ByVal title As String, ByVal document As String, ByVal need_reset As Integer)
        Dim j As Integer = -1
        If ini_array IsNot Nothing Then
            j = Array.IndexOf(ini_array, name)
        End If
        If j < 0 Then
            If ini_array Is Nothing Then
                j = 0
            Else
                j = ini_array.Length
            End If
            ReDim Preserve ini_array(j)
        End If
        ini_array(j).name = name
        ini_array(j).value = value_default
        ini_array(j).value_temp = value_default
        ini_array(j).value_default = value_default
        ini_array(j).value_type = value_type
        ini_array(j).genre = genre
        ini_array(j).title = title.Replace("\n", vbCrLf)
        ini_array(j).document = document.Replace("\n", vbCrLf)
        If need_reset >= 0 Then
            ini_array(j).need_reset = need_reset
        End If
        ini_array(j).ini_read = 1
    End Sub

    Public Sub set_ini_data(ByVal name As String, ByVal value As String)
        Dim j As Integer = -1
        If ini_array IsNot Nothing Then
            j = Array.IndexOf(ini_array, name)
        End If
        If j < 0 Then
            If ini_array Is Nothing Then
                j = 0
            Else
                j = ini_array.Length
            End If
            ReDim Preserve ini_array(j)
        End If
        ini_array(j).name = name
        ini_array(j).value = value
        ini_array(j).value_temp = value
        ini_array(j).ini_read = 2
    End Sub

End Module
