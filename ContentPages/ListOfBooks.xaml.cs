namespace MauiApp_StudyProject_.ContentPages;

public partial class ListOfBooks : ContentPage
{
    public IReadOnlyList<string> CurrentReadBooks { get; } = new List<string>
    {
        "The Hobbit",
        "The Lord of the Rings",
        "Harry Potter and the Sorcerer's Stone",
        "The Chronicles of Narnia",
        "The Hunger Games"
    };

    public IReadOnlyList<string> FavoriteBooks { get; } = new List<string>
    {
        "The Great Gatsby",
        "To Kill a Mockingbird",
        "Pride and Prejudice",
        "1984",
        "The Catcher in the Rye"
    };

    public ListOfBooks()
	{
        InitializeComponent();

        BindingContext = this;
    }
}