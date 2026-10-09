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

        #region INotifyPropertyChanged Implementation
        //string _title;
        //bool _isBusy;

        //public string Title
        //{
        //    get => _title;
        //    set
        //    {
        //        if (_title != value)
        //        {
        //            _title = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public bool IsBusy
        //{
        //    get => _isBusy;
        //    set
        //    {
        //        if (_isBusy != value)
        //        {
        //            _isBusy = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public event PropertyChangedEventHandler? PropertyChanged;

        //public void OnPropertyChanged([CallerMemberName] string? name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}
        #endregion
    }
}
