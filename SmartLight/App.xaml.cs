using SmartLight.Models;

namespace SmartLight;

public partial class App : Application
{

    public static DeviceDB DeviceDataBase { get; private set; }

    public App(DeviceDB dataBase)
	{
		InitializeComponent();

		MainPage = new AppShell();

        DeviceDataBase = dataBase;
	}
}
