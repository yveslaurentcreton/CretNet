namespace CretNet.Platform.Blazor.Services;

public interface ICnpToastService
{
    void Show(ToastTypes toastType, string title, string message, params object[] messageParameters);
    void Info(string title, string message, params object[] messageParameters);
    void Success(string title, string message, params object[] messageParameters);
    void Warning(string title, string message, params object[] messageParameters);
    void Error(string title, string message, params object[] messageParameters);
}

public enum ToastTypes
{
    Info,
    Success,
    Warning,
    Error
}
