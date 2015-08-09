Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Imports Microsoft.Win32.SafeHandles

Class Kernel32PipeManager
    Private _pipeHandle As SafeFileHandle
    Private streamEncoding As UnicodeEncoding

    Public Sub New(pipeIndex_str As String)
        Me._pipeHandle = CreateFile("\\.\pipe\" & pipeIndex_str, DesiredAccess.GENERIC_WRITE, 0, IntPtr.Zero, CreationDisposition.OPEN_EXISTING, 0, _
         IntPtr.Zero)
        streamEncoding = New UnicodeEncoding()
    End Sub

    Public Sub Close()
        'CloseHandle(this._pipeHandle);
        Me._pipeHandle.Close()
        '★ SEHException 対策として CloseHandle を呼ばないで、Close を呼ぶ
        'http://8thway.blogspot.jp/2012/11/csharp-ssd.html
    End Sub

    Public Function ReadString() As String
        Dim ret As Boolean
        Dim rb As UInteger = 0

        '★ret := ReadFile(hPipe, size, sizeOf(DWORD), rb, nil);
        Dim rsize As Byte()
        ret = ReadFile(Me._pipeHandle, rsize, 4, rb, IntPtr.Zero)
        System.Console.WriteLine("debug:ReadFile1")
        System.Console.WriteLine("debug:" & rb)
        System.Console.WriteLine("debug:" & ret)
        If ret <> True OrElse rb <= 0 Then
            Return Nothing
        End If

        Dim recv As Byte()
        '★ret := ReadFile(hPipe, Recv[1], size, rb, nil);
        ret = ReadFile(Me._pipeHandle, recv, 20, rb, IntPtr.Zero)
        System.Console.WriteLine("debug:ReadFile2")
        System.Console.WriteLine("debug:" & rb)
        System.Console.WriteLine("debug:" & ret)
        If ret <> True OrElse rb <= 0 Then
            Return Nothing
        End If

        Dim temp As String = streamEncoding.GetString(recv)
        Return temp
    End Function

    Public Sub WriteString(sendMsg As String)
        If Not Me._pipeHandle.IsInvalid Then
            Dim ret As Boolean
            Dim wb As UInteger = 0

            Dim msg As Byte() = streamEncoding.GetBytes(sendMsg)

            Dim wsize As Byte() = New Byte(3) {CByte(msg.Length), 0, 0, 0}
            Dim sendSize As UInteger = CUInt(msg.Length)

            '★ret := WriteFile(hPipe, size, sizeof(DWORD), wb, nil);
            ret = WriteFile(Me._pipeHandle, wsize, 4, wb, IntPtr.Zero)
            System.Console.WriteLine("debug:WriteFile1")
            System.Console.WriteLine("debug:" & wb)
            System.Console.WriteLine("debug:" & ret)
            If ret <> True OrElse wb <= 0 Then
                Return
            End If

            '★ret := WriteFile(hPipe, Msg[1], size, wb, nil);
            ret = WriteFile(Me._pipeHandle, msg, sendSize, wb, IntPtr.Zero)
            System.Console.WriteLine("debug:WriteFile2")
            System.Console.WriteLine("debug:" & wb)
            System.Console.WriteLine("debug:" & ret)
            If ret <> True OrElse wb <= 0 Then
                Return
            End If
        End If
    End Sub

    ' ---------------------------------------------------------
    '         * WINAPI STUFF
    '         * ------------------------------------------------------ 


    Private Sub ThrowLastWin32Err()
        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error())
    End Sub

    <Flags()> _
    Public Enum DesiredAccess As UInteger
        GENERIC_READ = &H80000000UI
        GENERIC_WRITE = &H40000000
        GENERIC_READ_WRITE = &HC0000000UI
    End Enum
    <Flags()> _
    Public Enum ShareMode As UInteger
        FILE_SHARE_NONE = &H0
        FILE_SHARE_READ = &H1
        FILE_SHARE_WRITE = &H2
        FILE_SHARE_DELETE = &H4

    End Enum
    Public Enum MoveMethod As UInteger
        FILE_BEGIN = 0
        FILE_CURRENT = 1
        FILE_END = 2
    End Enum
    Public Enum CreationDisposition As UInteger
        CREATE_NEW = 1
        CREATE_ALWAYS = 2
        OPEN_EXISTING = 3
        OPEN_ALWAYS = 4
        TRUNCATE_EXSTING = 5
    End Enum
    <Flags()> _
    Public Enum FlagsAndAttributes As UInteger
        FILE_ATTRIBUTES_ARCHIVE = &H20
        FILE_ATTRIBUTE_HIDDEN = &H2
        FILE_ATTRIBUTE_NORMAL = &H80
        FILE_ATTRIBUTE_OFFLINE = &H1000
        FILE_ATTRIBUTE_READONLY = &H1
        FILE_ATTRIBUTE_SYSTEM = &H4
        FILE_ATTRIBUTE_TEMPORARY = &H100
        FILE_FLAG_WRITE_THROUGH = &H80000000UI
        FILE_FLAG_OVERLAPPED = &H40000000
        FILE_FLAG_NO_BUFFERING = &H20000000
        FILE_FLAG_RANDOM_ACCESS = &H10000000
        FILE_FLAG_SEQUENTIAL_SCAN = &H8000000
        FILE_FLAG_DELETE_ON = &H4000000
        FILE_FLAG_POSIX_SEMANTICS = &H1000000
        FILE_FLAG_OPEN_REPARSE_POINT = &H200000
        FILE_FLAG_OPEN_NO_CALL = &H100000
    End Enum

    Public Const INVALID_HANDLE_VALUE As UInteger = &HFFFFFFFFUI
    Public Const INVALID_SET_FILE_POINTER As UInteger = &HFFFFFFFFUI
    ' Use interop to call the CreateFile function.
    ' For more information about CreateFile,
    ' see the unmanaged MSDN reference library.
    <System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError:=True)> _
    Friend Shared Function CreateFile(lpFileName As String, dwDesiredAccess As DesiredAccess, dwShareMode As ShareMode, lpSecurityAttributes As IntPtr, dwCreationDisposition As CreationDisposition, dwFlagsAndAttributes As FlagsAndAttributes, _
   hTemplateFile As IntPtr) As Microsoft.Win32.SafeHandles.SafeFileHandle
    End Function

    <System.Runtime.InteropServices.DllImport("kernel32", SetLastError:=True)> _
    Friend Shared Function CloseHandle(hObject As Microsoft.Win32.SafeHandles.SafeFileHandle) As Int32
    End Function

    <System.Runtime.InteropServices.DllImport("kernel32", SetLastError:=True)> _
    Friend Shared Function ReadFile(hFile As Microsoft.Win32.SafeHandles.SafeFileHandle, ByRef aBuffer As [Byte](), cbToRead As UInt32, ByRef cbThatWereRead As UInt32, pOverlapped As IntPtr) As Boolean
    End Function

    <System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError:=True)> _
    Friend Shared Function WriteFile(hFile As Microsoft.Win32.SafeHandles.SafeFileHandle, aBuffer As [Byte](), cbToWrite As UInt32, ByRef cbThatWereWritten As UInt32, pOverlapped As IntPtr) As Boolean
    End Function
End Class
