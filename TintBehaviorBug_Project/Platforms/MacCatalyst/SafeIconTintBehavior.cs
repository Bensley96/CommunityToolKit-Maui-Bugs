using System.ComponentModel;
using Microsoft.Maui.Platform;
using UIKit;
using PlatformView = UIKit.UIView;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Converters;
using Microsoft.Maui.LifecycleEvents;

namespace CoreGO.CommunityToolkit.Maui.Behaviors;

public class SafeIconTintColorBehavior: BasePlatformBehavior<View>
{
    /// <summary>
    	/// Attached Bindable Property for the <see cref="TintColor"/>.
    	/// </summary>
    	public static readonly BindableProperty TintColorProperty =
    		BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SafeIconTintColorBehavior), default);
    
    	/// <summary>
    	/// Property that represents the <see cref="Color"/> that Icon will be tinted.
    	/// </summary>
    	public Color? TintColor
    	{
    		get => (Color?)GetValue(TintColorProperty);
    		set => SetValue(TintColorProperty, value);
    	}
    
	/// <inheritdoc/>
	protected override void OnViewPropertyChanged(View sender, PropertyChangedEventArgs e)
	{
		base.OnViewPropertyChanged(sender, e);

		if ((e.PropertyName != ImageButton.IsLoadingProperty.PropertyName
			&& e.PropertyName != Image.SourceProperty.PropertyName
			&& e.PropertyName != ImageButton.SourceProperty.PropertyName)
			|| sender is not IImageElement element
			|| sender.Handler?.PlatformView is not UIView platformView)
		{
			return;
		}

		if (!element.IsLoading)
		{
			ApplyTintColor(platformView, (View)element, TintColor);
		}
	}

	/// <inheritdoc/>
	protected override void OnAttachedTo(View bindable, UIView platformView)
	{
		base.OnAttachedTo(bindable, platformView);

		ApplyTintColor(platformView, bindable, TintColor);

		this.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == TintColorProperty.PropertyName)
			{
				ApplyTintColor(platformView, bindable, TintColor);
			}
		};
	}

	/// <inheritdoc/>
	protected override void OnDetachedFrom(View bindable, UIView platformView)
	{
		base.OnDetachedFrom(bindable, platformView);

		ClearTintColor(platformView, bindable);
	}

	static void ClearTintColor(UIView platformView, View element)
	{
		switch (platformView)
		{
			case UIImageView imageView:
				if (imageView.Image is not null)
				{
					imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
				}

				break;
			case UIButton button:
				if (button.ImageView.Image is not null)
				{
					var originalImage = button.CurrentImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
					button.SetImage(originalImage, UIControlState.Normal);
				}

				break;

			default:
				throw new NotSupportedException($"{nameof(SafeIconTintColorBehavior)} only currently supports {nameof(UIButton)} and {nameof(UIImageView)}.");
		}
	}

	static void ApplyTintColor(UIView platformView, View element, Color? color)
	{
		if (color is null)
		{
			ClearTintColor(platformView, element);
			return;
		}

		switch (platformView)
		{
			case UIImageView imageView:
				SetUIImageViewTintColor(imageView, element, color);
				break;
			case UIButton button:
				SetUIButtonTintColor(button, element, color);
				break;
			default:
				throw new NotSupportedException($"{nameof(SafeIconTintColorBehavior)} only currently supports {nameof(UIButton)} and {nameof(UIImageView)}.");
		}
	}

	static void SetUIButtonTintColor(UIButton button, View element, Color color)
	{
		if (button.ImageView.Image is null || button.CurrentImage is null)
		{
			return;
		}

		var templatedImage = button.CurrentImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

		button.SetImage(null, UIControlState.Normal);
		var platformColor = color.ToPlatform();
		button.TintColor = platformColor;
		button.ImageView.TintColor = platformColor;
		button.SetImage(templatedImage, UIControlState.Normal);
	}

	static void SetUIImageViewTintColor(UIImageView imageView, View element, Color color)
	{
		if (imageView.Image is null)
		{
			return;
		}

		imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
		imageView.TintColor = color.ToPlatform();
	}
}

/// <summary>
    /// Abstract class for our behaviors to inherit.
    /// </summary>
    /// <typeparam name="TView">The <see cref="VisualElement"/> that the behavior can be applied to</typeparam>
    public abstract class BasePlatformBehavior<TView> : BasePlatformBehavior<TView, PlatformView>
    	where TView : Element
    {
    	private protected BasePlatformBehavior()
    	{
    
    	}
    }
    
    /// <summary>
    /// Abstract class for our behaviors to inherit.
    /// </summary>
    /// <typeparam name="TView">The <see cref="VisualElement"/> that the behavior can be applied to</typeparam>
    /// <typeparam name="TPlatformView">The <see langword="class"/> that the behavior can be applied to</typeparam>
    public abstract class BasePlatformBehavior<TView, TPlatformView> : PlatformBehavior<TView, TPlatformView>, ICommunityToolkitBehavior<TView>
    	where TView : Element
    	where TPlatformView : class
    {
    	private protected BasePlatformBehavior()
    	{
    
    	}
    
    	/// <summary>
    	/// View used by the Behavior
    	/// </summary>
    	protected TView? View { get; set; }
    
    	TView? ICommunityToolkitBehavior<TView>.View
    	{
    		get => View;
    		set => View = value;
    	}
    
    	/// <summary>
    	/// Virtual method that executes when a property on the View has changed
    	/// </summary>
    	/// <param name="sender"></param>
    	/// <param name="e"></param>
    	protected virtual void OnViewPropertyChanged(TView sender, PropertyChangedEventArgs e)
    	{
    
    	}
    
    	/// <inheritdoc/>
    	protected override void OnAttachedTo(TView bindable, TPlatformView platformView)
    	{
    		base.OnAttachedTo(bindable, platformView);
    
    		((ICommunityToolkitBehavior<TView>)this).AssignViewAndBingingContext(bindable);
    	}
    
    	/// <inheritdoc/>
    	protected override void OnDetachedFrom(TView bindable, TPlatformView platformView)
    	{
    		base.OnDetachedFrom(bindable, platformView);
    
    		((ICommunityToolkitBehavior<TView>)this).UnassignViewAndBingingContext(bindable);
    	}
    
    	void ICommunityToolkitBehavior<TView>.OnViewPropertyChanged(TView sender, PropertyChangedEventArgs e) => OnViewPropertyChanged(sender, e);
    }

/// <summary>
/// A common interface to be used across <see cref="BaseBehavior{TView}"/> and <see cref="BasePlatformBehavior{TView,TPlatformView}"/>
/// </summary>
public interface ICommunityToolkitBehavior<TView> where TView : Element
{
	/// <summary>
	/// View used by the Behavior
	/// </summary>
	protected TView? View { get; set; }

	internal bool TrySetBindingContextToAttachedViewBindingContext()
	{
		if (this is not Behavior behavior)
		{
			throw new InvalidOperationException($"{nameof(ICommunityToolkitBehavior<TView>)} can only be used for a {nameof(Behavior)}");
		}

		if (behavior.IsSet(BindableObject.BindingContextProperty) || View is null)
		{
			return false;
		}

		behavior.SetBinding(BindableObject.BindingContextProperty, new Binding
		{
			Source = View,
			Path = BindableObject.BindingContextProperty.PropertyName
		});

		return true;

	}

	internal bool TryRemoveBindingContext()
	{
		if (this is not Behavior behavior)
		{
			throw new InvalidOperationException($"{nameof(ICommunityToolkitBehavior<TView>)} can only be used for a {nameof(Behavior)}");
		}

		if (behavior.IsSet(BindableObject.BindingContextProperty))
		{
			behavior.RemoveBinding(BindableObject.BindingContextProperty);
			return true;
		}

		return false;
	}

	[MemberNotNull(nameof(View))]
	internal void AssignViewAndBingingContext(TView bindable)
	{
		View = bindable;
		bindable.PropertyChanged += OnViewPropertyChanged;

		TrySetBindingContextToAttachedViewBindingContext();
	}

	internal void UnassignViewAndBingingContext(TView bindable)
	{
		TryRemoveBindingContext();

		bindable.PropertyChanged -= OnViewPropertyChanged;

		View = null;
	}

	/// <summary>
	/// Executes when <see cref="BindableObject.OnPropertyChanged"/> fires
	/// </summary>
	/// <param name="sender"><see cref="Behavior"/></param>
	/// <param name="e"><see cref="PropertyChangedEventArgs"/> </param>
	/// <exception cref="ArgumentException">Throws when <paramref name="sender"/> is not of type <typeparamref name="TView"/></exception>
	protected void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (sender is not TView view)
		{
			throw new ArgumentException($"Behavior can only be attached to {typeof(TView)}");
		}

		try
		{
			OnViewPropertyChanged(view, e);
		}
		catch (Exception ex) when (Options.ShouldSuppressExceptionsInBehaviors)
		{
			Trace.TraceInformation("{0}", ex);
		}
	}

	/// <summary>
	/// Virtual method that executes when a property on the View has changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void OnViewPropertyChanged(TView sender, PropertyChangedEventArgs e);
}


/// <summary>
/// .NET MAUI Community Toolkit Options.
/// </summary>
public class Options()
{
	readonly MauiAppBuilder? builder;

	internal Options(in MauiAppBuilder builder) : this()
	{
		this.builder = builder;
	}

	internal static bool ShouldSuppressExceptionsInAnimations { get; private set; }
	internal static bool ShouldSuppressExceptionsInConverters { get; private set; }
	internal static bool ShouldSuppressExceptionsInBehaviors { get; private set; }
	internal static bool ShouldEnableSnackbarOnWindows { get; private set; }

	/// <summary>
	/// Allows to return default value instead of throwing an exception when using <see cref="BaseConverter{TFrom,TTo}"/>.
	/// </summary>
	/// <remarks>
	/// Default value is false.
	/// </remarks>
	public void SetShouldSuppressExceptionsInConverters(bool value) => ShouldSuppressExceptionsInConverters = value;

	/// <summary>
	/// Allows to return default value instead of throwing an exception when using <see cref="AnimationBehavior"/>.
	/// </summary>
	/// <remarks>
	/// Default value is false.
	/// </remarks>
	public void SetShouldSuppressExceptionsInAnimations(bool value) => ShouldSuppressExceptionsInAnimations = value;

	/// <summary>
	/// Allows to return default value instead of throwing an exception when using <see cref="BaseBehavior{TView}"/>.
	/// </summary>
	/// <remarks>
	/// Default value is false.
	/// </remarks>
	public void SetShouldSuppressExceptionsInBehaviors(bool value) => ShouldSuppressExceptionsInBehaviors = value;

	/// <summary>
	/// Enables <see cref="Alerts.Snackbar"/> for Windows
	/// </summary>
	/// <remarks>
	/// Additional setup is required in the Package.appxmanifest file to enable <see cref="Alerts.Snackbar"/> on Windows. See the <a href="https://learn.microsoft.com/dotnet/communitytoolkit/maui/alerts/snackbar">Snackbar Platform Specific Initialization Documentation</a> for more information. Default value is false.
	/// </remarks>
	public void SetShouldEnableSnackbarOnWindows(bool value)
	{
		ShouldEnableSnackbarOnWindows = value;
	}
}