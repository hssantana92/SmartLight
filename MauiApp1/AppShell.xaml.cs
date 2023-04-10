namespace SmartLight;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();


		Routing.RegisterRoute(nameof(DevicePage), typeof(DevicePage));
	}
}
