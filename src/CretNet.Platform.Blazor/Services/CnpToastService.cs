using Microsoft.FluentUI.AspNetCore.Components;
using SmartFormat;

namespace CretNet.Platform.Blazor.Services;
using IFluentToastService = IToastService;

public class CnpToastService : ICnpToastService
{
    private readonly IMessageService _messageService;
    private readonly IFluentToastService _fluentToastService;

    public CnpToastService(IMessageService messageService, IFluentToastService fluentToastService)
    {
        _messageService = messageService;
        _fluentToastService = fluentToastService;
    }
    
    public void Show(ToastTypes toastType, string title, string message, params object[] messageParameters)
    {
        var toastIntent = ToastIntent.Custom;
        var messageIntent = MessageIntent.Custom;

        switch (toastType)
        {
            case ToastTypes.Info:
                toastIntent = ToastIntent.Info;
                messageIntent = MessageIntent.Info;
                break;
            case ToastTypes.Success:
                toastIntent = ToastIntent.Success;
                messageIntent = MessageIntent.Success;
                break;
            case ToastTypes.Warning:
                toastIntent = ToastIntent.Warning;
                messageIntent = MessageIntent.Warning;
                break;
            case ToastTypes.Error:
                toastIntent = ToastIntent.Error;
                messageIntent = MessageIntent.Error;
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
        
        _messageService.ShowMessageBar(options =>
        {
            options.Intent = messageIntent;
            options.Title = formattedTitle;
            options.Body = formattedMessage.Replace("\r\n", "<br/>");
            options.Timestamp = DateTime.Now;
            options.Section = "MESSAGES_NOTIFICATION_CENTER";
        });

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

