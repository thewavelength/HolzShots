Imports System.Text
Imports HolzShots
Imports HolzShots.Interop
Imports Microsoft.WindowsAPICodePack.Dialogs

Namespace UI
    Friend Class AboutForm
        Public Shared Property IsAboutInstanciated As Boolean
        Private Const YoutubePlaylistPage As String = "https://holz.nu/tunnel"

        Public Sub New()
            InitializeComponent()
            IsAboutInstanciated = True
        End Sub

        Private Sub AboutFormFormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
            IsAboutInstanciated = False
        End Sub

        Private Sub AboutFormNewLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            applicationTitleLabel.Text = LibraryInformation.Name
            versionLabel.Text = $"{Application.ProductVersion} ({LibraryInformation.VersionDate.ToShortDateString()})"
            timestampLabel.Text = $"2010-{DateTime.Now.Year} {LibraryInformation.PublisherName}"
        End Sub

        Private Shared Sub HolzShotsLinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles holzShotsLinkLabel.LinkClicked
            LibraryInformation.SiteUrl.SafeProcessStart()
        End Sub

        Private Sub LicenseLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LicenseLabel.LinkClicked
            LibraryInformation.LicenseUrl.SafeProcessStart()
        End Sub

        Private Sub ShowGfxResourcesLinklabelLinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles showGfxResourcesLinklabel.LinkClicked
            ShowGfx(Me)
        End Sub

        Private Shared Sub ShowGfx(parent As IWin32Window)
            Const title = "About Graphics"

            Dim sb As New StringBuilder()
            sb.AppendLine("Marker Icons by:")
            sb.AppendLine("Everaldo Coelho - www.everaldo.com").AppendLine()
            sb.AppendLine("Info icon by:")
            sb.AppendLine("Visual Pharm - www.visualpharm.com").AppendLine()
            sb.AppendLine("Free/remaining icons:")
            sb.AppendLine("www.iconfinder.com and VS2017ImageLibrary")

            If Not TaskDialog.IsPlatformSupported Then
                MessageBox.Show(parent, sb.ToString(), title)
                Return
            End If

            Using td As New TaskDialog()
                td.OwnerWindowHandle = If(parent?.Handle, IntPtr.Zero)
                td.Icon = TaskDialogStandardIcon.Information
                td.Caption = title
                td.InstructionText = "Graphic Resources"
                td.Text = sb.ToString()
                td.StandardButtons = TaskDialogStandardButtons.Close
                td.Show()
            End Using
        End Sub
    End Class
End Namespace
