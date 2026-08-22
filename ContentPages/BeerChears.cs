namespace MauiApp_StudyProject_.ContentPages;

public class BeerChears : ContentPage
{
    private readonly HorizontalStackLayout _beerContainer;
    private readonly CheersLogic _logic;

    public BeerChears()
	{
        _logic = new CheersLogic();

        // container for beers emoji
        _beerContainer = new HorizontalStackLayout()
        {
            Spacing = 10,
            VerticalOptions = LayoutOptions.Center
        };

        var btnBeer = new Button()
        {
            Text = "Get Beer",
            VerticalOptions = LayoutOptions.Center,
        };
        
        btnBeer.Clicked += OnClickedEvent;

        // layout for the page
        // [Button] [BeerContainer]
        var layout = new HorizontalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(10),
            Children =
            {
                btnBeer,
                _beerContainer
            }
        };

        Content = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = layout
        };
	}

    private void OnClickedEvent(object? sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_logic.Count == 0)
            _beerContainer.Clear();

        var beer = _logic.GetBeer();

        if (beer is Cheers)
        {
            _beerContainer.Clear();
            button.Text = "Get Beer";
        }
        else if (_logic.Count == 2)
        {
            button.Text = "Cheers!";
        }

        _beerContainer.Add(beer.Get());
    }

    private class CheersLogic
    {
        public int Count { get; private set; }

        public Beer GetBeer()
        {
            if (Count == 2)
            {
                Count = 0;
                return new Cheers();
            }

            Count++;
            return new Beer();
        }
    }

    private class Cheers: Beer
    {
        protected override string Text => "Cheers! 🍻";
        override protected double FontSize => 50;
    }

    private class Beer
	{
		protected virtual string Text => "🍺";
        protected virtual double FontSize => 100;

        public Label Get()
        {
            return new Label
            {
                Text = this.Text,
                FontSize = this.FontSize,
                HorizontalOptions = LayoutOptions.Center,
            };
        }
    }
}