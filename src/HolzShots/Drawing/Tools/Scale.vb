Imports HolzShots.Drawing.Tools.UI
Imports HolzShots.UI.Controls

Namespace Drawing.Tools
    Friend Class Scale
        Inherits Tool

        Public Overrides ReadOnly Property Cursor As Cursor = Cursors.Arrow
        Public Overrides ReadOnly Property ToolType As PaintPanel.Tools = PaintPanel.Tools.Scale

        Public Overrides Sub RenderFinalImage(ByRef rawImage As Image, ByVal sender As PaintPanel)

            Using s As New ScaleWindow(rawImage)

                If s.ShowDialog(sender) = DialogResult.OK Then
                    Dim widh As Double = s.WidthBoxV
                    Dim hei As Double = s.HeightBoxV

                    Dim unit As ScaleWindow.ScaleUnit = s.CurrentScaleUnit

                    Dim newWidth As Integer
                    Dim newheight As Integer
                    Dim newCursorCoods As Point = sender.Screenshot.CursorPosition

                    Dim newCursorWidth As Integer = My.Resources.windowsCursorMedium.Width
                    Dim newCursorHeight As Integer = My.Resources.windowsCursorMedium.Height


                    If unit = ScaleWindow.ScaleUnit.Percent Then
                        newWidth = CInt(rawImage.Width * (widh / 100))
                        newheight = CInt(rawImage.Height * (hei / 100))
                        newCursorCoods.X = CInt(newCursorCoods.X * (hei / 100))
                        newCursorCoods.Y = CInt(newCursorCoods.Y * (widh / 100))
                        newCursorWidth = CInt(newCursorWidth * (widh / 100))
                        newCursorHeight = CInt(newCursorHeight * (hei / 100))
                    Else
                        newWidth = CInt(widh)
                        newheight = CInt(hei)
                    End If

                    Dim newRawImage As New Bitmap(newWidth, newheight)
                    Using g As Graphics = Graphics.FromImage(newRawImage)
                        g.DrawImage(rawImage, 0, 0, newWidth, newheight)
                        If sender.DrawCursor Then
                            g.DrawImage(My.Resources.windowsCursorMedium, newCursorCoods.X, newCursorCoods.Y, newCursorWidth, newCursorHeight)
                        End If
                    End Using

                    rawImage = newRawImage
                    sender.RawBox.Image = newRawImage
                    GC.Collect()

                Else
                    sender.CurrentTool = PaintPanel.Tools.None
                End If
            End Using
        End Sub
    End Class
End Namespace
