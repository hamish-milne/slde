using System;
using Gtk;
using Mono.TextEditor;

public partial class MainWindow: Gtk.Window
{

	TextEditor textEditor;
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		var options = new TextEditorOptions ();
		options.ColorScheme = "Oblivion";
		Mono.TextEditor.Highlighting.SyntaxModeService.LoadStylesAndModes (typeof(TextEditor).Assembly);

		textEditor = new TextEditor (new TextDocument (), options);
		Build ();
		scrolledWindow.Child = textEditor;
		textEditor.ShowAll ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
