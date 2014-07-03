Module モジュール_ログ
    'ログ
    Public log1 As String = ""
    Public log1_dummy As String = log1

    'ログを書き込む
    Public Sub log1write(ByVal s As String)
        log1 = Now() & "  " & s & vbCrLf & log1
    End Sub

End Module
