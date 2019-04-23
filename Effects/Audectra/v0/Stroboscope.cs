/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [EffectExtension("Stroboscope", "Audectra", "1.3.0")]
    class Stroboscope : EffectExtensionBase
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _flashDuration;

        private const float MaxFlashDuration = 0.1f;

        private enum TriggerId
        {
            Flash,
        }

        public Stroboscope(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0, 0);
            _render = _helper.CreateRender();

            _flashDuration = MaxFlashDuration;
        }

        public override IRgbRender Render(float dt)
        {
            if (_flashDuration > MaxFlashDuration)
                _color = new RgbColor(0, 0, 0);
            else
            {
                _flashDuration += dt;
                _color = _color * (1f - _flashDuration / MaxFlashDuration);
            }

            _render.Map((x, y) => _color);
            return _render;
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
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
                        _color = new RgbColor(1, 1, 1);
                        _flashDuration = 0;
                    }
                    break;
            }
        }
    }
}
