using System;
using System.Configuration;

internal sealed class Settings : ApplicationSettingsBase, INotifyPropertyChanged
{
    private static Settings defaultInstance = new Settings();
    public static Settings Default => defaultInstance;

    public string Email { get; set; }
    public string Sifre { get; set; }
    public bool BeniHatirla { get; set; }
    public int CurrentUserID { get; set; }

    // Fix for CS1061: Add LastActivity property  
    public DateTime LastActivity
    {
        get => (DateTime)this["LastActivity"];
        set => this["LastActivity"] = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
