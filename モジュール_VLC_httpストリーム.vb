Module モジュール_VLC_httpストリーム
    'どのhlsアプリを使用するか 1=vlc 2=ffmpeg
    Public HTTPSTREAM_App As Integer = 0

    'Stream_mode=2or3のときに付け加える文字列
    Public HTTPSTREAM_mode2_str As String = "[HS]"

    'WEB上のストリームボタンから直接tsファイルを再生するかどうか
    'androidために必要かなと思ったが今のところＷＥＢからHTTPストリームを開始する機能が付いていないので0
    Public HTTPSTREAM_WEB_VIEW_TS As Integer = 0

    'VLCがHTTPストリーム配信する際に使用する最初のTCPポート
    Public HTTPSTREAM_VLC_port As Integer = 0
    'VLCポートの指定が無い場合、UDPポートナンバーにいくつプラスするか
    Public HTTPSTREAM_VLC_port_plus As Integer = 40

End Module
