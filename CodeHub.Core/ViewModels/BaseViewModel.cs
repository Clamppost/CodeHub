using CodeHub.Core.Services;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Splat;

namespace CodeHub.Core.ViewModels
{
    public interface IProvidesTitle
    {
        string Title { get; }
    }

    public interface IRoutingViewModel
    {
        IObservable<IBaseViewModel> RequestNavigation { get; }

        IObservable<Unit> RequestDismiss { get; }
    }

    public interface IBaseViewModel : ISupportsActivation, IProvidesTitle, IRoutingViewModel
    {
    }

    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : ReactiveObject, IBaseViewModel
    {
        private readonly ViewModelActivator _viewModelActivator = new ViewModelActivator();
        private readonly ISubject<IBaseViewModel> _requestNavigationSubject = new Subject<IBaseViewModel>();
        private readonly ISubject<Unit> _requestDismissSubject = new Subject<Unit>();
    
        ViewModelActivator ISupportsActivation.Activator
        {
            get { return _viewModelActivator; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        protected void NavigateTo(IBaseViewModel viewModel)
        {
            _requestNavigationSubject.OnNext(viewModel);
        }

        protected void Dismiss()
        {
            _requestDismissSubject.OnNext(Unit.Default);
        }

        IObservable<IBaseViewModel> IRoutingViewModel.RequestNavigation
        {
            get { return _requestNavigationSubject; }
        }

        IObservable<Unit> IRoutingViewModel.RequestDismiss
        {
            get { return _requestDismissSubject; }
        }

        /// <summary>
        /// Gets the alert service
        /// </summary>
        /// <value>The alert service.</value>
        protected IAlertDialogService AlertService
        {
            get { return GetService<IAlertDialogService>(); }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
        protected TService GetService<TService>() where TService : class
        {
            return Locator.Current.GetService<TService>();
        }

        /// <summary>
        /// Display an error message to the user
        /// </summary>
        /// <param name="message">Message.</param>
        protected void DisplayAlert(string message)
        {
            AlertService.Alert("Error!", message).ToBackground();
        }

        /// <summary>
        /// Display an error message to the user
        /// </summary>
        /// <param name="message">Message.</param>
        protected Task DisplayAlertAsync(string message)
        {
            return AlertService.Alert("Error!", message);
        }
    }
}
