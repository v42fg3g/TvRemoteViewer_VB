Module モジュール_ビデオファイル一覧
    Public video() As videostructure
    Public Structure videostructure
        Implements IComparable
        Public fullpathfilename As String
        Public filename As String
        Public encstr As String 'エンコード済フルパス
        Public modifytime As DateTime
        Public datestr As String 'yyyyMMdd
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            '並べ替え用
            Return -Me.datestr.CompareTo(DirectCast(obj, videostructure).datestr)
        End Function
    End Structure

    '%VIDEOFROMDATE%　変換用
    Public VIDEOFROMDATE As DateTime = CDate("1970/01/01")

    'フォルダ監視用
    Public watcher() As System.IO.FileSystemWatcher = Nothing
    'ただいまファイル取得中 =1
    Public watcher_now As Integer = 0

End Module
