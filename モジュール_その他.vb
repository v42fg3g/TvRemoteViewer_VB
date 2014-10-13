Module モジュール_その他
    '最大配信ナンバー
    Public MAX_STREAM_NUMBER As Integer = 8

    'アプリCPU優先度
    Public UDP_PRIORITY As String = "" 'High
    Public HLS_PRIORITY As String = ""

    'TvRemoteViewer_VBの起動時、終了時、全停止時にRecTaskを名前付きで停止するかどうか
    Public Stop_RecTask_at_StartEnd As Integer = 1

    '余計な改行等を削除
    Public Function trim8(ByVal s As String) As String
        s = Trim(s)
        s = s.Replace(vbTab, "").Replace(vbCrLf, "").Replace("""", "")
        s = Trim(s)
        Return s
    End Function

    'パラメーター値を取得 例：instr_pickup_para("/ch 9","/ch "," ", 0) → 9
    Public Function instr_pickup_para(ByVal s As String, ByVal s1 As String, ByVal s2 As String, ByVal sp As Integer) As String
        Dim r As String = ""
        Try
            Dim a1 As Integer = s.IndexOf(s1, sp)
            Dim a2 As Integer = -1
            Try
                a2 = s.IndexOf(s2, a1 + s1.Length)
            Catch ex As Exception
                a2 = -1
            End Try
            If a1 >= 0 And a2 > a1 Then
                r = s.Substring(a1 + s1.Length, a2 - a1 - s1.Length)
            ElseIf a1 >= 0 And a2 < 0 Then
                '2番目の識別子が見つからない場合
                r = s.Substring(a1 + s1.Length)
            End If
        Catch ex As Exception
        End Try
        Return r
    End Function

End Module

