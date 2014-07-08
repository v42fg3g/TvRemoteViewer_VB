Module モジュール_チャンネル名
    '放送局
    Public ch_list() As sidstructure
    Public Structure sidstructure
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

    Public Sub ch_sid_load()
        Dim i As Integer = 0

        If file_exist("ch_sid.txt") = 0 Then
            log1write("ch_sid.txtが見つかりません。")
            '終了
            Exit Sub
        End If

        Try
            Dim line() As String = file2line("ch_sid.txt")
            If line Is Nothing Then
            Else
                For i = 0 To line.Length - 1
                    If line(i).IndexOf(";") >= 0 Then
                        line(i) = line(i).Substring(0, line(i).IndexOf(";"))
                    End If
                    If line(i).IndexOf("#") >= 0 Then
                        line(i) = line(i).Substring(0, line(i).IndexOf("#"))
                    End If
                    Dim youso() As String = line(i).Split(vbTab)
                    If youso Is Nothing Then
                    Else
                        If youso.Length = 5 Then
                            ReDim Preserve ch_list(i)
                            ch_list(i).sid = h16_10(Trim(youso(2)))
                            ch_list(i).jigyousha = StrConv(Trim(youso(4)), VbStrConv.Wide)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            log1write("ch_sid.txtの読み込みに失敗しました。")
            Exit Sub
        End Try
    End Sub

    Public Function h16_10(ByVal s As String) As Integer
        Dim r As Integer = 0
        s = Trim(s)

        If s.IndexOf("0x") = 0 Then
            Try
                's = s.Substring(2)
                If s.Length > 0 Then
                    r = Convert.ToInt32(s, 16)
                Else
                    r = -1
                End If
            Catch ex As Exception
                r = -1
            End Try
        Else
            r = Val(Trim(s))
        End If

        Return r
    End Function

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
