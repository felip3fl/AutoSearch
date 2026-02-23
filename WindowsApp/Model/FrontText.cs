using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsApp.Model;

public partial class FrontText : ObservableObject
{
    [ObservableProperty]
    public partial string ProjectName { get; set; }

    public string ProjectVersion { get; set; }
    public string HowManySearch { get; set; }
    public string HowLong { get; set; }
    public string ListOfSearchOption { get; set; }
    public OptionText TurnOfComputer { get; set; }
    public OptionText UpdateThePage { get; set; }
    public string MainButton { get; set; }
    public string Log { get; set; }

}

public class OptionText
{
    public string mainDescription { get; set; }
    public string turnOn { get; set; }
    public string turnOff { get; set; }
}
