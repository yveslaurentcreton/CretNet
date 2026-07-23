using Microsoft.FluentUI.AspNetCore.Components;
using SmartFormat;

namespace CretNet.Platform.Blazor.Services;
using IFluentToastService = IToastService;

public class CnpToastService : ICnpToastService
{
    private readonly IFluentToastService _fluentToastService;

    public CnpToastService(IFluentToastService fluentToastService)
    {
        _fluentToastService = fluentToastService;
    }
    
    public void Show(ToastTypes toastType, string title, string message, params object[] messageParameters)
    {
        var toastIntent = ToastIntent.Custom;

        switch (toastType)
        {
            case ToastTypes.Info:
                toastIntent = ToastIntent.Info;
                break;
            case ToastTypes.Success:
                toastIntent = ToastIntent.Success;
                break;
            case ToastTypes.Warning:
                toastIntent = ToastIntent.Warning;
                break;
            case ToastTypes.Error:
                toastIntent = ToastIntent.Error;
                break;
            default:
                break;
        }
        
        var formattedTitle = title;
        if (messageParameters.Length != 0)
            formattedTitle = Smart.Format(title, messageParameters);
        
        var formattedMessage = message;
        if (messageParameters.Length != 0)
            formattedMessage = Smart.Format(message, messageParameters);
        
        _fluentToastService.ShowCommunicationToast(new ToastParameters<CommunicationToastContent>()
        {
            Intent = toastIntent,
            Title = formattedTitle,
            Content = new CommunicationToastContent()
            {
                Details = formattedMessage,
            }
        });
    }

    public void Info(string title, string message, params object[] messageParameters)
    {
        Show(ToastTypes.Info, title, message, messageParameters);
    }

    public void Success(string title, string message, params object[] messageParameters)
    {
        Show(ToastTypes.Success, title, message, messageParameters);
    }

    public void Warning(string title, string message, params object[] messageParameters)
    {
        Show(ToastTypes.Warning, title, message, messageParameters);
    }

    public void Error(string title, string message, params object[] messageParameters)
    {
        Show(ToastTypes.Error, title, message, messageParameters);
    }
}

