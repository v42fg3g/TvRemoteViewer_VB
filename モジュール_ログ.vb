Module モジュール_ログ
    'ログ
    Public log1 As String = ""
    Public log1_dummy As String = log1

    'ログの最大文字数
    Public log_size As Integer = 30000

    'ログを書き込む
    Public Sub log1write(ByVal s As String)
        If log_size > 0 Then
            log1 = Now() & "  " & s & vbCrLf & log1
        End If
    End Sub

End Module
