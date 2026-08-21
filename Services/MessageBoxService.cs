using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaModel.Services
{
    public class MessageBoxService
    {
        public async Task<ButtonResult> ShowAsync(string message, string title, ButtonEnum button, Icon icon)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message, button, icon
            );

            return await box.ShowAsync();
        }
    }
}
