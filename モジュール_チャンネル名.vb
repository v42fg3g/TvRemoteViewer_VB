Module モジュール_チャンネル名
    '放送局
    Public ch_list() As ch_liststructure
    Public Structure ch_liststructure
        Public sid As Integer
        Public jigyousha As String
        Public bondriver As String
        Public chspace As Integer
        Public channel As Integer
        Public tsid As Integer
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
    Public Function F_sid2channelname(ByVal sid As Integer, ByVal chspace As Integer) As String
        Dim r As String = ""
        If ch_list IsNot Nothing Then
            For i As Integer = 0 To ch_list.Length - 1
                If ch_list(i).sid = sid And ch_list(i).chspace = chspace Then
                    r = ch_list(i).jigyousha
                    Exit For
                End If
            Next
        End If

        Return r
    End Function

    'sidからchannelに変換 RecTaskのPipeで命令する際のchannelを取得
    Public Function F_sid2channel(ByVal sid As Integer, ByVal chspace As Integer) As Integer
        Dim r As String = -1
        If ch_list IsNot Nothing Then
            For i As Integer = 0 To ch_list.Length - 1
                If ch_list(i).sid = sid And ch_list(i).chspace = chspace Then
                    r = ch_list(i).channel
                    Exit For
                End If
            Next
        End If

        Return r
    End Function

End Module
