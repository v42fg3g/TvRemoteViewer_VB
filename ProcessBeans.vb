Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Diagnostics
Imports System.Net
Imports System.IO
Imports System.Threading

Class ProcessBean
    Private _udpProc As Process = Nothing
    Private _udpPipeId As Integer = 0
    Public _hlsProc As Process = Nothing
    Private _procBrowserIndex As Integer = 0
    Public _udpApp As String = ""
    Public _udpOpt As String = ""
    Public _hlsApp As String = ""
    Public _hlsOpt As String = ""
    Public _num As Integer = 0
    Public _chk_proc As Integer = 0
    Public _udpPort As Integer = 0
    Public _stopping As Integer = 0 '停止を試みているときは1
    Public _ShowConsole As Boolean = False
    Public _resolution As String = ""
    Public _stream_mode As Integer = 0 '0=udp 1=file
    'Public _m3u8_update_time As Date 'm3u8の更新日時
    Public _NHK_dual_mono_mode_select As Integer
    'ffmpeg HTTPストリーム
    Public _IsStart As Boolean ' = False
    Public _ffmpegBuf As Byte() '= Nothing        'Public opt As String 'VLCオプション文字列

    Public Sub New(udpProc As Process, hlsProc As Process, procBrowserIndex As Integer, udpPipeId As Integer, udpApp As String, udpOpt As String, hlsApp As String, hlsOpt As String, udpPort As Integer, ShowConsole As Boolean, Stream_mode As Integer, NHK_dual_mono_mode_select As Integer, resolution As String)
        Me._udpProc = udpProc
        Me._hlsProc = hlsProc
        Me._procBrowserIndex = procBrowserIndex
        Me._udpPipeId = udpPipeId
        Me._udpApp = udpApp
        Me._udpOpt = udpOpt
        Me._hlsApp = hlsApp
        Me._hlsOpt = hlsOpt
        Me._num = procBrowserIndex
        Me._chk_proc = 0
        Me._udpPort = udpPort
        Me._stopping = 0
        Me._ShowConsole = ShowConsole
        Me._resolution = resolution
        Me._stream_mode = Stream_mode
        'Me._m3u8_update_time = Now()
        Me._NHK_dual_mono_mode_select = NHK_dual_mono_mode_select

        'ffmpeg HTTPストリーム
        Me._IsStart = False
        Me._ffmpegBuf = New Byte(88 * 20480 - 1) {}       '2048 Packet
    End Sub

    '========================================================================

    'ffmpeg HTTPストリーム　スタート
    Public Function ffmpeg_http_stream_Start(output As Stream) As Integer
        Dim r As Integer = 0

        Dim ffmpegApp As String = Me._hlsApp
        Dim ffmpegOpt As String = Me._hlsOpt

        Try
            If Me._IsStart Then
                log1write("すでに配信中です")
                Return r
                Exit Function
            End If
            Me._IsStart = True

            'ffmpegスタート
            Dim ffmpegStart As New System.Diagnostics.ProcessStartInfo() '("ffmpeg")
            ffmpegStart.FileName = ffmpegApp
            ffmpegStart.Arguments = ffmpegOpt
            ffmpegStart.RedirectStandardOutput = True
            ffmpegStart.UseShellExecute = False
            ffmpegStart.CreateNoWindow = True 'False

            'Dim ffmpeg As System.Diagnostics.Process = System.Diagnostics.Process.Start(ffmpegStart)
            Me._hlsProc = System.Diagnostics.Process.Start(ffmpegStart)

            Thread.Sleep(900)
            Me._hlsProc.StandardOutput.BaseStream.BeginRead(Me._ffmpegBuf, 0, Me._ffmpegBuf.Length, AddressOf StreamReceive, output)
            r = 1 '成功
            Me._stopping = 0 '3に設定したあったものを0に戻す
            'ログ表示
            log1write("No.=" & Me._num & "HLS アプリ=" & ffmpegApp)
            log1write("No.=" & Me._num & "HLS option=" & ffmpegOpt)
        Catch ex As Exception
            Me._IsStart = False
            log1write("ffmpeg HTTPストリームの開始に失敗しました。" & ex.Message)
        End Try

        Return r
    End Function

    'ffmpeg HTTPストリーム　ストップ
    Public Function ffmpeg_http_stream_Stop() As Integer
        Dim r As Integer = 0
        Try
            Dim ffmpeg As Process = Me.GetHlsProc '稼働中プロセスを取得
            If Not Me._IsStart Then
                'そもそもスタートしていない
                r = 1
                Return r
            End If
            If ffmpeg Is Nothing Then
                'プロセスが無い
                r = 1
                Return r
            End If
            Me._IsStart = False
            ffmpeg.Kill()
            log1write("No." & Me._num & "のffmpeg(HTTPストリーム)を終了しました")
            r = 1
        Catch ex As Exception
            Me._IsStart = False
            log1write("No." & Me._num & "のffmpeg(HTTPストリーム)の終了に失敗しました" & ex.Message)
        End Try

        Return r
    End Function

    'ffmpeg HTTPストリーム　受信
    Public Sub StreamReceive(Result As IAsyncResult)
        Try
            Dim size As Integer = Me._hlsProc.StandardOutput.BaseStream.EndRead(Result)
            Dim output As Stream = DirectCast(Result.AsyncState, Stream)
            If output IsNot Nothing Then
                output.Write(Me._ffmpegBuf, 0, size)
            End If
            Me._hlsProc.StandardOutput.BaseStream.BeginRead(Me._ffmpegBuf, 0, Me._ffmpegBuf.Length, AddressOf StreamReceive, output)
        Catch ex As Exception
            'クライアントから切断されるとここでエラー
            Me._IsStart = False
            'ffmpeg_http_stream_Stop() 'Me._stopping = 9とすることでタイマーでアプリごと終了させる
            log1write("ffmpeg HTTPストリーム受信に失敗しました。" & ex.Message)
            'ストップした旨を知らせる
            log1write("チャンネル変更が行われない場合は" & FFMPEG_HTTP_CUT_SECONDS & "秒後に配信を終了します。")
            Me._stopping = 100 + FFMPEG_HTTP_CUT_SECONDS 'チャンネル変更ならば数秒以内に処理されるかな。100になるFFMPEG_HTTP_CUT_SECONDS秒後にタイマーにより配信は停止される
        End Try
    End Sub

    '========================================================================

    Public Function GetUdpProc() As Process
        Return Me._udpProc
    End Function

    Public Function GetHlsProc() As Process
        Return Me._hlsProc
    End Function

    Public Function GetProcBrowserIndex() As Integer
        Return Me._procBrowserIndex
    End Function

    Public Function GetProcUdpPipeIndex() As Integer
        Return Me._udpPipeId
    End Function
End Class
