/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [EffectExtension("Stroboscope", "Audectra", "1.3.0")]
    class Stroboscope : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _flashDuration;

        private const float MaxFlashDuration = 0.1f;

        private enum TriggerId
        {
            Flash,
        }

        public Stroboscope(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _color = new SKColor(0, 0, 0);
            _render = _api.CreateRender();

            _flashDuration = MaxFlashDuration;
        }

        public override IRender Render(float dt)
        {
            if (_flashDuration > MaxFlashDuration)
                _color = new SKColor(0, 0, 0);
            else
            {
                _flashDuration += dt;
                _color = _color.WithScale(1f - _flashDuration / MaxFlashDuration);
            }

            using (var canvas = _api.CreateCanvas(_render))
                canvas.Clear(_color);
                
            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.Flash);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            switch ((TriggerId) triggerId)
            {
                case TriggerId.Flash:
                    if (risingEdge)
                    {
                        _color = new SKColor(255, 255, 255);
                        _flashDuration = 0;
                    }
                    break;
            }
        }
    }
}
