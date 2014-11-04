﻿Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports System
Imports System.IO

Module モジュール_ニコニコ実況

    Public jk_list() As sidstructure '放送局
    Public Structure sidstructure
        Public jkid As Integer
        Public nid As Integer
        Public sid As Integer
        Public chiiki As String
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

    Public Function sid2jk(ByVal sid As Integer, ByVal chspace As Integer) As String
        '放送局sidからjkフォルダに変換
        Dim r As String = ""

        '難視聴かCSか　CSなら排除
        Select Case sid
            Case 291, 292, 294, 295, 296, 297, 298
                If chspace = 1 Then sid = 999999999
        End Select

        Try
            Dim i As Integer = Array.IndexOf(jk_list, sid)

            If i >= 0 Then
                Dim b As Integer = jk_list(i).jkid
                If b > 0 Then
                    r = "jk" & b.ToString
                End If
            End If
        Catch ex As Exception
        End Try

        Return r
    End Function

    Public Sub ch_sid_load()
        Dim i As Integer = 0

        If file_exist("ch_sid.txt") = 0 Then
            log1write("【警告】ch_sid.txtが見つかりません。")
        Else
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
                            If youso.Length >= 5 Then
                                ReDim Preserve jk_list(i)
                                jk_list(i).jkid = Val(trim8(youso(0)))
                                jk_list(i).nid = Val(trim8(youso(1)))
                                jk_list(i).sid = h16_10(trim8(youso(2)))
                                jk_list(i).chiiki = trim8(youso(3))
                                jk_list(i).jigyousha = StrConv(trim8(youso(4)), VbStrConv.Wide)
                            End If
                        End If
                    Next
                    log1write("ch_sid.txtからニコニコ実況変換用データを読み込みました")
                End If
            Catch ex As Exception
                log1write("【警告】ch_sid.txtの読み込みに失敗しました。")
            End Try
        End If
    End Sub

    '16進数から10進数へ
    Public Function h16_10(ByVal s As String) As Integer
        Dim r As Integer = 0
        s = trim8(s)

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
            r = Val(trim8(s))
        End If

        Return r
    End Function

    'ニコニコ実況用接続文字列を取得
    Public Function get_nico_jkvalue(ByVal jknum As String) As String
        Dim r As String = ""
        Try
            Dim wc As WebClient = New WebClient()
            Dim st As Stream = wc.OpenRead("http://jk.nicovideo.jp/api/v2/getflv?v=" & jknum)
            Dim enc As Encoding = Encoding.GetEncoding("UTF-8")
            Dim sr As StreamReader = New StreamReader(st, enc)
            r = sr.ReadToEnd()
            wc.Dispose()
        Catch ex As Exception
        End Try

        Return r
    End Function
End Module
