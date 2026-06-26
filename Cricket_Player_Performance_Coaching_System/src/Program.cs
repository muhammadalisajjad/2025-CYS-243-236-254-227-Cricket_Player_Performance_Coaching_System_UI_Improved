using System;
using System.Windows.Forms;
using CricketPlayerManagementSystem.Forms;

namespace CricketPlayerManagementSystem;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}
