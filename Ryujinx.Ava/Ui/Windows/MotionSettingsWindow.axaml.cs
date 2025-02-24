using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.Common.Locale;
using Ryujinx.Ava.Ui.Models;
using Ryujinx.Ava.Ui.ViewModels;
using Ryujinx.Common.Configuration.Hid.Controller;
using System.Threading.Tasks;

namespace Ryujinx.Ava.Ui.Windows
{
    public partial class MotionSettingsWindow : UserControl
    {
        private readonly InputConfiguration<GamepadInputId, StickInputId> _viewmodel;

        public MotionSettingsWindow()
        {
            InitializeComponent();
            DataContext = _viewmodel;
        }

        public MotionSettingsWindow(ControllerSettingsViewModel viewmodel)
        {
            var config = viewmodel.Configuration as InputConfiguration<GamepadInputId, StickInputId>;

            _viewmodel = new InputConfiguration<GamepadInputId, StickInputId>()
            {
                Slot = config.Slot,
                AltSlot = config.AltSlot,
                DsuServerHost = config.DsuServerHost,
                DsuServerPort = config.DsuServerPort,
                MirrorInput = config.MirrorInput,
                EnableMotion = config.EnableMotion,
                Sensitivity = config.Sensitivity,
                GyroDeadzone = config.GyroDeadzone,
                EnableCemuHookMotion = config.EnableCemuHookMotion
            };

            InitializeComponent();
            DataContext = _viewmodel;
        }

        public static async Task Show(ControllerSettingsViewModel viewmodel)
        {
            MotionSettingsWindow content = new MotionSettingsWindow(viewmodel);

            ContentDialog contentDialog = new ContentDialog
            {
                Title = LocaleManager.Instance["ControllerMotionTitle"],
                PrimaryButtonText = LocaleManager.Instance["ControllerSettingsSave"],
                SecondaryButtonText = "",
                CloseButtonText = LocaleManager.Instance["ControllerSettingsClose"],
                Content = content
            };
            contentDialog.PrimaryButtonClick += (sender, args) =>
            {
                var config = viewmodel.Configuration as InputConfiguration<GamepadInputId, StickInputId>;
                config.Slot = content._viewmodel.Slot;
                config.EnableMotion = content._viewmodel.EnableMotion;
                config.Sensitivity = content._viewmodel.Sensitivity;
                config.GyroDeadzone = content._viewmodel.GyroDeadzone;
                config.AltSlot = content._viewmodel.AltSlot;
                config.DsuServerHost = content._viewmodel.DsuServerHost;
                config.DsuServerPort = content._viewmodel.DsuServerPort;
                config.EnableCemuHookMotion = content._viewmodel.EnableCemuHookMotion;
                config.MirrorInput = content._viewmodel.MirrorInput;
            };

            await contentDialog.ShowAsync();
        }
    }
}