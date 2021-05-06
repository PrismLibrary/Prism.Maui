using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Prism;
using Prism.Ioc;

namespace PrismMauiDemo
{
    public partial class App : PrismApplication
    {
        public App(IContainerExtension container)
            : base(container)
        {
            InitializeComponent();
        }

        protected override void OnInitialized()
        {
            Console.WriteLine("OnInitialized");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        //public override IWindow CreateWindow(IActivationState activationState)
        //{
        //    Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);

        //    return new MainWindow();
        //}
    }
}