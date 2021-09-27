/*
 * Copyright(c) 2020 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;

namespace Tizen.NUI.Components
{
    // Represents padding data : Start, End, Top, Bottom
    using PaddingType = ValueTuple<ushort, ushort, ushort, ushort>;

    /// <summary>
    /// The Scrollbar is a component that contains track and thumb to indicate the current scrolled position of a scrollable object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Scrollbar : ScrollbarBase
    {
        #region Fields

        /// <summary>Bindable property of TrackThickness</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly BindableProperty TrackThicknessProperty = BindableProperty.Create(nameof(TrackThickness), typeof(float), typeof(Scrollbar), default(float), propertyChanged: (bindable, oldValue, newValue) =>
        {
            var instance = ((Scrollbar)bindable);
            var thickness = (float?)newValue;

            instance.scrollbarStyle.TrackThickness = thickness;
            instance.UpdateTrackThickness(thickness ?? 0);
        },
        defaultValueCreator: (bindable) =>
        {
            return ((Scrollbar)bindable).scrollbarStyle.TrackThickness ?? 0;
        });

        /// <summary>Bindable property of ThumbThickness</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly BindableProperty ThumbThicknessProperty = BindableProperty.Create(nameof(ThumbThickness), typeof(float), typeof(Scrollbar), default(float), propertyChanged: (bindable, oldValue, newValue) =>
        {
            var instance = ((Scrollbar)bindable);
            var thickness = (float?)newValue;

            instance.scrollbarStyle.ThumbThickness = thickness;
            instance.UpdateThumbThickness(thickness ?? 0);
        },
        defaultValueCreator: (bindable) =>
        {
            return ((Scrollbar)bindable).scrollbarStyle.ThumbThickness ?? 0;
        });

        /// <summary>Bindable property of TrackColor</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(Scrollbar), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var instance = ((Scrollbar)bindable);
            var color = (Color)newValue;

            instance.scrollbarStyle.TrackColor = color;
            instance.UpdateTrackColor(color);
        },
        defaultValueCreator: (bindable) =>
        {
            return ((Scrollbar)bindable).scrollbarStyle.TrackColor;
        });

        /// <summary>Bindable property of ThumbColor</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(Scrollbar), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var instance = ((Scrollbar)bindable);
            var color = (Color)newValue;

            instance.scrollbarStyle.ThumbColor = color;
            instance.UpdateThumbColor(color);
        },
        defaultValueCreator: (bindable) =>
        {
            return ((Scrollbar)bindable).scrollbarStyle.ThumbColor;
        });

        /// <summary>Bindable property of TrackPadding</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly BindableProperty TrackPaddingProperty = BindableProperty.Create(nameof(TrackPadding), typeof(Extents), typeof(Scrollbar), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var instance = ((Scrollbar)bindable);
            var trackPadding = (Extents)newValue;

            instance.scrollbarStyle.TrackPadding = trackPadding;
            instance.UpdateTrackPadding(trackPadding);
        },
        defaultValueCreator: (bindable) =>
        {
            return ((Scrollbar)bindable).scrollbarStyle.TrackPadding;
        });

        private View trackView;
        private View thumbView;
        private Animation thumbPositionAnimation;
        private Animation thumbSizeAnimation;
        private Animation opacityAnimation;
        private Calculator calculator;
        private Size containerSize = new Size(0, 0);
        private ScrollbarStyle scrollbarStyle => ViewStyle as ScrollbarStyle;
        private bool mScrollEnabled = true;
        private float previousPosition;

        #endregion Fields


        #region Constructors

        /// <summary>
        /// Create an empty Scrollbar.
        /// </summary>
        public Scrollbar() : base()
        {
        }

        /// <summary>
        /// Create a Scrollbar and initialize with properties.
        /// </summary>
        /// <param name="contentLength">The length of the scrollable content area.</param>
        /// <param name="viewportLength">The length of the viewport representing the amount of visible content.</param>
        /// <param name="currentPosition">The current position of the viewport in scrollable content area. This is the viewport's top position if the scroller is vertical, otherwise, left.</param>
        /// <param name="isHorizontal">Whether the direction of scrolling is horizontal or not. It is vertical by default.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Scrollbar(float contentLength, float viewportLength, float currentPosition, bool isHorizontal = false) : this()
        {
            Initialize(contentLength, viewportLength, currentPosition, isHorizontal);
        }

        /// <summary>
        /// Create an empty Scrollbar with a ScrollbarStyle instance to set style properties.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Scrollbar(ScrollbarStyle style) : base(style)
        {
        }

        /// <summary>
        /// Static constructor to initialize bindable properties when loading.
        /// </summary>
        static Scrollbar()
        {
        }

        #endregion Constructors


        #region Properties

        /// <summary>
        /// Return a copied Style instance of Scrollbar
        /// </summary>
        /// <remarks>
        /// It returns copied Style instance and changing it does not effect to the Scrollbar.
        /// Style setting is possible by using constructor or the function of ApplyStyle(ViewStyle viewStyle)
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ScrollbarStyle Style
        {
            get
            {
                var result = new ScrollbarStyle(scrollbarStyle);
                result.CopyPropertiesFromView(this);
                return result;
            }
        }

        /// <summary>
        /// The thickness of the track.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float TrackThickness
        {
            get => (float)GetValue(TrackThicknessProperty);
            set => SetValue(TrackThicknessProperty, value);
        }

        /// <summary>
        /// The thickness of the thumb.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float ThumbThickness
        {
            get => (float)GetValue(ThumbThicknessProperty);
            set => SetValue(ThumbThicknessProperty, value);
        }

        /// <summary>
        /// The color of the track part.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color TrackColor
        {
            get => (Color)GetValue(TrackColorProperty);
            set => SetValue(TrackColorProperty, value);
        }

        /// <summary>
        /// The color of the thumb part.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        /// <summary>
        /// The padding value of the track.
        /// Note that when the scrollbar is for vertical direction, Start value is ignored.
        /// In case of horizontal direction, Top value is ignored.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Extents TrackPadding
        {
            get => (Extents)GetValue(TrackPaddingProperty);
            set => SetValue(TrackPaddingProperty, value);
        }

        #endregion Properties


        #region Methods

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Initialize(float contentLength, float viewportLength, float currentPosition, bool isHorizontal = false)
        {
            if (isHorizontal)
            {
                calculator = new HorizontalCalculator(contentLength > 0.0f ? contentLength : 0.0f, viewportLength, currentPosition);
            }
            else
            {
                calculator = new VerticalCalculator(contentLength > 0.0f ? contentLength : 0.0f, viewportLength, currentPosition);
            }

            thumbPositionAnimation?.Stop();
            thumbPositionAnimation = null;

            thumbSizeAnimation?.Stop();
            thumbSizeAnimation = null;

            opacityAnimation?.Stop();
            opacityAnimation = null;

            PaddingType ensuredPadding = EnsurePadding(TrackPadding);
            var trackSize = calculator.CalculateTrackSize(TrackThickness, containerSize, ensuredPadding);
            var trackPosition = calculator.CalculateTrackPosition(ensuredPadding);
            var thumbSize = calculator.CalculateThumbSize(ThumbThickness, trackSize);
            var thumbPosition = calculator.CalculateThumbPosition(trackSize, thumbSize, ensuredPadding);

            trackView?.Unparent();
            trackView = new View()
            {
                BackgroundColor = TrackColor,
                ParentOrigin = calculator.CalculatorTrackAlign(),
                PivotPoint = calculator.CalculatorTrackAlign(),
                Size = trackSize,
                Position = trackPosition,
                PositionUsesPivotPoint = true,
            };
            Add(trackView);

            thumbView?.Unparent();
            thumbView = new View()
            {
                BackgroundColor = ThumbColor,
                ParentOrigin = calculator.CalculatorThumbAlign(),
                PivotPoint = calculator.CalculatorThumbAlign(),
                Size = thumbSize,
                Position = thumbPosition,
                PositionUsesPivotPoint = true,
            };
            Add(thumbView);

            Opacity = calculator.IsScrollable() ? 1.0f : 0.0f;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Thrown when the scrollabr not yet initialized.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Update(float contentLength, float viewportLength, float position, uint durationMs = 0, AlphaFunction alphaFunction = null)
        {
            if (calculator == null)
            {
                throw new InvalidOperationException("Scrollbar is not initialized. Please call Initialize() first.");
            }

            calculator.visibleLength = viewportLength;
            Update(contentLength, position, durationMs, alphaFunction);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Thrown when the scrollabr not yet initialized.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Update(float contentLength, float position, uint durationMs = 0, AlphaFunction alphaFunction = null)
        {
            if (calculator == null)
            {
                throw new InvalidOperationException("Scrollbar is not initialized. Please call Initialize() first.");
            }

            calculator.contentLength = contentLength > 0.0f ? contentLength : 0.0f;
            calculator.currentPosition = position;

            float newOpacity = calculator.IsScrollable() ? 1.0f : 0.0f;
            bool opacityChanged = (int)Opacity != (int)newOpacity;

            var thumbSize = calculator.CalculateThumbSize(ThumbThickness, trackView.Size);
            var thumbPosition = calculator.CalculateThumbScrollPosition(trackView.Size, thumbView.Position, EnsurePadding(TrackPadding));

            if (durationMs == 0)
            {
                thumbView.Position = thumbPosition;
                thumbView.Size = thumbSize;

                if (opacityChanged)
                {
                    Opacity = newOpacity;
                }
                return;
            }

            EnsureThumbPositionAnimation().AnimateTo(thumbView, "Position", thumbPosition, 0, (int)durationMs, alphaFunction);
            thumbPositionAnimation.Play();

            EnsureThumbSizeAnimation().AnimateTo(thumbView, "Size", thumbSize, 0, (int)durationMs, alphaFunction);
            thumbSizeAnimation.Play();

            if (opacityChanged)
            {
                EnsureOpacityAnimation().AnimateTo(this, "Opacity", newOpacity, 0, (int)durationMs, alphaFunction);
                opacityAnimation.Play();
            }
        }

        /// <inheritdoc/>
        /// <remarks>Please note that, for now, only alpha functions created with BuiltinFunctions are valid when animating. Otherwise, it will be treated as a linear alpha function. </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ScrollTo(float position, uint durationMs = 0, AlphaFunction alphaFunction = null)
        {
            if (mScrollEnabled == false)
            {
                return;
            }

            if (calculator == null)
            {
                return;
            }

            previousPosition = calculator.currentPosition;
            calculator.currentPosition = position;
            var thumbPosition = calculator.CalculateThumbScrollPosition(trackView.Size, thumbView.Position, EnsurePadding(TrackPadding));

            if (durationMs == 0)
            {
                thumbView.Position = thumbPosition;
                return;
            }

            EnsureThumbPositionAnimation().AnimateTo(thumbView, "position", thumbPosition, 0, (int)durationMs, alphaFunction);
            thumbPositionAnimation.Play();
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnRelayout(Vector2 size, RelayoutContainer container)
        {
            base.OnRelayout(size, container);

            if (size.Width == containerSize.Width && size.Height == containerSize.Height)
            {
                return;
            }

            containerSize = new Size(size.Width, size.Height);

            if (calculator == null)
            {
                return;
            }

            PaddingType ensuredPadding = EnsurePadding(TrackPadding);
            trackView.Size = calculator.CalculateTrackSize(TrackThickness, containerSize, ensuredPadding);
            trackView.Position = calculator.CalculateTrackPosition(ensuredPadding);
            thumbView.Size = calculator.CalculateThumbSize(ThumbThickness, trackView.Size);
            thumbView.Position = calculator.CalculateThumbPosition(trackView.Size, thumbView.Size, ensuredPadding);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ApplyStyle(ViewStyle viewStyle)
        {
            if (viewStyle is ScrollbarStyle scrollbarStyle)
            {
                // Apply essential look.
                if (scrollbarStyle.TrackThickness == null) scrollbarStyle.TrackThickness = 6.0f;
                if (scrollbarStyle.ThumbThickness == null) scrollbarStyle.ThumbThickness = 6.0f;
                if (scrollbarStyle.TrackColor == null) scrollbarStyle.TrackColor = new Color(1.0f, 1.0f, 1.0f, 0.15f);
                if (scrollbarStyle.ThumbColor == null) scrollbarStyle.ThumbColor = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                if (scrollbarStyle.TrackPadding == null) scrollbarStyle.TrackPadding = 4;
                if (scrollbarStyle.WidthResizePolicy == null) scrollbarStyle.WidthResizePolicy = ResizePolicyType.FillToParent;
                if (scrollbarStyle.HeightResizePolicy == null) scrollbarStyle.HeightResizePolicy = ResizePolicyType.FillToParent;
            }

            base.ApplyStyle(viewStyle);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override ViewStyle CreateViewStyle()
        {
            return new ScrollbarStyle();
        }

        /// <summary>
        /// Update TrackThickness property of the scrollbar.
        /// </summary>
        /// <param name="thickness">The width of the track.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void UpdateTrackThickness(float thickness)
        {
            if (trackView == null)
            {
                return;
            }

            trackView.Size = calculator.CalculateTrackSize(thickness, containerSize, EnsurePadding(TrackPadding));
        }

        /// <summary>
        /// Update ThumbThickness property of the scrollbar.
        /// </summary>
        /// <param name="thickness">The width of the track.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void UpdateThumbThickness(float thickness)
        {
            if (thumbView == null)
            {
                return;
            }

            thumbView.Size = calculator.CalculateThumbSize(thickness, trackView.Size);
        }

        /// <summary>
        /// Update TrackColor property of the scrollbar.
        /// </summary>
        /// <param name="color">The color of the track.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void UpdateTrackColor(Color color)
        {
            if (trackView == null)
            {
                return;
            }

            trackView.BackgroundColor = color;
        }

        /// <summary>
        /// Update ThumbColor property of the scrollbar.
        /// </summary>
        /// <param name="color">The color of the thumb.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void UpdateThumbColor(Color color)
        {
            if (thumbView == null)
            {
                return;
            }

            thumbView.BackgroundColor = color;
        }

        /// <summary>
        /// Update TrackPadding property of the scrollbar.
        /// </summary>
        /// <param name="trackPadding">The padding of the track.</param>
        protected virtual void UpdateTrackPadding(Extents trackPadding)
        {
            if (calculator == null || trackView == null || thumbView == null)
            {
                return;
            }

            PaddingType ensuredPadding = EnsurePadding(trackPadding);
            trackView.Size = calculator.CalculateTrackSize(TrackThickness, containerSize, ensuredPadding);
            trackView.Position = calculator.CalculateTrackPosition(ensuredPadding);
            thumbView.Size = calculator.CalculateThumbSize(ThumbThickness, trackView.Size);
            thumbView.Position = calculator.CalculateThumbPaddingPosition(trackView.Size, thumbView.Size, thumbView.Position, ensuredPadding);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool ScrollEnabled
        {
            get
            {
                return mScrollEnabled;
            }
            set
            {
                if (value != mScrollEnabled)
                {
                    mScrollEnabled = value;
                }
            }
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Position ScrollPosition
        {
            get
            {
                if (calculator == null)
                {
                    return new Position(0.0f, 0.0f);
                }

                float length = Math.Min(Math.Max(calculator.currentPosition, 0.0f), calculator.contentLength - calculator.visibleLength);

                if (calculator is HorizontalCalculator)
                {
                    return new Position(length, 0.0f);
                }
                else
                {
                    return new Position(0.0f, length);
                }
            }
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Position ScrollCurrentPosition
        {
            get
            {
                if (calculator == null)
                {
                    return new Position(0.0f, 0.0f);
                }

                float length = Math.Min(Math.Max(calculator.currentPosition, 0.0f), calculator.contentLength - calculator.visibleLength);

                if (thumbPositionAnimation != null)
                {
                    float progress = thumbPositionAnimation.CurrentProgress;
                    float previousLength = Math.Min(Math.Max(previousPosition, 0.0f), calculator.contentLength - calculator.visibleLength);

                    length = ((1.0f - progress) * previousLength) + (progress * length);
                }

                if (calculator is HorizontalCalculator)
                {
                    return new Position(length, 0.0f);
                }
                else
                {
                    return new Position(0.0f, length);
                }
            }
        }

        private PaddingType EnsurePadding(Extents padding) => padding == null ? new PaddingType(0, 0, 0 ,0) : new PaddingType(padding.Start, padding.End, padding.Top, padding.Bottom);

        private Animation EnsureThumbPositionAnimation()
        {
            if (thumbPositionAnimation == null)
            {
                thumbPositionAnimation = new Animation();
            }
            else
            {
                thumbPositionAnimation.Stop();
                thumbPositionAnimation.Clear();
            }
            return thumbPositionAnimation;
        }

        private Animation EnsureThumbSizeAnimation()
        {
            if (thumbSizeAnimation == null)
            {
                thumbSizeAnimation = new Animation();
            }
            else
            {
                thumbSizeAnimation.Stop();
                thumbSizeAnimation.Clear();
            }
            return thumbSizeAnimation;
        }

        private Animation EnsureOpacityAnimation()
        {
            if (opacityAnimation == null)
            {
                opacityAnimation = new Animation();
            }
            else
            {
                opacityAnimation.Stop();
                opacityAnimation.Clear();
            }
            return opacityAnimation;
        }

        #endregion Methods


        #region Classes

        private abstract class Calculator
        {
            public float contentLength;
            public float visibleLength;
            public float currentPosition;

            public Calculator(float contentLength, float visibleLength, float currentPosition)
            {
                this.contentLength = contentLength;
                this.visibleLength = visibleLength;
                this.currentPosition = currentPosition;
            }

            public bool IsScrollable()
            {
                return contentLength > visibleLength;
            }

            public abstract Position CalculatorTrackAlign();
            public abstract Position CalculatorThumbAlign();
            public abstract Size CalculateTrackSize(float thickness, Size containerSize, PaddingType trackPadding);
            public abstract Position CalculateTrackPosition(PaddingType trackPadding);
            public abstract Size CalculateThumbSize(float thickness, Size trackSize);
            public abstract Position CalculateThumbPosition(Size trackSize, Size thumbSize, PaddingType trackPadding);
            public abstract Position CalculateThumbPaddingPosition(Size trackSize, Size thumbSize, Position thumbCurrentPosition, PaddingType trackPadding);
            public abstract Position CalculateThumbScrollPosition(Size trackSize, Position thumbCurrentPosition, PaddingType trackPadding);
        }

        private class HorizontalCalculator : Calculator
        {
            public HorizontalCalculator(float contentLength, float visibleLength, float currentPosition) : base(contentLength, visibleLength, currentPosition)
            {
            }

            public override Position CalculatorTrackAlign()
            {
                return Tizen.NUI.ParentOrigin.BottomLeft;
            }

            public override Position CalculatorThumbAlign()
            {
                return Tizen.NUI.ParentOrigin.BottomLeft;
            }

            public override Size CalculateTrackSize(float thickness, Size containerSize, PaddingType trackPadding)
            {
                return new Size(containerSize.Width - trackPadding.Item1 - trackPadding.Item2, thickness);
            }

            public override Position CalculateTrackPosition(PaddingType trackPadding)
            {
                return new Position(trackPadding.Item1, -trackPadding.Item4);
            }

            public override Size CalculateThumbSize(float thickness, Size trackSize)
            {
                return new Size(trackSize.Width * (IsScrollable() ? (visibleLength / contentLength) : 0.0f), thickness);
            }

            public override Position CalculateThumbPosition(Size trackSize, Size thumbSize, PaddingType trackPadding)
            {
                float padding = ((trackSize.Height - thumbSize.Height) / 2.0f) + trackPadding.Item4;
                return new Position(trackPadding.Item1 + (IsScrollable() ? (trackSize.Width * (Math.Min(Math.Max(currentPosition, 0.0f), contentLength - visibleLength)) / contentLength) : 0.0f), -padding);
            }

            public override Position CalculateThumbPaddingPosition(Size trackSize, Size thumbSize, Position thumbCurrentPosition, PaddingType trackPadding)
            {
                float padding = ((trackSize.Height - thumbSize.Height) / 2.0f) + trackPadding.Item4;
                return new Position(thumbCurrentPosition.X, -padding);
            }

            public override Position CalculateThumbScrollPosition(Size trackSize, Position thumbCurrentPosition, PaddingType trackPadding)
            { 
                return new Position(trackPadding.Item1 + (IsScrollable() ? (trackSize.Width * (Math.Min(Math.Max(currentPosition, 0.0f), contentLength - visibleLength)) / contentLength) : 0.0f), thumbCurrentPosition.Y);
            }
        }

        private class VerticalCalculator : Calculator
        {
            public VerticalCalculator(float contentLength, float visibleLength, float currentPosition) : base(contentLength, visibleLength, currentPosition)
            {
            }

            public override Position CalculatorTrackAlign()
            {
                return Tizen.NUI.ParentOrigin.TopRight;
            }

            public override Position CalculatorThumbAlign()
            {
                return Tizen.NUI.ParentOrigin.TopRight;
            }

            public override Size CalculateTrackSize(float thickness, Size containerSize, PaddingType trackPadding)
            {
                return new Size(thickness, containerSize.Height - trackPadding.Item3 - trackPadding.Item4);
            }

            public override Position CalculateTrackPosition(PaddingType trackPadding)
            {
                return new Position(-trackPadding.Item2, trackPadding.Item3);
            }

            public override Size CalculateThumbSize(float thickness, Size trackSize)
            {
                return new Size(thickness, trackSize.Height * (IsScrollable() ? (visibleLength / contentLength) : 0.0f));
            }

            public override Position CalculateThumbPosition(Size trackSize, Size thumbSize, PaddingType trackPadding)
            {
                float padding = ((trackSize.Width - thumbSize.Width) / 2.0f) + trackPadding.Item2;
                return new Position(-padding, trackPadding.Item3 + (IsScrollable() ? (trackSize.Height * Math.Min(Math.Max(currentPosition, 0.0f), contentLength - visibleLength) / contentLength) : 0.0f));
            }

            public override Position CalculateThumbPaddingPosition(Size trackSize, Size thumbSize, Position thumbCurrentPosition, PaddingType trackPadding)
            {
                float padding = ((trackSize.Width - thumbSize.Width) / 2.0f) + trackPadding.Item2;
                return new Position(-padding, thumbCurrentPosition.Y);
            }

            public override Position CalculateThumbScrollPosition(Size trackSize, Position thumbPosition, PaddingType trackPadding)
            {
                return new Position(thumbPosition.X, trackPadding.Item3 + (IsScrollable() ? (trackSize.Height * Math.Min(Math.Max(currentPosition, 0.0f), contentLength - visibleLength) / contentLength) : 0.0f));
            }
        }

        #endregion Classes
    }
}