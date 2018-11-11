Imports System
Imports System.IO

Module モジュール_HTMLキャッシュ
    'htmlキャッシュ用
    'index.html内 %SELECTBONSIDCH%用 WEB_make_select_Bondriver_html()で作成されたhtml
    Public html_selectbonsidch_a As String = ""
    Public html_selectbonsidch_b As String = ""
    Public bondriver_sort() As String = Nothing 'BonDriver並び替え用

    'htmlキャッシュ用
    'bondriver毎の<select><option>
    Public BonDriver_select_html() As bh_structure
    Public Structure bh_structure
        Public BonDriver As String
        Public html As String
        Public ichiran As String
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            'indexof用
            Dim pF As String = CType(obj, String) '検索内容を取得
            If pF = "" Then '空白である場合
                Return False '対象外
            Else
                If Me.BonDriver = pF Then
                    Return True '一致した
                Else
                    Return False '一致しない
                End If
            End If
        End Function
    End Structure

    'BonDriver一覧取得
    Public Function get_and_sort_BonDrivers(ByVal bondriver_folder As String) As String()
        Dim r() As String = Nothing

        Dim j As Integer = 0
        Try
            For Each stFilePath As String In System.IO.Directory.GetFiles(bondriver_folder, "*.dll")
                If System.IO.Path.GetExtension(stFilePath).ToLower = ".dll" Then
                    Dim s As String = stFilePath
                    'フルパスファイル名がsに入る
                    'ファイル名だけを取り出す
                    s = Path.GetFileName(s)
                    Dim sl As String = s.ToLower() '小文字に変換
                    '表示しないBonDriverかをチェック
                    If BonDriver_NGword IsNot Nothing Then
                        For i As Integer = 0 To BonDriver_NGword.Length - 1
                            If sl.IndexOf(BonDriver_NGword(i).ToLower) >= 0 Then
                                sl = ""
                            End If
                        Next
                    End If
                    If sl.IndexOf("bondriver") = 0 Then
                        ReDim Preserve r(j)
                        r(j) = s
                        j += 1
                    End If
                End If
            Next
        Catch ex As Exception
        End Try

        '並び替え
        r = F_bons_sort(r)

        Return r
    End Function

    'BonDriver並び替え
    Private Function F_bons_sort(ByVal bons() As String) As String()
        Dim r() As String = Nothing
        Dim k As Integer = 0

        If bons IsNot Nothing Then
            'bondriver_sort()に従って並べ替え
            If bondriver_sort IsNot Nothing Then
                For i As Integer = 0 To bondriver_sort.Length - 1
                    For j As Integer = 0 To bons.Length - 1
                        If Trim(bons(j)).Length > 0 Then
                            If bondriver_sort(i).ToLower = bons(j).ToLower Then
                                ReDim Preserve r(k)
                                r(k) = bons(j)
                                bons(j) = ""
                                k += 1
                            End If
                        End If
                    Next
                Next
            End If
            '並べ替え指定がなかったBonDriverを追加する
            For j As Integer = 0 To bons.Length - 1
                If Trim(bons(j)).Length > 0 Then
                    ReDim Preserve r(k)
                    r(k) = bons(j)
                    k += 1
                End If
            Next
        End If

        Return r
    End Function
End Module
