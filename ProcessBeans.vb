Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Diagnostics

Class ProcessBean
    Private _udpProc As Process = Nothing
    Private _udpPipeId As Integer = 0
    Private _hlsProc As Process = Nothing
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
    'Public _m3u8_update_time As Date 'm3u8の更新日時

    Public Sub New(udpProc As Process, hlsProc As Process, procBrowserIndex As Integer, udpPipeId As Integer, udpApp As String, udpOpt As String, hlsApp As String, hlsOpt As String, udpPort As Integer, ShowConsole As Boolean, resolution As String)
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
        'Me._m3u8_update_time = Now()
    End Sub

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
