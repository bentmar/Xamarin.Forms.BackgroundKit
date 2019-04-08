﻿using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables.Shapes;
using Android.Support.Design.Card;
using Android.Support.Design.Chip;
using Android.Views;
using System.Collections.Generic;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinBackgroundKit.Abstractions;
using XamarinBackgroundKit.Android.Renderers;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Extensions;
using AView = Android.Views.View;
using Color = Xamarin.Forms.Color;
using IBorderElement = XamarinBackgroundKit.Abstractions.IBorderElement;
using XView = Xamarin.Forms.View;

namespace XamarinBackgroundKit.Android.Extensions
{
    public static class ViewExtensions
	{
        public static void SetColor(this AView view, Color color)
        {
            view.GetGradientDrawable().Paint.Color = color == Color.Default
                ? Color.Black.ToAndroid()
                : color.ToAndroid();
        }

        #region Border

        public static void SetBorder(this Chip view, Context context, IBorderElement borderElement)
		{
			view.ChipStrokeColor = ColorStateList.ValueOf(borderElement.BorderColor.ToAndroid());
			view.ChipStrokeWidth = (int)context.ToPixels(borderElement.BorderWidth);
		}

		public static void SetBorder(this MaterialCardView view, Context context, IBorderElement borderElement)
		{
			view.SetBorder(context, borderElement.BorderColor, borderElement.BorderWidth);
		}

		public static void SetBorder(this MaterialCardView view, Context context, Color color, double width)
		{
			view.StrokeColor = color == Color.White ? new global::Android.Graphics.Color(254, 254, 254) : color.ToAndroid();
			view.StrokeWidth = (int)context.ToPixels(width);
		}

		public static void SetBorder(this AView view, Context context, VisualElement element, IBorderElement borderElement)
        {
            view.SetBorder(context, element, borderElement.BorderColor, borderElement.BorderWidth);
            view.SetDashedBorder(context, borderElement.DashWidth, borderElement.DashGap);
            view.SetBorderGradients(borderElement.BorderGradients, borderElement.BorderAngle);
		}

		public static void SetBorder(this AView view, Context context, VisualElement element, Color color, double width)
		{
            view.GetGradientDrawable().SetStroke((int)context.ToPixels(width), color);
        }

        public static void SetBorderGradients(this AView view, IList<GradientStop> gradients, float angle)
        {
            view.GetGradientDrawable().SetBorderGradient(gradients, angle);
        }

        public static void SetDashedBorder(this AView view, Context context, double dashWidth, double dashGap)
        {
            view.GetGradientDrawable().SetDashedBorder((int)context.ToPixels(dashWidth), (int)context.ToPixels(dashGap));
        }

        #endregion

        #region Corner Radius

        public static void SetCornerRadius(this AView view, Context context, VisualElement element, ICornerElement cornerElement, Color? color = null)
		{
            view.SetCornerRadius(context, element, cornerElement.CornerRadius, color);
        }

		public static void SetCornerRadius(this MaterialCardView view, Context context, ICornerElement cornerElement)
		{
			view.Radius = context.ToPixels(cornerElement.CornerRadius.TopLeft);
		}

		public static void SetCornerRadius(this Chip view, Context context, ICornerElement cornerElement)
		{
			view.ChipCornerRadius = context.ToPixels(cornerElement.CornerRadius.TopLeft);
		}

		public static void SetCornerRadius(this AView view, Context context, VisualElement element, CornerRadius cornerRadius, Color? color)
		{
            if (view == null || cornerRadius == new CornerRadius(0d)) return;

            var isUniform = cornerRadius.IsAllRadius() && !cornerRadius.IsEmpty();

            var uniformCornerRadius = context.ToPixels(cornerRadius.TopLeft);
            var cornerRadii = cornerRadius.ToRadii(context.Resources.DisplayMetrics.Density);

            var gradientDrawable = view.GetGradientDrawable();

            if (isUniform) gradientDrawable.SetCornerRadius(uniformCornerRadius);
            else gradientDrawable.SetCornerRadii(cornerRadii);
        }

		#endregion

		#region Gradient

		public static void SetGradient(this AView view, IGradientElement gradientElement)
		{
			view.SetGradient(gradientElement.Gradients, gradientElement.Angle);
		}

        private static void SetGradient(this AView view, IList<GradientStop> gradients, float angle)
        {
            view.GetGradientDrawable().SetGradient(gradients, angle);
        }

		#endregion

		#region Elevation

		public static void SetElevation(this MaterialCardView view, Context context, IElevationElement elevationElement)
		{
			view.SetElevation(context, elevationElement.Elevation);
		}

		public static void SetElevation(this MaterialCardView view, Context context, double elevation)
		{
			if (view == null) return;

			view.CardElevation = context.ToPixels(elevation);
		}

		public static void SetElevation(this AView view, Context context, IElevationElement elevationElement)
		{
			view.SetElevation(context, elevationElement.Elevation);
		}

		public static void SetElevation(this AView view, Context context, double elevation)
		{
			if (view == null) return;

			view.Elevation = context.ToPixels(elevation);
		}

        #endregion

        #region TranslationZ

        public static void SetTranslationZ(this AView view, Context context, IElevationElement elevationElement)
        {
            view.SetTranslationZ(context, elevationElement.TranslationZ);
        }

        public static void SetTranslationZ(this AView view, Context context, double translationZ)
        {
            if (view == null) return;

            view.TranslationZ = context.ToPixels(translationZ);
        }

        #endregion

        #region Rendering

        public static ViewGroup FindViewGroupParent(this AView view)
		{
			var parent = view.Parent;
			while (parent != null)
			{
				if (parent is ViewGroup viewGroup) return viewGroup;

				parent = parent.Parent;
			}

			return null;
		}

		public static IVisualElementRenderer GetOrCreateRenderer(this XView view, Context context) =>
			 GetRenderer(view, context);

		public static IVisualElementRenderer GetRenderer(this XView view, Context context)
		{
			/* Create the Native Renderer if not initialized */
			if (Platform.GetRenderer(view) == null || Platform.GetRenderer(view)?.Tracker == null)
			{
				var ctxRenderer = Platform.CreateRendererWithContext(view, context);
				Platform.SetRenderer(view, ctxRenderer);
			}

			/* Render the X.F. View */
			return Platform.GetRenderer(view);
		}

		#endregion

        private static GradientStrokeDrawable GetGradientDrawable(this AView view)
        {
            var constructNew = true;
            GradientStrokeDrawable gradientStrokeDrawable;

            if (view.Background is GradientStrokeDrawable oldGradientStrokeDrawable)
            {
                constructNew = false;
                gradientStrokeDrawable = oldGradientStrokeDrawable;
            }
            else
            {
                gradientStrokeDrawable = new GradientStrokeDrawable { Shape = new RectShape() };
            }

            if (constructNew)
            {
                view.Background?.Dispose();
                view.Background = gradientStrokeDrawable;
            }

            return gradientStrokeDrawable;
        }
	}
}
