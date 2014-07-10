Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization

Module モジュール_番組表
    Public TvProgram_ch() As Integer
    Public TvProgram_NGword() As String

    Public TvProgram_list() As TVprogramstructure
    Public Structure TVprogramstructure
        Public stationDispName As String
        Public ProgramInformation As String
        Public startDateTime As String
        Public endDateTime As String
        Public programTitle As String
        Public programContent As String
    End Structure

    Public Function make_TVprogram_html_now() As String
        Dim html As String = ""

        Dim chkstr As String = ":" '重複防止用

        If TvProgram_ch IsNot Nothing Then
            For i As Integer = 0 To TvProgram_ch.Length - 1
                If TvProgram_ch(i) > 0 Then
                    Dim program() As TVprogramstructure = get_TVprogram_now(TvProgram_ch(i))
                    If program IsNot Nothing Then
                        For Each p As TVprogramstructure In program
                            Dim s4 As String = p.stationDispName
                            If s4.Length > 4 Then
                                s4 = s4.Substring(0, 4)
                            End If
                            If s4.Length > 0 Then
                                If chkstr.IndexOf(":" & s4 & ":") < 0 Then
                                    Dim chk As Integer = 0
                                    If TvProgram_NGword IsNot Nothing Then
                                        For j As Integer = 0 To TvProgram_NGword.Length - 1
                                            If p.stationDispName = TvProgram_NGword(j) Then
                                                chk = 1
                                            End If
                                        Next
                                    End If

                                    If chk = 0 Then
                                        Dim startt As String = ""
                                        Dim endt As String = ""
                                        Try
                                            startt = p.startDateTime.Substring(p.startDateTime.IndexOf(" "))
                                            endt = p.endDateTime.Substring(p.endDateTime.IndexOf(" "))
                                        Catch ex As Exception
                                        End Try
                                        html &= "<p class=""p_name"">" & p.stationDispName & "</p>" & vbCrLf
                                        html &= "<p class=""p_time"">" & startt & " ～" & endt & "</p>"
                                        html &= "<p class=""p_title"">" & p.programTitle & "</p>" & vbCrLf
                                        html &= "<p class=""p_content"">" & p.programContent & "</p>" & vbCrLf
                                        html &= "<br>" & vbCrLf
                                        chkstr &= s4 & ":"
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            Next
        End If

        Return html
    End Function

    Public Function get_TVprogram_now(ByVal regionID As Integer) As Object

        Dim r() As TVprogramstructure = Nothing

        Try
            Dim sTargetUrl As String = "http://www.tvguide.or.jp/TXML301PG.php?type=TVG&regionId=" & regionID.ToString
            Debug.Print(sTargetUrl)
            Dim objWeb As WebClient = New WebClient()
            Dim objSrializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim objEncode As Encoding = Encoding.UTF8
            Dim bResult As Byte() = objWeb.DownloadData(sTargetUrl)
            Dim sJson As String = objEncode.GetString(bResult)
            Dim objHash As Hashtable = objSrializer.Deserialize(Of Hashtable)(sJson)

            Dim dict As Dictionary(Of String, Object) = objHash("ProgramScheduleInfomartion")
            Dim MediaLocation As Dictionary(Of String, Object) = dict("MediaLocation")
            Dim StationLocation() As Object = MediaLocation("StationLocation")
            For Each station As Dictionary(Of String, Object) In StationLocation
                Dim j As Integer = 0
                If r Is Nothing Then
                    j = 0
                Else
                    j = r.Length
                End If
                ReDim Preserve r(j)
                r(j).stationDispName = CType(station("stationDispName"), String)
                Dim program As Dictionary(Of String, Object) = station("ProgramInformation")
                r(j).startDateTime = CType(program("startDateTime"), String)
                r(j).endDateTime = CType(program("endDateTime"), String)
                r(j).programTitle = CType(program("programTitle"), String)
                'r(j).programSubTitle = CType(program("programSubTitle"), String) '空白
                r(j).programContent = CType(program("programContent"), String)
                'r(j).programExplanation = CType(program("programExplanation"), String) '空白
            Next

        Catch ex As Exception

        End Try

        Return r
    End Function
End Module
