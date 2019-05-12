/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(12)]
    [LandscapeAspectRatio()]
    [EffectExtension("Chroma Bars", "Audectra", "1.3.0")]
    class ChromaBars : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;
        private IAudioFeatureCache _featureCache;

        private const int NumBars = 12;
        private int _barSize;
        private float _threshold;

        private enum SettingId
        {
            Color = 0,
            ThresholdValue
        }

        public ChromaBars(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _color = new SKColor(0, 128, 128);
            _render = _api.CreateRender();
            _featureCache = _api.CreateAudioFeatureCache();

            _threshold = 0.0f;
            _barSize = (int)(width / NumBars);
        }

        public override IRender Render(float dt)
        {
            var chromas = _featureCache.GetChromas();

            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();

                for (int i = 0; i < NumBars; i++)
                {
                    if (chromas[i] >= _threshold)
                    {
                        var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Color = _color.WithScale(chromas[i]),
                            Style = SKPaintStyle.Fill
                        };

                        int x0 = i * _barSize;
                        canvas.DrawRect(x0, 0, _barSize, Height, paint);
                        paint.Dispose();
                    }
                }
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Threshold");
            settingsBuilder.AddSlider(_threshold, 0, 1, (uint)SettingId.ThresholdValue);
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

                case SettingId.ThresholdValue:
                    _threshold = value;
                    break;
            }
        }
    }
}
