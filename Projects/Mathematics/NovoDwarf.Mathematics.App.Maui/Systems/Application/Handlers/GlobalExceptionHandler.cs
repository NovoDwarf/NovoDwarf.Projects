namespace NovoDwarf.Mathematics.App.Systems.Application.Handlers;

public static class GlobalExceptionHandler
{
    public static event UnhandledExceptionEventHandler? UnhandledException;

    static GlobalExceptionHandler()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            UnhandledException?.Invoke(sender, args);
        };

#if Windows

        Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) =>
        {
            if (sender != null)
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, true));
        };
        
#endif

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            if (sender != null)
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, false));
        };

#if IOS

        ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
        {
            args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
        };

#elif ANDROID

        // For Android:
        // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
        // and NOT through AppDomain.CurrentDomain.UnhandledException

        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
        {
            args.Handled = true;
            
            if (sender != null)
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, true));
        };

        Java.Lang.Thread.DefaultUncaughtExceptionHandler = new CustomUncaughtExceptionHandler(e =>
            UnhandledException?.Invoke(null, new UnhandledExceptionEventArgs(e, true)));
#endif
    }
}

#if ANDROID
public class CustomUncaughtExceptionHandler(Action<Java.Lang.Throwable> callback)
    : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
{
    public void UncaughtException(Java.Lang.Thread t, Java.Lang.Throwable e)
    {
        callback(e);
    }
}
#endif