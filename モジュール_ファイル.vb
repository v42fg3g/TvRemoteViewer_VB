Imports System
Imports System.IO
Imports System.Text

Module モジュール_ファイル
    Public Function ReadAllTexts(ByVal path As String) As String
        Dim r As String = ""
        Try
            Using fs As New FileStream(path, FileMode.Open, _
                FileAccess.Read, FileShare.ReadWrite)
                Using sr As TextReader = New StreamReader(fs, _
                    Encoding.GetEncoding("shift-jis"))
                    Dim line As String
                    ' Read and display lines from the file until the end of
                    ' the file is reached.
                    Do
                        line = sr.ReadLine()
                        r &= line & vbCrLf
                    Loop Until line Is Nothing
                End Using
            End Using
        Catch ex As Exception
            ' Let the user know what went wrong.
            log1write("The file could not be read:" & ex.Message)
        End Try
        Return r
    End Function

    Public Function ReadAllBytes(ByVal path As String) As Byte()
        Dim bytes As Byte() = Nothing

        Try
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim index As Integer = 0
                Dim filelength As Integer = fs.Length
                Dim count As Integer = filelength
                bytes = New Byte(count - 1) {}
                While count > 0
                    Dim n As Integer = fs.Read(bytes, index, count)
                    If n = 0 Then
                        'Throw New InvalidOperationException("End of file reached before expected")
                        log1write(path & " End of file reached before expected")
                        Exit While
                    End If
                    index += n
                    count -= n
                End While
            End Using

        Catch ex As Exception
            ' Let the user know what went wrong.
            log1write("The file could not be read:" & ex.Message)
        End Try

        Return bytes
    End Function

    Public Function file2line(ByVal filename As String, Optional ByVal encode As String = "shift_jis") As Object
        file2line = Nothing
        '読み込むテキストファイル
        Dim textFile As String = filename
        '文字コード(ここでは、Shift JIS)
        Dim enc As System.Text.Encoding = Nothing
        Try
            enc = System.Text.Encoding.GetEncoding(encode)
        Catch ex As Exception
            ' Writerを破棄する
            If Not enc Is Nothing Then
                Dim cDisposable As System.IDisposable = enc
                cDisposable.Dispose()
            End If
            'エラーが起こったら終了
            log1write(filename & " " & ex.Message)
            Exit Function
        End Try

        '行ごとの配列として、テキストファイルの中身をすべて読み込む
        Dim line As String()
        Try
            line = System.IO.File.ReadAllLines(textFile, enc)
        Catch ex As Exception
            'If Not line Is Nothing Then
            'Dim cDisposable As System.IDisposable = line
            'cDisposable.Dispose()
            'End If
            'エラーが起こったら終了
            log1write(filename & " " & ex.Message)
            Exit Function
        End Try

        Return line
    End Function

    Public Function file_exist(ByVal fn As String) As Integer
        'ファイルの有無を確認する
        Dim r As Integer = -1
        'File.Exists メソッド
        '指定したファイルが存在するかどうかを確認します
        Try
            If System.IO.File.Exists(fn) Then
                'MessageBox.Show("ファイル[" & fn & "]は存在します。")
                r = 1
            Else
                'MessageBox.Show("ファイル[" & fn & "]は存在しません。")
                r = 0
            End If
        Catch ex As Exception
            log1write(fn & " " & ex.Message)
        End Try

        Return r
    End Function

    Public Function file2str(ByVal filename As String, Optional ByVal encode As String = "shift_jis") As String
        Dim s As String = ""
        Try
            Dim sr As New System.IO.StreamReader(filename, System.Text.Encoding.GetEncoding(encode))
            '内容をすべて読み込む
            s = sr.ReadToEnd()
            '閉じる
            sr.Close()
        Catch ex As Exception
            'ファイルオープンエラー
            log1write(filename & " " & ex.Message)
        End Try

        Return s
    End Function

    Public Function str2file(ByVal filename As String, ByVal str As String, Optional ByVal encode As String = "shift_jis") As Integer
        Dim r As Integer = 0
        Try
            Dim sw As New System.IO.StreamWriter(filename, False, System.Text.Encoding.GetEncoding(encode))
            '内容をすべて書き込む
            sw.Write(str)
            '閉じる
            sw.Close()
            r = 1
        Catch ex As Exception
            'ファイルオープンエラー
            log1write(filename & " " & ex.Message)
            r = 0
        End Try

        Return r
    End Function

    Public Function deletefile(ByVal filename As String) As Integer
        Dim r As Integer = 0
        Try
            'ファイルがあれば削除
            If file_exist(filename) = 1 Then
                Dim fi As New System.IO.FileInfo(filename)
                Try
                    fi.Delete()
                    r = 1
                    log1write(filename & "を削除しました")
                Catch ex As Exception
                    log1write(filename & "の削除に失敗しました")
                End Try
            End If
        Catch ex As Exception
            r = 0
            log1write(filename & "の削除に失敗しました[B]")
        End Try

        Return r
    End Function

    Public Function deletefile2(ByVal filename As String) As Integer
        Dim r As Integer = 0
        Dim i As Integer = 30 '3秒間チャレンジ
        Try
            'ファイルがあれば削除 
            If file_exist(filename) = 1 Then
                Dim fi As New System.IO.FileInfo(filename)
                While i > 0 And file_exist(filename) = 1
                    Try
                        fi.Delete()
                        r = 1
                        log1write(filename & "を削除しました")
                        Exit While
                    Catch ex As Exception
                        System.Threading.Thread.Sleep(100)
                    End Try
                    i -= 1
                End While

                If i <= 0 Or file_exist(filename) = 1 Then
                    r = 0
                    log1write(filename & "の削除に失敗しました")
                End If
            End If
        Catch ex As Exception
            r = 0
            log1write(filename & "の削除に失敗しました[B]")
        End Try

        Return r
    End Function

    'フルパスからフォルダを取り出して返す
    Public Function filepath2path(ByVal s As String, Optional ByVal addstr As String = "") As String
        Dim r As String
        Dim sp As Integer = s.LastIndexOf("\")
        If sp > 0 Then
            r = s.Substring(0, sp) & addstr
        Else
            r = ""
        End If
        Return r
    End Function

    'プログラムが存在するディレクトリにカレントディレクトリ変更
    Public Sub F_set_ppath4program()
        Dim ppath As String = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim psp As Integer
        If ppath.Length > 0 Then
            psp = ppath.LastIndexOf("\")
            If psp >= 0 Then
                ppath = ppath.Substring(0, psp + 1)
            Else
                ppath = ""
            End If
        End If
        Try
            System.IO.Directory.SetCurrentDirectory(ppath)
        Catch ex As Exception
            log1write("カレントフォルダの変更に失敗しました。" & ex.Message)
        End Try
    End Sub

End Module
