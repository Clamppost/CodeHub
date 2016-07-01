using System;
using Foundation;
using UIKit;
using CodeHub.Core.ViewModels.PullRequests;
using ReactiveUI;

namespace CodeHub.TableViewCells
{
    public partial class PullRequestCellView : BaseTableViewCell<PullRequestItemViewModel>
    {
        public static readonly UINib Nib = UINib.FromName("PullRequestCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PullRequestCellView");

        public PullRequestCellView(IntPtr handle) 
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            MainImageView.Layer.MasksToBounds = true;
            MainImageView.Layer.CornerRadius = MainImageView.Frame.Height / 2f;
            ContentView.Opaque = true;

            SeparatorInset = new UIEdgeInsets(0, TitleLabel.Frame.Left, 0, 0);
            TitleLabel.TextColor = Theme.CurrentTheme.MainTitleColor;
            TimeLabel.TextColor = Theme.CurrentTheme.MainTextColor;

            this.WhenAnyValue(x => x.ViewModel)
                .Subscribe(x =>
                {
                    TitleLabel.Text = x?.Title;
                    TimeLabel.Text = x?.Details;
                    MainImageView.SetAvatar(x?.Avatar);
                });
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseDesignerOutlets();
            base.Dispose(disposing);
        }
    }
}

