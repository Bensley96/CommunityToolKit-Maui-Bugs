using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TintBehaviorBug_Project;

public partial class MainPage_ViewModel : ObservableObject
{
    [ObservableProperty]
    private bool m_IsRunning = false;
    
    [ObservableProperty]
    private bool m_SafeIsRunning = false;
    
    [RelayCommand]
    private void ToggleIsRunningButton(object sender)
    {
        try
        {
            IsRunning = !IsRunning;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    [RelayCommand]
    private void ToggleSafeIsRunningButton(object sender)
    {
        try
        {
            SafeIsRunning = !SafeIsRunning;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}