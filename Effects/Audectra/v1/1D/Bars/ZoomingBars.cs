/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Collections.Generic;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(4)]
    [LandscapeAspectRatio]
    [EffectExtension("Zooming Bars", "Audectra", "v0.0.1")]
    class ZoomingBars : EffectExtensionBase
    {
        private IEffectApi _api;
        private IRender _render;

        private class BarItem
        {
            public float Position;
            public float Life;
            public float MaxLife;
            public float MaxSize;
            public SKColor Color;
        }

        private float _maxBarLife;
        private float _maxBarSize;
        private IList<BarItem> _bars;
        private Random _rand;

        private readonly object _barsLock = new object();

        private enum SettingId
        {
            BarSize,
            BarLife,
        }

        private enum TriggerId
        {
            BarBoom = 0,
        }

        public ZoomingBars(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _render = _api.CreateRender();

            _maxBarLife = 0.5f;
            _maxBarSize = width / 8f;
            _rand = new Random();
            _bars = new List<BarItem>();
        }

        public override IRender Render(float dt)
        {
            UpdateBars(dt);

            return RenderBars();
        }

        private void UpdateBars(float dt)
        {
            lock (_barsLock) {
                foreach (var bar in _bars.ToArray())
                {
                    bar.Life -= dt;

                    if (bar.Life < 0)
                        _bars.Remove(bar);
                }
            }
        }

        private IRender RenderBars()
        {
            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();

                lock (_barsLock)
                {
                    foreach (var bar in _bars)
                    {
                        float scale = bar.Life / bar.MaxLife;
                        float size = (1 - scale) * bar.MaxSize;
                        float x = bar.Position - size / 2f;

                        var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Color = bar.Color.WithScale(scale),
                            Style = SKPaintStyle.Fill
                        };

                        canvas.DrawRect(x, 0, size, Height, paint);
                        paint.Dispose();
                    }
                }
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Bar Size");
            settingsBuilder.AddBindableSlider(_maxBarSize, 0, Width / 2f, (uint)SettingId.BarSize);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Bar Life");
            settingsBuilder.AddBindableSlider(_maxBarLife, 0.1f, 1.0f, (uint)SettingId.BarLife);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.BarBoom);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.BarSize:
                    _maxBarSize = value;
                    break;

                case SettingId.BarLife:
                    _maxBarLife = value;
                    break;
            }
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            switch ((TriggerId) triggerId)
            {
                case TriggerId.BarBoom:
                {
                    var bar = CreateBar();

                    if (bar.MaxLife > 0 &&  bar.MaxSize > 0)
                        lock (_barsLock)
                        {
                            _bars.Add(bar);
                        }
                }
                break;
            }
        }

        private BarItem CreateBar()
        {
            return new BarItem
            {
                Position = (float)_rand.NextDouble() * Width,
                Life = _maxBarLife,
                MaxLife = _maxBarLife,
                MaxSize = _maxBarSize,
                Color = _api.CreateRandomColor(),
            };
        }
    }
}
