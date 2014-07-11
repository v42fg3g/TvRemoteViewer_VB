Module モジュール_HTMLキャッシュ
    'htmlキャッシュ用
    'index.html内 %SELECTBONSIDCH%用 WEB_make_select_Bondriver_html()で作成されたhtml
    Public html_selectbonsidch_a As String = ""
    Public html_selectbonsidch_b As String = ""

    'htmlキャッシュ用
    'bondriver毎の<select><option>
    Public BonDriver_select_html() As bh_structure
    Public Structure bh_structure
        Public BonDriver As String
        Public html As String
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

End Module
