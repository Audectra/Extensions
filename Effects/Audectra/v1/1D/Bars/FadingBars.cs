/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(8)]
    [LandscapeAspectRatio]
    [EffectExtension("Fading Bars", "Audectra", "v0.0.1")]
    class FadingBars : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private struct BarItem
        {
            public float Value;
            public float MaxLife;
            public SKColor Color;
        }

        private int _maxNumBars;
        private float _maxBarLife;
        private int _numBars;
        private int _barSize;
        private int _lastBar;
        private BarItem[] _bars;
        private Random _rand;

        private readonly object _barsLock = new object();

        private enum SettingId
        {
            Color = 0,
            NumBars,
            MaxBarLife,
        }

        private enum TriggerId
        {
            BarBoom = 0,
        }

        public FadingBars(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _maxNumBars = Math.Min((int)(width / 2), 12);
            _numBars = 4;
            _maxBarLife = 1.0f;
            _barSize = (int)(width / _numBars);
            _lastBar = -1;
            _rand = new Random();

            InitBars();
        }

        private void InitBars()
        {
            _bars = new BarItem[_numBars];

            for (int i = 0; i < _numBars; i++)
                _bars[i].Color = new SKColor(0, 0, 0);
        }

        public override IRender Render(float dt)
        {
            lock (_barsLock)
            {
                UpdateBars(dt);

                using (var canvas = _render.CreateCanvas())
                {
                    canvas.Clear();

                    for (int i = 0; i < _numBars; i++)
                    {
                        int x0 = i * _barSize;
                        float intensity = 0;
                        
                        if (_bars[i].MaxLife > 0)
                            intensity = _bars[i].Value / _bars[i].MaxLife;

                        var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Color = _bars[i].Color.WithScale(intensity),
                            Style = SKPaintStyle.Fill
                        };

                        canvas.DrawRect(x0, 0, _barSize, Height, paint);
                        paint.Dispose();
                    }
                }
            }

            return _render;
        }

        private void UpdateBars(float dt)
        {
            for (int i = 0; i < _numBars; i++)
            {
                _bars[i].Value -= dt;

                if (_bars[i].Value < 0)
                    _bars[i].Value = 0;
            }
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Num Bars");
            settingsBuilder.AddSlider(_numBars, 2, _maxNumBars, (uint)SettingId.NumBars);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Bar Life");
            settingsBuilder.AddBindableSlider(_maxBarLife, 0.1f, 2.5f, (uint)SettingId.MaxBarLife);
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
                case SettingId.Color:
                    _color = value;
                    break;

                case SettingId.NumBars:
                    lock (_barsLock)
                    {
                        _numBars = value;
                        _barSize = (int)(Width / _numBars);
                        _lastBar = -1;
                        InitBars();
                    }
                    break;

                case SettingId.MaxBarLife:
                    lock (_barsLock)
                    {
                        _maxBarLife = value;
                    }
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
                    int idxBar = GetNextBar();

                    lock (_barsLock)
                    {
                        _bars[idxBar].Value = _maxBarLife;
                        _bars[idxBar].MaxLife = _maxBarLife;
                        _bars[idxBar].Color = _color;
                    }
                }
                break;
            }
        }

        private int GetNextBar()
        {
            int nextBar;

            do
            {
                nextBar = _rand.Next(_numBars);
            } while (nextBar == _lastBar);

            _lastBar = nextBar;
            return nextBar;
        }
    }
}
