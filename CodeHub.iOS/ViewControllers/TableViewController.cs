﻿using System;
using UIKit;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using CodeHub.Core.ViewModels;
using System.Reactive.Linq;

namespace CodeHub.ViewControllers
{
    public abstract class TableViewController<TViewModel> : TableViewController, IViewFor<TViewModel> where TViewModel : class
    {
        private TViewModel _viewModel;
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected TableViewController(UITableViewStyle style = UITableViewStyle.Plain)
            : base(style)
        {
            Appearing
                .Take(1)
                .Select(_ => this.WhenAnyValue(x => x.ViewModel))
                .Switch()
                .OfType<ILoadableViewModel>()
                .Select(x => x.LoadCommand)
                .Subscribe(x => x.ExecuteIfCan());

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .OfType<IProvidesTitle>()
                    .Select(x => x.WhenAnyValue(y => y.Title))
                    .Switch()
                    .Subscribe(x => Title = x)
                    .AddTo(disposable);
            });
        }
    }

    public class TableViewController : BaseViewController
    {
        private readonly Lazy<UITableView> _tableView;
        private UIRefreshControl _refreshControl;

        public UITableView TableView { get { return _tableView.Value; } }

        public virtual UIRefreshControl RefreshControl
        {
            get { return _refreshControl; }
            set
            {
                _refreshControl?.RemoveFromSuperview();
                _refreshControl = value;

                if (_refreshControl != null)
                    TableView.AddSubview(_refreshControl);
            }
        }

        public TableViewController(UITableViewStyle style)
        {
            _tableView = new Lazy<UITableView>(() => new UITableView(CGRect.Empty, style));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Frame = View.Bounds;
            TableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            TableView.AutosizesSubviews = true;
            TableView.CellLayoutMarginsFollowReadableWidth = false;
            Add(TableView);
        }

        NSObject _hideNotification, _showNotification;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHideNotification);
            _showNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            View.EndEditing(true);
            
            if (_hideNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_hideNotification);
            if (_showNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_showNotification);
        }

        private void OnKeyboardHideNotification(NSNotification notification)
        {
            TableView.ContentInset = UIEdgeInsets.Zero;
            TableView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            var keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);
            var inset = new UIEdgeInsets(0, 0, keyboardFrame.Height, 0);
            TableView.ContentInset = inset;
            TableView.ScrollIndicatorInsets = inset;
        }
    }
}

