Imports System
Imports System.Runtime.InteropServices
Imports System.IO.MemoryMappedFiles
Imports System.Threading

Public Class SharedMemory

    Private _Size As Integer = Marshal.SizeOf(GetType(ServerTaskSharedInfo))

    Public Function Read(ProcessID As UInteger) As Integer
        Dim info As ServerTaskSharedInfo = New ServerTaskSharedInfo()
        Dim i As Integer
        'rectaskは最大30なのでとりあえず
        For i = 1 To 30
            'mutex確保
            'Dim mutex As Mutex = Nothing
            'While mutex Is Nothing
            ' Try
            ' mutex = mutex.OpenExisting("RecTask_Server_SharedMemory_" + i.ToString + "Mutex")
            ' Catch ex As Exception
            ' End Try
            'End While
            '
            'Using mutex
            ' If mutex.WaitOne() Then
            Try
                Using mmf As MemoryMappedFile = MemoryMappedFile.OpenExisting("RecTask_Server_SharedMemory_" + i.ToString)
                    Dim mma As MemoryMappedViewAccessor = mmf.CreateViewAccessor(0, _Size)
                    mma.Read(0, info)
                End Using
            Catch ex As Exception
            End Try
            ' End If
            ' mutex.ReleaseMutex()
            'End Using
            If info.Task.ProcessID = ProcessID Then
                Return info.Task.TaskID
            End If
        Next
        Return 0
    End Function
    Public Structure ServerTaskSharedInfo
        Public Header As TaskSharedInfoHeader
        Public Task As TaskInfo
        Public StatisticsUpdateCount As ULong
        Public Statistics As StreamStatistics
        Public TotTime As ULong
    End Structure
    Public Structure TaskSharedInfoHeader
        Public Size As UInteger
        Public Version As UInteger
    End Structure
    Public Structure TaskInfo
        Public TaskID As UInteger
        Public Type As TaskType
        Public ProcessID As UInteger
        Public Version As UInteger
        Public State As TaskState
    End Structure
    Enum TaskType
        TASK_TYPE_SERVER = 0
        TASK_TYPE_CLIENT
    End Enum
    Enum TaskState
        TASK_STATE_STARTING = 0
        TASK_STATE_RUNNING
        TASK_STATE_ENDING
    End Enum
    Public Structure StreamStatistics
        Public SignalLevel As Single
        Public BitRate As UInteger
        Public InputPacketCount As ULong
        Public ErrorPacketCount As ULong
        Public DiscontinuityCount As ULong
        Public ScramblePacketCount As ULong
    End Structure
End Class