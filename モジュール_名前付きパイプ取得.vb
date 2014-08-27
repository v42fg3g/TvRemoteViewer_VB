Imports System.Text
Imports System.Runtime.InteropServices

Module モジュール_名前付きパイプ取得
    '====================================================
    '地デジのロケフリシステムを作るスレ part3
    'http://peace.2ch.net/test/read.cgi/avi/1399389478/516
    '>>516さんからご提供いただきました
    '====================================================

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure FILETIME
        Public dwLowDateTime As UInteger
        Public dwHighDateTime As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Private Structure WIN32_FIND_DATA
        Public dwFileAttributes As UInteger
        Public ftCreationTime As FILETIME
        Public ftLastAccessTime As FILETIME
        Public ftLastWriteTime As FILETIME
        Public nFileSizeHigh As UInteger
        Public nFileSizeLow As UInteger
        Public dwReserved0 As UInteger
        Public dwReserved1 As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> _
        Public cFileName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=14)> _
        Public cAlternateFileName As String
    End Structure

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)> _
    Private Function FindFirstFile(ByVal lpFileName As String, ByRef lpFindFileData As WIN32_FIND_DATA) As IntPtr
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)> _
    Private Function FindNextFile(ByVal hFindFile As IntPtr, ByRef lpFindFileData As WIN32_FIND_DATA) As Integer
    End Function
    <DllImport("kernel32.dll")> _
    Private Function FindClose(ByVal hFindFile As IntPtr) As Boolean
    End Function

    Public Function GetPipes() As String()
        Dim pipes As New List(Of String)
        Dim data As New WIN32_FIND_DATA
        Dim handle As IntPtr = FindFirstFile("\\.\pipe\*", data)
        If handle <> New IntPtr(-1) Then
            Do
                pipes.Add(data.cFileName)
                'Console.WriteLine(data.cFileName)
            Loop While FindNextFile(handle, data) <> 0
            FindClose(handle)
        End If
        Return pipes.ToArray
    End Function


    '====================================================
    '外部プログラム使用
    '====================================================

    Public PipeListGetter As String = ""

    'iniで指定されたプログラムを使ってパイプ一覧を取得
    Public Function F_get_NamedPipeList_from_program() As Object
        Dim r() As String = Nothing

        If PipeListGetter.Length > 0 Then
            Dim PipeProgram As String = PipeListGetter
            Dim results As String = ""
            Dim psi As New System.Diagnostics.ProcessStartInfo()

            'ComSpecのパスを取得する
            psi.FileName = System.Environment.GetEnvironmentVariable("ComSpec")
            '出力を読み取れるようにする
            psi.RedirectStandardInput = False
            psi.RedirectStandardOutput = True
            psi.UseShellExecute = False
            'ウィンドウを表示しないようにする
            psi.CreateNoWindow = True

            'プログラムが存在するディレクトリ
            Dim ppath As String = ""
            If PipeProgram.IndexOf(":\") < 0 And PipeProgram.IndexOf("\\") < 0 Then
                '絶対パスで指定されていなければ
                ppath = System.Reflection.Assembly.GetExecutingAssembly().Location
                Dim psp As Integer
                If ppath.Length > 0 Then
                    psp = ppath.LastIndexOf("\")
                    If psp >= 0 Then
                        ppath = ppath.Substring(0, psp + 1)
                    Else
                        ppath = ""
                    End If
                End If
                PipeProgram = ppath & PipeProgram
            Else
                '絶対パスならそのまま
            End If

            If file_exist(PipeProgram) = 1 Then
                log1write("外部プログラムによりパイプ一覧を取得します")

                psi.Arguments = "/c " & """" & PipeProgram & """"

                psi.StandardOutputEncoding = Encoding.UTF8

                '起動
                Dim p As System.Diagnostics.Process
                Try
                    p = System.Diagnostics.Process.Start(psi)
                    '出力を読み取る
                    results = p.StandardOutput.ReadToEnd
                    'WaitForExitはReadToEndの後である必要がある
                    '(親プロセス、子プロセスでブロック防止のため)
                    p.WaitForExit()
                Catch ex As Exception
                End Try

                If results.Length >= 0 Then
                    Dim lines() As String = results.Split(vbCrLf)
                    Dim i As Integer = 0
                    Dim cnt As Integer = 0
                    If lines IsNot Nothing Then
                        For i = 0 To lines.Length - 1
                            Dim line As String = Trim(lines(i))
                            Dim sp As Integer = line.IndexOf(" ")
                            Dim sp2 As Integer = line.IndexOf(vbTab)
                            If sp2 > 0 Then
                                'TABまでがpipe名
                                ReDim Preserve r(cnt)
                                r(cnt) = Trim(line.Substring(0, sp2))
                                cnt += 1
                            ElseIf sp > 0 Then
                                '空白までがpipe名
                                ReDim Preserve r(cnt)
                                r(cnt) = Trim(line.Substring(0, sp))
                                cnt += 1
                            ElseIf line.Length > 1 Then
                                '行全体がpipe名
                                ReDim Preserve r(cnt)
                                r(cnt) = Trim(line)
                                cnt += 1
                            End If
                        Next
                    End If
                End If
            End If
        End If

        Return r
    End Function

End Module
