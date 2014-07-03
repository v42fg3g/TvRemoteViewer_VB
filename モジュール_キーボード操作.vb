Imports System.Runtime.InteropServices
Imports System.Text

'VLCのcrashダイアログを消すためだけに使用

Module モジュール_キーボード操作
    <DllImport("user32.dll")> _
    Public Function SendInput(ByVal nInputs As Integer, ByRef pInputs As INPUT, ByVal cbSize As Integer) As Integer
    End Function

    'for KEYBDINPUT Structure
    Public Enum dwFlags As Integer
        KEYEVENTF_EXTENDEDKEY = &H1
        KEYEVENTF_KEYUP = &H2
        KEYEVENTF_SCANCODE = &H8
        KEYEVENTF_UNICODE = &H4
    End Enum

    <StructLayout(LayoutKind.Explicit)> _
    Public Structure INPUT
        <FieldOffset(0)> Dim type As Integer
        <FieldOffset(4)> Dim mi As MOUSEINPUT
        <FieldOffset(4)> Dim ki As KEYBDINPUT
        '<FieldOffset(4)> Dim hi As HARDWAREINPUT
    End Structure

    <StructLayout(LayoutKind.Explicit)> _
    Public Structure MOUSEINPUT
        <FieldOffset(0)> Public dx As Integer
        <FieldOffset(4)> Public dy As Integer
        <FieldOffset(8)> Public mouseData As Integer
        <FieldOffset(12)> Public dwFlags As Integer
        <FieldOffset(16)> Public time As Integer
        <FieldOffset(20)> Public dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Explicit)> _
    Public Structure KEYBDINPUT
        <FieldOffset(0)> Public wVk As wVk
        <FieldOffset(2)> Public wScan As Short
        <FieldOffset(4)> Public dwFlags As dwFlags
        <FieldOffset(8)> Public time As Integer
        <FieldOffset(12)> Public dwExtraInfo As IntPtr
    End Structure

    '<StructLayout(LayoutKind.Explicit)> _
    'Public Structure HARDWAREINPUT
    '<FieldOffset(0)> Public uMsg As Integer
    '<FieldOffset(4)> Public wParamL As Short
    '<FieldOffset(6)> Public wParamH As Short
    'End Structure

    Public Const INPUT_MOUSE As Object = 0
    Public Const INPUT_KEYBOARD As Object = 1
    Public Const INPUT_HARDWARE As Object = 2

    Public Const MOUSE_MOVED As Object = &H1
    Public Const MOUSEEVENTF_ABSOLUTE As Object = &H8000& '　absolute move
    Public Const MOUSEEVENTF_XDOWN As Object = &H100
    Public Const MOUSEEVENTF_XUP As Object = &H200
    Public Const MOUSEEVENTF_WHEEL As Object = &H80
    Public Const MOUSEEVENTF_LEFTUP As Object = &H4   '左ボタンUP
    Public Const MOUSEEVENTF_LEFTDOWN As Object = &H2  '左ボタンDown
    Public Const MOUSEEVENTF_MIDDLEDOWN As Object = &H20 '中央ボタンDown
    Public Const MOUSEEVENTF_MIDDLEUP As Object = &H40  '中央ボタンUP
    Public Const MOUSEEVENTF_RIGHTDOWN As Object = &H8  '右ボタンDown
    Public Const MOUSEEVENTF_RIGHTUP As Object = &H10  '右ボタンUP

    Public Function ExtendedKeyFlagW(ByVal Key As wVk) As dwFlags
        Dim Flag As dwFlags = 0
        Select Case Key
            Case wVk.VK_CANCEL, wVk.VK_PRIOR, wVk.VK_NEXT, wVk.VK_END, wVk.VK_HOME, _
                 wVk.VK_LEFT, wVk.VK_UP, wVk.VK_RIGHT, wVk.VK_DOWN, _
                 wVk.VK_SNAPSHOT, wVk.VK_INSERT, wVk.VK_DELETE, _
                 wVk.VK_DEVIDE, wVk.VK_NUMLOCK, wVk.VK_RSHIFT, wVk.VK_RCONTROL, wVk.VK_RMENU
                Flag = dwFlags.KEYEVENTF_EXTENDEDKEY
        End Select
        Return Flag
    End Function

    'GetMessageExtraInfo Function
    'http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/messagesandmessagequeues/messagesandmessagequeuesreference/messagesandmessagequeuesfunctions/getmessageextrainfo.asp
    <DllImport("user32.dll")> _
    Public Function GetMessageExtraInfo() _
        As IntPtr
    End Function

    'MapVirtualKey Function
    'http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/mapvirtualkey.asp
    <DllImport("user32.dll")> _
    Public Function MapVirtualKey( _
        ByVal uCode As Integer, _
        ByVal uMapType As Integer) _
        As Integer
    End Function

    Public Enum wVk As Short
        VK_CANCEL = &H3 'BREAK(Control+Pause) key(ExtendedKey)
        VK_BACK = &H8 'BACKSPACE key
        VK_TAB = &H9 'TAB key
        VK_CLEAR = &HC
        VK_RETURN = &HD 'ENTER key
        VK_SHIFT = &H10 'SHIFT key
        VK_CONTROL = &H11 'CTRL key
        VK_MENU = &H12 'ALT key
        VK_PAUSE = &H13 'PAUSE key
        VK_CAPITAL = &H14 'CAPS LOCK key
        VK_KANA = &H15 'IME かな mode
        VK_JUNJA = &H17
        VK_FINAL = &H18
        VK_KANJI = &H19 'IME 漢字 mode
        VK_ESCAPE = &H1B 'ESC key
        VK_CONVERT = &H1C 'IME 変換 key
        VK_NONCONVERT = &H1D 'IME 無変換 key
        VK_ACCEPT = &H1E
        VK_MODECHANGE = &H1F
        VK_SPACE = &H20 'SPACEBAR
        VK_PRIOR = &H21 'PAGE UP key(ExtendedKey)
        VK_NEXT = &H22 'PAGE DOWN key(ExtendedKey)
        VK_END = &H23 'END key(ExtendedKey)
        VK_HOME = &H24 'HOME key(ExtendedKey)
        VK_LEFT = &H25 '← key(ExtendedKey)
        VK_UP = &H26 '↑ key(ExtendedKey)
        VK_RIGHT = &H27 '→ key(ExtendedKey)
        VK_DOWN = &H28 '↓ key(ExtendedKey)
        VK_SELECT = &H29
        VK_PRINT = &H2A
        VK_EXECUTE = &H2B
        VK_SNAPSHOT = &H2C 'PRINT SCREEN key(ExtendedKey)
        VK_INSERT = &H2D 'INS key(ExtendedKey)
        VK_DELETE = &H2E 'DEL key(ExtendedKey)
        VK_HELP = &H2F
        VK_0 = &H30 '0 key
        VK_1 = &H31 '1 key
        VK_2 = &H32 '2 key
        VK_3 = &H33 '3 key
        VK_4 = &H34 '4 key
        VK_5 = &H35 '5 key
        VK_6 = &H36 '6 key
        VK_7 = &H37 '7 key
        VK_8 = &H38 '8 key
        VK_9 = &H39 '9 key
        VK_A = &H41 'A key
        VK_B = &H42 'B key
        VK_C = &H43 'C key
        VK_D = &H44 'D key
        VK_E = &H45 'E key
        VK_F = &H46 'F key
        VK_G = &H47 'G key
        VK_H = &H48 'H key
        VK_I = &H49 'I key
        VK_J = &H4A 'J key
        VK_K = &H4B 'K key
        VK_L = &H4C 'L key
        VK_M = &H4D 'M key
        VK_N = &H4E 'N key
        VK_O = &H4F 'O key
        VK_P = &H50 'P key
        VK_Q = &H51 'Q key
        VK_R = &H52 'R key
        VK_S = &H53 'S key
        VK_T = &H54 'T key
        VK_U = &H55 'U key
        VK_V = &H56 'V key
        VK_W = &H57 'W key
        VK_X = &H58 'X key
        VK_Y = &H59 'Y key
        VK_Z = &H5A 'Z key
        VK_LWIN = &H5B 'Left Windows key
        VK_RWIN = &H5C 'Right Windows key
        VK_APPS = &H5D ' Applications key
        VK_NUMPAD0 = &H60 'Numeric keypad 0 key
        VK_NUMPAD1 = &H61 'Numeric keypad 1 key
        VK_NUMPAD2 = &H62 'Numeric keypad 2 key
        VK_NUMPAD3 = &H63 'Numeric keypad 3 key
        VK_NUMPAD4 = &H64 'Numeric keypad 4 key
        VK_NUMPAD5 = &H65 'Numeric keypad 5 key
        VK_NUMPAD6 = &H66 'Numeric keypad 6 key
        VK_NUMPAD7 = &H67 'Numeric keypad 7 key
        VK_NUMPAD8 = &H68 'Numeric keypad 8 key
        VK_NUMPAD9 = &H69 'Numeric keypad 9 key
        VK_MULTIPLY = &H6A '* key
        VK_ADD = &H6B '+ key
        VK_SEPERATOR = &H6C '
        VK_SUBTRACT = &H6D '- key
        VK_DECIMAL = &H6E 'テンキーの . key
        VK_DEVIDE = &H6F '/ key(ExtendedKey)
        VK_F1 = &H70 'F1 key
        VK_F2 = &H71 'F2 key
        VK_F3 = &H72 'F3 key
        VK_F4 = &H73 'F4 key
        VK_F5 = &H74 'F5 key
        VK_F6 = &H75 'F6 key
        VK_F7 = &H76 'F7 key
        VK_F8 = &H77 'F8 key
        VK_F9 = &H78 'F9 key
        VK_F10 = &H79 'F10 key
        VK_F11 = &H7A 'F11 key
        VK_F12 = &H7B 'F12 key
        VK_F13 = &H7C 'F13 key
        VK_F14 = &H7D 'F14 key
        VK_F15 = &H7E 'F15 key
        VK_F16 = &H7F 'F16 key
        VK_F17 = &H80 'F17 key
        VK_F18 = &H81 'F18 key
        VK_F19 = &H82 'F19 key
        VK_F20 = &H83 'F20 key
        VK_F21 = &H84 'F21 key
        VK_F22 = &H85 'F22 key
        VK_F23 = &H86 'F23 key
        VK_F24 = &H87 'F24 key
        VK_NUMLOCK = &H90 'NUM LOCK key(ExtendedKey)
        VK_SCROLL = &H91 'SCROLL LOCK key
        VK_LSHIFT = &HA0 'Left SHIFT key
        VK_RSHIFT = &HA1 ' Right SHIFT key(ExtendedKey)
        VK_LCONTROL = &HA2 'Left CONTROL key
        VK_RCONTROL = &HA3 'Right CONTROL key(ExtendedKey)
        VK_LMENU = &HA4 'Left MENU key
        VK_RMENU = &HA5 'Right MENU key(ExtendedKey)
        VK_BROWSER_BACK = &HA6 'Browser Back key
        VK_BROWSER_FORWARD = &HA7 'Browser Forward key
        VK_BROWSER_REFRESH = &HA8 'Browser Refresh key
        VK_BROWSER_STOP = &HA9 'Browser Stop key
        VK_BROWSER_SEARCH = &HAA 'Browser Search key
        VK_BROWSER_FAVORITES = &HAB 'Browser Favorites key
        VK_BROWSER_HOME = &HA6 'Browser Start and Home key
        VK_VOLUME_MUTE = &HAD 'Volume Mute key
        VK_VOLUME_DOWN = &HAE 'Volume Down key
        VK_VOLUME_UP = &HAF 'Volume Up key
        VK_MEDIA_NEXT_TRACK = &HB0 'Next Track key
        VK_MEDIA_PREV_TRACK = &HB1 'Previous Track key
        VK_MEDIA_STOP = &HB2 'Stop Media key
        VK_MEDIA_PLAY_PAUSE = &HB3 'Play/Pause Media key
        VK_LAUNCH_MAIL = &HB4 'Start Mail key
        VK_LAUNCH_MEDIA_SELECT = &HB5 'Select Media Key
        VK_LAUNCH_APP1 = &HB6 'Start Application 1 key
        VK_LAUNCH_APP2 = &HB7 'Start Application 2 key
        VK_OEM_1 = &HBA ': *  key
        VK_OEM_PLUS = &HBB '; + key
        VK_OEM_COMMA = &HBC ', < key
        VK_OEM_MINUS = &HBD '- = key
        VK_OEM_PERIOD = &HBE '. > key
        VK_OEM_2 = &HBF '/ ? key
        VK_OEM_3 = &HC0 '@ ` key
        VK_OEM_4 = &HDB '[ { key
        VK_OEM_5 = &HDC '\ | key
        VK_OEM_6 = &HDD '] } key
        VK_OEM_7 = &HDE '^ ~ key
        VK_OEM_8 = &HDF
        VK_PROCESSKEY = &HE5
        VK_OEM_ATTN = &HF0 '英数
        VK_OEM_102 = &HE2 '\ _ key
        VK_OEM_COPY = &HF2 'カタカナひらがな
        VK_OEM_AUTO = &HF3 '全角/半角
        VK_OEM_ENLW = &HF4 '全角/半角
        VK_OEM_BACKTAB = &HF5 'ローマ字
        VK_PACKET = &HE7
        VK_ATTN = &HF6
        VK_CRSEL = &HF7
        VK_EXSEL = &HF8
        VK_EREOF = &HF9
        VK_PLAY = &HFA
        VK_ZOOM = &HFB
        VK_NONAME = &HFC
        VK_PA1 = &HFD
        VK_OEM_CLEAR = &HFE
    End Enum

    Public Sub F_sendkeycode(ByVal virtualkeycode As Integer)
        If virtualkeycode <> 0 Then
            F_sendkey(virtualkeycode, 0)
            F_sendkey(virtualkeycode, dwFlags.KEYEVENTF_KEYUP)
        End If
    End Sub

    Private Sub F_sendkey(ByVal VirtualKeyCode As Integer, ByVal flag As dwFlags)
        Dim inputevents As New INPUT
        inputevents.type = INPUT_KEYBOARD
        inputevents.ki.wVk = VirtualKeyCode
        inputevents.ki.wScan = CShort(MapVirtualKey(CInt(VirtualKeyCode), 0))
        inputevents.ki.dwFlags = ExtendedKeyFlagW(VirtualKeyCode) Or flag
        inputevents.ki.time = 0
        inputevents.ki.dwExtraInfo = GetMessageExtraInfo()
        SendInput(1, inputevents, Marshal.SizeOf(inputevents))
    End Sub
End Module
