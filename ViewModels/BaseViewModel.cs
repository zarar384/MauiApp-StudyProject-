using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp_StudyProject_.ViewModels
{
    /// <summary>
    /// Notifies the view when a property value changes, 
    /// allowing for data binding in the MVVM pattern.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        bool isLoading;

        public bool IsNotLoading => !IsLoading;

        [ObservableProperty]
        bool isRefreshing;

        #region INotifyPropertyChanged Implementation

        #endregion
    }
}
