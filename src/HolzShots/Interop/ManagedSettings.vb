Imports HolzShots
Imports HolzShots.ScreenshotRelated
Imports HolzShots.ScreenshotRelated.Selection

Namespace Interop
    Friend Module ManagedSettings
        Public Property EnableShotEditor As Boolean
            Get
                Return My.Settings.EnableShotEditor
            End Get
            Set
                My.Settings.EnableShotEditor = Value
            End Set
        End Property

        Public Property EnableLinkViewer As Boolean
            Get
                Return My.Settings.EnableLinkViewer
            End Get
            Set
                My.Settings.EnableLinkViewer = Value
            End Set
        End Property

        Public Property ShellExtensionUpload As Boolean
            Get
                Return ShellExtensions.ShellExtensionUpload
            End Get
            Set
                If Value <> ShellExtensions.ShellExtensionUpload AndAlso InteropHelper.IsAdministrator() Then
                    ShellExtensions.ShellExtensionUpload = Value
                End If
            End Set
        End Property

        Public Property ShellExtensionOpen As Boolean
            Get
                Return ShellExtensions.ShellExtensionOpen
            End Get
            Set
                If Value <> ShellExtensions.ShellExtensionOpen AndAlso InteropHelper.IsAdministrator() Then
                    ShellExtensions.ShellExtensionOpen = Value
                End If
            End Set
        End Property

        Public Property SelectionDecoration As SelectionDecorations
            Get
                Return My.Settings.SelectionDecoration
            End Get
            Set
                My.Settings.SelectionDecoration = Value
            End Set
        End Property
    End Module
End Namespace
