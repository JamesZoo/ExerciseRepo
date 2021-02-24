﻿
namespace InventorySystem
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // Cleanup
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Startup
            base.OnStartup(e);
        }
    }
}
