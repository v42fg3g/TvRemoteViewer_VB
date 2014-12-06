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

    'ファイル再生時に指定が無い場合にシークする秒数
    Public VideoSeekDefault As Integer = 0

    '%VIDEOFROMDATE%　変換用
    Public VIDEOFROMDATE As DateTime = CDate("1970/01/01")

    'フォルダ監視用
    Public watcher() As System.IO.FileSystemWatcher = Nothing

    'ファイルリスト閲覧、最初の1回目かどうか
    Public videolist_firstview As Integer = 0

    '最後にファイル一覧に変化があったことを検知した日時
    Public watcher_lasttime As DateTime = C_DAY2038
End Module
