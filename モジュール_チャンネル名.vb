Module モジュール_チャンネル名
    '放送局
    Public ch_list() As ch_liststructure
    Public Structure ch_liststructure
        Public sid As Integer
        Public jigyousha As String
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得

            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.sid = pF Then
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    'sidからチャンネル名に変換
    Public Function F_sid2channelname(ByVal sid As Integer) As String
        Dim r As String = ""

        Dim i As Integer = Array.IndexOf(ch_list, sid)
        If i >= 0 Then
            r = ch_list(i).jigyousha
        End If

        Return r
    End Function

End Module
