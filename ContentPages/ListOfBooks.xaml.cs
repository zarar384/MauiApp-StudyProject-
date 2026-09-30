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

        ConfigureLayoutForPlatform();

        BindingContext = this;
    }

    /// <summary>
    /// Configures the layout properties based on the current platform.
    /// </summary>
    private void ConfigureLayoutForPlatform()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            BooksLayout.Padding = new Thickness(20, 12, 20, 24);
            BooksLayout.Spacing = 12;
        }
        else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            BooksLayout.Padding = 20;
            BooksLayout.Spacing = 12;
        }
        else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        {
            BooksLayout.Padding = new Thickness(64, 32);
            BooksLayout.Spacing = 20;
        }
        else // Default case
        {
            BooksLayout.Padding = 24;
            BooksLayout.Spacing = 16;
        }
    }
}