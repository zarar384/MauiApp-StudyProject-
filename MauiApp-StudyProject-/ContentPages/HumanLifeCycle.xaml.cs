namespace MauiApp_StudyProject_.ContentPages;

public partial class HumanLifeCycle : ContentPage
{
    private readonly LifeCycleLogic _logic = new();

    public HumanLifeCycle()
    {
        InitializeComponent();
        UpdateLifeCycleLabel();
    }

    private void Grow_Clicked(object sender, EventArgs e)
    {
        _logic.Grow();
        UpdateLifeCycleLabel();
    }

    private void UpdateLifeCycleLabel()
    {
        Age.Text = _logic.Age.ToString();
        LifeCycleLabel.Text = _logic.CurrentStage;
    }

    private sealed class LifeCycleLogic
    {
        public int Age { get; private set; }

        private static readonly (int Age, string Stage)[] LifeCycleStages =
        [
            (0, "👶"),   
            (5, "🧒"),  
            (13, "🧑"),  
            (18, "🧑"),  
            (50, "🧓"),  
            (80, "⚰️"),  
            (100, "🌱"), 
            (120, "🌳")  
        ];

        public string CurrentStage =>
            LifeCycleStages.Last(stage => Age >= stage.Age).Stage;

        public void Grow()
        {
            Age++;
        }
    }
}