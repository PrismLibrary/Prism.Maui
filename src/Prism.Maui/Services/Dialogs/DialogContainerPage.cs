﻿using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Layouts;
using Prism.Services.Xaml;
using Application = Microsoft.Maui.Controls.Application;
using Page = Microsoft.Maui.Controls.Page;

namespace Prism.Services;

[EditorBrowsable(EditorBrowsableState.Never)]
public class DialogContainerPage : ContentPage, IDialogContainer
{
    public const string AutomationIdName = "PrismDialogModal";

    public DialogContainerPage()
    {
        AutomationId = AutomationIdName;
        BackgroundColor = Colors.Transparent;
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
    }

    public View DialogView { get; private set; }

    public ICommand Dismiss { get; private set; }

    public async Task ConfigureLayout(Page currentPage, View dialogView, bool hideOnBackgroundTapped, ICommand dismissCommand)
    {
        Dismiss = dismissCommand;
        DialogView = dialogView;
        Content = GetContentLayout(currentPage, dialogView, hideOnBackgroundTapped, dismissCommand);

        await DoPush(currentPage);
    }

    protected virtual async Task DoPush(Page currentPage)
    {
        await currentPage.Navigation.PushModalAsync(this, false);
    }

    public virtual async Task DoPop(Page currentPage)
    {
        await currentPage.Navigation.PopModalAsync(false);
    }

    protected virtual View GetContentLayout(Page currentPage, View dialogView, bool hideOnBackgroundTapped, ICommand dismissCommand)
    {
        var overlay = new AbsoluteLayout();
        var popupContainer = new DialogContainerView
        {
            IsPopupContent = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = dialogView,
        };

        var relativeWidth = DialogLayout.GetRelativeWidthRequest(dialogView);
        if (relativeWidth != null)
        {
            popupContainer.SetBinding(WidthRequestProperty,
                new Binding(nameof(Width),
                            BindingMode.OneWay,
                            new RelativeContentSizeConverter { RelativeSize = relativeWidth.Value },
                            source: this));
        }

        var relativeHeight = DialogLayout.GetRelativeHeightRequest(dialogView);
        if (relativeHeight != null)
        {
            popupContainer.SetBinding(HeightRequestProperty,
                new Binding(nameof(Height),
                            BindingMode.OneWay,
                            new RelativeContentSizeConverter { RelativeSize = relativeHeight.Value },
                            source: this));
        }

        AbsoluteLayout.SetLayoutFlags(popupContainer, AbsoluteLayoutFlags.PositionProportional);
        var popupBounds = DialogLayout.GetLayoutBounds(dialogView);
        AbsoluteLayout.SetLayoutBounds(popupContainer, popupBounds);

        var useMask = DialogLayout.GetUseMask(popupContainer.Content) ?? true;
        if (useMask)
        {
            var mask = GetMask(currentPage, dialogView, hideOnBackgroundTapped, dismissCommand);
            AbsoluteLayout.SetLayoutFlags(mask, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(mask, new Rect(0, 0, 1, 1));
            overlay.Children.Add(mask);
        }

        overlay.Children.Add(popupContainer);
        return overlay;
    }

    private View GetMask(Page currentPage, View dialogView, bool hideOnBackgroundTapped, ICommand dismissCommand)
    {
        View mask = DialogLayout.GetMask(dialogView);
        var reference = currentPage.GetParentWindow().Page;
        if (mask is null)
        {
            Style overlayStyle = GetOverlayStyle(dialogView, currentPage);

            mask = new BoxView
            {
                Style = overlayStyle,
                //HeightRequest = reference.Height,
                //WidthRequest = reference.Width
            };
        }

        mask.SetBinding(WidthRequestProperty, new Binding
        {
            Path = nameof(Width),
            Source = reference
        });
        mask.SetBinding(HeightRequestProperty, new Binding
        {
            Path = nameof(Height),
            Source = reference
        });

        if (hideOnBackgroundTapped)
        {
            mask.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = dismissCommand
            });
        }

        return mask;
    }

    private Style GetOverlayStyle(View popupView, Page currentPage)
    {
        var style = DialogLayout.GetMaskStyle(popupView);
        if (style != null)
        {
            return style;
        }

        return GetStyle(currentPage);
    }

    private static Style GetStyle(Element element)
    {
        if (element is Page page && page.Resources.ContainsKey(DialogLayout.PopupOverlayStyle) && page.Resources[DialogLayout.PopupOverlayStyle] is Style pageStyle)
            return pageStyle;
        else if (element is Application app)
        {
            if (app.Resources.ContainsKey(DialogLayout.PopupOverlayStyle) && app.Resources[DialogLayout.PopupOverlayStyle] is Style appStyle)
                return appStyle;

            var overlayStyle = DefaultStyle();

            app.Resources.Add(DialogLayout.PopupOverlayStyle, overlayStyle);
            return overlayStyle;
        }
        else if (element is Window window && window.Parent is null)
        {
            // HACK: https://github.com/dotnet/maui/issues/8635
            window.Parent = Application.Current;
            return GetStyle(window.Parent);
        }
        else
            return GetStyle(element.Parent);
    }

    private static Style DefaultStyle()
    {
        var overlayStyle = new Style(typeof(BoxView));
        overlayStyle.Setters.Add(new Setter { Property = BoxView.OpacityProperty, Value = 0.75 });
        overlayStyle.Setters.Add(new Setter { Property = BoxView.BackgroundColorProperty, Value = new Color(0, 0, 0, 0x75) });
        return overlayStyle;
    }
}
