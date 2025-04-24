using System.Collections.ObjectModel;
using System.Windows.Input;
using BibliotecaProyectoIntegrado.Services;
using BibliotecaProyectoIntegrado.Models;

namespace BibliotecaProyectoIntegrado.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<Libro> AllBooks { get; set; } = new();
    public ObservableCollection<Libro> FilteredBooks
    {
        get => _filteredBooks;
        set
        {
            _filteredBooks = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Libro> _filteredBooks = new();

    public int AvailableBooks { get; set; }
    public int ActiveLoans { get; set; }

    public ICommand FilterCommand { get; }

    public MainPageViewModel()
    {
        FilterCommand = new Command<string>(FilterBooks);
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "biblioteca.db");
        await DatabaseService.InitAsync(dbPath);

        var books = await DatabaseService.GetBooksAsync();
        AllBooks = new ObservableCollection<Libro>(books);
        FilteredBooks = new ObservableCollection<Libro>(books);

        AvailableBooks = await DatabaseService.GetAvailableBooksCountAsync();
        ActiveLoans = await DatabaseService.GetActiveLoansCountAsync();

        OnPropertyChanged(nameof(AvailableBooks));
        OnPropertyChanged(nameof(ActiveLoans));
    }

    private string _selectedGenre = "All";
    public string SelectedGenre
    {
        get => _selectedGenre;
        set
        {
            _selectedGenre = value;
            OnPropertyChanged();
        }
    }

    private void FilterBooks(string genre)
    {
        SelectedGenre = genre;

        if (genre == "All")
            FilteredBooks = new ObservableCollection<Libro>(AllBooks);
        else
            FilteredBooks = new ObservableCollection<Libro>(AllBooks.Where(b => b.Genero == genre));
    }

}
