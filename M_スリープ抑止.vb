Module M_スリープ抑止
    'http://akademeia.info/index.php?VB.NET%2F%A5%C6%A5%AF%A5%CB%A5%C3%A5%AF%2F%A5%B9%A5%EA%A1%BC%A5%D7%A5%E2%A1%BC%A5%C9%CD%DE%BB%DF

    Public viewing_NoSleep As Integer = 0
    Public DisableSleep_ON As Integer = 0
    Public sleep_stopping_utime As Integer = 0

    'スリープ関連
    Public previousExecutionState As Integer = &HD

    ''' <summary>
    ''' スリープモードを抑止にする.
    ''' </summary>
    ''' <returns>無効にする前のスリープモード状態</returns>
    ''' <remarks></remarks>
    Public Function DisableSleepMode() As Integer
        Return SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED Or EXECUTION_STATE.ES_CONTINUOUS)
    End Function

    ''' <summary>
    ''' 指定したスリープモード状態にする.
    ''' </summary>
    ''' <param name="state">スリープモード状態</param>
    ''' <remarks></remarks>
    Public Sub SetSleepMode(ByVal state As Integer)
        SetThreadExecutionState(state)
    End Sub

    ''' <summary>
    ''' スリープモードの抑止を解除する.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EnableSleepMode()
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS)
    End Sub

    Private Enum EXECUTION_STATE As Integer
        ES_SYSTEM_REQUIRED = &H1
        ES_DISPLAY_REQUIRED = &H2
        ES_USER_PRESENT = &H4
        ES_CONTINUOUS = &H80000000
    End Enum

    Private Declare Auto Function SetThreadExecutionState Lib "kernel32.dll" (ByVal esFlags As Integer) As Integer

    Public Sub DisableSleep(ByVal a As Integer)
        If a = 1 Then
            DisableSleep_ON = 1
            DisableSleepMode()
            log1write("スリープ抑止します")
        Else
            DisableSleep_ON = 0
            EnableSleepMode()
            log1write("スリープ抑止を解除しました")
        End If
    End Sub

End Module

