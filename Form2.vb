Public Class Form2

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Close()
    End Sub

    Private Sub textBoxSeekFileList_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBoxSeekFileList.TextChanged
        'ffmpeg シーク方法を変更するファイルリスト
        ffmpeg_seek_method_files = textBoxSeekFileList.Text.ToString
    End Sub

    Private Sub Form2_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        textBoxSeekFileList.Text = ffmpeg_seek_method_files
    End Sub
End Class