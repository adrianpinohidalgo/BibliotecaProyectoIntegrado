namespace BibliotecaProyectoIntegrado.Views;

public partial class LibrosPage : ContentPage
{
	public LibrosPage()
	{
		InitializeComponent();
	}

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}