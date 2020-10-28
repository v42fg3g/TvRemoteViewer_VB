﻿Module モジュール_ログ
    'ログ
    Public log1 As String = ""
    Public log1_dummy As String = log1
    Private log1_delm As String = ""

    'ログの最大文字数
    Public log_size As Integer = 30000

    'ログを書き込む
    Public Sub log1write(ByVal s As String)
        If log_size > 0 Then
            log1 &= log1_delm & Now() & "   " & s
            log1_delm = vbCrLf
        End If
    End Sub

    'アクセスログ
    Public AccessLogList() As accesslogstructure
    Public Structure accesslogstructure
        Implements IComparable
        Public IP As String
        Public domain As String
        Public URL As String
        Public utime As Integer
        Public UserAgent As String
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得
            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.IP = pF Then
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            '並べ替え用
            Return -Me.utime.CompareTo(DirectCast(obj, accesslogstructure).utime)
        End Function
    End Structure

    'アクセスログに記録
    Public AccessLogDays As Integer = 365 '何日間保持するか
    Public Sub SetAccessLog(ByVal IP As String, ByVal URL As String, ByVal UserAgent As String)
        Dim j As Integer = -1
        If AccessLogList IsNot Nothing Then
            j = Array.IndexOf(AccessLogList, IP)
            If j < 0 Then
                For i As Integer = 0 To AccessLogList.Length - 1
                    If time2unix(Now()) - AccessLogList(i).utime > (3600 * 24 * AccessLogDays) Then
                        'AccessLogDays日以上前のデータは上書き
                        j = i
                        AccessLogList(i).IP = ""
                        AccessLogList(i).domain = ""
                        AccessLogList(i).UserAgent = ""
                        AccessLogList(i).URL = ""
                        AccessLogList(i).utime = 0
                    End If
                Next
                If j < 0 Then
                    j = AccessLogList.Length
                    ReDim Preserve AccessLogList(j)
                End If
            End If
        Else
            j = 0
            ReDim Preserve AccessLogList(j)
        End If
        AccessLogList(j).IP = IP
        AccessLogList(j).domain &= ""
        AccessLogList(j).UserAgent = UserAgent
        AccessLogList(j).URL = URL
        AccessLogList(j).utime = time2unix(Now())
    End Sub

    Public Function Analyse_UserAgent(ByVal UA As String) As String
        Dim r As String = ""

        UA &= ""
        If Not String.IsNullOrEmpty(UA) Then
            UA = UA.ToLower

            If UA.IndexOf("windows") >= 0 Then
                r = "Windows"
            ElseIf UA.IndexOf("iphone") >= 0 Or UA.IndexOf("ipod") >= 0 Or UA.IndexOf("ipad") >= 0 Then
                r = "iOS"
            ElseIf UA.IndexOf("android") >= 0 Then
                r = "Android"
            ElseIf UA.IndexOf("os x") >= 0 Or UA.IndexOf("mac") >= 0 Then
                r = "OS X"
            ElseIf UA.IndexOf("linux") >= 0 Or UA.IndexOf("bsd") >= 0 Or UA.IndexOf("sunos") >= 0 Then
                r = "Linux"
            ElseIf UA.IndexOf("nintendo") >= 0 Then
                r = "Nintendo"
            ElseIf UA.IndexOf("playstation") >= 0 Then
                r = "PlayStation"
            ElseIf UA.IndexOf("docomo") >= 0 Then
                r = "docomo"
            ElseIf UA.IndexOf("kddi") >= 0 Then
                r = "AU"
            ElseIf UA.IndexOf("softbank") >= 0 Or UA.IndexOf("j-phone") >= 0 Then
                r = "SoftBank"
            ElseIf UA.IndexOf("willcom") >= 0 Then
                r = "WILLCOM"
            End If
        End If

        If r.Length = 0 And UA.Length > 0 Then
            r = "不明"
        End If

        If r.Length > 0 Then
            r = r & ":    " & UA
        End If
        Return r
    End Function
End Module
