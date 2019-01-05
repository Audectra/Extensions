/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;


namespace Audectra.Extensions.Effects
{
    class Stroboscope : EffectBase, IExtension
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

        public Stroboscope() { }

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
            settingsBuilder.AddBindableTrigger(this, (uint)TriggerId.Flash);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, object value)
        {
            
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

        public string GetName()
        {
            return "Stroboscope";
        }

        public string GetVersion()
        {
            return "v1.1.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
