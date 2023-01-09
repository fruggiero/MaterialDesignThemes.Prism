using Prism.Ioc;
using Prism.Mvvm;
using System.Windows;
using System;
using System.Windows.Interop;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

// ReSharper disable once CheckNamespace
namespace Prism.Services.Dialogs
{
    public class MaterialDialogService : DialogService
    {
        private readonly IContainerExtension _containerExtension;

        public MaterialDialogService(IContainerExtension containerExtension) : base(containerExtension)
        {
            _containerExtension = containerExtension;
        }

        public void ShowDialogHost(string name, IDialogParameters parameters, Action<IDialogResult> callback) =>
            ShowDialogHost(name, null, parameters, callback);

        public void ShowDialogHost(string name, string dialogHostName, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            if (parameters == null)
                parameters = new DialogParameters();
            
            var content = _containerExtension.Resolve<object>(name);
            if (!(content is FrameworkElement dialogContent))
            {
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");
            }
            
            AutowireViewModel(dialogContent);

            if (!(dialogContent.DataContext is IDialogAware dialogAware))
            {
                throw new ArgumentException("A dialog's ViewModel must implement IDialogAware interface");
            }

            var openedEventHandler = new DialogOpenedEventHandler((sender, args) =>
            {
                dialogAware.OnDialogOpened(parameters);
            });
            var closedEventHandler = new DialogClosedEventHandler((sender, args) =>
            {
                dialogAware.OnDialogClosed();
            });
            
            dialogAware.RequestClose += res =>
            {
                if (DialogHost.IsDialogOpen(dialogHostName))
                {
                    callback(res);
                    DialogHost.Close(dialogHostName);
                }
            };

            var dispatcherFrame = new DispatcherFrame();
            if (dialogHostName == null)
            {
                _ = DialogHost.Show(dialogContent, openedEventHandler, null, closedEventHandler)
                    .ContinueWith(_ => dispatcherFrame.Continue = false);;
            }
            else
            {
                _ = DialogHost.Show(dialogContent, dialogHostName, openedEventHandler, null, closedEventHandler)
                    .ContinueWith(_ => dispatcherFrame.Continue = false);
            }

            try
            {
                // tell users we're going modal
                ComponentDispatcher.PushModal();
 
                Dispatcher.PushFrame(dispatcherFrame);
            }
            finally
            {
                // tell users we're going non-modal
                ComponentDispatcher.PopModal();
            }

            dialogAware.RequestClose -= callback;
        }

        private static void AutowireViewModel(object viewOrViewModel)
        {
            if (viewOrViewModel is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
            {
                ViewModelLocator.SetAutoWireViewModel(view, true);
            }
        }
    }
}