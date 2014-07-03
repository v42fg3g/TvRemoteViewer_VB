Imports System
Imports System.Text
Imports System.Runtime.InteropServices

'VLCのcrashダイアログが表示されているかどうかを調べるためだけに使用

Module モジュール_ウィンドウ関連

    'ウインドウ関係
    Public Declare Function GetDesktopWindow Lib "user32" () As IntPtr
    Public Declare Function GetDC Lib "user32" Alias "GetDC" (ByVal hwnd As IntPtr) As IntPtr
    Public Declare Function GetWindowDC Lib "user32" Alias "GetWindowDC" (ByVal hwnd As IntPtr) As IntPtr
    Public Declare Function ReleaseDC Lib "user32" Alias "ReleaseDC" (ByVal hwnd As IntPtr, ByVal hdc As IntPtr) As Integer
    Private Declare Function CreateCompatibleBitmap Lib "gdi32" Alias "CreateCompatibleBitmap" (ByVal hdc As IntPtr, ByVal nWidth As Integer, ByVal nHeight As Integer) As IntPtr
    Public Declare Function FindWindowEx Lib "user32" Alias "FindWindowExA" (ByVal hWnd1 As Integer, ByVal hWnd2 As Integer, ByVal lpsz1 As String, ByVal lpsz2 As String) As Integer
    'FindWindowの第２引数が壊れるバグがある。
    Public Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Public Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Integer) As Integer
    Public Declare Function GetWindow Lib "user32.dll" (ByVal hWnd As Integer, ByVal uCmd As Integer) As Integer
    Public Declare Function CloseWindow Lib "user32" Alias "CloseWindow" (ByVal hwnd As IntPtr) As Integer
    Public Declare Function GetForegroundWindow Lib "user32" Alias "GetForegroundWindow" () As Integer

    'ウインドウの大きさと位置
    Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure
    Public Declare Function GetWindowRect Lib "user32" Alias "GetWindowRect" (ByVal hwnd As Integer, ByRef lpRect As RECT) As Integer

    '最前面にする API関数の宣言
    <DllImport("user32.dll")> _
    Public Function SetForegroundWindow(hWnd As IntPtr) As _
    <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    ' GetWindowText API関数の宣言
    <DllImport("user32", EntryPoint:="GetWindowText", CharSet:=CharSet.Auto)> _
    Public Function GetWindowText( _
        ByVal hWnd As IntPtr, _
        ByVal lpString As StringBuilder, _
        ByVal nMaxCount As Integer) _
        As Integer
    End Function

    ' IsWindowVisible API関数の宣言
    <DllImport("user32", EntryPoint:="IsWindowVisible")> _
    Public Function IsWindowVisible( _
        ByVal hWnd As IntPtr) _
        As Integer
    End Function

End Module

