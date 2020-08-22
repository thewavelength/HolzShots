Imports System.Linq
Imports HolzShots.Interop
Imports HolzShots.UI.Controls

Namespace UI.Specialized

    Friend Class SettingsWindow
        Inherits Form

        Public Shared ReadOnly Instance As SettingsWindow = New SettingsWindow()

        Private Shared ReadOnly BorderPen As New Pen(Color.FromArgb(255, &HCC, &HCC, &HCC))

        Private ReadOnly _pluginInfoItemList As New List(Of PluginInfoItem)

        Private Sub New()
            InitializeComponent()
            DisplayPlugins()
        End Sub


        Private Sub DisplayPlugins()

            If My.Application.Uploaders.Loaded Then
                pluginListPanel.Controls.Clear()
                _pluginInfoItemList.Clear()

                Dim metadata = My.Application.Uploaders.GetMetadata()
                Dim metaArr = metadata.Select(Function(i) New PluginInfoItem(i)).ToArray()
                pluginListPanel.Controls.AddRange(metaArr)
                _pluginInfoItemList.AddRange(metaArr)
            End If
        End Sub

        Private Sub LoadPolicies()
            start_with_windows.Checked = HolzShotsEnvironment.AutoStart
        End Sub

        Private Sub SavePolicies()
            HolzShotsEnvironment.AutoStart = start_with_windows.Checked

            My.Settings.Save()
        End Sub

        Private Sub SettingsLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            LoadPolicies()
        End Sub

        Private Sub SavebtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles savebtn.Click
            SavePolicies()
            Close()
        End Sub

        Private Sub AbortClick(ByVal sender As Object, ByVal e As EventArgs) Handles Abort.Click
            Close()
        End Sub

        Private Sub SettingsShown(sender As Object, e As EventArgs) Handles MyBase.Shown
            Focus()
        End Sub

        Private Shared Sub OpenPluginFolderLinkLabelLinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles openPluginFolderLinkLabel.LinkClicked
            My.Application.Uploaders.Plugins.PluginDirectory.OpenFolderInExplorer()
        End Sub

        Private Sub PluginsTabPaint(sender As Object, e As PaintEventArgs) Handles PluginsTab.Paint
            e.Graphics.DrawLine(BorderPen, 0, pluginListPanel.Location.Y - 1, Width - 1, pluginListPanel.Location.Y - 1)
        End Sub
    End Class
End Namespace
